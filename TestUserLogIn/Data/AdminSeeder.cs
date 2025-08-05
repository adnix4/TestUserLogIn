using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace TestUserLogIn.Data
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Staff", "VolunteerOrganizer", "RegisteredUser" };
            //string memberRole = "Member";
            string adminEmail = "admin@yahoo.com";
            string adminPassword = "P@ssword1!#";
            string staffEmail = "staff@yahoo.com";
            string password = "P@ssword2!#";
            string volunteerOrganizerEmail = "vol@yahoo.com";            

            // Create Member role if it doesn't exist
            //if (!await roleManager.RoleExistsAsync(memberRole))
            //{
            //    await roleManager.CreateAsync(new IdentityRole(memberRole));
            //}

            // Create role if it doesn't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
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
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            // Create staff user if it doesn't exist
            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new ApplicationUser { UserName = staffEmail, Email = staffEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(staffUser, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception("Staff user creation failed: " + errors);
                }
            }
            // Assign user to Staff role
            if (!await userManager.IsInRoleAsync(staffUser, "Staff"))
            {
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }
            // Create volunteer organizer user if it doesn't exist
            var volunteerOrganizerUser = await userManager.FindByEmailAsync(volunteerOrganizerEmail);
            if (volunteerOrganizerUser == null)
            {
                volunteerOrganizerUser = new ApplicationUser { UserName = volunteerOrganizerEmail, Email = volunteerOrganizerEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(volunteerOrganizerUser, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception("Volunteer Organizer user creation failed: " + errors);
                }
            }
            // Assign user to VolunteerOrganizer role
            if (!await userManager.IsInRoleAsync(volunteerOrganizerUser, "VolunteerOrganizer"))
            {
                await userManager.AddToRoleAsync(volunteerOrganizerUser, "VolunteerOrganizer");
            }
            // Assign any unassigned user to RegisteredUser role
            var users = userManager.Users.ToList();

            foreach (var user in users)
            {
                var assignedRoles = await userManager.GetRolesAsync(user);

                if (!assignedRoles.Contains("RegisteredUser") && !assignedRoles.Contains("Admin") && !assignedRoles.Contains("Staff") && !assignedRoles.Contains("VolunteerOrganizer"))
                {
                    await userManager.AddToRoleAsync(user, "RegisteredUser");
                }
            }

        }
    }
}
