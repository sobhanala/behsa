using Application.DTOs.Identity.CreateUser;
using Application.DTOs.Identity.LoginUser;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Mappers;
using Application.Services.DomainService;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Web.DTOs.User.Login;
using Web.Mappers;

namespace test.Application.UnitTests.Services.DomainService;

public class UserServiceTests
{
    private readonly IUserManagerRepository _userManagerRepository;
    private readonly IRoleManagerRepository _roleManagerRepository;
    private readonly ITokenService _tokenService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userManagerRepository = Substitute.For<IUserManagerRepository>();
        _roleManagerRepository = Substitute.For<IRoleManagerRepository>();
        _tokenService = Substitute.For<ITokenService>();
        _userService = new UserService(_userManagerRepository, _roleManagerRepository, _tokenService);
    }

    // Signup Tests
    [Fact]
    public async Task SignUpUser_WhenRoleDoesNotExist_ReturnsFailResult()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            UserName = "MobinBarfi",
            Password = "Abc@123",
            Email = "mobinbr99@gmail.com",
            Role = "NonExistentRole"
        };

        _roleManagerRepository.RoleExistsAsync(createUserRequest.Role).Returns(Task.FromResult(false));

        // Act
        var result = await _userService.SignUp(createUserRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Role does not exist.", result.Message);
    }
    
    [Fact]
    public async Task SignUpUser_WhenUserCreationFails_ReturnsFailResult()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            UserName = "MobinBarfi",
            Password = "Abc@123",
            Email = "mobinbr99@gmail.com",
            Role = "Admin"
        };

        _roleManagerRepository.RoleExistsAsync(createUserRequest.Role).Returns(Task.FromResult(true));

        _userManagerRepository.CreateAsync(Arg.Any<AppUser>(), createUserRequest.Password)
            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User creation failed" })));

        // Act
        var result = await _userService.SignUp(createUserRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("User creation failed", result.Message);
    }
    
    [Fact]
    public async Task SignUpUser_WhenRoleAssignmentFails_ReturnsFailResult()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            UserName = "MobinBarfi",
            Password = "Abc@123",
            Email = "mobinbr99@gmail.com",
            Role = "Admin"
        };

        _roleManagerRepository.RoleExistsAsync(createUserRequest.Role).Returns(Task.FromResult(true));

        _userManagerRepository.CreateAsync(Arg.Any<AppUser>(), createUserRequest.Password)
            .Returns(Task.FromResult(IdentityResult.Success));

        _userManagerRepository.SetRoleAsync(Arg.Any<AppUser>(), createUserRequest.Role)
            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" })));

        // Act
        var result = await _userService.SignUp(createUserRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Role assignment failed", result.Message);
    }

    [Fact]
    public async Task SignUpUser_WhenUserIsCreatedAndRoleAssignedSuccessfully_ReturnsSuccessResult()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            UserName = "MobinBarfi",
            Password = "Abc@123",
            Email = "mobinbr99@gmail.com",
            Role = "Admin"
        };

        var appUser = new AppUser { UserName = createUserRequest.UserName };
        var identityResultSuccess = IdentityResult.Success;

        _roleManagerRepository.RoleExistsAsync(createUserRequest.Role)
            .Returns(Task.FromResult(true));

        _userManagerRepository.CreateAsync(Arg.Is<AppUser>(u => u.UserName == createUserRequest.UserName), createUserRequest.Password)
            .Returns(Task.FromResult(identityResultSuccess));

        _userManagerRepository.SetRoleAsync(Arg.Is<AppUser>(u => u.UserName == createUserRequest.UserName), createUserRequest.Role)
            .Returns(Task.FromResult(identityResultSuccess));

        var expectedResponse = new CreateUserResponse
        {
            UserName = createUserRequest.UserName,
            Email = createUserRequest.Email,
            FirstName = "",
            LastName = "",
            Role = createUserRequest.Role
        };

        // Act
        var result = await _userService.SignUp(createUserRequest);

        // Assert
        Assert.True(result.Succeed);
        Assert.NotNull(result.Value);

        var actualResponse = result.Value;

        Assert.Equal(expectedResponse.UserName, actualResponse.UserName);
        Assert.Equal(expectedResponse.Email, actualResponse.Email);
        Assert.Equal(expectedResponse.FirstName, actualResponse.FirstName);
        Assert.Equal(expectedResponse.LastName, actualResponse.LastName);
        Assert.Equal(expectedResponse.Role, actualResponse.Role);
    }
    
    // Login Tests
    [Fact]
    public async Task Login_WhenUsernameNotFound_ReturnsFailResult()
    {
        // Arrange
        var loginUserRequest = new LoginUserRequest
        {
            UserName = "NonExistentUser",
            Password = "Abc@123"
        };

        _userManagerRepository.FindByNameAsync(loginUserRequest.UserName).Returns(Task.FromResult<AppUser?>(null));

        // Act
        var result = await _userService.Login(loginUserRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Invalid username/email!", result.Message);
    }
    
    [Fact]
    public async Task Login_WhenPasswordIncorrect_ReturnsFailResult()
    {
        // Arrange
        var loginUserRequest = new LoginUserRequest
        {
            UserName = "MobinBarfi",
            Password = "WrongPassword"
        };

        var appUser = new AppUser();
        _userManagerRepository.FindByNameAsync(loginUserRequest.UserName).Returns(Task.FromResult(appUser));
        _userManagerRepository.CheckPasswordAsync(appUser, loginUserRequest.Password).Returns(Task.FromResult(false));

        // Act
        var result = await _userService.Login(loginUserRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Username/Email not found and/or password incorrect", result.Message);
    }

    [Fact]
    public async Task Login_WhenLoginSucceeds_ReturnsSuccessResult()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UserName = "MobinBarfi",
            Password = "Abc@123"
        };

        var loginRequest = loginDto.ToLoginUserRequest();
        var appUser = new AppUser
        {
            UserName = loginDto.UserName,
            Email = "mobinbr99@gmail.com"
        };
        var role = "Admin";
        var token = "valid.jwt.token";

        var loginUserResponse = new LoginUserResponse
        {
            UserName = appUser.UserName,
            Email = appUser.Email,
            Token = token
        };

        // Set up the mock behavior
        _userManagerRepository.FindByNameAsync(loginRequest.UserName)
            .Returns(Task.FromResult(appUser));
        _userManagerRepository.CheckPasswordAsync(appUser, loginRequest.Password)
            .Returns(Task.FromResult(true));
        _userManagerRepository.GetRoleAsync(appUser)
            .Returns(Task.FromResult(role));
        _tokenService.GenerateToken(appUser, role)
            .Returns(token);

        // Act
        var result = await _userService.Login(loginRequest);

        // Assert
        Assert.True(result.Succeed);
        var loginResponse = result.Value;

        Assert.NotNull(loginResponse);
        Assert.Equal(loginUserResponse.UserName, loginResponse.UserName);
        Assert.Equal(loginUserResponse.Email, loginResponse.Email);
        Assert.Equal(loginUserResponse.Token, loginResponse.Token);
    }
    
    // ChangeRole Tests
    [Fact]
    public async Task ChangeRole_WhenRoleDoesNotExist_ReturnsFailResult()
    {
        // Arrange
        var changeRoleRequest = new ChangeRoleRequest
        {
            UserName = "MobinBarfi",
            Role = "NonExistentRole"
        };

        _roleManagerRepository.RoleExistsAsync(changeRoleRequest.Role).Returns(Task.FromResult(false));

        // Act
        var result = await _userService.ChangeRole(changeRoleRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("role does not exist", result.Message);
    }
    
    [Fact]
    public async Task ChangeRole_WhenUserDoesNotExist_ReturnsFailResult()
    {
        // Arrange
        var changeRoleRequest = new ChangeRoleRequest
        {
            UserName = "NonExistentUser",
            Role = "Admin"
        };

        _roleManagerRepository.RoleExistsAsync(changeRoleRequest.Role).Returns(Task.FromResult(true));
        _userManagerRepository.FindByNameAsync(changeRoleRequest.UserName).Returns(Task.FromResult<AppUser?>(null));

        // Act
        var result = await _userService.ChangeRole(changeRoleRequest);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Invalid username", result.Message);
    }

    [Fact]
    public async Task ChangeRole_WhenOperationSucceeds_ReturnsSuccessResult()
    {
        // Arrange
        var changeRoleRequest = new ChangeRoleRequest
        {
            UserName = "MobinBarfi",
            Role = "Admin"
        };

        var appUser = new AppUser();

        _roleManagerRepository.RoleExistsAsync(changeRoleRequest.Role).Returns(Task.FromResult(true));
        _userManagerRepository.FindByNameAsync(changeRoleRequest.UserName).Returns(Task.FromResult(appUser));
        _userManagerRepository.ChangeRoleAsync(appUser, changeRoleRequest.Role).Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var result = await _userService.ChangeRole(changeRoleRequest);

        // Assert
        Assert.True(result.Succeed);
    }
    
    // GetUsers Tests
    [Fact]
    public async Task GetUsersAsync_ReturnsUserListWithRoles()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new AppUser { UserName = "User1", Email = "user1@example.com" },
            new AppUser { UserName = "User2", Email = "user2@example.com" }
        };

        var roles = new List<string> { "Admin", "User" };

        var userResponses = new List<GetUserResponse>
        {
            new GetUserResponse { UserName = "User1", Email = "user1@example.com", Role = "Admin" },
            new GetUserResponse { UserName = "User2", Email = "user2@example.com", Role = "User" }
        };

        _userManagerRepository.GetUsersAsync()
            .Returns(Task.FromResult(users));

        _userManagerRepository.GetRoleAsync(users[0])
            .Returns(Task.FromResult(roles[0]));

        _userManagerRepository.GetRoleAsync(users[1])
            .Returns(Task.FromResult(roles[1]));

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Succeed, "Result should be successful");
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);

        var user1Response = result.Value.SingleOrDefault(r => r.UserName == "User1");
        Assert.NotNull(user1Response);
        Assert.Equal("Admin", user1Response.Role);

        var user2Response = result.Value.SingleOrDefault(r => r.UserName == "User2");
        Assert.NotNull(user2Response);
        Assert.Equal("User", user2Response.Role);
    }

}