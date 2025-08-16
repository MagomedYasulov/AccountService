namespace AccountService.Domain.Events
{
    public class InterestAccrued
    {
        public Guid EventId { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid AccountId { get; set; }

        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public decimal Amount { get; set; }
    }
}
