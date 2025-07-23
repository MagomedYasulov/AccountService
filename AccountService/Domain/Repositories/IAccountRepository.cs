using AccountService.Domain.Entities;

namespace AccountService.Domain.Repositories
{
    public interface IAccountRepository
    {
        public Task<Account?> GetByIdAsync(Guid id);
    }
}
