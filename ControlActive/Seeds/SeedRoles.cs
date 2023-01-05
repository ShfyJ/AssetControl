using Microsoft.AspNetCore.Identity;
using ControlActive.Constants;
using ControlActive.Models;
using System.Threading.Tasks;

namespace ControlActive.Seeds
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(DefaultRoles.Role_Admin));
            await roleManager.CreateAsync(new IdentityRole(DefaultRoles.Role_SimpleUser)); 
         
        }
    }
}