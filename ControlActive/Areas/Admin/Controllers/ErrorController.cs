using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Areas.Admin.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        [HttpGet]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Uzur, siz qidirgan sahifa topilmadi";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Uzur, serverda xatolik yuz bergan ko'rinadi";
                    break;
            }
            return View("_NotFound");
        }
    }
}
