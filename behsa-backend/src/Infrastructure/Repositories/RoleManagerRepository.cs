using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories;

public class RoleManagerRepository : IRoleManagerRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleManagerRepository(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }
}