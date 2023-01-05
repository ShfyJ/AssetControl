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
using System.Globalization;
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class AssetEvaluationsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AssetEvaluationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment )
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: SimpleUser/AssetEvaluations
        public async Task<IActionResult> Index(bool success=false)
        {
            var userId = _userManager.GetUserId(User);

            ViewBag.Success = success;
            return View(await _context.AssetEvaluations
                .Include(a => a.RealEstate)
                .Include(a => a.Share)
                .Where(a => a.RealEstate.ApplicationUserId == userId || a.Share.ApplicationUserId == userId)
                .ToListAsync());
        }

        // GET: SimpleUser/AssetEvaluations/Create
        public IActionResult Create(int? id, int? target)
        {
            ViewBag.Target = target;
            ViewData["Id"] = id;

            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id).RealEstateName;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id).BusinessEntityName;
            }

            return View();
        }
       

        // POST: SimpleUser/AssetEvaluations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id,int target, string name, IFormFile reportFile, IFormFile examReportFile, [Bind("AssetEvaluationId,EvaluatingOrgName,ReportDate,ReportRegNumber,ReportFileLink,MarketValue,ExaminingOrgName,ExamReportDate,ExamReportRegNumber,ExamReportFileLink,ReportStatus")] AssetEvaluation assetEvaluation)
        {
            string assetName = "";
           
            if(target == 1)
            {
                assetEvaluation.RealEstateId = id;
                var realEstate = await _context.RealEstates.FindAsync(id);
                if (realEstate != null)
                    assetName = realEstate.RealEstateName;
            }

            if (target == 2)
            {
                assetEvaluation.ShareId = id;
                var share = await _context.Shares.FindAsync(id);
                if (share != null)
                    assetName = share.BusinessEntityName;
            }

            assetEvaluation.Status = false;
           
            if (ModelState.IsValid)
            {
                _context.Add(assetEvaluation);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, assetName);

                var asset_evaluation = _context.AssetEvaluations.Find(assetEvaluation.AssetEvaluationId);

                List<IFormFile> files = new ();

                if (reportFile != null)
                { files.Add(reportFile); }

                if (examReportFile != null)
                    files.Add(examReportFile);

                foreach (var file in files)
                {
                    var createdFile = UploadFile(asset_evaluation.AssetEvaluationId, file);


                    if (files.IndexOf(file) == 0)
                    {
                        assetEvaluation.ReportFileId = createdFile.Result.FileId;
                        assetEvaluation.ReportFileLink = createdFile.Result.SystemPath;

                    }
                    if (files.IndexOf(file) == 1)
                    {
                        assetEvaluation.ExamReportFileId = createdFile.Result.FileId;
                        assetEvaluation.ExamReportFileLink = createdFile.Result.SystemPath;

                    }

                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "AssetEvaluations", new { success = true });

            }

           
            ViewData["Id"] = id;
            
            ViewBag.Target = target;
            ViewData["Name"] = name;

            return View(assetEvaluation);
            
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] JObject data)
        {
            if (data["id"] == null || data["target"] == null)
            {
                return Json(new { success = false, message = "Хатолик!" });
            }

            int id = (int)data["id"];
            int target = (int)data["target"];

            var assetEvaluation = await _context.AssetEvaluations.FindAsync(id);
            if (assetEvaluation != null)
                assetEvaluation.Confirmed = true;

            if (assetEvaluation.ReportStatus)
            {
                assetEvaluation.Status = true;                
            }

            assetEvaluation.StatusChangedDate = DateTime.Now.AddYears(1000);

            List<AssetEvaluation> assetEvaluations = new();
            string assetName = "";

            if (target == 1)
            {
                assetEvaluations = _context.AssetEvaluations.Where(a => a.RealEstateId == assetEvaluation.RealEstateId && a.Status == true).ToList();

                var realEstate = _context.RealEstates.Find(assetEvaluation.RealEstateId);
                
                if(realEstate != null)
                {
                    if (realEstate.TransferredAssetId == null)
                    {
                        realEstate.SubmissionOnBiddingOn = true;
                    }
                    assetName = realEstate.RealEstateName;
                }
                
            }

            if (target == 2)
            {
                assetEvaluations = _context.AssetEvaluations.Where(a => a.ShareId == assetEvaluation.ShareId && a.Status == true).ToList();

                var share = _context.Shares.Find(assetEvaluation.ShareId);
               
                if(share != null)
                {
                    if (share.TransferredAssetId == null)
                    {
                        share.SubmissionOnBiddingOn = true;
                    }
                    assetName = share.BusinessEntityName;
                }
            }
               

            foreach (var item in assetEvaluations)
            {
                item.Status = false;
                item.StatusChangedDate = assetEvaluation.ReportDate;
            }


            try
            {
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, assetName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Json(new { success = true, message = "Маълумотлар тасдиқланди!" });
        }


        public async Task<FileModel> UploadFile(int id, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/AssetEvaluations/" + id.ToString());

            bool basePathExists = Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var filePath = Path.Combine(basePath, file.FileName);
            var extension = Path.GetExtension(file.FileName);
            string temp=fileName;

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
                AssetEvaluationId = id
            };
            _context.FileModels.Add(fileModel);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

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
            var asset_evaluation= await _context.AssetEvaluations.Include(a => a.RealEstate).Include(a => a.Share).FirstOrDefaultAsync(a => a.AssetEvaluationId == assetEvaluationId);
            
            if (asset_evaluation == null)
                return NotFound();

            string assetName = "";
            if (asset_evaluation.RealEstate != null)
            {
                assetName = asset_evaluation.RealEstate.RealEstateName;
            }
            if(asset_evaluation.Share != null)
            {
                assetName = asset_evaluation.Share.BusinessEntityName;
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
                    fileModel.SystemPath = Path.Combine("/Files/AssetEvaluations/", assetEvaluationId.ToString(), temp + extension);
                    if (finder == 0)
                    {

                        asset_evaluation.ReportFileLink = fileModel.SystemPath;

                    }
                    if (finder == 1)
                    {

                        asset_evaluation.ExamReportFileLink = fileModel.SystemPath;

                    }

                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, assetName);

                }

                else if (file != null)
                {
                    var createdFile = UploadFile(assetEvaluationId, file);

                    if (finder == 0)
                    {
                        asset_evaluation.ReportFileId = createdFile.Result.FileId;
                        asset_evaluation.ReportFileLink = createdFile.Result.SystemPath;

                    }
                    if (finder == 1)
                    {
                        asset_evaluation.ExamReportFileId = createdFile.Result.FileId;
                        asset_evaluation.ExamReportFileLink = createdFile.Result.SystemPath;

                    }
                    
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, assetName);

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
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex);
            }


            TempData["Message"] = $"Removed {file.Name + file.Extension} successfully from File System.";

            return View();
        }


        // GET: SimpleUser/AssetEvaluations/Edit/5
        public async Task<IActionResult> Edit(int? id, int? target)
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
            ViewBag.Target = target;
          
            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == assetEvaluation.RealEstateId).RealEstateName;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == assetEvaluation.ShareId).BusinessEntityName;
            }

            return View(assetEvaluation);
        }

        // POST: SimpleUser/AssetEvaluations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, int target, [Bind("AssetEvaluationId,EvaluatingOrgName,ReportDate,ReportRegNumber,ReportFileLink,ReportFileId,ExamReportFileId,RealEstateId,ShareId,MarketValue,ExaminingOrgName,ExamReportDate,ExamReportRegNumber,ExamReportFileLink,ReportStatus")] AssetEvaluation assetEvaluation)
        {
            if (id != assetEvaluation.AssetEvaluationId)
            {
                return NotFound();
            }
            var asset_eval = await _context.AssetEvaluations.FindAsync(id);
            
            if(asset_eval == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                if(asset_eval.EvaluatingOrgName != assetEvaluation.EvaluatingOrgName)
                    asset_eval.EvaluatingOrgName = assetEvaluation.EvaluatingOrgName;
                if(asset_eval.ReportDate != assetEvaluation.ReportDate)
                    asset_eval.ReportDate = assetEvaluation.ReportDate;
                if(asset_eval.ReportRegNumber != assetEvaluation.ReportRegNumber)
                    asset_eval.ReportRegNumber = assetEvaluation.ReportRegNumber;
                if (asset_eval.ExaminingOrgName != assetEvaluation.ExaminingOrgName)
                    asset_eval.ExaminingOrgName = assetEvaluation.ExaminingOrgName;
                if (asset_eval.ExamReportDate != assetEvaluation.ExamReportDate)
                    asset_eval.ExamReportDate = assetEvaluation.ExamReportDate;
                if (asset_eval.ExamReportRegNumber != assetEvaluation.ExamReportRegNumber)
                    asset_eval.ExamReportRegNumber = assetEvaluation.ExamReportRegNumber;
                if (asset_eval.ReportStatus != assetEvaluation.ReportStatus)
                    asset_eval.ReportStatus = assetEvaluation.ReportStatus;

                try
                {
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, name);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AssetEvaluations.Where(s => s.AssetEvaluationId == id).Any())
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

            ViewBag.Target = target;
            ViewData["Name"] = name;
            return View(assetEvaluation);
        }


        [HttpGet]
        public IActionResult GetSentRealEstates()
        {

            var userId = _userManager.GetUserId(User);
            if (!_context.AssetEvaluations.Include(t => t.RealEstate).Any())
            {
                return Json(null);
            }

            var assetEvaluations = _context.AssetEvaluations
                 .Include(t => t.RealEstate)
                 .Where(t => t.Share == null && t.RealEstate.ApplicationUserId == userId && t.Confirmed == true)
                 .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in assetEvaluations)
            {
                item.ReportDateStr = item.ReportDate.ToShortDateString();
                item.ExamReportDateStr = item.ExamReportDate.ToShortDateString();

                if (item.ReportStatus)
                {
                    item.ReportStatusStr = "ишончли";    
                }

                else
                {
                    item.ReportStatusStr = "ишончсиз";
                }

                GeneralViewModel viewModel = new()
                {
                    AssetEvaluation = item,
                    Target = 1,
                    RealEstate = item.RealEstate
                };

                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });
        }

        [HttpGet]
        public IActionResult GetUnSentRealEstates()
        {
            var userId = _userManager.GetUserId(User);

            if (!_context.AssetEvaluations.Include(t => t.RealEstate).Any())
            {
                return Json(null);
            }

            var assetEvaluations = _context.AssetEvaluations
                 .Include(t => t.RealEstate)
                 .Where(t => t.Share == null && t.RealEstate.ApplicationUserId == userId && t.Confirmed == false)
                 .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in assetEvaluations)
            {
                item.ReportDateStr = item.ReportDate.ToShortDateString();
                item.ExamReportDateStr = item.ExamReportDate.ToShortDateString();

                if (item.ReportStatus)
                {
                    item.ReportStatusStr = "ишончли";
                }

                else
                {
                    item.ReportStatusStr = "ишончсиз";
                }

                GeneralViewModel viewModel = new()
                {
                    AssetEvaluation = item,
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

            if (!_context.AssetEvaluations.Include(t => t.Share).Any())
            {
                return Json(null);
            }

            var assetEvaluations = _context.AssetEvaluations
                 .Include(t => t.Share)
                 .Where(t => t.RealEstate == null && t.Share.ApplicationUserId == userId && t.Confirmed == true)
                 .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in assetEvaluations)
            {
                if (item.ReportStatus)
                {
                    item.ReportStatusStr = "ишончли";
                }

                else
                {
                    item.ReportStatusStr = "ишончсиз";
                }

                item.ReportDateStr = item.ReportDate.ToShortDateString();
                item.ExamReportDateStr = item.ExamReportDate.ToShortDateString();

                GeneralViewModel viewModel = new()
                {
                    AssetEvaluation = item,
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

            if (!_context.AssetEvaluations.Include(t => t.Share).Any())
            {
                return Json(null);
            }

            var assetEvaluations = _context.AssetEvaluations
                 .Include(t => t.Share)
                 .Where(t => t.RealEstate == null && t.Share.ApplicationUserId == userId && t.Confirmed == false)
                 .ToList();

            List<GeneralViewModel> viewModels = new();

            foreach (var item in assetEvaluations)
            {
                item.ReportDateStr = item.ReportDate.ToShortDateString();
                item.ExamReportDateStr = item.ExamReportDate.ToShortDateString();

                if (item.ReportStatus)
                {
                    item.ReportStatusStr = "ишончли";
                }

                else
                {
                    item.ReportStatusStr = "ишончсиз";
                }

                GeneralViewModel viewModel = new()
                {
                    AssetEvaluation = item,
                    Target = 2,
                    Share = item.Share
                };

                viewModels.Add(viewModel);

            }

            return Json(new { data = viewModels });

        }

       

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var assetEval = await _context.AssetEvaluations.Include(a => a.RealEstate).Include(a => a.Share).FirstOrDefaultAsync(t => t.AssetEvaluationId == id);
            
            string assetName = "";
            
            if (assetEval == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }
            if (assetEval.RealEstate != null)
                assetName = assetEval.RealEstate.RealEstateName;

            if (assetEval.Share != null)
                assetName = assetEval.Share.BusinessEntityName;

            var fileModels = _context.FileModels.Where(f => f.AssetEvaluationId == assetEval.AssetEvaluationId).ToList();

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
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, assetName);
            }

            try
            {
                _context.AssetEvaluations.Remove(assetEval);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, assetName);
            }

            catch (Exception ex)
            {
                var e1x = ex;
            }


            return Json(new { success = true, message = "Ўчирилди" });
        }

       
    }
}
