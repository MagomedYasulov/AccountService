using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Features.Accounts.Models;
using AutoMapper;

namespace AccountService.Tests.UnitTests
{
    public class AccountAutoMapperProfileTests
    {
        private IMapper _mapper;

        public AccountAutoMapperProfileTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AccountAutoMapperProfile()));
            _mapper = new Mapper(mapperConfig);
        }


        [Fact]
        public void Map_Account_To_AccountDto()
        {
            // Arrange
            var account = new Account()
            {
                Id = Guid.NewGuid(),
                Balance = 424,
                ClosedAt = DateTime.UtcNow,
                OpenedAt = new DateTime(2025, 02, 2),
                OwnerId = Guid.NewGuid(),
                CurrencyCode = "RUB",
                InterestRate = 23,
                Type = AccountType.Credit,
                Revoked = true,
            };

            // Act
            var dto = _mapper.Map<AccountDto>(account);

            // Assert
            Assert.Equal(account.Id, dto.Id);
            Assert.Equal(account.Balance, dto.Balance);
            Assert.Equal(account.ClosedAt, dto.ClosedAt);
            Assert.Equal(account.OpenedAt, dto.OpenedAt);
            Assert.Equal(account.OwnerId, dto.OwnerId);
            Assert.Equal(account.CurrencyCode, dto.CurrencyCode);
            Assert.Equal(account.InterestRate, dto.InterestRate);
            Assert.Equal(account.Type, dto.Type);
            Assert.Equal(account.Revoked, dto.Revoked);
        }

        [Fact]
        public void Map_Transaction_To_TransactionStatementDto()
        {
            // Arrange
            var account = new Transaction()
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                CounterpartyAccountId = Guid.NewGuid(),
                CurrencyCode = "EUR",
                Description = "Transaction desc",
                Sum = 564,
                Type = TransactionType.Credit,
                TransferTime = DateTime.UtcNow,
            };

            // Act
            var dto = _mapper.Map<TransactionStatementDto>(account);

            // Assert
            Assert.Equal(account.CounterpartyAccountId, dto.CounterpartyAccountId);
            Assert.Equal(account.Description, dto.Description);
            Assert.Equal(account.Sum, dto.Sum);
            Assert.Equal(account.CurrencyCode, dto.CurrencyCode);
            Assert.Equal(account.TransferTime, dto.TransferTime);
            Assert.Equal(account.Type, dto.Type);
        }

        [Fact]
        public void Map_Account_To_AccountStatementDto()
        {
            // Arrange
            var counterpartyAccountId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var transferTime = DateTime.UtcNow;

            var account = new Account()
            {
                Id = accountId,
                Balance = 424,
                ClosedAt = DateTime.UtcNow,
                OpenedAt = new DateTime(2025, 02, 2),
                OwnerId = Guid.NewGuid(),
                CurrencyCode = "RUB",
                InterestRate = 23,
                Type = AccountType.Credit,
                Revoked = true,
                Transactions = [
                    new Transaction()
                    {
                        AccountId = accountId,
                        CounterpartyAccountId = counterpartyAccountId,
                        CurrencyCode = "EUR",
                        Description = "Transaction desc",
                        Sum = 564,
                        Type = TransactionType.Credit,
                        TransferTime = transferTime,
                    }
                ],
                CounterpartyTransactions = [
                    new Transaction()
                    {
                        AccountId = counterpartyAccountId,
                        CounterpartyAccountId = accountId,
                        CurrencyCode = "EUR",
                        Description = "Transaction desc",
                        Sum = 564,
                        Type = TransactionType.Debit, //Для аккаунта в транзакциях будет наоборот Credit
                        TransferTime = transferTime,
                    }
                ]
            };

            // Act
            var dto = _mapper.Map<AccountStatementDto>(account);

            // Assert
            Assert.Equal(account.Balance, dto.Balance);
            Assert.Equal(account.CurrencyCode, dto.CurrencyCode);

            foreach (var transaction in dto.Transactions)
            {
                Assert.Equal(counterpartyAccountId, transaction.CounterpartyAccountId);
                Assert.Equal(564, transaction.Sum);
                Assert.Equal("EUR", transaction.CurrencyCode);
                Assert.Equal("Transaction desc", transaction.Description);
                Assert.Equal(transferTime, transaction.TransferTime);
                Assert.Equal(TransactionType.Credit, transaction.Type);
            }
        }
    }
}
