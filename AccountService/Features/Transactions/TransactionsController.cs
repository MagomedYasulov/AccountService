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
public class TransactionsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public TransactionsController(
        IMediator mediator,
        IMapper mapper)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>
    /// Регистрация транзакций и перевод между счетами
    /// </summary>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionDto createDto)
    {
        var command = _mapper.Map<CreateTransactionCommand>(createDto);
        var transactionDto = await _mediator.Send(command);
        return Created("", transactionDto);
    }
}