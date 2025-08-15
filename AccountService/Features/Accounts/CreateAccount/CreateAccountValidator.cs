using AccountService.Domain.Enums;
using AccountService.Extensions;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.CreateAccount;

[UsedImplicitly]
public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(c => c.OwnerId).NotEmpty();
        RuleFor(c => c.Type).NotNull().IsInEnum();
        RuleFor(c => c.CurrencyCode).NotEmpty().Iso4217().WithMessage("'CurrencyCode' code must be in Iso4217");
        RuleFor(c => c.InterestRate).Null().When(command => command.Type == AccountType.Checking).WithMessage("'InterestRate' mush be null for checking account"); 
        RuleFor(c => c.InterestRate).NotNull().When(command => command.Type != AccountType.Checking);
        RuleFor(c => c.ClosedAt).NotNull().GreaterThan(DateTime.UtcNow).When(c => c.Type == AccountType.Deposit);
        RuleFor(c => c.ClosedAt).Null().When(c => c.Type != AccountType.Deposit);
    }
}