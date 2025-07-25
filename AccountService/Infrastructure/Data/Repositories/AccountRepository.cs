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
                OpenedAt = new DateTime(2024, 9, 29, 0, 0, 0, DateTimeKind.Utc), 
                OwnerId = new Guid("7dc4a2af-305c-4ec3-810b-718157d010ae"), 
                CurrencyCode = "RUB", 
                Type = AccountType.Checking,
                Transactions = [
                    new Transaction()
                    {
                        Id = Guid.NewGuid(),
                        AccountId = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"),
                        CurrencyCode = "RUB",
                        Description = "transaction desc",
                        Sum = 200,
                        TransferTime = new DateTime(2025, 9, 29,  0, 0, 0, DateTimeKind.Utc),
                        Type = TransactionType.Credit,                                            
                    },
                    new Transaction()
                    {
                        Id = Guid.NewGuid(),
                        AccountId = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"),
                        CurrencyCode = "RUB",
                        Description = "transaction desc 2",
                        Sum = 100,
                        TransferTime = new DateTime(2025, 9, 29, 0, 0,0, DateTimeKind.Utc),
                        Type = TransactionType.Debit,                      
                    }
                ]
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
            var isOwnerIdNull = filter.OwnerId == null;
            var isRevokedNull = filter.Revoked == null;

            Func<Account, bool> predicate = (account) => (isOwnerIdNull || account.OwnerId == filter.OwnerId) &&
                                                         (isRevokedNull || account.Revoked == filter.Revoked);
            var result = _accounts.Where(predicate).ToArray();
            return Task.FromResult(result);
        }

        public Task<Account?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
        }

        public Task UpdateAsync(Account account)
        {
            _accounts.RemoveAll(a => a.Id == account.Id);
            _accounts.Add(account);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Account account)
        {
            _accounts.RemoveAll(a => a.Id == account.Id);
            return Task.CompletedTask;
        }

        public Task CreateTransactionAsync(Transaction transaction)
        {
            var account = _accounts.First(a => a.Id == transaction.AccountId);
            transaction.Id = Guid.NewGuid();
            account.Transactions.Add(transaction);
            return Task.CompletedTask;
        }
    }
}
