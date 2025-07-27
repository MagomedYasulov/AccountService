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

        return mapper.Map<AccountStatementDto>(account);
    }
}