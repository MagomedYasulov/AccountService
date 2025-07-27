using AccountService.Domain.Data.Repositories;
using AccountService.Domain.Models;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.GetAccounts;

public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, AccountDto[]>
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;

    public GetAccountsHandler(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _mapper = mapper;
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto[]> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AccountFilter { OwnerId = request.OwnerId, Revoked = request.Revoked };
        var accounts = await _accountRepository.GetAsync(filter);
        return _mapper.Map<AccountDto[]>(accounts);
    }
}