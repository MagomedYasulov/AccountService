using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.Models;

public class TransactionStatementDto
{
    public Guid? CounterpartyAccountId { get; set; }

    [UsedImplicitly]
    public decimal Sum { get; set; }

    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;
    public TransactionType Type { get; set; }

    [UsedImplicitly]
    public string Description { get; set; } = string.Empty;
    public DateTime TransferTime { get; set; }
}