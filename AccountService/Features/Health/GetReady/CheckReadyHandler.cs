using AccountService.Exceptions;
using AccountService.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Health.GetReady;

public class CheckReadyHandler(AppDbContext dbContext) : IRequestHandler<CheckReadyQuery>
{
    public async Task Handle(CheckReadyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (await dbContext.OutboxMessages.CountAsync(cancellationToken: cancellationToken) >= 100)
                throw new ServiceException("Server Not Ready", "Unpublished outbox messages over 100", StatusCodes.Status503ServiceUnavailable);
        }
        catch(Exception ex)
        {
            throw new ServiceException("Server Not Ready", ex.Message, StatusCodes.Status503ServiceUnavailable);
        }
    }
}