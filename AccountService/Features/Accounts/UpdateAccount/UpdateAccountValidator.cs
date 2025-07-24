using FluentValidation;

namespace AccountService.Features.Accounts.UpdateAccount
{
    public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountValidator()
        {
            RuleFor(c => c.InterestRate).NotNull();
        }
    }
}
