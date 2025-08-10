using AccountService.Domain.Data.Entities;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AccountService.Features.Accounts.GetAccountStatement;

public class GetAccountStatementHandler(
    AppDbContext dbContext,
    IMapper mapper) : IRequestHandler<GetAccountStatementQuery, AccountStatementDto>
{
    public async Task<AccountStatementDto> Handle(GetAccountStatementQuery request, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        if(account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

        var isStartNull = request.Start == null;
        var isEndNull = request.End == null;

        Expression<Func<Transaction, bool>> predicate = t =>  (t.AccountId == request.Id || t.CounterpartyAccountId == request.Id) &&
                                                                (isStartNull || t.TransferTime >= request.Start) &&
                                                                (isEndNull || t.TransferTime <= request.End);

        var transactions = await dbContext.Transactions.AsNoTracking().Where(predicate).ToArrayAsync(cancellationToken: cancellationToken);
        
        account.Transactions = transactions.Where(t => t.AccountId == account.Id).ToList();
        account.CounterpartyTransactions = transactions.Where(t => t.CounterpartyAccountId == account.Id).ToList();

        var dto = mapper.Map<AccountStatementDto>(account);
        return dto;
    }
}