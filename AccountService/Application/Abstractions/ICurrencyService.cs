namespace AccountService.Application.Abstractions;

public interface ICurrencyService
{
    public Task<bool> IsSupportedCurrency(string currency);
}