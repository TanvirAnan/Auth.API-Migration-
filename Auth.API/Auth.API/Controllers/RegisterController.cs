using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class RegisterController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var cmd = new CreateUserCommand(request.FirstName, request.LastName, request.UserName, request.Email, request.Password);
        var result = await _mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.User.Id.Value }, result);
    }
}

public record RegisterRequest(string FirstName, string LastName, string UserName, string Email, string Password);
