using AccountService.Application.Abstractions;
using AccountService.Extensions;
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
        builder.AddRabbitMq();

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

        AddHangfireJobs();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }


    private static void AddHangfireJobs()
    {
        RecurringJob.AddOrUpdate<IInterestAccrualService>(
          "accrue-interest-daily",
          service => service.AccrueInterestForAllDeposits(),
          Cron.Daily(0, 0)
       );

        RecurringJob.AddOrUpdate<IOutboxDispatcher>(
            "events-dispatch",
            service => service.Dispatch(),
            "*/10 * * * * *"
        );
    }
}