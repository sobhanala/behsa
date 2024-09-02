using Application.DTOs.Profile;
using Application.DTOs.Profile.EditProfile;
using Application.DTOs.Profile.GetProfileInfo;
using Application.Interfaces.Repositories;
using Application.Services.DomainService;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace test.Application.UnitTests.Services.DomainService;

public class ProfileServiceTests
{
    private readonly IUserManagerRepository _userManagerMock;
    private readonly ProfileService _profileService;

    public ProfileServiceTests()
    {
        _userManagerMock = Substitute.For<IUserManagerRepository>();
        _profileService = new ProfileService(_userManagerMock);
    }
    
    [Fact]
    public async Task EditProfileInfo_WhenUserNotFound_ReturnsFail()
    {
        // Arrange
        var request = new EditProfileInfoRequest
        {
            UserId = "1",
            UserName = "MobinBarfi",
            FirstName = "Mobin",
            LastName = "Barfi"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns((AppUser?)null);

        // Act
        var result = await _profileService.EditProfileInfo(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("User not found!", result.Message);
    }
    
    [Fact]
    public async Task EditProfileInfo_WhenUsernameIsReserved_ReturnsFail()
    {
        // Arrange
        var request = new EditProfileInfoRequest
        {
            UserId = "1",
            UserName = "MobinBarfi",
            FirstName = "Mobin",
            LastName = "Barfi"
        };

        var user1 = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfiHajiAbadi"
        };
        
        var user2 = new AppUser
        {
            Id = "2",
            UserName = "MobinBarfi"
        };
        
        _userManagerMock.FindByIdAsync(request.UserId).Returns(user1);
        _userManagerMock.FindByNameAsync(request.UserName).Returns(user2);

        // Act
        var result = await _profileService.EditProfileInfo(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Username is already reserved by another user!", result.Message);
    }
    
    [Fact]
    public async Task EditProfileInfo_WhenUpdateFails_ReturnsFail()
    {
        // Arrange
        var request = new EditProfileInfoRequest
        {
            UserId = "1",
            UserName = "MobinBarfi",
            FirstName = "Mobin",
            LastName = "Barfi"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfi"
        };
        
        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.UpdateAsync(user).Returns(IdentityResult.Failed(new IdentityError { Description = "Update failed!" }));

        // Act
        var result = await _profileService.EditProfileInfo(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Update failed!", result.Message);
    }
    
    [Fact]
    public async Task EditProfileInfo_WhenUsernameIsNoChanged_ReturnsOk()
    {
        // Arrange
        var request = new EditProfileInfoRequest
        {
            UserId = "1",
            UserName = "MobinBarfi",
            FirstName = "Mobin",
            LastName = "Barfi"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfi"
        };
        
        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.UpdateAsync(user).Returns(IdentityResult.Success);

        // Act
        var result = await _profileService.EditProfileInfo(request);

        // Assert
        Assert.True(result.Succeed);
        Assert.Equal("MobinBarfi", result.Value.UserName);
        Assert.Equal("Mobin", result.Value.FirstName);
        Assert.Equal("Barfi", result.Value.LastName);
    }

    [Fact]
    public async Task EditProfileInfo_WhenAllDataIsChangedCorrectly_ReturnsOk()
    {
        // Arrange
        var request = new EditProfileInfoRequest
        {
            UserId = "1",
            UserName = "NewMobinBarfi",
            FirstName = "NewMobin",
            LastName = "NewBarfi"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "OldMobinBarfi",
            FirstName = "OldMobin",
            LastName = "OldBarfi"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.FindByNameAsync(request.UserName).Returns((AppUser?)null);  // No other user with the new username
        _userManagerMock.UpdateAsync(user).Returns(IdentityResult.Success);

        // Act
        var result = await _profileService.EditProfileInfo(request);

        // Assert
        Assert.True(result.Succeed);
        Assert.Equal("NewMobinBarfi", result.Value.UserName);
        Assert.Equal("NewMobin", result.Value.FirstName);
        Assert.Equal("NewBarfi", result.Value.LastName);
    }
    
    [Fact]
    public async Task GetProfileInfo_WhenUserNotFound_ReturnsFail()
    {
        // Arrange
        var request = new GetProfileInfoRequest
        {
            UserId = "1"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns((AppUser?)null);

        // Act
        var result = await _profileService.GetProfileInfo(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("User not found!", result.Message);
    }
    
    [Fact]
    public async Task GetProfileInfo_WhenSuccess_ReturnsOk()
    {
        // Arrange
        var request = new GetProfileInfoRequest
        {
            UserId = "1"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfi",
            FirstName = "Mobin",
            LastName = "Barfi"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.GetRoleAsync(user).Returns("Admin");

        // Act
        var result = await _profileService.GetProfileInfo(request);

        // Assert
        Assert.True(result.Succeed);
        Assert.Equal("MobinBarfi", result.Value.UserName);
        Assert.Equal("Mobin", result.Value.FirstName);
        Assert.Equal("Barfi", result.Value.LastName);
        Assert.Equal("Admin", result.Value.Role);
    }
    
    [Fact]
    public async Task ChangePassword_WhenUserNotFound_ReturnsFail()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            UserId = "1",
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns((AppUser?)null);

        // Act
        var result = await _profileService.ChangePassword(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("User not found!", result.Message);
    }
    
    [Fact]
    public async Task ChangePassword_WhenCurrentPasswordIsIncorrect_ReturnsFail()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            UserId = "1",
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfi"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.CheckPasswordAsync(user, request.CurrentPassword).Returns(false);

        // Act
        var result = await _profileService.ChangePassword(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Incorrect current password!", result.Message);
    }
    
    [Fact]
    public async Task ChangePassword_WhenChangePasswordFails_ReturnsFail()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            UserId = "1",
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfi"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.CheckPasswordAsync(user, request.CurrentPassword).Returns(true);
        _userManagerMock.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword).Returns(IdentityResult.Failed(new IdentityError { Description = "Change password failed!" }));

        // Act
        var result = await _profileService.ChangePassword(request);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal("Change password failed!", result.Message);
    }
    
    [Fact]
    public async Task ChangePassword_WhenSuccess_ReturnsOk()
    {
        // Arrange
        var request = new ChangePasswordRequest
        {
            UserId = "1",
            CurrentPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        var user = new AppUser
        {
            Id = "1",
            UserName = "MobinBarfi"
        };

        _userManagerMock.FindByIdAsync(request.UserId).Returns(user);
        _userManagerMock.CheckPasswordAsync(user, request.CurrentPassword).Returns(true);
        _userManagerMock.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword).Returns(IdentityResult.Success);

        // Act
        var result = await _profileService.ChangePassword(request);

        // Assert
        Assert.True(result.Succeed);
    }
    
}