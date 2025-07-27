using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountHandler(
    IAccountRepository accountRepository,
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
            OpenedAt = DateTime.UtcNow
        };

        await accountRepository.CreateAsync(account);
        return mapper.Map<AccountDto>(account);
    }
}