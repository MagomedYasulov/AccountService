using AccountService.Domain.Data.Entities;
using AccountService.Domain.Models;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AccountService.Features.Accounts.GetAccounts;

public class GetAccountsHandler(
    AppDbContext dbContext,
    IMapper mapper) : IRequestHandler<GetAccountsQuery, AccountDto[]>
{
    public async Task<AccountDto[]> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AccountFilter { OwnerId = request.OwnerId, Revoked = request.Revoked };

        var isOwnerIdNull = filter.OwnerId == null;
        var isRevokedNull = filter.Revoked == null;

        Expression<Func<Account, bool>> predicate = (account) => (isOwnerIdNull || account.OwnerId == filter.OwnerId) &&
                                                                 (isRevokedNull || account.Revoked == filter.Revoked);

        var accounts = await dbContext.Accounts.AsNoTracking().Where(predicate).ToArrayAsync(cancellationToken: cancellationToken);
        return mapper.Map<AccountDto[]>(accounts);
    }
}