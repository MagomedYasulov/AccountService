using AccountService.Features.Accounts.DTOs;
using MediatR;

namespace AccountService.Features.Accounts.GetAccounts
{
    public class GetAccountsQuery : IRequest<AccountDto[]>
    {
        public Guid? OwnerId { get; set; }
    }
}
