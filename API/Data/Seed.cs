using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedRoles(RoleManager<AppRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new AppRole { Name = "Admin" });
            }

            if (!await roleManager.RoleExistsAsync("Member"))
            {
                await roleManager.CreateAsync(new AppRole { Name = "Member" });
            }

            // Add more roles if needed
        }

    }
}
