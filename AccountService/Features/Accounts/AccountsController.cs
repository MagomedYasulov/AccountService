using AccountService.Features.Accounts.DTOs;
using AccountService.Features.Accounts.GetAccount;
using AccountService.Filters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter<ApiExceptionFilter>]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;   
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AccountDto>> Get(Guid id)
        {
            var accountDto = await _mediator.Send(new GetAccountByIdQuery { Id = id });
            return Ok(accountDto);
        }
    }
}
