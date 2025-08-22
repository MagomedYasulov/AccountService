using AccountService.Domain.Models;
using AccountService.Features.Health.GetReady;
using AccountService.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Health;

[Route("[controller]")]
[ApiController]
[TypeFilter<ApiExceptionFilter>]
public class HealthController(IMediator mediator) : ControllerBase
{
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
        mediator.Send(new CheckReadyQuery());
        return Ok();
    }
}