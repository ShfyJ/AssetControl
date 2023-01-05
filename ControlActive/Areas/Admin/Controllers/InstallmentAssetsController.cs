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
using Microsoft.AspNetCore.Hosting;
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class InstallmentAssetsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InstallmentAssetsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/InstallmentAssets
        public async Task<IActionResult> Index(bool success = false)
        {
            ViewBag.Success = success;
            var applicationDbContext = _context.InstallmentAssets.Include(i => i.Share)
                .Include(i => i.RealEstate);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/InstallmentAssets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var installmentAsset = await _context.InstallmentAssets
                .Include(i => i.RealEstate)
                .Include(i => i.Share)
                .Include(i => i.Step2)
                .FirstOrDefaultAsync(m => m.InstallmentAssetId == id);
            if (installmentAsset == null)
            {
                return NotFound();
            }

            return View(installmentAsset);
        }

        // GET: Admin/InstallmentAssets/Create
        public IActionResult Create()
        {
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity");
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare");
            ViewData["InstallmentStep2Id"] = new SelectList(_context.InstallmentStep2, "InstallmentStep2Id", "InstallmentStep2Id");
            return View();
        }

        // POST: Admin/InstallmentAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstallmentAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileLink,SolutionFileId,BiddingDate,AssetBuyerName,AmountOfAssetSold,AggreementDate,AggreementNumber,AggreementFileLink,AggreementFileId,InstallmentTime,ActualInitPayment,PaymentPeriodType,ActualPayment,Status,InstallmentStep2Id,RealEstateId,ShareId")] InstallmentAsset installmentAsset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(installmentAsset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", installmentAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", installmentAsset.ShareId);
            ViewData["InstallmentStep2Id"] = new SelectList(_context.InstallmentStep2, "InstallmentStep2Id", "InstallmentStep2Id", installmentAsset.InstallmentStep2Id);
            return View(installmentAsset);
        }

        // GET: Admin/InstallmentAssets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var installmentAsset = await _context.InstallmentAssets.FindAsync(id);
            if (installmentAsset == null)
            {
                return NotFound();
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", installmentAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", installmentAsset.ShareId);
            ViewData["InstallmentStep2Id"] = new SelectList(_context.InstallmentStep2, "InstallmentStep2Id", "InstallmentStep2Id", installmentAsset.InstallmentStep2Id);
            return View(installmentAsset);
        }

        // POST: Admin/InstallmentAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstallmentAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileLink,SolutionFileId,BiddingDate,AssetBuyerName,AmountOfAssetSold,AggreementDate,AggreementNumber,AggreementFileLink,AggreementFileId,InstallmentTime,ActualInitPayment,PaymentPeriodType,ActualPayment,Status,InstallmentStep2Id,RealEstateId,ShareId")] InstallmentAsset installmentAsset)
        {
            if (id != installmentAsset.InstallmentAssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(installmentAsset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstallmentAssetExists(installmentAsset.InstallmentAssetId))
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
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", installmentAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", installmentAsset.ShareId);
            ViewData["InstallmentStep2Id"] = new SelectList(_context.InstallmentStep2, "InstallmentStep2Id", "InstallmentStep2Id", installmentAsset.InstallmentStep2Id);
            return View(installmentAsset);
        }

        // GET: Admin/InstallmentAssets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var installmentAsset = await _context.InstallmentAssets
                .Include(i => i.RealEstate)
                .Include(i => i.Share)
                .Include(i => i.Step2)
                .FirstOrDefaultAsync(m => m.InstallmentAssetId == id);
            if (installmentAsset == null)
            {
                return NotFound();
            }

            return View(installmentAsset);
        }

        // POST: Admin/InstallmentAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var installmentAsset = await _context.InstallmentAssets.FindAsync(id);
            _context.InstallmentAssets.Remove(installmentAsset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstallmentAssetExists(int id)
        {
            return _context.InstallmentAssets.Any(e => e.InstallmentAssetId == id);
        }
    }
}
