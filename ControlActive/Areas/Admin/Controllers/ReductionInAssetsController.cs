using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class ReductionInAssetsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReductionInAssetsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/ReductionInAssets
        public async Task<IActionResult> Index(int? activeId, float? assetValueBeforeDecline, bool success = false)
        {
            ViewBag.Success = success;

            ReductionInAsset reduction = new();
            if (activeId != null)
            {
                reduction = _context.ReductionInAssets.Find(activeId);
                ViewBag.ValueBeforeDecline = assetValueBeforeDecline;
                ViewBag.ValueAfterDecline = reduction.AssetValueAfterDecline;
            }

            var applicationDbContext = _context.ReductionInAssets.Include(r => r.RealEstate).ThenInclude(h => h.SubmissionOnBiddings).Include(r => r.Share).ThenInclude(h => h.SubmissionOnBiddings);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/ReductionInAssets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reductionInAsset = await _context.ReductionInAssets
                .Include(r => r.RealEstate)
                .Include(r => r.Share)
                .FirstOrDefaultAsync(m => m.ReductionInAssetId == id);
            if (reductionInAsset == null)
            {
                return NotFound();
            }

            return View(reductionInAsset);
        }

        // GET: Admin/ReductionInAssets/Create
        public IActionResult Create()
        {
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity");
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare");
            return View();
        }

        // POST: Admin/ReductionInAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReductionInAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileId,Percentage,Amount,NumberOfSteps,AssetValueAfterDecline,Status,RealEstateId,ShareId")] ReductionInAsset reductionInAsset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reductionInAsset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", reductionInAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", reductionInAsset.ShareId);
            return View(reductionInAsset);
        }

        // GET: Admin/ReductionInAssets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reductionInAsset = await _context.ReductionInAssets.FindAsync(id);
            if (reductionInAsset == null)
            {
                return NotFound();
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", reductionInAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", reductionInAsset.ShareId);
            return View(reductionInAsset);
        }

        // POST: Admin/ReductionInAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReductionInAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileId,Percentage,Amount,NumberOfSteps,AssetValueAfterDecline,Status,RealEstateId,ShareId")] ReductionInAsset reductionInAsset)
        {
            if (id != reductionInAsset.ReductionInAssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reductionInAsset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReductionInAssetExists(reductionInAsset.ReductionInAssetId))
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
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", reductionInAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", reductionInAsset.ShareId);
            return View(reductionInAsset);
        }
        [DisableRequestSizeLimit]
        public FileModel UploadFile(int id, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/ReductionInAssets/" + id.ToString());

            bool basePathExists = Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var filePath = Path.Combine(basePath, file.FileName);
            var extension = Path.GetExtension(file.FileName);
            string temp = fileName;

            for (int i = 1; ; i++)
            {
                if (System.IO.File.Exists(filePath))
                {
                    temp = "";
                    temp += fileName + "(" + i.ToString() + ")";
                    filePath = Path.Combine(basePath, temp + extension);
                    continue;
                }

                break;
            }

            var systemPath = Path.Combine("/Files/ReductionInAssets/" + id.ToString() + "/" + temp + extension);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(stream);
            }
            var fileModel = new FileModel
            {
                CreatedOn = DateTime.UtcNow,
                FileType = file.ContentType,
                Extension = extension,
                Name = temp,
                FilePath = filePath,
                SystemPath = systemPath,
                BasePath = basePath,
                ReductionInAssetId = id
            };
            _context.FileModels.Add(fileModel);
            _context.SaveChanges();

            var createdFile = _context.FileModels.Where(f => f.FilePath == filePath).FirstOrDefault();

            return createdFile;

        }

        public async Task<IActionResult> DownloadFile(int id)
        {

            var file = await _context.FileModels.Where(x => x.FileId == id).FirstOrDefaultAsync();

            try
            {
                if (file != null)
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(file.FilePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, file.FileType, file.Name + file.Extension);
                }
                return NotFound("Not Found");

            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex);
            }

        }


        [HttpPost]
        public async Task<IActionResult> ReplaceFile(int reductionInAssetId, int fileId, IFormFile file)
        {
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.ReductionInAssetId == reductionInAssetId).FirstOrDefault();
            var reductionIn_asset = _context.ReductionInAssets.Find(reductionInAssetId);
            try
            {
                if (fileModel != null && file != null)
                {
                    if (System.IO.File.Exists(fileModel.FilePath))
                    {
                        System.IO.File.Delete(fileModel.FilePath);
                    }


                    bool basePathExists = Directory.Exists(fileModel.BasePath);
                    if (!basePathExists) Directory.CreateDirectory(fileModel.BasePath);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var filePath = Path.Combine(fileModel.BasePath, file.FileName);
                    var extension = Path.GetExtension(file.FileName);

                    string temp = fileName;

                    for (int i = 1; ; i++)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            temp = "";
                            temp += fileName + "(" + i.ToString() + ")";
                            filePath = Path.Combine(fileModel.BasePath, temp + extension);
                            continue;
                        }

                        break;
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    fileModel.CreatedOn = DateTime.UtcNow;
                    fileModel.Extension = extension;
                    fileModel.FilePath = filePath;
                    fileModel.FileType = file.ContentType;
                    fileModel.Name = fileName;
                    fileModel.SystemPath = Path.Combine("/Files/ReductionInAssets/" + reductionInAssetId.ToString() + "/" + temp + extension);

                    await _context.SaveChangesAsync();

                }

                else if (file != null)
                {
                    var createdFile = UploadFile(reductionInAssetId, file);

                    reductionIn_asset.SolutionFileId = createdFile.FileId;

                    await _context.SaveChangesAsync();

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }

            return RedirectToAction("Edit", new { id = reductionInAssetId });

        }

        public async Task<IActionResult> DeleteFile(int id)
        {

            var file = await _context.FileModels.Where(x => x.FileId == id).FirstOrDefaultAsync();

            try
            {
                if (file != null)
                {
                    if (System.IO.File.Exists(file.FilePath))
                    {
                        System.IO.File.Delete(file.FilePath);
                    }
                    _context.FileModels.Remove(file);
                    await _context.SaveChangesAsync();
                }

            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex);
            }


            TempData["Message"] = $"Removed {file.Name + file.Extension} successfully from File System.";

            return View();
        }
        // GET: Admin/ReductionInAssets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reductionInAsset = await _context.ReductionInAssets
                .Include(r => r.RealEstate)
                .Include(r => r.Share)
                .FirstOrDefaultAsync(m => m.ReductionInAssetId == id);
            if (reductionInAsset == null)
            {
                return NotFound();
            }

            return View(reductionInAsset);
        }

        // POST: Admin/ReductionInAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reductionInAsset = await _context.ReductionInAssets.FindAsync(id);
            _context.ReductionInAssets.Remove(reductionInAsset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReductionInAssetExists(int id)
        {
            return _context.ReductionInAssets.Any(e => e.ReductionInAssetId == id);
        }
    }
}
