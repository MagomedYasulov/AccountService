using AccountService.Domain.Enums;

namespace AccountService.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;
        public Guid? CounterpartyAccountId { get; set; }
        public Account? CounterpartyAccount { get; set; }
        public decimal Sum { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransferTime { get; set; }
    }
}
