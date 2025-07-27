using AccountService.Domain.Enums;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.Models;

public class AccountDto
{
    [UsedImplicitly]
    public Guid Id { get; set; }

    [UsedImplicitly]
    public Guid OwnerId { get; set; }

    [UsedImplicitly]
    public AccountType Type { get; set; }

    [UsedImplicitly]
    public string CurrencyCode { get; set; } = string.Empty;

    [UsedImplicitly]
    public decimal Balance { get; set; }

    [UsedImplicitly]
    public decimal? InterestRate { get; set; }

    [UsedImplicitly]
    public DateTime OpenedAt { get; set; }

    [UsedImplicitly]
    public DateTime? ClosedAt { get; set; }

    [UsedImplicitly]
    public bool Revoked { get; set; }
}