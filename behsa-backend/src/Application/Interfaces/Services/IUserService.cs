using Application.DTOs;
using Application.DTOs.Identity.CreateUser;
using Application.DTOs.Identity.LoginUser;
using Application.DTOs.User;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<CreateUserResponse>> SignUp(CreateUserRequest createIdentityDto);
    Task<Result<LoginUserResponse>> Login(LoginUserRequest loginDto);
    Task<Result> ChangeRole(ChangeRoleRequest changeRoleRequest);
    Task<Result<List<GetUserResponse>>> GetAllUsersAsync();
}