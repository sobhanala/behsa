using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;
public class AppUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; } = String.Empty;
    [MaxLength(50)] 
    public string LastName { get; set; } = String.Empty;
}