using AccountService.Domain.Enums;

namespace AccountService.Domain.Events
{
    public class AccountOpened
    {
        public Guid EventId { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid AccountId { get; set; }
        public Guid OwnerId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public AccountType Type { get; set; }
    }
}
