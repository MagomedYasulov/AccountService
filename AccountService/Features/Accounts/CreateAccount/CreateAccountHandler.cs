using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;

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
        return mapper.Map<AccountDto>(account);
    }
}