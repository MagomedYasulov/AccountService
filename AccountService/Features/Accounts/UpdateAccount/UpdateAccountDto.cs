using AccountService.Features.Accounts.Models;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount
{
    public class UpdateAccountDto
    {
        public decimal? InterestRate { get; set; }
    }
}
