namespace AccountService.Application.Abstractions
{
    public interface IInterestAccrualService
    {
        public Task AccrueInterestForAllDeposits();
    }
}
