using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Accounts.GetAccount;

public class GetAccountByIdHandler(AppDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetAccountByIdQuery, AccountDto>
{
    public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        if (account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

        return mapper.Map<AccountDto>(account);
    }
}