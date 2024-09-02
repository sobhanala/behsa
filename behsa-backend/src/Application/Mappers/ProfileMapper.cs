using Application.DTOs.Profile.EditProfile;
using Application.DTOs.Profile.GetProfileInfo;
using Domain.Entities;

namespace Application.Mappers;

public static class ProfileMapper
{
    public static GetProfileInfoResponse ToGetProfileInfoResponse(this AppUser user, string role)
    {
        return new GetProfileInfoResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            Role = role
        };
    }
    public static EditProfileInfoResponse ToEditProfileInfoResponse(this AppUser user)
    {
        return new EditProfileInfoResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName
        };
    }
}