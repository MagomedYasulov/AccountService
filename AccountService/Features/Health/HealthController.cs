using AccountService.Domain.Models;
using AccountService.Features.Health.GetReady;
using AccountService.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Health
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter<ApiExceptionFilter>]
    public class HealthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HealthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("live")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Live()
        {
            return Ok();
        }

        [HttpGet("ready")]
        [ProducesResponseType(typeof(MbError), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ready()
        {
            _mediator.Send(new CheckReadyQuery());
            return Ok();
        }
    }
}
