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
using System.Globalization;
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class ReductionInAssetsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NumberFormatInfo _cultureInfo; 

        public ReductionInAssetsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _cultureInfo = CultureInfo.InvariantCulture.NumberFormat; 
        }

        // GET: SimpleUser/ReductionInAssets
        public async Task<IActionResult> Index(int? activeId, float? assetValueBeforeDecline, bool success = false)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Success = success;

            ReductionInAsset reduction = new();
            if(activeId != null)
            {
               reduction = _context.ReductionInAssets.Find(activeId);
               ViewBag.ValueBeforeDecline = assetValueBeforeDecline;
               ViewBag.ValueAfterDecline = reduction.AssetValueAfterDecline;
            }

            return View(await _context.ReductionInAssets
                .Include(s => s.Share)
                .ThenInclude(h => h.SubmissionOnBiddings)
                .Include(s => s.RealEstate)
                .ThenInclude(r => r.SubmissionOnBiddings)
                .Where(s => s.RealEstate.ApplicationUserId == userId || s.Share.ApplicationUserId == userId).ToListAsync());
        }

       
        // GET: SimpleUser/ReductionInAssets/Create
        public IActionResult Create(int? id, int? target)
        {
            ViewBag.Target = target;
             
            ViewData["Id"] = id;

            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id).RealEstateName;
                ViewBag.MarketValue = _context.RealEstates.Where(r => r.RealEstateId == id)
                                                          .SelectMany(r => r.SubmissionOnBiddings).FirstOrDefault(a => a.Status == "Сотилмади" && a.IsActiveForPriceReduction == true).ActiveValue;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id).BusinessEntityName;
                ViewBag.MarketValue = _context.Shares.Where(r => r.ShareId == id)
                                                          .SelectMany(r => r.SubmissionOnBiddings).FirstOrDefault(a => a.Status == "Сотилмади" && a.IsActiveForPriceReduction == true).ActiveValue;
            }

            return View();
        }

        // POST: SimpleUser/ReductionInAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Create(int id, int target, string name, string marketValue, IFormFile solutionFile, [Bind("ReductionInAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileLink,Percentage,Amount,NumberOfSteps,AssetValueAfterDecline, IsInstallment")] ReductionInAsset reductionInAsset)
        {
            //SubmissionOnBidding submissionOnBidding = new();
            List<ReductionInAsset> reductionInAssets = new();
           
            RealEstate realEstate = new();
            Share share = new();

            if (target == 1)
            {
                reductionInAsset.RealEstateId = id;
                reductionInAssets = _context.ReductionInAssets.Where(r => r.RealEstateId == id && r.Status == true).ToList();
                realEstate = _context.RealEstates.Find(id);
                realEstate.SubmissionOnBiddingOn = false;
                realEstate.InstallmentAssetOn = false;
                realEstate.TransferredAssetOn = false;

            }

            if (target == 2)
            {
                reductionInAsset.ShareId = id;
                reductionInAssets = _context.ReductionInAssets.Where(r => r.ShareId == id && r.Status == true).ToList();
                share = _context.Shares.Find(id);
                share.SubmissionOnBiddingOn = false;
                share.InstallmentAssetOn = false;
                share.TransferredAssetOn = false;
            }

            reductionInAsset.AssetValueAfterDecline = Math.Round(float.Parse(reductionInAsset.AssetValueAfterDecline, _cultureInfo), 2).ToString();

            reductionInAsset.Status = true;

            if (ModelState.IsValid)
            {
                reductionInAsset.StatusChangedDate = DateTime.Now.AddYears(1000);
             
                _context.Add(reductionInAsset);
                await _context.SaveChangesAsync();

                foreach(var item in reductionInAssets)
                {
                    item.Status = false;
                    item.StatusChangedDate = reductionInAsset.SolutionDate;
                }

                await _context.SaveChangesAsync();

                var reduction_inAsset = _context.ReductionInAssets.Find(reductionInAsset.ReductionInAssetId);

                var createdFile = UploadFile(reduction_inAsset.ReductionInAssetId, solutionFile);

                reduction_inAsset.SolutionFileId = createdFile.Result.FileId;
                reduction_inAsset.SolutionFileLink = createdFile.Result.SystemPath;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ReductionInAssets", new { success = true, assetValueBeforeDecline = marketValue, activeId = reduction_inAsset.ReductionInAssetId});
            }

            ViewData["Id"] = id;

            ViewBag.Target = target;
            ViewData["Name"] = name;
            ViewBag.MarketValue = marketValue;

            return View(reductionInAsset);
        }

        [DisableRequestSizeLimit]
        public async Task<FileModel> UploadFile(int id, IFormFile file)
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
               await file.CopyToAsync(stream);
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
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.ReductionInAssetId== reductionInAssetId).FirstOrDefault();
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

                    reductionIn_asset.SolutionFileId = createdFile.Result.FileId;

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


        // GET: SimpleUser/ReductionInAssets/Edit/5
        public async Task<IActionResult> Edit(int? id, int? target)
        {
            if (id == null || target == null)
            {
                return NotFound();
            }

            string name = "";

            var reductionInAsset = await _context.ReductionInAssets.FindAsync(id);
            
            if (reductionInAsset == null)
            {
                return NotFound();
            }

            if(target == 1)
            {
                RealEstate realEstate = await _context.RealEstates.Include(r => r.ReductionInAssets)
                    .FirstOrDefaultAsync(r => r.ReductionInAssets.Any(a => a.ReductionInAssetId == id) == true);
                name = realEstate.RealEstateName;
                ViewBag.MarketValue = _context.RealEstates.Where(r => r.RealEstateId == realEstate.RealEstateId)
                                                         .SelectMany(r => r.SubmissionOnBiddings).FirstOrDefault(a => a.Status == "Сотилмади" && a.IsActiveForPriceReduction == true).ActiveValue;
            }

            if(target == 2)
            {
                Share share = await _context.Shares.Include(s => s.ReductionInAssets)
                    .FirstOrDefaultAsync(s => s.ReductionInAssets.Any(a => a.ReductionInAssetId == id) == true);
                name = share.BusinessEntityName;
                ViewBag.MarketValue = _context.Shares.Include(s=>s.SubmissionOnBiddings).Where(r => r.ShareId == share.ShareId)
                                                          .SelectMany(r => r.SubmissionOnBiddings).FirstOrDefault(a => a.Status == "Сотилмади" && a.IsActiveForPriceReduction == true).ActiveValue;

            }

            ViewBag.Name = name;

            return View(reductionInAsset);
        }

        // POST: SimpleUser/ReductionInAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string marketValue, [Bind("ReductionInAssetId,GoverningBodyName,RealEstateId,SolutionNumber,ShareId,SolutionDate,SolutionFileLink,SolutionFileId,Percentage,Amount,NumberOfSteps,AssetValueAfterDecline")] ReductionInAsset reductionInAsset)
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

            ViewBag.Name = name;
            ViewBag.MarketValue = marketValue;

            return View(reductionInAsset);
        }


        [HttpPost]
        public async Task<IActionResult> GetRealEstates([FromBody] int? target)
        {

            if (target == null)
            {
                return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            }
            //int target = (int)(data["target"]);

            List<ReductionInAsset> reductionInAssets = new();
            

            if(target == 0)
            {
                reductionInAssets = await _context.ReductionInAssets.Include(r => r.RealEstate)
                    .Where(r => r.Confirmed == false && r.RealEstate != null && r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(r => r.ReductionInAssetId).ToListAsync();
                
            }

            if(target == 1)
            {
                reductionInAssets = await _context.ReductionInAssets.Include(r => r.RealEstate)
                    .Where(r => r.Confirmed == true && r.Status == true && r.RealEstate != null && r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(r => r.ReductionInAssetId).ToListAsync();
            }

            if (target == 2)
            {
                reductionInAssets = await _context.ReductionInAssets.Include(r => r.RealEstate)
                    .Where(r => r.Status == false && r.RealEstate != null && r.RealEstate.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(r => r.ReductionInAssetId).ToListAsync();
            }

            List<GeneralViewModel> viewModels = new();

            foreach(var item in reductionInAssets)
            {
                var realEstate = await _context.RealEstates.FindAsync(item.RealEstateId);
                item.SolutionDateStr = item.SolutionDate.ToShortDateString();
                item.AssetValueAfterDecline = Math.Round(float.Parse(item.AssetValueAfterDecline, _cultureInfo), 2).ToString();
                GeneralViewModel viewModel = new()
                {
                    ReductionInAsset = item,
                    Name = realEstate.RealEstateName,
                    Target = 1

                };

                viewModels.Add(viewModel);  
            }

            return Json(new { data = viewModels});
        }

        [HttpPost]
        public async Task<IActionResult> GetShares([FromBody] int? target)
        {

            if (target == null)
            {
                return NotFound();
            }

            //int target = (int)data["target"];

            List<ReductionInAsset> reductionInAssets = new();
            

            if(target == 0)
            {
                reductionInAssets = await _context.ReductionInAssets.Include(r => r.Share)
                    .Where(r => r.Confirmed == false && r.Share != null && r.Share.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(r => r.ReductionInAssetId).ToListAsync();
                
            }

            if(target == 1)
            {
                reductionInAssets = await _context.ReductionInAssets.Include(r => r.Share)
                    .Where(r => r.Confirmed == true && r.Status == true && r.Share != null && r.Share.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(r => r.ReductionInAssetId).ToListAsync();
            }

            if (target == 2)
            {
                reductionInAssets = await _context.ReductionInAssets.Include(r => r.Share)
                    .Where(r => r.Status == false && r.Share != null && r.Share.ApplicationUserId == _userManager.GetUserId(User)).OrderByDescending(r => r.ReductionInAssetId).ToListAsync();
            }

            List<GeneralViewModel> viewModels = new();

            foreach(var item in reductionInAssets)
            {
                var share = await _context.Shares.FindAsync(item.ShareId);

                GeneralViewModel viewModel = new()
                {
                    ReductionInAsset = item,
                    Name = share.BusinessEntityName,
                    Target = 2

                };

                viewModels.Add(viewModel);  
            }

            return Json(new { data = viewModels});
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] JObject data)
        {
            if (data == null)
            {
                return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            }
            int id = Convert.ToInt32(data["id"]); 
            int target = Convert.ToInt32(data["target"]);   

            var reduction = await _context.ReductionInAssets.FindAsync(id);
            
            if (reduction != null)
                reduction.Confirmed = true;
            else
            {
                return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            }

            if (target == 1)
            {
                var realEstate = _context.RealEstates.Include(r => r.ReductionInAssets)
                    .Where(r => r.ReductionInAssets.Any(a => a.ReductionInAssetId == id)==true).FirstOrDefault();
               
                if(realEstate != null)
                    realEstate.SubmissionOnBiddingOn = true;
                else
                    return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            }
            if (target == 2)
            {
                var share = _context.Shares.Include(r => r.ReductionInAssets)
                    .Where(r => r.ReductionInAssets.Any(a => a.ReductionInAssetId == id) == true).FirstOrDefault();

                if (share != null)
                    share.SubmissionOnBiddingOn = true;
                else
                    return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            }
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Json(new { success = true, message = "Маълумотлар тасдиқланди!" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id, int? target)
        {
            if (id == null || target == null)
            {
                return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
            }

            var reductionInAsset = await _context.ReductionInAssets.FirstOrDefaultAsync(t => t.ReductionInAssetId == id);

            if (reductionInAsset == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }


            if (target == 1)
            {
                var realEstate = _context.RealEstates.Include(r => r.ReductionInAssets)
                    .FirstOrDefault(r => r.ReductionInAssets.Any(a => a.ReductionInAssetId == id) == true);
                realEstate.SubmissionOnBiddingOn = true;
                realEstate.InstallmentAssetOn = true;
                realEstate.TransferredAssetOn = true;
            }

            if (target == 2)
            {
                var share = _context.Shares.Include(r => r.ReductionInAssets)
                    .FirstOrDefault(r => r.ReductionInAssets.Any(a => a.ReductionInAssetId == id) == true);
                share.SubmissionOnBiddingOn = true;
                share.InstallmentAssetOn = true;
                share.TransferredAssetOn = true;
            }


            var fileModels = _context.FileModels.Where(f => f.ReductionInAssetId == reductionInAsset.ReductionInAssetId).ToList();

            if (fileModels.Any())
            {
                foreach (var item in fileModels)
                {
                    if (System.IO.File.Exists(item.FilePath))
                    {
                        System.IO.File.Delete(item.FilePath);
                    }
                    _context.FileModels.Remove(item);
                }
                
            }

            try
            {
                _context.ReductionInAssets.Remove(reductionInAsset);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


            return Json(new { success = true, message = "Ўчирилди" });
        }

        

        // POST: SimpleUser/ReductionInAssets/Delete/5
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
