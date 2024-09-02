using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserManagerRepository : IUserManagerRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UserManagerRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> SetRoleAsync(AppUser user, string role)
    {
        if(_userManager.GetRolesAsync(user).Result.Count > 0)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User already has a role" });
        }
        return await _userManager.AddToRoleAsync(user, role);
    }
    
    public async Task<IdentityResult> ChangeRoleAsync(AppUser user, string newRole)
    {
        var currentRole = await GetRoleAsync(user);
        if (currentRole == newRole)
        {
            return IdentityResult.Success;
        }
        
        var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
        if (!removeRoleResult.Succeeded)
        {
            return removeRoleResult;
        }

        return await _userManager.AddToRoleAsync(user, newRole);
    }

    public async Task<AppUser?> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<AppUser?> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<string> GetRoleAsync(AppUser user)
    {
        return (await _userManager.GetRolesAsync(user)).Single();
    }

    public async Task<bool> CheckPasswordAsync(AppUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<List<AppUser>> GetUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }
}