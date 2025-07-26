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
            new () 
            { 
                Id = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"), 
                Balance = 150, 
                OpenedAt = new DateTime(2003, 9, 29, 0, 0, 0, DateTimeKind.Utc), 
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
                        TransferTime = new DateTime(2017, 9, 29,  0, 0, 0, DateTimeKind.Utc),
                        Type = TransactionType.Credit,                                            
                    },
                    new Transaction()
                    {
                        Id = Guid.NewGuid(),
                        AccountId = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"),
                        CurrencyCode = "RUB",
                        Description = "transaction desc 2",
                        Sum = 100,
                        TransferTime = new DateTime(2018, 9, 29, 0, 0,0, DateTimeKind.Utc),
                        Type = TransactionType.Debit,                      
                    }
                ],
                CounterpartyTransactions = [
                    new Transaction()
                    {
                        Id = new Guid("c4067091-9adb-4219-b022-c5e81164e219"),
                        AccountId = new Guid("b795e3ed-0bed-4326-846f-370ee340191d"),
                        CounterpartyAccountId = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"),
                        CurrencyCode = "RUB",
                        Description = "transaction desc",
                        Sum = 50,
                        TransferTime = new DateTime(2016, 10, 29,  0, 0, 0, DateTimeKind.Utc),
                        Type = TransactionType.Debit,
                    },
                ]
            },
            new ()
            {
                Id = new Guid("b795e3ed-0bed-4326-846f-370ee340191d"),
                Balance = 250,
                OpenedAt = new DateTime(2004, 9, 29, 0, 0, 0, DateTimeKind.Utc),
                OwnerId = new Guid("ddfb9c53-e007-4262-af3d-8026b33642cb"),
                CurrencyCode = "RUB",
                Type = AccountType.Checking,
                Transactions = [
                    new Transaction()
                    {
                        Id = Guid.NewGuid(),
                        AccountId = new Guid("b795e3ed-0bed-4326-846f-370ee340191d"),
                        CurrencyCode = "RUB",
                        Description = "transaction desc account 2",
                        Sum = 300,
                        TransferTime = new DateTime(2014, 9, 29,  0, 0, 0, DateTimeKind.Utc),
                        Type = TransactionType.Credit,
                    },
                    new Transaction()
                    {
                        Id = new Guid("c4067091-9adb-4219-b022-c5e81164e219"),
                        AccountId = new Guid("b795e3ed-0bed-4326-846f-370ee340191d"),
                        CounterpartyAccountId = new Guid("438af497-aece-4b49-8448-e3f8d142aaa1"),
                        CurrencyCode = "RUB",
                        Description = "transaction desc",
                        Sum = 50,
                        TransferTime = new DateTime(2016, 10, 29,  0, 0, 0, DateTimeKind.Utc),
                        Type = TransactionType.Debit,
                    },
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

            var result = _accounts.Where(Predicate).ToArray();
            return Task.FromResult(result);

            bool Predicate(Account account) => (isOwnerIdNull || account.OwnerId == filter.OwnerId) &&
                                               (isRevokedNull || account.Revoked == filter.Revoked);
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

            if (transaction.CounterpartyAccountId != null)
            {
                var counterpartyAccount = _accounts.First(a => a.Id == transaction.CounterpartyAccountId);
                counterpartyAccount.CounterpartyTransactions.Add(transaction);
            }
            return Task.CompletedTask;
        }
    }
}
