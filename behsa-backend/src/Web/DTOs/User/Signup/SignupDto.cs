using System.ComponentModel.DataAnnotations;

namespace Web.DTOs.User.Signup;

public class SignupDto
{
    [Required]
    [MaxLength(50)]
    public string? FirstName { get; set; }
    [Required]
    [MaxLength(50)]
    public string? LastName { get; set; }
    [Required]
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    [MaxLength(50)]
    public string? UserName { get; set; }
    [Required]
    [MaxLength(50)]
    [MinLength(8)]
    public string? Password { get; set; }
    [Required]
    [MaxLength(50)]
    public string? Role { get; set; }
}