using AccountService.Domain.Data.Entities;
using AccountService.Domain.Models;

namespace AccountService.Domain.Data.Repositories
{
    public interface IAccountRepository
    {
        public Task CreateAsync(Account account);
        public Task<Account?> GetByIdAsync(Guid id);
        public Task<Account[]> GetAsync(AccountFilter filter);
        public Task UpdateAsync(Account account);
        public Task DeleteAsync(Account account);
    }
}
