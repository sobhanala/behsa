namespace Application.DTOs.Identity.CreateUser;

public class CreateUserResponse
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Role { get; set; } = String.Empty;
}   