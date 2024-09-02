using Application.DTOs;
using Application.DTOs.Profile;
using Application.DTOs.Profile.EditProfile;
using Application.DTOs.Profile.GetProfileInfo;
using Application.ExtensionMethods;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappers;

namespace Application.Services.DomainService;

public class ProfileService : IProfileService
{
    private readonly IUserManagerRepository _userManagerRepository;

    public ProfileService(IUserManagerRepository userManagerRepository)
    {
        _userManagerRepository = userManagerRepository;
    }

    public async Task<Result<EditProfileInfoResponse>> EditProfileInfo(EditProfileInfoRequest infoRequest)
    {
        try
        {
            var user = await _userManagerRepository.FindByIdAsync(infoRequest.UserId);
            if (user == null)
                return Result<EditProfileInfoResponse>.Fail("User not found!");
        
            if (user.UserName != infoRequest.UserName)
            {
                var existingUser = await _userManagerRepository.FindByNameAsync(infoRequest.UserName);
                if (existingUser != null)
                    return Result<EditProfileInfoResponse>.Fail("Username is already reserved by another user!");
            }
        
            user.UserName = infoRequest.UserName;
            user.FirstName = infoRequest.FirstName;
            user.LastName = infoRequest.LastName;

            var updateResult = await _userManagerRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<EditProfileInfoResponse>.Fail(updateResult.Errors.FirstMessage());
        
            return Result<EditProfileInfoResponse>.Ok(user.ToEditProfileInfoResponse());
        }
        catch (Exception ex)
        {
            return Result<EditProfileInfoResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<GetProfileInfoResponse>> GetProfileInfo(GetProfileInfoRequest getProfileInfoRequest)
    {
        try
        {
            var user = await _userManagerRepository.FindByIdAsync(getProfileInfoRequest.UserId);
        
            if (user == null)
                return Result<GetProfileInfoResponse>.Fail("User not found!");

            var role = await _userManagerRepository.GetRoleAsync(user);
        
            return Result<GetProfileInfoResponse>.Ok(user.ToGetProfileInfoResponse(role));
        }
        catch (Exception ex)
        {
            return Result<GetProfileInfoResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            var user = await _userManagerRepository.FindByIdAsync(request.UserId);
            if (user == null)
                return Result.Fail("User not found!");

            var isPasswordCorrect = await _userManagerRepository.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isPasswordCorrect)
                return Result.Fail("Incorrect current password!");

            var passwordChangeResult = await _userManagerRepository.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!passwordChangeResult.Succeeded)
                return Result.Fail(passwordChangeResult.Errors.FirstMessage());

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}