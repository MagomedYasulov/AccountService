using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Exceptions;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        using var dbTransaction = dbContext.Database.BeginTransaction();

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

            await dbTransaction.CommitAsync(cancellationToken);

            return mapper.Map<TransactionDto>(transaction);
        }
        catch (DbUpdateConcurrencyException)
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

        if (request.CurrencyCode != account.CurrencyCode)
            throw new ServiceException("Currency doesn't match", $"The currency of the account {accountId} and the transaction do not match", StatusCodes.Status409Conflict);

        if (request.Type == operationType && account.Balance < request.Sum)
            throw new ServiceException("Insufficient funds", "Insufficient funds for the operation", StatusCodes.Status409Conflict);
    }
}