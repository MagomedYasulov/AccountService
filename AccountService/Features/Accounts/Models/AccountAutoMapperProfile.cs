using AccountService.Domain.Data.Entities;
using AutoMapper;

namespace AccountService.Features.Accounts.Models;

public class AccountAutoMapperProfile : Profile
{
    public AccountAutoMapperProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<Transaction, TransactionStatementDto>();

        CreateMap<Account, AccountStatementDto>()
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .AfterMap((src, dest, ctx) =>
            {
                var normal = src.Transactions
                    .Select(tx => ctx.Mapper.Map<TransactionStatementDto>(tx));

                var counterparty = src.CounterpartyTransactions
                    .Select(tx =>
                    {
                        var dto = ctx.Mapper.Map<TransactionStatementDto>(tx);
                        dto.Type = tx.CounterpartyType;
                        dto.CounterpartyAccountId = tx.AccountId;
                        return dto;
                    });

                dest.Transactions = normal.Concat(counterparty).OrderBy(t => t.TransferTime).ToArray();
            });
    }
}