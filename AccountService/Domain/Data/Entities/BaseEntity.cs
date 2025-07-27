using JetBrains.Annotations;

namespace AccountService.Domain.Data.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }

    [UsedImplicitly]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}