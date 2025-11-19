using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Application.Queries;
public class LoginQueryHandler : IQueryHandler<LoginQuery, LoginResult>
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginQueryHandler> _logger;

    public LoginQueryHandler(IUserRepository userRepository, IConfiguration config, ILogger<LoginQueryHandler> logger)
    {
        _userRepository = userRepository;
        _config = config;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(request.UserName, request.Password);

        if (user is null)
        {
            throw new NotFoundException("User", request.UserName);
        }

        _logger.LogInformation("User with id {userId} logged in at {now}", user.Id, DateTime.UtcNow);

        var token = GenerateToken(user.UserName, user.Email.Value, user.Id.Value.ToString());

        return new LoginResult(user, token);
    }

    private string GenerateToken(string userName, string email, string userId)
    {
        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];
        var key = _config["JwtSettings:Key"];
        var expiresMinutes = int.TryParse(_config["JwtSettings:ExpiresMinutes"], out var m) ? m :60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userName),
            new(JwtRegisteredClaimNames.Email, email),
            new("uid", userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
