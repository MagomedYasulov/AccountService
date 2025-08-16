using AccountService.Application.Abstractions;
using AccountService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountService.Infrastructure.Services
{
    public class OutboxDispatcher : IOutboxDispatcher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly AppDbContext _dbContext;

        public OutboxDispatcher(
            IPublishEndpoint publishEndpoint,
            AppDbContext dbContext)
        {
            _publishEndpoint = publishEndpoint;
            _dbContext = dbContext;
        }

        public async Task Dispatch()
        {
            var messages = await _dbContext.OutboxMessages.Where(x => !x.Sent).ToArrayAsync();

            foreach (var message in messages)
            {
                var type = Type.GetType(message.EventType);
                var @event = JsonConvert.DeserializeObject(message.Payload, type!);
                await _publishEndpoint.Publish(@event!, context => context.SetRoutingKey(message.RoutingKey), CancellationToken.None);
     
                message.Sent = true;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

