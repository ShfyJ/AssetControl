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
using System.Net;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TemplatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public TemplatesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Templates
        public async Task<IActionResult> Index(int? target)
        {
            List<Template> templates = new();
            if(target == null)
            {
                templates = await _context.Template.Include(t => t.File).ToListAsync();
                target = 0;
            }

            if (target == 1) //realEstates
            {
                templates = await _context.Template.Include(t => t.File).Where(t => t.IsRealEstate && !t.IsShare).ToListAsync();
            }

            if (target == 2) //shares
            {
                templates = await _context.Template.Include(t => t.File).Where(t => t.IsShare && !t.IsRealEstate).ToListAsync();
            }

            ViewData["Target"] = target;
            var s = _context.Template.Where(t => t.IsRealEstate && !t.IsShare).Count();
            ViewData["RCount"] = s;
            ViewData["SCount"] = _context.Template.Where(t => !t.IsRealEstate && t.IsShare).Count();

            return View(templates);
        }


        // POST: Admin/Templates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]string TemplateName, [FromForm]IFormFile TemplateFile, [FromForm] string SelectedType, [FromForm]string HasUser, [FromForm] string HasTransferredAsset, [FromForm] string HasAssetEvaluation, [FromForm] string HasAuction, [FromForm] string HasReduction, [FromForm] string HasOneTimePayment, [FromForm] string HasInstallment)
        {

            bool isRealEstate = false;
            bool isShare = false;
            bool hasAuction = false;
            bool hasTransferredAsset = false;
            bool hasUser = false;
            bool hasAssetEvaluation = false;
            bool hasReduction = false;
            bool hasInstallment = false;
            bool hasOneTimePayment = false;

            var templates = _context.Template.Where(t => t.IsActive);

            string target = "";

            if (SelectedType.Equals("1"))
            {
                isRealEstate = true;
                target = "RealEstates";
                if(templates != null)
                 templates = templates.Where(t => t.IsRealEstate);
            }

            else if (SelectedType.Equals("2"))
            {
                isShare = true;
                target = "Shares";
                if (templates != null)
                    templates = templates.Where(t => t.IsShare);
            }

            if (HasTransferredAsset!=null)
            {
                hasTransferredAsset = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasTransferredAssets);
            }
            else
            {
                if (templates != null)
                {
                    templates = templates.Where(t => !t.HasTransferredAssets);
                }
            }

            if (HasAssetEvaluation != null)
            {
                hasAssetEvaluation = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasAssetEvaluation);
            }

            else
            {
                if (templates != null)
                    templates = templates.Where(t => !t.HasAssetEvaluation);
            }

            if (HasAuction != null)
            {
                hasAuction = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasAuction);
                
            }

            else
            {
                if (templates != null)
                    templates = templates.Where(t => !t.HasAuction);
            }

            if (HasReduction != null)
            {
                hasReduction = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasReductionInAsset);
            }
            else
            {
                if (templates != null)
                    templates = templates.Where(t => !t.HasReductionInAsset);
            }

            if (HasOneTimePayment != null)
            {
                hasOneTimePayment = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasOneTimePaymentAsset);
            }
            else
            {
                if (templates != null)
                    templates = templates.Where(t => !t.HasOneTimePaymentAsset);
            }

            if (HasInstallment != null)
            {
                hasInstallment = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasInstallmentAsset);
            }
            else
            {
                if (templates != null)
                    templates = templates.Where(t => !t.HasInstallmentAsset);
            }

            if (HasUser != null)
            {
                hasUser = true;
                if (templates != null)
                    templates = templates.Where(t => t.HasUser);                              
            }

            else
            {
                if (templates != null)
                    templates = templates.Where(t => !t.HasUser);
            }

            Template template = new()
            {
                TemplateName = TemplateName,
                IsActive = true,
                IsRealEstate = isRealEstate,
                IsShare = isShare,
                HasAssetEvaluation = hasAssetEvaluation,
                HasAuction = hasAuction,
                HasInstallmentAsset = hasInstallment,
                HasOneTimePaymentAsset = hasOneTimePayment,
                HasReductionInAsset = hasReduction,
                HasTransferredAssets = hasTransferredAsset,
                HasUser = hasUser,
                
            };

            if(template != null)
            {
                foreach (var item in templates)
                {
                    item.IsActive = false;
                }
            }

            var context = new ValidationContext(template, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(template, context, validationResults, true);

            if (isValid)
            {
                try
                {
                    var templateFile = await UploadFile(TemplateFile, target);

                    if (templateFile != null)
                        template.FileId = templateFile.FileId;
                    else
                    {
                        return Json(new { success = false, message = "Шаблон файлини юклашда хатолик юз берди!" });
                    }
                    _context.Add(template);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User));

                }
                catch(Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                

                return Json(new { success = true, message = "Шаблон қўшилди!" });

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
        public async Task<IActionResult> Edit([FromForm]string TemplateId, [FromForm] string TemplateName, [FromForm] IFormFile TemplateFile, [FromForm] string SelectedType, [FromForm] string HasUser, [FromForm] string HasTransferredAsset, [FromForm] string HasAssetEvaluation, [FromForm] string HasAuction, [FromForm] string HasReduction, [FromForm] string HasOneTimePayment, [FromForm] string HasInstallment)
        {

            var template = await _context.Template.FirstAsync(t => t.TemplateId == int.Parse(TemplateId));

            string target = "";

            if (SelectedType.Equals("1"))
            {
                template.IsRealEstate = true;
                target = "RealEstates";
               
            }
            else
            {
                template.IsRealEstate = false;
            }

            if (SelectedType.Equals("2"))
            {
                template.IsShare = true;
                target = "Shares";
            }
            else
            {
                template.IsShare = false;
            }

            if (HasTransferredAsset != null)
            {
                template.HasTransferredAssets = true;
               
            }
            else
            {
                template.HasTransferredAssets = false;
            }

            if (HasAssetEvaluation != null)
            {
                template.HasAssetEvaluation = true;
                
            }
            else
            {
                template.HasAssetEvaluation = false;
            }

            if (HasAuction != null)
            {
                template.HasAuction = true;


            }
            else
            {
                template.HasAuction = false;
            }

            if (HasReduction != null)
            {
                template.HasReductionInAsset = true;

            }
            else
            {
                template.HasReductionInAsset = false;
            }

            if (HasOneTimePayment != null)
            {
                template.HasOneTimePaymentAsset = true;

            }
            else
            {
                template.HasOneTimePaymentAsset = false;
            }

            if (HasInstallment != null)
            {
                template.HasInstallmentAsset = true;

            }
            else
            {
                template.HasInstallmentAsset = false;
            }

            if (HasUser != null)
            {
                template.HasUser = true;
            }
            else
            {
                template.HasUser = false;
            }

            template.TemplateName = TemplateName;

            var context = new ValidationContext(template, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(template, context, validationResults, true);

            if (isValid)
            {
                try
                {
                    if(TemplateFile != null)
                    {
                        var result = await ReplaceFile(TemplateFile, template.FileId, target, template.TemplateId);
                         if(!result)
                        {
                            return Json(new { success = false, message = "Шаблон юклашда хатолик юз берди!" });
                        }
                    }
                   
                    _context.Update(template);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User));

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
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
        public async Task<IActionResult> ChangeStatus([FromForm] string TemplateId, [FromForm]string Status)
        {
            if(TemplateId == null)
            {
                return Json(new { success = false, message = "Хатолик - Шаблон аниқланмади!" });
            }
            var templateId = int.Parse(TemplateId);
            var template = await _context.Template.FirstOrDefaultAsync(t => t.TemplateId == templateId);
            
            if(Status == null)
                template.IsActive = false;
            else
            {
                template.IsActive = true;
                var templates = _context.Template.Where(t => t.IsRealEstate == template.IsRealEstate && t.HasTransferredAssets == template.HasTransferredAssets
                                    && t.HasAssetEvaluation == template.HasAssetEvaluation && t.HasAuction == template.HasAuction && t.HasReductionInAsset == template.HasReductionInAsset
                                    && t.HasOneTimePaymentAsset == template.HasOneTimePaymentAsset && t.HasInstallmentAsset == t.HasInstallmentAsset && t.HasUser == template.HasUser && t.IsActive);

                foreach (var item in templates)
                {
                    item.IsActive = false;
                }
            }
               

            try
            {
                await _context.SaveChangesAsync(_userManager.GetUserId(User));
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            if(Status == null)
                return Json(new { success = true, message = "Шаблон нофаол қилинди!" });

            else
            {
                return Json(new { success = true, message = "Шаблон фаоллаштирилди!" });
            }
        }
        // GET: Admin/Templates/Edit/5
       
        public async Task<FileModel> UploadFile(IFormFile file, string target)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/Templates/" + target);

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
            var systemPath = Path.Combine("/Files/Templates/" + target + "/" + temp + extension);

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
                UploadedById = _userManager.GetUserId(User),

            };

            await _context.FileModels.AddAsync(fileModel);
            await _context.SaveChangesAsync();

            var createdFile = await _context.FileModels.Where(f => f.FilePath == filePath).FirstOrDefaultAsync();

            return createdFile;

        }


        public async Task<bool> ReplaceFile(IFormFile file, int fileId, string target, int templateId)
        {
            if (file == null)
            {
                return false;
            }

            var fileModel = await _context.FileModels.FirstAsync(f => f.FileId == fileId);

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
                fileModel.SystemPath = Path.Combine("/Files/Templates/" + target + "/" + temp + extension);

                await _context.SaveChangesAsync();

            }

            else if (fileModel == null)
            {
                var createdFile = UploadFile(file, target);
                if (createdFile != null)
                {
                    var template = await _context.Template.FirstAsync(t => t.TemplateId == templateId);
                    template.FileId = createdFile.Result.FileId;
                }
                await _context.SaveChangesAsync();

            }

            return true;
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
           
            var template = await _context.Template.Where(x => x.TemplateId == id).FirstOrDefaultAsync();
            
            try
            {

                if (template == null)
                {
                    return NotFound();
                }

                var file = await _context.FileModels.Where(x => x.FileId == template.FileId).FirstOrDefaultAsync();


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
                return NotFound();

            }
            catch (FileNotFoundException ex)
            {
                return Json(new {message = ex.Message });
            }

        }

        // GET: Admin/Templates/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new {success = false, message= "Шаблон аниқлашда хатолик - қайтадан уриниб кўринг!"});
            }

            var template = await _context.Template
                .Include(t => t.File)
                .FirstOrDefaultAsync(m => m.TemplateId == id);
            
            if (template == null)
            {
                return Json(new { success = false, message = "Шаблон топилмади!" });
            }

            try
            {
                _context.Template.Remove(template);
                await _context.SaveChangesAsync(_userManager.GetUserId(User));
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); 
            }

            return Json(new { success = true, message = "Ўчирилди!" });
        }

        private bool TemplateExists(int id)
        {
            return _context.Template.Any(e => e.TemplateId == id);
        }
    }
}
