﻿using AccountService.Application.Abstractions;
using AccountService.Application.PipelineBehaviors;
using AccountService.Domain.Data.Repositories;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Data.Repositories;
using AccountService.Infrastructure.Services;
using AccountService.Middlewares;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountService.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddData(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
        return builder;
    }

    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddRouting(opt => opt.LowercaseUrls = true);
        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });
        return builder;
    }

    public static WebApplicationBuilder AddMediator(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviors<,>));
        return builder;
    }

    public static WebApplicationBuilder AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        return builder;
    }

    public static WebApplicationBuilder AddAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AccountAutoMapperProfile>();
            cfg.AddProfile<TransactionAutoMapperProfile>();
        });
        return builder;
    }

    public static WebApplicationBuilder AddExceptionHandler(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        return builder;
    }

    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AccountService API",
                Version = "v1"
            });
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AccountService.xml"));
        });

        return builder;
    }

    public static WebApplicationBuilder AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
        builder.Services.AddSingleton<IClientService, ClientService>();
        return builder;
    }

}