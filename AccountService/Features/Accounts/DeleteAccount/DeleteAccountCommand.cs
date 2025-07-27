using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public class DeleteAccountCommand : IRequest
{
    public Guid Id { get; set; }
    public bool IsSoft { get; set; }
}