
namespace AccountService.Domain.Events
{
    public class TransferCompleted
    {
        public Guid EventId { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public Guid TransferId { get; set; }
    }
}
