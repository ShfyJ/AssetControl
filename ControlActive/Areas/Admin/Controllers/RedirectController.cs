using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ControlActive.Areas.Admin.Controllers
{
    public class RedirectController : Controller
    {
        [Area("Admin")]
        [Authorize]
        public ActionResult Index()
        {
            ClaimsPrincipal currentUser = User;

            if (currentUser.IsInRole(DefaultRoles.Role_SimpleUser))
            {

                return RedirectToAction("Index", "Home", new { Area = "SimpleUser" });
            }
            if (currentUser.IsInRole(DefaultRoles.Role_Admin))
            {

                return RedirectToAction("Dashboard", "Users", new { Area = "Admin" });
            }

            if (currentUser.IsInRole(DefaultRoles.Role_SuperAdmin))
            {

                return RedirectToAction("Index", "SuperAdmin", new { Area = "SuperAdmin" });
            }

            return View();
        }

       
    }
}
