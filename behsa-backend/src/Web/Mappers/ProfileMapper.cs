using Application.DTOs.Profile;
using Application.DTOs.Profile.EditProfile;
using Application.DTOs.Profile.GetProfileInfo;
using Web.DTOs.Profile;

namespace Web.Mappers;

public static class ProfileMapper
{
    public static EditProfileInfoRequest ToEditProfileInfoRequest(this EditProfileInfoDto editProfileInfoDto,
        string userId)
    {
        return new EditProfileInfoRequest
        {
            UserId = userId,
            UserName = editProfileInfoDto.UserName,
            FirstName = editProfileInfoDto.FirstName,
            LastName = editProfileInfoDto.LastName,
        };
    }
    
    public static ProfileInfoDto ToProfileInfoDto(this GetProfileInfoResponse getProfileInfoResponse)
    {
        return new ProfileInfoDto
        {
            FirstName = getProfileInfoResponse.FirstName,
            LastName = getProfileInfoResponse.LastName,
            Email = getProfileInfoResponse.Email,
            UserName = getProfileInfoResponse.UserName,
            Role = getProfileInfoResponse.Role
        };
    }

    public static ChangePasswordRequest ToChangePasswordRequest(this ChangePasswordDto changePasswordDto, string userId)
    {
        return new ChangePasswordRequest
        {
            UserId = userId,
            CurrentPassword = changePasswordDto.CurrentPassword,
            NewPassword = changePasswordDto.NewPassword
        };
    }
}