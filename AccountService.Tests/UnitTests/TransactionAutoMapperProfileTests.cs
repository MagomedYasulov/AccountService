using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.Models;
using AutoMapper;

namespace AccountService.Tests.UnitTests
{   
    public class TransactionAutoMapperProfileTests
    {
        private IMapper _mapper;

        public TransactionAutoMapperProfileTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new TransactionAutoMapperProfile()));
            _mapper = new Mapper(mapperConfig);
        }

        [Fact]
        public void Map_Transaction_To_TransactionDto()
        {
            // Arrange
            var transaction = new Transaction()
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                CounterpartyAccountId = Guid.NewGuid(),
                TransferTime = DateTime.UtcNow,
                CurrencyCode = "RUB",
                Description = "transaction desc",
                Sum = 796,               
                Type = TransactionType.Credit,
            };

            // Act
            var dto = _mapper.Map<TransactionDto>(transaction);

            // Assert
            Assert.Equal(transaction.Id, dto.Id);
            Assert.Equal(transaction.AccountId, dto.AccountId);
            Assert.Equal(transaction.CounterpartyAccountId, dto.CounterpartyAccountId);
            Assert.Equal(transaction.TransferTime, dto.TransferTime);
            Assert.Equal(transaction.CurrencyCode, dto.CurrencyCode);
            Assert.Equal(transaction.Description, dto.Description);
            Assert.Equal(transaction.Type, dto.Type);
            Assert.Equal(transaction.Sum, dto.Sum);
        }

        [Fact]
        public void Map_CreateTransactionDto_To_CreateTransactionCommand()
        {
            // Arrange
            var createTransactionDto = new CreateTransactionDto()
            {
                AccountId = Guid.NewGuid(),
                CounterpartyAccountId = Guid.NewGuid(),
                CurrencyCode = "RUB",
                Description = "transaction desc",
                Sum = 796,
                Type = TransactionType.Credit,
            };

            // Act
            var dto = _mapper.Map<CreateTransactionCommand>(createTransactionDto);

            // Assert
            Assert.Equal(createTransactionDto.AccountId, dto.AccountId);
            Assert.Equal(createTransactionDto.CounterpartyAccountId, dto.CounterpartyAccountId);
            Assert.Equal(createTransactionDto.CurrencyCode, dto.CurrencyCode);
            Assert.Equal(createTransactionDto.Description, dto.Description);
            Assert.Equal(createTransactionDto.Type, dto.Type);
            Assert.Equal(createTransactionDto.Sum, dto.Sum);
        }

        [Fact]
        public void Map_CreateTransactionCommand_To_Transaction()
        {
            // Arrange
            var createTransactionCommand = new CreateTransactionCommand()
            {
                AccountId = Guid.NewGuid(),
                CounterpartyAccountId = Guid.NewGuid(),
                CurrencyCode = "RUB",
                Description = "transaction desc",
                Sum = 796,
                Type = TransactionType.Credit,
            };

            // Act
            var dto = _mapper.Map<Transaction>(createTransactionCommand);

            // Assert
            Assert.Equal(createTransactionCommand.AccountId, dto.AccountId);
            Assert.Equal(createTransactionCommand.CounterpartyAccountId, dto.CounterpartyAccountId);
            Assert.Equal(createTransactionCommand.CurrencyCode, dto.CurrencyCode);
            Assert.Equal(createTransactionCommand.Description, dto.Description);
            Assert.Equal(createTransactionCommand.Type, dto.Type);
            Assert.Equal(createTransactionCommand.Sum, dto.Sum);
        }

    }
}
