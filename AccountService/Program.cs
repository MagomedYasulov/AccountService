
using AccountService.Extensions;
using AccountService.Infrastructure.Services;
using Hangfire;

namespace AccountService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.AddData();
        builder.AddControllers();
        builder.AddMediator();
        builder.AddAppServices();
        builder.AddFluentValidation();
        builder.AddAutoMapper();
        builder.AddExceptionHandler();
        builder.AddSwagger();
        builder.AddCors();
        builder.AddAuthentication();
        builder.AddAuthorization();
        builder.AddHangfire();
        builder.AddRabbitMQ();

        var app = builder.Build();

        app.MigrateDb();
    
        app.UseCors("cors");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
           
        }

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = []
        });

        RecurringJob.AddOrUpdate<InterestAccrualService>(
           "accrue-interest-daily",
           service => service.AccrueInterestForAllDeposits(),
           Cron.Daily(0, 0)
        );


        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}