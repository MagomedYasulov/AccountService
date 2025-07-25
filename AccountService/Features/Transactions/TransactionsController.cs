using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.Models;
using AccountService.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter<ApiExceptionFilter>]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Регистрация транзакций и перевод между счетами
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create(CreateTransactionCommand command)
        {
            var transactionDto = await _mediator.Send(command);
            return Created("", transactionDto);
        }
    }
}
