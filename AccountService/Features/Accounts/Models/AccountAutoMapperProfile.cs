using AccountService.Domain.Data.Entities;
using AutoMapper;

namespace AccountService.Features.Accounts.Models
{
    public class AccountAutoMapperProfile : Profile
    {
        public AccountAutoMapperProfile()
        {
            CreateMap<Account, AccountDto>();
            CreateMap<Account, AccountStatementDto>();
            CreateMap<Transaction, TransactionStatementDto>();
        }
    }
}
