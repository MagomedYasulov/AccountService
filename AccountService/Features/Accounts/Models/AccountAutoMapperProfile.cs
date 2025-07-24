using AccountService.Domain.Data.Entities;
using AccountService.Features.Accounts.DTOs;
using AutoMapper;

namespace AccountService.Features.Accounts.Models
{
    public class AccountAutoMapperProfile : Profile
    {
        public AccountAutoMapperProfile()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}
