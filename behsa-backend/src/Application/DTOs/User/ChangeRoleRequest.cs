namespace Application.DTOs.User;

public class ChangeRoleRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}