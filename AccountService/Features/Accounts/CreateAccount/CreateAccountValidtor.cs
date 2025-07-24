using AccountService.Domain.Enums;
using AccountService.Extensions;
using FluentValidation;

namespace AccountService.Features.Accounts.CreateAccount
{
    public class CreateAccountValidtor : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountValidtor()
        {
            RuleFor(c => c.OwnerId).NotEmpty();
            RuleFor(c => c.Type).NotNull().IsInEnum();
            RuleFor(c => c.CurrencyCode).NotEmpty().ISO4217().WithMessage("'CurrencyCode' code must be in ISO4217");
            RuleFor(c => c.InterestRate).Null().When(command => command.Type == AccountType.Checking); 
            RuleFor(c => c.InterestRate).NotNull().When(command => command.Type != AccountType.Checking);
        }
    }
}
