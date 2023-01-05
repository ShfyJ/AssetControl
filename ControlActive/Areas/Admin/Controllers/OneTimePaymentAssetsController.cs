using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class OneTimePaymentAssetsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OneTimePaymentAssetsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/OneTimePaymentAssets
        public async Task<IActionResult> Index(bool success = false)
        {
            ViewBag.Success = success;

            var applicationDbContext = _context.OneTimePaymentAssets.Include(s => s.Share)
                .ThenInclude(h => h.ApplicationUser)
                .Include(s => s.RealEstate)
                .ThenInclude(h => h.ApplicationUser)
                .Include(s => s.Step2)
                .Include(s => s.Step3);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/OneTimePaymentAssets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oneTimePaymentAsset = await _context.OneTimePaymentAssets
                .Include(o => o.RealEstate)
                .Include(o => o.Share)
                .Include(o => o.Step2)
                .Include(o => o.Step3)
                .FirstOrDefaultAsync(m => m.OneTimePaymentAssetId == id);
            if (oneTimePaymentAsset == null)
            {
                return NotFound();
            }

            return View(oneTimePaymentAsset);
        }

        // GET: Admin/OneTimePaymentAssets/Create
        public IActionResult Create()
        {
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity");
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare");
            ViewData["OneTimePaymentStep2Id"] = new SelectList(_context.OneTimePaymentStep2, "OneTimePaymentStep2Id", "AmountOfAssetSold");
            ViewData["OneTimePaymentStep3Id"] = new SelectList(_context.OneTimePaymentStep3, "OneTimePaymentStep3Id", "OneTimePaymentStep3Id");
            return View();
        }

        // POST: Admin/OneTimePaymentAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OneTimePaymentAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileLink,SolutionFileId,BiddingDate,Status,RealEstateId,ShareId,OneTimePaymentStep2Id,OneTimePaymentStep3Id")] OneTimePaymentAsset oneTimePaymentAsset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oneTimePaymentAsset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", oneTimePaymentAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", oneTimePaymentAsset.ShareId);
            ViewData["OneTimePaymentStep2Id"] = new SelectList(_context.OneTimePaymentStep2, "OneTimePaymentStep2Id", "AmountOfAssetSold", oneTimePaymentAsset.OneTimePaymentStep2Id);
            ViewData["OneTimePaymentStep3Id"] = new SelectList(_context.OneTimePaymentStep3, "OneTimePaymentStep3Id", "OneTimePaymentStep3Id", oneTimePaymentAsset.OneTimePaymentStep3Id);
            return View(oneTimePaymentAsset);
        }

        // GET: Admin/OneTimePaymentAssets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(id);
            if (oneTimePaymentAsset == null)
            {
                return NotFound();
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", oneTimePaymentAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", oneTimePaymentAsset.ShareId);
            ViewData["OneTimePaymentStep2Id"] = new SelectList(_context.OneTimePaymentStep2, "OneTimePaymentStep2Id", "AmountOfAssetSold", oneTimePaymentAsset.OneTimePaymentStep2Id);
            ViewData["OneTimePaymentStep3Id"] = new SelectList(_context.OneTimePaymentStep3, "OneTimePaymentStep3Id", "OneTimePaymentStep3Id", oneTimePaymentAsset.OneTimePaymentStep3Id);
            return View(oneTimePaymentAsset);
        }

        // POST: Admin/OneTimePaymentAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OneTimePaymentAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileLink,SolutionFileId,BiddingDate,Status,RealEstateId,ShareId,OneTimePaymentStep2Id,OneTimePaymentStep3Id")] OneTimePaymentAsset oneTimePaymentAsset)
        {
            if (id != oneTimePaymentAsset.OneTimePaymentAssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oneTimePaymentAsset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OneTimePaymentAssetExists(oneTimePaymentAsset.OneTimePaymentAssetId))
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
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", oneTimePaymentAsset.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", oneTimePaymentAsset.ShareId);
            ViewData["OneTimePaymentStep2Id"] = new SelectList(_context.OneTimePaymentStep2, "OneTimePaymentStep2Id", "AmountOfAssetSold", oneTimePaymentAsset.OneTimePaymentStep2Id);
            ViewData["OneTimePaymentStep3Id"] = new SelectList(_context.OneTimePaymentStep3, "OneTimePaymentStep3Id", "OneTimePaymentStep3Id", oneTimePaymentAsset.OneTimePaymentStep3Id);
            return View(oneTimePaymentAsset);
        }

        // GET: Admin/OneTimePaymentAssets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oneTimePaymentAsset = await _context.OneTimePaymentAssets
                .Include(o => o.RealEstate)
                .Include(o => o.Share)
                .Include(o => o.Step2)
                .Include(o => o.Step3)
                .FirstOrDefaultAsync(m => m.OneTimePaymentAssetId == id);
            if (oneTimePaymentAsset == null)
            {
                return NotFound();
            }

            return View(oneTimePaymentAsset);
        }
        public FileModel UploadFile(int id, IFormFile file, int target)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/OneTimePaymentAssets/" + id.ToString());

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
            var systemPath = Path.Combine("/Files/OneTimePaymentAssets/" + id.ToString() + "/" + temp + extension);

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

            };

            if (target == 1)
            {
                fileModel.OneTimePaymentAssetId = id;
            }
            if (target == 2)
            {
                fileModel.OneTimePaymentStep2Id = id;
            }
            if (target == 3)
            {
                fileModel.OneTimePaymentStep3Id = id;
            }

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
        public async Task<IActionResult> ReplaceFile(int id, int fileId, IFormFile file, int target)
        {
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.OneTimePaymentAssetId == id).FirstOrDefault();

            OneTimePaymentAsset step1 = new();
            OneTimePaymentStep2 step2 = new();
            OneTimePaymentStep3 step3 = new();
            if (target == 1)
            {
                step1 = _context.OneTimePaymentAssets.Find(id);
            }
            if (target == 2)
            {
                step2 = _context.OneTimePaymentStep2.Find(id);
            }
            if (target == 1)
            {
                step3 = _context.OneTimePaymentStep3.Find(id);
            }

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
                    fileModel.SystemPath = Path.Combine("/Files/OneTimePaymentAssets/" + id.ToString() + "/" + temp + extension);
                    if (target == 1)
                    {

                        step1.SolutionFileLink = fileModel.SystemPath;

                    }
                    if (target == 3)
                    {

                        step3.ActAndAssetFileLink = fileModel.SystemPath;

                    }
                    if (target == 2)
                    {

                        step2.AggreementFileLink = fileModel.SystemPath;

                    }


                    await _context.SaveChangesAsync();

                }

                else if (file != null && fileModel == null)
                {
                    FileModel createdFile = new();

                    if (target == 1)
                    {
                        createdFile = UploadFile(id, file, 1);
                        step1.SolutionFileId = createdFile.FileId;
                        step1.SolutionFileLink = createdFile.SystemPath;
                    }
                    if (target == 2)
                    {
                        createdFile = UploadFile(id, file, 2);
                        step1.SolutionFileId = createdFile.FileId;
                        step1.SolutionFileLink = createdFile.SystemPath;
                    }
                    if (target == 1)
                    {
                        createdFile = UploadFile(id, file, 3);
                        step1.SolutionFileId = createdFile.FileId;
                        step1.SolutionFileLink = createdFile.SystemPath;
                    }

                    await _context.SaveChangesAsync();

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }


            //need changes here
            //return RedirectToAction("Edit", new { id = id});
            return RedirectToAction(nameof(Index));

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
        // POST: Admin/OneTimePaymentAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(id);
            _context.OneTimePaymentAssets.Remove(oneTimePaymentAsset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OneTimePaymentAssetExists(int id)
        {
            return _context.OneTimePaymentAssets.Any(e => e.OneTimePaymentAssetId == id);
        }
    }
}
