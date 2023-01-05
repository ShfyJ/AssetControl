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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
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

        // GET: SimpleUser/TransferredAssets
        public IActionResult Index(bool success = false, bool editSuccess = false)
        {
            ViewBag.Success = success;
            ViewBag.EditSuccess = editSuccess;
            
            return View();
        }

        // GET: SimpleUser/TransferredAssets/Details/5
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

        // GET: SimpleUser/TransferredAssets/Create
        
        public IActionResult Create(int? id, int? target)
        {
            ViewBag.Target = target;
            ViewData["Id"] = id;

            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormName");
            
            if(target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id).RealEstateName;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id).BusinessEntityName;
            }

            return View();
        }

        // POST: SimpleUser/TransferredAssets/CreateR
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, string name, int target, IFormFile solutionFile, IFormFile actAndAssetFile, IFormFile agreementFile, [Bind("AssetId,TransferFormId,OrgName,SolutionNumber,SolutionDate,SolutionFileLink,OrgNameOfAsset,TotalCost,ActAndAssetDate,ActAndAssetNumber,ActAndAssetFileLink,AgreementDate,AgreementNumber,AgreementFileLink")] TransferredAsset transferredAsset)
        {
            RealEstate realEstate = new();
            Share share = new();


            if (ModelState.IsValid)
            {
                if (target == 1)
                {
                    realEstate = _context.RealEstates.Find(id);
                    realEstate.TransferredAssetOn = false;
                    realEstate.SubmissionOnBiddingOn = false;
                    realEstate.ReductionInAssetOn = false;
                    realEstate.AssetEvaluationOn = true;
                    realEstate.InstallmentAssetOn = false;
                    realEstate.OutOfAccountDate = transferredAsset.AgreementDate;
                    transferredAsset.RealEstateId = id;
                    
                }

                if (target == 2)
                {
                    share = _context.Shares.Find(id);
                    share.TransferredAssetOn = false;
                    share.SubmissionOnBiddingOn = false;
                    share.AssetEvaluationOn = true;
                    share.InstallmentAssetOn= false;
                    share.ReductionInAssetOn = false;   
                    share.OutOfAccountDate = transferredAsset.AgreementDate;
                    transferredAsset.ShareId = id;
                    
                }


                _context.Add(transferredAsset);
                await _context.SaveChangesAsync();

                if(target == 1)
                {
                    realEstate.TransferredAssetId = transferredAsset.AssetId;
                }

                if (target == 2)
                {
                    share.TransferredAssetId = transferredAsset.AssetId;
                }
                await _context.SaveChangesAsync();

                var transferred_Asset = _context.TransferredAssets.Find(transferredAsset.AssetId);

                List<IFormFile> files = new ();

                if (solutionFile != null)
                { files.Add(solutionFile); }

                if (actAndAssetFile != null)
                    files.Add(actAndAssetFile);

                if (agreementFile != null)
                    files.Add(agreementFile);


                foreach (var file in files)
                {
                    var createdFile = UploadFile(transferred_Asset.AssetId, file);

                    if (files.IndexOf(file) == 0)
                    {
                        transferred_Asset.SolutionFileId = createdFile.Result.FileId;
                        transferred_Asset.SolutionFileLink = createdFile.Result.SystemPath;

                    }
                    if (files.IndexOf(file) == 1)
                    {
                        transferred_Asset.ActAndAssetFileId = createdFile.Result.FileId;
                        transferred_Asset.ActAndAssetFileLink = createdFile.Result.SystemPath;

                    }
                    if (files.IndexOf(file) == 2)
                    {
                        transferredAsset.AgreementFileId = createdFile.Result.FileId;
                        transferredAsset.AggreementFileLink = createdFile.Result.SystemPath;

                    }

                }
                await _context.SaveChangesAsync();

                TempData["Message"] = "File successfully uploaded to File System.";

                return RedirectToAction("Index", "TransferredAssets", new { success = true });
            }
            ViewData["Id"] = id;
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormName");
            ViewBag.Target = target;
            ViewData["Name"] = name;

            return RedirectToAction("Create", "TransferredAssets");
        }


        public async Task<FileModel> UploadFile(int id, IFormFile file)
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
                        transferred_asset.SolutionFileId = createdFile.Result.FileId;
                        transferred_asset.SolutionFileLink = createdFile.Result.SystemPath;

                    }
                    if (finder == 1)
                    {
                        transferred_asset.ActAndAssetFileId = createdFile.Result.FileId;
                        transferred_asset.ActAndAssetFileLink = createdFile.Result.SystemPath;

                    }
                    if (finder == 2)
                    {
                        transferred_asset.AgreementFileId = createdFile.Result.FileId;
                        transferred_asset.AggreementFileLink = createdFile.Result.SystemPath;

                    }
                    
                    await _context.SaveChangesAsync();

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }



            return RedirectToAction("Edit", new { id = assetId});

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

        // GET: SimpleUser/TransferredAssets/Edit/5
        public async Task<IActionResult> Edit(int? id, int? id1, int? target)
        {
            if (id == null || id1 == null)
            {
                return NotFound();
            }
            ViewBag.Target = target;
            ViewData["Id1"] = id1;

          
            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id1).RealEstateName;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id1).BusinessEntityName;
            }
            var transferredAsset = await _context.TransferredAssets.FindAsync(id);
            if (transferredAsset == null)
            {
                return NotFound();
            }
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormName", transferredAsset.TransferFormId);
            return View(transferredAsset);
        }

        // POST: SimpleUser/TransferredAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? target,int? id1,[Bind("AssetId,TransferFormId,SolutionFileId,ActAndAssetFileId,OrgName,SolutionNumber,SolutionDate,SolutionFileLink,OrgNameOfAsset,TotalCost,ActAndAssetDate,ActAndAssetNumber,ActAndAssetFileLink,AgreementDate,AgreementNumber,AgreementFileLink,AgreementFileId")] TransferredAsset transferredAsset)
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
                return RedirectToAction("Index", "TransferredAssets", new { editSuccess = true });
            }
            ViewBag.Target = target;
            ViewData["Id1"] = id1;


            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id1).RealEstateName;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id1).BusinessEntityName;
            }
        
            ViewData["TransferFormId"] = new SelectList(_context.TransferForms, "TransferFormId", "TransferFormName", transferredAsset.TransferFormId);
            return View(transferredAsset);
        }

       [HttpGet]
       public IActionResult GetSentRealEstates()
        {

            var userId = _userManager.GetUserId(User);

            if (!_context.TransferredAssets.Include(t => t.RealEstate).Any())
            {
                return Json(null);
            }

            var transferredAssets = _context.TransferredAssets
               .Include(t => t.TransferForm)
               .Include(t => t.RealEstate)
               .Where(t => t.Share == null && t.RealEstate.ApplicationUserId == userId && t.Confirmed == true)
               .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in transferredAssets)
            {
                item.ActAndAssetDateStr = item.ActAndAssetDate.ToShortDateString();
                item.AgreementDateStr = item.AgreementDate.ToShortDateString();
                item.SolutionDateStr = item.SolutionDate.ToShortDateString();   

                GeneralViewModel viewModel = new GeneralViewModel()
                {
                    TransferredAsset = item,
                    Target = 1,
                    RealEstate = item.RealEstate

                };

                viewModels.Add(viewModel);

            }

            return Json(new {data = viewModels });
        }

        [HttpGet]
        public IActionResult GetUnSentRealEstates()
        {
            var userId = _userManager.GetUserId(User);

            if (!_context.TransferredAssets.Include(t => t.RealEstate).Any())
            {
                return Json(null);
            }

            var transferredAssets = _context.TransferredAssets
                    .Include(t => t.TransferForm)
                    .Include(t => t.RealEstate)
                    .Where(t => t.Share == null && t.RealEstate.ApplicationUserId == userId && t.Confirmed == false)
                    .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in transferredAssets)
            {
                item.ActAndAssetDateStr = item.ActAndAssetDate.ToShortDateString();
                item.AgreementDateStr = item.AgreementDate.ToShortDateString();
                item.SolutionDateStr = item.SolutionDate.ToShortDateString();

                GeneralViewModel viewModel = new GeneralViewModel()
                {
                    TransferredAsset = item,
                    Target = 1,
                    RealEstate = item.RealEstate

                };

                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }

        [HttpGet]
        public IActionResult GetSentShares()
        {
            var userId = _userManager.GetUserId(User);

            if (!_context.TransferredAssets.Include(t => t.Share).Any())
            {
                return Json(null);
            }

            var transferredAssets = _context.TransferredAssets
                 .Include(t => t.TransferForm)
                 .Include(t => t.Share)
                 .Where(t => t.RealEstate == null && t.Share.ApplicationUserId == userId && t.Confirmed == true)
                 .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in transferredAssets)
            {
                item.ActAndAssetDateStr = item.ActAndAssetDate.ToShortDateString();
                item.AgreementDateStr = item.AgreementDate.ToShortDateString();
                item.SolutionDateStr = item.SolutionDate.ToShortDateString();

                GeneralViewModel viewModel = new GeneralViewModel()
                {
                    TransferredAsset = item,
                    Target = 2,
                    Share = item.Share
                };

                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });

        }

        [HttpGet]
        public IActionResult GetUnSentShares()
        {
            var userId = _userManager.GetUserId(User);

            if (!_context.TransferredAssets.Include(t => t.Share).Any())
            {
                return Json(null);
            }

            var transferredAssets = _context.TransferredAssets
                 .Include(t => t.TransferForm)
                 .Include(t => t.Share)
                 .Where(t => t.RealEstate == null && t.Share.ApplicationUserId == userId && t.Confirmed == false)
                 .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in transferredAssets)
            {
                item.ActAndAssetDateStr = item.ActAndAssetDate.ToShortDateString();
                item.AgreementDateStr = item.AgreementDate.ToShortDateString();
                item.SolutionDateStr = item.SolutionDate.ToShortDateString();

                GeneralViewModel viewModel = new GeneralViewModel()
                {
                    TransferredAsset = item,
                    Target = 2,
                    Share = item.Share
                };

                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });

        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] JObject data)
        {
            if (data["id"] == null)
            {
                return NotFound();
            }

            int id = (int)data["id"];
            int targetId = (int)data["targetId"];
            int target = (int)data["target"];

            var asset = await _context.TransferredAssets.FindAsync(id);
            if (asset != null)
                asset.Confirmed = true;

            else
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FirstOrDefaultAsync(r => r.RealEstateId == targetId);
                realEstate.Status = false;
                realEstate.OutOfAccountDate = asset.ActAndAssetDate;
            }


            if (target == 2)
            {
                var share = await _context.Shares.FirstOrDefaultAsync(r => r.ShareId == targetId);
                share.Status = false;
                share.OutOfAccountDate = asset.ActAndAssetDate;
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
            if(id == null || target == null)
            {
                return Json(new { success = false, message = "Хатолик! - Қайтадан уриниб кўринг!" });
            }
            var transferredAsset = await _context.TransferredAssets.FirstOrDefaultAsync(t => t.AssetId == id);

            if(transferredAsset == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }

            var fileModels = _context.FileModels.Where(f => f.TransferredAssetId == transferredAsset.AssetId).ToList();

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
                await _context.SaveChangesAsync();
            }

            if (target == 1)
            {
                var realEstate = _context.RealEstates.Include(r => r.AssetEvaluations).FirstOrDefault(r => r.TransferredAssetId == id);
                if(realEstate != null)
                {
                    realEstate.TransferredAssetId = null;
                    realEstate.AssetEvaluationOn = true;
                    realEstate.TransferredAssetOn = true;
                    realEstate.InstallmentAssetOn = true;
                    
                    if(realEstate.AssetEvaluations.Any())
                        realEstate.SubmissionOnBiddingOn = true;

                }
                
            }

            if (target == 2)
            {
                var share = _context.Shares.Include(r => r.AssetEvaluations).FirstOrDefault(r => r.TransferredAssetId == id);
                share.TransferredAssetId = null;
                share.AssetEvaluationOn = true;
                share.TransferredAssetOn = true;
                share.InstallmentAssetOn = true;
                
                if(share.AssetEvaluations.Any())
                    share.SubmissionOnBiddingOn= true;
            }

            await _context.SaveChangesAsync();

            try
            {
                _context.TransferredAssets.Remove(transferredAsset);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                var e1x = ex;
                Console.WriteLine(e1x.Message); 
            }


            return Json(new {success = true, message = "Ўчирилди" });
        }

        private bool TransferredAssetExists(int id)
        {
            return _context.TransferredAssets.Any(e => e.AssetId == id);
        }
    }
}
