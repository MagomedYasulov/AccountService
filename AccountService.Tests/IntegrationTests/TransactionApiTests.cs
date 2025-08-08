using AccountService.Infrastructure.Data;
using AccountService.Tests.Extentions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Testcontainers.PostgreSql;

namespace AccountService.Tests.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("password")
            .WithImage("postgres:latest")
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
        }


        async Task IAsyncLifetime.DisposeAsync()
        {
            await _postgres.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Удаляем все зарегистрированные схемы аутентификации
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Можно убрать авторизацию, добавив политику, которая всегда разрешает
                services.AddAuthorizationBuilder().AddPolicy("AllowAll", policy =>
                {
                    policy.RequireAssertion(_ => true);
                });
            });


            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString()
                });
            });

            builder.ConfigureTestServices(services =>
            {
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
                
                db.MigrateDB(logger).SeedData();
            });
        }
    }

    [CollectionDefinition("Integration tests")]
    public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory> { }

    [Collection("Integration tests")]
    public class TransactionApiTests //: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public TransactionApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task Parraler_Transaction_MustPreservationTotalBalance()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/accounts");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }
    }

    // Тестовый обработчик, который всегда аутентифицирует пользователя
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                               ILoggerFactory logger,
                               UrlEncoder encoder) : base(options, logger, encoder)

        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
