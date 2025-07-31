using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.GetAccountStatement;

public class GetAccountStatementHandler(
    IAccountRepository accountRepository,
    IMapper mapper) : IRequestHandler<GetAccountStatementQuery, AccountStatementDto>
{
    public async Task<AccountStatementDto> Handle(GetAccountStatementQuery request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id);
        if(account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

        var dto = mapper.Map<AccountStatementDto>(account);

        var isStartNull = request.Start == null;
        var isEndNull = request.End == null;

        dto.Transactions = dto.Transactions.Where(Predicate).ToArray();
        return dto;

        bool Predicate(TransactionStatementDto t) => (isStartNull || t.TransferTime >= request.Start) &&
                                                     (isEndNull || t.TransferTime <= request.End);
    }
}