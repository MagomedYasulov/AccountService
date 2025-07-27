using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Accounts.UpdateAccount;

[UsedImplicitly]
public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountValidator()
    {
        RuleFor(c => c.InterestRate).NotNull();
    }
}