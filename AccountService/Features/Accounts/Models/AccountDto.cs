using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.Models;

public class AccountDto
{
    /// <summary>
    /// Id Счета
    /// </summary>
    [UsedImplicitly]
    public Guid Id { get; set; }

    /// <summary>
    /// Id Владельца
    /// </summary>
    [UsedImplicitly]
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Тип счета Checking | Deposit | Credit
    /// </summary>
    [UsedImplicitly]
    public AccountType Type { get; set; }

    /// <summary>
    /// Код валюты ISO4217
    /// </summary>
    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Баланс счета
    /// </summary>
    [UsedImplicitly]
    public decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка
    /// </summary>
    [UsedImplicitly]
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Дата открытия
    /// </summary>
    [UsedImplicitly]
    public DateTime OpenedAt { get; set; }

    /// <summary>
    /// Дата закрытия
    /// </summary>
    [UsedImplicitly]
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Аннулированный(удаленый) или нет счет
    /// </summary>
    [UsedImplicitly]
    public bool Revoked { get; set; }
}