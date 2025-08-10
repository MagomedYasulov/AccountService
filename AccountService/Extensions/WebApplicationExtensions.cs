using AccountService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        const int maxRetryAttempts = 5;
        var retryDelay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxRetryAttempts; attempt++)
        {
            try
            {
                logger.LogInformation("Applying migrations (Attempt {Attempt}/{MaxAttempts})...", attempt, maxRetryAttempts);
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations applied successfully!");
                return app;
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

        return app;
    }
}