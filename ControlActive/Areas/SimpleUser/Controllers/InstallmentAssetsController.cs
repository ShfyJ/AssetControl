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
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Xml;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
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

        // GET: SimpleUser/InstallmentAssets
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var realEstates = _context.RealEstates.Include(r => r.SubmissionOnBiddings).Where(r => r.InstallmentAssetOn == true && r.Confirmed == true && r.ApplicationUserId == userId).ToList();
            var shares = _context.Shares.Include(s => s.SubmissionOnBiddings).Where(s => s.InstallmentAssetOn == true && s.Confirmed == true && s.ApplicationUserId == userId).ToList();

            List<SelectListItem> rList = new();
            List<SelectListItem> sList = new();
            foreach (var r in realEstates)
            {
                rList.Add(new SelectListItem(r.RealEstateName, r.RealEstateId.ToString()));
            }

            foreach (var s in shares)
            {
                sList.Add(new SelectListItem(s.BusinessEntityName, s.ShareId.ToString()));
            }

            ViewBag.rAssets = JsonSerializer.Serialize(rList);
            ViewBag.sAssets = JsonSerializer.Serialize(sList);

            return View(await _context.InstallmentAssets
                .Include(i => i.Share)
                .Include(i => i.RealEstate)
                .Where(s => s.RealEstate.ApplicationUserId == userId || s.Share.ApplicationUserId == userId)
                .ToListAsync());
        }


       [HttpPost]
        public async Task<IActionResult> Create([FromForm]string target, [FromForm]string assetId, [FromForm]string governingBodyName, IFormFile solutionFile, [FromForm]string solutionDate, [FromForm]string solutionNumber,[FromForm]string biddingDate,[FromForm]string assetBuyerName,[FromForm]string amountOfAssetSold,[FromForm]string aggreementDate,[FromForm]string aggreementNumber, IFormFile agreementFile, [FromForm]string installmentTime,[FromForm] string actualInitPayment, [FromForm] string paymentPeriodType, [FromForm] string actualPayment)
        {
            if(assetId == null)
            {
                return Json(new { success = false, message = "Хатолик - Қайтадан уриниб кўринг!" });
            }

            InstallmentAsset installmentAsset = new()
            {
                
                GoverningBodyName = governingBodyName,
                SolutionDate = DateTime.Parse(solutionDate),
                SolutionNumber = solutionNumber,
                BiddingDate = DateTime.Parse(biddingDate),
                AssetBuyerName = assetBuyerName,
                AmountOfAssetSold = amountOfAssetSold,
                AggreementDate = DateTime.Parse(aggreementDate),
                AggreementNumber = aggreementNumber,
                InstallmentTime = int.Parse(installmentTime),
                PaymentPeriodType = int.Parse(paymentPeriodType),
                ActualInitPayment = actualInitPayment,
                ActualPayment = actualPayment,
                Status = 1
            };

            string assetName = "";

            int id = int.Parse(assetId);

            RealEstate realEstate;
            Share share;

            if (target == "1")
            {
                installmentAsset.RealEstateId = id;
                realEstate = await _context.RealEstates.FindAsync(id);
                if(realEstate == null)
                {
                    return Json(new { success = false, message = "Объект топилмади!" });
                }
                realEstate.OneTimePaymentAssetOn = false;
                realEstate.SubmissionOnBiddingOn = false;
                realEstate.TransferredAssetOn = false;
                realEstate.ReductionInAssetOn = false;
                assetName = realEstate.RealEstateName;
                
            }

            if (target == "2")
            {
                installmentAsset.ShareId = id;
                share = await _context.Shares.FindAsync(id);
                if(share == null)
                    return Json(new { success = false, message = "Актив топилмади!" });

                share.OneTimePaymentAssetOn = false;
                share.SubmissionOnBiddingOn = false;
                share.TransferredAssetOn = false;
                share.ReductionInAssetOn = false;
                assetName = share.BusinessEntityName;
            }

            var context = new ValidationContext(installmentAsset, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(installmentAsset, context, validationResults, true);
            

            if (isValid)
            {
                installmentAsset.ContractCancelledDate = DateTime.Now.AddYears(1000);
                _context.Add(installmentAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

                List<IFormFile> files = new ();

                if (solutionFile != null)
                { files.Add(solutionFile); }             

                if (agreementFile != null)
                    files.Add(agreementFile);

                foreach (var file in files)
                {
                    var createdFile = UploadFile(installmentAsset.InstallmentAssetId, file, 1);

                    if (files.IndexOf(file) == 0)
                    {
                        installmentAsset.SolutionFileId = createdFile.Result.FileId;
                        installmentAsset.SolutionFileLink = createdFile.Result.SystemPath;

                    }
                   
                    if (files.IndexOf(file) == 1)
                    {
                        installmentAsset.AggreementFileId = createdFile.Result.FileId;
                        installmentAsset.AggreementFileLink = createdFile.Result.SystemPath;

                    }

                }
                await _context.SaveChangesAsync();

                TempData["Message"] = "File successfully uploaded to File System.";

                return Json(new { success = true, message = "Амалиёт муваффақиятли бажарилди!" });
            }

            List<string> errorMessages = new();

            foreach(var item in validationResults)
            {
                foreach (string name in item.MemberNames)
                {
                    string msg = name + ":" + item.ErrorMessage;
                    errorMessages.Add(msg);
                }
   
            }
            string json = JsonSerializer.Serialize(errorMessages);
            return Json(new { success = false, message = json });
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateAct([FromForm]string installmentId, IFormFile actFile, [FromForm]string actAndAssetDate, [FromForm]string actAndAssetNumber)
        {
            if(installmentId == null)
            {
                return Json(new { success = false, message = "Сотув ID юборилмади - бироздан сўнг уриниб кўринг!" });
            }
            var id = Int32.Parse(installmentId);
            var installment =await _context.InstallmentAssets.Include(i => i.RealEstate).Include(i => i.Share).FirstOrDefaultAsync(i => i.InstallmentAssetId == id);
            if(installment == null)
            {
                return Json(new { success = false, message = "Хатолик - маълумот топилмади!"});
            }
            string assetName = "";

            if (installment.RealEstate != null)
                assetName = installment.RealEstate.RealEstateName;
            if (installment.Share != null)
                assetName = installment.Share.BusinessEntityName;

            InstallmentStep2 installmentStep2 = new()
            {

                ActAndAssetDate = DateTime.Parse(actAndAssetDate),
                ActAndAssetNumber = actAndAssetNumber,
                InstallmentAssetId = id
            };

            var context = new ValidationContext(installmentStep2, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(installmentStep2, context, validationResults, true);

            if (isValid)
            {
                _context.Add(installmentStep2);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

                installment.InstallmentStep2Id = installmentStep2.InstallmentStep2Id;
                installment.Status = 2;

                var createdFile = UploadFile(installmentStep2.InstallmentStep2Id, actFile, 2);
                if(createdFile != null)
                {
                    installmentStep2.ActAndAssetFileId = createdFile.Result.FileId;
                    installmentStep2.ActAndAssetFileLink = createdFile.Result.SystemPath;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Амалиёт муваффақиятли бажарилди!" });

            }

            List<string> errorMessages = new();

            foreach (var item in validationResults)
            {
                foreach (string name in item.MemberNames)
                {
                    string msg = name + ":" + item.ErrorMessage;
                    errorMessages.Add(msg);
                }

            }
            string json = JsonSerializer.Serialize(errorMessages);
            return Json(new { success = false, message = json });
        }

        [HttpPost]
        public async Task<IActionResult> GetAssets([FromBody]string data)
        {
            if (data == null)
            {
                return Json(new { success = false, message = "Хатолик - Қайтадан уриниб кўринг!" });
            }

            var userId = _userManager.GetUserId(User);

            int target = int.Parse(data);
            List<InstallmentAsset> installmentAssets = new();
            if(target == 1)
            {
                installmentAssets = await _context.InstallmentAssets.Include(i => i.RealEstate).Where(i => i.RealEstate != null && i.RealEstate.ApplicationUserId == userId).ToListAsync();
            }

            if (target == 2)
            {
                installmentAssets = await _context.InstallmentAssets.Include(i => i.Share).Where(i => i.Share != null && i.Share.ApplicationUserId == userId).ToListAsync();
            }

            string name = "";

            List<GeneralViewModel> viewModels = new();
            
            try
            {
                foreach (var item in installmentAssets)
                {
                    item.SolutionDateStr = item.SolutionDate.ToShortDateString();
                    item.BiddingDateStr = item.BiddingDate.ToShortDateString();
                    item.AggreementDateStr = item.AggreementDate.ToShortDateString();

                    var installmentStep2 = await _context.InstallmentStep2.FirstOrDefaultAsync(i => i.InstallmentStep2Id == item.InstallmentStep2Id);
                    if (installmentStep2 != null)
                        installmentStep2.ActAndAssetDateStr = installmentStep2.ActAndAssetDate.ToShortDateString();

                    if (target == 1)
                    {
                        name = _context.RealEstates.FirstOrDefaultAsync(r => r.RealEstateId == item.RealEstateId).Result.RealEstateName;
                    }

                    if (target == 2)
                    {
                        name = _context.Shares.FirstOrDefaultAsync(r => r.ShareId == item.ShareId).Result.BusinessEntityName;
                    }

                    GeneralViewModel viewModel = new()
                    {
                        Name = name,
                        InstallmentAsset = item,
                        InstallmentStep2 = installmentStep2
                    };

                    viewModels.Add(viewModel);

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
           

            return Json(new { data = viewModels });

        }

        public async Task<FileModel> UploadFile(int? id, IFormFile file, int? target)
        {
            if (file == null || id == null || target == null)
            {
                return null;
            }

            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/InstallmentAssets/" + id.ToString());
            
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

            var systemPath = Path.Combine("/Files/InstallmentAssets/" + id.ToString() + "/" + temp + extension);

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
                BasePath = basePath
                
            };
            if (target == 1)
            {
                fileModel.InstallmentAssetId = id;
            }
            if (target == 2)
            {
                fileModel.InstallmentStep2Id = id;
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
        public async Task<bool> ReplaceFile(int? id, int? fileId, IFormFile file, int? target, int finder=-1)
        {

            if (fileId == null || id == null || target == null || file == null)
            {
                return false;
            }

            FileModel fileModel = new();
            InstallmentAsset step1 = new();
            InstallmentStep2 step2 = new();
            string assetName = "";
           
            if (target == 1)
            {
                step1 = await _context.InstallmentAssets.Include(i => i.RealEstateId).Include(i => i.Share).FirstOrDefaultAsync(i => i.InstallmentAssetId == id);
                
                fileModel = await _context.FileModels.Where(f => f.FileId == fileId && f.InstallmentAssetId == id).FirstOrDefaultAsync();

                if(fileModel == null || step1 == null)
                {
                    return false;
                }

                if (step1.RealEstate != null)
                    assetName = step1.RealEstate.RealEstateName;
                if (step1.Share != null)
                    assetName = step1.Share.BusinessEntityName;
            }
            if (target == 2)
            {
                step2 = await _context.InstallmentStep2.Include(i => i.InstallmentAsset.RealEstate).Include(i => i.InstallmentAsset.Share).FirstOrDefaultAsync(i => i.InstallmentStep2Id == id);
                fileModel = await _context.FileModels.Where(f => f.FileId == fileId && f.InstallmentStep2Id == id).FirstOrDefaultAsync();

                if (fileModel == null || step2 == null)
                {
                    return false;
                }

                if(step2.InstallmentAsset.RealEstate!= null)
                    assetName = step2.InstallmentAsset.RealEstate.RealEstateName;
                if (step2.InstallmentAsset.Share != null)
                    assetName = step2.InstallmentAsset.Share.BusinessEntityName;
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
                    fileModel.SystemPath = Path.Combine("/Files/InstallmentAssets/" + id.ToString() + "/" + temp + extension);
                    if (target == 1)
                    { 
                        if(finder == 0)
                        {
                            step1.SolutionFileLink = fileModel.SystemPath;
                        }

                        else if (finder == 1)
                        {
                            step1.AggreementFileLink = fileModel.SystemPath;
                        }

                    }
                    
                    if (target == 2)
                    {

                        step2.ActAndAssetFileLink = fileModel.SystemPath;

                    }                    

                    await _context.SaveChangesAsync(_userManager.GetUserId(User),assetName);
                }

                else if (fileModel == null && file != null)
                {
                    var createdFile = UploadFile(id, file, target);
                    if (target == 1)
                    {
                        if (finder == 1)
                        {
                            step1.SolutionFileId = createdFile.Result.FileId;
                            step1.SolutionFileLink = createdFile.Result.SystemPath;

                        }

                        if (finder == 2)
                        {
                            step1.AggreementFileId = createdFile.Result.FileId;
                            step1.AggreementFileLink = createdFile.Result.SystemPath;

                        }
                    }
                    if (target == 2)
                    {
                        step2.ActAndAssetFileId = createdFile.Result.FileId;
                        step2.ActAndAssetFileLink = createdFile.Result.SystemPath;
                    }

                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);
                }
            }
            catch (NotSupportedException ex)
            {
                return false;
            }

            return true;

            //return RedirectToAction("Edit", new { id = id});

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


        // GET: SimpleUser/InstallmentAssets/Edit/5
        public async Task<IActionResult> Edit(int? id, int? targetId, int? target)
        {
            var share = _context.Shares.Find(targetId);
            var realEstate = _context.RealEstates.Find(targetId);

            ViewBag.Name = "";
            if(target == 1 && realEstate!=null)
            {
                ViewBag.Name = realEstate.RealEstateName;
            }

            if (target == 2 && share != null)
            {
                ViewBag.Name = share.BusinessEntityName;
            }

            ViewBag.Target = target;

            if (id == null)
            {
                return NotFound();
            }
            List<SelectListItem> periods = new();

            SelectListItem selectListItem1 = new()
            {
                Text = "год",
                Value = "12",

            };
            SelectListItem selectListItem2 = new()
            {
                Text = "квартал",
                Value = "4",

            };
            SelectListItem selectListItem3 = new()
            {
                Text = "месяц",
                Value = "1",

            };


            periods.Add(selectListItem1);
            periods.Add(selectListItem2);
            periods.Add(selectListItem3);

            ViewBag.Period = periods;

            var installmentAsset = await _context.InstallmentAssets.FindAsync(id);
            if (installmentAsset == null)
            {
                return NotFound();
            }
            return View(installmentAsset);
        }

        public async Task<IActionResult> EditAct(int? id, int? targetId, int? target)
        {
            var share = _context.Shares.Find(targetId);
            var realEstate = _context.RealEstates.Find(targetId);

            ViewBag.Name = "";
            if (target == 1 && realEstate != null)
            {
                ViewBag.Name = realEstate.RealEstateName;
            }

            if (target == 2 && share != null)
            {
                ViewBag.Name = share.BusinessEntityName;
            }

            ViewBag.Target = target;

            if (id == null)
            {
                return NotFound();
            }
          
            var installmentAsset2 = await _context.InstallmentStep2.FindAsync(id);
            if (installmentAsset2 == null)
            {
                return NotFound();
            }
            return View(installmentAsset2);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditAct([FromForm] string installmentStep2Id, IFormFile actFile, [FromForm] string actAndAssetDate, [FromForm] string actAndAssetNumber)
        {
            if (installmentStep2Id == null)
            {
                return Json(new { success = false, message = "Сотув ID юборилмади - бироздан сўнг уриниб кўринг!" });
            }
            int step2Id = int.Parse(installmentStep2Id);

            var step2 = await _context.InstallmentStep2.Include(i => i.InstallmentAsset).Include(i => i.InstallmentAsset.RealEstate).Include(i => i.InstallmentAsset.Share).FirstOrDefaultAsync(i => i.InstallmentStep2Id == step2Id);

            if (step2 == null)
            {
                return Json(new { success = false, message = "Ушбу сотув ID бўйича маълумот топилмади!" });
            }
            string assetName = "";

            if (step2.InstallmentAsset.RealEstate != null)
                assetName = step2.InstallmentAsset.RealEstate.RealEstateName;
            if (step2.InstallmentAsset.Share != null)
                assetName = step2.InstallmentAsset.Share.BusinessEntityName;

            if (step2.ActAndAssetNumber != actAndAssetNumber)
                step2.ActAndAssetNumber = actAndAssetNumber;
            if (step2.ActAndAssetDate != DateTime.Parse(actAndAssetDate))
                step2.ActAndAssetDate = DateTime.Parse(actAndAssetDate);
            
            var context = new ValidationContext(step2, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(step2, context, validationResults, true);

            if (isValid)
            {
                try
                {
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                Task<bool> resultOne = null;

                if (actFile != null)
                {
                    resultOne = ReplaceFile(step2Id, step2.ActAndAssetFileId, actFile, 2);
                    await Task.Delay(100);

                    if (resultOne != null)
                    {
                        if (!resultOne.Result)
                            return Json(new { success = false, message = "Муваффақиятли таҳрирланди! - аммо акт файлини ўзгартиришда хатолик!" });
                    }

                    if (resultOne == null)
                    {
                        return Json(new { success = false, message = "Муваффақиятли таҳрирланди! - аммо акт файлини ўзгартиришда хатолик!" });
                    }
                }


                //await Task.WhenAll(resultOne, resultTwo);

                return Json(new { success = true, message = "Муваффақиятли таҳрирланди!" });

            }

            List<string> errorMessages = new();

            foreach (var item in validationResults)
            {
                foreach (string name in item.MemberNames)
                {
                    string msg = name + ":" + item.ErrorMessage;
                    errorMessages.Add(msg);
                }

            }
            string json = JsonSerializer.Serialize(errorMessages);
            return Json(new { success = false, message = json });
        }
            // POST: SimpleUser/InstallmentAssets/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm]string id, [FromForm] string governingBodyName, IFormFile solutionFile, [FromForm] string solutionDate, [FromForm] string solutionNumber, [FromForm] string biddingDate, [FromForm] string assetBuyerName, [FromForm] string amountOfAssetSold, [FromForm] string aggreementDate, [FromForm] string aggreementNumber, IFormFile agreementFile, [FromForm] string installmentTime, [FromForm] string actualInitPayment, [FromForm] string paymentPeriodType, [FromForm] string actualPayment)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Хатолик - Қайтадан уриниб кўринг!" });
            }
            int installmentAssetId = int.Parse(id);

            var installmentAsset = await _context.InstallmentAssets.Include(i => i.RealEstate).Include(i => i.Share).FirstOrDefaultAsync(i => i.InstallmentAssetId == installmentAssetId);

            if(installmentAsset == null)
            {
                return NotFound();
            }
            string assetName = "";

            if (installmentAsset.RealEstate != null)
                assetName = installmentAsset.RealEstate.RealEstateName;
            if (installmentAsset.Share != null)
                assetName = installmentAsset.Share.BusinessEntityName;

            if(installmentAsset.GoverningBodyName!= governingBodyName)
                installmentAsset.GoverningBodyName = governingBodyName;
            if(installmentAsset.SolutionDate!= DateTime.Parse(solutionDate))
                installmentAsset.SolutionDate = DateTime.Parse(solutionDate);
            if(installmentAsset.SolutionNumber!=solutionNumber)
                installmentAsset.SolutionNumber = solutionNumber;
            if(installmentAsset.BiddingDate!= DateTime.Parse(biddingDate))
                installmentAsset.BiddingDate = DateTime.Parse(biddingDate);
            if(installmentAsset.AssetBuyerName!= assetBuyerName)
                installmentAsset.AssetBuyerName = assetBuyerName;
            if(installmentAsset.AmountOfAssetSold!=amountOfAssetSold)
                installmentAsset.AmountOfAssetSold = amountOfAssetSold;
            if(installmentAsset.AggreementDate!= DateTime.Parse(aggreementDate))
                 installmentAsset.AggreementDate = DateTime.Parse(aggreementDate);
            if(installmentAsset.AggreementNumber!=aggreementNumber)
                installmentAsset.AggreementNumber = aggreementNumber;
            if(installmentAsset.InstallmentTime!=int.Parse(installmentTime))
                installmentAsset.InstallmentTime = int.Parse(installmentTime);
            if(installmentAsset.PaymentPeriodType!=int.Parse(paymentPeriodType))
                installmentAsset.PaymentPeriodType = int.Parse(paymentPeriodType);
            if(installmentAsset.ActualInitPayment!=actualInitPayment)
                 installmentAsset.ActualInitPayment = actualInitPayment;
            if(installmentAsset.ActualPayment!=actualPayment)
                installmentAsset.ActualPayment = actualPayment;

            var context = new ValidationContext(installmentAsset, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(installmentAsset, context, validationResults, true);

            if (isValid)
            {
                try
                {                  
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InstallmentAssetExists(installmentAsset.InstallmentAssetId))
                    {
                        return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = ex.Message });
                    }
                }

                Task<bool> resultOne = null;
                Task<bool> resultTwo = null;

                if (solutionFile != null)
                {
                    resultOne = ReplaceFile(installmentAsset.InstallmentAssetId, installmentAsset.SolutionFileId, solutionFile, 1, 0);
                    await Task.Delay(100);
                }

                if (agreementFile != null)
                {
                    resultTwo = ReplaceFile(installmentAsset.InstallmentAssetId, installmentAsset.AggreementFileId, agreementFile, 1, 1);
                }

                //await Task.WhenAll(resultOne, resultTwo);

                if(resultOne != null && resultTwo != null)
                {
                    if (!resultOne.Result && !resultTwo.Result)
                        return Json(new { success = true, message = "Муваффақиятли таҳрирланди! - аммо файллар ўзгартиришда хатолик!" });

                    if (resultOne.Result && !resultTwo.Result)
                        return Json(new { success = true, message = "Муваффақиятли таҳрирланди! - аммо шартномани ўзгартиришда хатолик!" });

                    if (!resultOne.Result && resultTwo.Result)
                        return Json(new { success = true, message = "Муваффақиятли таҳрирланди! - аммо қарор файлини ўзгартиришда хатолик!" });

                }

                if(resultOne == null && resultTwo != null)
                {
                    if (!resultTwo.Result)
                        return Json(new { success = true, message = "Муваффақиятли таҳрирланди! - аммо шартномани ўзгартиришда хатолик!" });
                }

                if (resultOne != null && resultTwo == null)
                {
                    if (!resultOne.Result)
                        return Json(new { success = true, message = "Муваффақиятли таҳрирланди! - аммо қарор файлини ўзгартиришда хатолик!" });
                }

                return Json(new { success = true, message = "Муваффақиятли таҳрирланди!" });

            }

            List<string> errorMessages = new();

            foreach (var item in validationResults)
            {
                foreach (string name in item.MemberNames)
                {
                    string msg = name + ":" + item.ErrorMessage;
                    errorMessages.Add(msg);
                }

            }
            string json = JsonSerializer.Serialize(errorMessages);
            return Json(new { success = false, message = json });
        }


        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody]JObject data)
        {
            if (data["id"] == null || data["target"] == null)
                return Json(new { success = false, message = "Хатолик юз берди!" });
            int id = (int)data["id"];
            int target = (int)data["target"];

            var assetName = "";

            if(target == 1)
            {
                var installmentAsset = await _context.InstallmentAssets.Include(r => r.Share).Include(r => r.RealEstate).FirstOrDefaultAsync(r => r.InstallmentAssetId == id);
                if (installmentAsset == null)
                    return Json(new { success = false, message ="Маълумот топилмади!" });
                if (installmentAsset.RealEstate != null)
                    assetName = installmentAsset.RealEstate.RealEstateName;
                if (installmentAsset.Share != null)
                    assetName = installmentAsset.Share.BusinessEntityName;
                installmentAsset.Confirmed = true;

            }

            if(target == 2)
            {
                var step2 = await _context.InstallmentStep2.Include(i => i.InstallmentAsset.RealEstate)
                                                           .Include(i => i.InstallmentAsset.Share).FirstOrDefaultAsync(i => i.InstallmentStep2Id == id);
                if (step2 == null)
                    return Json(new { success = false, message = "Маълумот топилмади!" });
                
                if (step2.InstallmentAsset.RealEstate != null)
                {
                    var realEstate = step2.InstallmentAsset.RealEstate;
                    assetName = realEstate.RealEstateName;
                    realEstate.Status = false;
                    realEstate.OutOfAccountDate = step2.ActAndAssetDate;
                }
                    
                if (step2.InstallmentAsset.Share != null)
                {
                    var share = step2.InstallmentAsset.Share;
                    assetName = share.BusinessEntityName;
                    share.Status = false;
                    share.OutOfAccountDate = step2.ActAndAssetDate;
                }
                    
                step2.Confirmed = true;


            }

            await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            return Json(new { success = true, message = "Муваффақиятли тасдиқланди!" });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSale([FromBody]int id)
        {
            if(id == 0)
            {
                return Json(new { success = false, message = "Сотув ID рақами юборилмади!" });
            }
            var installmentAsset = await _context.InstallmentAssets.Include(i => i.RealEstate).Include(i => i.Share).FirstOrDefaultAsync(i => i.InstallmentAssetId == id);
            
            if(installmentAsset == null)
            {
                return Json(new { success = false, message = "Юборилган сотув ID бўйича маълумот топилмади!" });
            }

            string assetName = "";

            if(installmentAsset.RealEstate != null)
            {
                var realEstate = installmentAsset.RealEstate;
                assetName = realEstate.RealEstateName;
                
                realEstate.SubmissionOnBiddingOn = true;
                realEstate.TransferredAssetOn = true;

            }

            if (installmentAsset.Share != null)
            {
                var share = installmentAsset.Share;
                assetName = share.BusinessEntityName;

                share.SubmissionOnBiddingOn = true;
                share.TransferredAssetOn = true;

            }

            installmentAsset.Status = 0; //сотилмади

            await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            return Json(new { success = true, message = "Сотув бекор қилинди!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]JObject data)
        {
            if (data["id"] == null || data["target"] == null || data["step"] == null)
            {
                return Json(new { success = false, message = "Хатолик - Қайтадан уриниб кўринг!" });
            }

            int id = (int)data["id"];
            int target = (int)data["target"];
            int step = (int)data["step"];

            InstallmentAsset installmentAsset = new();
            InstallmentStep2 step2 = new();
            List<FileModel> fileModels = new();

            string assetName = "";

            if (step == 1)
            {
                installmentAsset = await _context.InstallmentAssets
                .FirstOrDefaultAsync(m => m.InstallmentAssetId == id);

                if (installmentAsset == null)
                {
                    return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
                }



                if (target == 1)
                {
                    var realEstate = await _context.RealEstates.Include(r => r.InstallmentAssets).FirstOrDefaultAsync(r => r.InstallmentAssets.Any(i => i.InstallmentAssetId == id));

                    if (realEstate != null)
                    {
                        realEstate.TransferredAssetOn = true;
                        realEstate.SubmissionOnBiddingOn = true;
                        realEstate.ReductionInAssetOn = true;
                        assetName = realEstate.RealEstateName;
                    }
                    
                }

                if (target == 2)
                {
                    var share = await _context.Shares.Include(s => s.InstallmentAssets).FirstOrDefaultAsync(i => i.InstallmentAssets.Any(i => i.InstallmentAssetId == id));

                    if (share != null)
                    {
                        share.TransferredAssetOn = true;
                        share.SubmissionOnBiddingOn = true;
                        share.ReductionInAssetOn = true;
                        assetName = share.BusinessEntityName;
                    }
                    
                }

                fileModels = await _context.FileModels.Where(i => i.InstallmentAssetId == id).ToListAsync();
            }


            if (step == 2)
            {
                step2 = await _context.InstallmentStep2.Include(i => i.InstallmentAsset).Include(i => i.InstallmentAsset.RealEstate).Include(i => i.InstallmentAsset.Share)
                .FirstOrDefaultAsync(m => m.InstallmentStep2Id == id);

                if (step2 == null)
                {
                    return Json(new { success = false, message = "Хатолик - Маълумот топилмади!" });
                }

                var step1 = step2.InstallmentAsset;
                step1.Status = 1;

                if (step2.InstallmentAsset.RealEstate != null)
                    assetName = step2.InstallmentAsset.RealEstate.RealEstateName;
                if (step2.InstallmentAsset.Share != null)
                    assetName = step2.InstallmentAsset.Share.BusinessEntityName;

                fileModels = await _context.FileModels.Where(i => i.InstallmentStep2Id == id).ToListAsync();
            }

            
            
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

                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            try
            {
                if(step == 1)
                    _context.InstallmentAssets.Remove(installmentAsset);
                if(step == 2)
                    _context.InstallmentStep2.Remove(step2);

                await _context.SaveChangesAsync(_userManager.GetUserId(User),assetName);  
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Муваффақиятли ўчирилди!" });
        }


        private bool InstallmentAssetExists(int id)
        {
            return _context.InstallmentAssets.Any(e => e.InstallmentAssetId == id);
        }
    }
}
