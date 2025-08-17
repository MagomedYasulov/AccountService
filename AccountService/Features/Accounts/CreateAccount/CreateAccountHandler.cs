using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Domain.Events;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountHandler(
    AppDbContext dbContext,
    IClientService clientService,
    ICurrencyService currencyService,
    IMapper mapper)
    : IRequestHandler<CreateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!await clientService.VerifyClient(request.OwnerId))
            throw new ServiceException("Invalid owner", $"Client with id {request.OwnerId} not verified", StatusCodes.Status409Conflict);

        if(!await currencyService.IsSupportedCurrency(request.CurrencyCode))
            throw new ServiceException("Not Supported Currency Type", $"Currency type {request.CurrencyCode} not supported", StatusCodes.Status409Conflict);

        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var account = new Account
            {
                OwnerId = request.OwnerId,
                CurrencyCode = request.CurrencyCode,
                InterestRate = request.InterestRate,
                Type = request.Type!.Value,
                OpenedAt = DateTime.UtcNow,
                ClosedAt = request.Type == AccountType.Deposit ? request.ClosedAt : null
            };

            await dbContext.Accounts.AddAsync(account, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var accountOpened = new AccountOpened()
            {
                AccountId = account.Id,
                Currency = account.CurrencyCode,
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                OwnerId = account.OwnerId,
                Type = account.Type
            };
            var json = JsonConvert.SerializeObject(accountOpened);

            var outboxMessage = new OutboxMessage()
            {
                Payload = json,
                EventType = typeof(AccountOpened).AssemblyQualifiedName!,
                RoutingKey = "account.opened",
                OccurredAt = DateTime.UtcNow,     
            };

            await dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await dbTransaction.CommitAsync(cancellationToken);
            return mapper.Map<AccountDto>(account);
        }
        catch(Exception)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}