using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.Models;

public class TransactionStatementDto
{
    /// <summary>
    /// Счет контрагента транзакции
    /// </summary>
    public Guid? CounterpartyAccountId { get; set; }

    /// <summary>
    /// Сумма транзакции
    /// </summary>
    [UsedImplicitly]
    public decimal Sum { get; set; }

    /// <summary>
    /// Код валюты ISO4217
    /// </summary>
    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Тип транзакции (снятие или зачисление)
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Описание транзакции
    /// </summary>
    [UsedImplicitly]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Время совершения транзакции
    /// </summary>
    public DateTime TransferTime { get; set; }
}