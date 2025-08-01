
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
        builder.AddAuthentication();
        builder.AddAuthorization();

        var app = builder.Build();

        app.UseCors("cors");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}