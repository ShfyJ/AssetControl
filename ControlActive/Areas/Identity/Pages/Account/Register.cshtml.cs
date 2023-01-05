using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ControlActive.Constants;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ControlActive.Areas.Identity.Pages.Account
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _emailSender = emailSender;
        }

        [BindProperty]
        public string Message1 { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,15}$", ErrorMessage = "Парол узунлиги 6-15 орасида бўлиши керак! Камида битта катта ҳарф, битта кичик ҳарф, битта рақам ва белгидан иборат бўлиши керак!")]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Парол ва тасдиқлаш пароли мос келмади")]
            public string ConfirmPassword { get; set; }

            [Required]
            [RegularExpression("^[9,7,3][0-9]{8}$", ErrorMessage = "Тел. рақами нотўғри кўрсатилган!")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "PhoneNumber")]
            public string PhoneNumber { get; set; }
           

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string Position { get; set; }

            [Required]
            public string LastName { get; set; }
            [Required]
            public string MiddleName { get; set; }

            public string Role { get; set; }

           [Required]
           public int OrganizationId { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }

            public IEnumerable<SelectListItem> Organizations { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            var userList = _context.ApplicationUsers.ToList();
            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            

            foreach (var item in roles)
            {
                if (item.Name.Equals("SimpleUser"))
                {
                    item.Name = "Фойдаланувчи";
                }

                if (item.Name.Equals("Admin"))
                {
                    item.Name = "Марказий аппарат ходими";
                }

            }

            foreach (var user in userList)
            {
                string roleId;
              
                var userRole = userRoles.FirstOrDefault(u => u.UserId == user.Id);

                if (userRole != null)
                {
                    roleId = userRole.RoleId;

                }
                else
                {
                    break;
                }

                var role = roles.FirstOrDefault(u => u.Id == roleId);

                if (role != null)
                {
                    user.Role = role.Name;
                }

            }
         
            var organizations = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName");

            Input = new InputModel()
            {
               
                RoleList = roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                    Selected = false
                }),


                Organizations = organizations
            };


            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var userList = _context.ApplicationUsers.ToList();
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            foreach (var item in userList)
            {
                if (item.Email == Input.Email)
                {
                    Message1 = "Электрон почта мавжуд";
                }
            }

            var createdById = _userManager.GetUserId(User);
            string role = "";
            if(Input.Role == "Фойдаланувчи")
            {
                role = "SimpleUser";
            }
            if(Input.Role == "Марказий аппарат ходими")
            {
                role = "Admin";
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    MiddleName = Input.MiddleName,
                    PhoneNumber = "+998" + Input.PhoneNumber,
                    Role = role,
                    Postion = Input.Position,
                    OrganizationId = Input.OrganizationId,
                    EmailConfirmed = true,
                    CreatedById = createdById

                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var comfirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, comfirmationLink);
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(DefaultRoles.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(DefaultRoles.Role_SimpleUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.Role_SimpleUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(DefaultRoles.Role_SuperAdmin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.Role_SuperAdmin));

                    }

                    _context.Add(user);
                    await _userManager.AddToRoleAsync(user, user.Role);
                   


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                            //admin is registering new user
                            return RedirectToAction("Index", "Users", new { Area = "Admin" });
                    
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        //return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form

            var roles = _context.Roles.ToList();

            foreach (var item in roles)
            {
                if (item.Name.Equals("SimpleUser"))
                {
                    item.Name = "Фойдаланувчи";
                }

                if (item.Name.Equals("Admin"))
                {
                    item.Name = "Марказий аппарат ходими";
                }

            }

            Input = new InputModel()
            {

                RoleList = roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                    Selected = false
                }),

                Organizations = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName")

        };

            foreach (var item in Input.RoleList)
            {
                if (item.Text.Equals("SimpleUser"))
                {
                    item.Text = "Фойдаланувчи";
                }

                if (item.Text.Equals("Admin"))
                {
                    item.Text = "Марказий аппарат ходими";
                }

            }

            return Page();
        }
    }
}
