using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Web.AccessControl;

namespace Web.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _symmetricSecurityKey;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _symmetricSecurityKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"])
        );
    }

    public string GenerateToken(AppUser user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(Claims.UserId, user.Id),
            new Claim(Claims.Role, role)
        };

        var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(8),
            SigningCredentials = credentials,
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}