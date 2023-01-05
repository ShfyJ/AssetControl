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
using Microsoft.AspNetCore.Identity;
using System.IO;
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ControlActive.ViewModels;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class SharesController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SharesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: SimpleUser/Shares
        public ActionResult Index(bool success = false, bool editSuccess = false)
        {
          
            ViewBag.Success = success;
            ViewBag.EditSuccess = editSuccess;
            
            return View();
        }

        public async Task<IActionResult> SelectShare(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            ViewBag.Id = id;

            //«Переданные активы»
            if (id == 1)
            {
                return View(await _context.Shares.Where(r => r.TransferredAssetOn == true && r.Confirmed == true
                 && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Оценка актива»
            if (id == 2)
            {
                return View(await _context.Shares.Where(r => r.AssetEvaluationOn == true && r.Confirmed == true && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Выставление на торги»
            if (id == 3)
            {
                return View(await _context.Shares.Include(r => r.AssetEvaluations).Where(r => r.SubmissionOnBiddingOn == true && r.Confirmed == true && r.ApplicationUserId == userId && r.AssetEvaluations.Count != 0).ToListAsync());
            }

            //«Пошаговое снижение стоимости актива» 
            if (id == 4)
            {
                return View(await _context.Shares.Include(r => r.SubmissionOnBiddings).Where(r => r.Confirmed == true && r.SubmissionOnBiddings.Any(s => s.IsActiveForPriceReduction == true) == true
                && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Реализованные активы с единовременной оплатой»
            if (id == 5)
            {
                return View(await _context.Shares.Include(r => r.SubmissionOnBiddings).Where(r => r.OneTimePaymentAssetOn == true && r.Confirmed == true && r.SubmissionOnBiddings.Any(s => s.Status == "Сотувда" || s.Status == "Сотилди") == true
                && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Активы, реализованные в рассрочку»
            if (id == 6)
            {
                return View(await _context.Shares.Include(r => r.SubmissionOnBiddings).Where(r => r.InstallmentAssetOn == true  && r.Confirmed == true && r.ApplicationUserId == userId).ToListAsync());
            }

            return NotFound();
        }


        public async Task<IActionResult> GetUsersIds()
        {
            var user = await _context.ApplicationUsers.FirstAsync(s => s.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return Json(new { success = false, message = "Xатолик юз берди!" });
            }

            var ToUser = _context.ApplicationUsers.FirstOrDefault(s => s.Id == user.CreatedById);

            if (ToUser == null)
            {
                return Json(new { success = false, message = "Xатолик юз берди!" });
            }

            string[] ids = { user.Id, ToUser.Id };

            return Json(new { data = ids, success = true });

        }

        [HttpPost]
        public IActionResult GetShare([FromBody] JObject data)
        {
            if (data["id"] == null || data["forDetails"] == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            int id = (int)(data["id"]);
            bool forDetails = (bool)(data["forDetails"]);

            var share = _context.Shares
                .Include(r => r.DistrictOfObject)
                .Include(r => r.RegionOfObject)
                .FirstOrDefault(r => r.ShareId == id);

            if (share == null)
            {
                return Json(new { success = false, message = "Объект топилмади!" });
            }

            share.FoundationYearStr = share.FoundationYear.ToShortDateString();
            share.StateRegistrationDateStr = share.StateRegistrationDate.ToShortDateString();

           
            List<Share> shareData = new()
            {
                share
            };

            if (forDetails)
                return Json(new { data = shareData });

            return Json(new { data = share, success = true });
        }
        // GET: SimpleUser/Shares/Create
        public IActionResult Create()
        {
            
            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName");
           
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName");
            
            var userId = _userManager.GetUserId(User);
            var organizationId = _context.ApplicationUsers.FirstOrDefault(a => a.Id == userId).OrganizationId;
            var name = _context.Organizations.FirstOrDefault(u => u.OrganizationId == organizationId).OrganizationName;
            ViewBag.Name = name;

            return View();
        }


        // POST: SimpleUser/Shares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[]shareHolders, string[] AmountFromAuthCapitalHolder, IFormFile RegCertificate, IFormFile OrgCharter, IFormFile BalanceSheet, IFormFile FinancialResult, IFormFile AuditConclusion,
            [Bind("ShareId,BusinessEntityName,IdRegNumber,ParentOrganization,Activities,ActivityShare,FoundationYear,StateRegistrationDate,RegCertificateLink,OrgCharterLink,ApplicationUserId,TransferredAssetId,RegionId,DistrictId,Address,AuthorizedCapital,NumberOfShares,ParValueOfShares,AdministrativeStaff,ProductionPersonal,AverageMonthlySalary,MaintanenceCostForYear,ProductionArea,BuildingsArea,AmountPayable,AmountReceivable,Year1,ProfitOrLossOfYear1,Year2,ProfitOrLossOfYear2,Year3,ProfitOrLossOfYear3,Comments")] Share share)
        {
            if (ModelState.IsValid)
            {
                //share.ShareHolderName = String.Join("; ", shareHolders);
               
                //share.AmountFromAuthCapital = String.Join("; ", AmountFromAuthCapitalHolder);

                var userId = _userManager.GetUserId(User);
                share.ApplicationUserId = userId;
                
                share.Status = true;
                share.OutOfAccountDate = DateTime.Now.AddYears(1000);

                _context.Add(share);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);

                var new_share = _context.Shares.Find(share.ShareId);

                for(int i = 0; i<shareHolders.Length; i++)
                {

                    Shareholder shareholder = new()
                    {
                        ShareholderName = shareHolders[i],
                        AmountFromAuthCapital = AmountFromAuthCapitalHolder[i]
                    };

                    _context.Add(shareholder);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);

                    SharesAndHolders shareAndHolder = new()
                    {
                        ShareholderId = shareholder.ShareholderId,
                        ShareId = share.ShareId
                    };

                    _context.Add(shareAndHolder);
                    await _context.SaveChangesAsync();
                }

                List<IFormFile> files = new ();

                if (RegCertificate != null)
                { files.Add(RegCertificate); }

                if (OrgCharter != null)
                    files.Add(OrgCharter);

                if (BalanceSheet != null)
                    files.Add(BalanceSheet);

                if (FinancialResult != null)
                    files.Add(FinancialResult);

                if (AuditConclusion != null)
                    files.Add(AuditConclusion);

                foreach (var file in files)
                {
                    var createdFile = UploadFile(new_share.ShareId, file);

                    if (files.IndexOf(file) == 0)
                    {
                        new_share.RegCertificateId = createdFile.Result.FileId;
                        new_share.RegCertificateLink = createdFile.Result.SystemPath;

                    }
                    if (files.IndexOf(file) == 1)
                    {
                        new_share.OrgCharterId = createdFile.Result.FileId;
                        new_share.OrgCharterLink = createdFile.Result.SystemPath;

                    }

                    if (files.IndexOf(file) == 2)
                    {
                        new_share.BalanceSheetId = createdFile.Result.FileId;
                        new_share.BalanceSheetLink = createdFile.Result.SystemPath;

                    }

                    if (files.IndexOf(file) == 3)
                    {
                        new_share.FinancialResultId = createdFile.Result.FileId;
                        new_share.FinancialResultLink = createdFile.Result.SystemPath;

                    }


                    if (files.IndexOf(file) == 4)
                    {
                        new_share.AuditConclusionId = createdFile.Result.FileId;
                        new_share.AuditConclusionLink = createdFile.Result.SystemPath;

                    }


                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Shares", new { success = true });


            }

            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName");

            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName");
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName");

            return View(share);
        }

        public async Task<FileModel> UploadFile(int id, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/Shares/" + id.ToString());

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

            var systemPath = Path.Combine("/Files/Shares/" + id.ToString() + "/" + temp + extension);

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
                ShareId = id
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
        public async Task<IActionResult> ReplaceFile(int shareId, int fileId, IFormFile file, int finder)
        {   
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.ShareId == shareId).FirstOrDefault();
            var share = _context.Shares.Find(shareId);
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
                    fileModel.SystemPath = Path.Combine("/Files/Shares/" + shareId.ToString() + "/" + temp + extension);
            
                    if (finder == 0)
                    {

                        share.RegCertificateLink = fileModel.SystemPath;

                    }
                    if (finder == 1)
                    {

                        share.OrgCharterLink = fileModel.SystemPath;

                    }


                    await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);

                }

                else if (file != null)
                {
                    var createdFile = UploadFile(shareId, file);

                    if (finder == 0)
                    {
                        share.RegCertificateId = createdFile.Result.FileId;
                        share.RegCertificateLink = createdFile.Result.SystemPath;

                    }
                    if (finder == 1)
                    {
                        share.OrgCharterId = createdFile.Result.FileId;
                        share.OrgCharterLink = createdFile.Result.SystemPath;

                    }
                   
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }



            return RedirectToAction("Edit", new { id = shareId });

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

        // GET: SimpleUser/Shares/Edit/5/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var share = await _context.Shares.FindAsync(id);
            if (share == null)
            {
                return NotFound();
            }

            var shareAndholders = _context.SharesAndHolders.Where(s => s.ShareId == id).ToList();
            List<Shareholder> shareHolders = new();

            foreach (var item in shareAndholders)
            {
                try
                {
                    var shareholder = await _context.Shareholders.FirstAsync(s => s.ShareholderId == item.ShareholderId);
                    shareHolders.Add(shareholder);
                }
                catch(Exception ex)
                {
                    var m = ex.Message;
                }
                
            }

            ViewData["Shareholders"] = shareHolders;
            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName", share.DistrictId);
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName", share.RegionId);
         
            return View(share);
        }

        // POST: SimpleUser/Shares/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShareId,Status,Confirmed,OutOfAccountDate,BusinessEntityName,IdRegNumber,ParentOrganization,Activities,ActivityShare,FoundationYear,StateRegistrationDate,RegionId,DistrictId,Address,AuthorizedCapital,NumberOfShares,ParValueOfShares,AdministrativeStaff,ProductionPersonal,AverageMonthlySalary,MaintanenceCostForYear,ProductionArea,BuildingsArea," +
            "ApplicationUserId,TransferredAssetId,OrgCharterId, OrgCharterLink, RegCertificateId, RegCertificateLink,BalanceSheetId,BalanceSheetLink,FinancialResultId,FinancialResultLink,AuditConclusionId,AuditConclusionLink" +
            "TransferredAssetOn, AssetEvaluationOn, SubmissionOnBiddingOn, ReductionInAssetOn, OneTimePaymentAssetOn, InstallmentAssetOn,AmountPayable,AmountReceivable,ProfitOrLossOfYear1,ProfitOrLossOfYear2,ProfitOrLossOfYear3,Year1, Year2, Year3,Comments")] Share share)
        {
            if (id != share.ShareId)
            {
                return NotFound();
            }

            var _share = await _context.Shares.FindAsync(id);

            if (_share == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                if (_share.BusinessEntityName != share.BusinessEntityName)
                    _share.BusinessEntityName = share.BusinessEntityName;
                if (_share.IdRegNumber != share.IdRegNumber)
                    _share.IdRegNumber = share.IdRegNumber;
                if (_share.ParentOrganization != share.ParentOrganization)
                    _share.ParentOrganization = share.ParentOrganization;
                if (_share.Activities != share.Activities)
                    _share.Activities = share.Activities;
                if (_share.ActivityShare != share.ActivityShare)
                    _share.ActivityShare = share.ActivityShare;
                if (_share.FoundationYear != share.FoundationYear)
                    _share.FoundationYear = share.FoundationYear;
                if (_share.StateRegistrationDate != share.StateRegistrationDate)
                    _share.StateRegistrationDate = share.StateRegistrationDate;
                if (_share.RegionId != share.RegionId)
                    _share.RegionId = share.RegionId;
                if (_share.DistrictId != share.DistrictId)
                    _share.DistrictId = share.DistrictId;
                if (_share.Address != share.Address)
                    _share.Address = share.Address;
                if (_share.AuthorizedCapital != share.AuthorizedCapital)
                    _share.AuthorizedCapital = share.AuthorizedCapital;
                if (_share.NumberOfShares != share.NumberOfShares)
                    _share.NumberOfShares = share.NumberOfShares;
                if (_share.ParValueOfShares != share.ParValueOfShares)
                    _share.ParValueOfShares = share.ParValueOfShares;
                if (_share.AdministrativeStaff != share.AdministrativeStaff)
                    _share.AdministrativeStaff = share.AdministrativeStaff;
                if (_share.ProductionPersonal != share.ProductionPersonal)
                    _share.ProductionPersonal = share.ProductionPersonal;
                if (_share.AverageMonthlySalary != share.AverageMonthlySalary)
                    _share.AverageMonthlySalary = share.AverageMonthlySalary;
                if (_share.MaintanenceCostForYear != share.MaintanenceCostForYear)
                    _share.MaintanenceCostForYear = share.MaintanenceCostForYear;
                if (_share.ProductionArea != share.ProductionArea)
                    _share.ProductionArea = share.ProductionArea;
                if (_share.BuildingsArea != share.BuildingsArea)
                    _share.BuildingsArea = share.BuildingsArea;
                if (_share.AmountPayable != share.AmountPayable)
                    _share.AmountPayable = share.AmountPayable;
                if (_share.AmountReceivable != share.AmountReceivable)
                    _share.AmountReceivable = share.AmountReceivable;
                if (_share.ProfitOrLossOfYear1 != share.ProfitOrLossOfYear1)
                    _share.ProfitOrLossOfYear1 = share.ProfitOrLossOfYear1;
                if (_share.ProfitOrLossOfYear2 != share.ProfitOrLossOfYear2)
                    _share.ProfitOrLossOfYear2 = share.ProfitOrLossOfYear2;
                if (_share.ProfitOrLossOfYear3 != share.ProfitOrLossOfYear3)
                    _share.ProfitOrLossOfYear3 = share.ProfitOrLossOfYear3;
                if (_share.Year1 != share.Year1)
                    _share.Year1 = share.Year1;
                if (_share.Year2 != share.Year2)
                    _share.Year2 = share.Year2;
                if (_share.Year3 != share.Year3)
                    _share.Year3 = share.Year3;
                if (_share.Comments != share.Comments)
                    _share.Comments = share.Comments;

                try
                {
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShareExists(share.ShareId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Shares", new { editSuccess = true });
            }
           
            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName", share.DistrictId);
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName", share.RegionId);
            
            return View(share);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShareholders(int shareId, string[] shareHoldersId, string[] shareHolders, string[] AmountFromAuthCapitalHolder, string[] shareHoldersNew, string[] AmountFromAuthCapitalHolderNew)
        {
            var share = _context.Shares.Find(shareId);

            if (share == null)
            {
                return NotFound();
            }


            for (int i = 0; i < shareHoldersId.Length; i++)
            {
                var shareHolder = _context.Shareholders.Find(int.Parse(shareHoldersId[i]));
                shareHolder.ShareholderName = shareHolders[i];
                shareHolder.AmountFromAuthCapital = AmountFromAuthCapitalHolder[i];

                await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);
            }


            for (int i = 0; i < shareHoldersNew.Length; i++)
            {
                Shareholder shareholder = new()
                {
                    ShareholderName = shareHoldersNew[i],
                    AmountFromAuthCapital = AmountFromAuthCapitalHolderNew[i]
                };

                _context.Add(shareholder);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);

                SharesAndHolders sharesAndHolders = new()
                {
                    ShareholderId = shareholder.ShareholderId,
                    ShareId = share.ShareId
                };

                _context.Add(sharesAndHolders);
                await _context.SaveChangesAsync();

            }


            return RedirectToAction("Edit", "Shares", new { id = share.ShareId });
        }

        [HttpGet]
        public IActionResult GetSent()
        {

            var userId = _userManager.GetUserId(User);

            //var realEstateInfrastructures = _context.RealEstates.Where(r => r.ApplicationUserId == userId);

            var shares = _context.Shares.Include(r => r.ApplicationUser)
                .Include(r => r.AssetEvaluations).Include(r => r.DistrictOfObject)
                .Include(r => r.ReductionInAssets).Include(r => r.RegionOfObject)
                .Include(r => r.SubmissionOnBiddings)
                .Include(r => r.TransferredAsset)
                .Include(r => r.InstallmentAssets)
                .Include(r => r.OneTimePaymentAssets)
                .Where(r => r.ApplicationUserId == userId && r.Confirmed == true).ToList();

            foreach(var item in shares)
            {
                item.FoundationYearStr = item.FoundationYear.ToShortDateString();
                item.StateRegistrationDateStr = item.StateRegistrationDate.ToShortDateString();
            }

            List<SharesAndHoldersViewModel> sharesAndHoldersViewModels = new();
            

            Shareholder shareholder = new();
            List<SharesAndHolders> sharesAndHolders = new();

            foreach (var share in shares)
            {
                string shareholders = "";
                string amount;
                string shareAmount = "";
                string sharepercentage = "";

                share.StateRegistrationDateStr = share.StateRegistrationDate.ToShortDateString();
                share.FoundationYearStr = share.FoundationYear.ToShortDateString();

                var user = _context.ApplicationUsers.Include(a => a.Organization).FirstOrDefault(u => u.Id == share.ApplicationUserId);

                sharesAndHolders = _context.SharesAndHolders.Where(s => s.ShareId == share.ShareId).ToList();

                foreach (var item in sharesAndHolders)
                {
                    shareholders += _context.Shareholders.First(s => s.ShareholderId == item.ShareholderId).ShareholderName + " | ";
                    amount = _context.Shareholders.First(s => s.ShareholderId == item.ShareholderId).AmountFromAuthCapital;
                    shareAmount += amount + " | ";
                    sharepercentage += Math.Round((decimal.Parse(amount, CultureInfo.InvariantCulture.NumberFormat) * 100 / decimal.Parse(share.AuthorizedCapital, CultureInfo.InvariantCulture.NumberFormat)), 2) + " | ";
                }

                SharesAndHoldersViewModel sharesAndHoldersViewModel = new()
                {
                    Shareholders = shareholders,
                    ShareAmount = shareAmount,
                    SharePercentage = sharepercentage,
                    Share = share,
                    ApplicationUser = user
                };

                sharesAndHoldersViewModels.Add(sharesAndHoldersViewModel);
            }

            return Json(new { data = sharesAndHoldersViewModels });
        }

        [HttpGet]
        public IActionResult GetUnSent()
        {

            var userId = _userManager.GetUserId(User);

            //var realEstateInfrastructures = _context.RealEstates.Where(r => r.ApplicationUserId == userId);

            var shares = _context.Shares.Include(r => r.ApplicationUser)
                .Include(r => r.AssetEvaluations).Include(r => r.DistrictOfObject)
                .Include(r => r.ReductionInAssets).Include(r => r.RegionOfObject)
                .Include(r => r.SubmissionOnBiddings)
                .Include(r => r.TransferredAsset)
                .Include(r => r.InstallmentAssets)
                .Include(r => r.OneTimePaymentAssets)
                .Where(r => r.ApplicationUserId == userId && r.Confirmed == false).ToList();

            foreach (var item in shares)
            {
                item.FoundationYearStr = item.FoundationYear.ToShortDateString();
                item.StateRegistrationDateStr = item.StateRegistrationDate.ToShortDateString();
            }

            List<SharesAndHoldersViewModel> sharesAndHoldersViewModels = new();


            Shareholder shareholder = new();
            List<SharesAndHolders> sharesAndHolders = new();

            foreach (var share in shares)
            {
                string shareholders = "";
                string amount;
                string shareAmount = "";
                string sharepercentage = "";

                share.StateRegistrationDateStr = share.StateRegistrationDate.ToShortDateString();
                share.FoundationYearStr = share.FoundationYear.ToShortDateString();

                var user = _context.ApplicationUsers.Include(a => a.Organization).FirstOrDefault(u => u.Id == share.ApplicationUserId);

                sharesAndHolders = _context.SharesAndHolders.Where(s => s.ShareId == share.ShareId).ToList();

                foreach (var item in sharesAndHolders)
                {
                    shareholders += _context.Shareholders.First(s => s.ShareholderId == item.ShareholderId).ShareholderName + " | ";
                    amount = _context.Shareholders.First(s => s.ShareholderId == item.ShareholderId).AmountFromAuthCapital;
                    shareAmount += amount + " | ";
                    sharepercentage += Math.Round((decimal.Parse(amount, CultureInfo.InvariantCulture.NumberFormat) * 100 / decimal.Parse(share.AuthorizedCapital, CultureInfo.InvariantCulture.NumberFormat)), 2) + " | ";
                }

                SharesAndHoldersViewModel sharesAndHoldersViewModel = new()
                {
                    Shareholders = shareholders,
                    ShareAmount = shareAmount,
                    SharePercentage = sharepercentage,
                    Share = share,
                    ApplicationUser = user
                };

                sharesAndHoldersViewModels.Add(sharesAndHoldersViewModel);
            }

            return Json(new { data = sharesAndHoldersViewModels });
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var share = await _context.Shares.FindAsync(id);
            if (share != null)
            {
                share.Confirmed = true;
                share.TransferredAssetOn = true;
                share.AssetEvaluationOn = true;
                share.InstallmentAssetOn = true;
            }
                

           
            try
            {
                await _context.SaveChangesAsync(_userManager.GetUserId(User), share.BusinessEntityName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Json(new { success = true, message = "Маълумотлар тасдиқланди!" });
        }

        
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var share = await _context.Shares.FindAsync(id);

            if (share == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var businessEntityName = share.BusinessEntityName;

            var fileModels = _context.FileModels.Where(f => f.ShareId == id);
            var transferredAsset = _context.TransferredAssets.FirstOrDefault(t => t.AssetId == t.Share.TransferredAssetId);
            var assetEvaluations = _context.AssetEvaluations.Where(a => a.ShareId == id).ToList();
            var submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.ShareId == id).ToList();
            var reductionInAssets = _context.ReductionInAssets.Where(s => s.ShareId == id).ToList();
            var oneTimePaymentAssets = _context.OneTimePaymentAssets.Where(s => s.ShareId == id).ToList();
            var installmentAssets = _context.InstallmentAssets.Where(s => s.ShareId == id).ToList();


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
                await _context.SaveChangesAsync(_userManager.GetUserId(User),businessEntityName);
            }



        

            if (assetEvaluations.Any())
            {
                foreach (var item in assetEvaluations)
                {
                    var evaluationFiles = _context.FileModels.Where(f => f.AssetEvaluationId == item.AssetEvaluationId);

                    if (evaluationFiles.Any())
                    {
                        foreach (var item2 in evaluationFiles)
                        {
                            if (System.IO.File.Exists(item2.FilePath))
                            {
                                System.IO.File.Delete(item2.FilePath);
                            }
                            _context.FileModels.Remove(item2);
                        }

                    }
                    _context.AssetEvaluations.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
                }

            }

            if (submissionOnBiddings.Any())
            {
                foreach (var item in submissionOnBiddings)
                {
                    _context.SubmissionOnBiddings.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
                }

            }



            if (reductionInAssets.Any())
            {
                foreach (var item in reductionInAssets)
                {

                    var reductionFile = _context.FileModels.FirstOrDefault(f => f.ReductionInAssetId == item.ReductionInAssetId);

                    if (reductionFile != null)
                    {
                        if (System.IO.File.Exists(reductionFile.FilePath))
                        {
                            System.IO.File.Delete(reductionFile.FilePath);
                        }
                        _context.FileModels.Remove(reductionFile);

                    }
                    _context.ReductionInAssets.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
                }

            }

            if (oneTimePaymentAssets.Any())
            {
                foreach (var item in oneTimePaymentAssets)
                {

                    var oneTimePaymentFile = _context.FileModels.FirstOrDefault(f => f.OneTimePaymentAssetId == item.OneTimePaymentAssetId);

                    if (oneTimePaymentFile != null)
                    {

                        if (System.IO.File.Exists(oneTimePaymentFile.FilePath))
                        {
                            System.IO.File.Delete(oneTimePaymentFile.FilePath);
                        }
                        _context.FileModels.Remove(oneTimePaymentFile);
                    }

                    _context.OneTimePaymentAssets.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
                }


            }

            if (installmentAssets.Any())
            {
                foreach (var item in installmentAssets)
                {

                    var installmentFile = _context.FileModels.Where(f => f.InstallmentAssetId == item.InstallmentAssetId);

                    if (installmentFile.Any())
                    {
                        foreach (var item2 in fileModels)
                        {
                            if (System.IO.File.Exists(item2.FilePath))
                            {
                                System.IO.File.Delete(item2.FilePath);
                            }
                            _context.FileModels.Remove(item2);
                        }
                    }

                    _context.InstallmentAssets.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
                }

            }
            try { 
            _context.Shares.Remove(share);
            await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
            }
            catch (Exception ex)
               
            {
                var e1x = ex;
            }
            if (transferredAsset != null)
            {
                var transferredAssetFiles = _context.FileModels.Where(f => f.TransferredAssetId == transferredAsset.AssetId);

                if (transferredAssetFiles.Any())
                {
                    foreach (var item2 in transferredAssetFiles)
                    {
                        if (System.IO.File.Exists(item2.FilePath))
                        {
                            System.IO.File.Delete(item2.FilePath);
                        }
                        _context.FileModels.Remove(item2);
                    }


                }
                try
                {
                    _context.TransferredAssets.Remove(transferredAsset);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
                }
                catch (Exception ex)

                {
                    var e1x = ex;
                }
            }
            return Json(new { success = true, message = "Муваффақиятли ўчирилди" });

        }

        [HttpDelete]
        public async Task<IActionResult> RemoveShareholder(int shareHolderId, int shareId)
        {
            var shareholder = await _context.Shareholders.FindAsync(shareHolderId);
            if (shareholder == null)
                return Json(new { success = false, message = "Акционер/участник топилмади" });

            var share = await _context.Shares.FindAsync(shareId);
            var businessEntityName = "";
            if (share != null)
                businessEntityName = share.BusinessEntityName;
            if (shareholder == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            try
            {
                _context.Shareholders.Remove(shareholder);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
            }
            catch (Exception ex)

            {
                var e1x = ex;
            }

            return Json(new { success = true, message = "Муваффақиятли!" });

        }


        private bool ShareExists(int id)
        {
            return _context.Shares.Any(e => e.ShareId == id);
        }
    }
}
