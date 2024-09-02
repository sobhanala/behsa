using System.ComponentModel.DataAnnotations;

namespace Web.DTOs.Profile;

public class ChangePasswordDto
{
    [Required]
    public string? CurrentPassword { get; set; }
    [Required]
    public string? NewPassword { get; set; }
}