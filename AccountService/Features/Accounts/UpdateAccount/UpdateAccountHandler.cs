using AccountService.Domain.Enums;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountHandler(
    AppDbContext dbContext,
    IMapper mapper) : IRequestHandler<UpdateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken);
        if (account == null)
            throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);
            
        if(account.Revoked)
            throw new ServiceException("Can`t modify account", "Revoked account can`t be modified", StatusCodes.Status404NotFound);

        if (account.Type == AccountType.Checking)
            throw new ServiceException("Can`t modify account", "Interest rate can`t be modified in checking account", StatusCodes.Status409Conflict);

        account.InterestRate = request.InterestRate;
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<AccountDto>(account);
    }
}