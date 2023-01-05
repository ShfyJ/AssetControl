using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ControlActive.Constants;
using ControlActive.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ControlActive.Areas.Identity.Pages.Account
{

    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class RenewPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<RenewPasswordModel> _logger;
        private readonly ApplicationDbContext _db;

        public RenewPasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RenewPasswordModel> logger,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,15}$", ErrorMessage = "Парол узунлиги 6-15 орасида бўлиши керак! Камида битта катта ҳарф, битта кичик ҳарф, битта рақам ва белгидан иборат бўлиши керак!")]
            [DataType(DataType.Password)]
            [Display(Name = "Парол")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "Янги парол ва тасдиқлаш пароли мос келмади.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _db.ApplicationUsers.FindAsync(_userManager.GetUserId(User));
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (user.isPasswordRenewed)
            {
                return Redirect("/Admin/Redirect/");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (!ModelState.IsValid)
            {
                ViewData["Password"] = "active";
                
                return Page();
            }

            //var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Ушбу ID билан фойдаланувчи топилмади: '{_userManager.GetUserId(User)}'.");
            }

           
            var flag1 = _userManager.CheckPasswordAsync(user, Input.OldPassword);
            var flag2 = _userManager.CheckPasswordAsync(user, Input.NewPassword);

            if (!flag1.Result)
            {
                ErrorMessage = "Эски парол нотўғри киритилган";
                return Page();
            }

            if (flag2.Result)
            {
                ErrorMessage = "Янги парол ва эски парол фарқли бўлиши керак!";
                return Page();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ErrorMessage = changePasswordResult.Errors.ToString();
                return Page();
            }

            _db.ApplicationUsers.FirstOrDefault(a => a.Id == user.Id).isPasswordRenewed = true;
            await _db.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Парол муваффақиятли ўзгартирилди!");
            StatusMessage = "Паролингиз ўзгарди!";
            ViewData["Password"] = "active";
            return Redirect("/Identity/Account/Manage/Index/");
        }
    }
}
