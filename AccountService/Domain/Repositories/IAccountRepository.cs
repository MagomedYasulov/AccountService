using AccountService.Domain.Entities;
using AccountService.Domain.Models;

namespace AccountService.Domain.Repositories
{
    public interface IAccountRepository
    {
        public Task<Account?> GetByIdAsync(Guid id);

        public Task<Account[]> GetAsync(AccountFilter filter);
    }
}
