using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Features.Transactions.Models;

public class TransactionDto
{
    [UsedImplicitly]
    public Guid Id { get; set; }

    [UsedImplicitly]
    public Guid AccountId { get; set; }

    [UsedImplicitly]
    public Guid? CounterpartyAccountId { get; set; }

    [UsedImplicitly]
    public decimal Sum { get; set; }

    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;

    [UsedImplicitly]
    public TransactionType Type { get; set; }

    [UsedImplicitly]
    public string Description { get; set; } = string.Empty;

    [UsedImplicitly]
    public DateTime TransferTime { get; set; }
}