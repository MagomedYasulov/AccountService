using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id);
            if(account == null)
                throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

            if (request.IsSoft)
            {
                account.Revoked = true;
                account.ClosedAt = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account);
            }
            else
            {
                await _accountRepository.DeleteAsync(account);
            }
        }
    }
}
