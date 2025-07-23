using AccountService.Domain.Enums;

namespace AccountService.Domain.Entities
{
    public class Account : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public AccountType Type { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public List<Transaction> Transactions { get; set; } = [];
    }
}
