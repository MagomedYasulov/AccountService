using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly ICurrencyService _currencyService;
    private readonly IAccountRepository _accountRepository;
    private readonly IClientService _clientService;
    private readonly IMapper _mapper;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        IClientService clientService,
        ICurrencyService currencyService,
        IMapper mapper)
    {
        _currencyService = currencyService;
        _accountRepository = accountRepository;
        _clientService = clientService;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!await _clientService.VerifyClient(request.OwnerId))
            throw new ServiceException("Invalid owner", $"Client with id {request.OwnerId} not verified", StatusCodes.Status409Conflict);

        if(!await _currencyService.IsSupportedCurrency(request.CurrencyCode))
            throw new ServiceException("Not Supported Currency Type", $"Currency type {request.CurrencyCode} not supported", StatusCodes.Status409Conflict);

        var account = new Account
        {
            OwnerId = request.OwnerId,
            CurrencyCode = request.CurrencyCode,
            InterestRate = request.InterestRate,
            Type = request.Type!.Value,
            OpenedAt = DateTime.UtcNow
        };

        await _accountRepository.CreateAsync(account);
        return _mapper.Map<AccountDto>(account);
    }
}