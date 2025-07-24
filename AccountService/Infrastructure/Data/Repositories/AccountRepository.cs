using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using AccountService.Domain.Models;
using AccountService.Domain.Repositories;

namespace AccountService.Infrastructure.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly List<Account> _accounts =
        [
            new Account() 
            { 
                Id = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"), 
                Balance = 100, 
                OpenedAt = DateTime.Now, 
                OwnerId = Guid.NewGuid(), 
                CurrencyCode = "RUB", 
                Type = AccountType.Checking
            }
        ];

        public Task<Account[]> GetAsync(AccountFilter filter)
        {
            var result = _accounts.Where(a => filter.OwnerId == null || a.OwnerId == filter.OwnerId).ToArray();
            return Task.FromResult(result);
        }

        public Task<Account?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
        }
    }
}
