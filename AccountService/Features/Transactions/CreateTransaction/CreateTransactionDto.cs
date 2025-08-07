using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Features.Transactions.CreateTransaction;

public class CreateTransactionDto
{
    /// <summary>
    /// Id Счета транзакции
    /// </summary>
    [UsedImplicitly]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Id Счета контрагента счета транзакции
    /// </summary>
    [UsedImplicitly]
    public Guid? CounterpartyAccountId { get; set; }

    /// <summary>
    /// Сумма транзакции
    /// </summary>
    [UsedImplicitly]
    public decimal Sum { get; set; }

    /// <summary>
    /// Код валюты транзакции ISO4217
    /// </summary>
    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Тип тразакции (снятие или зачисление)
    /// </summary>
    [UsedImplicitly]
    public TransactionType? Type { get; set; }

    /// <summary>
    /// Описание транзакции
    /// </summary>
    [UsedImplicitly]
    public string Description { get; set; } = string.Empty;
}