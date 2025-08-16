using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Domain.Events;
using AccountService.Exceptions;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace AccountService.Features.Transactions.CreateTransaction;

public class CreateTransactionHandler(
    AppDbContext dbContext,
    ICurrencyService currencyService,
    IMapper mapper)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        if(!await currencyService.IsSupportedCurrency(request.CurrencyCode))
            throw new ServiceException("Not Supported Currency Type", $"Currency type {request.CurrencyCode} not supported", StatusCodes.Status409Conflict);

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken: cancellationToken);
        AccountValidation(request.AccountId, account, request, TransactionType.Debit);

        Account? counterpartyAccount = null;
        if(request.CounterpartyAccountId != null)
        {
            counterpartyAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.CounterpartyAccountId.Value, cancellationToken: cancellationToken);
            AccountValidation(request.CounterpartyAccountId.Value, counterpartyAccount, request, TransactionType.Credit);
        }

        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            if (request.Type == TransactionType.Debit)
            {
                account!.Balance -= request.Sum;
                if (counterpartyAccount != null)
                    counterpartyAccount.Balance += request.Sum;
            }
            else
            {
                account!.Balance += request.Sum;
                if (counterpartyAccount != null)
                    counterpartyAccount.Balance -= request.Sum;
            }

            var transaction = mapper.Map<Transaction>(request);
            transaction.TransferTime = DateTime.UtcNow;

            await dbContext.Transactions.AddAsync(transaction, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var outboxMessage = GetOutboxMessage(request, transaction.Id);
            await dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await dbTransaction.CommitAsync(cancellationToken);

            return mapper.Map<TransactionDto>(transaction);
        }
        catch (DbUpdateConcurrencyException)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw new ServiceException("Concurrency Exception", "Account was modified by another transaction", StatusCodes.Status409Conflict);
        }
        catch(InvalidOperationException)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw new ServiceException("Concurrency Exception", "Account was modified by another transaction", StatusCodes.Status409Conflict);
        }
        catch (Exception)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void AccountValidation(Guid accountId, Account? account, CreateTransactionCommand request, TransactionType operationType)
    {
        if (account == null)
            throw new ServiceException("Account Not Found", $"Account with id {accountId} not found", StatusCodes.Status404NotFound);

        if (account.Revoked)
            throw new ServiceException("Account Revoked", $"Account with id {accountId} is revoked", StatusCodes.Status404NotFound);

        var isDebit = request.Type == operationType;

        if (isDebit && account.Frozen)
            throw new ServiceException("Account Frozen", $"Accoutn with id {accountId} is frozen", StatusCodes.Status409Conflict);

        if (request.CurrencyCode != account.CurrencyCode)
            throw new ServiceException("Currency doesn't match", $"The currency of the account {accountId} and the transaction do not match", StatusCodes.Status409Conflict);

        if (isDebit && account.Balance < request.Sum)
            throw new ServiceException("Insufficient funds", "Insufficient funds for the operation", StatusCodes.Status409Conflict);
    }

    private static OutboxMessage GetOutboxMessage(CreateTransactionCommand request, Guid transactionId)
    {
        string eventType;
        string json;

        if (request.CounterpartyAccountId == null)
        {
            if (request.Type == TransactionType.Debit)
            {
                var moneyCredited = new MoneyDebited()
                {
                    AccountId = request.AccountId,
                    Amount = request.Sum,
                    Currency = request.CurrencyCode,
                    EventId = Guid.NewGuid(),
                    OccurredAt = DateTime.UtcNow,
                    OperationId = transactionId,
                    Reason = request.Description,
                };
                eventType = typeof(MoneyDebited).AssemblyQualifiedName!;
                json = JsonConvert.SerializeObject(moneyCredited);
            }
            else
            {
                var moneyCredited = new MoneyCredited()
                {
                    AccountId = request.AccountId,
                    Amount = request.Sum,
                    Currency = request.CurrencyCode,
                    EventId = Guid.NewGuid(),
                    OccurredAt = DateTime.UtcNow,
                    OperationId = transactionId,
                };
                eventType = typeof(MoneyCredited).AssemblyQualifiedName!;
                json = JsonConvert.SerializeObject(moneyCredited);
            }
        }
        else
        {
            var transferCompleted = new TransferCompleted()
            {
                Amount = request.Sum,
                Currency = request.CurrencyCode,
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                TransferId = transactionId,
                SourceAccountId = request.Type == TransactionType.Debit ? request.AccountId : request.CounterpartyAccountId.Value,
                DestinationAccountId = request.Type == TransactionType.Credit ? request.AccountId : request.CounterpartyAccountId.Value
            };
            eventType = typeof(TransferCompleted).AssemblyQualifiedName!;
            json = JsonConvert.SerializeObject(transferCompleted);
        }

        return new OutboxMessage() { Payload = json, EventType = eventType, OccurredAt = DateTime.UtcNow, RoutingKey = "money.*" };
    }
}