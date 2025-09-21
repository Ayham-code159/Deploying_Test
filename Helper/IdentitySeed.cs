using Deploying_Test.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Deploying_Test.Helper
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<Owner>>();

            // Ensure roles exist
            foreach (var role in new[] { "User", "Admin" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Create a default admin if none exists (use appsettings.json values if you like)
            //var adminEmail = config["Seed:AdminEmail"] ?? "admin@yej.local";
            //var adminPass = config["Seed:AdminPassword"] ?? "Admin123!";

            //var admin = await userManager.FindByEmailAsync(adminEmail);
            //if (admin is null)
            //{
            //    admin = new Owner
            //    {
            //        UserName = adminEmail,
            //        Email = adminEmail,
            //        EmailConfirmed = true
            //    };
            //    var created = await userManager.CreateAsync(admin, adminPass);
            //    if (created.Succeeded)
            //        await userManager.AddToRoleAsync(admin, "Admin");
            //}
        }
    }

}
