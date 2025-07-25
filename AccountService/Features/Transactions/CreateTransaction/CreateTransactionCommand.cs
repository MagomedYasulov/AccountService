using AccountService.Domain.Enums;
using AccountService.Features.Transactions.Models;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction
{
    public class CreateTransactionCommand : IRequest<TransactionDto>
    {
        public Guid AccountId { get; set; }
        public Guid? CounterpartyAccountId { get; set; }
        public decimal Sum { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public TransactionType? Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
