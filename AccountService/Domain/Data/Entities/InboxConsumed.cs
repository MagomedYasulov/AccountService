namespace AccountService.Domain.Data.Entities;

public class InboxConsumed
{
    public Guid MessageId { get; set; }
    public DateTime ProcessedAt { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Handler { get; set; } = string.Empty;
}