using AccountService.Features.Accounts.Models;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace AccountService.Features.Accounts.UpdateAccount
{
    public class UpdateAccountCommand : IRequest<AccountDto>
    {
        [SwaggerSchema(ReadOnly = true)]
        public Guid Id { get; set; }
        public decimal? InterestRate { get; set; }
    }
}
