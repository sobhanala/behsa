using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces.Repositories;

public interface IUserManagerRepository
{ 
    Task<IdentityResult> CreateAsync(AppUser user, string password);
    Task<IdentityResult> SetRoleAsync(AppUser user, string role);
    Task<IdentityResult> ChangeRoleAsync(AppUser user, string newRole);
    Task<AppUser?> FindByNameAsync(string userName);
    Task<AppUser?> FindByEmailAsync(string email);
    Task<AppUser?> FindByIdAsync(string userId);
    Task<IdentityResult> UpdateAsync(AppUser user);
    Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
    Task<string> GetRoleAsync(AppUser user);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
    Task<List<AppUser>> GetUsersAsync();
}