using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Domain.Data.Entities;

public class Transaction : BaseEntity
{
    public Guid AccountId { get; set; }

    [UsedImplicitly]
    public Account Account { get; set; } = null!;
    public Guid? CounterpartyAccountId { get; set; }

    [UsedImplicitly]
    public Account? CounterpartyAccount { get; set; }
    public decimal Sum { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransferTime { get; set; }

    public TransactionType CounterpartyType => Type == TransactionType.Credit ? TransactionType.Debit : TransactionType.Credit;
}