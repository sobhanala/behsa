namespace Application.DTOs.Identity.LoginUser;

public class LoginUserResponse
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Role { get; set; } = String.Empty;
    public string Token { get; set; } = String.Empty;
}