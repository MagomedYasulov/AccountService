﻿using AccountService.Extensions;
using FluentValidation;
using JetBrains.Annotations;

namespace AccountService.Features.Transactions.CreateTransaction;

[UsedImplicitly]
public class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(c => c.AccountId).NotEmpty();
        RuleFor(c => c.CurrencyCode).NotEmpty().Iso4217().WithMessage("'CurrencyCode' code must be in Iso4217");
        RuleFor(c => c.Sum).GreaterThan(0);
        RuleFor(c => c.Type).NotNull().IsInEnum();
    }
}