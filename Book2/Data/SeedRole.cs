using Microsoft.AspNetCore.Identity;
using Book2.Models;

namespace Book2.Data
{
    public static class SeedRole
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Staff", "Shipper", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminEmail = "admin@gmail.com";
            string adminPassword = "Admin123@";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    HoTen = "Quản trị viên"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                var currentRoles = await userManager.GetRolesAsync(admin);
                if (!currentRoles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}