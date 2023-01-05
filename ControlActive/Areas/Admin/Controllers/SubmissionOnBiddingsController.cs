using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class SubmissionOnBiddingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubmissionOnBiddingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/SubmissionOnBiddings
        public async Task<IActionResult> Index(bool success = false)
        {
            ViewBag.Success = success;

            ViewBag.rInSale = _context.SubmissionOnBiddings.Where(s => s.RealEstate != null && s.Status == "Сотувда").Count();
            ViewBag.rSold = _context.SubmissionOnBiddings.Where(s => s.RealEstate != null && s.Status == "Сотилди").Count();
            ViewBag.rNotSold = _context.SubmissionOnBiddings.Where(s => s.RealEstate != null && s.Status == "Сотилмади").Count();

            ViewBag.sInSale = _context.SubmissionOnBiddings.Where(s => s.Share != null && s.Status == "Сотувда").Count();
            ViewBag.sSold = _context.SubmissionOnBiddings.Where(s => s.Share != null && s.Status == "Сотилди").Count();
            ViewBag.sNotSold = _context.SubmissionOnBiddings.Where(s => s.Share != null && s.Status == "Сотилмади").Count();
            var applicationDbContext = _context.SubmissionOnBiddings.Include(s => s.RealEstate).Include(s => s.Share);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/SubmissionOnBiddings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionOnBidding = await _context.SubmissionOnBiddings
                .Include(s => s.RealEstate)
                .Include(s => s.Share)
                .FirstOrDefaultAsync(m => m.SubmissionOnBiddingId == id);
            if (submissionOnBidding == null)
            {
                return NotFound();
            }

            return View(submissionOnBidding);
        }

        // GET: Admin/SubmissionOnBiddings/Create
        public IActionResult Create(int? id, int? target)
        {
            if (id == null || target == null)
            {
                return NotFound();
            }

            ViewBag.Target = target;
            ViewData["Id"] = id;

            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id).RealEstateName;

                if (_context.ReductionInAssets.Any(r => r.Status == true && r.RealEstateId == id))
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


        // POST: Admin/SubmissionOnBiddings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubmissionOnBiddingId,TradingPlatformName,AmountOnBidding,BiddingExposureDate,ExposureTime,ActiveValue,BiddingHoldDate,Status,RealEstateId,ShareId")] SubmissionOnBidding submissionOnBidding)
        {
            if (ModelState.IsValid)
            {
                _context.Add(submissionOnBidding);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", submissionOnBidding.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", submissionOnBidding.ShareId);
            return View(submissionOnBidding);
        }

        // GET: Admin/SubmissionOnBiddings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionOnBidding = await _context.SubmissionOnBiddings.FindAsync(id);
            if (submissionOnBidding == null)
            {
                return NotFound();
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", submissionOnBidding.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", submissionOnBidding.ShareId);
            return View(submissionOnBidding);
        }

        // POST: Admin/SubmissionOnBiddings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubmissionOnBiddingId,TradingPlatformName,AmountOnBidding,BiddingExposureDate,ExposureTime,ActiveValue,BiddingHoldDate,Status,RealEstateId,ShareId")] SubmissionOnBidding submissionOnBidding)
        {
            if (id != submissionOnBidding.SubmissionOnBiddingId)
            {
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
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", submissionOnBidding.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", submissionOnBidding.ShareId);
            return View(submissionOnBidding);
        }

        // GET: Admin/SubmissionOnBiddings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submissionOnBidding = await _context.SubmissionOnBiddings
                .Include(s => s.RealEstate)
                .Include(s => s.Share)
                .FirstOrDefaultAsync(m => m.SubmissionOnBiddingId == id);
            if (submissionOnBidding == null)
            {
                return NotFound();
            }

            return View(submissionOnBidding);
        }

        // POST: Admin/SubmissionOnBiddings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submissionOnBidding = await _context.SubmissionOnBiddings.FindAsync(id);
            _context.SubmissionOnBiddings.Remove(submissionOnBidding);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubmissionOnBiddingExists(int id)
        {
            return _context.SubmissionOnBiddings.Any(e => e.SubmissionOnBiddingId == id);
        }
    }
}
