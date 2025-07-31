using AccountService.Domain.Models;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.Models;
using AccountService.Filters;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

[Route("api/v1/[controller]")]
[ApiController]
[TypeFilter<ApiExceptionFilter>]
public class TransactionsController(
    IMediator mediator,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Регистрация транзакций и перевод между счетами
    /// </summary>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(MbResult<TransactionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbResult), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MbResult<TransactionDto>>> Create(CreateTransactionDto createDto)
    {
        var command = mapper.Map<CreateTransactionCommand>(createDto);
        var transactionDto = await mediator.Send(command);
        return Created("", new MbResult<TransactionDto>(transactionDto));
    }
}