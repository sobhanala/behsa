using System.ComponentModel.DataAnnotations;

namespace Web.DTOs.User;

public class ChangeRoleDto
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Role { get; set; }
}