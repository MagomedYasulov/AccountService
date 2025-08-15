using AccountService.Domain.Models;
using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.DeleteAccount;
using AccountService.Features.Accounts.GetAccount;
using AccountService.Features.Accounts.GetAccounts;
using AccountService.Features.Accounts.GetAccountStatement;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.UpdateAccount;
using AccountService.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
[TypeFilter<ApiExceptionFilter>]
public class AccountsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Создание счета
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(MbResult<AccountDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MbResult<AccountDto>>> Create(CreateAccountCommand command)
    {
        var accountDto = await mediator.Send(command);
        return Created("", new MbResult<AccountDto>(accountDto));
    }

    /// <summary>
    /// Получение счета
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MbResult<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MbResult<AccountDto>>> Get(Guid id)
    {
        var accountDto = await mediator.Send(new GetAccountByIdQuery { Id = id });
        return Ok(new MbResult<AccountDto>(accountDto));
    }

    /// <summary>
    /// Получение выписки по счету
    /// </summary>
    /// <param name="id"></param>
    /// <param name="start">дата начала</param>
    /// <param name="end">дата конца</param>
    /// <returns></returns>
    [HttpGet("{id:guid}/statement")]
    [ProducesResponseType(typeof(MbResult<AccountStatementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MbResult<AccountStatementDto>>> GetStatement(Guid id, DateTime? start = null, DateTime? end = null)
    {
        var statementDto = await mediator.Send(new GetAccountStatementQuery { Id = id, Start = start, End = end });
        return Ok(new MbResult<AccountStatementDto>(statementDto));
    }

    /// <summary>
    /// Получение счетов
    /// </summary>
    /// <param name="ownerId">получить счета клиента</param>
    /// <param name="revoked">получить все счета / анулированные / не анулированные</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MbResult<AccountDto[]>), StatusCodes.Status200OK)]
    public async Task<ActionResult<MbResult<AccountDto[]>>> Get([FromQuery] Guid? ownerId, [FromQuery] bool? revoked)
    {
        var accountsDto = await mediator.Send(new GetAccountsQuery { OwnerId = ownerId, Revoked = revoked });
        return Ok(new MbResult<AccountDto[]>(accountsDto));
    }

    /// <summary>
    /// Изменение счета
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}/interestRate")]
    [ProducesResponseType(typeof(MbResult<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MbResult<AccountDto>>> Update(Guid id, UpdateAccountDto command)
    {
        var accountDto = await mediator.Send(new UpdateAccountCommand { Id = id, InterestRate = command.InterestRate});
        return Ok(new MbResult<AccountDto>(accountDto));
    }

    /// <summary>
    /// Удаление счета
    /// </summary>
    /// <param name="id">id удаляемого счета</param>
    /// <param name="isSoft">Полное / мягкое удаление</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MbError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] bool isSoft)
    {
        await mediator.Send(new DeleteAccountCommand { Id = id, IsSoft = isSoft });
        return Ok();
    }
}