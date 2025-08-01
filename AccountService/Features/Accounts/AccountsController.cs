﻿using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.DeleteAccount;
using AccountService.Features.Accounts.GetAccount;
using AccountService.Features.Accounts.GetAccounts;
using AccountService.Features.Accounts.GetAccountStatement;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.UpdateAccount;
using AccountService.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

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
    public async Task<ActionResult<AccountDto>> Create(CreateAccountCommand command)
    {
        var accountDto = await mediator.Send(command);
        return Created("", accountDto);
    }

    /// <summary>
    /// Получение счета
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountDto>> Get(Guid id)
    {
        var accountDto = await mediator.Send(new GetAccountByIdQuery { Id = id });
        return Ok(accountDto);
    }

    /// <summary>
    /// Получение выписки по счету
    /// </summary>
    /// <param name="id"></param>
    /// <param name="start">начало получения</param>
    /// <param name="end">конец получения</param>
    /// <returns></returns>
    [HttpGet("{id:guid}/statement")]
    public async Task<ActionResult<AccountStatementDto>> GetStatement(Guid id, DateTime? start = null, DateTime? end = null)
    {
        var statementDto = await mediator.Send(new GetAccountStatementQuery { Id = id, Start = start, End = end });
        return Ok(statementDto);
    }

    /// <summary>
    /// Получение счетов
    /// </summary>
    /// <param name="ownerId">получить счета клиента</param>
    /// <param name="revoked">получить все счета / анулированные / не анулированные</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<AccountDto[]>> Get([FromQuery] Guid? ownerId, [FromQuery] bool? revoked)
    {
        var accountsDto = await mediator.Send(new GetAccountsQuery { OwnerId = ownerId, Revoked = revoked });
        return Ok(accountsDto);
    }

    /// <summary>
    /// Изменение счета
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}/interestRate")]
    public async Task<ActionResult<AccountDto>> Update(Guid id, UpdateAccountDto command)
    {
        var accountDto = await mediator.Send(new UpdateAccountCommand { Id = id, InterestRate = command.InterestRate});
        return Ok(accountDto);
    }

    /// <summary>
    /// Удаление счета
    /// </summary>
    /// <param name="id">id удаляемого счета</param>
    /// <param name="isSoft">Полное / мягкое удаление</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, [FromQuery] bool isSoft)
    {
        await mediator.Send(new DeleteAccountCommand { Id = id, IsSoft = isSoft });
        return Ok();
    }
}