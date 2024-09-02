using Application.DTOs;
using Application.DTOs.Identity.CreateUser;
using Application.DTOs.Identity.LoginUser;
using Application.DTOs.User;
using Application.ExtensionMethods;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappers;
using Domain.Entities;

namespace Application.Services.DomainService;

public class UserService : IUserService
{
    private readonly IUserManagerRepository _userManagerRepository;
    private readonly IRoleManagerRepository _roleManagerRepository;
    private readonly ITokenService _tokenService;
    public UserService(IUserManagerRepository userManagerRepository,
        IRoleManagerRepository roleManagerRepository,
        ITokenService tokenService)
    {
        _userManagerRepository = userManagerRepository;
        _roleManagerRepository = roleManagerRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<CreateUserResponse>> SignUp(CreateUserRequest createUserRequest)
    {
        try
        {
            if (!await _roleManagerRepository.RoleExistsAsync(createUserRequest.Role))
            {
                return Result<CreateUserResponse>.Fail("Role does not exist.");
            }

            var appUser = createUserRequest.ToAppUser();

            var appUserResult = await _userManagerRepository.CreateAsync(appUser, createUserRequest.Password);
            if (!appUserResult.Succeeded)
            {
                return Result<CreateUserResponse>.Fail(appUserResult.Errors.FirstMessage());
            }

            var roleResult = await _userManagerRepository.SetRoleAsync(appUser, createUserRequest.Role);
            if (!roleResult.Succeeded)
            {
                return Result<CreateUserResponse>.Fail(roleResult.Errors.FirstMessage());
            }

            return Result<CreateUserResponse>.Ok(appUser.ToCreateUserResponse(createUserRequest.Role));
        }
        catch (Exception ex)
        {
            return Result<CreateUserResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<LoginUserResponse>> Login(LoginUserRequest loginUserRequest)
    {
        try
        {
            AppUser? appUser;

            if (!string.IsNullOrEmpty(loginUserRequest.UserName))
            {
                appUser = await _userManagerRepository.FindByNameAsync(loginUserRequest.UserName);
            }
            else if (!string.IsNullOrEmpty(loginUserRequest.Email))
            {
                appUser = await _userManagerRepository.FindByEmailAsync(loginUserRequest.Email);
            }
            else
            {
                return Result<LoginUserResponse>.Fail("You should enter email or username!");
            }

            if (appUser is null) return Result<LoginUserResponse>.Fail("Invalid username/email!");

            var succeed = await _userManagerRepository.CheckPasswordAsync(appUser, loginUserRequest.Password);

            if (!succeed) return Result<LoginUserResponse>.Fail("Username/Email not found and/or password incorrect");
        
            var role = await _userManagerRepository.GetRoleAsync(appUser);
            var token = _tokenService.GenerateToken(appUser, role);

            return Result<LoginUserResponse>.Ok(appUser.ToLoginUserResponse(role, token));
        }
        catch (Exception ex)
        {
            return Result<LoginUserResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result> ChangeRole(ChangeRoleRequest request)
    {
        try
        {
            if (!await _roleManagerRepository.RoleExistsAsync(request.Role))
            {
                return Result.Fail("role does not exist");
            }
            AppUser? appUser = await _userManagerRepository.FindByNameAsync(request.UserName);

            if (appUser is null) return Result<LoginUserResponse>.Fail("Invalid username");

            var result = await _userManagerRepository.ChangeRoleAsync(appUser, request.Role);
        
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.FirstMessage());
        }
        catch (Exception ex)
        {
            return Result.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<List<GetUserResponse>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userManagerRepository.GetUsersAsync();
            var userWithRoles = new List<GetUserResponse>();

            foreach (var user in users)
            {
                var role = await _userManagerRepository.GetRoleAsync(user);
                var userResponse = user.ToGetUserResponse(role);
                userWithRoles.Add(userResponse);
            }

            return Result<List<GetUserResponse>>.Ok(userWithRoles);
        }
        catch (Exception ex)
        {
            return Result<List<GetUserResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}