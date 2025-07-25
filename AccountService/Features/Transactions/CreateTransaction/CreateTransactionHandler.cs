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
            if (account == null)
                throw new ServiceException("Account Not Found", $"Account wiht id {request.AccountId} not found", StatusCodes.Status404NotFound);

            if(request.CurrencyCode != account.CurrencyCode)
                throw new ServiceException("Currency doesn't match", $"The currency of the account and the transaction do not match", StatusCodes.Status409Conflict);

            if (request.Type == TransactionType.Debit && account.Balance < request.Sum)
                throw new ServiceException("Insufficient funds", $"Insufficient funds for the operation", StatusCodes.Status409Conflict);

            Account? counterpartyAccount = null;
            if(request.CounterpartyAccountId != null)
            {
                counterpartyAccount = await _accountRepository.GetByIdAsync(request.CounterpartyAccountId.Value);
                if(counterpartyAccount == null)
                    throw new ServiceException("Counterparty Account Not Found", $" Counterparty account wiht id {request.AccountId} not found", StatusCodes.Status404NotFound);

                if (request.CurrencyCode != counterpartyAccount.CurrencyCode)
                    throw new ServiceException("Currency doesn't match", $"The currency of the counterparty account and the transaction do not match", StatusCodes.Status409Conflict);

                if (request.Type == TransactionType.Credit && counterpartyAccount.Balance < request.Sum)
                    throw new ServiceException("Insufficient funds", $"Insufficient funds for the operation", StatusCodes.Status409Conflict);
            }


            if (request.Type == TransactionType.Debit)
            {
                account.Balance -= request.Sum;
                if(counterpartyAccount != null)
                    counterpartyAccount.Balance += request.Sum;
            }             
            else
            {
                account.Balance += request.Sum;
                if (counterpartyAccount != null)
                    counterpartyAccount.Balance -= request.Sum;
            }

            var transaction = _mapper.Map<Transaction>(request);
            transaction.TransferTime = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);
            await _accountRepository.CreateTransactionAsync(transaction);

            if(counterpartyAccount != null)
            {
                var counterpartyTransaction = new Transaction()
                {
                    AccountId = counterpartyAccount.Id,
                    CounterpartyAccountId = account.Id,
                    CurrencyCode = request.CurrencyCode,
                    Description = request.Description,
                    Sum = request.Sum,
                    Type = request.Type == TransactionType.Debit ? TransactionType.Credit : TransactionType.Debit,
                    TransferTime = DateTime.UtcNow,
                };
                await _accountRepository.UpdateAsync(counterpartyAccount);
                await _accountRepository.CreateTransactionAsync(counterpartyTransaction);
            }

            return _mapper.Map<TransactionDto>(transaction);
        }
    }
}
