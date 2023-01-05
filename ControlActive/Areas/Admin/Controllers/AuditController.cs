using ControlActive.Constants;
using ControlActive.Data;
using ControlActive.Models;
using ControlActive.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public AuditController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var AuditLogs = await _context.AuditLogs.Where(a => !a.TableName.Equals("RealEstateInfrastructure") && !a.TableName.Equals("Notification")
                                    && !a.TableName.Equals("SharesAndHolders") && a.UserId != null).OrderByDescending(a => a.AuditId).ToListAsync();
            List<AuditViewModel> auditViewModelList = new();
            foreach(var log in AuditLogs)
            {
                var user = _context.ApplicationUsers.FindAsync(log.UserId);
                string userFullName = "";
                if(user.Result != null)
                {
                  userFullName = user.Result.Fullname;
                }
                var time = log.DateTime.ToShortTimeString();
                var dateTime = time + ", " + log.DateTime.ToLongDateString();
                AuditViewModel audit = new()
                {
                    ActionName = log.Type,
                    UserFullName = userFullName,
                    TableName = log.TableName,
                    DateTime = dateTime,
                    OldValue = log.OldValues,
                    NewValue = log.NewValues,
                    AffectedColumn = log.AffectedColumns,
                    PrimaryKey = log.PrimaryKey
                };
                auditViewModelList.Add(audit);
            }
            return Json(new { data = auditViewModelList });
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetLogs()
        {
            var AuditLogs = await _context.AuditLogs.Where(a => (a.TableName.Equals("RealEstate") || a.TableName.Equals("Share")
                                    || a.TableName.Equals("TransferredAsset") || a.TableName.Equals("AssetEvaluation")
                                    || a.TableName.Equals("SubmissionOnBidding") || a.TableName.Equals("ReductionInAsset")
                                    || a.TableName.Equals("OneTimePaymentAsset") || a.TableName.Equals("InstallmentAsset")) && a.UserId!=null).OrderByDescending(a => a.AuditId).ToListAsync();
            List<AuditViewModel> auditViewModelList = new();

            try
            {
                foreach (var log in AuditLogs)
                {
                    var user = _context.ApplicationUsers.FindAsync(log.UserId);
                    string userFullName = "";
                    //string assetName = "";
                    if (user.Result != null)
                    {
                        userFullName = user.Result.Fullname;
                    }
                    var time = log.DateTime.ToShortTimeString();
                    var dateTime = time + ", " + log.DateTime.ToLongDateString();

                    #region comment
                    //var primaryKeyDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(log.PrimaryKey);

                    //Dictionary<string, object> NewValues = new();
                    //if (log.NewValues != null)
                    //    NewValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(log.NewValues);

                    //Dictionary<string, object> AssetValues = new();
                    //if (log.AssetValues != null)
                    //    AssetValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(log.AssetValues);
                    //switch (log.TableName)
                    //{
                    //    case "RealEstate":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }

                    //        if (primaryKeyDict["RealEstateId"] <= 0)
                    //        {
                    //            assetName = NewValues["RealEstateName"].ToString();
                    //            break;
                    //        }

                    //        var realEstate = await _context.RealEstates.FirstAsync(r => r.RealEstateId == primaryKeyDict["RealEstateId"]);
                    //        if (realEstate != null)
                    //            assetName = realEstate.RealEstateName;
                    //        break;
                    //    case "Share":
                    //        if (primaryKeyDict["ShareId"] <= 0)
                    //        {
                    //            assetName = NewValues["BusinessEntityName"].ToString();
                    //            break;
                    //        }
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        var share = await _context.Shares.FirstAsync(s => s.ShareId == primaryKeyDict["ShareId"]);
                    //        if (share != null)
                    //            assetName = share.BusinessEntityName;
                    //        break;
                    //    case "TransferredAsset":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["AssetId"] <= 0)
                    //        {                              
                    //            if (NewValues["RealEstateId"] != null)
                    //            {
                    //                var r = await _context.RealEstates.FindAsync((int)NewValues["RealEstateId"]);
                    //                if (r != null)
                    //                    assetName = r.RealEstateName;

                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;

                    //            }

                    //            break;
                    //        }
                    //        var transferredAsset = await _context.TransferredAssets.Include(t => t.RealEstate).Include(t => t.Share).FirstAsync(t => t.AssetId == primaryKeyDict["AssetId"]);
                    //        if (transferredAsset != null)
                    //            if (transferredAsset.RealEstate != null)
                    //                assetName = transferredAsset.RealEstate.RealEstateName;
                    //            else if (transferredAsset.Share != null)
                    //                assetName = transferredAsset.Share.BusinessEntityName;
                    //        break;

                    //    case "AssetEvaluation":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["AssetEvaluationId"] <= 0)
                    //        {

                    //            if (NewValues["RealEstateId"] != null)
                    //            {
                    //                var r = await _context.RealEstates.FindAsync((int)NewValues["RealEstateId"]);
                    //                if (r != null)
                    //                    assetName = r.RealEstateName;

                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;

                    //            }

                    //            break;
                    //        }
                    //        var assetEvaluation = await _context.AssetEvaluations.Include(t => t.RealEstate).Include(t => t.Share).FirstAsync(a => a.AssetEvaluationId == primaryKeyDict["AssetEvaluationId"]);
                    //        if (assetEvaluation != null)
                    //            if (assetEvaluation.RealEstate != null)
                    //                assetName = assetEvaluation.RealEstate.RealEstateName;
                    //            else if (assetEvaluation.Share != null)
                    //                assetName = assetEvaluation.Share.BusinessEntityName;
                    //        break;
                    //    case "SubmissionOnBidding":

                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["SubmissionOnBiddingId"] <= 0)
                    //        {

                    //            if (NewValues["RealEstateId"] != null)
                    //            {
                    //                var r = await _context.RealEstates.FindAsync((int)NewValues["RealEstateId"]);
                    //                if (r != null)
                    //                    assetName = r.RealEstateName;
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var submissionOnBidding = await _context.SubmissionOnBiddings.Include(t => t.RealEstate).Include(t => t.Share).FirstAsync(a => a.SubmissionOnBiddingId == primaryKeyDict["SubmissionOnBiddingId"]);
                    //        if (submissionOnBidding != null)
                    //            if (submissionOnBidding.RealEstate != null)
                    //                assetName = submissionOnBidding.RealEstate.RealEstateName;
                    //            else if (submissionOnBidding.Share != null)
                    //                assetName = submissionOnBidding.Share.BusinessEntityName;
                    //        break;
                    //    case "ReductionInAsset":

                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }

                    //        if (primaryKeyDict["ReductionInAssetId"] <= 0)
                    //        {

                    //            if (NewValues["RealEstateId"] != null)
                    //            {
                    //                var r = await _context.RealEstates.FindAsync((int)NewValues["RealEstateId"]);
                    //                if (r != null)
                    //                    assetName = r.RealEstateName;
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var reductionInAsset = await _context.ReductionInAssets.Include(t => t.RealEstate).Include(t => t.Share).FirstAsync(a => a.ReductionInAssetId == primaryKeyDict["ReductionInAssetId"]);
                    //        if (reductionInAsset != null)
                    //            if (reductionInAsset.RealEstate != null)
                    //                assetName = reductionInAsset.RealEstate.RealEstateName;
                    //            else if (reductionInAsset.Share != null)
                    //                assetName = reductionInAsset.Share.BusinessEntityName;
                    //        break;
                    //    case "OneTimePaymentAsset":

                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["OneTimePaymentAssetId"] <= 0)
                    //        {
                    //            if (NewValues["RealEstateId"] != null)
                    //            {
                    //                var r = await _context.RealEstates.FindAsync((int)NewValues["RealEstateId"]);
                    //                if (r != null)
                    //                    assetName = r.RealEstateName;
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var oneTimePaymentAsset = await _context.OneTimePaymentAssets.Include(t => t.RealEstate).Include(t => t.Share).FirstAsync(a => a.OneTimePaymentAssetId == primaryKeyDict["OneTimePaymentAssetId"]);
                    //        if (oneTimePaymentAsset != null)
                    //            if (oneTimePaymentAsset.RealEstate != null)
                    //                assetName = oneTimePaymentAsset.RealEstate.RealEstateName;
                    //            else if (oneTimePaymentAsset.Share != null)
                    //                assetName = oneTimePaymentAsset.Share.BusinessEntityName;
                    //        break;
                    //    case "OneTimePaymentStep2":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["OneTimePaymentStep2Id"] <= 0)
                    //        {                              
                    //            if (NewValues["OneTimePaymentAssetId"] != null)
                    //            {
                    //                var a = await _context.OneTimePaymentAssets.FindAsync((int)NewValues["OneTimePaymentAssetId"]);
                    //                if (a != null)
                    //                    if (a.RealEstateId != null)
                    //                    {
                    //                        var r = await _context.RealEstates.FindAsync(a.RealEstateId);
                    //                        if (r != null)
                    //                            assetName = r.RealEstateName;
                    //                    }
                    //                    else if (a.ShareId != null)
                    //                    {
                    //                        var s = await _context.Shares.FindAsync(a.ShareId);
                    //                        if (s != null)
                    //                            assetName = s.BusinessEntityName;
                    //                    }
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var oneTimePaymentStep2 = await _context.OneTimePaymentStep2.Include(t => t.OneTimePaymentAsset).FirstAsync(a => a.OneTimePaymentStep2Id == primaryKeyDict["OneTimePaymentStep2Id"]);
                    //        if (oneTimePaymentStep2 != null)
                    //            if (oneTimePaymentStep2.OneTimePaymentAsset != null)
                    //            {
                    //                if (oneTimePaymentStep2.OneTimePaymentAsset.RealEstateId != null)
                    //                {
                    //                    var r = await _context.RealEstates.FindAsync(oneTimePaymentStep2.OneTimePaymentAsset.RealEstateId);
                    //                    if (r != null)
                    //                        assetName = r.RealEstateName;
                    //                }
                    //                else if (oneTimePaymentStep2.OneTimePaymentAsset.ShareId != null)
                    //                {
                    //                    var s = await _context.Shares.FindAsync(oneTimePaymentStep2.OneTimePaymentAsset.ShareId);
                    //                    if (s != null)
                    //                        assetName = s.BusinessEntityName;
                    //                }
                    //            }

                    //        break;
                    //    case "OneTimePaymentStep3":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["OneTimePaymentStep3Id"] <= 0)
                    //        {
                    //            if (NewValues["OneTimePaymentAssetId"] != null)
                    //            {
                    //                var a = await _context.OneTimePaymentAssets.FindAsync((int)NewValues["OneTimePaymentAssetId"]);
                    //                if (a != null)
                    //                    if (a.RealEstateId != null)
                    //                    {
                    //                        var r = await _context.RealEstates.FindAsync(a.RealEstateId);
                    //                        if (r != null)
                    //                            assetName = r.RealEstateName;
                    //                    }
                    //                    else if (a.ShareId != null)
                    //                    {
                    //                        var s = await _context.Shares.FindAsync(a.ShareId);
                    //                        if (s != null)
                    //                            assetName = s.BusinessEntityName;
                    //                    }
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var oneTimePaymentStep3 = await _context.OneTimePaymentStep3.Include(t => t.OneTimePaymentAsset).FirstAsync(a => a.OneTimePaymentStep3Id == primaryKeyDict["OneTimePaymentStep3Id"]);
                    //        if (oneTimePaymentStep3 != null)
                    //            if (oneTimePaymentStep3.OneTimePaymentAsset != null)
                    //            {
                    //                if (oneTimePaymentStep3.OneTimePaymentAsset.RealEstateId != null)
                    //                {
                    //                    var r = await _context.RealEstates.FindAsync(oneTimePaymentStep3.OneTimePaymentAsset.RealEstateId);
                    //                    if (r != null)
                    //                        assetName = r.RealEstateName;
                    //                }
                    //                else if (oneTimePaymentStep3.OneTimePaymentAsset.ShareId != null)
                    //                {
                    //                    var s = await _context.Shares.FindAsync(oneTimePaymentStep3.OneTimePaymentAsset.ShareId);
                    //                    if (s != null)
                    //                        assetName = s.BusinessEntityName;
                    //                }
                    //            }

                    //        break;
                    //    case "InstallmentAsset":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }
                    //        if (primaryKeyDict["InstallmentAssetId"] <= 0)
                    //        {
                    //            if (NewValues["RealEstateId"] != null)
                    //            {
                    //                var r = await _context.RealEstates.FindAsync((int)NewValues["RealEstateId"]);
                    //                if (r != null)
                    //                    assetName = r.RealEstateName;
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var installmentAsset = await _context.InstallmentAssets.Include(t => t.RealEstate).Include(t => t.Share).FirstAsync(a => a.InstallmentAssetId == primaryKeyDict["InstallmentAssetId"]);
                    //        if (installmentAsset != null)
                    //            if (installmentAsset.RealEstate != null)
                    //                assetName = installmentAsset.RealEstate.RealEstateName;
                    //            else if (installmentAsset.Share != null)
                    //                assetName = installmentAsset.Share.BusinessEntityName;
                    //        break;
                    //    case "InstallmentStep2":
                    //        if (log.Type.Equals("Delete"))
                    //        {
                    //            assetName = log.AssetName;
                    //            break;
                    //        }

                    //        if (primaryKeyDict["InstallmentStep2Id"] <= 0)
                    //        {
                    //            if (NewValues["InstallmentAssetId"] != null)
                    //            {
                    //                var a = await _context.InstallmentAssets.FindAsync((int)NewValues["InstallmentAssetId"]);
                    //                if (a != null)
                    //                    if (a.RealEstateId != null)
                    //                    {
                    //                        var r = await _context.RealEstates.FindAsync(a.RealEstateId);
                    //                        if (r != null)
                    //                            assetName = r.RealEstateName;
                    //                    }
                    //                    else if (a.ShareId != null)
                    //                    {
                    //                        var s = await _context.Shares.FindAsync(a.ShareId);
                    //                        if (s != null)
                    //                            assetName = s.BusinessEntityName;
                    //                    }
                    //            }
                    //            else if (NewValues["ShareId"] != null)
                    //            {
                    //                var s = await _context.Shares.FindAsync((int)NewValues["ShareId"]);
                    //                if (s != null)
                    //                    assetName = s.BusinessEntityName;
                    //            }

                    //            break;
                    //        }
                    //        var installmentStep2 = await _context.InstallmentStep2.Include(t => t.InstallmentAsset).FirstAsync(a => a.InstallmentStep2Id == primaryKeyDict["InstallmentStep2Id"]);
                    //        if (installmentStep2 != null)
                    //            if (installmentStep2.InstallmentAsset != null)
                    //            {
                    //                if (installmentStep2.InstallmentAsset.RealEstateId != null)
                    //                {
                    //                    var r = await _context.RealEstates.FindAsync(installmentStep2.InstallmentAsset.RealEstateId);
                    //                    if (r != null)
                    //                        assetName = r.RealEstateName;
                    //                }
                    //                else if (installmentStep2.InstallmentAsset.ShareId != null)
                    //                {
                    //                    var s = await _context.Shares.FindAsync(installmentStep2.InstallmentAsset.ShareId);
                    //                    if (s != null)
                    //                        assetName = s.BusinessEntityName;
                    //                }
                    //            }

                    //        break;
                    //}
                    #endregion comment finishes here

                    AuditViewModel audit = new()
                    {
                        ActionName = log.Type,
                        UserFullName = userFullName,
                        AssetName = log.AssetName,
                        TableName = log.TableName,
                        DateTime = dateTime,
                        OldValue = log.OldValues,
                        NewValue = log.NewValues,
                        AffectedColumn = log.AffectedColumns,
                    };
                    auditViewModelList.Add(audit);
                }
            }
            catch(Exception ex)
            {
                var m = ex.Message;
            }

            
            return Json(new { data = auditViewModelList });
        }
    }
}
