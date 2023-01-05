using Microsoft.AspNetCore.Identity;
using ControlActive.Constants;
using ControlActive.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ControlActive.Seeds
{
    public static class SeedUsers
    { 
        public static async Task SeedBasicUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "user@ung.uz",
                Email = "user@ung.uz",
                FirstName ="User",
                LastName ="ICT",
                MiddleName = "Developer",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "User@123");
                    await userManager.AddToRoleAsync(defaultUser,DefaultRoles.Role_SimpleUser );
                }
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@ung.uz",
                Email = "admin@ung.uz",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                FirstName="Admin",
                LastName="Ict",
                MiddleName="Developer"
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUser, DefaultRoles.Role_Admin);
                }
                
            }
        }


       
    }
}