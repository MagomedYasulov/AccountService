using AccountService.Application.Abstractions;
using AccountService.Domain.Data.Entities;
using AccountService.Domain.Data.Repositories;
using AccountService.Domain.Enums;
using AccountService.Exceptions;
using AccountService.Features.Transactions.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrencyService _currencyService;

        public CreateTransactionHandler(
            IAccountRepository accountRepository,
            ICurrencyService currencyService,
            IMapper mapper)
        {
            _mapper = mapper;
            _accountRepository = accountRepository;
            _currencyService = currencyService;
        }

        public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            if(!await _currencyService.IsSupportedCurrency(request.CurrencyCode))
                throw new ServiceException("Not Supported Currency Type", $"Currency type {request.CurrencyCode} not supported", StatusCodes.Status409Conflict);

            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            AccountValidation(request.AccountId, account, request, TransactionType.Debit);

            Account? counterpartyAccount = null;
            if(request.CounterpartyAccountId != null)
            {
                counterpartyAccount = await _accountRepository.GetByIdAsync(request.CounterpartyAccountId.Value);
                AccountValidation(request.CounterpartyAccountId.Value, counterpartyAccount, request, TransactionType.Credit);
            }


            if (request.Type == TransactionType.Debit)
            {
                account!.Balance -= request.Sum;
                if(counterpartyAccount != null)
                    counterpartyAccount.Balance += request.Sum;
            }             
            else
            {
                account!.Balance += request.Sum;
                if (counterpartyAccount != null)
                    counterpartyAccount.Balance -= request.Sum;
            }

            var transaction = _mapper.Map<Transaction>(request);
            transaction.TransferTime = DateTime.UtcNow;

            await _accountRepository.CreateTransactionAsync(transaction);
            await _accountRepository.UpdateAsync(account);
            if(counterpartyAccount != null)
                await _accountRepository.UpdateAsync(counterpartyAccount);

            return _mapper.Map<TransactionDto>(transaction);
        }

        private static void AccountValidation(Guid accountId, Account? account, CreateTransactionCommand request, TransactionType operationType)
        {
            if (account == null)
                throw new ServiceException("Account Not Found", $"Account wiht id {accountId} not found", StatusCodes.Status404NotFound);

            if (account.Revoked)
                throw new ServiceException("Account Revoked", $"Account wiht id {accountId} is revoked", StatusCodes.Status404NotFound);

            if (request.CurrencyCode != account.CurrencyCode)
                throw new ServiceException("Currency doesn't match", $"The currency of the account {accountId} and the transaction do not match", StatusCodes.Status409Conflict);

            if (request.Type == operationType && account.Balance < request.Sum)
                throw new ServiceException("Insufficient funds", $"Insufficient funds for the operation", StatusCodes.Status409Conflict);
        }
    }
}
