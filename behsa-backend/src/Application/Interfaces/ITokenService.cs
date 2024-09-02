using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(AppUser user, string role);
}