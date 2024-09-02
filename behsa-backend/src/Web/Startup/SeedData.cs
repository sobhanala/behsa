using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Web.Startup;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, IConfigurationManager config)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var id = config["RootUser:Id"]!;
        var roleName = config["RootUser:RoleName"]!;
        var userName = config["RootUser:UserName"]!;
        var email = config["RootUser:Email"];
        var password = config["RootUser:Password"]!;
        
        var rootUser = await userManager.FindByIdAsync(id);
        if (rootUser == null)
        {
            rootUser = new AppUser
            {
                Id = id,
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(rootUser, password);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to create root user");
            }
            
            var roleResult = await userManager.AddToRoleAsync(rootUser, roleName);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to add to role");
            }
        }
    }
}
