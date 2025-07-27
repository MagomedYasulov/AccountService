using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public class DeleteAccountHandler(IAccountRepository accountRepository) : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id);
        if(account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

        if (request.IsSoft)
        {
            account.Revoked = true;
            account.ClosedAt = DateTime.UtcNow;
            await accountRepository.UpdateAsync(account);
        }
        else
        {
            await accountRepository.DeleteAsync(account);
        }
    }
}