using AccountService.Domain.Data.Entities;
using AccountService.Domain.Events;
using AccountService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.RabbitMQ.Consumers
{
    public class AntifraudConsumer : IConsumer<ClientBlocked>, IConsumer<ClientUnblocked>
    {
        private readonly AppDbContext _dbContext;

        public AntifraudConsumer(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ClientBlocked> context)
        {
            if (await _dbContext.InboxConsumeds.AnyAsync(ic => ic.MessageId == context.MessageId))
                return;

            var accounts = await _dbContext.Accounts.Where(a => a.OwnerId == context.Message.ClientId).ToArrayAsync();
            foreach (var account in accounts)
            {
                account.Frozen = true;
            }

            var consumed = new InboxConsumed()
            {
                MessageId = context.MessageId!.Value,
                ProcessedAt = DateTime.UtcNow,
                Handler = nameof(AntifraudConsumer)
            };
            await _dbContext.InboxConsumeds.AddAsync(consumed);

            await _dbContext.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<ClientUnblocked> context)
        {
            if (await _dbContext.InboxConsumeds.AnyAsync(ic => ic.MessageId == context.MessageId))
                return;

            var accounts = await _dbContext.Accounts.Where(a => a.OwnerId == context.Message.ClientId).ToArrayAsync();
            foreach (var account in accounts)
            {
                account.Frozen = false;
            }

            var consumed = new InboxConsumed()
            {
                MessageId = context.MessageId!.Value,
                ProcessedAt = DateTime.UtcNow,
                Handler = nameof(AntifraudConsumer)
            };
            await _dbContext.InboxConsumeds.AddAsync(consumed);

            await _dbContext.SaveChangesAsync();
        }
    }
}
