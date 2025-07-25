using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.GetAccountStatement
{
    public class GetAccountStatementQuery : IRequest<AccountStatementDto>
    {
        public Guid Id { get; set; }
    }
}
