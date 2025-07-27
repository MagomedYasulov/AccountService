using AccountService.Domain.Data.Repositories;
using AccountService.Domain.Enums;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, AccountDto>
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountHandler(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _mapper = mapper;
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.Id);
        if (account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);
            
        if(account.Revoked)
            throw new ServiceException("Can`t modify account", "Revoked account can`t be modified", StatusCodes.Status404NotFound);

        if (account.Type == AccountType.Checking)
            throw new ServiceException("Can`t modify account", "Interest rate can`t be modified in checking account", StatusCodes.Status409Conflict);

        account.InterestRate = request.InterestRate;
        await _accountRepository.UpdateAsync(account);

        return _mapper.Map<AccountDto>(account);
    }
}