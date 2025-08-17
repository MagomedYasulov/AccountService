using AccountService.Exceptions;
using AccountService.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Health.GetReady
{
    public class CheckReadyHandler : IRequestHandler<CheckReadyQuery>
    {
        private readonly AppDbContext _dbContext;

        public CheckReadyHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(CheckReadyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _dbContext.OutboxMessages.CountAsync(cancellationToken: cancellationToken) >= 100)
                    throw new ServiceException("Server Not Ready", "Unpublished outbox messages over 100", StatusCodes.Status503ServiceUnavailable);
            }
            catch(Exception ex)
            {
                throw new ServiceException("Server Not Ready", ex.Message, StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
