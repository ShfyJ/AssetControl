using ControlActive.Constants;
using ControlActive.Data;
using ControlActive.Models;
using ControlActive.ViewModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        // GET: UsersController
        public IActionResult Dashboard(DateTime? chosenDate = null)
        {
            if (!chosenDate.HasValue)
            {
                chosenDate = DateTime.Today;
            }

            var realEstates = _db.RealEstates.Include(r => r.ApplicationUser).Where(r => r.Confirmed == true).ToList();
            var chosenRealEstates = new List<RealEstate>();

            var shares = _db.Shares.Include(s => s.ApplicationUser).Where(s => s.Confirmed == true).ToList();
            var chosenShares = new List<Share>();

            var tranfAssets = _db.TransferredAssets.Where(t => t.Confirmed == true).Include(s => s.RealEstate).Include(s => s.Share).ToList();
            var chosenTransfAssets = new List<TransferredAsset>();

            var assetEvaluations = _db.AssetEvaluations.Where(a => a.Confirmed == true).Include(s => s.RealEstate).Include(s => s.Share).ToList();
            var chosenAssetEvals = new List<AssetEvaluation>();

            var submissionOnBiddings = _db.SubmissionOnBiddings.Where(s => s.Confirmed == true).Include(s => s.RealEstate).Include(s => s.Share).ToList();
            var chosenSubmOnBs = new List<SubmissionOnBidding>();

            var reductionInAssets = _db.ReductionInAssets.Where(r => r.Confirmed == true).Include(s => s.RealEstate).Include(s => s.Share).ToList();
            var chosenReductionInAssets = new List<ReductionInAsset>();

            var oneTimePaymentAssets = _db.OneTimePaymentAssets.Where(o => o.Confirmed == true).Include(s => s.Step2).Include(s => s.Step3).Include(s => s.RealEstate).Include(s => s.Share).ToList();
            var chosenOneTimePaymentAssets = new List<OneTimePaymentAsset>();

            var oneTimePaymentStep2s = _db.OneTimePaymentStep2.Where(o => o.Confirmed == true).ToList();
            var chosenOneTimePaymentStep2s = new List<OneTimePaymentStep2>();

            var oneTimePaymentStep3s = _db.OneTimePaymentStep3.Where(o => o.Confirmed == true).ToList();
            var chosenOneTimePaymentStep3s = new List<OneTimePaymentStep3>();

            var installmentAssets = _db.InstallmentAssets.Where(i => i.Confirmed == true).Include(s => s.Step2).Include(s => s.RealEstate).Include(s => s.Share).ToList();
            var chosenInstallmentAssets = new List<InstallmentAsset>();

            var installmentStep2s = _db.InstallmentStep2.Where(i => i.Confirmed == true).ToList();
            var chosenInstallmentStep2s = new List<InstallmentStep2>();


            foreach (var item in realEstates)
            {
                if (item.OutOfAccountDate > chosenDate)
                {
                    chosenRealEstates.Add(item);
                }
            }

            foreach (var item in shares)
            {
                if (item.OutOfAccountDate > chosenDate)
                {
                    chosenShares.Add(item);
                }
            }

            foreach (var item in tranfAssets)
            {
                if (item.ActAndAssetDate <= chosenDate)
                {
                    chosenTransfAssets.Add(item);
                }
            }

            foreach (var item in assetEvaluations)
            {
                if (item.ReportDate <= chosenDate && item.StatusChangedDate > chosenDate)
                {
                    chosenAssetEvals.Add(item);
                }
            }

            foreach (var item in submissionOnBiddings)
            {
                if (item.BiddingExposureDate <= chosenDate && item.AuctionCancelledDate > chosenDate)
                {
                    chosenSubmOnBs.Add(item);
                }
            }

            foreach (var item in reductionInAssets)
            {
                if (item.SolutionDate <= chosenDate && item.StatusChangedDate > chosenDate)
                {
                    chosenReductionInAssets.Add(item);
                }
            }

            foreach (var item in oneTimePaymentAssets)
            {
                if (item.SolutionDate <= chosenDate && item.BiddingCancelledDate > chosenDate)
                {
                    chosenOneTimePaymentAssets.Add(item);
                }
            }

            foreach (var item in oneTimePaymentStep2s)
            {
                if (item.AggreementDate <= chosenDate && item.ContractCancelledDate > chosenDate)
                {
                    chosenOneTimePaymentStep2s.Add(item);
                }
            }

            foreach (var item in oneTimePaymentStep3s)
            {
                if (item.ActAndAssetDate <= chosenDate)
                {
                    chosenOneTimePaymentStep3s.Add(item);
                }
            }

            foreach (var item in installmentAssets)
            {
                if (item.AggreementDate <= chosenDate && item.ContractCancelledDate > chosenDate)
                {
                    chosenInstallmentAssets.Add(item);
                }
            }

            foreach (var item in installmentStep2s)
            {
                if (item.ActAndAssetDate <= chosenDate)
                {
                    chosenInstallmentStep2s.Add(item);
                }
            }

            ViewData["RealEstates"] = chosenRealEstates.Count;
            ViewData["Shares"] = chosenShares.Count;

            ViewData["TransfAssets"] = chosenTransfAssets.Count;
            ViewData["AssetEval"] = chosenAssetEvals.Count;

            ViewData["SubmissionOnB"] = chosenSubmOnBs.Count;
            ViewData["ReductionIn"] = chosenReductionInAssets.Count;
            ViewData["OneTimeP"] = chosenOneTimePaymentStep3s.Count;
            ViewData["Installment"] = chosenInstallmentStep2s.Count;

            //number of # for realEstate
            ViewBag.rTransfAssets = chosenTransfAssets.FindAll(t => t.RealEstate != null).Count;
            ViewBag.rAssetEval = chosenAssetEvals.FindAll(t => t.RealEstate != null).Count;
            ViewBag.rSubmissionOnB = chosenSubmOnBs.FindAll(t => t.RealEstate != null).Count;
            ViewBag.rReductionIn = chosenReductionInAssets.FindAll(t => t.RealEstate != null).Count;
            ViewBag.rOneTimeP = chosenOneTimePaymentAssets.FindAll(t => t.Step3 != null && t.RealEstate != null).Count;
            ViewBag.rInstallment = chosenInstallmentAssets.FindAll(t => t.Step2 != null && t.RealEstate != null).Count;

            //number of # for share
            ViewBag.sTransfAssets = chosenTransfAssets.FindAll(t => t.Share != null).Count;
            ViewBag.sAssetEval = chosenAssetEvals.FindAll(t => t.Share != null).Count;
            ViewBag.sSubmissionOnB = chosenSubmOnBs.FindAll(t => t.Share != null).Count;
            ViewBag.sReductionIn = chosenReductionInAssets.FindAll(t => t.Share != null).Count;
            ViewBag.sOneTimeP = chosenOneTimePaymentAssets.FindAll(t => t.Step3 != null && t.Share != null).Count;
            ViewBag.sInstallment = chosenInstallmentAssets.FindAll(t => t.Step2 != null && t.Share != null).Count;

            ViewBag.Orgs = _db.Organizations.Count();
            ViewBag.Users = _db.Users.Count();

            var regions = _db.Regions.ToList();
            AssetsByRegionViewmodel[] mapData = new AssetsByRegionViewmodel[regions.Count - 1];

            string[] colors = { "#87ada5", "#8aabb0", "#87a3ad", "#8aad87", "#868dad", "#9e89b3", "#b1bd8f", "#FFB300", "#039BE5", "#A1887F", "#BA68C8", "#E57373" };

            int m = 0;
            int c = 0;
            foreach (var region in regions)
            {
                AssetsByRegionViewmodel map = new();
                if (region.AmchartMapId == null)
                    continue;
                map.id = region.AmchartMapId;
                map.name = region.RegionName;
                map.value = chosenRealEstates.Where(r => r.RegionId == region.RegionId).Count() + chosenShares.Where(r => r.RegionId == region.RegionId).Count();
                map.color = colors[c];

                mapData[m] = map;
                m++; c++;
                if (colors.Length == c)
                    c = 0;
            }

            string mapJson = JsonConvert.SerializeObject(mapData, Formatting.Indented);
            ViewBag.MapData = mapJson;

            var organizations = _db.Organizations.ToArray();

            ViewBag.ChosenDate = chosenDate.Value.ToString("dd.MM.yyyy");


            OrganizationViewModel[]data = new OrganizationViewModel[organizations.Length];
            
            //OrganizationViewModel.Children children = new();
            
           
            string[] assetTypes = { "Кўчмас мулк объектлари", "Улушлaр" };
            string[] types = { "RealEstates", "Shares" };
            int[] assetCount = new int[2];

            for(int i = 0; i < organizations.Length; i++)
            {
                OrganizationViewModel organizationViewModel = new();
                organizationViewModel.children = new OrganizationViewModel.Children[2];
                organizationViewModel.name = organizations[i].OrganizationName;
               

                var rs = chosenRealEstates.Where(c => c.AssetHolderName.Equals(organizations[i].OrganizationName)).ToArray();
                var orgRealEstates = rs.Length;
                var sh = chosenShares.Where(c => c.ParentOrganization.Equals(organizations[i].OrganizationName)).ToArray();
                var orgShares = sh.Length;

                //object[][] arrayOfObjects = new object[2][];

                assetCount[0] = orgRealEstates;
                assetCount[1] = orgShares;

                for(int j = 0; j<2; j++)
                {
                    OrganizationViewModel.Children children = new ();
                    children.children = new ();  
                    
                    children.name = assetTypes[j];
                    children.type = types[j];   

                    for(int l=0; l<assetCount[j]; l++)
                    {
                        OrganizationViewModel.SmallChildren smallChildren = new();
                        if (j == 0)
                        {
                            smallChildren.name = rs[l].RealEstateName;
                        }

                        if (j == 1)
                        {
                            smallChildren.name = sh[l].BusinessEntityName;
                        }
                      children.children.Add(smallChildren);
                    }
                    
                    organizationViewModel.children[j] = children;
                }

                
                data[i] = organizationViewModel;
                
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            ViewData["Data"] = json;
            

            return View();
        }
  

        public IActionResult Index()
        {
            
            return View();
        }

        #region API CALLS

        [HttpPost]
        public async Task<IActionResult> GivePermissionToEdit([FromBody]JObject data)
        {
            if (data["id"] == null || data["type"] == null || data["notiId"] == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            int id = (int)data["id"];
            int type =(int)data["type"];
            int notiId = (int)data["notiId"];

            RealEstate realEstate = new();
            Share share = new();
            TransferredAsset transferredAsset = new TransferredAsset();
            AssetEvaluation assetEvaluation = new AssetEvaluation();
            SubmissionOnBidding submissionOnBidding = new SubmissionOnBidding();
            ReductionInAsset reductionInAsset = new();
            OneTimePaymentAsset oneTimePaymentAsset = new();
            OneTimePaymentStep2 oneTimePaymentStep2 = new();
            OneTimePaymentStep3 oneTimePaymentStep3 = new();
            InstallmentAsset installmentAsset = new();
            InstallmentStep2 installmentStep2 = new();
            Noti noti = new();

            if(type == 1)
            {
                realEstate = await _db.RealEstates.FindAsync(id);
                if(realEstate!=null)
                    realEstate.Confirmed = false;
                else
                {
                    Json(new { success = false, message = "Объект топилмади!" });
                }

                noti = await _db.Notifications.FindAsync(notiId);
                if (noti != null)
                    noti.isPermitted = true;
                

                await _db.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return Json(new { success = true, message = "Рухсат берилди" });

            }

            if (type == 2)
            {
                share = await _db.Shares.FindAsync(id);
                if (share != null)
                    share.Confirmed = false;
                else
                {
                    Json(new { success = false, message = "Объект топилмади!" });
                }

                noti = await _db.Notifications.FindAsync(notiId);
                if (noti != null)
                    noti.isPermitted = true;


                await _db.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return Json(new { success = true, message = "Рухсат берилди" });

            }


            return Json(new { success = false, message = "Рухсат берилмади" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {

            var userList = _db.ApplicationUsers.Include(u => u.Organization).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                string roleId;
                string roleName;
                var userRole = userRoles.FirstOrDefault(u => u.UserId == user.Id); 
                
                if(userRole != null)
                {
                    roleId = userRole.RoleId;
                    
                }
                else
                {
                    continue;
                }

                var role = roles.FirstOrDefault(u => u.Id == roleId);

                if(role != null)
                {
                    user.Role = role.Name;
                }
                
                
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserForAsset([FromBody] JObject data)
        {
            if (data["userId"] == null || data["assetId"] == null || data["target"] == null)
            {
                return Json(new { success = false, message = "Маълумотни юборишда хатолик - қайтадан уриниб кўринг!" });
            }
           
            string userId = data["userId"].ToString();
            string target = data["target"].ToString();
            int assetId = (int)data["assetId"];
           
            string assetName = "";

            var user = await _db.ApplicationUsers.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Танланган фойдаланувчи топилмади!" });
            }

            if (target.Equals("1"))
            {
                var realEstate = await _db.RealEstates.FindAsync(assetId);
                if (realEstate == null)
                {
                    return Json(new { success = false, message = "Объект топилмади!" });
                }
                assetName = realEstate.RealEstateName;
                realEstate.ApplicationUserId = userId;
                await _db.SaveChangesAsync(realEstate.RealEstateName, _userManager.GetUserId(User));

            }

            if (target.Equals("2"))
            {
                var share = await _db.Shares.FindAsync(assetId);
                if (share == null)
                {
                    return Json(new { success = false, message = "Актив топилмади!" });
                }
                assetName = share.BusinessEntityName;
                share.ApplicationUserId = userId;
                await _db.SaveChangesAsync(share.BusinessEntityName, _userManager.GetUserId(User));

            }

            return Json(new { success = true, message = $"{assetName} {user.Fullname} га муваффақиятли бириктирилди!" });
        }

        [HttpPost]
        public IActionResult ResetPassword([FromBody] string id)
        {
            //password is 'User@123'

            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }
            objFromDb.PasswordHash = "AQAAAAEAACcQAAAAEHtBeJMEQH3a//8IvwafBNPbnHHKuI6J9IfyA7IkRgXxN76SEdw/O4b/ajvr210B1A==";
            objFromDb.SecurityStamp = "PK6MBGF33TLWJXBWD6HZGHYGT5JQU644";
            objFromDb.ConcurrencyStamp = "a6504a40-b039-473c-9247-ff60b250a56a";
            _db.SaveChanges();
            return Json(new { success = true, message = objFromDb.Email + " нинг пароли муваффақиятли User@123 га ўзгартирилди!" });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.Users.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Муваффақиятли бажарилди!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string id)
        {
            if(id == null)
            {
                return Json(new { success = false, message = "Хатолик- қайтадан ўриниб кўринг!" });
            }

            var user = await _userManager.FindByIdAsync(id);
            
            if(user == null)
            {
                return Json(new { success = false, message = "Фойдаланувчи топилмади!" });
            }

            await _userManager.DeleteAsync(user);

            return Json(new { success = true, message = $"{user.UserName} ўчирилди!" });

        }
            
        //[HttpDelete]
        //public IActionResult Delete(string id)
        //{
        //    var objFromDb = _unitOfWork.User.GetId(id);

        //    var userRole = _db.UserRoles.ToList();
        //    var roles = _db.Roles.ToList();
        //    var roleId = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id).RoleId;
        //    objFromDb.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
        //    var requestMakers = _db.RequestMakers.ToList();
        //    var requestMakerId = requestMakers.FirstOrDefault(u => u.UserId == objFromDb.Id).RequestmakerId;
        //    if (objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting" });
        //    }
        //    if (objFromDb.Role == SD.Role_Requester)
        //    {
        //        var requests = _db.Request.ToList();
        //        var requestList = requests.Include(u => u.RequestmakerId == requestMakerId).

        //        var requestObjFromDb_ = _unitOfWork.Requester.Get(requestId);
        //        _unitOfWork.Requester.Remove(requestObjFromDb_);

        //        var objFromDb_ = _unitOfWork.Requester.Get(requestMakerId);
        //        _unitOfWork.Requester.Remove(objFromDb_);
        //    }


        //    _unitOfWork.User.Remove(objFromDb);          

        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successful" });

        //}
        #endregion
    }
}
