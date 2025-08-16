namespace AccountService.Domain.Data.Entities
{
    public class OutboxMessage : BaseEntity
    {
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public string RoutingKey { get; set; } = string.Empty;
        public bool Sent { get; set; }
    }
}
