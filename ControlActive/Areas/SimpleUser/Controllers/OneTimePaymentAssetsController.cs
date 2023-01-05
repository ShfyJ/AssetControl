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
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
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

        // GET: SimpleUser/OneTimePaymentAssets
        public async Task<IActionResult> Index(bool success = false)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Success = success;

            return View(await _context.OneTimePaymentAssets
                .Include(s => s.Share)
                .ThenInclude(h => h.ApplicationUser)
                .Include(s => s.RealEstate)
                .ThenInclude(h => h.ApplicationUser)
                .Include(s => s.Step2)
                .Include(s => s.Step3)
                .Where(s => s.RealEstate.ApplicationUserId == userId || s.Share.ApplicationUserId == userId).ToListAsync());
        }


        // GET: SimpleUser/OneTimePaymentAssets/Create
        public IActionResult Create(int? id, int? target)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Target = target;
            ViewData["Id"] = id;

            if (target == 1)
            {
                ViewData["Name"] = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == id).RealEstateName;
                var bidding = _context.SubmissionOnBiddings.FirstOrDefault(s => s.RealEstateId == id && (s.Status == "Сотувда" || s.Status == "Сотилди"));
                ViewData["BiddingDate"] = bidding.BiddingHoldDate;
            }

            if (target == 2)
            {
                ViewData["Name"] = _context.Shares.FirstOrDefault(r => r.ShareId == id).BusinessEntityName;
                var bidding = _context.SubmissionOnBiddings.FirstOrDefault(s => s.ShareId == id && (s.Status == "Сотувда" || s.Status == "Сотилди"));
                ViewData["BiddingDate"] = bidding.BiddingHoldDate;
            }

            return View();
        }

        // POST: SimpleUser/OneTimePaymentAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int target, string name, IFormFile solutionFile, [Bind("OneTimePaymentAssetId,GoverningBodyName,SolutionNumber,SolutionDate,SolutionFileLink,BiddingDate")] OneTimePaymentAsset oneTimePaymentAsset)
        {
            RealEstate realEstate = new();
            Share share = new();
            if (target == 1)
            {
                oneTimePaymentAsset.RealEstateId = id;
                realEstate = _context.RealEstates.Find(id);
            }

            if (target == 2)
            {
                oneTimePaymentAsset.ShareId = id;
                share = _context.Shares.Find(id);
            }

            if (ModelState.IsValid)
            {
                oneTimePaymentAsset.BiddingCancelledDate = DateTime.Now.AddYears(1000);
                oneTimePaymentAsset.Status = "сотувда";
                _context.Add(oneTimePaymentAsset);
                await _context.SaveChangesAsync();

                if(target == 1)
                {
                    realEstate.InstallmentAssetOn = false;
                    realEstate.SubmissionOnBiddingOn = false;
                    realEstate.OneTimePaymentAssetOn = false;
                }

                if (target == 2)
                {
                    share.InstallmentAssetOn = false;
                    share.SubmissionOnBiddingOn = false;
                    share.OneTimePaymentAssetOn = false;
                }

                await _context.SaveChangesAsync();

                var oneTimePayment_asset = _context.OneTimePaymentAssets.Find(oneTimePaymentAsset.OneTimePaymentAssetId);

                if (solutionFile != null)
                {
                    var createdFile = UploadFile(oneTimePayment_asset.OneTimePaymentAssetId, solutionFile, 1);

                    oneTimePayment_asset.SolutionFileId = createdFile.Result.FileId;
                    oneTimePayment_asset.SolutionFileLink = createdFile.Result.SystemPath;
                }

                await _context.SaveChangesAsync();

                TempData["Message"] = "File successfully uploaded to File System.";

                return RedirectToAction("Index", "OneTimePaymentAssets", new { success = true });
            }

            ViewData["Id"] = id;

            ViewBag.Target = target;
            ViewData["Name"] = name;

            return View(oneTimePaymentAsset);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] string saleId,[FromForm] string orgName,
                                                        [FromForm] string solutionNumber, [FromForm] string solutionDate)

        {
            if (saleId == null)
            {
                return Json(new { success = false, message = "Хатолик!" });
            }
            int id = int.Parse(saleId);
            DateTime dateTime = DateTime.Parse(solutionDate);

            var oneTimePayment = await _context.OneTimePaymentAssets.FindAsync(id);
            if (oneTimePayment == null)
            {
                return Json(new { success = false, message = "Хатолик - Акт топилмади!" });
            }

            oneTimePayment.GoverningBodyName = orgName;
            oneTimePayment.SolutionDate = dateTime;
            oneTimePayment.SolutionNumber = solutionNumber;

            try
            {
                _context.Update(oneTimePayment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Муваффақиятли таҳрирланди!" });


        }

        [HttpPost]
        public async Task<IActionResult> EditContract([FromForm] string assetBuyerName, 
                                                        [FromForm] string amountOfAssetSold, [FromForm] string agreementDate, 
                                                          [FromForm] string agreementNumber, [FromForm] string amountPayed,[FromForm] string Percentage, [FromForm] string contractId)
        {
            int id = int.Parse(contractId);
            DateTime dateTime = DateTime.Parse(agreementDate);

            var oneTimePaymentStep2 = await _context.OneTimePaymentStep2.FindAsync(id);
            if (oneTimePaymentStep2 == null)
            {
                return Json(new { success = false, message = "Хатолик - Шартномани топилмади!" });
            }

            oneTimePaymentStep2.AssetBuyerName = assetBuyerName;
            oneTimePaymentStep2.AmountOfAssetSold = amountOfAssetSold;
            oneTimePaymentStep2.AmountPayed = amountPayed;
            oneTimePaymentStep2.AggreementNumber = agreementNumber;
            oneTimePaymentStep2.AggreementDate = dateTime;

            try
            {
                _context.Update(oneTimePaymentStep2);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Шартнома мувффақиятли таҳрирланди!" });


        }


        [HttpPost]
        public async Task<IActionResult> CreateContract([FromForm]string assetBuyerName, [FromForm] string amountOfAssetSold, 
                                                            [FromForm] string agreementDate, [FromForm]string agreementNumber,
                                                                [FromForm] IFormFile agreementFile, [FromForm] string amountPayed, 
                                                                    [FromForm] string paymentId)
        {
        
            var id = int.Parse(paymentId);
            var oneTimePaymentAsset = _context.OneTimePaymentAssets.Find(id);
            OneTimePaymentStep2 oneTimePaymentStep2 = new()
            {
                AssetBuyerName = assetBuyerName,
                AmountOfAssetSold = amountOfAssetSold,  
                AggreementDate = DateTime.Parse(agreementDate), 
                AggreementNumber = agreementNumber,
                AmountPayed = amountPayed,
                OneTimePaymentAssetId = id
                
            };

            var context = new ValidationContext(oneTimePaymentStep2, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(oneTimePaymentStep2, context, validationResults, true);

            if (isValid)
            {
                oneTimePaymentStep2.ContractCancelledDate = DateTime.Now.AddYears(1000);
                _context.Add(oneTimePaymentStep2);

                await _context.SaveChangesAsync();


                oneTimePaymentAsset.OneTimePaymentStep2Id = oneTimePaymentStep2.OneTimePaymentStep2Id;
                oneTimePaymentAsset.Status = "шартнома";

                var oneTimePayment_step2 = _context.OneTimePaymentStep2.Find(oneTimePaymentStep2.OneTimePaymentStep2Id);


                if (agreementFile != null)
                {
                    var createdFile = UploadFile(oneTimePayment_step2.OneTimePaymentStep2Id, agreementFile, 2);
                    oneTimePayment_step2.AggreementFileId = createdFile.Result.FileId;
                    oneTimePayment_step2.AggreementFileLink = createdFile.Result.SystemPath;
                }

                await _context.SaveChangesAsync();

                TempData["Message"] = "File successfully uploaded to File System.";

                return Json(new { success = true, message = "Шартнома муваффақиятли яратилди!" });
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
        public async Task<IActionResult> CreateAct([FromForm]string actDate, [FromForm] string actNumber, [FromForm] IFormFile actFile, [FromForm] IFormFile invoiceFile, [FromForm] string paymentId)
        {
            var id = int.Parse(paymentId);
            var date = DateTime.Parse(actDate); 
            var oneTimePaymentAsset = _context.OneTimePaymentAssets.Find(id);
            OneTimePaymentStep3 oneTimePaymentStep3 = new()
            {
                ActAndAssetDate = date,
                ActAndAssetNumber = actNumber,               
                OneTimePaymentAssetId = id
            };

            var context = new ValidationContext(oneTimePaymentStep3, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(oneTimePaymentStep3, context, validationResults, true);

            if (isValid)
            {
               
                _context.Add(oneTimePaymentStep3);

                await _context.SaveChangesAsync();


                oneTimePaymentAsset.OneTimePaymentStep3Id = oneTimePaymentStep3.OneTimePaymentStep3Id;
                oneTimePaymentAsset.Status = "акт";

                var oneTimePayment_step3 = _context.OneTimePaymentStep3.Find(oneTimePaymentStep3.OneTimePaymentStep3Id);


                if (actFile != null)
                {
                    var createdFile = UploadFile(oneTimePayment_step3.OneTimePaymentStep3Id, actFile, 3);
                    oneTimePayment_step3.ActAndAssetFileId = createdFile.Result.FileId;
                    oneTimePayment_step3.ActAndAssetFileLink = createdFile.Result.SystemPath;
                }

                if (invoiceFile != null)
                {
                    var createdFile = UploadFile(oneTimePayment_step3.OneTimePaymentStep3Id, invoiceFile, 3);
                    oneTimePayment_step3.InvoiceFileId = createdFile.Result.FileId;
                    oneTimePayment_step3.InvoiceFileLink = createdFile.Result.SystemPath;
                }

                await _context.SaveChangesAsync();

                TempData["Message"] = "File successfully uploaded to File System.";

                return Json(new { success = true, message = "Акт муваффақиятли яратилди!" });
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
        public async Task<IActionResult> EditAct([FromForm] string actDate,
                                                        [FromForm] string actNumber, [FromForm] string actId)
                                                         
        {
            if (actId == null)
            {
                return Json(new { success = false, message = "Хатолик!" });
            }
            int id = int.Parse(actId);
            DateTime dateTime = DateTime.Parse(actDate);

            var oneTimePaymentStep3 = await _context.OneTimePaymentStep3.FindAsync(id);
            if (oneTimePaymentStep3 == null)
            {
                return Json(new { success = false, message = "Хатолик - Акт топилмади!" });
            }

            oneTimePaymentStep3.ActAndAssetDate = dateTime;
            oneTimePaymentStep3.ActAndAssetNumber = actNumber;

            try
            {
                _context.Update(oneTimePaymentStep3);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Акт муваффақиятли таҳрирланди!" });


        }

        [HttpPost]
        public async Task<IActionResult> ReplaceFile([FromForm]IFormFile file, [FromForm]string fileId, [FromForm]string target, [FromForm]string finder)
        {
            if (fileId == null || target == null || file == null)
            {
                return Json(new { success = false, message = "Хатолик!" });
            }

            FileModel fileModel = new();


            OneTimePaymentAsset step1 = new();
            OneTimePaymentStep2 step2 = new();
            OneTimePaymentStep3 step3 = new();

            int id = int.Parse(fileId);

            if (target.Equals("1"))
            {
                fileModel = _context.FileModels.Where(f => f.OneTimePaymentAssetId == id).FirstOrDefault();
                step1 = _context.OneTimePaymentAssets.Find(id);
            }
            if (target.Equals("2"))
            {
                fileModel = _context.FileModels.Where(f => f.OneTimePaymentStep2Id == id).FirstOrDefault();
                step2 = _context.OneTimePaymentStep2.Find(id);
            }
            if (target.Equals("3"))
            {
                fileModel = _context.FileModels.Where(f => f.OneTimePaymentStep3Id == id).FirstOrDefault();
                step3 = _context.OneTimePaymentStep3.Find(id);
            }

            try
            {
                if (fileModel != null)
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
                    if (target.Equals("1"))
                    {

                        step1.SolutionFileLink = fileModel.SystemPath;

                    }
                    if (target.Equals("3"))
                    {
                        if (finder.Equals("1"))
                        {
                            step3.ActAndAssetFileLink = fileModel.SystemPath;
                        }

                        else if (finder.Equals("2"))
                        {
                            step3.InvoiceFileLink = fileModel.SystemPath;
                        }

                    }
                    if (target.Equals("2"))
                    {

                        step2.AggreementFileLink = fileModel.SystemPath;

                    }


                    await _context.SaveChangesAsync();

                }

                else if (fileModel == null)
                {
                    var createdFile = UploadFile(id, file, int.Parse(target));

                    if (target.Equals("1"))
                    {

                        step1.SolutionFileId = createdFile.Result.FileId;
                        step1.SolutionFileLink = createdFile.Result.SystemPath;
                    }
                    if (target.Equals("2"))
                    {

                        step2.AggreementFileId = createdFile.Result.FileId;
                        step2.AggreementFileLink = createdFile.Result.SystemPath;
                    }
                    if (target.Equals("3"))
                    {
                        if (finder.Equals("1"))
                        {
                            step3.ActAndAssetFileId = createdFile.Result.FileId;
                            step3.ActAndAssetFileLink = createdFile.Result.SystemPath;
                        }

                        else if (finder.Equals("2"))
                        {
                            step3.InvoiceFileId = createdFile.Result.FileId;
                            step3.InvoiceFileLink = createdFile.Result.SystemPath;
                        }


                    }

                    await _context.SaveChangesAsync();

                }
            }
            catch (NotSupportedException ex)
            {
                return Json(new {success = false, message = ex.Message});
            }


            return Json(new { success = true, message = "Янги ҳужжат муваффақиятли юкланди!" });
        }
        public async Task<FileModel> UploadFile(int id, IFormFile file, int target)
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

            await _context.FileModels.AddAsync(fileModel);
            await _context.SaveChangesAsync();

            var createdFile = await _context.FileModels.Where(f => f.FilePath == filePath).FirstOrDefaultAsync();

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

        
        [HttpPost]
        public async Task<IActionResult> GetAssetsOnSale([FromBody] JObject data)
        {
            if (data["target"] == null || data["type"] == null)
            {
                return Json(new {success = false, message = "Хатолик юз берди!" });
            }

            var userId = _userManager.GetUserId(User);

            List<OneTimePaymentAsset> oneTimePaymentAssetList = new();
            string name ="";
            bool status = false;
            int target = (int)data["target"];
            int type = (int)data["type"];   

            if (type == 1)
            {
                if (target == 0)
                {
                    oneTimePaymentAssetList = await _context.OneTimePaymentAssets.Include(o => o.RealEstate).Where(o => o.Confirmed == false && o.Status.Equals("сотувда") && o.Step2 == null 
                                                                                            && o.Step3 == null && o.Share == null && o.RealEstate != null && o.RealEstate.ApplicationUserId == userId).ToListAsync();
                }

                if (target == 1)
                {
                    oneTimePaymentAssetList = await _context.OneTimePaymentAssets.Include(o => o.RealEstate).Where(o => o.Confirmed == true && o.Status.Equals("сотувда") &&  o.Step2 == null 
                                                                                            && o.Step3 == null &&  o.Share == null && o.RealEstate != null && o.RealEstate.ApplicationUserId == userId).ToListAsync();
                }
            }

            if (type == 2)
            {
                if (target == 0)
                {
                    oneTimePaymentAssetList = await _context.OneTimePaymentAssets.Where(o => o.Confirmed == false && o.Step2 == null && o.Status.Equals("сотувда")
                                                                                            && o.Step3 == null && o.Share != null && o.RealEstate == null && o.Share.ApplicationUserId == userId).ToListAsync();
                }

                if (target == 1)
                {
                    oneTimePaymentAssetList = await _context.OneTimePaymentAssets.Where(o => o.Confirmed == true  && o.Step2 == null && o.Status.Equals("сотувда")
                                                                                            && o.Step3 == null && o.Share != null && o.RealEstate == null && o.Share.ApplicationUserId == userId).ToListAsync();
                }
            }


            List<GeneralViewModel> viewmodels = new();

            foreach(var item in oneTimePaymentAssetList)
            {
                if(type == 1)
                    name = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == item.RealEstateId).RealEstateName;
                else if(type == 2)
                    name = _context.Shares.FirstOrDefault(s => s.ShareId == item.ShareId).BusinessEntityName;

                if (_context.SubmissionOnBiddings.Any(s => s.RealEstateId == item.RealEstateId && s.Status == "Сотилди"))
                    status = true;

                item.BiddingDateStr = item.BiddingDate.ToShortDateString();
                item.SolutionDateStr = item.SolutionDate.ToShortDateString();
                GeneralViewModel viewmodel = new()
                {
                    Name = name,
                    OneTimePaymentAsset = item,
                    Status = status,
                };

                viewmodels.Add(viewmodel);
            }

            return Json(new { data = viewmodels});
        }

        [HttpPost]
        public async Task<IActionResult> GetAssetsOnContract([FromBody] JObject data)
        {
            if (data["target"] == null || data["type"] == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            int target = (int)data["target"];
            int type = (int)data["type"];

            var userId = _userManager.GetUserId(User);

            List<OneTimePaymentStep2> oneTimePaymentStep2List = new();
            string name="";

            if (type==1)
            {
                if (target==0)
                {
                    oneTimePaymentStep2List = await _context.OneTimePaymentStep2.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.RealEstate).Where(o => o.Confirmed == false && o.ContractCancelledDate > DateTime.Now && o.OneTimePaymentAsset != null && o.OneTimePaymentAsset.Step3 == null
                                                                                             && o.OneTimePaymentAsset.RealEstate != null && o.OneTimePaymentAsset.RealEstate.ApplicationUserId == userId).ToListAsync();
                }

                if (target==1)
                {
                    oneTimePaymentStep2List = await _context.OneTimePaymentStep2.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.RealEstate).Where(o => o.Confirmed == true && o.ContractCancelledDate > DateTime.Now && o.OneTimePaymentAsset != null && o.OneTimePaymentAsset.Step3 == null
                                                                                            && o.OneTimePaymentAsset.RealEstate != null && o.OneTimePaymentAsset.RealEstate.ApplicationUserId == userId).ToListAsync();
                }
            }

            if (type==2)
            {
                if (target==0)
                {
                    oneTimePaymentStep2List = await _context.OneTimePaymentStep2.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.Share).Where(o => o.Confirmed == false && o.ContractCancelledDate > DateTime.Now && o.OneTimePaymentAsset != null && o.OneTimePaymentAsset.Step3 == null
                                                                                             && o.OneTimePaymentAsset.Share != null && o.OneTimePaymentAsset.Share.ApplicationUserId == userId).ToListAsync();
                }

                if (target==1)
                {
                    oneTimePaymentStep2List = await _context.OneTimePaymentStep2.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.Share).Where(o => o.Confirmed == true && o.ContractCancelledDate > DateTime.Now && o.OneTimePaymentAsset != null && o.OneTimePaymentAsset.Step3 == null
                                                                                             && o.OneTimePaymentAsset.Share != null && o.OneTimePaymentAsset.Share.ApplicationUserId == userId).ToListAsync();
                }
            }

            List<GeneralViewModel> viewmodels = new();
            OneTimePaymentAsset oneTimePaymentAsset = new();

            foreach (var item in oneTimePaymentStep2List)
            {
                try
                {
                    if (type==1)
                        name = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == item.OneTimePaymentAsset.RealEstateId).RealEstateName;
                    else if (type==2)
                        name = _context.Shares.FirstOrDefault(s => s.ShareId == item.OneTimePaymentAsset.ShareId).BusinessEntityName;
                    
                    oneTimePaymentAsset = _context.OneTimePaymentAssets.FirstOrDefault(o => o.OneTimePaymentStep2Id == item.OneTimePaymentStep2Id);
                    item.AgreementDateStr = item.AggreementDate.ToShortDateString();
                    oneTimePaymentAsset.SolutionDateStr = oneTimePaymentAsset.SolutionDate.ToShortDateString();
                    oneTimePaymentAsset.BiddingDateStr = oneTimePaymentAsset.BiddingDate.ToShortDateString();
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                }
                
                
                GeneralViewModel viewmodel = new()
                {
                    Name = name,
                    OneTimePaymentStep2 = item,
                    OneTimePaymentAsset = oneTimePaymentAsset,
                    
                };

                viewmodels.Add(viewmodel);
            }

            return Json(new { data = viewmodels });
        }

        [HttpPost]
        public async Task<IActionResult> GetAssetsOnAct([FromBody] JObject data)
        {
            if (data["target"] == null || data["type"] == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            List<OneTimePaymentStep3> oneTimePaymentStep3List = new();
            string name = "";
            int target = (int)data["target"];
            int type = (int)data["type"];

            var userId = _userManager.GetUserId(User);

            if (type == 1)
            {
                if (target == 0)
                {
                    oneTimePaymentStep3List = await _context.OneTimePaymentStep3.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.RealEstate).Where(o => o.Confirmed == false && o.OneTimePaymentAsset != null
                                                                                             && o.OneTimePaymentAsset.RealEstate != null && o.OneTimePaymentAsset.RealEstate.ApplicationUserId == userId).ToListAsync();
                }

                if (target == 1)
                {
                    oneTimePaymentStep3List = await _context.OneTimePaymentStep3.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.RealEstate).Where(o => o.Confirmed == true && o.OneTimePaymentAsset != null
                                                                                            && o.OneTimePaymentAsset.RealEstate != null && o.OneTimePaymentAsset.RealEstate.ApplicationUserId == userId).ToListAsync();
                }
            }

            if (type == 2)
            {
                if (target == 0)
                {
                    oneTimePaymentStep3List = await _context.OneTimePaymentStep3.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.Share).Where(o => o.Confirmed == false && o.OneTimePaymentAsset != null
                                                                                             && o.OneTimePaymentAsset.Share != null && o.OneTimePaymentAsset.Share.ApplicationUserId == userId).ToListAsync();
                }

                if (target == 1)
                {
                    oneTimePaymentStep3List = await _context.OneTimePaymentStep3.Include(o => o.OneTimePaymentAsset).ThenInclude(o => o.Share).Where(o => o.Confirmed == true && o.OneTimePaymentAsset != null
                                                                                             && o.OneTimePaymentAsset.Share != null && o.OneTimePaymentAsset.Share.ApplicationUserId == userId).ToListAsync();
                }
            }

           
            List<GeneralViewModel> viewmodels = new();
            OneTimePaymentAsset oneTimePaymentAsset = new();
            OneTimePaymentStep2 oneTimePaymentStep2 = new();

            foreach (var item in oneTimePaymentStep3List)
            {
                try
                {
                    if (type == 1)
                        name = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == item.OneTimePaymentAsset.RealEstateId).RealEstateName;
                    else if (type == 2)
                        name = _context.Shares.FirstOrDefault(s => s.ShareId == item.OneTimePaymentAsset.ShareId).BusinessEntityName;
                   
                    oneTimePaymentAsset = _context.OneTimePaymentAssets.FirstOrDefault(o => o.OneTimePaymentStep3Id == item.OneTimePaymentStep3Id);
                    oneTimePaymentStep2 = _context.OneTimePaymentStep2.FirstOrDefault(o => o.OneTimePaymentStep2Id == oneTimePaymentAsset.OneTimePaymentStep2Id);
                    item.ActAndAssetDateStr = item.ActAndAssetDate.ToShortDateString();
                    oneTimePaymentAsset.SolutionDateStr = oneTimePaymentAsset.SolutionDate.ToShortDateString();
                    oneTimePaymentAsset.BiddingDateStr = oneTimePaymentAsset.BiddingDate.ToShortDateString();
                    oneTimePaymentStep2.AgreementDateStr = oneTimePaymentStep2.AggreementDate.ToShortDateString();  
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                }


                GeneralViewModel viewmodel = new()
                {
                    Name = name,
                    OneTimePaymentStep3 = item,
                    OneTimePaymentAsset = oneTimePaymentAsset,
                    OneTimePaymentStep2 = oneTimePaymentStep2

                };

                viewmodels.Add(viewmodel);
            }

            return Json(new { data = viewmodels });
        }

        [HttpPost]
        public async Task<IActionResult> GetNotSoldAssets([FromBody]int? type)
        {
            if(type == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            List<GeneralViewModel> viewmodels = new();
            List<OneTimePaymentAsset> oneTimePaymentAssetList = new();

            var userId = _userManager.GetUserId(User);

            if(type == 1)
            {
                oneTimePaymentAssetList = await _context.OneTimePaymentAssets.Include(o => o.RealEstate).Where(o => o.Status == "сотилмади" && o.RealEstate != null && o.RealEstate.ApplicationUserId == userId).ToListAsync();
            }

            if (type == 2)
            {
                oneTimePaymentAssetList = await _context.OneTimePaymentAssets.Include(o => o.Share).Where(o => o.Status == "сотилмади" && o.Share != null && o.Share.ApplicationUserId == userId).ToListAsync();
            }


            OneTimePaymentStep2 oneTimePaymentStep2 = new();
            OneTimePaymentStep3 oneTimePaymentStep3 = new();

            string name = "";

            foreach (var item in oneTimePaymentAssetList)
            {
                try
                {
                    if(type == 1)
                        name = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == item.RealEstateId).RealEstateName;
                    if(type == 2)
                        name = _context.Shares.FirstOrDefault(r => r.ShareId == item.ShareId).BusinessEntityName;

                    oneTimePaymentStep2 = await _context.OneTimePaymentStep2.FirstOrDefaultAsync(o => o.OneTimePaymentStep2Id == item.OneTimePaymentStep2Id);
                    oneTimePaymentStep3 = await _context.OneTimePaymentStep3.FirstOrDefaultAsync(o => o.OneTimePaymentStep3Id == item.OneTimePaymentStep3Id);
                    item.SolutionDateStr = item.SolutionDate.ToShortDateString(); 
                    item.BiddingDateStr = item.BiddingDate.ToShortDateString();
                    
                    if(oneTimePaymentStep2 != null)
                        oneTimePaymentStep2.AgreementDateStr = oneTimePaymentStep2.AggreementDate.ToShortDateString();
                    if(oneTimePaymentStep3 != null)
                        oneTimePaymentStep3.ActAndAssetDateStr = oneTimePaymentStep3.ActAndAssetDate.ToShortDateString();
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                }


                GeneralViewModel viewmodel = new()
                {
                    Name = name,
                    
                    OneTimePaymentAsset = item,
                    OneTimePaymentStep2 = oneTimePaymentStep2,
                    OneTimePaymentStep3 = oneTimePaymentStep3,

                };

                viewmodels.Add(viewmodel);
            }

            return Json(new { data = viewmodels });
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] JObject data)
        {

            if(data["id"] == null || data["target"] == null)
            {
                return Json(new { Success = false, message = "Хатолик юз берди!" });
            }

            int id = (int)(data["id"]);
            int target = (int)(data["target"]);

            OneTimePaymentAsset oneTimePaymentAsset;
            OneTimePaymentStep2 oneTimePaymentStep2;
            OneTimePaymentStep3 oneTimePaymentStep3;    

            if(target == 1)
            {
                oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(id);

                if (oneTimePaymentAsset == null)
                {
                    return Json(new { Success = false, message = "Хатолик юз берди - Объект топилмади!" });
                }

                try
                {
                    oneTimePaymentAsset.Confirmed = true;
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }

            if (target == 2)
            {
                oneTimePaymentStep2 = await _context.OneTimePaymentStep2.FindAsync(id);

                if (oneTimePaymentStep2 == null)
                {
                    return Json(new { Success = false, message = "Хатолик юз берди - Объект топилмади!" });
                }

                try
                {
                    oneTimePaymentStep2.Confirmed = true;
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }

            if (target == 3)
            {
                oneTimePaymentStep3 = await _context.OneTimePaymentStep3.FindAsync(id);
                if (oneTimePaymentStep3 == null)
                {
                    return Json(new { Success = false, message = "Хатолик юз берди - Объект топилмади!" });
                }
                var step1 = await _context.OneTimePaymentAssets.Include(o => o.RealEstate).Include(o => o.Share).FirstOrDefaultAsync(o => o.OneTimePaymentStep3Id == id);
                
                Share share;
                RealEstate realEstate;

                if (step1.RealEstate != null)
                {
                    realEstate = step1.RealEstate;
                    realEstate.Status = false;
                    realEstate.OutOfAccountDate = oneTimePaymentStep3.ActAndAssetDate;
                }

                else if(step1.Share != null)
                {
                    share = step1.Share;
                    share.Status = false;
                    share.OutOfAccountDate = oneTimePaymentStep3.ActAndAssetDate;
                }
                    
                try
                {
                    oneTimePaymentStep3.Confirmed = true;
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }

            return Json(new { Success = true, message = "Маълумотлар тасдиқланди!" });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSale([FromBody] JObject data)
        {
            
            if (data["id"] == null || data["target"] == null)
            {
                return Json(new { Success = false, message = "Хатолик юз берди!" });
            }

            int id = (int)data["id"];
            int target = (int)data["target"];

            RealEstate realEstate = new();
            Share share = new();


            if (target == 1)
            {
                try
                {
                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.Include(o=> o.RealEstate).Include(o => o.Share).FirstOrDefaultAsync(o => o.OneTimePaymentAssetId ==id);
                    if (oneTimePaymentAsset == null)
                    {
                        return Json(new { Success = false, message = "Хатолик юз берди - Объект топилмади!" });
                    }

                    if (oneTimePaymentAsset.RealEstate != null)
                    {
                        realEstate = _context.RealEstates.Find(oneTimePaymentAsset.RealEstateId);
                        realEstate.SubmissionOnBiddingOn = true;
                        realEstate.OneTimePaymentAssetOn = false;

                        var auktsion = _context.SubmissionOnBiddings.FirstOrDefault(s => s.Status.Equals("Сотилди") && s.RealEstateId == realEstate.RealEstateId);
                        auktsion.Status = "Сотилмади";
                        auktsion.IsActiveForPriceReduction = true;
                    }
                    else
                    {
                        share = _context.Shares.Find(oneTimePaymentAsset.ShareId);
                        share.SubmissionOnBiddingOn = true;
                        share.OneTimePaymentAssetOn = false;

                        var auktsion = _context.SubmissionOnBiddings.FirstOrDefault(s => s.Status.Equals("Сотилди") && s.ShareId == share.ShareId);
                        auktsion.Status = "Сотилмади";
                        auktsion.IsActiveForPriceReduction = true;
                    }
                    oneTimePaymentAsset.Status = "сотилмади";
                    oneTimePaymentAsset.BiddingCancelledDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }

            if (target == 2)
            {
                try
                {
                    var oneTimePaymentStep2 = await _context.OneTimePaymentStep2.FindAsync(id);
                    if (oneTimePaymentStep2 == null)
                    {
                        return Json(new { Success = false, message = "Хатолик юз берди - Объект топилмади!" });
                    }

                    var oneTimePaymentAsset = _context.OneTimePaymentAssets.Include(o => o.RealEstate).Include(o => o.Share).FirstOrDefault(o => o.OneTimePaymentStep2Id == id);
                    if (oneTimePaymentAsset == null)
                    {
                        return Json(new { Success = false, message = "Хатолик юз берди - Объект топилмади!" });
                    }

                    if (oneTimePaymentAsset.RealEstate != null)
                    {
                        realEstate = _context.RealEstates.Find(oneTimePaymentAsset.RealEstateId);
                        realEstate.SubmissionOnBiddingOn = true;
                        realEstate.OneTimePaymentAssetOn = false;

                        var auktsion = _context.SubmissionOnBiddings.FirstOrDefault(s => s.Status.Equals("Сотилди") && s.RealEstateId == realEstate.RealEstateId);
                        auktsion.Status = "Сотилмади";
                        auktsion.IsActiveForPriceReduction = true;
                    }
                    else
                    {
                        share = _context.Shares.Find(oneTimePaymentAsset.ShareId);
                        share.SubmissionOnBiddingOn = true;
                        share.OneTimePaymentAssetOn = false;

                        var auktsion = _context.SubmissionOnBiddings.FirstOrDefault(s => s.Status.Equals("Сотилди") && s.ShareId == share.ShareId);
                        auktsion.Status = "Сотилмади";
                        auktsion.IsActiveForPriceReduction = true;
                    }

                    oneTimePaymentAsset.Status = "сотилмади";
                    oneTimePaymentStep2.ContractCancelledDate = DateTime.Now;                    
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }



            return Json(new { Success = true, message = "Маълумотлар тасдиқланди!" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] JObject data)
        {
            if (data["id"] == null || data["target"]==null)
            {
                return Json(new { Success = false, message="Хатолик юз берди!"});
            }

            int id = (int)data["id"];
            int target = (int)data["target"];   

            OneTimePaymentAsset oneTimePaymentAsset = new();
            OneTimePaymentStep2 oneTimePaymentStep2 = new();
            OneTimePaymentStep3 oneTimePaymentStep3 = new();

            if (target == 1)
            {
                oneTimePaymentAsset = await _context.OneTimePaymentAssets.Include(o=>o.RealEstate).Include(o=>o.Share).FirstOrDefaultAsync(o => o.OneTimePaymentAssetId == id);

                RealEstate realEstate = new();
                Share share = new();

                if(oneTimePaymentAsset.RealEstate != null)
                {
                    realEstate = _context.RealEstates.FirstOrDefault(r => r.RealEstateId == oneTimePaymentAsset.RealEstateId);
                    realEstate.OneTimePaymentAssetOn = true;
                }

                else
                {
                    share = _context.Shares.FirstOrDefault(s => s.ShareId == oneTimePaymentAsset.ShareId);
                    share.OneTimePaymentAssetOn = true;
                }

                if (oneTimePaymentAsset == null) { return Json(new { Success = false, message = "Маълумот топилмади!" }); }

                var solutionFile = await _context.FileModels.FirstOrDefaultAsync(f => f.FileId == oneTimePaymentAsset.SolutionFileId);

                try
                {
                    _context.OneTimePaymentAssets.Remove(oneTimePaymentAsset);

                    if (solutionFile != null)
                    {
                        if (System.IO.File.Exists(solutionFile.FilePath))
                        {
                            System.IO.File.Delete(solutionFile.FilePath);
                        }
                        _context.FileModels.Remove(solutionFile);
                    }
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }

            if (target == 2)
            {
                oneTimePaymentStep2 = await _context.OneTimePaymentStep2.FindAsync(id);

                if (oneTimePaymentStep2 == null) { return Json(new { Success = false, message = "Маълумот топилмади!" }); }

                var agreementFile = await _context.FileModels.FirstOrDefaultAsync(f => f.FileId == oneTimePaymentStep2.AggreementFileId);
                var oneTimeP = await _context.OneTimePaymentAssets.FirstOrDefaultAsync(o => o.OneTimePaymentStep2Id == oneTimePaymentStep2.OneTimePaymentStep2Id);

                try
                {
                    oneTimeP.Step2 = null;
                    oneTimeP.OneTimePaymentStep2Id = null;
                    oneTimeP.Status = "сотувда";
                    _context.OneTimePaymentStep2.Remove(oneTimePaymentStep2);

                    if (agreementFile != null)
                    {
                        if (System.IO.File.Exists(agreementFile.FilePath))
                        {
                            System.IO.File.Delete(agreementFile.FilePath);
                        }
                        _context.FileModels.Remove(agreementFile);
                    }
                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }

            if (target == 3)
            {
                oneTimePaymentStep3 = await _context.OneTimePaymentStep3.FindAsync(id);

                if (oneTimePaymentStep3 == null) { return Json(new { Success = false, message = "Маълумот топилмади!" }); }

                var actFile = await _context.FileModels.FirstOrDefaultAsync(f => f.FileId == oneTimePaymentStep3.ActAndAssetFileId);
                var invoiceFile = await _context.FileModels.FirstOrDefaultAsync(f => f.FileId == oneTimePaymentStep3.InvoiceFileId);

                var oneTimeP = await _context.OneTimePaymentAssets.FirstOrDefaultAsync(o => o.OneTimePaymentStep3Id == oneTimePaymentStep3.OneTimePaymentStep3Id);

                try
                {
                    oneTimeP.Step3 = null;
                    oneTimeP.OneTimePaymentStep3Id = null;
                    oneTimeP.Status = "шартнома";
                    _context.OneTimePaymentStep3.Remove(oneTimePaymentStep3);

                    if (actFile != null)
                    {
                        if (System.IO.File.Exists(actFile.FilePath))
                        {
                            System.IO.File.Delete(actFile.FilePath);
                        }
                        _context.FileModels.Remove(actFile);
                    }

                    if (invoiceFile != null)
                    {
                        if (System.IO.File.Exists(invoiceFile.FilePath))
                        {
                            System.IO.File.Delete(invoiceFile.FilePath);
                        }
                        _context.FileModels.Remove(invoiceFile);
                    }

                    await _context.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    return Json(new { Success = false, message = ex.Message });
                }
            }




            return Json(new { Success = true, message = "Муваффақиятли бажарилди!" });
        }

        private bool OneTimePaymentAssetExists(int id)
        {
            return _context.OneTimePaymentAssets.Any(e => e.OneTimePaymentAssetId == id);
        }
        private bool OneTimePaymentAssetExistsStep2(int id)
        {
            return _context.OneTimePaymentStep2.Any(e => e.OneTimePaymentStep2Id == id);
        }
        private bool OneTimePaymentAssetExistsStep3(int id)
        {
            return _context.OneTimePaymentStep3.Any(e => e.OneTimePaymentStep3Id == id);
        }
        
    }
}
