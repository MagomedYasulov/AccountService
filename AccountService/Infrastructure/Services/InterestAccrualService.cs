using AccountService.Application.Abstractions;
using AccountService.Domain.Enums;
using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.Services
{
    public class InterestAccrualService : IInterestAccrualService
    {
        private readonly AppDbContext _dbContext;

        public InterestAccrualService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AccrueInterestForAllDeposits()
        {
            var depositAccountIds = await _dbContext.Accounts.Where(a => a.Type == AccountType.Deposit).Select(a => a.Id).ToArrayAsync();

            foreach (var accountId in depositAccountIds)
            {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"CALL public.accrue_interest({accountId})");
            }
        }
    }
}
