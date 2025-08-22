using JetBrains.Annotations;

namespace AccountService.Domain.Events;

public class ClientBlocked
{
    [UsedImplicitly]
    public Guid EventId { get; set; }
    [UsedImplicitly]
    public DateTime OccurredAt { get; set; }
    public Guid ClientId { get; set; }
}