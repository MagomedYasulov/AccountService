using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.GetAccounts;

public class GetAccountsQuery : IRequest<AccountDto[]>
{
    public Guid? OwnerId { get; set; }
    public bool? Revoked { get; set; }
}