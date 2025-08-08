using AccountService.Exceptions;
using AccountService.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Accounts.DeleteAccount;

public class DeleteAccountHandler(AppDbContext dbContext) : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        if(account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

        if (request.IsSoft)
        {
            account.Revoked = true;
            account.ClosedAt = DateTime.UtcNow;
        }
        else
        {
            dbContext.Accounts.Remove(account);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}