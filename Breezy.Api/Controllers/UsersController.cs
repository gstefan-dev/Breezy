using Breezy.Application.Users.Commands;
using Breezy.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        var result = await _mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserCommand request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("subscription")]
    public async Task<ActionResult> UpdateSubscription([FromBody] UpdateSubscriptionCommand request)
    {
        var command = new UpdateSubscriptionCommand(CurrentUserId, request.NewPlan);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}