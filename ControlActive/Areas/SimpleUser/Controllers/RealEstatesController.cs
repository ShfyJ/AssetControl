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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;
using System.Security.Claims;
using ControlActive.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using ControlActive.Services;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ControlActive.Areas.SimpleUser.Controllers
{
    [Area("SimpleUser")]
    [Authorize(Roles = DefaultRoles.Role_SimpleUser)]
    public class RealEstatesController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpClientServiceImplementation _httpClientService;

        public RealEstatesController(ApplicationDbContext context, UserManager<IdentityUser> userManager
            , IWebHostEnvironment hostEnvironment, IHttpClientServiceImplementation httpClientService)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _httpClientService = httpClientService; 
        }

        // GET: SimpleUser/RealEstates


        public ActionResult Index(bool success = false, bool editSuccess = false)
        {
            
            ViewBag.Success = success;
            ViewBag.EditSuccess = editSuccess;

            return View();
        }


        public IActionResult Cadastre()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCadastre([FromBody] JObject data)
        {
            if (data["cad_num"] == null)
            {
                return NotFound();
            }

            string cad_num = data["cad_num"].ToString();
            await _httpClientService.Execute(cad_num);

            return Json(new { success = true, message = "Маълумотлар тасдиқланди!" });
        } 
        
        public async Task<IActionResult> SelectRealEstate(int? id)
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
                return View(await _context.RealEstates.Where(r => r.TransferredAssetOn == true && r.Confirmed == true
                 && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Оценка актива»
            if (id == 2)
            {
                return View(await _context.RealEstates.Where(r => r.AssetEvaluationOn == true && r.Confirmed == true && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Выставление на торги»
            if (id == 3)
            {

                return View(await _context.RealEstates.Include(r => r.AssetEvaluations).Where(r =>  r.SubmissionOnBiddingOn == true && r.Confirmed == true && r.ApplicationUserId == userId && r.AssetEvaluations.Count != 0).ToListAsync());
            }

            //«Пошаговое снижение стоимости актива» 
            if (id == 4)
            {
                
                return View(await _context.RealEstates.Include(r => r.SubmissionOnBiddings).Include(r => r.ReductionInAssets).Where(r => r.Confirmed == true && r.SubmissionOnBiddings.Any(s => s.IsActiveForPriceReduction == true) == true
                && r.ReductionInAssets.Any(a => a.Status == true) == false && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Реализованные активы с единовременной оплатой»
            if (id == 5)
            {
                return View(await _context.RealEstates.Include(r => r.SubmissionOnBiddings).Where(r => r.OneTimePaymentAssetOn == true && r.Confirmed == true && r.SubmissionOnBiddings.Any(s => s.Status == "Сотувда" || s.Status == "Сотилди") == true
                && r.ApplicationUserId == userId).ToListAsync());
            }

            //«Активы, реализованные в рассрочку»
            if (id == 6)
            {
                return View(await _context.RealEstates.Include(r => r.SubmissionOnBiddings).Where(r => r.InstallmentAssetOn == true && r.Confirmed == true && r.ApplicationUserId == userId).ToListAsync());
            }

            return NotFound();
        }

        public async Task<IActionResult> GetUsersIds()
        {
            var user = await _context.ApplicationUsers.FirstAsync(s => s.Id == _userManager.GetUserId(User));
            
            if(user == null)
            {
                return Json(new { success = false, message = "Xатолик юз берди!" });
            }

            var ToUser = _context.ApplicationUsers.FirstOrDefault(s => s.Id == user.CreatedById);
            
            if(ToUser == null)
            {
                return Json(new { success = false, message = "Xатолик юз берди!" });
            }

            string []ids = { user.Id, ToUser.Id };

            return Json(new {data = ids, success = true });

        }

        [HttpPost]
        public IActionResult GetRealEstate([FromBody] JObject data)
        {
            if (data["id"] == null || data["forDetails"] == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            int id = (int)(data["id"]);
            bool forDetails = (bool)(data["forDetails"]);

            var realEstate = _context.RealEstates
                .Include(r => r.DistrictOfObject)
                .Include(r => r.Proposal)
                .Include(r => r.RegionOfObject)
                .FirstOrDefault(r => r.RealEstateId == id);

            if (realEstate == null)
            {
                return Json(new { success = false, message = "Объект топилмади!" });
            }

            realEstate.CadasterRegDateStr = realEstate.CadastreRegDate.ToShortDateString();
            realEstate.CommisioningDateStr = realEstate.CommisioningDate.ToShortDateString();

            realEstate.InfrastructureNames = "";
            var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == realEstate.RealEstateId).ToList();
            

            foreach (var temp in realEstateInfrastructures)
            {
                var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();

                realEstate.InfrastructureNames += infrastructure.InfrastructureName + "; ";

            }

            List<RealEstate> realEstateData = new()
            {
                realEstate
            };

            if (forDetails)
                return Json(new { data = realEstateData });

            return Json(new { data = realEstate, success = true });
        }
        // GET: SimpleUser/RealEstates/Create
        public IActionResult Create()
        {
            //var Infrastructures = _context.Infrastuctures.ToList();

            var userId = _userManager.GetUserId(User);
            var organizationId = _context.ApplicationUsers.FirstOrDefault(a => a.Id == userId).OrganizationId;
            var name = _context.Organizations.FirstOrDefault(u => u.OrganizationId == organizationId).OrganizationName;
            ViewBag.Name = name;

            List<SelectListItem> empl = new();
            foreach (var item in _context.Infrastuctures)
            {
                SelectListItem selectListItem = new()
                {
                    Text = item.InfrastructureName,
                    Value = item.InfrastructureId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            //List<SelectListItem> empl2 = new();
            //foreach (var item in _context.TechnicalCharcs)
            //{
            //    SelectListItem selectListItem = new()
            //    {
            //        Text = item.TechnicalCharcName,
            //        Value = item.TechnicalCharcId.ToString(),

            //    };
            //    SelectListItem sel2 = selectListItem;
            //    empl2.Add(sel2);
            //}
            ViewBag.Infrastructures = empl;
            //ViewBag.TechnicalCharcs = empl2;

            ViewData["ProposalId"] = new SelectList(_context.Proposals, "ProposalId", "ProposalName");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName");
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName"); ;

            //  ViewData["Infrastructures"] = _context.Infrastuctures.Select(i => new SelectListItem { Value = i.InfrastructureId.ToString(), Text = i.InfrastructureName }).ToList();

            return View();
        }

        // POST: SimpleUser/RealEstates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile cadastreFile, IFormFile myImage1, IFormFile myImage2, IFormFile myImage3, List<int> Infrastructures, 
                                                [Bind("RealEstateId,RealEstateName,Status,OutOfAccountDate,CadastreNumber,Infrastructures,CadastreRegDate,CommisioningDate,Activity,ApplicationUserId,TransferredAssetId," +
                                                "RegionId,DistrictId,Address,AssetHolderName,FullArea,BuildingArea,NumberOfEmployee," +
                                                "MaintenanceCostForYear,InitialCostOfObject,Wear,ResidualValueOfObject,ProposalId, Comment")] RealEstate realEstate)

        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                realEstate.ApplicationUserId = userId;


                realEstate.Date_Added = DateTime.Now;
                realEstate.Status = true;
                realEstate.OutOfAccountDate = DateTime.Now.AddYears(1000);

                _context.Add(realEstate);

                await _context.SaveChangesAsync(userId, realEstate.RealEstateName);
                

                var real_Estate = _context.RealEstates.Find(realEstate.RealEstateId);

                foreach (var i in Infrastructures)
                {
                    RealEstateInfrastructure rI = new()
                    {
                        RealEstateId = realEstate.RealEstateId,
                        InfrastuctureId = i
                    };
                    _context.RRealEstateInfrastructures.Add(rI);
                    await _context.SaveChangesAsync(userId);
                }

                Task<FileModel> createdFile;
                Bitmap image;

                if (myImage1 != null)
                {
                    using var memoryStream = new MemoryStream();
                    await myImage1.CopyToAsync(memoryStream);
                    using var img = Image.FromStream(memoryStream);
                    image = ResizeImage(img, 370, 180);
                    createdFile = UploadImage(realEstate.RealEstateId, image, myImage1.FileName, myImage1.ContentType);
                    real_Estate.PhotoOfObject1Id = createdFile.Result.FileId;
                    real_Estate.PhotoOfObjectLink1 = createdFile.Result.SystemPath;

                }
                    
                if (myImage2 != null)
                {
                    using var memoryStream = new MemoryStream();
                    await myImage2.CopyToAsync(memoryStream);
                    using var img = Image.FromStream(memoryStream);
                    image = ResizeImage(img, 370, 180);
                    createdFile = UploadImage(realEstate.RealEstateId, image, myImage2.FileName, myImage2.ContentType);
                    real_Estate.PhotoOfObject2Id = createdFile.Result.FileId;
                    real_Estate.PhotoOfObjectLink2 = createdFile.Result.SystemPath;
                }
                    
                if (myImage3 != null)
                {
                    using var memoryStream = new MemoryStream();
                    await myImage3.CopyToAsync(memoryStream);
                    using var img = Image.FromStream(memoryStream);
                    image = ResizeImage(img, 370, 180);
                    createdFile = UploadImage(realEstate.RealEstateId, image, myImage3.FileName, myImage3.ContentType);
                    real_Estate.PhotoOfObject3Id = createdFile.Result.FileId;
                    real_Estate.PhotoOfObjectLink3 = createdFile.Result.SystemPath;
                }

                if(cadastreFile != null && cadastreFile.Length != 0)
                {
                    createdFile = UploadFile(realEstate.RealEstateId, cadastreFile);

                    real_Estate.CadastreFileId = createdFile.Result.FileId;
                    real_Estate.CadastreFileLink = createdFile.Result.SystemPath;

                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "RealEstates", new { success = true });

            }


            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName", realEstate.DistrictId);
            ViewData["ProposalId"] = new SelectList(_context.Proposals, "ProposalId", "ProposalName", realEstate.ProposalId);
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName", realEstate.RegionId);
            //ViewData["TechnicalCharcs"] = _context.TechnicalCharcs.Select(i => new SelectListItem { Value = i.TechnicalCharcId.ToString(), Text = i.TechnicalCharcName }).ToList();
            ViewData["Infrastructures"] = _context.Infrastuctures.Select(i => new SelectListItem { Value = i.InfrastructureId.ToString(), Text = i.InfrastructureName }).ToList();
            return View();
        }


        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public async Task<FileModel> UploadImage(int id, Bitmap image, string fileN, string contentType)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/RealEstates/" + id.ToString());
            bool basePathExists = Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var fileName = Path.GetFileNameWithoutExtension(fileN);
            var filePath = Path.Combine(basePath, fileN);
            var extension = ".jpg";
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

            var systemPath = Path.Combine("/Files/RealEstates/" + id.ToString() + "/" + temp + extension);

            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);
            // Save the bitmap as a JPEG file with quality level 75.
            myEncoderParameter = new EncoderParameter(myEncoder, 75L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            image.Save(filePath, myImageCodecInfo, myEncoderParameters);

            var fileModel = new FileModel
            {
                CreatedOn = DateTime.UtcNow,
                FileType = contentType,
                Extension = extension,
                Name = temp,
                FilePath = filePath,
                SystemPath = systemPath,
                BasePath = basePath,
                RealEstateId = id
            };
            _context.FileModels.Add(fileModel);
            await _context.SaveChangesAsync();

            var createdFile = _context.FileModels.Where(f => f.FilePath == filePath).FirstOrDefault();

            return createdFile;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public async Task<FileModel> UploadFile(int id, IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var basePath = Path.Combine(wwwRootPath + "/Files/RealEstates/" + id.ToString());

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

            var systemPath = Path.Combine("/Files/RealEstates/" + id.ToString() + "/" + temp + extension);

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
                RealEstateId = id
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
                return Redirect("/NotFound");

            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ReplaceFile(int realEstateId, int fileId, IFormFile file, int finder)
        {
            var fileModel = _context.FileModels.Where(f => f.FileId == fileId && f.RealEstateId == realEstateId).FirstOrDefault();
            var real_Estate = _context.RealEstates.Find(realEstateId);
            try
            {
                if (fileModel != null && file != null)
                {
                    if (System.IO.File.Exists(fileModel.FilePath))
                    {
                        System.IO.File.Delete(fileModel.FilePath);
                    }

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    var basePath = Path.Combine(wwwRootPath + "/Files/RealEstates/" + realEstateId.ToString());

                    bool basePathExists = Directory.Exists(fileModel.BasePath);
                    if (!basePathExists || !fileModel.BasePath.Equals(basePath))
                    {
                        
                        bool newBasePathExists = Directory.Exists(basePath);
                        if (!newBasePathExists)
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        fileModel.BasePath = basePath;
                    }

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

                    
                    if(finder == 0)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            var a = file.CopyToAsync(stream).IsCompletedSuccessfully;
                        }
                    }

                    else
                    {
                        Bitmap image;

                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                               image = ResizeImage(img, 370, 180);
                            }
                        }

                        ImageCodecInfo myImageCodecInfo;
                        System.Drawing.Imaging.Encoder myEncoder;
                        EncoderParameter myEncoderParameter;
                        EncoderParameters myEncoderParameters;

                        // Get an ImageCodecInfo object that represents the JPEG codec.
                        myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        // Create an Encoder object based on the GUID
                        // for the Quality parameter category.
                        myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        // Create an EncoderParameters object.
                        // An EncoderParameters object has an array of EncoderParameter
                        // objects. In this case, there is only one
                        // EncoderParameter object in the array.
                        myEncoderParameters = new EncoderParameters(1);
                        // Save the bitmap as a JPEG file with quality level 75.
                        myEncoderParameter = new EncoderParameter(myEncoder, 75L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        image.Save(filePath, myImageCodecInfo, myEncoderParameters);

                    }


                    fileModel.CreatedOn = DateTime.UtcNow;
                    fileModel.Extension = extension;
                    fileModel.FilePath = filePath;
                    fileModel.FileType = file.ContentType;
                    fileModel.Name = fileName;
                    fileModel.SystemPath = Path.Combine("/Files/RealEstates/" + realEstateId.ToString() + "/" + temp + extension);
                    if (finder == 0)
                    {

                        real_Estate.CadastreFileLink = fileModel.SystemPath;

                    }
                    if (finder == 1)
                    {

                        real_Estate.PhotoOfObjectLink1 = fileModel.SystemPath;

                    }
                    if (finder == 2)
                    {

                        real_Estate.PhotoOfObjectLink2 = fileModel.SystemPath;

                    }
                    if (finder == 3)
                    {

                        real_Estate.PhotoOfObjectLink3 = fileModel.SystemPath;

                    }

                    await _context.SaveChangesAsync(_userManager.GetUserId(User), real_Estate.RealEstateName);

                }

                else if (file != null)
                {
                    

                    if (finder == 0)
                    {
                        var createdFile = UploadFile(realEstateId, file);

                        real_Estate.CadastreFileId = createdFile.Result.FileId;
                        real_Estate.CadastreFileLink = createdFile.Result.SystemPath;

                    }
                    if (finder == 1)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                var image = ResizeImage(img, 370, 180);
                                var createdFile = UploadImage(realEstateId, image, file.FileName, file.ContentType);
                                real_Estate.PhotoOfObject1Id = createdFile.Result.FileId;
                                real_Estate.PhotoOfObjectLink1 = createdFile.Result.SystemPath;
                            }
                        }
                        
                    }
                    if (finder == 2)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                var image = ResizeImage(img, 370, 180);
                                var createdFile = UploadImage(realEstateId, image, file.FileName, file.ContentType);
                                real_Estate.PhotoOfObject2Id = createdFile.Result.FileId;
                                real_Estate.PhotoOfObjectLink2 = createdFile.Result.SystemPath;
                            }
                        }

                    }
                    if (finder == 3)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                var image = ResizeImage(img, 370, 180);
                                var createdFile = UploadImage(realEstateId, image, file.FileName, file.ContentType);
                                real_Estate.PhotoOfObject3Id = createdFile.Result.FileId;
                                real_Estate.PhotoOfObjectLink3 = createdFile.Result.SystemPath;
                            }
                        }

                    }
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), real_Estate.RealEstateName);

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }

            return RedirectToAction("Edit", new { id = realEstateId });

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
                    await _context.SaveChangesAsync(_userManager.GetUserId(User));
                }

            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex);
            }


            TempData["Message"] = $"Removed {file.Name + file.Extension} successfully from File System.";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(int id)
        {
            var districts = await _context.Districts.Where(d => d.RegionId == id).ToListAsync();
            var districtList = new SelectList(districts, "DistrictId", "DistrictName");

            return new JsonResult(districtList);
        }

        // GET: SimpleUser/RealEstates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var realEstate = await _context.RealEstates.FindAsync(id);
            if (realEstate == null)
            {
                return NotFound();
            }

            List<SelectListItem> empl = new();
            List<SelectListItem> elmp = new();
            List<Infrastucture> inf = new();
            List<SelectListItem> tech = new();
            List<SelectListItem> techn = new();
            List<TechnicalCharc> info = new();

            var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == id).ToList();

            foreach (var temp in realEstateInfrastructures)
            {
                var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();
                inf.Add(infrastructure);

                SelectListItem selectListItem = new()
                {
                    Text = infrastructure.InfrastructureName,
                    Value = infrastructure.InfrastructureId.ToString(),

                };

                SelectListItem sel = selectListItem;

                empl.Add(sel);

            }

            var infras = _context.Infrastuctures.ToList();
            var nInfras = infras.Except(inf);

            foreach (var temp in nInfras)
            {
                var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastructureId).FirstOrDefault();

                SelectListItem selectListItem2 = new()
                {
                    Text = infrastructure.InfrastructureName,
                    Value = infrastructure.InfrastructureId.ToString(),

                };

                SelectListItem sel2 = selectListItem2;

                elmp.Add(sel2);

            }

            //var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == id).ToList();

            //foreach (var temp in realEstateTechnicalCharcs)
            //{
            //    var technicalCharc = _context.TechnicalCharcs.Where(i => i.TechnicalCharcId == temp.TechnicalCharcId).FirstOrDefault();
            //    info.Add(technicalCharc);

            //    SelectListItem selectListItem = new()
            //    {
            //        Text = technicalCharc.TechnicalCharcName,
            //        Value = technicalCharc.TechnicalCharcId.ToString(),

            //    };

            //    SelectListItem item = selectListItem;

            //    tech.Add(item);

            //}

            //var Techs = _context.TechnicalCharcs.ToList();
            //var nTechs = Techs.Except(info);

            //foreach (var temp in nTechs)
            //{
            //    SelectListItem selectListItem2 = new()
            //    {
            //        Text = temp.TechnicalCharcName,
            //        Value = temp.TechnicalCharcId.ToString(),

            //    };

            //    SelectListItem item2 = selectListItem2;

            //    techn.Add(item2);

            //}

            ViewBag.Infrastructures = empl;
            ViewBag.nInfrastructures = elmp;
            //ViewBag.TechnicalCharcs = tech;
            //ViewBag.nTechnicalCharcs = techn;


            ViewData["DistrictId"] = new SelectList(_context.Districts.Where(d => d.RegionId == realEstate.RegionId), "DistrictId", "DistrictName", realEstate.DistrictId);

            ViewData["ProposalId"] = new SelectList(_context.Proposals, "ProposalId", "ProposalName", realEstate.ProposalId);

            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName", realEstate.RegionId);


            return View(realEstate);
        }

        // POST: SimpleUser/RealEstates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> Edit(int id, int RegionId,int DistrictId, List<int> Infrastructures, List<SelectListItem> empl, List<SelectListItem> elmp,
                                                int cadastreFileId, string cadastreFileLink, int photoId1, int photoId2, int photoId3, string photolink1, string photolink2, string photolink3,
                                                [Bind("RealEstateId,Confirmed,Status,OutOfAccountDate,ApplicationUserId,TransferredAssetId, RealEstateName,CadastreNumber,CadastreRegDate,CommisioningDate,Activity," +
                                                "RegionId,DistrictId,Address,AssetHolderName,FullArea,BuildingArea,NumberOfEmployee," +
                                                "MaintenanceCostForYear,InitialCostOfObject,Wear,ResidualValueOfObject,ProposalId, Comment," +
                                                "TransferredAssetOn,AssetEvaluationOn,SubmissionOnBiddingOn,ReductionInAssetOn,OneTimePaymentAssetOn,InstallmentAssetOn")] RealEstate realEstate)
        {
            if (id != realEstate.RealEstateId)
            {
                return NotFound();
            }

            var _realEstate = await _context.RealEstates.FindAsync(id);
            if(_realEstate == null)
            {
                return NotFound();
            }

            //realEstate.RegionId = regionId;

            //realEstate.CadastreFileLink = cadastreFileLink;
            //realEstate.CadastreFileId = cadastreFileId;
            //realEstate.PhotoOfObject1Id = photoId1;
            //realEstate.PhotoOfObject2Id = photoId2;
            //realEstate.PhotoOfObject3Id = photoId3;
            //realEstate.PhotoOfObjectLink1 = photolink1;
            //realEstate.PhotoOfObjectLink2 = photolink2;
            //realEstate.PhotoOfObjectLink3 = photolink3;
            //realEstate.ShareOfActivity = ShareOfActivity.ToString();
            //realEstate.ShareOfActivity = "12";

            if (ModelState.IsValid)
            {
                if(_realEstate.RealEstateName != realEstate.RealEstateName)
                    _realEstate.RealEstateName = realEstate.RealEstateName;
                if (_realEstate.CadastreRegDate != realEstate.CadastreRegDate)
                    _realEstate.CadastreRegDate = realEstate.CadastreRegDate;
                if (_realEstate.CadastreNumber != realEstate.CadastreNumber)
                    _realEstate.CadastreNumber = realEstate.CadastreNumber;
                if(_realEstate.CommisioningDate != realEstate.CommisioningDate)
                    _realEstate.CommisioningDate = realEstate.CommisioningDate;
                if(_realEstate.Activity != realEstate.Activity)
                    _realEstate.Activity = realEstate.Activity;
                if(_realEstate.AssetHolderName != realEstate.AssetHolderName)
                    _realEstate.AssetHolderName = realEstate.AssetHolderName;
                if(_realEstate.RegionId != RegionId)
                    _realEstate.RegionId = RegionId;
                if (_realEstate.DistrictId != DistrictId)
                    _realEstate.DistrictId = DistrictId;
                if(_realEstate.Address != realEstate.Address)
                    _realEstate.Address = realEstate.Address;
                if(_realEstate.BuildingArea != realEstate.BuildingArea)
                    _realEstate.BuildingArea = realEstate.BuildingArea;
                if (_realEstate.FullArea != realEstate.FullArea)
                    _realEstate.FullArea = realEstate.FullArea;
                if (_realEstate.NumberOfEmployee != realEstate.NumberOfEmployee)
                    _realEstate.NumberOfEmployee = realEstate.NumberOfEmployee;
                if (_realEstate.MaintenanceCostForYear != realEstate.MaintenanceCostForYear)
                    _realEstate.MaintenanceCostForYear = realEstate.MaintenanceCostForYear;
                if (_realEstate.InitialCostOfObject != realEstate.InitialCostOfObject)
                    _realEstate.InitialCostOfObject = realEstate.InitialCostOfObject;
                if (_realEstate.Wear != realEstate.Wear)
                    _realEstate.Wear = realEstate.Wear;
                if (_realEstate.ResidualValueOfObject != realEstate.ResidualValueOfObject)
                    _realEstate.ResidualValueOfObject = realEstate.ResidualValueOfObject;
                
                try
                {
                    //_context.Update(realEstate);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), _realEstate.RealEstateName);

                    var realInfras = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == id);
                    foreach (var rinf in realInfras)
                    {
                        _context.Remove(rinf);
                    }
                    foreach (var i in Infrastructures)
                    {
                        RealEstateInfrastructure rI = new()
                        {
                            RealEstateId = realEstate.RealEstateId,
                            InfrastuctureId = i
                        };
                        _context.RRealEstateInfrastructures.Add(rI);
                        await _context.SaveChangesAsync(_userManager.GetUserId(User));
                    }

                    //var realTechs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == id);
                    //foreach (var rTech in realTechs)
                    //{
                    //    _context.Remove(rTech);
                    //}

                    //foreach (var i in TechnicalCharcs)
                    //{
                    //    RealEstateTechnicalCharcs rT = new()
                    //    {
                    //        RealEstateId = realEstate.RealEstateId,
                    //        TechnicalCharcId = i
                    //    };
                    //    _context.RealEstateTechnicalCharcs.Add(rT);
                    //    await _context.SaveChangesAsync();
                    //}

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RealEstateExists(realEstate.RealEstateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "RealEstates", new { editSuccess = true });
            }

            ViewBag.Infrastructures = empl;
            ViewBag.nInfrastructures = elmp;
            //ViewBag.TechnicalCharcs = tech;
            //ViewBag.nTechnicalCharcs = techn;

            ViewData["DistrictId"] = new SelectList(_context.Districts, "DistrictId", "DistrictName", realEstate.DistrictId);

            ViewData["ProposalId"] = new SelectList(_context.Proposals, "ProposalId", "ProposalName", realEstate.ProposalId);

            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionName", realEstate.RegionId);


            return View(realEstate);
        }


        [HttpGet]
        public IActionResult GetUnSent()
        {

            var userId = _userManager.GetUserId(User);

            //var realEstateInfrastructures = _context.RealEstates.Where(r => r.ApplicationUserId == userId);

            var realEstates = _context.RealEstates.Include(r => r.ApplicationUser)
                .Include(r => r.AssetEvaluations).Include(r => r.DistrictOfObject)
                .Include(r => r.InstallmentAssets).Include(r => r.Proposal)
                .Include(r => r.ReductionInAssets).Include(r => r.RegionOfObject)
                .Include(r => r.SubmissionOnBiddings).Include(r => r.OneTimePaymentAssets)
                .Include(r => r.TransferredAsset)
                .Where(r => r.ApplicationUserId == userId && r.Confirmed == false).ToList();

            foreach (var item in realEstates)
            {
                item.CadasterRegDateStr = item.CadastreRegDate.ToShortDateString();
                item.CommisioningDateStr = item.CommisioningDate.ToShortDateString();

                item.InfrastructureNames = "";
               // item.TechnicalCharcNames = "";
                var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == item.RealEstateId).ToList();
               // var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == item.RealEstateId).ToList();

                foreach (var temp in realEstateInfrastructures)
                {
                    var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();

                    item.InfrastructureNames += infrastructure.InfrastructureName + "; ";

                }

                //foreach (var temp in realEstateTechnicalCharcs)
                //{
                //    var technicalCharc = _context.TechnicalCharcs.Where(t => t.TechnicalCharcId == temp.TechnicalCharcId).FirstOrDefault();

                //    item.TechnicalCharcNames += technicalCharc.TechnicalCharcName + "; ";

                //}

            }


            return Json(new { data = realEstates });
        }

        [HttpGet]
        public IActionResult GetSent()
        {

            var userId = _userManager.GetUserId(User);

            //var realEstateInfrastructures = _context.RealEstates.Where(r => r.ApplicationUserId == userId);

            var realEstates = _context.RealEstates.Include(r => r.ApplicationUser)
                .Include(r => r.AssetEvaluations).Include(r => r.DistrictOfObject)
                .Include(r => r.InstallmentAssets).Include(r => r.Proposal)
                .Include(r => r.ReductionInAssets).Include(r => r.RegionOfObject)
                .Include(r => r.SubmissionOnBiddings).Include(r => r.OneTimePaymentAssets)
                .Include(r => r.TransferredAsset)
                .Where(r => r.ApplicationUserId == userId && r.Confirmed == true).ToList();

            foreach (var item in realEstates)
            {
                item.CadasterRegDateStr = item.CadastreRegDate.ToShortDateString();
                item.CommisioningDateStr = item.CommisioningDate.ToShortDateString();

                item.InfrastructureNames = "";
               // item.TechnicalCharcNames = "";
                var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == item.RealEstateId).ToList();
               // var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == item.RealEstateId).ToList();

                foreach (var temp in realEstateInfrastructures)
                {
                    var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();

                    item.InfrastructureNames += infrastructure.InfrastructureName + "; ";

                }

            }


            return Json(new { data = realEstates });
        }


        [HttpPost]
        public async Task<IActionResult> Send ([FromBody] int? id){

            if(id == null)
            {
                return Json(new { success = false, message = "Объект топилмади!" });
            }

            var realEstate = await _context.RealEstates.FindAsync(id);
            if(realEstate != null)
            {
                realEstate.Confirmed = true;
                realEstate.TransferredAssetOn = true;
                realEstate.AssetEvaluationOn = true;
                realEstate.InstallmentAssetOn = true;
            }
            

            try
            {
                await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            return Json(new { success = true, message = "Маълумотлар тасдиқланди!" });
        }

       
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if(id == 0)
            {
                return Json(new { success = false, message = "Ўчиришда хатолик юз берди!" });
            }

            var realEstate = await _context.RealEstates.FindAsync(id);
            var realEstateName = realEstate.RealEstateName;
            if (realEstate == null)
            {
                return Json(new { success = false, message = "Ўчиришда хатолик юз берди!" });
            }

            var fileModels = _context.FileModels.Where(f => f.RealEstateId == id);
            var transferredAsset = _context.TransferredAssets.Where(t => t.AssetId == t.RealEstate.TransferredAssetId).ToList();
            var assetEvaluations = _context.AssetEvaluations.Where(a => a.RealEstateId == id).ToList();
            var submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.RealEstateId == id).ToList();
            var reductionInAssets = _context.ReductionInAssets.Where(s => s.RealEstateId == id).ToList();
            var oneTimePaymentAssets = _context.OneTimePaymentAssets.Where(s => s.RealEstateId == id).ToList();
            var installmentAssets = _context.InstallmentAssets.Where(s => s.RealEstateId == id).ToList();


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
                await _context.SaveChangesAsync(_userManager.GetUserId(User),realEstate.RealEstateName);
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
                            //await _context.SaveChangesAsync();
                        }

                    }
                    try
                    {

                        _context.AssetEvaluations.Remove(item);
                        await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        int a;
                        a = 6;
                    }
                }

            }

            if (submissionOnBiddings.Any())
            {
                foreach (var item in submissionOnBiddings)
                {
                    _context.SubmissionOnBiddings.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
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
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
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
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
                }


            }

            if (installmentAssets.Any())
            {
                foreach (var item in installmentAssets)
                {

                    var installmentFile = _context.FileModels.Where(f => f.InstallmentAssetId == item.InstallmentAssetId);

                    if (installmentFile.Any())
                    {
                        foreach (var item2 in installmentFile)
                        {
                            if (System.IO.File.Exists(item2.FilePath))
                            {
                                System.IO.File.Delete(item2.FilePath);
                            }
                            _context.FileModels.Remove(item2);
                        }
                    }

                    _context.InstallmentAssets.Remove(item);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
                }

            }
            try
            {
                _context.RealEstates.Remove(realEstate);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);
            }
            catch (Exception ex)
            {
                var e1x = ex;
            }

            if (transferredAsset.Any())
            {
                foreach (var item in transferredAsset)
                {
                    var transferredAssetFiles = _context.FileModels.Where(f => f.TransferredAssetId == item.AssetId);

                    if (transferredAssetFiles.Any())
                    {
                        foreach (var item2 in transferredAssetFiles)
                        {
                            if (System.IO.File.Exists(item2.FilePath))
                            {
                                System.IO.File.Delete(item2.FilePath);
                            }
                            _context.FileModels.Remove(item2);
                            //await _context.SaveChangesAsync();
                        }

                    }
                    try
                    {

                        _context.TransferredAssets.Remove(item);
                        await _context.SaveChangesAsync(_userManager.GetUserId(User),realEstateName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }


            return Json(new { success = true, message = "Маълумотлар бутунлай ўчирилди" });

        }

        public IActionResult GetPassport(int id)
        {
            var realEstate =_context.RealEstates.Include(r => r.RegionOfObject)
                                .Include(r => r.DistrictOfObject)
                                .Include(r => r.Proposal)
                                .FirstOrDefault(r => r.RealEstateId == id);
           // realEstate.TechnicalCharcNames = "";
            realEstate.InfrastructureNames = "";
            
           //var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == id).ToList();
           var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == id).ToList();

            //foreach (var temp in realEstateTechnicalCharcs)
            //{
            //    var technicalCharc = _context.TechnicalCharcs.Where(t => t.TechnicalCharcId == temp.TechnicalCharcId).FirstOrDefault();

            //    realEstate.TechnicalCharcNames += technicalCharc.TechnicalCharcName + "; ";

            //}

            List<List<string>> infrastructures = new();
            List<Infrastucture> infra = new();

            foreach (var temp in realEstateInfrastructures)
            {
                var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();
                infra.Add(infrastructure);
                List<string> infras = new();
                infras.Add(infrastructure.InfrastructureName);
                infras.Add("Mavjud");
                infrastructures.Add(infras);

            }
            var infr = _context.Infrastuctures.ToList();
            var nInfras = infr.Except(infra);

            foreach (var temp in nInfras)
            {
                
                List<string> infras = new();
                infras.Add(temp.InfrastructureName);
                infras.Add("Mavjud emas");
                infrastructures.Add(infras);

            }

            ViewBag.Infrastructures = infrastructures;

            return View(realEstate);
        }

            private bool RealEstateExists(int id)
        {
            return _context.RealEstates.Any(e => e.RealEstateId == id);
        }
    }
}


   