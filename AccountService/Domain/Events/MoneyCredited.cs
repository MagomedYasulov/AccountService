namespace AccountService.Domain.Events
{
    public class MoneyCredited
    {
        public Guid EventId { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public Guid OperationId { get; set; }
    }
}
