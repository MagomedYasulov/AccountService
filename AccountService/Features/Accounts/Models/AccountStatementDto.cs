using JetBrains.Annotations;

namespace AccountService.Features.Accounts.Models;

public class AccountStatementDto
{
    [UsedImplicitly]
    public decimal Balance { get; set; }

    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;
    public TransactionStatementDto[] Transactions { get; set; } = [];
}