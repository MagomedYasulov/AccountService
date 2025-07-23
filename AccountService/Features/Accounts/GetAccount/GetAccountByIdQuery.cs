using AccountService.Features.Accounts.DTOs;
using MediatR;

namespace AccountService.Features.Accounts.GetAccount
{
    public class GetAccountByIdQuery : IRequest<AccountDto>
    {
        public Guid Id { get; set; }
    }
}
