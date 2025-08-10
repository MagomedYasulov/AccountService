using AccountService.Domain.Enums;
using AccountService.Domain.Models;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.CreateTransaction;
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
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

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
        private readonly ITestOutputHelper _output;

        public TransactionApiTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
        }


        [Fact]
        public async Task Parraler_Transaction_MustPreservationTotalBalance()
        {
            // Arrange
            var account1Id = new Guid("45342ce3-c18e-4572-be2f-e1563d2c0f6d");
            var account2Id = new Guid("215e98c9-c890-4a64-9664-07a755b9f01a");

            var debitTransactionAccount1 = new CreateTransactionDto()
            {
                AccountId = account1Id,
                CounterpartyAccountId = account2Id,
                CurrencyCode = "RUB",
                Sum = 100,
                Type = TransactionType.Debit
            };

            var debitTransactionAccount2 = new CreateTransactionDto()
            {
                AccountId = account2Id,
                CounterpartyAccountId = account1Id,
                CurrencyCode = "RUB",
                Sum = 100,
                Type = TransactionType.Debit
            };

            var json1 = JsonConvert.SerializeObject(debitTransactionAccount1);
            var json2 = JsonConvert.SerializeObject(debitTransactionAccount2);


            var client = _factory.CreateClient();

            // Act
            var tasks = new Task<HttpResponseMessage>[50];
            StringContent? httpContent = null;
            for(int i = 0; i < 50; i++)
            {
                if(i % 2 == 0)
                    httpContent = new StringContent(json1, Encoding.UTF8, "application/json");
                else
                    httpContent = new StringContent(json2, Encoding.UTF8, "application/json");

                tasks[i] = client.PostAsync("/api/v1/transactions", httpContent);
            }

            await Task.WhenAll(tasks);

            var response1 = await client.GetAsync($"/api/v1/accounts/{account1Id}/statement");
            var response2 = await client.GetAsync($"/api/v1/accounts/{account2Id}/statement");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);


            var accountStatement1 = JsonConvert.DeserializeObject<MbResult<AccountStatementDto>>(await response1.Content.ReadAsStringAsync())?.Value;
            var accountStatement2 = JsonConvert.DeserializeObject<MbResult<AccountStatementDto>>(await response2.Content.ReadAsStringAsync())?.Value;

            Assert.NotNull(accountStatement1);
            Assert.NotNull(accountStatement2);

            _output.WriteLine($"Account1 transactions count {accountStatement1.Transactions.Length}");
            _output.WriteLine($"Account2 transactions count {accountStatement2.Transactions.Length}");
            Assert.True(accountStatement1.Transactions.Length >= 2); //Минимун 2 потому что в бд уже есть 1 транзакция
            Assert.True(accountStatement2.Transactions.Length >= 1);

            Assert.Equal(100, accountStatement1.Balance + accountStatement2.Balance);
            Assert.True(accountStatement1.Balance >= 0);
            Assert.True(accountStatement2.Balance >= 0);

            var sum = 0M;
            foreach(var transaction in accountStatement1.Transactions.OrderBy(t => t.TransferTime))
            {
                if (transaction.Type == TransactionType.Debit)
                    sum -= transaction.Sum;
                else
                    sum += transaction.Sum;

                Assert.True(sum <= 100);
                Assert.True(sum >= 0);
            }
            Assert.Equal(sum, accountStatement1.Balance);

            sum = 0M;
            foreach (var transaction in accountStatement2.Transactions.OrderBy(t => t.TransferTime))
            {
                if (transaction.Type == TransactionType.Debit)
                    sum -= transaction.Sum;
                else
                    sum += transaction.Sum;

                Assert.True(sum >= 0);
                Assert.True(sum <= 100);
            }
            Assert.Equal(sum, accountStatement2.Balance);
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
