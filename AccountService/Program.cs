
using AccountService.Extensions;

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

        var app = builder.Build();

        app.UseCors("cors");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}