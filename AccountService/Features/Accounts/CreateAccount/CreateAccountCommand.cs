using AccountService.Domain.Enums;
using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountCommand : IRequest<AccountDto>
{
    public Guid OwnerId { get; set; }
    public AccountType? Type { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal? InterestRate { get; set; }
}