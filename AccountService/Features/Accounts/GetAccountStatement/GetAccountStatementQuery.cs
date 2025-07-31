using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.GetAccountStatement;

public class GetAccountStatementQuery : IRequest<AccountStatementDto>
{
    public Guid Id { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
}