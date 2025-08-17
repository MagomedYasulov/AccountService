namespace AccountService.Domain.Data.Entities;

public class OutboxMessage : BaseEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string EventType { get; set; } = string.Empty;
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string RoutingKey { get; set; } = string.Empty;
    public bool Sent { get; set; }
}