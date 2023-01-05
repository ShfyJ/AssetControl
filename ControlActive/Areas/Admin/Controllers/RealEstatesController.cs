using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using ControlActive.ViewModels;
using System.IO;
using ClosedXML.Excel;
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Pdf;
using MigraDoc.Rendering;
using System.Diagnostics;
using System.Text;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.InkML;
using Table = MigraDoc.DocumentObjectModel.Tables.Table;
using System.Globalization;
using System.Web.WebPages;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class RealEstatesController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RealEstatesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }
        [HttpPost]
        public async Task<IActionResult> ExcelReport(int? orgId, string balance, string notBalance, int? region,  DateTime chosenDate,
                                                        string notBalance_block2, string notBalance_block6, string notBalance_block7,
                                                            string balance_block1, string balance_block4,
                                                                string block1, string block2, string block3, string block4, string block5, string block6, string block7, string block8,
                                                                    string rNameB1, string cadNumberB1, string cadDateB1, string activityB1, string comDateB1, string regionB1, string districtB1, string adressB1, string assetHNameB1, string bAreaB1, string fAreaB1, string infraB1, string noEmployeeB1, string maintCostB1, string initCostB1, string wearB1, string resValueB1, string proposalB1,
                                                                        string trFormB2, string orgNameB2, string sNumberB2, string sDateB2, string orgNameAssetB2, string tCostB2, string vatB2, string actDateB2, string actNumberB2, string agDateB2, string agNumberB2,
                                                                            string evOrgNameB3, string rDateB3, string rRegNumberB3, string mValueB3, string examOrgNameB3, string examRDateB3, string examRNumberB3, string rStatusB3,
                                                                                string tradePNameB4, string amountBidB4, string biddingExpDateB4, string expoTimeB4, string activeValB4, string bidHoldDateB4,
                                                                                    string gBodyNameB5, string sDateB5, string sNumberB5, string percentageB5, string amountB5, string numberStepsB5, string assetValDecB5,
                                                                                        string gBodyNameB6, string sDateB6, string sNumberB6, string bidDateB6, string assetBNameB6, string amountAssetSoldB6, string agDateB6, string agNumberB6, string amountPayedB6, string percentB6, string actDateB6, string actNumberB6,
                                                                                            string gBodyNameB7, string sDateB7, string sNumberB7, string bidDateB7, string assetBNameB7, string amountAssetB7, string agDateB7, string agNumB7, string instTimeB7, string minInitPB7, string actInitPercentB7, string paymentPeriodB7, string scheduleAmountB7, string actPaymentB7, string diffB7, string actDateB7, string actNumB7,
                                                                                                string fNameB8, string positionB8, string phNumberB8, string emailB8 )
        {
            if(chosenDate == DateTime.MinValue)
            {
                chosenDate = DateTime.Now;
            }

            int End;

            int sumRow;

            int number = 1;

            #region block1 values to calculate
            float buildingArea=0; 
            
            float fullArea=0; 

            int numberOfEmployees=0; 

            float maintanenceCostForYear=0; 

            float initialCostOfObject=0; 

            float wear=0; 

            float residualValueOfObject=0; 
            #endregion

            #region block2 values to calculate
            float totalCost=0;

            double vat=0;
            #endregion

            #region block3 values to calculate
            float marketvalue=0;
            #endregion

            #region block4 values to calculate
            int AmountOnBidding=0;

            float ActiveValue=0;
            #endregion

            #region block5 values to calculate
            float Amount=0;

            float AssetValueAfterDecline=0;
            #endregion

            #region block6 values to calculate
            float amountOfAssetSold1=0;

            float AmountPayed=0;
            #endregion

            #region block7 values to calculate
            float amountOfAssetSold2=0;

            float actualInitPayment=0;

            float scheduledAmount=0;

            float actualPayment=0;

            float difference=0;

            #endregion

            List<RealEstate> RealEstates = new();

            if(orgId == 0) 
            {
                if(balance != null && notBalance != null)
                {
                    RealEstates = await _context.RealEstates
                      .Include(r => r.ApplicationUser)
                      .ThenInclude(a => a.Organization)
                      .Include(r => r.DistrictOfObject)
                      .Include(r => r.Proposal)
                      .Include(r => r.RegionOfObject)
                      .Where(i => i.Confirmed == true )
                      .ToListAsync();

                    
                }

                if(balance != null && notBalance == null)
                {
                    if(balance_block1 != null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.Status == true)
                                      .ToListAsync();
                    }


                    if (balance_block1 != null && balance_block4 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.Status == true && !i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }

                    if (balance_block1 == null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.Status == true && i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }
                }

                if (balance == null && notBalance != null)
                {
                    if(notBalance_block2 != null && notBalance_block6!= null && notBalance_block7!= null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.Status == false)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r=> r.TransferredAsset)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset != null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }
                }
                 
            }

            else
            {
                string name = _context.Organizations.FirstOrDefault(o => o.OrganizationId == orgId).OrganizationName;

                if (balance != null && notBalance != null)
                {
                    RealEstates = await _context.RealEstates
                      .Include(r => r.ApplicationUser)
                      .ThenInclude(a => a.Organization)
                      .Include(r => r.DistrictOfObject)
                      .Include(r => r.Proposal)
                      .Include(r => r.RegionOfObject)
                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) )
                      .ToListAsync();
                }

                if (balance != null && notBalance == null)
                {
                    if (balance_block1 != null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == true)
                                      .ToListAsync();
                    }


                    if (balance_block1 != null && balance_block4 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == true && !i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }

                    if (balance_block1 == null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == true && i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }
                }

                if (balance == null && notBalance != null)
                {
                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset == null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset == null && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset == null && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset != null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }
                }
            }

            if (region != 0)
            {
                RealEstates = RealEstates.Where(r => r.RegionId == region).ToList();
            }

            sumRow = RealEstates.Count() +6;

            IXLWorkbook workbook = new XLWorkbook();

            IXLWorksheet sheet = workbook.Worksheets.Add("Кўчмас Мулк Объектлари");
            
            #region Create headers for table

            int i = 1;

            sheet.Cell(5, i).Value = "№";
            i++;

            sheet.Cell(5, i).Value = "Объект номи";
            i++;

            if (block1 != null)
            {
                var A = sheet.Cell(4, i-1).Address;
                 
                #region Block 1

                if (cadNumberB1 != null)
                {
                    sheet.Cell(5, i).Value = "Кадастр рақами";
                    i++;
                }

                if (cadDateB1 != null)
                {

                    sheet.Cell(5, i).Value = "Кадастрни рўйхатдан ўтказиш санаси";
                    i++;
                }

                if (activityB1 != null)
                {
                    sheet.Cell(5, i).Value = "Фаолият тури";
                    i++;
                }

                if (comDateB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ишга тушириш санаси";
                    i++;
                }

                if (regionB1 != null)
                {
                    sheet.Cell(5, i).Value = "Вилоят";
                    i++;
                }

                if (districtB1 != null)
                {
                    sheet.Cell(5, i).Value = "Туман";
                    i++;
                }

                if (adressB1 != null)
                {
                    sheet.Cell(5, i).Value = "Манзил (махалла, кўчаси, уй, КФЙ)";
                    i++;
                }

                if (assetHNameB1 != null)
                {
                    sheet.Cell(5, i).Value = "Объект эгаси номи";
                    i++;
                }

                if (bAreaB1 != null)
                {
                    sheet.Cell(5, i).Value = "Бино иншоот майдони (кв.м.)";
                    i++;
                }

                if (fAreaB1 != null)
                {
                    sheet.Cell(5, i).Value = "Умумий майдон (Га)";
                    i++;
                }

                if (infraB1 != null)
                {
                    sheet.Cell(5, i).Value = "Инфратузилманинг мавжудлиги";
                    i++;
                }

                if (noEmployeeB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ишчилар сони";
                    i++;
                }

                if (maintCostB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ҳисобот даври учун йиллик сақлаш харажати (минг сўм)";
                    i++;
                }

                if (initCostB1 != null)
                {
                    sheet.Cell(5, i).Value = "Объектнинг бошланғич баланс қиймати (минг сўм)";
                    i++;
                }

                if (wearB1 != null)
                {
                    sheet.Cell(5, i).Value = "Амортизатсия (минг сўм)";
                    i++;
                }

                if (resValueB1 != null)
                {
                    sheet.Cell(5, i).Value = "Объектнинг баланс (қолдиқ) қиймати (минг сўм)";
                    i++;
                }

                if (proposalB1 != null)
                {
                    sheet.Cell(5, i).Value = "Объектдан янада самарали фойдаланиш бўйича таклифлар";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB1 = sheet.Range(A, Z);
                rangeB1.Merge();
                rangeB1.Value = "«Умумий Маълумотлар»";
                rangeB1.Style.Fill.SetBackgroundColor(XLColor.LightYellow);
                #endregion
            }

            else
            {
                var Z = sheet.Cell(4, 1).Address;
                var A = Z;

                IXLRange rangeB1 = sheet.Range(A, Z);
                rangeB1.Merge();
                rangeB1.Value = "";
                rangeB1.Style.Fill.SetBackgroundColor(XLColor.LightYellow);
            }

            if (block2 != null)
            {
                var A = sheet.Cell(4, i).Address;

                //Block 2
                #region

                if (trFormB2 != null)
                {
                    sheet.Cell(5, i).Value = "Трансеф шакли";
                    i++;
                }

                if (orgNameB2 != null)
                {
                    sheet.Cell(5, i).Value = "Активни топшириш тўғрисида қарор қабул қилган бошқарув органи";
                    i++;
                }

                if (sNumberB2 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор рақами";
                    i++;
                }

                if (sDateB2 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор санаси";
                    i++;
                }

                if (orgNameAssetB2 != null)
                {
                    sheet.Cell(5, i).Value = "Актив топширилган ташкилот номи";
                    i++;
                }

                if (tCostB2 != null)
                {
                    sheet.Cell(5, i).Value = "Умумий қиймати (минг сўм)";
                    i++;
                }

                if (vatB2 != null)
                {
                    sheet.Cell(5, i).Value = "ҚҚС (минг сўм)";
                    i++;
                }

                if (actDateB2 != null)
                {
                    sheet.Cell(5, i).Value = "Актив қабул қилиш ва ўтказиш далолатномасининг санаси";
                    i++;
                }

                if (actNumberB2 != null)
                {
                    sheet.Cell(5, i).Value = "Актив қабул қилиш ва ўтказиш далолатномасининг сони";
                    i++;
                }

                if (agDateB2 != null)
                {
                    sheet.Cell(5, i).Value = "Актив трансфер шартномаси санаси";
                    i++;
                }

                if (agNumberB2 != null)
                {
                    sheet.Cell(5, i).Value = "Топширилган актив шартномаси сони";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB2 = sheet.Range(A, Z);
                rangeB2.Merge();
                rangeB2.Value = "«Бериб Юборилган Активлар» ";
                rangeB2.Style.Fill.SetBackgroundColor(XLColor.LightApricot);
                #endregion
            }

            if (block3 != null)
            {
                var A = sheet.Cell(4, i).Address;


                //Block 3
                #region

                if (evOrgNameB3 != null)
                {
                    sheet.Cell(5, i).Value = "Баҳолаш ташкилотининг номи";
                    i++;
                }

                if (rDateB3 != null)
                {
                    sheet.Cell(5, i).Value = "Баҳолаш ҳисоботининг санаси";
                    i++;
                }

                if (rRegNumberB3 != null)
                {
                    sheet.Cell(5, i).Value = "Баҳолаш ҳисоботининг рўйхатга олиш рақами";
                    i++;
                }

                if (mValueB3 != null)
                {
                    sheet.Cell(5, i).Value = "Активнинг бозор қиймати (минг сўм)";
                    i++;
                }

                if (examOrgNameB3 != null)
                {
                    sheet.Cell(5, i).Value = "Баҳолаш ҳисоботини экспертизадан ўтказиш учун танланган ташкилот номи";
                    i++;
                }

                if (examRDateB3 != null)
                {
                    sheet.Cell(5, i).Value = "Экспертиза ҳисоботининг санаси";
                    i++;
                }

                if (examRNumberB3 != null)
                {
                    sheet.Cell(5, i).Value = "Экспертиза ҳулосасини рўйхатга олиш рақами";
                    i++;
                }

                if (rStatusB3 != null)
                {
                    sheet.Cell(5, i).Value = "Баҳолаш ҳисоботининг холати (ишончли/ишончсиз)";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;


                IXLRange rangeB3 = sheet.Range(A, Z);

                rangeB3.Merge();
                rangeB3.Value = "«Активларни Баҳолаш»";
                rangeB3.Style.Fill.SetBackgroundColor(XLColor.Almond);

                #endregion
            }

            if (block4 != null)
            {
                var A = sheet.Cell(4, i).Address;
                //Block 4
                #region

                if (tradePNameB4 != null)
                {
                    sheet.Cell(5, i).Value = "Актив сотиладиган савдо майдончасининг номи";
                    i++;
                }

                if (amountBidB4 != null)
                {
                    sheet.Cell(5, i).Value = "Аукционга чиқарилган активлар сони";
                    i++;
                }

                if (biddingExpDateB4 != null)
                {
                    sheet.Cell(5, i).Value = "Аукционга қўйилган сана";
                    i++;
                }

                if (expoTimeB4 != null)
                {
                    sheet.Cell(5, i).Value = "Аукцион муддати (кун)";
                    i++;
                }

                if (amountBidB4 != null)
                {
                    sheet.Cell(5, i).Value = "Актив қиймати (минг сўм)";
                    i++;
                }

                if (bidHoldDateB4 != null)
                {
                    sheet.Cell(5, i).Value = "Аукцион  санаси";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB4 = sheet.Range(A, Z);
                rangeB4.Merge();
                rangeB4.Value = "«Аукционга қўйиш»";
                rangeB4.Style.Fill.SetBackgroundColor(XLColor.BabyBlueEyes);

                #endregion
            }

            if (block5 != null)
            {
                var A = sheet.Cell(4, i).Address;
                //Block 5
                #region

                if (gBodyNameB5 != null)
                {
                    sheet.Cell(5, i).Value = "Ижроия органининг номи";
                    i++;
                }

                if (sDateB5 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор санаси";
                    i++;
                }

                if (sNumberB5 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор рақами";
                    i++;
                }

                if (percentageB5 != null)
                {
                    sheet.Cell(5, i).Value = "Пасайтириш қадами ( % )";
                    i++;
                }

                if (amountB5 != null)
                {
                    sheet.Cell(5, i).Value = "Нархини камайтириш (миқдор)";
                    i++;
                }

                if (numberStepsB5 != null)
                {
                    sheet.Cell(5, i).Value = "Қўлланиладиган қадамлар сони";
                    i++;
                }

                if (assetValDecB5 != null)
                {
                    sheet.Cell(5, i).Value = "Активнинг пасайишдан кейинги қиймати (минг сўм)";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB5 = sheet.Range(A, Z);
                rangeB5.Merge();
                rangeB5.Value = "«Актив Қийматини Босқичма-Босқич Камайтириш»";
                rangeB5.Style.Fill.SetBackgroundColor(XLColor.Amber);

                #endregion
            }

            if (block6 != null)
            {
                var A = sheet.Cell(4, i).Address;
                //Block 6
                #region
                if (gBodyNameB6 != null)
                {
                    sheet.Cell(5, i).Value = "Бошқарув органининг номи";
                    i++;
                }

                if (sDateB6 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор санаси";
                    i++;
                }

                if (sNumberB6 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор рақами";
                    i++;
                }

                if (bidDateB6 != null)
                {
                    sheet.Cell(5, i).Value = "Аукцион санаси";
                    i++;
                }

                if (assetBNameB6 != null)
                {
                    sheet.Cell(5, i).Value = "Актив харидорнинг тўлиқ номи";
                    i++;
                }

                if (amountAssetSoldB6 != null)
                {
                    sheet.Cell(5, i).Value = "Активлар реализацияси суммаси";
                    i++;
                }

                if (agDateB6 != null)
                {
                    sheet.Cell(5, i).Value = "Шартнома санаси";
                    i++;
                }

                if (agNumberB6 != null)
                {
                    sheet.Cell(5, i).Value = "Шартнома рақами";
                    i++;
                }

                if (amountPayedB6 != null)
                {
                    sheet.Cell(5, i).Value = "Ҳақиқатда тўланган суммаси";
                    i++;
                }

                if (percentB6 != null)
                {
                    sheet.Cell(5, i).Value = "Ҳақиқатда тўланган сумма(%)";
                    i++;
                }

                if (actDateB6 != null)
                {
                    sheet.Cell(5, i).Value = "Активни қабул қилиш ва ўтказиш далолатномасининг санаси";
                    i++;
                }

                if (actNumberB6 != null)
                {
                    sheet.Cell(5, i).Value = "Актив қабул қилиш ва ўтказиш гувоҳномасининг сони";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB6 = sheet.Range(A, Z);

                rangeB6.Merge();
                rangeB6.Value = "«Бир Марталик Тўлов Асосида Сотилган Активлар» ";
                rangeB6.Style.Fill.SetBackgroundColor(XLColor.LightCoral);
                #endregion
            }

            if (block7 != null)
            {
                var A = sheet.Cell(4, i).Address;
                //Block 7
                #region

                if (gBodyNameB7 != null)
                {
                    sheet.Cell(5, i).Value = "Бошқарув органининг номи";
                    i++;
                }

                if (sDateB7 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор санаси";
                    i++;
                }

                if (sNumberB7 != null)
                {
                    sheet.Cell(5, i).Value = "Қарор сони";
                    i++;
                }

                if (bidDateB7 != null)
                {
                    sheet.Cell(5, i).Value = "Сотув санаси";
                    i++;
                }

                if (assetBNameB7 != null)
                {
                    sheet.Cell(5, i).Value = "Актив харидорининг тўлиқ исми";
                    i++;
                }

                if (amountAssetB7 != null)
                {
                    sheet.Cell(5, i).Value = "Активларни реализация қилиш суммаси, сўм";
                    i++;
                }

                if (agDateB7 != null)
                {
                    sheet.Cell(5, i).Value = "Шартнома санаси";
                    i++;
                }

                if (agNumB7 != null)
                {
                    sheet.Cell(5, i).Value = "Шартнома рақами";
                    i++;
                }

                if (instTimeB7 != null)
                {
                    sheet.Cell(5, i).Value = "Тўлаш муддати (ой)";
                    i++;
                }

                if (minInitPB7 != null)
                {
                    sheet.Cell(5, i).Value = "Минимал бошланғич тўлов (минг сўм)";
                    i++;
                }

                if (actPaymentB7 != null)
                {
                    sheet.Cell(5, i).Value = "Тўланган бошланғич тўлов (минг сўм)";
                    i++;
                }

                if (paymentPeriodB7 != null)
                {
                    sheet.Cell(5, i).Value = "Тўлов вақти";
                    i++;
                }

                if (scheduleAmountB7 != null)
                {
                    sheet.Cell(5, i).Value = "График бўйича тўланадиган сумма";
                    i++;
                }

                if (actPaymentB7 != null)
                {
                    sheet.Cell(5, i).Value = "Ҳақиқий тўлов (ўтган давр учун тўланган сумма)";
                    i++;
                }

                if (diffB7 != null)
                {
                    sheet.Cell(5, i).Value = "Қолган сумма (минг сум)";
                    i++;
                }

                if (actDateB7 != null)
                {
                    sheet.Cell(5, i).Value = "Акт санаси";
                    i++;
                }

                if (actNumB7 != null)
                {
                    sheet.Cell(5, i).Value = "Акт сони";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB7 = sheet.Range(A, Z);
                rangeB7.Merge();
                rangeB7.Value = "«Бўлиб Бўлиб Тўлашга Сотилган Активлар» ";
                rangeB7.Style.Fill.SetBackgroundColor(XLColor.BabyBlueEyes);
                #endregion
            }

            if (block8 != null)
            {
                var A = sheet.Cell(4, i).Address;
                //Block 8
                #region
                if (fNameB8 != null)
                {
                    sheet.Cell(5, i).Value = "Масъул шахс Ф.И.Ш";
                    i++;
                }

                if (positionB8 != null)
                {
                    sheet.Cell(5, i).Value = "Лавозим";
                    i++;
                }

                if (phNumberB8 != null)
                {
                    sheet.Cell(5, i).Value = "Алоқа тел. рақами";
                    i++;
                }

                if (emailB8 != null)
                {
                    sheet.Cell(5, i).Value = "Электрон почта адреси";
                    i++;
                }

                var Z = sheet.Cell(4, i - 1).Address;

                IXLRange rangeB8 = sheet.Range(A, Z);
                rangeB8.Merge();
                rangeB8.Value = "«Маълумотларни тўлдириш учун масъул шахс» ";
                rangeB8.Style.Fill.SetBackgroundColor(XLColor.AppleGreen);
                #endregion
            }

            #endregion

            End = i-1;

            int rowForRealEstate = 6;
            int rowsToMerge = 0;
            List<int> termsList = new();

            try
            {
                foreach (var realEstate in RealEstates)
                {
                    termsList.Clear();

                    var transferredAsset = _context.TransferredAssets.Where(t => t.AssetId == realEstate.TransferredAssetId && t.AgreementDate.Date <= chosenDate).ToList();

                    var assetEvaluations = _context.AssetEvaluations.Where(a => a.RealEstateId == realEstate.RealEstateId
                        && a.ReportDate.Date <= chosenDate && a.StatusChangedDate.Date > chosenDate.Date).ToList();

                    var submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.RealEstateId == realEstate.RealEstateId
                        && s.BiddingExposureDate.Date <= chosenDate && s.AuctionCancelledDate.Date > chosenDate).ToList();

                    var reductionInAssets = _context.ReductionInAssets.Where(s => s.RealEstateId == realEstate.RealEstateId
                        && s.SolutionDate.Date <= chosenDate && s.StatusChangedDate.Date > chosenDate.Date).ToList();

                    var oneTimePaymentAssets = _context.OneTimePaymentAssets.Where(s => s.RealEstateId == realEstate.RealEstateId
                        && s.SolutionDate.Date <= chosenDate && s.BiddingCancelledDate.Date > chosenDate.Date
                        && s.Step2.ContractCancelledDate > chosenDate).Include(s => s.Step2).Include(s => s.Step3).ToList();

                    var installmentAssets = _context.InstallmentAssets.Where(s => s.RealEstateId == realEstate.RealEstateId
                        && s.AggreementDate.Date <= chosenDate && s.ContractCancelledDate.Date > chosenDate.Date).ToList();

                    termsList.Add(transferredAsset.Count);
                    termsList.Add(assetEvaluations.Count);
                    termsList.Add(submissionOnBiddings.Count);
                    termsList.Add(reductionInAssets.Count);
                    termsList.Add(oneTimePaymentAssets.Count);
                    termsList.Add(installmentAssets.Count);

                    if (termsList.Max() > 1)
                        rowsToMerge = termsList.Max() - 1;

                    string infr = "";
                    foreach (var inf in _context.RRealEstateInfrastructures.Include(i => i.Infrastucture).Where(i => i.RealEstateId == realEstate.RealEstateId))
                    {
                        infr += inf.Infrastucture.InfrastructureName + " ";
                    }

                    var reg = await _context.Regions.FirstOrDefaultAsync(i => i.RegionId == realEstate.RegionId);
                    var dis = await _context.Districts.FirstOrDefaultAsync(i => i.DistrictId == realEstate.DistrictId);
                    var pr = _context.Proposals.FirstOrDefault(i => i.ProposalId == realEstate.ProposalId);


                    #region Writing values into cells

                    int j = 1;

                    sheet.Cell(rowForRealEstate, j).Value = number;
                    number++;
                    j++;

                    var RealEstateNameSh = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                    RealEstateNameSh.Value = realEstate.RealEstateName;
                    RealEstateNameSh.Merge();
                    j++;

                    if (block1 != null)
                    {
                        #region Block 1 

                        if (cadNumberB1 != null)
                        {
                            var CadastreNumberSh = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            CadastreNumberSh.Value = realEstate.CadastreNumber;
                            CadastreNumberSh.Merge();
                            j++;
                        }

                        if (cadDateB1 != null)
                        {
                            var CadastreRegDateSh = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            CadastreRegDateSh.Value = realEstate.CadastreRegDate.ToString("dd/MM/yyy");
                            CadastreRegDateSh.Merge();
                            j++;
                        }

                        if (activityB1 != null)
                        {
                            var ActivitySh = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            ActivitySh.Value = realEstate.Activity;
                            ActivitySh.Merge();

                            j++;
                        }

                        if (comDateB1 != null)
                        {
                            var CommisioningDateS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            CommisioningDateS.Value = realEstate.CommisioningDate.ToString("dd/MM/yyy");
                            CommisioningDateS.Merge();

                            j++;
                        }

                        if (regionB1 != null)
                        {
                            var RegionNameS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j); //d
                            RegionNameS.Value = reg.RegionName;
                            RegionNameS.Merge();

                            j++;
                        }

                        if (districtB1 != null)
                        {

                            var DistrictNameS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j); //d
                            DistrictNameS.Value = dis.DistrictName;
                            DistrictNameS.Merge();

                            j++;
                        }

                        if (adressB1 != null)
                        {

                            var AddressS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            AddressS.Value = realEstate.Address;
                            AddressS.Merge();

                            j++;
                        }

                        if (assetHNameB1 != null)
                        {

                            var AssetHolderName = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j); //d
                            AssetHolderName.Value = realEstate.AssetHolderName;
                            AssetHolderName.Merge();

                            j++;
                        }

                        if (bAreaB1 != null)
                        {

                            var BuildingAreaS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            BuildingAreaS.Value = realEstate.BuildingArea;
                            BuildingAreaS.Merge();

                            buildingArea += float.Parse(realEstate.BuildingArea, CultureInfo.InvariantCulture.NumberFormat);

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = buildingArea;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (fAreaB1 != null)
                        {

                            var FullAreaS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            FullAreaS.Value = realEstate.FullArea;
                            FullAreaS.Merge();

                            fullArea += float.Parse(realEstate.FullArea, CultureInfo.InvariantCulture.NumberFormat);
                                                        
                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = fullArea;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (infraB1 != null)
                        {

                            var infrS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j); //d
                            infrS.Value = infr;
                            infrS.Merge();

                            j++;
                        }

                        if (noEmployeeB1 != null)
                        {

                            var NumberOfEmployeeS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            NumberOfEmployeeS.Value = realEstate.NumberOfEmployee;
                            NumberOfEmployeeS.Merge();

                            numberOfEmployees += realEstate.NumberOfEmployee;

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = numberOfEmployees;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (maintCostB1 != null)
                        {

                            var MaintenanceCostForYearS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            MaintenanceCostForYearS.Value = realEstate.MaintenanceCostForYear;
                            MaintenanceCostForYearS.Merge();

                            maintanenceCostForYear += float.Parse(realEstate.MaintenanceCostForYear, CultureInfo.InvariantCulture.NumberFormat);

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = maintanenceCostForYear;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (initCostB1 != null)
                        {

                            var InitialCostOfObjectS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            InitialCostOfObjectS.Value = realEstate.InitialCostOfObject;
                            InitialCostOfObjectS.Merge();

                            initialCostOfObject += float.Parse(realEstate.InitialCostOfObject, CultureInfo.InvariantCulture.NumberFormat);

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = initialCostOfObject;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (wearB1 != null)
                        {

                            var WearS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            WearS.Value = realEstate.Wear;
                            WearS.Merge();

                            wear += float.Parse(realEstate.Wear, CultureInfo.InvariantCulture.NumberFormat);

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = wear;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (resValueB1 != null)
                        {

                            var ResidualValueOfObjectS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                            ResidualValueOfObjectS.Value = realEstate.ResidualValueOfObject;
                            ResidualValueOfObjectS.Merge();

                            residualValueOfObject += float.Parse(realEstate.ResidualValueOfObject, CultureInfo.InvariantCulture.NumberFormat);

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = residualValueOfObject;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (proposalB1 != null)
                        {

                            var ProposalNameS = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j); //d
                            ProposalNameS.Value = pr.ProposalName;
                            ProposalNameS.Merge();

                            j++;
                        }

                        #endregion
                    }

                    int rowForInfo = rowForRealEstate;

                    if (block2 != null)
                    {
                        #region Block 2

                        // 3. Блок «Переданные активы» 
                        foreach (var tr in transferredAsset ?? Enumerable.Empty<TransferredAsset>())
                        {
                            var trForm = _context.TransferForms.FirstOrDefault(i => i.TransferFormId == tr.TransferFormId);

                            if (trFormB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = trForm.TransferFormName;
                                j++;
                            }

                            if (orgNameB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.OrgName;
                                j++;
                            }

                            if (sNumberB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.SolutionNumber;
                                j++;
                            }

                            if (sDateB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.SolutionDate.ToString("dd/MM/yyyy"); ;
                                j++;
                            }

                            if (orgNameAssetB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.OrgNameOfAsset;
                                j++;
                            }

                            if (tCostB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.TotalCost;

                                totalCost += float.Parse(tr.TotalCost, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = totalCost;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (vatB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.VAT;

                                vat += tr.VAT;
                                sheet.Cell(sumRow, j).Value = vat;

                                j++;
                            }

                            if (actDateB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.ActAndAssetDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (actNumberB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.ActAndAssetNumber;
                                j++;
                            }

                            if (agDateB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.AgreementDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (agNumberB2 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = tr.AgreementNumber;
                                j++;
                            }

                            rowForInfo++;
                        }

                        if (!transferredAsset.Any())
                        {
                            if (trFormB2 != null)
                            {
                                j++;
                            }

                            if (orgNameB2 != null)
                            {
                                j++;
                            }

                            if (sNumberB2 != null)
                            {
                                j++;
                            }

                            if (sDateB2 != null)
                            {
                                j++;
                            }

                            if (orgNameAssetB2 != null)
                            {
                                j++;
                            }

                            if (tCostB2 != null)
                            {
                                j++;
                            }

                            if (vatB2 != null)
                            {
                                j++;
                            }

                            if (actDateB2 != null)
                            {
                                j++;
                            }

                            if (actNumberB2 != null)
                            {
                                j++;
                            }

                            if (agDateB2 != null)
                            {
                                j++;
                            }

                            if (agNumberB2 != null)
                            {
                                j++;
                            }
                        }
                        #endregion

                    }

                    if (block3 != null)
                    {
                        #region Block 3

                        //4. Блок «Оценка актива» 
                        rowForInfo = rowForRealEstate;

                        foreach (var assetE in assetEvaluations ?? Enumerable.Empty<AssetEvaluation>())
                        {
                            if (evOrgNameB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.EvaluatingOrgName;
                                j++;
                            }

                            if (rDateB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.ReportDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (rRegNumberB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.ReportRegNumber;
                                j++;
                            }

                            if (mValueB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.MarketValue;

                                marketvalue += float.Parse(assetE.MarketValue, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = marketvalue;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (examOrgNameB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.ExaminingOrgName;
                                j++;
                            }

                            if (examRDateB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.ExamReportDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (examRNumberB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.ExamReportRegNumber;
                                j++;
                            }

                            if (rStatusB3 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = assetE.ReportS;
                                j++;
                            }

                            rowForInfo++;
                        }

                        if (!assetEvaluations.Any())
                        {
                            if (evOrgNameB3 != null)
                            {
                                j++;
                            }

                            if (rDateB3 != null)
                            {
                                j++;
                            }

                            if (rRegNumberB3 != null)
                            {
                                j++;
                            }

                            if (mValueB3 != null)
                            {
                                j++;
                            }

                            if (examOrgNameB3 != null)
                            {
                                j++;
                            }

                            if (examRDateB3 != null)
                            {
                                j++;
                            }

                            if (examRNumberB3 != null)
                            {
                                j++;
                            }

                            if (rStatusB3 != null)
                            {
                                j++;
                            }
                        }

                        #endregion
                    }

                    if (block4 != null)
                    {
                        #region Block 4

                        //5.Блок «Выставление на торги» 
                        rowForInfo = rowForRealEstate;

                        foreach (var sbOnB in submissionOnBiddings ?? Enumerable.Empty<SubmissionOnBidding>())
                        {
                            if (tradePNameB4 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = sbOnB.TradingPlatformName;
                                j++;
                            }

                            if (amountBidB4 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = sbOnB.AmountOnBidding;

                                AmountOnBidding += sbOnB.AmountOnBidding;

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = AmountOnBidding;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (biddingExpDateB4 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = sbOnB.BiddingExposureDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (expoTimeB4 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = sbOnB.ExposureTime;
                                j++;
                            }

                            if (amountBidB4 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = sbOnB.ActiveValue;

                                ActiveValue += float.Parse(sbOnB.ActiveValue, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = ActiveValue;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (bidHoldDateB4 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = sbOnB.BiddingHoldDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            rowForInfo++;
                        }

                        if (!submissionOnBiddings.Any())
                        {
                            if (tradePNameB4 != null)
                            {
                                j++;
                            }

                            if (amountBidB4 != null)
                            {
                                j++;
                            }

                            if (biddingExpDateB4 != null)
                            {
                                j++;
                            }

                            if (expoTimeB4 != null)
                            {
                                j++;
                            }

                            if (amountBidB4 != null)
                            {
                                j++;
                            }

                            if (bidHoldDateB4 != null)
                            {
                                j++;
                            }
                        }

                        #endregion
                    }

                    if (block5 != null)
                    {
                        #region Block 5

                        // 6. Блок «Пошаговое снижение стоимости актива»  
                        rowForInfo = rowForRealEstate;

                        foreach (var red in reductionInAssets ?? Enumerable.Empty<ReductionInAsset>())
                        {
                            if (gBodyNameB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.GoverningBodyName;
                                j++;
                            }

                            if (sDateB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.SolutionDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (sNumberB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.SolutionNumber;
                                j++;
                            }

                            if (percentageB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.Percentage;
                                j++;
                            }

                            if (amountB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.Amount;

                                Amount += float.Parse(red.Amount, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = Amount;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (numberStepsB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.NumberOfSteps;
                                j++;
                            }

                            if (assetValDecB5 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = red.AssetValueAfterDecline;

                                AssetValueAfterDecline += float.Parse(red.AssetValueAfterDecline, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = AssetValueAfterDecline;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            rowForInfo++;
                        }

                        if (!reductionInAssets.Any())
                        {
                            if (gBodyNameB5 != null)
                            {
                                j++;
                            }

                            if (sDateB5 != null)
                            {
                                j++;
                            }

                            if (sNumberB5 != null)
                            {
                                j++;
                            }

                            if (percentageB5 != null)
                            {
                                j++;
                            }

                            if (amountB5 != null)
                            {
                                j++;
                            }

                            if (numberStepsB5 != null)
                            {
                                j++;
                            }

                            if (assetValDecB5 != null)
                            {
                                j++;
                            }
                        }

                        #endregion
                    }

                    if (block6 != null)
                    {
                        #region Block 6

                        //7. Блок «Реализованные активы с единовременной оплатой»
                        rowForInfo = rowForRealEstate;

                        foreach (var oneT in oneTimePaymentAssets ?? Enumerable.Empty<OneTimePaymentAsset>())
                        {

                            if (gBodyNameB6 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = oneT.GoverningBodyName;
                                j++;
                            }

                            if (sDateB6 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = oneT.SolutionDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (sNumberB6 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = oneT.SolutionNumber;
                                j++;
                            }

                            if (bidDateB6 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = oneT.BiddingDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            var oneT2 = _context.OneTimePaymentStep2.FirstOrDefault(i => i.OneTimePaymentStep2Id == oneT.OneTimePaymentStep2Id);

                            if (oneT2 != null)
                            {
                                if (assetBNameB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT2.AssetBuyerName;
                                    j++;
                                }

                                if (amountAssetSoldB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT2.AmountOfAssetSold;

                                    amountOfAssetSold1 += float.Parse(oneT2.AmountOfAssetSold, CultureInfo.InvariantCulture.NumberFormat);

                                    var cell = sheet.Cell(sumRow, j);
                                    cell.Value = amountOfAssetSold1;

                                    cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                    cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                    cell.Style.Alignment.WrapText = true;
                                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                    j++;
                                }

                                if (agDateB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT2.AggreementDate.ToString("dd/MM/yyyy");
                                    j++;
                                }

                                if (agNumberB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT2.AggreementNumber;
                                    j++;
                                }

                                if (amountPayedB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT2.AmountPayed;

                                    AmountPayed += float.Parse(oneT2.AmountPayed, CultureInfo.InvariantCulture.NumberFormat);

                                    var cell = sheet.Cell(sumRow, j);
                                    cell.Value = AmountPayed;

                                    cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                    cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                    cell.Style.Alignment.WrapText = true;
                                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                    cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                    j++;
                                }

                                if (percentB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT2.Percentage;
                                    j++;
                                }

                            }

                            else
                            {
                                if (assetBNameB6 != null)
                                {
                                    j++;
                                }

                                if (amountAssetSoldB6 != null)
                                {
                                    j++;
                                }

                                if (agDateB6 != null)
                                {
                                    j++;
                                }

                                if (agNumberB6 != null)
                                {
                                    j++;
                                }

                                if (amountPayedB6 != null)
                                {
                                    j++;
                                }

                                if (percentB6 != null)
                                {
                                    j++;
                                }
                            }

                            var oneT3 = _context.OneTimePaymentStep3.FirstOrDefault(i => i.OneTimePaymentStep3Id == oneT.OneTimePaymentStep3Id);

                            if (oneT3 != null)
                            {
                                if (actDateB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT3.ActAndAssetDate.ToString("dd/MM/yyyy");
                                    j++;
                                }

                                if (actNumberB6 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = oneT3.ActAndAssetNumber;
                                    j++;
                                }
                            }

                            else
                            {
                                if (actDateB6 != null)
                                {
                                    j++;
                                }

                                if (actNumberB6 != null)
                                {
                                    j++;
                                }
                            }

                            rowForInfo++;
                        }

                        if (!oneTimePaymentAssets.Any())
                        {
                            if (gBodyNameB6 != null)
                            {
                                j++;
                            }

                            if (sDateB6 != null)
                            {
                                j++;
                            }

                            if (sNumberB6 != null)
                            {
                                j++;
                            }

                            if (bidDateB6 != null)
                            {
                                j++;
                            }

                            if (assetBNameB6 != null)
                            {
                                j++;
                            }

                            if (amountAssetSoldB6 != null)
                            {
                                j++;
                            }

                            if (agDateB6 != null)
                            {
                                j++;
                            }

                            if (agNumberB6 != null)
                            {
                                j++;
                            }

                            if (amountPayedB6 != null)
                            {
                                j++;
                            }

                            if (percentB6 != null)
                            {
                                j++;
                            }

                            if (actDateB6 != null)
                            {
                                j++;
                            }

                            if (actNumberB6 != null)
                            {
                                j++;
                            }
                        }

                        #endregion
                    }

                    if (block7 != null)
                    {

                        #region Block 7

                        //8. Блок «Активы, реализованные в рассрочку» 
                        rowForInfo = rowForRealEstate;

                        foreach (var inst in installmentAssets ?? Enumerable.Empty<InstallmentAsset>())
                        {
                            if (gBodyNameB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.GoverningBodyName;
                                j++;
                            }

                            if (sDateB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.SolutionDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (sNumberB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.SolutionNumber;
                                j++;
                            }

                            if (bidDateB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.BiddingDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (assetBNameB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.AssetBuyerName;
                                j++;
                            }

                            if (amountAssetB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.AmountOfAssetSold;

                                amountOfAssetSold2 += float.Parse(inst.AmountOfAssetSold, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = amountOfAssetSold2;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (agDateB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.AggreementDate.ToString("dd/MM/yyyy");
                                j++;
                            }

                            if (agNumB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.AggreementNumber;
                                j++;
                            }

                            if (instTimeB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.InstallmentTime + " ой"; // month or year
                                j++;
                            }

                            if (minInitPB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.MinInitialPaymentAmount + " (" + inst.MinInitialPaymentPercentage + " %)";
                                j++;
                            }

                            if (actPaymentB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.ActualInitPayment + " (" + inst.ActualInitPaymentPercentage + ")";

                                actualInitPayment += float.Parse(inst.ActualInitPayment, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = actualInitPayment;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (paymentPeriodB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.PaymentPeriodType; //look
                                j++;
                            }

                            if (scheduleAmountB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.ScheduledAmount;

                                scheduledAmount += inst.ScheduledAmount;

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = scheduledAmount;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (actPaymentB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.ActualPayment;

                                actualPayment += float.Parse(inst.ActualPayment, CultureInfo.InvariantCulture.NumberFormat);

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = actualPayment;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            if (diffB7 != null)
                            {
                                sheet.Cell(rowForInfo, j).Value = inst.Difference;

                                difference += inst.Difference;

                                var cell = sheet.Cell(sumRow, j);
                                cell.Value = difference;

                                cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                                cell.Style.Alignment.WrapText = true;
                                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                                cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                                j++;
                            }

                            var inst2 = _context.InstallmentStep2.FirstOrDefault(i => i.InstallmentStep2Id == inst.InstallmentStep2Id);

                            if (inst2 != null)
                            {
                                if (actDateB7 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = inst2.ActAndAssetDate.ToString("dd/MM/yyyy");
                                    j++;
                                }

                                if (actNumB7 != null)
                                {
                                    sheet.Cell(rowForInfo, j).Value = inst2.ActAndAssetNumber;
                                    j++;
                                }
                            }

                            else
                            {
                                if (actDateB7 != null)
                                    j++;

                                if (actNumB7 != null)
                                    j++;
                            }

                            rowForInfo++;
                        }

                        if (!installmentAssets.Any())
                        {
                            if (gBodyNameB7 != null)
                            {
                                j++;
                            }

                            if (sDateB7 != null)
                            {
                                j++;
                            }

                            if (sNumberB7 != null)
                            {
                                j++;
                            }

                            if (bidDateB7 != null)
                            {
                                j++;
                            }

                            if (assetBNameB7 != null)
                            {
                                j++;
                            }

                            if (amountAssetB7 != null)
                            {
                                j++;
                            }

                            if (agDateB7 != null)
                            {
                                j++;
                            }

                            if (agNumB7 != null)
                            {
                                j++;
                            }

                            if (instTimeB7 != null)
                            {
                                j++;
                            }

                            if (minInitPB7 != null)
                            {
                                j++;
                            }

                            if (actPaymentB7 != null)
                            {
                                j++;
                            }

                            if (paymentPeriodB7 != null)
                            {
                                j++;
                            }

                            if (scheduleAmountB7 != null)
                            {
                                j++;
                            }

                            if (actPaymentB7 != null)
                            {
                                j++;
                            }

                            if (diffB7 != null)
                            {
                                j++;
                            }

                            if (actDateB7 != null)
                            {
                                j++;
                            }

                            if (actNumB7 != null)
                            {
                                j++;
                            }
                        }

                        #endregion
                    }

                    if (block8 != null)
                    {

                        #region Block 8

                        //9. Блок «Ответственное лицо по заполнению данных» 

                        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(i => i.Id == realEstate.ApplicationUserId);

                        if (user != null)
                        {

                            if (fNameB8 != null)
                            {
                                var FN = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                                FN.Merge();
                                FN.Value = user.Fullname;
                                j++;
                            }

                            if (positionB8 != null)
                            {
                                var Pos = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                                Pos.Merge();
                                Pos.Value = user.Postion;
                                j++;
                            }

                            if (phNumberB8 != null)
                            {
                                var PhN = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                                PhN.Merge();
                                PhN.Value = user.PhoneNumber.Substring(user.PhoneNumber.Length - 9);

                                j++;
                            }

                            if (emailB8 != null)
                            {
                                var Em = sheet.Range(rowForRealEstate, j, (rowForRealEstate + rowsToMerge), j);
                                Em.Merge();
                                Em.Value = user.Email;

                                j++;
                            }
                        }

                        #endregion
                    }

                    #endregion

                    rowForRealEstate = rowForRealEstate + rowsToMerge + 1;
                    rowsToMerge = 0;

                }
            }
            catch(Exception e)
            {
                var ex = e.Message;
            }
            

                var Bb = sheet.Range(4, 1, rowForRealEstate-1, End); // change to other rows
                Bb.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                Bb.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                Bb.Style.Alignment.WrapText = true;
                Bb.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                Bb.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                sheet.Columns(2, End).Width = 24;
                //wrap
                //sheet.Cell(e, 2).Style.Font.FontSize = 8;
                sheet.Rows(4, 5).Style.Font.Bold = true;
                //sheet.Cell(e, 2).Style.Font.Italic = true;
                //sheet.Cell(e, 2).Style.Alignment.WrapText = true;
                //sheet.Cell(e, 3).Style.Font.FontSize = 8;
                //sheet.Cell(e, 3).Style.Alignment.WrapText = true;
                sheet.SheetView.Freeze(5, 1);
                sheet.SheetView.Freeze(5, 2);
                //sheet.Cell(e, 3).Style.Font.Bold = true;
                //sheet.Cell(e, 3).Style.Font.Italic = true;
                //sheet.Row(6).Height = 50;
                //sheet.Row(5).Height = 170;
                sheet.Rows(6, rowForRealEstate).Height = 28;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Вывод итоговой информации.xlsx"
                        );
                }

            
        }
        
        [HttpPost]
        public async Task<IActionResult> PassportReport(int? orgId, string balance, string notBalance, int? region,  DateTime chosenDate,
                                                         string notBalance_block2,  string notBalance_block6, string notBalance_block7,
                                                            string balance_block1,  string balance_block4)
        {

            List<RealEstate> RealEstates = new();

            if (orgId == 0)
            {
                if (balance != null && notBalance != null)
                {
                    RealEstates = await _context.RealEstates
                      .Include(r => r.ApplicationUser)
                      .ThenInclude(a => a.Organization)
                      .Include(r => r.DistrictOfObject)
                      .Include(r => r.Proposal)
                      .Include(r => r.RegionOfObject)
                      .Where(i => i.Confirmed == true )
                      .ToListAsync();


                }

                if (balance != null && notBalance == null)
                {
                    if (balance_block1 != null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.Status == true)
                                      .ToListAsync();
                    }


                    if (balance_block1 != null && balance_block4 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true  && i.Status == true && !i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }

                    if (balance_block1 == null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.Status == true && i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }
                }

                if (balance == null && notBalance != null)
                {
                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.Status == false)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset != null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }
                }

            }

            else
            {
                string name = _context.Organizations.FirstOrDefault(o => o.OrganizationId == orgId).OrganizationName;

                if (balance != null && notBalance != null)
                {
                    RealEstates = await _context.RealEstates
                      .Include(r => r.ApplicationUser)
                      .ThenInclude(a => a.Organization)
                      .Include(r => r.DistrictOfObject)
                      .Include(r => r.Proposal)
                      .Include(r => r.RegionOfObject)
                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) )
                      .ToListAsync();
                }

                if (balance != null && notBalance == null)
                {
                    if (balance_block1 != null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == true)
                                      .ToListAsync();
                    }


                    if (balance_block1 != null && balance_block4 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == true && !i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }

                    if (balance_block1 == null && balance_block4 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == true && i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }
                }

                if (balance == null && notBalance != null)
                {
                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset == null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset == null && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset == null && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && i.TransferredAsset != null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        RealEstates = await _context.RealEstates
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.Proposal)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.AssetHolderName.Equals(name) && i.Status == false && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }
                }
            }

            if (region != 0)
            {
                RealEstates = RealEstates.Where(r => r.RegionId == region).ToList();
            }

            //Set the stream:
            var myStream = new MemoryStream();

            Document document = new Document();
            document.Info.Title = "Объектлар паспорти";
            document.Info.Author = "Активларни бошқариш департаменти, Узбекнефтегаз АЖ";

            #region Define styles in the document

            Style style = document.Styles["Normal"];
            style.Font.Name = "Times New Roman";

            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

           // Create a new style called TextBox based on style Normal

            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal

            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;

            #endregion

            #region Define the cover page
            Section section = document.AddSection();

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = "3cm";

            MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage("wwwroot/assets/images/logo/small-logo.png");
            image.Width = "10cm";

            paragraph = section.AddParagraph("Танланган объектлар паспорт маълумотларини кўрсатувчи ҳужжат");
            paragraph.Format.Font.Size = 16;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "5cm";
            paragraph.Format.SpaceAfter = "3cm";

            paragraph = section.AddParagraph("Ҳужжат шакллантирилган сана: ");
            paragraph.AddDateField();

            #endregion

            #region Defines page setup, headers, and footers.
            Section section2 = document.AddSection();
            section2.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section2.PageSetup.StartingNumber = 1;
            section2.PageSetup = document.DefaultPageSetup.Clone();
            section2.PageSetup.PageFormat = PageFormat.A3;
            section2.PageSetup.Orientation = Orientation.Landscape;

            //HeaderFooter header = section2.Headers.Primary;
            //header.AddParagraph("\tOdd Page Header");

            //header = section2.Headers.EvenPage;
            //header.AddParagraph("Even Page Header");

            //Create a paragraph with centered page number. See definition of style "Footer".
            Paragraph paragraph2 = new Paragraph();
            paragraph2.AddTab();
            paragraph2.AddPageField();

            //Add paragraph to footer for odd pages.
            section2.Footers.Primary.Add(paragraph2);

            //Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            //not belong to more than one other object. If you forget cloning an exception is thrown.
            section2.Footers.EvenPage.Add(paragraph2.Clone());

            #endregion

            #region Define tables
            
            //document.LastSection.AddParagraph("Cell Merge", "Heading2");

            List<string> InfraNamesOn = new();

            var infrastuctures = _context.Infrastuctures.ToList();

            List<string> allInfraNames = new();

            foreach (var infra in infrastuctures)
            {
                allInfraNames.Add(infra.InfrastructureName);
            }

            IEnumerable<string> InfraNamesOff;

            List<string> InfraResults = new();

            foreach(var item in RealEstates)
            {
                InfraNamesOn.Clear();
                InfraNamesOff = Enumerable.Empty<string>();
                InfraResults.Clear();

                var realEstateInfras = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == item.RealEstateId).ToList();

                foreach(var r in realEstateInfras)
                {
                    var infrastructure = await _context.Infrastuctures.FindAsync(r.InfrastuctureId);
                    InfraNamesOn.Add(infrastructure.InfrastructureName);
                }

                InfraNamesOff = allInfraNames.Except(InfraNamesOn);

                foreach(var infra in InfraNamesOn)
                {
                    InfraResults.Add(infra);
                    InfraResults.Add("Мавжуд");
                }

                foreach(var infra in InfraNamesOff)
                {
                    InfraResults.Add(infra);
                    InfraResults.Add("Мавжуд эмас");
                }

                Paragraph paragraph3 = document.LastSection.AddParagraph(item.RealEstateName, "Heading1");
                paragraph3.AddBookmark(item.RealEstateName);

                Table table3 = document.LastSection.AddTable();
                table3.Borders.Visible = true;
                table3.TopPadding = 2;
                table3.BottomPadding = 2;

                Column column3 = table3.AddColumn(Unit.FromCentimeter(5));
                column3.Format.Alignment = ParagraphAlignment.Left;

                column3 = table3.AddColumn(Unit.FromCentimeter(12));
                column3.Format.Alignment = ParagraphAlignment.Left;

                column3 = table3.AddColumn(Unit.FromCentimeter(10));
                column3.Format.Alignment = ParagraphAlignment.Center;

                table3.Rows.Height = 28;

                Row row3 = table3.AddRow();
                row3.Format.Font.Bold = true;
                row3.Cells[0].AddParagraph("Объект Паспорти");
               
                row3.Cells[0].MergeRight = 1;
                row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                string link1 = "wwwroot" + item.PhotoOfObjectLink1;
                MigraDoc.DocumentObjectModel.Shapes.Image image1 = row3.Cells[2].AddImage(link1);
                image1.Height = "5cm";
                image1.Width = "9.8cm";
                image1.LockAspectRatio = true;
                row3.Cells[2].MergeDown = 4;

                row3 = table3.AddRow();
                row3.VerticalAlignment = VerticalAlignment.Center;
                row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row3.Cells[0].AddParagraph("Объект номи");
                row3.Cells[1].AddParagraph(item.RealEstateName);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Жойлашган манзили");
                row3.Cells[1].AddParagraph(item.Address);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Баланс сақловчи ташкилот");
                row3.Cells[1].AddParagraph(item.AssetHolderName);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Умумий майдон (кв.м.)");
                row3.Cells[1].AddParagraph(item.FullArea);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Бино иншоот майдони (кв.м.)");
                row3.Cells[1].AddParagraph(item.BuildingArea);
                string link2 = "wwwroot" + item.PhotoOfObjectLink2;
                MigraDoc.DocumentObjectModel.Shapes.Image image2 = row3.Cells[2].AddImage(link2);
                image2.Height = "5cm";
                image2.Width = "9.8cm";
                image2.LockAspectRatio = true;
                row3.Cells[2].MergeDown = 4;

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Ўртача йиллик сақлаш харажатлари (минг сўм)");
                row3.Cells[1].AddParagraph(item.MaintenanceCostForYear);


                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Инфратузилманинг мавжудлиги");
                row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row3.Cells[0].MergeRight = 1;
                row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph(InfraResults[0]);
                row3.Cells[1].AddParagraph(InfraResults[1]);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph(InfraResults[2]);
                row3.Cells[1].AddParagraph(InfraResults[3]);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph(InfraResults[4]);
                row3.Cells[1].AddParagraph(InfraResults[5]);
                string link3 = "wwwroot" + item.PhotoOfObjectLink3;
                MigraDoc.DocumentObjectModel.Shapes.Image image3 = row3.Cells[2].AddImage(link3);
                image3.Height = "5cm";
                image3.Width = "9.8cm";
                image3.LockAspectRatio = true;
                row3.Cells[2].MergeDown = 4;

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph(InfraResults[6]);
                row3.Cells[1].AddParagraph(InfraResults[7]);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph(InfraResults[8]);
                row3.Cells[1].AddParagraph(InfraResults[9]);

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Активдан янада самарали фойдаланиш бўйича таклифлар");
                row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row3.Cells[0].MergeRight = 1;
                row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                row3 = table3.AddRow();
                row3.Cells[0].AddParagraph("Активдан янада самарали фойдаланиш бўйича таклиф");
                row3.Cells[1].AddParagraph(item.Proposal.ProposalName);
            }


            #endregion
           

            document.UseCmykColor = true;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            pdfRenderer.Document = document;   
            
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(myStream);


            var content = myStream.ToArray();
            return File(
                content,
                "application/pdf",
                "Passport.pdf"
                );

        }

        
        // GET: Admin/RealEstates
        public ActionResult Index(bool success = false, bool editSuccess = false)
        {
            
            ViewBag.Success = success;
            ViewBag.EditSuccess = editSuccess;
            ViewBag.Organizations = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName");
            ViewBag.Regions = new SelectList(_context.Regions, "RegionId", "RegionName");

            return View();
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

       
        public async Task<IActionResult> GetPassport(int id)
        {
            var realEstate = _context.RealEstates.Include(r => r.RegionOfObject)
                                .Include(r => r.DistrictOfObject)
                                .Include(r => r.Proposal)
                                .FirstOrDefault(r => r.RealEstateId == id);
            // realEstate.TechnicalCharcNames = "";

            var myStream = new MemoryStream();

            Document document = new Document();
            document.Info.Title = "Объектлар паспорти";
            document.Info.Author = "Активларни бошқариш департаменти, Узбекнефтегаз АЖ";

            #region Define styles in the document

            Style style = document.Styles["Normal"];
            style.Font.Name = "Times New Roman";

            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal

            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal

            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;

            #endregion

            #region Define the cover page
            Section section = document.AddSection();

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = "3cm";

            MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage("wwwroot/assets/images/logo/small-logo.png");
            image.Width = "10cm";

            paragraph = section.AddParagraph("Танланган объектлар паспорт маълумотларини кўрсатувчи ҳужжат");
            paragraph.Format.Font.Size = 16;
            paragraph.Format.Font.Color = Colors.DarkRed;
            paragraph.Format.SpaceBefore = "5cm";
            paragraph.Format.SpaceAfter = "3cm";

            paragraph = section.AddParagraph("Ҳужжат шакллантирилган сана: ");
            paragraph.AddDateField();

            #endregion

            #region Defines page setup, headers, and footers.
            Section section2 = document.AddSection();
            section2.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section2.PageSetup.StartingNumber = 1;
            section2.PageSetup = document.DefaultPageSetup.Clone();
            section2.PageSetup.PageFormat = PageFormat.A3;
            section2.PageSetup.Orientation = Orientation.Landscape;

            //HeaderFooter header = section2.Headers.Primary;
            //header.AddParagraph("\tOdd Page Header");

            //header = section2.Headers.EvenPage;
            //header.AddParagraph("Even Page Header");

            //Create a paragraph with centered page number. See definition of style "Footer".
            Paragraph paragraph2 = new Paragraph();
            paragraph2.AddTab();
            paragraph2.AddPageField();

            //Add paragraph to footer for odd pages.
            section2.Footers.Primary.Add(paragraph2);

            //Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            //not belong to more than one other object. If you forget cloning an exception is thrown.
            section2.Footers.EvenPage.Add(paragraph2.Clone());

            #endregion

            #region Define tables

            //document.LastSection.AddParagraph("Cell Merge", "Heading2");

            List<string> InfraNamesOn = new();

            var infrastuctures = _context.Infrastuctures.ToList();

            List<string> allInfraNames = new();

            foreach (var infra in infrastuctures)
            {
                allInfraNames.Add(infra.InfrastructureName);
            }

            IEnumerable<string> InfraNamesOff;

            List<string> InfraResults = new();


            InfraNamesOn.Clear();
            InfraNamesOff = Enumerable.Empty<string>();
            InfraResults.Clear();

            var realEstateInfras = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == realEstate.RealEstateId).ToList();

            foreach (var r in realEstateInfras)
            {
                var infrastructure = await _context.Infrastuctures.FindAsync(r.InfrastuctureId);
                InfraNamesOn.Add(infrastructure.InfrastructureName);
            }

            InfraNamesOff = allInfraNames.Except(InfraNamesOn);

            foreach (var infra in InfraNamesOn)
            {
                InfraResults.Add(infra);
                InfraResults.Add("Мавжуд");
            }

            foreach (var infra in InfraNamesOff)
            {
                InfraResults.Add(infra);
                InfraResults.Add("Мавжуд эмас");
            }

            Paragraph paragraph3 = document.LastSection.AddParagraph(realEstate.RealEstateName, "Heading1");
            paragraph3.AddBookmark(realEstate.RealEstateName);

            Table table3 = document.LastSection.AddTable();
            table3.Borders.Visible = true;
            table3.TopPadding = 2;
            table3.BottomPadding = 2;

            Column column3 = table3.AddColumn(Unit.FromCentimeter(5));
            column3.Format.Alignment = ParagraphAlignment.Left;

            column3 = table3.AddColumn(Unit.FromCentimeter(12));
            column3.Format.Alignment = ParagraphAlignment.Left;

            column3 = table3.AddColumn(Unit.FromCentimeter(10));
            column3.Format.Alignment = ParagraphAlignment.Center;

            table3.Rows.Height = 28;

            Row row3 = table3.AddRow();
            row3.Format.Font.Bold = true;
            row3.Cells[0].AddParagraph("Объект Паспорти");

            row3.Cells[0].MergeRight = 1;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            string link1 = "wwwroot" + realEstate.PhotoOfObjectLink1;
            MigraDoc.DocumentObjectModel.Shapes.Image image1 = row3.Cells[2].AddImage(link1);
            image1.Height = "5cm";
            image1.Width = "9.8cm";
            image1.LockAspectRatio = true;
            row3.Cells[2].MergeDown = 4;

            row3 = table3.AddRow();
            row3.VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[0].AddParagraph("Объект номи");
            row3.Cells[1].AddParagraph(realEstate.RealEstateName);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Жойлашган манзили");
            row3.Cells[1].AddParagraph(realEstate.Address);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Баланс сақловчи ташкилот");
            row3.Cells[1].AddParagraph(realEstate.AssetHolderName);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Умумий майдон (кв.м.)");
            row3.Cells[1].AddParagraph(realEstate.FullArea);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Бино иншоот майдони (кв.м.)");
            row3.Cells[1].AddParagraph(realEstate.BuildingArea);
            string link2 = "wwwroot" + realEstate.PhotoOfObjectLink2;
            MigraDoc.DocumentObjectModel.Shapes.Image image2 = row3.Cells[2].AddImage(link2);
            image2.Height = "5cm";
            image2.Width = "9.8cm";
            image2.LockAspectRatio = true;
            row3.Cells[2].MergeDown = 4;

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Ўртача йиллик сақлаш харажатлари (минг сўм)");
            row3.Cells[1].AddParagraph(realEstate.MaintenanceCostForYear);


            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Инфратузилманинг мавжудлиги");
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[0].MergeRight = 1;
            row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph(InfraResults[0]);
            row3.Cells[1].AddParagraph(InfraResults[1]);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph(InfraResults[2]);
            row3.Cells[1].AddParagraph(InfraResults[3]);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph(InfraResults[4]);
            row3.Cells[1].AddParagraph(InfraResults[5]);
            string link3 = "wwwroot" + realEstate.PhotoOfObjectLink3;
            MigraDoc.DocumentObjectModel.Shapes.Image image3 = row3.Cells[2].AddImage(link3);
            image3.Height = "5cm";
            image3.Width = "9.8cm";
            image3.LockAspectRatio = true;
            row3.Cells[2].MergeDown = 4;

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph(InfraResults[6]);
            row3.Cells[1].AddParagraph(InfraResults[7]);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph(InfraResults[8]);
            row3.Cells[1].AddParagraph(InfraResults[9]);

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Активдан янада самарали фойдаланиш бўйича таклифлар");
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[0].MergeRight = 1;
            row3.Cells[0].VerticalAlignment = VerticalAlignment.Center;

            row3 = table3.AddRow();
            row3.Cells[0].AddParagraph("Активдан янада самарали фойдаланиш бўйича таклиф");
            row3.Cells[1].AddParagraph(realEstate.Proposal.ProposalName);

            #endregion

            document.UseCmykColor = true;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            pdfRenderer.Document = document;

            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(myStream);


            var content = myStream.ToArray();
            return File(
                content,
                "application/pdf",
                "Passport.pdf"
                );

        }

        [HttpGet]
        public async Task<IActionResult> GetConfirmed()
        {
            var realEstates = _context.RealEstates
                .Include(r => r.ApplicationUser)
                .Include(r => r.DistrictOfObject)
                .Include(r => r.Proposal)
                .Include(r => r.RegionOfObject)
                .Where(r => r.Confirmed == true )
                .ToList();

            if (!realEstates.Any())
            {
               return Json(new EmptyResult());
            }

            float number;

            List<RealEstateShortDto> viewModels = new();
            try
            {
                foreach (var item in realEstates)
                {
                    item.CadasterRegDateStr = item.CadastreRegDate.ToShortDateString();
                    item.CommisioningDateStr = item.CommisioningDate.ToShortDateString();

                    #region comment   
                    //if (float.TryParse(item.BuildingArea, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    //{
                    //    item.BuildingArea = Math.Round(Decimal.Parse(item.BuildingArea, CultureInfo.InvariantCulture.NumberFormat), 3, MidpointRounding.AwayFromZero).ToString();

                    //}

                    //if (float.TryParse(item.FullArea, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    //{
                    //    item.FullArea = Math.Round(Decimal.Parse(item.FullArea, CultureInfo.InvariantCulture.NumberFormat), 3, MidpointRounding.AwayFromZero).ToString();

                    //}

                    //if (float.TryParse(item.MaintenanceCostForYear, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    //{
                    //    item.MaintenanceCostForYear = Math.Round(Decimal.Parse(item.MaintenanceCostForYear, CultureInfo.InvariantCulture.NumberFormat), 3, MidpointRounding.AwayFromZero).ToString();
                    //}

                    //if (float.TryParse(item.InitialCostOfObject, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    //{
                    //    item.InitialCostOfObject = Math.Round(Decimal.Parse(item.InitialCostOfObject, CultureInfo.InvariantCulture.NumberFormat), 3, MidpointRounding.AwayFromZero).ToString();
                    //}

                    //if (float.TryParse(item.Wear, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    //{
                    //    item.Wear = Math.Round(Decimal.Parse(item.Wear, CultureInfo.InvariantCulture.NumberFormat), 3, MidpointRounding.AwayFromZero).ToString();
                    //}

                    //if (float.TryParse(item.ResidualValueOfObject, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    //{
                    //    item.ResidualValueOfObject = Math.Round(Decimal.Parse(item.ResidualValueOfObject, CultureInfo.InvariantCulture.NumberFormat), 3, MidpointRounding.AwayFromZero).ToString();
                    //}


                    //item.InfrastructureNames = "";
                    ////item.TechnicalCharcNames = "";
                    //var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == item.RealEstateId).ToList();
                    //// var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == item.RealEstateId).ToList();

                    //foreach (var temp in realEstateInfrastructures)
                    //{
                    //    var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();

                    //    item.InfrastructureNames += infrastructure.InfrastructureName + "; ";

                    //}

                    //var user = _context.ApplicationUsers.Include(a => a.Organization).FirstOrDefault(u => u.Id == item.ApplicationUserId);
                    #endregion

                    RealEstateShortDto viewModel = new()
                    {
                        RealEstateId = item.RealEstateId,
                        RealEstateName = item.RealEstateName,
                        CadastreNumber = item.CadastreNumber,
                        CadastreRegDate = item.CadasterRegDateStr,
                        CommisioningDate = item.CommisioningDateStr,
                        Activity = item.Activity,
                        Region = item.RegionOfObject.RegionName,
                        AssetHolderName = item.AssetHolderName,
                        CadastreFileLink = item.CadastreFileLink,
                        Photo1Link = item.PhotoOfObjectLink1,
                        Photo2Link = item.PhotoOfObjectLink2,
                        Photo3Link = item.PhotoOfObjectLink3,
                    };

                    viewModels.Add(viewModel);


                }
            }
            catch(Exception ex)
            {
                var  message = ex.Message;
            }
            

            await _context.SaveChangesAsync();

            return Json(new { data = viewModels });
        }

        [HttpPost]
        public IActionResult GetUser([FromBody] int id)
        {
            var user = _context.ApplicationUsers.Include(u => u.Organization).Include(u => u.Shares).Include(u => u.RealEstates).FirstOrDefault(u => u.RealEstates.Any(r => r.RealEstateId == id) || u.Shares.Any(r=> r.ShareId == id));
            
            if(user == null)
            {
                return Json(new { success = false, message ="Масъул шахс топилмади!" });
            }

            var realEstates = _context.RealEstates.Where(r => r.ApplicationUserId.Equals(user.Id) && r.Confirmed);
            var realEstateCount = realEstates.Count();
            var shares = _context.Shares.Where(s => s.ApplicationUserId.Equals(user.Id) && s.Confirmed);
            var shareCount = shares.Count();
            var soldCount = realEstates.Where(r => !r.Status).Count() +
                            shares.Where(s => !s.Status).Count();
            
            UserViewModel userViewModel = new()
            {
                FullName = user.Fullname,
                Position = user.Postion,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                OrganizationName = user.Organization.OrganizationName,
                RealEstateCount = realEstateCount,
                ShareCount = shareCount,
                SoldAssetCount = soldCount
            };

            return Json(new {data = userViewModel, success = true });
        }

        [HttpPost]
        public IActionResult GetRealEstate([FromBody] JObject data)
        {
            if(data["id"] == null || data["forDetails"] == null)
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
            //var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == realEstate.RealEstateId).ToList();

            foreach (var temp in realEstateInfrastructures)
            {
                var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();

                realEstate.InfrastructureNames += infrastructure.InfrastructureName + "; ";

            }

            List<RealEstate> realEstateData = new()
            {
                realEstate
            };

            if(forDetails)
                return Json(new { data = realEstateData});

            return Json(new { data = realEstate, success = true});
        }

        [HttpPost]
        public IActionResult GetFilteredData([FromBody] JObject data)
        {
            if (data["orgId"] == null)
            {
                return Json(new {});
            }

            int orgId = (int)data["orgId"];
            int regionId = (int)data["regionId"];
            bool onBalance = (bool)data["balance"];
            bool offBalance = (bool)data["notBalance"];

            List<RealEstateShortDto> filteredData = new();
            List<RealEstate> filteredAssets = new();
            
            List<RealEstate> rOnBalance = new();
            List<RealEstate> rOffBalance = new();

            var realEstates = _context.RealEstates
            .Include(r => r.ApplicationUser)
            .ThenInclude(a => a.Organization)
            .Include(r => r.DistrictOfObject)
            .Include(r => r.Proposal)
            .Include(r => r.RegionOfObject)
            .Where(r => r.Confirmed == true );

               
           
           if(orgId == 0 && regionId != 0)
            {

                realEstates = _context.RealEstates
                    .Include(r => r.ApplicationUser)
                    .ThenInclude(a => a.Organization)
                    .Include(r => r.DistrictOfObject)
                    .Include(r => r.Proposal)
                    .Include(r => r.RegionOfObject)
                    .Where(r => r.Confirmed == true && r.RegionId == regionId );
            }

            else if (orgId != 0 && regionId == 0)
            {
                string name = _context.Organizations.FirstOrDefault(o => o.OrganizationId == orgId).OrganizationName;

                realEstates = _context.RealEstates
                    .Include(r => r.ApplicationUser)
                    .ThenInclude(a => a.Organization)
                    .Include(r => r.DistrictOfObject)
                    .Include(r => r.Proposal)
                    .Include(r => r.RegionOfObject)
                    .Where(r => r.Confirmed == true && r.AssetHolderName.Equals(name) );
            }

            else if (orgId != 0 && regionId != 0)
            {
                string name = _context.Organizations.FirstOrDefault(o => o.OrganizationId == orgId).OrganizationName;

                realEstates = _context.RealEstates
                    .Include(r => r.ApplicationUser)
                    .ThenInclude(a => a.Organization)
                    .Include(r => r.DistrictOfObject)
                    .Include(r => r.Proposal)
                    .Include(r => r.RegionOfObject)
                    .Where(r => r.Confirmed == true && r.RegionId == regionId && r.AssetHolderName.Equals(name) );
            }

            if (onBalance)
            {
                rOnBalance = realEstates.Where(r => r.Status == true).ToList();
            }

            if (offBalance)
            {
                rOffBalance = realEstates.Where((r) => r.Status == false).ToList();
            }

            rOnBalance.AddRange(rOffBalance);
            filteredAssets.AddRange(rOnBalance);


            if (!filteredAssets.Any())
            {
                return Json(new {data=filteredData});
            }

            foreach (var item in filteredAssets)
            {
                item.CadasterRegDateStr = item.CadastreRegDate.ToShortDateString();
                item.CommisioningDateStr = item.CommisioningDate.ToShortDateString();

                item.InfrastructureNames = "";
                //  item.TechnicalCharcNames = "";
                var realEstateInfrastructures = _context.RRealEstateInfrastructures.Where(r => r.RealEstateId == item.RealEstateId).ToList();
                // var realEstateTechnicalCharcs = _context.RealEstateTechnicalCharcs.Where(r => r.RealEstateId == item.RealEstateId).ToList();

                foreach (var temp in realEstateInfrastructures)
                {
                    var infrastructure = _context.Infrastuctures.Where(i => i.InfrastructureId == temp.InfrastuctureId).FirstOrDefault();

                    item.InfrastructureNames += infrastructure.InfrastructureName + "; ";

                }
                
                RealEstateShortDto viewModel = new()
                {
                    RealEstateId = item.RealEstateId,
                    RealEstateName = item.RealEstateName,
                    CadastreNumber = item.CadastreNumber,
                    CadastreRegDate = item.CadasterRegDateStr,
                    CommisioningDate = item.CommisioningDateStr,
                    Activity = item.Activity,
                    Region = item.RegionOfObject.RegionName,
                    AssetHolderName = item.AssetHolderName,
                    CadastreFileLink = item.CadastreFileLink,
                    Photo1Link = item.PhotoOfObjectLink1,
                    Photo2Link = item.PhotoOfObjectLink2,
                    Photo3Link = item.PhotoOfObjectLink3,
                };

                filteredData.Add(viewModel);

            }

            return Json(new { data = filteredData });
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(int id)
        {
            var districts = await _context.Districts.Where(d => d.RegionId == id).ToListAsync();
            var districtList = new SelectList(districts, "DistrictId", "DistrictName");

            return new JsonResult(districtList);
        }
        public async Task<IActionResult> GetExcel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var realEstate = await _context.RealEstates
                .Include(r => r.ApplicationUser)
                .Include(r => r.DistrictOfObject)
                .Include(r => r.Proposal)
                .Include(r => r.RegionOfObject)
                .Include(r => r.TransferredAsset)
                .FirstOrDefaultAsync(m => m.RealEstateId == id);
            if (realEstate == null)
            {
                return NotFound();
            }

            return View(realEstate);
        }

        // GET: Admin/RealEstates/Create
        

        // POST: Admin/RealEstates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        // GET: Admin/RealEstates/Edit/5
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
            //List<SelectListItem> tech = new();
            //List<SelectListItem> techn = new();
            //List<TechnicalCharc> info = new();

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
        public async Task<IActionResult> Edit(int id, int RegionId,int DistrictId, List<int> Infrastructures,  List<SelectListItem> empl, List<SelectListItem> elmp, 
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
            if (_realEstate == null)
                return NotFound();
            //realEstate.RegionId = RegionId;
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
                if (_realEstate.RealEstateName != realEstate.RealEstateName)
                    _realEstate.RealEstateName = realEstate.RealEstateName;
                if (_realEstate.CadastreRegDate != realEstate.CadastreRegDate)
                    _realEstate.CadastreRegDate = realEstate.CadastreRegDate;
                if (_realEstate.CadastreNumber != realEstate.CadastreNumber)
                    _realEstate.CadastreNumber = realEstate.CadastreNumber;
                if (_realEstate.CommisioningDate != realEstate.CommisioningDate)
                    _realEstate.CommisioningDate = realEstate.CommisioningDate;
                if (_realEstate.Activity != realEstate.Activity)
                    _realEstate.Activity = realEstate.Activity;
                if (_realEstate.AssetHolderName != realEstate.AssetHolderName)
                    _realEstate.AssetHolderName = realEstate.AssetHolderName;
                if (_realEstate.RegionId != RegionId)
                    _realEstate.RegionId = RegionId;
                if (_realEstate.DistrictId != DistrictId)
                    _realEstate.DistrictId = DistrictId;
                if (_realEstate.Address != realEstate.Address)
                    _realEstate.Address = realEstate.Address;
                if (_realEstate.BuildingArea != realEstate.BuildingArea)
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
                if (_realEstate.ProposalId != realEstate.ProposalId)
                    _realEstate.ProposalId = realEstate.ProposalId;
                if (_realEstate.Comment != realEstate.Comment)
                    _realEstate.Comment = realEstate.Comment;

                try
                {
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstate.RealEstateName);

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

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
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
            _context.SaveChanges();

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


                    if (finder == 0)
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
                            using (var img = System.Drawing.Image.FromStream(memoryStream))
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

                    await _context.SaveChangesAsync(_userManager.GetUserId(User));

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
                            using (var img = System.Drawing.Image.FromStream(memoryStream))
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
                            using (var img = System.Drawing.Image.FromStream(memoryStream))
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
                            using (var img = System.Drawing.Image.FromStream(memoryStream))
                            {
                                var image = ResizeImage(img, 370, 180);
                                var createdFile = UploadImage(realEstateId, image, file.FileName, file.ContentType);
                                real_Estate.PhotoOfObject3Id = createdFile.Result.FileId;
                                real_Estate.PhotoOfObjectLink3 = createdFile.Result.SystemPath;
                            }
                        }

                    }
                    await _context.SaveChangesAsync(_userManager.GetUserId(User));

                }
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex);
            }

            return RedirectToAction("Edit", new { id = realEstateId });

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var realEstate = await _context.RealEstates.FindAsync(id);

            if (realEstate == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }
            var realEstateName = realEstate.RealEstateName;

            var fileModels = _context.FileModels.Where(f => f.RealEstateId == id);
            var transferredAsset = _context.TransferredAssets.Where(t => t.AssetId == t.RealEstate.TransferredAssetId).ToList();
            var assetEvaluations = _context.AssetEvaluations.Where(a => a.RealEstateId == id).ToList();
            var submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.RealEstateId == id).ToList();
            var reductionInAssets = _context.ReductionInAssets.Where(s => s.RealEstateId == id).ToList();
            var oneTimePaymentAssets = _context.OneTimePaymentAssets.Include(s => s.Step2).Include(s => s.Step3).Where(s => s.RealEstateId == id).ToList();
            var installmentAssets = _context.InstallmentAssets.Include(s => s.Step2).Where(s => s.RealEstateId == id).ToList();

            try
            {
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

                    }

                }

                if (submissionOnBiddings.Any())
                {
                    foreach (var item in submissionOnBiddings)
                    {

                        _context.SubmissionOnBiddings.Remove(item);

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

                        var oneTimePaymentStep2 = item.Step2;

                        if(oneTimePaymentStep2 != null)
                        {
                            var step2File = await _context.FileModels.FirstOrDefaultAsync(s => s.OneTimePaymentStep2Id == oneTimePaymentStep2.OneTimePaymentStep2Id);

                            if (step2File != null)
                            {

                                if (System.IO.File.Exists(step2File.FilePath))
                                {
                                    System.IO.File.Delete(step2File.FilePath);
                                }

                                _context.FileModels.Remove(step2File);

                            }

                            _context.OneTimePaymentStep2.Remove(oneTimePaymentStep2);
                            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);
                        }
                        
                        var oneTimePaymentStep3 = item.Step3;

                        if(oneTimePaymentStep3 != null)
                        {
                            var step3Files = await _context.FileModels.Where(s => s.OneTimePaymentStep3Id == oneTimePaymentStep3.OneTimePaymentStep3Id).ToListAsync();

                            if (step3Files.Any())
                            {
                                foreach (var file in step3Files)
                                {
                                    if (System.IO.File.Exists(file.FilePath))
                                    {
                                        System.IO.File.Delete(file.FilePath);
                                    }

                                    _context.FileModels.Remove(file);

                                }
                            }

                            _context.OneTimePaymentStep3.Remove(oneTimePaymentStep3);
                            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);
                        }

                        _context.OneTimePaymentAssets.Remove(item);


                    }


                }

                if (installmentAssets.Any())
                {
                    foreach (var item in installmentAssets)
                    {

                        var installmentFiles = _context.FileModels.Where(f => f.InstallmentAssetId == item.InstallmentAssetId);

                        if (installmentFiles.Any())
                        {
                            foreach (var item2 in installmentFiles)
                            {
                                if (System.IO.File.Exists(item2.FilePath))
                                {
                                    System.IO.File.Delete(item2.FilePath);
                                }

                                _context.FileModels.Remove(item2);

                            }
                        }

                        var installmentStep2 = item.Step2;

                        if (installmentStep2 != null)
                        {
                            var step2Files = await _context.FileModels.Where(s => s.InstallmentStep2Id == installmentStep2.InstallmentStep2Id).ToListAsync();
                            if (step2Files.Any())
                            {
                                foreach(var file in step2Files)
                                {
                                    if (System.IO.File.Exists(file.FilePath))
                                    {
                                        System.IO.File.Delete(file.FilePath);
                                    }

                                    _context.FileModels.Remove(file);
                                }
                            }

                            _context.InstallmentStep2.Remove(installmentStep2);
                            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);
                        }

                        _context.InstallmentAssets.Remove(item);                       

                    }

                }


                _context.RealEstates.Remove(realEstate);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);

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

                            }

                        }

                        _context.TransferredAssets.Remove(item);
                        await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);

                    }

                }
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
            

            return Json(new { success = true, message = "Ўчирилди" });

        }

        [HttpPost]
        public async Task<IActionResult> DeleteByOrganization ([FromBody] int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Хатолик - Ташкилот ID сини юборишда хатолик юз берди!" });
            }

            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
                return Json(new { success = false, message = "Хатолик - Ташкилот топилмади!" });

            var realEstates =await _context.RealEstates.Where(r => r.AssetHolderName.Equals(organization.OrganizationName)).ToListAsync();

            if(realEstates.Count == 0)
            {
                return Json(new { success = false, message = $"{organization.OrganizationName} га боғланган объектлар топилмади!" });
            }

            try
            {
                foreach (var item in realEstates)
                {
                    await Delete(item.RealEstateId);
                }
            }
            catch(Exception ex)
            {
                return Json(new {success = false, message= ex.Message});
            }          

            return Json(new { success = true, message = $"{organization.OrganizationName} га боғланган объектлар муваффақиятли ўчирилди!" });
        }

        public async Task<IActionResult> DeleteByOne(int id)
        {
            var realEstate = await _context.RealEstates.FindAsync(id);

            if (realEstate == null)
            {
                return Json(new { success = false, message = "Хатолик! Объект Топилмади!" });
            }
            var realEstateName = realEstate.RealEstateName;

            var fileModels = _context.FileModels.Where(f => f.RealEstateId == id);
            var transferredAsset = _context.TransferredAssets.Where(t => t.AssetId == t.RealEstate.TransferredAssetId).ToList();
            var assetEvaluations = _context.AssetEvaluations.Where(a => a.RealEstateId == id).ToList();
            var submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.RealEstateId == id).ToList();
            var reductionInAssets = _context.ReductionInAssets.Where(s => s.RealEstateId == id).ToList();
            var oneTimePaymentAssets = _context.OneTimePaymentAssets.Include(s => s.Step2).Include(s => s.Step3).Where(s => s.RealEstateId == id).ToList();
            var installmentAssets = _context.InstallmentAssets.Include(s => s.Step2).Where(s => s.RealEstateId == id).ToList();

            try
            {
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

                    }

                }

                if (submissionOnBiddings.Any())
                {
                    foreach (var item in submissionOnBiddings)
                    {

                        _context.SubmissionOnBiddings.Remove(item);

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

                        var oneTimePaymentStep2 = item.Step2;

                        if (oneTimePaymentStep2 != null)
                        {
                            var step2File = await _context.FileModels.FirstOrDefaultAsync(s => s.OneTimePaymentStep2Id == oneTimePaymentStep2.OneTimePaymentStep2Id);

                            if (step2File != null)
                            {

                                if (System.IO.File.Exists(step2File.FilePath))
                                {
                                    System.IO.File.Delete(step2File.FilePath);
                                }

                                _context.FileModels.Remove(step2File);

                            }

                            _context.OneTimePaymentStep2.Remove(oneTimePaymentStep2);
                            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);
                        }

                        var oneTimePaymentStep3 = item.Step3;

                        if (oneTimePaymentStep3 != null)
                        {
                            var step3Files = await _context.FileModels.Where(s => s.OneTimePaymentStep3Id == oneTimePaymentStep3.OneTimePaymentStep3Id).ToListAsync();

                            if (step3Files.Any())
                            {
                                foreach (var file in step3Files)
                                {
                                    if (System.IO.File.Exists(file.FilePath))
                                    {
                                        System.IO.File.Delete(file.FilePath);
                                    }

                                    _context.FileModels.Remove(file);

                                }
                            }

                            _context.OneTimePaymentStep3.Remove(oneTimePaymentStep3);
                            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);
                        }

                        _context.OneTimePaymentAssets.Remove(item);


                    }


                }

                if (installmentAssets.Any())
                {
                    foreach (var item in installmentAssets)
                    {

                        var installmentFiles = _context.FileModels.Where(f => f.InstallmentAssetId == item.InstallmentAssetId);

                        if (installmentFiles.Any())
                        {
                            foreach (var item2 in installmentFiles)
                            {
                                if (System.IO.File.Exists(item2.FilePath))
                                {
                                    System.IO.File.Delete(item2.FilePath);
                                }

                                _context.FileModels.Remove(item2);

                            }
                        }

                        var installmentStep2 = item.Step2;

                        if (installmentStep2 != null)
                        {
                            var step2Files = await _context.FileModels.Where(s => s.InstallmentStep2Id == installmentStep2.InstallmentStep2Id).ToListAsync();
                            if (step2Files.Any())
                            {
                                foreach (var file in step2Files)
                                {
                                    if (System.IO.File.Exists(file.FilePath))
                                    {
                                        System.IO.File.Delete(file.FilePath);
                                    }

                                    _context.FileModels.Remove(file);
                                }
                            }

                            _context.InstallmentStep2.Remove(installmentStep2);
                            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);
                        }

                        _context.InstallmentAssets.Remove(item);

                    }

                }


                _context.RealEstates.Remove(realEstate);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);

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

                            }

                        }

                        _context.TransferredAssets.Remove(item);
                        await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);

                    }

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }


            return Json(new { success = true, message = "Ўчирилди" });

        }
        private bool RealEstateExists(int id)
        {
            return _context.RealEstates.Any(e => e.RealEstateId == id);
        }
    }
}
