using AccountService.Application.Abstractions;

namespace AccountService.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly List<Guid> _clientsId =
        [
            new Guid("5925b266-906e-47e4-9ed3-e1bd92fa1f0f"),
            new Guid("7dc4a2af-305c-4ec3-810b-718157d010ae"),
            new Guid("8bee19c3-5a45-4174-b452-38f13b4e8f59"),
            new Guid("ddfb9c53-e007-4262-af3d-8026b33642cb"),
            new Guid("2d9962ea-f43f-4dbf-8ffc-4bae23866e87")
        ];

        public bool VerifyClient(Guid clientId)
        {
           return _clientsId.Contains(clientId);
        }
    }
}
