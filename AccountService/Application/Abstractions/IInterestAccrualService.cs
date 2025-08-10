using JetBrains.Annotations;

namespace AccountService.Application.Abstractions;

public interface IInterestAccrualService
{
    [UsedImplicitly]
    public Task AccrueInterestForAllDeposits();
}