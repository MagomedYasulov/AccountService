using AccountService.Domain.Data.Repositories;
using AccountService.Domain.Models;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.GetAccounts;

public class GetAccountsHandler(
    IAccountRepository accountRepository,
    IMapper mapper) : IRequestHandler<GetAccountsQuery, AccountDto[]>
{
    public async Task<AccountDto[]> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AccountFilter { OwnerId = request.OwnerId, Revoked = request.Revoked };
        var accounts = await accountRepository.GetAsync(filter);
        return mapper.Map<AccountDto[]>(accounts);
    }
}