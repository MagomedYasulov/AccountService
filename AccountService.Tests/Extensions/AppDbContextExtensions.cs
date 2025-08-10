using AccountService.Domain.Data.Entities;
using AccountService.Domain.Enums;
using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Tests.Extensions;

public static class AppDbContextExtensions
{
    public static void SeedData(this AppDbContext dbContext)
    {
        var account1 = new Account
        {
            Id = new Guid("45342ce3-c18e-4572-be2f-e1563d2c0f6d"),
            Balance = 100,
            CurrencyCode = "RUB",
            Type = AccountType.Checking,
            OwnerId = Guid.NewGuid(),
            Transactions = [
                new Transaction
                {
                    CurrencyCode = "RUB",
                    Sum = 100,
                    TransferTime = DateTime.UtcNow,
                    Type = TransactionType.Credit
                }
            ]
        };

        var account2 = new Account
        {
            Id = new Guid("215e98c9-c890-4a64-9664-07a755b9f01a"),
            Balance = 0,
            CurrencyCode = "RUB",
            Type = AccountType.Checking,
            OwnerId = Guid.NewGuid()
        };


        dbContext.Accounts.AddRange(account1, account2);
        dbContext.SaveChanges();
    }

    public static AppDbContext MigrateDb(this AppDbContext dbContext, ILogger<AppDbContext> logger)
    {
        const int maxRetryAttempts = 5;
        var retryDelay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxRetryAttempts; attempt++)
        {
            try
            {
                logger.LogInformation("Applying migrations (Attempt {Attempt}/{MaxAttempts})...", attempt, maxRetryAttempts);
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations applied successfully!");
                return dbContext;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration attempt {Attempt}/{MaxAttempts} failed.", attempt, maxRetryAttempts);

                if (attempt == maxRetryAttempts)
                {
                    logger.LogCritical("All migration attempts failed. Application will exit.");
                    throw;
                }

                Thread.Sleep(retryDelay);
            }
        }

        return dbContext;
    }
}