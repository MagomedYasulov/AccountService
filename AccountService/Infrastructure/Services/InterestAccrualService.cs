using AccountService.Application.Abstractions;
using AccountService.Domain.Enums;
using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.Services;

public class InterestAccrualService(AppDbContext dbContext) : IInterestAccrualService
{
    public async Task AccrueInterestForAllDeposits()
    {
        var depositAccountIds = await dbContext.Accounts.Where(a => a.Type == AccountType.Deposit).Select(a => a.Id).ToArrayAsync();

        foreach (var accountId in depositAccountIds)
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"CALL public.accrue_interest({accountId})");
        }
    }
}