using ControlActive.Constants;
using ControlActive.Data;

using ControlActive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
       // List<Noti> _oNotifications = new List<Noti>();
        

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            return LocalRedirect(returnUrl);
        }


        // GET: HomeController
        public ActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            ViewData["RealEstates"] = _context.RealEstates.Where(r => r.ApplicationUserId == _userManager.GetUserId(User)).Count();
            ViewData["Shares"] = _context.Shares.Where(r => r.ApplicationUserId == _userManager.GetUserId(User)).Count();
            ViewData["TransfAssets"] = _context.TransferredAssets.Where(r => r.Share.ApplicationUserId == _userManager.GetUserId(User)).Count() +
                _context.TransferredAssets.Where(r => r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).Count();
            ViewData["AssetEval"] = _context.AssetEvaluations.Where(r => r.Share.ApplicationUserId == _userManager.GetUserId(User)).Count() +
                _context.AssetEvaluations.Where(r => r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).Count();

            ViewData["SubmissionOnB"] = _context.SubmissionOnBiddings.Where(r => r.Share.ApplicationUserId == _userManager.GetUserId(User)).Count() +
                _context.SubmissionOnBiddings.Where(r => r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).Count();
            ViewData["ReductionIn"] = _context.ReductionInAssets.Where(r => r.Share.ApplicationUserId == _userManager.GetUserId(User)).Count() +
                _context.ReductionInAssets.Where(r => r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).Count();
            ViewData["OneTimeP"] = _context.OneTimePaymentAssets.Where(r => r.Share.ApplicationUserId == _userManager.GetUserId(User)).Count() +
                _context.OneTimePaymentAssets.Where(r => r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).Count();
            ViewData["Installment"] = _context.InstallmentAssets.Where(r => r.Share.ApplicationUserId == _userManager.GetUserId(User)).Count() +
                _context.InstallmentAssets.Where(r => r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).Count();

            //number of # for realEstate
            ViewBag.rTransfAssets = _context.TransferredAssets.Where(t => t.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rAssetEval = _context.AssetEvaluations.Where(t => t.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rSubmissionOnB = _context.SubmissionOnBiddings.Where(t => t.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rReductionIn = _context.ReductionInAssets.Where(t => t.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rOneTimeP = _context.OneTimePaymentAssets.Where(t => t.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rInstallment = _context.InstallmentAssets.Where(t => t.RealEstate.ApplicationUserId == userId).Count();

            //number of # for share
            ViewBag.sTransfAssets = _context.TransferredAssets.Where(t => t.Share.ApplicationUserId == userId).Count();
            ViewBag.sAssetEval = _context.AssetEvaluations.Where(t => t.Share.ApplicationUserId == userId).Count();
            ViewBag.sSubmissionOnB = _context.SubmissionOnBiddings.Where(t => t.Share.ApplicationUserId == userId).Count();
            ViewBag.sReductionIn = _context.ReductionInAssets.Where(t => t.Share.ApplicationUserId == userId).Count();
            ViewBag.sOneTimeP = _context.OneTimePaymentAssets.Where(t => t.Share.ApplicationUserId == userId).Count();
            ViewBag.sInstallment = _context.InstallmentAssets.Where(t => t.Share.ApplicationUserId == userId).Count();

            return View();
        }

        public IActionResult AllNotifications()
        {
            return View();
        }

        //public JsonResult GetNotifications(bool bIsGetOnlyUnread = false)
        //{
        //    var nToUserId = _userManager.GetUserId(User);
        //    _oNotifications = new List<Noti>();
        //    _oNotifications = _notiService.GetNotifications(nToUserId, bIsGetOnlyUnread);
        //    return Json(_oNotifications);

        //}


        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
