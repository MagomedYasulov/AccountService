using AccountService.Domain.Data.Entities;
using AccountService.Domain.Data.Repositories;
using AccountService.Domain.Enums;
using AccountService.Domain.Models;

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
                OwnerId = new Guid("7dc4a2af-305c-4ec3-810b-718157d010ae"), 
                CurrencyCode = "RUB", 
                Type = AccountType.Checking               
            }
        ];

        public Task CreateAsync(Account account)
        {
            account.Id = Guid.NewGuid();
            _accounts.Add(account);
            return Task.CompletedTask;
        }

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
