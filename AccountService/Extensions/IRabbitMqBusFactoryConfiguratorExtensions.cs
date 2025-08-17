using AccountService.Domain.Events;
using AccountService.Infrastructure.RabbitMQ.Consumers;
using MassTransit;
using RabbitMQ.Client;

namespace AccountService.Extensions
{
    public static class IRabbitMqBusFactoryConfiguratorExtensions
    {
        public static IRabbitMqBusFactoryConfigurator ConfigureEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
        {
            cfg.ReceiveEndpoint("account.crm", e =>
            {
                e.Durable = true;
                e.Bind("account.events", x =>
                {
                    x.ExchangeType = ExchangeType.Topic;
                    x.RoutingKey = "account.*";
                });
            });

            cfg.ReceiveEndpoint("account.notifications", e =>
            {
                e.Durable = true;
                e.Bind("account.events", x =>
                {
                    x.ExchangeType = ExchangeType.Topic;
                    x.RoutingKey = "money.#";
                });
            });

            cfg.ReceiveEndpoint("account.antifraud", e =>
            {

                e.Durable = true;
                e.Bind("account.events", x =>
                {
                    x.ExchangeType = ExchangeType.Topic;
                    x.RoutingKey = "client.#";
                });

                e.ConfigureConsumer<AntifraudConsumer>(context);
            });

            cfg.ReceiveEndpoint("account.audit", e =>
            {
                e.Durable = true;
                e.Bind("account.events", x =>
                {
                    x.ExchangeType = ExchangeType.Topic;
                    x.RoutingKey = "#";
                });
            });
            return cfg;
        }

        public static IRabbitMqBusFactoryConfigurator ConfigureMessages(this IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Message<AccountOpened>(x => x.SetEntityName("account.events"));
            cfg.Message<InterestAccrued>(x => x.SetEntityName("account.events"));
            cfg.Message<MoneyCredited>(x => x.SetEntityName("account.events"));
            cfg.Message<MoneyDebited>(x => x.SetEntityName("account.events"));
            cfg.Message<TransferCompleted>(x => x.SetEntityName("account.events"));


            cfg.Publish<AccountOpened>(x => x.ExchangeType = ExchangeType.Topic);
            cfg.Publish<InterestAccrued>(x => x.ExchangeType = ExchangeType.Topic);
            cfg.Publish<MoneyCredited>(x => x.ExchangeType = ExchangeType.Topic);
            cfg.Publish<MoneyDebited>(x => x.ExchangeType = ExchangeType.Topic);
            cfg.Publish<TransferCompleted>(x => x.ExchangeType = ExchangeType.Topic);

            return cfg;
        }
    }
}
