using AccountService.Application.Abstractions;
using AccountService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountService.Infrastructure.Services;

public class OutboxDispatcher(
    IPublishEndpoint publishEndpoint,
    AppDbContext dbContext)
    : IOutboxDispatcher
{
    public async Task Dispatch()
    {
        var messages = await dbContext.OutboxMessages.Where(x => !x.Sent).ToArrayAsync();

        foreach (var message in messages)
        {
            var type = Type.GetType(message.EventType);
            var @event = JsonConvert.DeserializeObject(message.Payload, type!);
            await publishEndpoint.Publish(@event!, context => context.SetRoutingKey(message.RoutingKey), CancellationToken.None);
     
            message.Sent = true;
        }

        await dbContext.SaveChangesAsync();
    }
}