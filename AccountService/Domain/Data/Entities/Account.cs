using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Domain.Data.Entities;

public class Account : BaseEntity
{
    public Guid OwnerId { get; set; }
    public AccountType Type { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    [UsedImplicitly]
    public DateTime? LastAccruedAt { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
    public List<Transaction> CounterpartyTransactions { get; set; } = [];

    /// <summary>
    /// Soft delete
    /// </summary>
    public bool Revoked { get; set; }

    /// <summary>
    /// Оптимистичная блокировка через concurrency‑token
    /// </summary>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public uint xmin { get; set; }
}