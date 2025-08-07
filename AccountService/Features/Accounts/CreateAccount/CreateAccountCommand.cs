using AccountService.Domain.Enums;
using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountCommand : IRequest<AccountDto>
{
    /// <summary>
    /// Id Клиента владельца счета
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Тип аккаунта
    /// </summary>
    public AccountType? Type { get; set; }

    /// <summary>
    /// Код валюты ISO4217 счета
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Процентная ставка счета
    /// </summary>
    public decimal? InterestRate { get; set; }
}