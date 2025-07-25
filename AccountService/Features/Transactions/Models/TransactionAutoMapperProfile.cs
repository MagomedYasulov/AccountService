using AccountService.Domain.Data.Entities;
using AccountService.Features.Transactions.CreateTransaction;
using AutoMapper;

namespace AccountService.Features.Transactions.Models
{
    public class TransactionAutoMapperProfile : Profile
    {
        public TransactionAutoMapperProfile()
        {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<CreateTransactionCommand, Transaction>();
        }
    }
}
