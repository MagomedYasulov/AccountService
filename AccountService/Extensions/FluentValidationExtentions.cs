using FluentValidation;
using ISO._4217;

namespace AccountService.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Iso4217<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(code => CurrencyCodesResolver.Codes.Any(c => c.Code == code));
    }
}