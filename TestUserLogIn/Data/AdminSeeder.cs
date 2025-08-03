using Microsoft.AspNetCore.Identity;

namespace TestUserLogIn.Data
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminRole = "Admin";
            string memberRole = "Member";
            string adminEmail = "fshromen@yahoo.com";
            string adminPassword = "P@assword1!#";

            // Create Member role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(memberRole))
            {
                await roleManager.CreateAsync(new IdentityRole(memberRole));
            }

            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Create admin user if it doesn't exist
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                //await userManager.CreateAsync(adminUser, adminPassword);
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception("Admin user creation failed: " + errors);
                }

            }

            // Assign user to Admin role
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        
        }
    }
}
