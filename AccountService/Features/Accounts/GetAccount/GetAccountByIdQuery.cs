using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.GetAccount;

public class GetAccountByIdQuery : IRequest<AccountDto>
{
    public Guid Id { get; set; }
}