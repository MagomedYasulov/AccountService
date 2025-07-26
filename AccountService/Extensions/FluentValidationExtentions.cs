using FluentValidation;

namespace AccountService.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> Iso4217<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(code => ISO._4217.CurrencyCodesResolver.Codes.Any(c => c.Code == code));
        }
    }
}
