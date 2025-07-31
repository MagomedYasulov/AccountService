using AccountService.Application.Abstractions;

namespace AccountService.Infrastructure.Services;

public class ClientService : IClientService
{
    public Task<bool> VerifyClient(Guid clientId)
    {
        return Task.FromResult(true);
    }
}