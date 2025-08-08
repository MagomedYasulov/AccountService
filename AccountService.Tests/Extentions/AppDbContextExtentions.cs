using AccountService.Domain.Data.Entities;
using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Tests.Extentions
{
    public static class AppDbContextExtentions
    {
        public static void SeedData(this AppDbContext dbContext)
        {
            var account1 = new Account()
            {
                Balance = 500000,
                CurrencyCode = "RUB",
                Type = Domain.Enums.AccountType.Checking,
                OwnerId = Guid.NewGuid(),
            };

            var account2 = new Account()
            {
                Balance = 500000,
                CurrencyCode = "RUB",
                Type = Domain.Enums.AccountType.Checking,
                OwnerId = Guid.NewGuid(),
            };


            dbContext.Accounts.AddRange(account1, account2);
            dbContext.SaveChanges();
        }

        public static AppDbContext MigrateDB(this AppDbContext dbContext, ILogger<AppDbContext> logger)
        {
            int maxRetryAttempts = 5;
            TimeSpan retryDelay = TimeSpan.FromSeconds(3);

            for (int attempt = 1; attempt <= maxRetryAttempts; attempt++)
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
}
