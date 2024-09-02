using System.ComponentModel.DataAnnotations;

namespace Web.DTOs.Profile;

public class EditProfileInfoDto
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
}