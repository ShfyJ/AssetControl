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
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class AssetEvaluationsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AssetEvaluationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }


        // GET: Admin/AssetEvaluations
        public async Task<IActionResult> Index(bool success=false)
        {
            ViewBag.Success = success;
            var applicationDbContext = _context.AssetEvaluations.Include(a => a.RealEstate).Include(a => a.Share);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/AssetEvaluations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetEvaluation = await _context.AssetEvaluations
                .Include(a => a.RealEstate)
                .Include(a => a.Share)
                .FirstOrDefaultAsync(m => m.AssetEvaluationId == id);
            if (assetEvaluation == null)
            {
                return NotFound();
            }

            return View(assetEvaluation);
        }

        // GET: Admin/AssetEvaluations/Create
        public IActionResult Create()
        {
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity");
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare");
            return View();
        }

        // POST: Admin/AssetEvaluations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetEvaluationId,EvaluatingOrgName,ReportDate,ReportRegNumber,ReportFileLink,ReportFileId,MarketValue,ExaminingOrgName,ExamReportDate,ExamReportRegNumber,ExamReportFileLink,ExamReportFileId,ReportStatus,Status,RealEstateId,ShareId")] AssetEvaluation assetEvaluation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assetEvaluation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", assetEvaluation.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", assetEvaluation.ShareId);
            return View(assetEvaluation);
        }

        // GET: Admin/AssetEvaluations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetEvaluation = await _context.AssetEvaluations.FindAsync(id);
            if (assetEvaluation == null)
            {
                return NotFound();
            }
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", assetEvaluation.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", assetEvaluation.ShareId);
            return View(assetEvaluation);
        }

        // POST: Admin/AssetEvaluations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetEvaluationId,EvaluatingOrgName,ReportDate,ReportRegNumber,ReportFileLink,ReportFileId,MarketValue,ExaminingOrgName,ExamReportDate,ExamReportRegNumber,ExamReportFileLink,ExamReportFileId,ReportStatus,Status,RealEstateId,ShareId")] AssetEvaluation assetEvaluation)
        {
            if (id != assetEvaluation.AssetEvaluationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetEvaluation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetEvaluationExists(assetEvaluation.AssetEvaluationId))
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
            ViewData["RealEstateId"] = new SelectList(_context.RealEstates, "RealEstateId", "Activity", assetEvaluation.RealEstateId);
            ViewData["ShareId"] = new SelectList(_context.Shares, "ShareId", "ActivityShare", assetEvaluation.ShareId);
            return View(assetEvaluation);
        }
        public FileModel UploadFile(int id, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/AssetEvaluations/" + id.ToString());

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

            var systemPath = Path.Combine("/Files/AssetEvaluations/" + id.ToString() + "/" + temp + extension);

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
                AssetEvaluationId = id
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
        public async Task<IActionResult> ReplaceFile(int assetEvaluationId, int fileId, IFormFile file, int finder)
        {
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.AssetEvaluationId == assetEvaluationId).FirstOrDefault();
            var asset_evaluation = _context.AssetEvaluations.Find(assetEvaluationId);
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
                    fileModel.SystemPath = Path.Combine("/Files/AssetEvaluations/", assetEvaluationId.ToString(), temp + extension);
                    if (finder == 0)
                    {

                        asset_evaluation.ReportFileLink = fileModel.SystemPath;

                    }
                    if (finder == 1)
                    {

                        asset_evaluation.ExamReportFileLink = fileModel.SystemPath;

                    }

                    await _context.SaveChangesAsync();

                }

                else if (file != null)
                {
                    var createdFile = UploadFile(assetEvaluationId, file);

                    if (finder == 0)
                    {
                        asset_evaluation.ReportFileId = createdFile.FileId;
                        asset_evaluation.ReportFileLink = createdFile.SystemPath;

                    }
                    if (finder == 1)
                    {
                        asset_evaluation.ExamReportFileId = createdFile.FileId;
                        asset_evaluation.ExamReportFileLink = createdFile.SystemPath;

                    }

                    await _context.SaveChangesAsync();

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }



            return RedirectToAction("Edit", new { id = assetEvaluationId });

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
        // GET: Admin/AssetEvaluations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetEvaluation = await _context.AssetEvaluations
                .Include(a => a.RealEstate)
                .Include(a => a.Share)
                .FirstOrDefaultAsync(m => m.AssetEvaluationId == id);
            if (assetEvaluation == null)
            {
                return NotFound();
            }

            return View(assetEvaluation);
        }

        // POST: Admin/AssetEvaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetEvaluation = await _context.AssetEvaluations.FindAsync(id);
            _context.AssetEvaluations.Remove(assetEvaluation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetEvaluationExists(int id)
        {
            return _context.AssetEvaluations.Any(e => e.AssetEvaluationId == id);
        }
    }
}
