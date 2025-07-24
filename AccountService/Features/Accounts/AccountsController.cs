using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.DTOs;
using AccountService.Features.Accounts.GetAccount;
using AccountService.Features.Accounts.GetAccounts;
using AccountService.Filters;
using MediatR;
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

        [HttpPost]
        public async Task<ActionResult<AccountDto>> Get(CreateAccountCommand command)
        {
            var accountDto = await _mediator.Send(command);
            return Ok(accountDto);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AccountDto>> Get(Guid id)
        {
            var accountDto = await _mediator.Send(new GetAccountByIdQuery { Id = id });
            return Ok(accountDto);
        }

        [HttpGet]
        public async Task<ActionResult<AccountDto>> Get([FromQuery] Guid? ownerId)
        {
            var accountDto = await _mediator.Send(new GetAccountsQuery { OwnerId = ownerId });
            return Ok(accountDto);
        }
    }
}
