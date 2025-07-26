using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using AccountService.Features.Accounts.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.GetAccountStatement
{
    public class GetAccountStatementHandler : IRequestHandler<GetAccountStatementQuery, AccountStatementDto>
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public GetAccountStatementHandler(
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<AccountStatementDto> Handle(GetAccountStatementQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id);
            if(account == null)
                throw new ServiceException("Account Not Found", $"Account with id {request.Id} not found", StatusCodes.Status404NotFound);

            return _mapper.Map<AccountStatementDto>(account);
        }
    }
}
