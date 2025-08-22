using AccountService.Domain.Data.Entities;
using AccountService.Domain.Events;
using AccountService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.RabbitMQ.Consumers;

public class AntifraudConsumer(AppDbContext dbContext) : IConsumer<ClientBlocked>, IConsumer<ClientUnblocked>
{
    public async Task Consume(ConsumeContext<ClientBlocked> context)
    {
        if (await dbContext.InboxConsumeds.AnyAsync(ic => ic.MessageId == context.MessageId))
            return;

        var accounts = await dbContext.Accounts.Where(a => a.OwnerId == context.Message.ClientId).ToArrayAsync();
        foreach (var account in accounts)
        {
            account.Frozen = true;
        }

        var consumed = new InboxConsumed
        {
            MessageId = context.MessageId!.Value,
            ProcessedAt = DateTime.UtcNow,
            Handler = nameof(AntifraudConsumer)
        };
        await dbContext.InboxConsumeds.AddAsync(consumed);

        await dbContext.SaveChangesAsync();
    }

    public async Task Consume(ConsumeContext<ClientUnblocked> context)
    {
        if (await dbContext.InboxConsumeds.AnyAsync(ic => ic.MessageId == context.MessageId))
            return;

        var accounts = await dbContext.Accounts.Where(a => a.OwnerId == context.Message.ClientId).ToArrayAsync();
        foreach (var account in accounts)
        {
            account.Frozen = false;
        }

        var consumed = new InboxConsumed
        {
            MessageId = context.MessageId!.Value,
            ProcessedAt = DateTime.UtcNow,
            Handler = nameof(AntifraudConsumer)
        };
        await dbContext.InboxConsumeds.AddAsync(consumed);

        await dbContext.SaveChangesAsync();
    }
}