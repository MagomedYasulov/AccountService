using AccountService.Domain.Data.Repositories;
using AccountService.Domain.Enums;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountHandler(
    IAccountRepository accountRepository,
    IMapper mapper) : IRequestHandler<UpdateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id);
        if (account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);
            
        if(account.Revoked)
            throw new ServiceException("Can`t modify account", "Revoked account can`t be modified", StatusCodes.Status404NotFound);

        if (account.Type == AccountType.Checking)
            throw new ServiceException("Can`t modify account", "Interest rate can`t be modified in checking account", StatusCodes.Status409Conflict);

        account.InterestRate = request.InterestRate;
        await accountRepository.UpdateAsync(account);

        return mapper.Map<AccountDto>(account);
    }
}