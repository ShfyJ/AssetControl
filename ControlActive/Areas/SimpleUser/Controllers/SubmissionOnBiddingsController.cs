using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;
using Microsoft.AspNetCore.Identity;
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class SubmissionOnBiddingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubmissionOnBiddingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SimpleUser/SubmissionOnBiddings
        public IActionResult Index(bool success = false, bool activeError = false, bool failure = false)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Success = success;
            ViewBag.ActiveError = activeError;
            ViewBag.Failure = failure;

            ViewBag.rInSale = _context.SubmissionOnBiddings.Include(s => s.RealEstate).Where(s => s.RealEstate != null && s.Status == "Сотувда" && s.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rSold = _context.SubmissionOnBiddings.Where(s => s.RealEstate != null && s.Status == "Сотилди" && s.RealEstate.ApplicationUserId == userId).Count();
            ViewBag.rNotSold = _context.SubmissionOnBiddings.Where(s => s.RealEstate != null && s.Status == "Сотилмади" && s.RealEstate.ApplicationUserId == userId).Count();

            ViewBag.sInSale = _context.SubmissionOnBiddings.Include(s => s.Share).Where(s => s.Share != null && s.Status == "Сотувда" && s.Share.ApplicationUserId == userId).Count();
            ViewBag.sSold = _context.SubmissionOnBiddings.Where(s => s.Share != null && s.Status == "Сотилди" && s.Share.ApplicationUserId == userId).Count();
            ViewBag.sNotSold = _context.SubmissionOnBiddings.Where(s => s.Share != null && s.Status == "Сотилмади" && s.Share.ApplicationUserId == userId).Count();

            ViewBag.InSale = ViewBag.rInSale + ViewBag.sInSale;
            ViewBag.Sold = ViewBag.rSold+ ViewBag.sSold;
            ViewBag.NotSold = ViewBag.rNotSold+ ViewBag.sNotSold;

            var realEstates = _context.RealEstates.Include(r => r.AssetEvaluations)
                .Where(r => r.Confirmed == true && r.ApplicationUserId == userId && r.SubmissionOnBiddingOn == true && r.AssetEvaluations.Any() && r.AssetEvaluations.Any(a => a.Status == true && a.ReportStatus == true)).ToList();

            var shares = _context.Shares.Include(r => r.AssetEvaluations)
                .Where(r => r.SubmissionOnBiddingOn == true && r.ApplicationUserId == userId && r.AssetEvaluations.Any() &&
                r.AssetEvaluations.Any(a => a.Status == true && a.ReportStatus == true)).ToList();

            List<SelectListItem> list1 = new ();
            List<SelectListItem> list2 = new ();

            foreach(var item in realEstates)
            {
                list1.Add(new SelectListItem(item.RealEstateName, item.RealEstateId.ToString()));
            }
            
            foreach(var item in shares)
            {
                list2.Add(new SelectListItem(item.BusinessEntityName, item.ShareId.ToString()));
            }

            ViewBag.List1 = list1;
            ViewBag.List2 = list2;
            

            Dictionary<int, string> realEstatesD = new ();  
            Dictionary<int, string> sharesD = new ();
            string activeValue;

            foreach (var item in realEstates)
            {
                if (_context.ReductionInAssets.Any(r => r.Status == true && r.RealEstateId == item.RealEstateId))
                {
                    activeValue = _context.ReductionInAssets.FirstOrDefault(r => r.Status == true && r.RealEstateId == item.RealEstateId).AssetValueAfterDecline;
                }
                else
                {
                    activeValue = _context.AssetEvaluations.FirstOrDefault(a => a.RealEstateId == item.RealEstateId && a.Status == true).MarketValue;
                    
                }

                realEstatesD.Add(item.RealEstateId, activeValue);
            }

            ViewBag.RealEstatesD = realEstatesD; 

            foreach (var item in shares)
            {
                if (_context.ReductionInAssets.Any(r => r.Status == true && r.ShareId == item.ShareId))
                {
                    activeValue = _context.ReductionInAssets.FirstOrDefault(r => r.ShareId == item.ShareId).AssetValueAfterDecline;
                }
                else
                {
                    activeValue = _context.AssetEvaluations.FirstOrDefault(a => a.ShareId == item.ShareId && a.Status == true).MarketValue;
                }
                    
                sharesD.Add(item.ShareId, activeValue);
            }
            ViewBag.SharesD = sharesD;

            return View();
        }

        
        // GET: SimpleUser/SubmissionOnBiddings/Create
        public IActionResult Create(int? id, int? target)
        {
            if(id == null || target == null)
            {
                return NotFound();
            }

            ViewBag.Target = target;
            ViewData["Id"] = id;

            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id).RealEstateName;
               
                if(_context.ReductionInAssets.Any(r => r.Status == true && r.RealEstateId == id))
                {
                    ViewData["Price"] = _context.ReductionInAssets.FirstOrDefault(r => r.Status == true && r.RealEstateId == id).AssetValueAfterDecline;
                }
                else
                {
                    ViewData["Price"] = _context.AssetEvaluations.FirstOrDefault(a => a.RealEstateId == id && a.Status == true).MarketValue;
                }
                
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id).BusinessEntityName;
                if (_context.ReductionInAssets.Any(r => r.Status == true && r.ShareId == id))
                {
                    ViewData["Price"] = _context.ReductionInAssets.FirstOrDefault(r => r.Status == true && r.ShareId == id).AssetValueAfterDecline;
                }
                else
                {
                    ViewData["Price"] = _context.AssetEvaluations.FirstOrDefault(a => a.ShareId == id && a.Status == true).MarketValue;
                }
            }

            return View();
        }

        // POST: SimpleUser/SubmissionOnBiddings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int target, string name, [Bind("SubmissionOnBiddingId,TradingPlatformName,AmountOnBidding,BiddingExposureDate,ExposureTime,ActiveValue,BiddingHoldDate")] SubmissionOnBidding submissionOnBidding)
        {
            RealEstate realEstate = new();
            Share share = new();

            List<SubmissionOnBidding> submissionOnBiddings = new();

            if (target == 1)
            {
                submissionOnBidding.RealEstateId = id;
                realEstate = await _context.RealEstates.Include(a => a.AssetEvaluations).FirstOrDefaultAsync(a => a.RealEstateId ==id);

                if(realEstate == null || !realEstate.AssetEvaluations.Any(a => a.Status == true && a.ReportStatus == true))
                {
                    return RedirectToAction("Index", "SubmissionOnBiddings", new { activeError = true });
                }

                realEstate.ReductionInAssetOn = false;
                realEstate.InstallmentAssetOn = false;
                realEstate.OneTimePaymentAssetOn = false;
                realEstate.TransferredAssetOn = false;
                realEstate.SubmissionOnBiddingOn = false;
                realEstate.AssetEvaluationOn = false;

                submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.RealEstateId == id && s.Status == "Сотилмади").ToList();
            }

            if (target == 2)
            {
                submissionOnBidding.ShareId = id;
                share = await _context.Shares.Include(a => a.AssetEvaluations).FirstOrDefaultAsync(a => a.ShareId == id);

                if (share == null || !share.AssetEvaluations.Where(a => a.Status == true && a.ReportStatus == true).Any())
                {
                    return RedirectToAction("Index", "SubmissionOnBiddings", new { activeError = true });
                }

                share.ReductionInAssetOn = false;
                share.InstallmentAssetOn = false;
                share.OneTimePaymentAssetOn = false;
                share.TransferredAssetOn = false;
                share.SubmissionOnBiddingOn = false;
                share.AssetEvaluationOn = false;

                submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.ShareId == id && s.Status == "Сотилмади").ToList();
            }

                      
            if (ModelState.IsValid)
            {
                submissionOnBidding.AuctionCancelledDate = DateTime.Now.AddYears(1000);
                submissionOnBidding.Status = "Сотувда";
                submissionOnBidding.IsActiveForPriceReduction = false;

                foreach (var item in submissionOnBiddings)
                {
                    item.IsActiveForPriceReduction = false;
                    item.AuctionCancelledDate = submissionOnBidding.BiddingExposureDate;
                }
     
                _context.Add(submissionOnBidding);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "SubmissionOnBiddings", new { success = true });
            }

            ViewData["Id"] = id;

            ViewBag.Target = target;
            ViewData["Name"] = name;


            return RedirectToAction("Index", "SubmissionOnBiddings", new { failure = true });
        }

        public async Task<IActionResult> Edit(int? id, int? id1, int? target)
        {
            if (id == null || id1 == null || target == null)
            {
                return NotFound();
            }
         

            ViewBag.Target = target;
            ViewData["Id"] = id1;

            if (target == 1)
            {
                ViewBag.Name = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id1).RealEstateName;

                if (_context.ReductionInAssets.Any(r => r.Status == true && r.RealEstateId == id1))
                {
                    ViewData["Price"] = _context.ReductionInAssets.FirstOrDefault(r => r.Status == true && r.RealEstateId == id1).AssetValueAfterDecline;
                }
                else
                {
                    ViewData["Price"] = _context.AssetEvaluations.FirstOrDefault(a => a.RealEstateId == id1 && a.Status == true).MarketValue;
                }

            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id1).BusinessEntityName;
                if (_context.ReductionInAssets.Any(r => r.Status == true && r.ShareId == id1))
                {
                    ViewData["Price"] = _context.ReductionInAssets.FirstOrDefault(r => r.Status == true && r.ShareId == id1).AssetValueAfterDecline;
                }
                else
                {
                    ViewData["Price"] = _context.AssetEvaluations.FirstOrDefault(a => a.ShareId == id1 && a.Status == true).MarketValue;
                }
            }
            var submissionOnBidding = await _context.SubmissionOnBiddings.FindAsync(id);
            if (submissionOnBidding == null)
            {
                return NotFound();
            }
            return View(submissionOnBidding);
        }

        // POST: SimpleUser/SubmissionOnBiddings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, [Bind("SubmissionOnBiddingId,TradingPlatformName,Status,RealEstateId,ShareId,AmountOnBidding,BiddingExposureDate,ExposureTime,ActiveValue,BiddingHoldDate")] SubmissionOnBidding submissionOnBidding)
        {
            if (id != submissionOnBidding.SubmissionOnBiddingId)
            {
               // Console.WriteLine("asdfasd");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submissionOnBidding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionOnBiddingExists(submissionOnBidding.SubmissionOnBiddingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Name = name;
            return View(submissionOnBidding);
        }

        [HttpGet]
        public IActionResult GetOnSaleRealEstates()
        {
            var userId = _userManager.GetUserId(User);

            var submissionOnBiddings = _context.SubmissionOnBiddings.Include(s => s.RealEstate).Where(s => s.RealEstate.ApplicationUserId == userId && s.RealEstate != null
            && s.Share == null && s.Status =="Сотувда").OrderByDescending(s => s.SubmissionOnBiddingId).ToList();


            var s = _context.SubmissionOnBiddings.Include(s => s.RealEstate).Where(s => s.RealEstate != null && s.RealEstate.ApplicationUserId == userId && s.Status.Equals("Сотувда"));

            List<GeneralViewModel> viewModels = new();

            foreach(var item in submissionOnBiddings)
            {
                
                item.BiddingExposureDateStr = item.BiddingExposureDate.ToShortDateString();
                item.BiddingHoldDateStr = item.BiddingHoldDate.ToString("yyyy-MM-dd");

                GeneralViewModel viewModel = new()
                {
                    RealEstate = item.RealEstate,
                    SubmissionOnBidding = item,
                    Target = 1
                };
                viewModels.Add(viewModel);
                
            }

            return Json(new { data = viewModels});
        }


        [HttpGet]
        public IActionResult GetOnSaleShares()
        {
            var userId = _userManager.GetUserId(User);

            var submissionOnBiddings = _context.SubmissionOnBiddings.Include(s => s.Share).Where(s => s.Share.ApplicationUserId == userId && s.Share != null
            && s.RealEstate == null && s.Status == "Сотувда").OrderByDescending(s => s.SubmissionOnBiddingId).ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in submissionOnBiddings)
            {

                item.BiddingExposureDateStr = item.BiddingExposureDate.ToShortDateString();
                item.BiddingHoldDateStr = item.BiddingHoldDate.ToString("yyyy-MM-dd");

                GeneralViewModel viewModel = new()
                {
                    Share = item.Share,
                    SubmissionOnBidding = item,
                    Target = 2
                };
                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }


        [HttpGet]
        public IActionResult GetSoldRealEstates()
        {
            var userId = _userManager.GetUserId(User);

            var submissionOnBiddings = _context.SubmissionOnBiddings.Include(s => s.RealEstate).Where(s => s.RealEstate.ApplicationUserId == userId && s.RealEstate != null
            && s.Share == null && s.Status == "Сотилди").OrderByDescending(s => s.SubmissionOnBiddingId).ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in submissionOnBiddings)
            {

                item.BiddingExposureDateStr = item.BiddingExposureDate.ToShortDateString();
                item.BiddingHoldDateStr = item.BiddingHoldDate.ToString("yyyy-MM-dd");

                GeneralViewModel viewModel = new()
                {
                    RealEstate = item.RealEstate,
                    SubmissionOnBidding = item,
                    Target = 1
                };
                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }


        [HttpGet]
        public IActionResult GetSoldShares()
        {
            var userId = _userManager.GetUserId(User);

            var submissionOnBiddings = _context.SubmissionOnBiddings.Include(s => s.Share).Where(s => s.Share.ApplicationUserId == userId && s.Share != null
            && s.RealEstate == null && s.Status == "Сотилди").ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in submissionOnBiddings)
            {

                item.BiddingExposureDateStr = item.BiddingExposureDate.ToShortDateString();
                item.BiddingHoldDateStr = item.BiddingHoldDate.ToString("yyyy-MM-dd");

                GeneralViewModel viewModel = new()
                {
                    Share = item.Share,
                    SubmissionOnBidding = item,
                    Target = 2
                };
                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }


        [HttpGet]
        public IActionResult GetNotSoldRealEstates()
        {
            var userId = _userManager.GetUserId(User);

            var submissionOnBiddings = _context.SubmissionOnBiddings.Include(s => s.RealEstate).Where(s => s.RealEstate.ApplicationUserId == userId && s.RealEstate != null
            && s.Share == null && s.Status == "Сотилмади").OrderBy(s => s.SubmissionOnBiddingId).ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in submissionOnBiddings)
            {

                item.BiddingExposureDateStr = item.BiddingExposureDate.ToShortDateString();
                item.BiddingHoldDateStr = item.BiddingHoldDate.ToString("yyyy-MM-dd");

                GeneralViewModel viewModel = new()
                {
                    RealEstate = item.RealEstate,
                    SubmissionOnBidding = item,
                    Target = 1
                };
                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }


        [HttpGet]
        public IActionResult GetNotSoldShares()
        {
            var userId = _userManager.GetUserId(User);

            var submissionOnBiddings = _context.SubmissionOnBiddings.Include(s => s.Share).Where(s => s.Share.ApplicationUserId == userId && s.Share != null
            && s.RealEstate == null && s.Status == "Сотилмади").OrderByDescending(s => s.SubmissionOnBiddingId).ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in submissionOnBiddings)
            {

                item.BiddingExposureDateStr = item.BiddingExposureDate.ToShortDateString();
                item.BiddingHoldDateStr = item.BiddingHoldDate.ToString("yyyy-MM-dd");

                GeneralViewModel viewModel = new()
                {
                    Share = item.Share,
                    SubmissionOnBidding = item,
                    Target = 2
                };
                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] JObject data)
        {
            if (data["id"] == null || data["target"] == null)
            {
                return Json(new { success = false, message = "Хатолик - Қайтадан уриниб кўринг!" });
            }
            int id = (int)data["id"];
            int target = (int)data["target"];

            var submissionOnBidding = await _context.SubmissionOnBiddings.FindAsync(id);

            if (submissionOnBidding == null)
                return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            
            if(target == 1)
            {
                var realEstate = await _context.RealEstates.Include(r => r.SubmissionOnBiddings)
                    .FirstOrDefaultAsync(r => r.SubmissionOnBiddings.Any(s => s.SubmissionOnBiddingId == id));
                realEstate.OneTimePaymentAssetOn = true;
            }

            if (target == 2)
            {
                var share = await _context.Shares.Include(r => r.SubmissionOnBiddings)
                    .FirstOrDefaultAsync(r => r.SubmissionOnBiddings.Any(s => s.SubmissionOnBiddingId == id));
                share.OneTimePaymentAssetOn = true;
            }

            submissionOnBidding.Confirmed = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

            return Json(new { success = true, message = "Маълумотлар тасдиқланди!" });
        }


        [HttpPost]
        public async Task<IActionResult> SendResult([FromBody] JObject data)
        {
            if (data["id"] == null)
            {
                return NotFound();
            }

            int id = (int)data["id"];
            bool result = (bool)data["result"];
            int target = (int)data["target"];

            var submissionOnBidding = await _context.SubmissionOnBiddings.FindAsync(id);
            
            if (submissionOnBidding == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }

            RealEstate realEstate = new();
            Share share = new();

            if(target == 1)
            {
                realEstate = _context.RealEstates.Include(r => r.SubmissionOnBiddings)
                    .FirstOrDefault(r => r.SubmissionOnBiddings.Any(a => a.SubmissionOnBiddingId == id));
                if (realEstate == null)
                    return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });

            }

            if(target == 2)
            {
                share = _context.Shares.Include(r => r.SubmissionOnBiddings)
                   .FirstOrDefault(r => r.SubmissionOnBiddings.Any(a => a.SubmissionOnBiddingId == id));
                if (share == null)
                    return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }
            

            if (result)
            {
                submissionOnBidding.Status = "Сотилди";
                                
            }
                
            else
            {
                submissionOnBidding.Status = "Сотилмади";
                submissionOnBidding.IsActiveForPriceReduction = true;
                if (target == 1)
                {
                    realEstate.TransferredAssetOn = true;
                    realEstate.InstallmentAssetOn = true;
                    realEstate.ReductionInAssetOn = true;
                    
                    realEstate.SubmissionOnBiddingOn = true;
                    realEstate.AssetEvaluationOn = true;
                }
                if(target == 2)
                {
                    share.TransferredAssetOn = true;
                    share.InstallmentAssetOn = true;
                    share.ReductionInAssetOn = true;
                    share.SubmissionOnBiddingOn = true;
                    share.AssetEvaluationOn=true;
                }

            }
                

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message});
            }

            if (result)
                return Json(new { success = true, message = "Актив сотилди!" });

            else
                return Json(new { success = true, message = "Актив сотилмади!" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] JObject data)
        {
            if (data["id"] == null || data["target"] == null)
            {
                return Json(new { success = false, message = "Хатолик! - Қайтадан уриниб кўринг." });
            }

            int id = (int)data["id"];
            int target = (int)data["target"];   

            var submissionOnBidding = await _context.SubmissionOnBiddings.FirstOrDefaultAsync(t => t.SubmissionOnBiddingId == id);

            if (submissionOnBidding == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }

            if (target == 1)
            {
                var realEstate = _context.RealEstates.Include(r => r.SubmissionOnBiddings)
                    .FirstOrDefault(r => r.SubmissionOnBiddings.Any(a => a.SubmissionOnBiddingId == id) == true);
                realEstate.SubmissionOnBiddingOn = true;
                realEstate.InstallmentAssetOn = true;
                realEstate.TransferredAssetOn = true;
                realEstate.AssetEvaluationOn = true;
            }

            if (target == 2)
            {
                var share = _context.Shares.Include(r => r.SubmissionOnBiddings)
                    .FirstOrDefault(r => r.SubmissionOnBiddings.Any(a => a.SubmissionOnBiddingId == id) == true);
                share.SubmissionOnBiddingOn = true;
                share.InstallmentAssetOn = true;
                share.TransferredAssetOn = true;
                share.AssetEvaluationOn = true;
            }

            try
            {
                _context.SubmissionOnBiddings.Remove(submissionOnBidding);

                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                var e1x = ex;
            }


            return Json(new { success = true, message = "Амалиёт муваффақиятли якунланди!" });
        }
        

        private bool SubmissionOnBiddingExists(int id)
        {
            return _context.SubmissionOnBiddings.Any(e => e.SubmissionOnBiddingId == id);
        }
    }
}
