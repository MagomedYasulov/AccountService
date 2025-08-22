using JetBrains.Annotations;

namespace AccountService.Domain.Events;

public class InterestAccrued
{
    [UsedImplicitly]
    public Guid EventId { get; set; }

    [UsedImplicitly]
    public DateTime OccurredAt { get; set; }

    [UsedImplicitly]
    public Guid AccountId { get; set; }

    [UsedImplicitly]
    public DateTime PeriodFrom { get; set; }

    [UsedImplicitly]
    public DateTime PeriodTo { get; set; }

    [UsedImplicitly]
    public decimal Amount { get; set; }
}