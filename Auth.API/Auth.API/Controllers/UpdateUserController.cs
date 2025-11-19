using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UpdateUserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        // Ensure JWT belongs to the same user being updated: subject must match username
        var tokenUserName =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(tokenUserName))
        {
            return Unauthorized();
        }
        if (!string.Equals(tokenUserName, request.UserName, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var cmd = new UpdateUserCommand(
            request.FirstName,
            request.LastName,
            request.UserName,
            request.Email,
            request.Password);
        var result = await _mediator.Send(cmd, cancellationToken);
        return Ok(result);
    }
}

public record UpdateUserRequest(string FirstName, string LastName, string UserName, string Email, string Password);
