using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using ClosedXML.Excel;
using System.IO;
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;
using ControlActive.ViewModels;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Http;


namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
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
        // GET: Admin/Shares
        public ActionResult Index(bool success = false, bool editSuccess = false)
        {

            ViewBag.Success = success;
            ViewBag.EditSuccess = editSuccess;
            ViewBag.Organizations = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName");
            ViewBag.Regions = new SelectList(_context.Regions, "RegionId", "RegionName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExcelReport(int? orgId, string balance, string notBalance, int? region, DateTime chosenDate,
                                                        string notBalance_block2, string notBalance_block6, string notBalance_block7,
                                                            string balance_block1, string balance_block4,
                                                                string block1, string block2, string block3, string block4, string block5, string block6, string block7, string block8,
                                                                    string bNameB1, string idRegNumberB1, string parentOrgB1, string activityB1, string activityShareB1, string regionB1, string districtB1, string adressB1, string foundationYB1, string stateRegDateB1, string authCapB1, string shareHolderB1, string amountFromAuthCapB1, string shareOfAuthCapB1, string numberOfSharesB1, string parValueShareB1, string administStaffB1, string productPersB1, string averageMSalB1, string maintCostB1, string pAreaB1, string bAreaB1, string amountPayB1, string amountRecB1, string plYearsB1, string commentB1,
                                                                        string trFormB2, string orgNameB2, string sNumberB2, string sDateB2, string orgNameAssetB2, string tCostB2, string vatB2, string actDateB2, string actNumberB2, string agDateB2, string agNumberB2,
                                                                            string evOrgNameB3, string rDateB3, string rRegNumberB3, string mValueB3, string examOrgNameB3, string examRDateB3, string examRNumberB3, string rStatusB3,
                                                                                string tradePNameB4, string amountBidB4, string biddingExpDateB4, string expoTimeB4, string activeValB4, string bidHoldDateB4,
                                                                                    string gBodyNameB5, string sDateB5, string sNumberB5, string percentageB5, string amountB5, string numberStepsB5, string assetValDecB5,
                                                                                        string gBodyNameB6, string sDateB6, string sNumberB6, string bidDateB6, string assetBNameB6, string amountAssetSoldB6, string agDateB6, string agNumberB6, string amountPayedB6, string percentB6, string actDateB6, string actNumberB6,
                                                                                            string gBodyNameB7, string sDateB7, string sNumberB7, string bidDateB7, string assetBNameB7, string amountAssetB7, string agDateB7, string agNumB7, string instTimeB7, string minInitPB7, string actInitPercentB7, string paymentPeriodB7, string scheduleAmountB7, string actPaymentB7, string diffB7, string actDateB7, string actNumB7,
                                                                                                string fNameB8, string positionB8, string phNumberB8, string emailB8)
        {
            if (chosenDate == DateTime.MinValue)
            {
                chosenDate = DateTime.Now;
            }

            int End;

            int sumRow;

            int number = 1;

            #region block1 values to calculate
            float productionArea = 0;

            float buildingArea = 0;

            int numberOfEmployees = 0;

            float maintanenceCostForYear = 0;

            float initialCostOfObject = 0;

            float wear = 0;

            float residualValueOfObject = 0;
            #endregion

            #region block2 values to calculate
            float totalCost = 0;

            double vat = 0;
            #endregion

            #region block3 values to calculate
            float marketvalue = 0;
            #endregion

            #region block4 values to calculate
            int AmountOnBidding = 0;

            float ActiveValue = 0;
            #endregion

            #region block5 values to calculate
            float Amount = 0;

            float AssetValueAfterDecline = 0;
            #endregion

            #region block6 values to calculate
            float amountOfAssetSold1 = 0;

            float AmountPayed = 0;
            #endregion

            #region block7 values to calculate
            float amountOfAssetSold2 = 0;

            float actualInitPayment = 0;

            float scheduledAmount = 0;

            float actualPayment = 0;

            float difference = 0;

            #endregion

            List<Share> Shares = new();

            if (orgId == 0)
            {
                if (balance != null && notBalance != null)
                {
                    Shares = await _context.Shares
                      .Include(r => r.ApplicationUser)
                      .ThenInclude(a => a.Organization)
                      .Include(r => r.DistrictOfObject)
                      .Include(r => r.RegionOfObject)
                      .Include(r => r.Shareholders)
                      .Include(r => r.AssetEvaluations)
                      .Where(i => i.Confirmed == true)
                      .ToListAsync();

                }

                if (balance != null && notBalance == null)
                {
                    if (balance_block1 != null && balance_block4 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Where(i => i.Confirmed == true && i.Status == true)
                                      .ToListAsync();
                    }


                    if (balance_block1 != null && balance_block4 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.Status == true && !i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }

                    if (balance_block1 == null && balance_block4 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.Status == true && i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }
                }

                if (balance == null && notBalance != null)
                {
                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Where(i => i.Confirmed == true && i.Status == false)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset == null && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && i.TransferredAsset != null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.Status == false && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
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
                    Shares = await _context.Shares
                      .Include(r => r.ApplicationUser)
                      .ThenInclude(a => a.Organization)
                      .Include(r => r.DistrictOfObject)
                      .Include(r => r.RegionOfObject)
                      .Include(r => r.Shareholders)
                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name))
                      .ToListAsync();
                }

                if (balance != null && notBalance == null)
                {
                    if (balance_block1 != null && balance_block4 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == true)
                                      .ToListAsync();
                    }


                    if (balance_block1 != null && balance_block4 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == true && !i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }

                    if (balance_block1 == null && balance_block4 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == true && i.SubmissionOnBiddings.Any())
                                      .ToListAsync();
                    }
                }

                if (balance == null && notBalance != null)
                {
                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false && i.TransferredAsset == null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false && i.TransferredAsset == null && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 == null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false && i.TransferredAsset == null && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false && i.TransferredAsset != null)
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 != null && notBalance_block7 == null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false && !i.InstallmentAssets.Any())
                                      .ToListAsync();
                    }

                    if (notBalance_block2 != null && notBalance_block6 == null && notBalance_block7 != null)
                    {
                        Shares = await _context.Shares
                                      .Include(r => r.ApplicationUser)
                                      .ThenInclude(a => a.Organization)
                                      .Include(r => r.DistrictOfObject)
                                      .Include(r => r.RegionOfObject)
                                      .Include(r => r.Shareholders)
                                      .Include(r => r.SubmissionOnBiddings)
                                      .Include(r => r.TransferredAsset)
                                      .Include(r => r.OneTimePaymentAssets)
                                      .Include(r => r.InstallmentAssets)
                                      .Where(i => i.Confirmed == true && i.ParentOrganization.Equals(name) && i.Status == false && !i.OneTimePaymentAssets.Any())
                                      .ToListAsync();
                    }
                }
            }

            if (region != 0)
            {
                Shares = Shares.Where(r => r.RegionId == region).ToList();
            }            

            IXLWorkbook workbook = new XLWorkbook();
           
            IXLWorksheet sheet = workbook.Worksheets.Add("Улушлар");

            #region Create headers for table

            int i = 1;

            sheet.Cell(5, i).Value = "№";
            i++;

            sheet.Cell(5, i).Value = "Ташкилот номи";
            i++;

            if (block1 != null)
            {
                var  A = sheet.Cell(4, i - 1).Address;

                #region Block 1

                if (idRegNumberB1 != null)
                {
                    sheet.Cell(5, i).Value = "Идентификация регистр рақами";
                    i++;
                }

                if (parentOrgB1 != null)
                {

                    sheet.Cell(5, i).Value = "Олий даражадаги ташкилот ёки соҳага аъзолик";
                    i++;
                }

                if (activityB1 != null)
                {
                    sheet.Cell(5, i).Value = "Хўжалик юритувчи субъектнинг асосий фаолияти";
                    i++;
                }

                if (activityShareB1 != null)
                {
                    sheet.Cell(5, i).Value = "Асосий фаолият улуши";
                    i++;
                }

                if (foundationYB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ташкилот ташкил этилган йили";
                    i++;
                }

                if (stateRegDateB1 != null)
                {
                    sheet.Cell(5, i).Value = "Давлат рўйхатидан ўтказилган сана";
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

                if (authCapB1 != null)
                {
                    sheet.Cell(5, i).Value = "Устав капитали (минг сўм)";
                    i++;
                }

                if (shareHolderB1 != null)
                {
                    sheet.Cell(5, i).Value = "Аксиядор / Иштирокчи";
                    i++;
                }

                if (amountFromAuthCapB1 != null)
                {
                    sheet.Cell(5, i).Value = "Устав капиталининг миқдори (минг сўм)";
                    i++;
                }

                if (numberOfSharesB1 != null)
                {
                    sheet.Cell(5, i).Value = "Акциялар сони (дона)";
                    i++;
                }

                if (parValueShareB1 != null)
                {
                    sheet.Cell(5, i).Value = "Аксияларнинг номинал қиймати (сЎм)";
                    i++;
                }

                if (administStaffB1 != null)
                {
                    sheet.Cell(5, i).Value = "Маъмурий ходимлар";
                    i++;
                }

                if (productPersB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ишлаб чиқариш ходимлари";
                    i++;
                }

                if (pAreaB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ишлаб чиқариш майдони (кв.м.)";
                    i++;
                }

                if (bAreaB1 != null)
                {
                    sheet.Cell(5, i).Value = "Бинолар иншоотлар (кв.м.)";
                    i++;
                }

                if (maintCostB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ўртача йиллик сақлаш харажати (сўм)";
                    i++;
                }

                if (averageMSalB1 != null)
                {
                    sheet.Cell(5, i).Value = "Ўртача ойлик иш ҳақи (сўм)";
                    i++;
                }

                if (amountPayB1 != null)
                {
                    sheet.Cell(5, i).Value = "Охирги ҳисобот санасидаги кредиторлик қарзлари суммаси (сўм)";
                    i++;
                }

                if (amountRecB1 != null)
                {
                    sheet.Cell(5, i).Value = "Охирги ҳисобот санаси бўйича дебиторлик қарзлари суммаси (сўм)";
                    i++;
                }

                if (plYearsB1 != null)
                {
                    IXLRange rangePlYears = sheet.Range(sheet.Cell(5, i).Address, sheet.Cell(5, i + 2).Address);
                   
                    rangePlYears.Merge();
                    rangePlYears.Value = "Охирги уч йиллик фойда/зарар (минг сўм)";
   
                    i+=3;
                }

                if (commentB1 != null)
                {
                    sheet.Cell(5, i).Value = "Изоҳ";
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

            End = i - 1;

            int rowForShare = 6;
            int rowsToMerge = 0;
            int rowsToMergeShareholders = 0;
            List<int> termsList = new();

            int shareholderMax = 0;

            foreach (var s in Shares)
            {
                List<Shareholder> _shareholders = new();
                List<SharesAndHolders> shareAndshareholderList = new();

                shareAndshareholderList = _context.SharesAndHolders.Where(a => a.ShareId == s.ShareId).ToList();

                foreach (var sItem in shareAndshareholderList ?? Enumerable.Empty<SharesAndHolders>())
                {
                    var shareholder = _context.Shareholders.FirstOrDefault(s => s.ShareholderId == sItem.ShareholderId);
                    _shareholders.Add(shareholder);
                }

                if(shareholderMax < _shareholders.Count)
                {
                    shareholderMax = _shareholders.Count;
                }
            }

            try
            {
                foreach (var share in Shares)
                {
                    termsList.Clear();

                    var transferredAsset = _context.TransferredAssets.Where(t => t.AssetId == share.TransferredAssetId && t.AgreementDate.Date <= chosenDate).ToList();

                    var assetEvaluations = _context.AssetEvaluations.Where(a => a.ShareId == share.ShareId
                        && a.ReportDate.Date <= chosenDate && a.StatusChangedDate.Date > chosenDate.Date).ToList();

                    var submissionOnBiddings = _context.SubmissionOnBiddings.Where(s => s.ShareId == share.ShareId
                        && s.BiddingExposureDate.Date <= chosenDate && s.AuctionCancelledDate.Date > chosenDate && (s.Status.Equals("Сотилди")||s.Status.Equals("Сотувда"))).ToList();

                    var reductionInAssets = _context.ReductionInAssets.Where(s => s.ShareId == share.ShareId
                        && s.SolutionDate.Date <= chosenDate && s.StatusChangedDate.Date > chosenDate.Date).ToList();

                    var oneTimePaymentAssets = _context.OneTimePaymentAssets.Where(s => s.ShareId == share.ShareId
                        && s.SolutionDate.Date <= chosenDate && s.BiddingCancelledDate.Date > chosenDate.Date
                        && s.Step2.ContractCancelledDate > chosenDate).Include(s => s.Step2).Include(s => s.Step3).ToList();

                    var installmentAssets = _context.InstallmentAssets.Where(s => s.ShareId == share.ShareId
                        && s.AggreementDate.Date <= chosenDate && s.ContractCancelledDate.Date > chosenDate.Date).ToList();

                    termsList.Add(transferredAsset.Count);
                    termsList.Add(assetEvaluations.Count);
                    termsList.Add(submissionOnBiddings.Count);
                    termsList.Add(reductionInAssets.Count);
                    termsList.Add(oneTimePaymentAssets.Count);
                    termsList.Add(installmentAssets.Count);

                    var shareAndshareholderList = _context.SharesAndHolders.Where(s => s.ShareId == share.ShareId).ToList();
                    List<Shareholder> shareholders = new();
                    try
                    {

                        foreach (var sItem in shareAndshareholderList ?? Enumerable.Empty<SharesAndHolders>())
                        {
                            var shareholder = _context.Shareholders.FirstOrDefault(s => s.ShareholderId == sItem.ShareholderId);
                            shareholders.Add(shareholder);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    var shareholderCount = shareholders.Count;

                    if (shareholderCount > 0 && termsList.Max() > 0)
                    {
                        rowsToMerge = Ekuk(shareholderCount, termsList.Max())-1;
                        rowsToMergeShareholders = (rowsToMerge+1)/shareholderCount - 1;
                    }

                    else if (shareholderCount == 0 && termsList.Max() > 1)
                    {
                        rowsToMerge = termsList.Max() - 1;
                    }

                    else if (shareholderCount > 0 && termsList.Max() == 0)
                    {
                        rowsToMerge = shareholderCount - 1;
                    }

                    sumRow = Shares.Count() + 5 + shareholderMax;

                    var reg = await _context.Regions.FirstOrDefaultAsync(i => i.RegionId == share.RegionId);
                    var dis = await _context.Districts.FirstOrDefaultAsync(i => i.DistrictId == share.DistrictId);

                    #region Writing values into cells

                    int j = 1;

                    var numberSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                    numberSh.Value = number;
                    numberSh.Merge();
                    
                    number++;
                    j++;

                    var BusinessEntitySh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                    BusinessEntitySh.Value = share.BusinessEntityName;
                    BusinessEntitySh.Merge();
                    j++;

                    if (block1 != null)
                    {
                        #region Block 1 

                        if (idRegNumberB1 != null)
                        {
                            var CadastreNumberSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            CadastreNumberSh.Value = share.IdRegNumber;
                            CadastreNumberSh.Merge();
                            j++;
                        }

                        if (parentOrgB1 != null)
                        {
                            var ParentOrgSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            ParentOrgSh.Value = share.ParentOrganization;
                            ParentOrgSh.Merge();
                            j++;
                        }

                        if (activityB1 != null)
                        {
                            var ActivitySh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            ActivitySh.Value = share.Activities;
                            ActivitySh.Merge();

                            j++;
                        }

                        if (activityShareB1 != null)
                        {
                            var ActivityShareSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            ActivityShareSh.Value = share.ActivityShare;
                            ActivityShareSh.Merge();

                            j++;
                        }

                        if (foundationYB1 != null)
                        {

                            var FoundationYSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            FoundationYSh.Value = share.FoundationYear;
                            FoundationYSh.Merge();

                            j++;
                        }

                        if (stateRegDateB1 != null)
                        {

                            var StateRegDateSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            StateRegDateSh.Value = share.StateRegistrationDate.ToString("dd/MM/yyy");
                            StateRegDateSh.Merge();

                            j++;
                        }

                        if (regionB1 != null)
                        {
                            var RegionNameS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            RegionNameS.Value = reg.RegionName;
                            RegionNameS.Merge();

                            j++;
                        }

                        if (districtB1 != null)
                        {

                            var DistrictNameS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            DistrictNameS.Value = dis.DistrictName;
                            DistrictNameS.Merge();

                            j++;
                        }

                        if (adressB1 != null)
                        {

                            var AddressS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            AddressS.Value = share.Address;
                            AddressS.Merge();

                            j++;
                        }

                        if (authCapB1 != null)
                        {

                            var AuthCapsh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            AuthCapsh.Value = share.AuthorizedCapital;
                            AuthCapsh.Merge();

                            j++;
                        }

                        if (shareHolderB1 != null)
                        {
                            IXLRange ShareholderSh;
                            int r = 0;
                            foreach (var item in shareholders)
                            {

                                ShareholderSh = sheet.Range(rowForShare + r, j, (rowForShare + r + rowsToMergeShareholders), j);
                                ShareholderSh.Value = item.ShareholderName;
                                ShareholderSh.Merge();
                                r++;
                            }

                            j++;
                        }

                        if (amountFromAuthCapB1 != null)
                        {
                            IXLRange amountFromAuthSh;
                            int r = 0;
                            foreach (var item in shareholders)
                            {
                                var sharepercentage = Math.Round((decimal.Parse(item.AmountFromAuthCapital, CultureInfo.InvariantCulture.NumberFormat) * 100 / decimal.Parse(share.AuthorizedCapital, CultureInfo.InvariantCulture.NumberFormat)), 2);


                                amountFromAuthSh = sheet.Range(rowForShare + r, j, (rowForShare + r + rowsToMergeShareholders), j);
                                amountFromAuthSh.Value = item.AmountFromAuthCapital + "(" + sharepercentage + "%)";
                                amountFromAuthSh.Merge();
                                r++;
                            }

                            j++;
                        }

                        if (numberOfSharesB1 != null)
                        {

                            var numberOfShareSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            numberOfShareSh.Value = share.NumberOfShares;
                            numberOfShareSh.Merge();
                            j++;
                        }

                        if (parValueShareB1 != null)
                        {

                            var parValueSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            parValueSh.Value = share.ParValueOfShares;
                            parValueSh.Merge();
                            j++;
                        }

                        if (administStaffB1 != null)
                        {

                            var administSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            administSh.Value = share.AdministrativeStaff;
                            administSh.Merge();
                            j++;
                        }

                        if (productPersB1 != null)
                        {

                            var productPerSh = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            productPerSh.Value = share.ProductionPersonal;
                            productPerSh.Merge();
                            j++;
                        }

                        if (pAreaB1 != null)
                        {

                            var ProductionAreaS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            ProductionAreaS.Value = share.ProductionArea;
                            ProductionAreaS.Merge();

                            productionArea += float.Parse(share.ProductionArea, CultureInfo.InvariantCulture.NumberFormat);

                            var cell = sheet.Cell(sumRow, j);
                            cell.Value = productionArea;

                            cell.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                            cell.Style.Alignment.WrapText = true;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Fill.SetBackgroundColor(XLColor.LightGreen);

                            j++;
                        }

                        if (bAreaB1 != null)
                        {

                            var BuildingAreaS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                            BuildingAreaS.Value = share.BuildingsArea;
                            BuildingAreaS.Merge();

                            buildingArea += float.Parse(share.BuildingsArea);

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

                        if (maintCostB1 != null)
                        {

                            var mainCostS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            mainCostS.Value = share.MaintanenceCostForYear;
                            mainCostS.Merge();

                            j++;
                        }

                        if (averageMSalB1 != null)
                        {
                            var averageMS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            averageMS.Value = share.AverageMonthlySalary;
                            averageMS.Merge();

                            j++;
                        }

                        if (amountPayB1 != null)
                        {
                            var amountPayS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            amountPayS.Value = share.AmountPayable;
                            amountPayS.Merge();

                            j++;
                        }

                        if (amountRecB1 != null)
                        {
                            var amountRecS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            amountRecS.Value = share.AmountReceivable;
                            amountRecS.Merge();

                            j++;
                        }

                        if (plYearsB1 != null)
                        {
                            var firstYS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            firstYS.Value = "(" + share.Year1 + " йил)" + share.ProfitOrLossOfYear1;
                            firstYS.Merge();

                            var secondYS = sheet.Range(rowForShare, j + 1, (rowForShare + rowsToMerge), j + 1); //d
                            secondYS.Value = "(" + share.Year2 + " йил)" + share.ProfitOrLossOfYear2;
                            secondYS.Merge();

                            var thirdYS = sheet.Range(rowForShare, j + 2, (rowForShare + rowsToMerge), j + 2); //d
                            thirdYS.Value = "(" + share.Year3 + " йил)" + share.ProfitOrLossOfYear3;
                            thirdYS.Merge();

                            j += 3;
                        }

                        if (commentB1 != null)
                        {
                            var commentS = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j); //d
                            commentS.Value = share.Comments;
                            commentS.Merge();

                            j++;
                        }


                        #endregion
                    }

                    int rowForInfo = rowForShare;

                    if (block2 != null)
                    {
                        #region Block 2

                        // 3. Блок «Переданные активы» 
                        foreach (var tr in transferredAsset ?? Enumerable.Empty<TransferredAsset>())
                        {
                            var trForm = _context.TransferForms.FirstOrDefault(i => i.TransferFormId == tr.TransferFormId);

                            if (trFormB2 != null)
                            {
                                var trFormSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                trFormSh.Value = trForm.TransferFormName;
                                trFormSh.Merge();
                                j++;
                                
                            }

                            if (orgNameB2 != null)
                            {
                                var orgNameSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                orgNameSh.Value = tr.OrgName;
                                orgNameSh.Merge();
                                j++;
                                                            }

                            if (sNumberB2 != null)
                            {
                                var sNumberSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                sNumberSh.Value = tr.SolutionNumber;
                                sNumberSh.Merge();
                                j++;
                            }

                            if (sDateB2 != null)
                            {
                                var sDateSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                sDateSh.Value = tr.SolutionDate.ToString("dd/MM/yyyy");
                                sDateSh.Merge();
                                
                                j++;
                            }

                            if (orgNameAssetB2 != null)
                            {
                                var orgNameAssetSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                orgNameAssetSh.Value = tr.OrgNameOfAsset;
                                orgNameAssetSh.Merge();
                                
                                j++;
                            }

                            if (tCostB2 != null)
                            {
                                var tCostSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                tCostSh.Value = tr.TotalCost;
                                tCostSh.Merge();
                                
                                totalCost += float.Parse(tr.TotalCost);

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
                                var vatSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                vatSh.Value = tr.VAT;
                                vatSh.Merge();
                                
                                vat += tr.VAT;
                                sheet.Cell(sumRow, j).Value = vat;

                                j++;
                            }

                            if (actDateB2 != null)
                            {
                                var actDateSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                actDateSh.Value = tr.ActAndAssetDate.ToString("dd/MM/yyyy");
                                actDateSh.Merge();
                                
                                j++;
                            }

                            if (actNumberB2 != null)
                            {
                                var actNSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                actNSh.Value = tr.ActAndAssetNumber;
                                actNSh.Merge();
                                
                                j++;
                            }

                            if (agDateB2 != null)
                            {
                                var agDateSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                agDateSh.Value = tr.AgreementDate.ToString("dd/MM/yyyy");
                                agDateSh.Merge();
                                
                                j++;
                            }

                            if (agNumberB2 != null)
                            {
                                var agNSh = sheet.Range(rowForInfo, j, (rowForInfo + rowsToMerge), j);
                                agNSh.Value = tr.AgreementNumber;
                                agNSh.Merge();
                                
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
                        rowForInfo = rowForShare;

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

                                marketvalue += float.Parse(assetE.MarketValue);

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
                        rowForInfo = rowForShare;

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

                                ActiveValue += float.Parse(sbOnB.ActiveValue);

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
                        rowForInfo = rowForShare;

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

                                Amount += float.Parse(red.Amount);

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

                                AssetValueAfterDecline += float.Parse(red.AssetValueAfterDecline);

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
                        rowForInfo = rowForShare;

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

                                    amountOfAssetSold1 += float.Parse(oneT2.AmountOfAssetSold);

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

                                    AmountPayed += float.Parse(oneT2.AmountPayed);

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
                        rowForInfo = rowForShare;

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

                                amountOfAssetSold2 += float.Parse(inst.AmountOfAssetSold);

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

                                actualInitPayment += float.Parse(inst.ActualInitPayment);

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

                                actualPayment += float.Parse(inst.ActualPayment);

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

                        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(i => i.Id == share.ApplicationUserId);

                        if (user != null)
                        {

                            if (fNameB8 != null)
                            {
                                var FN = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                                FN.Merge();
                                FN.Value = user.Fullname;
                                j++;
                            }

                            if (positionB8 != null)
                            {
                                var Pos = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                                Pos.Merge();
                                Pos.Value = user.Postion;
                                j++;
                            }

                            if (phNumberB8 != null)
                            {
                                var PhN = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                                PhN.Merge();
                                PhN.Value = user.PhoneNumber.Substring(user.PhoneNumber.Length - 9);

                                j++;
                            }

                            if (emailB8 != null)
                            {
                                var Em = sheet.Range(rowForShare, j, (rowForShare + rowsToMerge), j);
                                Em.Merge();
                                Em.Value = user.Email;

                                j++;
                            }
                        }

                        #endregion
                    }

                    #endregion

                    rowForShare = rowForShare + rowsToMerge + 1;
                    rowsToMerge = 0;
                    
                }
            }

            catch(Exception ex)
            {
                var s = ex.Message;
            }

            var Bb = sheet.Range(4, 1, rowForShare - 1, End); // change to other rows
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
            sheet.Rows(6, rowForShare).Height = 28;

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

            int degree;

            int Ekuk(int a, int b)
            {
                int ekuk = 1;

                List<int[]> arrayA = new();
                List<int[]> arrayB = new();

                int max = Math.Max(a, b);
                int min = Math.Min(a, b);
                int remainder = max % min;

                if (remainder == 0)
                {
                    return max;
                }

                if (isPrime(a) || isPrime(b))
                {
                    return a * b;
                }

                int rootA = (int)Math.Pow(a, 0.5);

                for (int i = 2; i <= rootA; )
                {
                    if (isPrime(i) && a%i==0)
                    {
                        degree = 0;
                        int primeDegree = findDegree(a, i);
                        int[] arr = new int[] { i, primeDegree };
                        arrayA.Add(arr);
                    }

                    if (i < 3)
                        i++;
                    else
                        i += 2;
                }

                int rootB = (int)Math.Pow(b, 0.5);

                for (int i = 2; i <= rootB; i++)
                {
                    if (isPrime(i))
                    {
                        degree = 0;
                        int primeDegree = findDegree(b, i);
                        int[] arr = new int[] { i, primeDegree };
                        arrayB.Add(arr);
                    }
                }

                int lengthA = arrayA.Count;
                int lengthB = arrayB.Count;

                if (lengthA >= lengthB)
                {
                    for (int i = 0; i < lengthA; i++)
                    {
                        for (int j = 0; j < lengthB; j++)
                        {
                            if (arrayA[i][0] == arrayB[j][0])
                            {
                                ekuk *= (int)Math.Pow(arrayA[i][0], Math.Max(arrayA[i][1], arrayB[j][1]));
                                break;
                            }

                            ekuk *= (int)Math.Pow(arrayA[i][0], arrayA[i][1]);
                        }
                    }

                }

                if (lengthB > lengthA)
                {
                    for (int i = 0; i < lengthB; i++)
                    {
                        for (int j = 0; j < lengthA; j++)
                        {
                            if (arrayB[i][0] == arrayA[j][0])
                            {
                                ekuk *= (int)Math.Pow(arrayB[i][0], Math.Max(arrayB[i][1], arrayA[j][1]));
                                break;
                            }

                            ekuk *= (int)Math.Pow(arrayB[i][0], arrayB[i][1]);
                        }
                    }

                }

                return ekuk;

            }

            bool isPrime(int num)
            {
                int root = (int)Math.Pow(num, 0.5);

                for(int i = 2; i<=root; i++)
                {
                    int remainder = num % i;
                    if (remainder == 0)
                        return false;
                }

                return true;
            }

            int findDegree(int num, int prime)
            {

                if (num % prime == 0 && num != 0)
                {
                    degree++;
                    return findDegree(num / prime, prime);
                }

                else
                    return degree;
                    
            }

        }

        [HttpGet]
        public IActionResult GetConfirmed()
        {
            var shares = _context.Shares
                .Include(r => r.ApplicationUser)
                .ThenInclude(r =>r.Organization)
                .Include(r => r.DistrictOfObject)
                .Include(r => r.RegionOfObject)
                .Where(r => r.Confirmed == true)
                .ToList();

            if (!shares.Any())
            {
                return Json(new EmptyResult());
            }

            List<ShareShortDto> sharesShortDtos = new();

            foreach (var share in shares)
            {
               
                ShareShortDto sharesShortDto = new()
                {
                    ShareId = share.ShareId,
                    BusinessEntityName = share.BusinessEntityName,
                    ParentOrganization = share.ParentOrganization,
                    IdRegNumber = share.IdRegNumber,
                    Activities = share.Activities,
                    ActivityShare = share.ActivityShare,
                    RegionName = share.RegionOfObject.RegionName,
                    RegCertificateLink = share.RegCertificateLink,
                    OrgCharterLink = share.OrgCharterLink,
                    BalanceSheetLink = share.BalanceSheetLink,
                    AuditConclusionLink = share.AuditConclusionLink,
                    FinancialResultLink = share.FinancialResultLink
                };

                sharesShortDtos.Add(sharesShortDto);

            }
            //return null;
            return Json(new { data = sharesShortDtos });

        }

        
        [HttpPost]
        public IActionResult GetFilteredData([FromBody] JObject data)
        {
            if (data["orgId"] == null)
            {
                return Json(new { });
            }

            int orgId = (int)data["orgId"];
            int regionId = (int)data["regionId"];
            bool onBalance = (bool)data["balance"];
            bool offBalance = (bool)data["notBalance"];

            List<ShareShortDto> sharesShortDtos = new();

            Shareholder shareholder = new();
            List<SharesAndHolders> sharesAndHolders = new();

            List<Share> filteredAssets = new();

            List<Share> rOnBalance = new();
            List<Share> rOffBalance = new();

            var shares = _context.Shares
            .Include(r => r.ApplicationUser)
            .ThenInclude(a => a.Organization)
            .Include(r => r.DistrictOfObject)
            .Include(r => r.RegionOfObject)
            .Where(r => r.Confirmed == true);



            if (orgId == 0 && regionId != 0)
            {

                shares = _context.Shares
                    .Include(r => r.ApplicationUser)
                    .ThenInclude(a => a.Organization)
                    .Include(r => r.DistrictOfObject)
                    .Include(r => r.RegionOfObject)
                    .Where(r => r.Confirmed == true && r.RegionId == regionId);
            }

            else if (orgId != 0 && regionId == 0)
            {
                string name = _context.Organizations.FirstOrDefault(o => o.OrganizationId == orgId).OrganizationName;

                shares = _context.Shares
                    .Include(r => r.ApplicationUser)
                    .ThenInclude(a => a.Organization)
                    .Include(r => r.DistrictOfObject)
                    .Include(r => r.RegionOfObject)
                    .Where(r => r.Confirmed == true && r.ParentOrganization.Equals(name));
            }

            else if (orgId != 0 && regionId != 0)
            {
                string name = _context.Organizations.FirstOrDefault(o => o.OrganizationId == orgId).OrganizationName;

                shares = _context.Shares
                    .Include(r => r.ApplicationUser)
                    .ThenInclude(a => a.Organization)
                    .Include(r => r.DistrictOfObject)
                    .Include(r => r.RegionOfObject)
                    .Where(r => r.Confirmed == true && r.RegionId == regionId && r.ParentOrganization.Equals(name));
            }

            if (onBalance)
            {
                rOnBalance = shares.Where(r => r.Status == true).ToList();
            }

            if (offBalance)
            {
                rOffBalance = shares.Where((r) => r.Status == false).ToList();
            }

            rOnBalance.AddRange(rOffBalance);
            filteredAssets.AddRange(rOnBalance);


            if (!filteredAssets.Any())
            {
                return Json(new EmptyResult());
            }

            
            foreach (var share in filteredAssets)
            {

                ShareShortDto sharesShortDto = new()
                {
                    ShareId = share.ShareId,
                    BusinessEntityName = share.BusinessEntityName,
                    ParentOrganization = share.ParentOrganization,
                    IdRegNumber = share.IdRegNumber,
                    Activities = share.Activities,
                    ActivityShare = share.ActivityShare,
                    RegionName = share.RegionOfObject.RegionName,
                    RegCertificateLink = share.RegCertificateLink,
                    OrgCharterLink = share.OrgCharterLink,
                    BalanceSheetLink = share.BalanceSheetLink,
                    AuditConclusionLink = share.AuditConclusionLink,
                    FinancialResultLink = share.FinancialResultLink
                };

                sharesShortDtos.Add(sharesShortDto);

            }
            //return null;
            return Json(new { data = sharesShortDtos });
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
        public IActionResult GetUser([FromBody] int id)
        {
            var user = _context.ApplicationUsers.Include(u => u.Organization).Include(u => u.RealEstates).Include(u => u.Shares).FirstOrDefault(u => u.Shares.Any(r => r.ShareId == id));

            if (user == null)
            {
                return Json(new { success = false, message = "Масъул шахс топилмади!" });
            }

            return Json(new { data = user, success = true });
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

            //List<SharesAndHoldersViewModel> sharesAndHoldersData = new();

            //List<SharesAndHolders> sharesAndHolders = new();


            //string shareholders = "";
            //string amount;
            //string shareAmount = "";
            //string sharepercentage = "";

            share.StateRegistrationDateStr = share.StateRegistrationDate.ToShortDateString();
            share.FoundationYearStr = share.FoundationYear.ToShortDateString();

            //var user = _context.ApplicationUsers.Include(a => a.Organization).FirstOrDefault(u => u.Id == share.ApplicationUserId);

            //sharesAndHolders = _context.SharesAndHolders.Where(s => s.ShareId == share.ShareId).ToList();

            //foreach (var item in sharesAndHolders)
            //{
            //    shareholders += _context.Shareholders.First(s => s.ShareholderId == item.ShareholderId).ShareholderName + " | ";
            //    amount = _context.Shareholders.First(s => s.ShareholderId == item.ShareholderId).AmountFromAuthCapital;
            //    shareAmount += amount + " | ";
            //    sharepercentage += Math.Round((decimal.Parse(amount, CultureInfo.InvariantCulture.NumberFormat) * 100 / decimal.Parse(share.AuthorizedCapital, CultureInfo.InvariantCulture.NumberFormat)), 2) + " | ";
            //}

            //SharesAndHoldersViewModel sharesAndHoldersViewModel = new()
            //{
            //    Shareholders = shareholders,
            //    ShareAmount = shareAmount,
            //    SharePercentage = sharepercentage,
            //    Share = share

            //};

            //sharesAndHoldersData.Add(sharesAndHoldersViewModel);

            List<Share> sharesData = new()
            {
                share
            };

            if (forDetails)
                return Json(new { data = sharesData });

            return Json(new { data = share, success = true });
        }

        [HttpPost]
        public IActionResult GetShareHolders([FromBody] int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            var share = _context.Shares.Find(id);

            if (share == null)
            {
                return Json(new { success = false, message = "Объект топилмади!" });
            }

            List<ShareholderViewModel> Shareholders = new();

            var ShareAndHolders = _context.SharesAndHolders.Where(s => s.ShareId == share.ShareId).ToList();

            foreach(var item in ShareAndHolders)
            {
                var shareholder = _context.Shareholders.FirstOrDefault(s => s.ShareholderId == item.ShareholderId);
                if (shareholder != null)
                {
                    ShareholderViewModel viewmodel = new()
                    {
                        ShareholderName = shareholder.ShareholderName,
                        ShareAmount = shareholder.AmountFromAuthCapital,
                        SharePercentage = Math.Round((decimal.Parse(shareholder.AmountFromAuthCapital, CultureInfo.InvariantCulture.NumberFormat) * 100 / decimal.Parse(share.AuthorizedCapital, CultureInfo.InvariantCulture.NumberFormat)), 2)
                    };

                    Shareholders.Add(viewmodel);
                }
                    
            }

            return Json(new { success = true, data = Shareholders });
        }

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
                catch (Exception ex)
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

            if(_share == null)
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

            var shareAndholders = _context.SharesAndHolders.Where(s => s.ShareId == id).ToList();
            List<Shareholder> shareHolders = new();

            foreach (var item in shareAndholders)
            {
                try
                {
                    var shareholder = await _context.Shareholders.FirstAsync(s => s.ShareholderId == item.ShareholderId);
                    shareHolders.Add(shareholder);
                }
                catch (Exception ex)
                {
                    var m = ex.Message;
                }

            }

            ViewData["Shareholders"] = shareHolders;

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
                    if (finder == 2)
                    {

                        share.BalanceSheetLink = fileModel.SystemPath;

                    }

                    if (finder == 3)
                    {

                        share.FinancialResultLink = fileModel.SystemPath;

                    }

                    if (finder == 4)
                    {

                        share.AuditConclusionLink = fileModel.SystemPath;

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

                    if (finder == 2)
                    {
                        share.BalanceSheetId = createdFile.Result.FileId;
                        share.BalanceSheetLink = createdFile.Result.SystemPath;

                    }

                    if (finder == 3)
                    {
                        share.FinancialResultId= createdFile.Result.FileId;
                        share.FinancialResultLink = createdFile.Result.SystemPath;

                    }

                    if (finder == 4)
                    {
                        share.AuditConclusionId = createdFile.Result.FileId;
                        share.AuditConclusionLink = createdFile.Result.SystemPath;

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
                await _context.SaveChangesAsync(_userManager.GetUserId(User), businessEntityName);
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
            try
            {
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

            return Json(new { success = true, message = "Муваффақиятли!" });

        }

        [HttpDelete]
        public async Task<IActionResult> RemoveShareholder(int shareHolderId, int shareId)
        {
            var shareholder = await _context.Shareholders.FindAsync(shareHolderId);
            if (shareholder == null)
                return Json(new { success = false, message ="Акционер/участник топилмади" }); 

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
