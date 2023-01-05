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
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class TransferredAssetsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TransferredAssetsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }


        // GET: Admin/TransferredAssets
        public async Task<IActionResult> Index(bool success = false)
        {
            ViewBag.Success = success;
            var applicationDbContext = _context.TransferredAssets.Include(t => t.TransferForm).Include(t => t.Share)
           
                .Include(t => t.RealEstate); 
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/TransferredAssets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferredAsset = await _context.TransferredAssets
                .Include(t => t.TransferForm)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (transferredAsset == null)
            {
                return NotFound();
            }

            return View(transferredAsset);
        }

        public FileModel UploadFile(int id, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/TransferredAssets/" + id.ToString());

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
            var systemPath = Path.Combine("/Files/TransferredAssets/" + id.ToString() + "/" + temp + extension);

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
                TransferredAssetId = id
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
                if (file != null && System.IO.File.Exists(file.FilePath))
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
        public async Task<IActionResult> ReplaceFile(int assetId, int fileId, IFormFile file, int finder)
        {
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.TransferredAssetId == assetId).FirstOrDefault();
            var transferred_asset = _context.TransferredAssets.Find(assetId);
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
                    fileModel.SystemPath = Path.Combine("/Files/TransferredAssets/" + assetId.ToString() + "/" + temp + extension);
                    if (finder == 0)
                    {

                        transferred_asset.SolutionFileLink = fileModel.SystemPath;

                    }
                    if (finder == 1)
                    {

                        transferred_asset.ActAndAssetFileLink = fileModel.SystemPath;

                    }
                    if (finder == 2)
                    {

                        transferred_asset.AggreementFileLink = fileModel.SystemPath;

                    }

                    await _context.SaveChangesAsync();

                }

                else if (file != null)
                {
                    var createdFile = UploadFile(assetId, file);

                    if (finder == 0)
                    {
                        transferred_asset.SolutionFileId = createdFile.FileId;
                        transferred_asset.SolutionFileLink = createdFile.SystemPath;

                    }
                    if (finder == 1)
                    {
                        transferred_asset.ActAndAssetFileId = createdFile.FileId;
                        transferred_asset.ActAndAssetFileLink = createdFile.SystemPath;

                    }
                    if (finder == 2)
                    {
                        transferred_asset.AgreementFileId = createdFile.FileId;
                        transferred_asset.AggreementFileLink = createdFile.SystemPath;

                    }

                    await _context.SaveChangesAsync();

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }



            return RedirectToAction("Edit", new { id = assetId });

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
        // GET: Admin/TransferredAssets/Create
        public IActionResult Create()
        {
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormId");
            return View();
        }

        // POST: Admin/TransferredAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetId,TransferFormId,OrgName,SolutionNumber,SolutionDate,SolutionFileLink,SolutionFileId,OrgNameOfAsset,TotalCost,ActAndAssetDate,ActAndAssetNumber,ActAndAssetFileLink,ActAndAssetFileId,AgreementDate,AgreementNumber,AggreementFileLink,AgreementFileId")] TransferredAsset transferredAsset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transferredAsset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormId", transferredAsset.TransferFormId);
            return View(transferredAsset);
        }

        // GET: Admin/TransferredAssets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferredAsset = await _context.TransferredAssets.FindAsync(id);
            if (transferredAsset == null)
            {
                return NotFound();
            }
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormId", transferredAsset.TransferFormId);
            return View(transferredAsset);
        }

        // POST: Admin/TransferredAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetId,TransferFormId,OrgName,SolutionNumber,SolutionDate,SolutionFileLink,SolutionFileId,OrgNameOfAsset,TotalCost,ActAndAssetDate,ActAndAssetNumber,ActAndAssetFileLink,ActAndAssetFileId,AgreementDate,AgreementNumber,AggreementFileLink,AgreementFileId")] TransferredAsset transferredAsset)
        {
            if (id != transferredAsset.AssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transferredAsset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferredAssetExists(transferredAsset.AssetId))
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
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormId", transferredAsset.TransferFormId);
            return View(transferredAsset);
        }

        // GET: Admin/TransferredAssets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferredAsset = await _context.TransferredAssets
                .Include(t => t.TransferForm)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (transferredAsset == null)
            {
                return NotFound();
            }

            return View(transferredAsset);
        }

        // POST: Admin/TransferredAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transferredAsset = await _context.TransferredAssets.FindAsync(id);
            _context.TransferredAssets.Remove(transferredAsset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransferredAssetExists(int id)
        {
            return _context.TransferredAssets.Any(e => e.AssetId == id);
        }
    }
}
