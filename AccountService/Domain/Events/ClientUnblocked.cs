namespace AccountService.Domain.Events
{
    public class ClientUnblocked
    {
        public Guid EventId { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid ClientId { get; set; }
    }
}
