using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace ControlActive.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
     

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public string FName { get; set; }
        //public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string SName { get; set; }
        public string Lavozm { get; set; }
        public string Role { get; set; }
      
        public IEnumerable<SelectListItem> Organizations { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Required]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }
            public string FName { get; set; }
            public string Lavozm { get; set; }
            public string Name { get; set; }
            public string SName { get; set; }
            public int OrgId { get; set; }

            
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == user.Id);
            var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId).Name;
            var org = _context.Organizations.FirstOrDefault(o => o.OrganizationId == user.OrganizationId);
            var selected ="";
            if (org == null)
            {
                selected = "---";
            }
            else
                selected = org.OrganizationName;

            var organizations= new SelectList (_context.Organizations, "OrganizationId", "OrganizationName", selected );
            var organizationId = user.OrganizationId;

            ViewData["Orgs"] = organizations;

            Username = userName;
            FName = user.LastName;
            SName = user.MiddleName;
            Name = user.FirstName;
            Role = role;
            Lavozm = user.Postion;
            Organizations = organizations;
            //PhoneNumber = user.PhoneNumber;
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FName = FName,
                Name = Name,
                SName = SName,
                Lavozm = Lavozm,
                OrgId = organizationId
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationUser = _context.ApplicationUsers.Find(user.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(applicationUser);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationUser = _context.ApplicationUsers.Find(user.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(applicationUser);
                return Page();
            }


            var user_ = new ApplicationUser
            {
                Postion = Input.Lavozm,
                FirstName = Input.Name,
                LastName = Input.FName,
                PhoneNumber = Input.PhoneNumber,
                OrganizationId = Input.OrgId
                
            };
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var objFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);
            var position = objFromDb.Postion;

           
            if (Input.Lavozm != position)
            {
                objFromDb.Postion = Input.Lavozm;
                _context.SaveChanges();

            }

            if (Input.Name != objFromDb.FirstName)
            {
                objFromDb.FirstName = Input.Name;
                _context.SaveChanges();

            }

            if (Input.FName != objFromDb.LastName)
            {
                objFromDb.LastName = Input.FName;
                _context.SaveChanges();

            }

            if (Input.SName != objFromDb.MiddleName)
            {
                objFromDb.MiddleName = Input.SName;
                _context.SaveChanges();

            }

            if(Input.OrgId != objFromDb.OrganizationId)
            {
                objFromDb.OrganizationId = Input.OrgId;
                _context.SaveChanges();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Профилингиз янгиланди!";
            ViewData["All"] = "show active";
            return RedirectToPage();
        }
    }
}
