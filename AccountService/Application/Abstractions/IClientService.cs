namespace AccountService.Application.Abstractions
{
    public interface IClientService
    {
        public bool VerifyClient(Guid clientId);
    }
}
