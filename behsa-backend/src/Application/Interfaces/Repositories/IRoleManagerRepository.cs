namespace Application.Interfaces.Repositories;

public interface IRoleManagerRepository
{
    Task<bool> RoleExistsAsync(string roleName);
}