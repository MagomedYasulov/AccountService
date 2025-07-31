using JetBrains.Annotations;

namespace AccountService.Features.Accounts.Models;

public class AccountStatementDto
{
    /// <summary>
    /// Баланс счета
    /// </summary>
    [UsedImplicitly]
    public decimal Balance { get; set; }

    /// <summary>
    /// Код валюты счета ISO4217
    /// </summary>
    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// История транзакций
    /// </summary>
    public TransactionStatementDto[] Transactions { get; set; } = [];
}