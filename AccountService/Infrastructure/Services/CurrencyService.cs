using AccountService.Application.Abstractions;

namespace AccountService.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private readonly List<string> _supportedCurrency = [ "RUB", "EUR", "USD" ];

    public Task<bool> IsSupportedCurrency(string currency)
    {
        return Task.FromResult(_supportedCurrency.Contains(currency));
    }
}