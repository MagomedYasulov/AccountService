using AccountService.Domain.Data.Repositories;
using AccountService.Exceptions;
using AccountService.Features.Accounts.DTOs;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Accounts.GetAccount
{
    public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountByIdHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.Id);
            if (account == null)
                throw new ServiceException("Account Not Found", $"Account wiht id {request.Id} not found", StatusCodes.Status404NotFound);

            return _mapper.Map<AccountDto>(account);
        }
    }
}
