namespace AccountService.Application.Abstractions
{
    public interface IClientService
    {
        public Task<bool> VerifyClient(Guid clientId);
    }
}
