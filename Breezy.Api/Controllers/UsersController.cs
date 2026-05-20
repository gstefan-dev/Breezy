using Breezy.Application.Users.Commands;
using Breezy.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        var result = await _mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }
    
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserCommand request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{id}/subscription")]
    public async Task<ActionResult> UpdateSubscription(Guid id, [FromBody] UpdateSubscriptionCommand request)
    {
        var command = new UpdateSubscriptionCommand(id, request.NewPlan);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}