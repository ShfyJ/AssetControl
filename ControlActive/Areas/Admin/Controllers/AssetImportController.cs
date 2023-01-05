using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ControlActive.Models;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using ControlActive.Data;
using System.Linq;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.Drawing;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel.Tables;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using System.Web;
using System.Web.WebPages;
using System.Net;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.CodeAnalysis;
using System.Globalization;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using static ClosedXML.Excel.XLPredefinedFormat;
using DateTime = System.DateTime;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class AssetImportController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly Random _random = new();
        public AssetImportController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string RandomEmail(int length)
        {
            var userList = _context.ApplicationUsers.ToList();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string value = new(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
            value += "@ung.uz";

            foreach (var item in userList)
            {
                if (item.Email == value)
                {
                    RandomEmail(7);
                }
            }

            return value;


        }

        public async Task<List<string>> CreateUser(ApplicationUser user)
        {

            List<string> message = new();

            var context = new ValidationContext(user, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(user, context, validationResults, true);

            if (isValid)
            {
                var result = await _userManager.CreateAsync(user, "User@123");
                if (result.Succeeded)
                {
                    _context.Add(user);
                    await _userManager.AddToRoleAsync(user, user.Role);

                    message.Add(user.Id);

                    return message;
                }

                message.Add("");
                message.Add("Фойдаланувчи яратишда хатолик!");
            }


            message.Add("");
            message.Add("Фойдаланувчи маълумотларида хатолик!");

            return message;
        }

        public async Task MakeAssetOutOfAccount(int assetId, int target, DateTime outDate)
        {

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FindAsync(assetId);

                if (realEstate != null)
                {
                    realEstate.Status = false;
                    realEstate.OutOfAccountDate = outDate;
                }
            }

            if (target == 2)
            {
                var share = await _context.Shares.FindAsync(assetId);

                if (share != null)
                {
                    share.Status = false;
                    share.OutOfAccountDate = outDate;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> AddRealEstate(int row, ExcelWorksheet worksheet, string userId, DateTime outOfAccountDate)
        {

            string realEstateName;
            List<string> result = new();
            int numberOfEmployee = 0;
            string address = "";
            string fullArea = "0";
            string buildingArea = "0";
            string maintenanceCostForYear = "0";
            string initialCostOfObject = "0";
            string wear = "0";
            string residualValueOfObject = "0";
            string cadastreNumber;
            string activity;

            bool status = true;

            DateTime commisiioningDate = new();
            DateTime cadastreDate = new();

            DateTime date;
            float number;

            realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();

            if (outOfAccountDate < DateTime.Now)
            {
                status = false;
            }

            if (worksheet.Cells[row, 2].Value != null && !worksheet.Cells[row, 2].Value.ToString().Trim().IsEmpty())
            {
                cadastreNumber = worksheet.Cells[row, 2].Value.ToString().Trim();
            }

            else
            {
                cadastreNumber = "00:00:00:00:00:0000";
            }


            if (worksheet.Cells[row, 3].Value != null && !worksheet.Cells[row, 3].Value.ToString().Trim().Equals(""))
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out date))
                {
                    result.Add("bad");
                    result.Add(realEstateName + " нинг кадастр санаси { ой/кун/йил } форматида киритилмаган! ");

                    return result;
                }

                else
                {
                    cadastreDate = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString().Trim());
                }
            }

            if (worksheet.Cells[row, 4].Value != null && !worksheet.Cells[row, 4].Value.ToString().Trim().IsEmpty())
            {
                activity = worksheet.Cells[row, 4].Value.ToString().Trim();
            }

            else
            {
                activity = string.Empty;
            }


            if (worksheet.Cells[row, 5].Value != null && !worksheet.Cells[row, 5].Value.ToString().Trim().Equals(""))
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 5].Value.ToString().Trim(), out date))
                {
                    result.Add("bad");
                    result.Add(realEstateName + " нинг ишга тушириш санаси { ой/кун/йил } форматида киритилмаган! ");

                    return result;
                }

                else
                {
                    commisiioningDate = DateTime.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                }
            }


            if (worksheet.Cells[row, 8].Value.ToString().Trim().Equals("#N/A"))
            {
                result.Add("bad");
                result.Add(realEstateName + " жойлашган вилоят номи киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 9].Value.ToString().Trim().Equals("#N/A"))
            {
                result.Add("bad");
                result.Add(realEstateName + " жойлашган туман номи киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 16].Value != null && !worksheet.Cells[row, 16].Value.Equals(""))
            {
                numberOfEmployee = int.Parse(worksheet.Cells[row, 16].Value.ToString().Trim());
            }

            if (worksheet.Cells[row, 22].Value.ToString().Trim().Equals("#N/A"))
            {
                result.Add("bad");
                result.Add(realEstateName + " ни янада самарали фойдаланиш бўйича таклиф киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 11].Value == null || worksheet.Cells[row, 11].Value.ToString().Trim().IsEmpty())
            {
                result.Add("bad");
                result.Add(realEstateName + " нинг <<Ташкилот номи (Объект эгаси номи)>> киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 10].Value != null && !worksheet.Cells[row, 10].Value.ToString().Trim().IsEmpty())
            {
                address = worksheet.Cells[row, 10].Value.ToString().Trim();
            }

            if (worksheet.Cells[row, 13].Value != null && !worksheet.Cells[row, 13].Value.ToString().Trim().IsEmpty())
            {
                if (float.TryParse(worksheet.Cells[row, 13].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    buildingArea = worksheet.Cells[row, 13].Value.ToString().Trim();
                    buildingArea = buildingArea.Replace(',', '.');
                }
                
            }

            if (worksheet.Cells[row, 14].Value != null && !worksheet.Cells[row, 14].Value.ToString().Trim().IsEmpty())
            {
                
                if (float.TryParse(worksheet.Cells[row, 14].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    fullArea = worksheet.Cells[row, 14].Value.ToString().Trim();
                    fullArea = fullArea.Replace(',', '.');
                }
            }

            if (worksheet.Cells[row, 17].Value != null && !worksheet.Cells[row, 17].Value.ToString().Trim().IsEmpty())
            {
                
                if (float.TryParse(worksheet.Cells[row, 17].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    maintenanceCostForYear = worksheet.Cells[row, 17].Value.ToString().Trim();
                    maintenanceCostForYear = maintenanceCostForYear.Replace(',', '.');
                }
            }

            if (worksheet.Cells[row, 18].Value != null && !worksheet.Cells[row, 18].Value.ToString().Trim().IsEmpty())
            {
                
                if (float.TryParse(worksheet.Cells[row, 18].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    initialCostOfObject = worksheet.Cells[row, 18].Value.ToString().Trim();
                    initialCostOfObject = initialCostOfObject.Replace(',', '.');
                }
            }

            if (worksheet.Cells[row, 19].Value != null && !worksheet.Cells[row, 19].Value.ToString().Trim().IsEmpty())
            {
               
                if (float.TryParse(worksheet.Cells[row, 19].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    wear = worksheet.Cells[row, 19].Value.ToString().Trim();
                    wear = wear.Replace(',', '.');
                }
            }

            if (worksheet.Cells[row, 20].Value != null && !worksheet.Cells[row, 20].Value.ToString().Trim().IsEmpty())
            {
                
                if (float.TryParse(worksheet.Cells[row, 20].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    residualValueOfObject = worksheet.Cells[row, 20].Value.ToString().Trim();
                    residualValueOfObject = residualValueOfObject.Replace(',', '.');
                }
            }

            RealEstate realEstate = new()
            {
                ApplicationUserId = userId,
                Date_Added = DateTime.Now,
                OutOfAccountDate = outOfAccountDate,
                Status = status,
                Confirmed = true,
                RealEstateName = realEstateName,
                CadastreNumber = cadastreNumber,
                CadastreRegDate = cadastreDate,
                Activity = activity,
                CommisioningDate = commisiioningDate,
                RegionId = Int32.Parse(worksheet.Cells[row, 8].Value.ToString().Trim()),
                DistrictId = Int32.Parse(worksheet.Cells[row, 9].Value.ToString().Trim()),
                Address = address,
                AssetHolderName = worksheet.Cells[row, 11].Value.ToString().Trim(),
                BuildingArea = buildingArea,
                FullArea = fullArea,
                NumberOfEmployee = numberOfEmployee,
                MaintenanceCostForYear = maintenanceCostForYear,
                InitialCostOfObject = initialCostOfObject,
                Wear = wear,
                ResidualValueOfObject = residualValueOfObject,
                ProposalId = Int32.Parse(worksheet.Cells[row, 22].Value.ToString().Trim())
            };

            await _context.AddAsync(realEstate);
            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);

            if (worksheet.Cells[row, 15].Value != null && worksheet.Cells[row, 15].Value.ToString().Trim() != "")
            {
                string[] chosenInfrastructures = worksheet.Cells[row, 15].Value.ToString().Trim().Split(',');

                var infrastructures = _context.Infrastuctures.ToList();

                List<int> infrastructureIds = new();

                for (int i = 0; i < chosenInfrastructures.Length; i++)
                {
                    for (int j = 0; j < infrastructures.Count; j++)
                    {
                        if (chosenInfrastructures[i].Equals(infrastructures[j].InfrastructureName))
                        {
                            infrastructureIds.Add(infrastructures[j].InfrastructureId);
                            break;
                        }
                    }
                }

                foreach (var id in infrastructureIds)
                {
                    RealEstateInfrastructure RI = new()
                    {
                        RealEstateId = realEstate.RealEstateId,
                        InfrastuctureId = id
                    };

                    await _context.RRealEstateInfrastructures.AddAsync(RI);
                }
            }


            result.Add("good");
            result.Add(realEstate.RealEstateId.ToString());
            result.Add(realEstateName);

            return result;
        }

        public async Task<List<string>> AddShare(int row, ExcelWorksheet worksheet, string userId, DateTime outOfAccountDate)
        {

            string realEstateName;
            List<string> result = new();
            int numberOfEmployee = 0;

            DateTime date;

            realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();


            if (!DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out date))
            {
                result.Add("bad");
                result.Add(realEstateName + " нинг кадастр санаси { ой/кун/йил } форматида киритилмаган! ");

                return result;
            }

            if (!DateTime.TryParse(worksheet.Cells[row, 5].Value.ToString().Trim(), out date))
            {
                result.Add("bad");
                result.Add(realEstateName + " нинг ишга тушиш санаси { ой/кун/йил } форматида киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 8].Value.Equals("#N/A"))
            {
                result.Add("bad");
                result.Add(realEstateName + " жойлашган вилоят номи киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 9].Value.Equals("#N/A"))
            {
                result.Add("bad");
                result.Add(realEstateName + " жойлашган туман номи киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 16].Value != null || !worksheet.Cells[row, 16].Value.Equals(""))
            {
                numberOfEmployee = int.Parse(worksheet.Cells[row, 16].Value.ToString().Trim());
            }

            if (worksheet.Cells[row, 22].Value.Equals("#N/A"))
            {
                result.Add("bad");
                result.Add(realEstateName + " ни янада самарали фойдаланиш бўйича таклиф киритилмаган! ");

                return result;
            }

            RealEstate realEstate = new()
            {
                ApplicationUserId = userId,
                Date_Added = DateTime.Now,
                OutOfAccountDate = outOfAccountDate,
                Status = true,
                Confirmed = true,
                RealEstateName = realEstateName,
                CadastreNumber = worksheet.Cells[row, 2].Value.ToString().Trim(),
                CadastreRegDate = (DateTime)worksheet.Cells[row, 3].Value,
                Activity = worksheet.Cells[row, 4].Value.ToString().Trim(),
                CommisioningDate = (DateTime)worksheet.Cells[row, 5].Value,
                RegionId = Int32.Parse(worksheet.Cells[row, 8].Value.ToString().Trim()),
                DistrictId = Int32.Parse(worksheet.Cells[row, 9].Value.ToString().Trim()),
                Address = worksheet.Cells[row, 10].Value.ToString().Trim(),
                AssetHolderName = worksheet.Cells[row, 11].Value.ToString().Trim(),
                BuildingArea = worksheet.Cells[row, 13].Value.ToString().Trim(),
                FullArea = worksheet.Cells[row, 14].Value.ToString().Trim(),
                NumberOfEmployee = numberOfEmployee,
                MaintenanceCostForYear = worksheet.Cells[row, 17].Value.ToString().Trim(),
                InitialCostOfObject = worksheet.Cells[row, 18].Value.ToString().Trim(),
                Wear = worksheet.Cells[row, 19].Value.ToString().Trim(),
                ResidualValueOfObject = worksheet.Cells[row, 20].Value.ToString().Trim(),
                ProposalId = Int32.Parse(worksheet.Cells[row, 22].Value.ToString().Trim()),
            };

            await _context.AddAsync(realEstate);
            await _context.SaveChangesAsync(_userManager.GetUserId(User), realEstateName);

            if (worksheet.Cells[row, 15].Value != null || worksheet.Cells[row, 15].Value.ToString().Trim() != "")
            {
                string[] chosenInfrastructures = worksheet.Cells[row, 15].Value.ToString().Trim().Split(',');

                var infrastructures = _context.Infrastuctures.ToList();

                List<int> infrastructureIds = new();

                for (int i = 0; i < chosenInfrastructures.Length; i++)
                {
                    for (int j = 0; j < infrastructures.Count; j++)
                    {
                        if (chosenInfrastructures[i].Equals(infrastructures[j].InfrastructureName))
                        {
                            infrastructureIds.Add(infrastructures[j].InfrastructureId);
                            break;
                        }
                    }
                }

                foreach (var id in infrastructureIds)
                {
                    RealEstateInfrastructure RI = new()
                    {
                        RealEstateId = realEstate.RealEstateId,
                        InfrastuctureId = id
                    };

                    await _context.RRealEstateInfrastructures.AddAsync(RI);
                }
            }


            result.Add("good");
            result.Add(realEstate.RealEstateId.ToString());
            result.Add(realEstateName);

            return result;
        }
        public async Task<List<string>> AddTransferredAsset(int row, ExcelWorksheet worksheet, string userId, int target)
        {
            List<string> result = new();

            DateTime date;

            TransferredAsset transferredAsset;

            string orgName;
            string solutionNumber;
            DateTime solutionDate;
            string orgNameAsset;
            string totalCost = "0";
            DateTime actAndAssetDate;
            string actAndAssetNumber;
            DateTime agreementDate;
            string agreementNumber;
            float number;

            if (worksheet.Cells[row, 27].Value == null || worksheet.Cells[row, 27].Value.ToString().Trim().Equals(""))
            {
                result.Add("bad");
                result.Add(row + "-қаторда активнинг бериб юбориш қарори санаси киритилмаган!");
                return result;
            }

            if (worksheet.Cells[row, 24].Value == null || worksheet.Cells[row, 27].Value.ToString().Trim().Equals(""))
            {
                result.Add("bad");
                result.Add(row + "-қаторда <<ўтказиш шакли>> киритилмаган!");
                return result;
            }

            if (!DateTime.TryParse(worksheet.Cells[row, 27].Value.ToString().Trim(), out date))
            {
                result.Add("bad");
                result.Add(row + "-қаторда активнинг бериб юбориш қарори санаси { ой/кун/йил } форматида киритилмаган! ");

                return result;
            }

            if (worksheet.Cells[row, 29].Value == null || worksheet.Cells[row, 29].Value.ToString().Trim().IsEmpty())
            {
                result.Add("bad");
                result.Add(row + "-қаторда активнинг <<умумий қиймати>> киритилмаган!");
                return result;
            }

            if (worksheet.Cells[row, 29].Value != null && !worksheet.Cells[row, 29].Value.ToString().Trim().IsEmpty())
            {
                if (float.TryParse(worksheet.Cells[row, 29].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    totalCost = worksheet.Cells[row, 29].Value.ToString().Trim();
                    totalCost = totalCost.Replace(',', '.');
                }
            }

            if (worksheet.Cells[row, 25].Value != null && !worksheet.Cells[row, 25].Value.ToString().Trim().IsEmpty())
            {
                orgName = worksheet.Cells[row, 25].Value.ToString().Trim();
            }
            else
            {
                orgName = string.Empty;
            }

            if (worksheet.Cells[row, 26].Value != null && !worksheet.Cells[row, 26].Value.ToString().Trim().IsEmpty())
            {
                solutionNumber = worksheet.Cells[row, 26].Value.ToString().Trim();
            }
            else
            {
                solutionNumber = string.Empty;
            }

            if (worksheet.Cells[row, 28].Value != null && !worksheet.Cells[row, 28].Value.ToString().Trim().IsEmpty())
            {
                orgNameAsset = worksheet.Cells[row, 28].Value.ToString().Trim();
            }
            else
            {
                orgNameAsset = string.Empty;
            }

            if (worksheet.Cells[row, 31].Value != null && worksheet.Cells[row, 31].Value.ToString().IsEmpty())
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 31].Value.ToString().Trim(), out actAndAssetDate))
                {
                    actAndAssetDate = DateTime.Parse(worksheet.Cells[row, 31].Value.ToString().Trim());
                }

                else
                {
                    actAndAssetDate = DateTime.MinValue;
                }
            }

            else
            {
                actAndAssetDate = DateTime.MinValue;
            }

            if (worksheet.Cells[row, 32].Value != null && !worksheet.Cells[row, 32].Value.ToString().Trim().IsEmpty())
            {
                actAndAssetNumber = worksheet.Cells[row, 32].Value.ToString().Trim();
            }
            else
            {
                actAndAssetNumber = string.Empty;
            }

            if (worksheet.Cells[row, 33].Value != null && worksheet.Cells[row, 33].Value.ToString().IsEmpty())
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 33].Value.ToString().Trim(), out agreementDate))
                {
                    agreementDate = DateTime.Parse(worksheet.Cells[row, 33].Value.ToString().Trim());
                }

                else
                {
                    agreementDate = DateTime.MinValue;
                }
            }

            else
            {
                agreementDate = DateTime.MinValue;
            }

            if (worksheet.Cells[row, 34].Value != null && !worksheet.Cells[row, 34].Value.ToString().Trim().IsEmpty())
            {
                agreementNumber = worksheet.Cells[row, 34].Value.ToString().Trim();
            }
            else
            {
                agreementNumber = string.Empty;
            }


            DateTime outDate = DateTime.Parse(worksheet.Cells[row, 27].Value.ToString().Trim());

            List<string> assetResult = new();

            if (target == 1)
            {
                assetResult = AddRealEstate(row, worksheet, userId, outDate).Result;

                transferredAsset = new()
                {
                    TransferFormId = Int32.Parse(worksheet.Cells[row, 24].Value.ToString().Trim()),
                    RealEstateId = int.Parse(assetResult[1]),
                    OrgName = orgName,
                    SolutionNumber = solutionNumber,
                    SolutionDate = DateTime.Parse(worksheet.Cells[row, 27].Value.ToString().Trim()),
                    OrgNameOfAsset = orgNameAsset,
                    TotalCost = totalCost,
                    ActAndAssetDate = actAndAssetDate,
                    ActAndAssetNumber = actAndAssetNumber,
                    AgreementDate = agreementDate,
                    AgreementNumber = agreementNumber,
                    Confirmed = true
                };

                await _context.AddAsync(transferredAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetResult[2]);
            }

            if (target == 2)
            {
                assetResult = AddShare(row, worksheet, userId, outDate).Result;

                transferredAsset = new()
                {
                    TransferFormId = Int32.Parse(worksheet.Cells[row, 24].Value.ToString().Trim()),
                    ShareId = int.Parse(assetResult[1]),
                    OrgName = worksheet.Cells[row, 25].Value.ToString().Trim(),
                    SolutionNumber = worksheet.Cells[row, 26].Value.ToString().Trim(),
                    SolutionDate = DateTime.Parse(worksheet.Cells[row, 27].Value.ToString().Trim()),
                    OrgNameOfAsset = worksheet.Cells[row, 28].Value.ToString().Trim(),
                    TotalCost = worksheet.Cells[row, 29].Value.ToString().Trim(),
                    ActAndAssetDate = DateTime.Parse(worksheet.Cells[row, 31].Value.ToString().Trim()),
                    ActAndAssetNumber = worksheet.Cells[row, 32].Value.ToString().Trim(),
                    AgreementDate = DateTime.Parse(worksheet.Cells[row, 33].Value.ToString().Trim()),
                    AgreementNumber = worksheet.Cells[row, 34].Value.ToString().Trim(),
                    Confirmed = true

                };

                await _context.AddAsync(transferredAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetResult[2]);
            }

            result.Add("good");
            result.Add(assetResult[1]);

            return result;
        }


        public async Task<List<string>> AddAssetEvaluation(int row, bool hasBlock2, ExcelWorksheet worksheet, int assetId, int target)
        {
            bool reportStatus = true;

            List<string> result = new();

            AssetEvaluation assetEvaluation;

            string evaluatingOrgName = "";
            DateTime reportDate = new();
            string reportRegNumber = "";
            string marketValue = "0";
            string ExaminingOrgName = "";
            DateTime examReportDate = new();
            string examReportRegNumber = "";
            DateTime date;
            float number;
            string assetName;

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FindAsync(assetId);
                if (realEstate == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган объект топилмади! Объектларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = realEstate.RealEstateName;

                if (hasBlock2)
                {
                    if (worksheet.Cells[row, 42].Value != null && worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Ишончcиз"))
                    {
                        reportStatus = false;
                    }

                    if (worksheet.Cells[row, 35].Value != null && !worksheet.Cells[row, 35].Value.ToString().Trim().IsEmpty())
                    {
                        evaluatingOrgName = worksheet.Cells[row, 35].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 36].Value != null && !worksheet.Cells[row, 36].Value.ToString().Trim().IsEmpty())
                    {
                        if(DateTime.TryParse(worksheet.Cells[row, 36].Value.ToString().Trim(), out date))
                        {
                            reportDate = DateTime.Parse(worksheet.Cells[row, 36].Value.ToString().Trim());
                        }
                        else
                        {
                            reportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 37].Value != null && !worksheet.Cells[row, 37].Value.ToString().Trim().IsEmpty())
                    {
                        reportRegNumber = worksheet.Cells[row, 37].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 38].Value != null && !worksheet.Cells[row, 38].Value.ToString().Trim().IsEmpty())
                    {
                        
                        if (float.TryParse(worksheet.Cells[row, 29].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            marketValue = worksheet.Cells[row, 29].Value.ToString().Trim();
                            marketValue = marketValue.Replace(',', '.');
                        }
                    }

                    if (worksheet.Cells[row, 39].Value != null && !worksheet.Cells[row, 39].Value.ToString().Trim().IsEmpty())
                    {
                        ExaminingOrgName = worksheet.Cells[row, 39].Value.ToString().Trim();
                    }


                    if (worksheet.Cells[row, 40].Value != null && !worksheet.Cells[row, 40].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 40].Value.ToString().Trim(), out date))
                        {
                            examReportDate = DateTime.Parse(worksheet.Cells[row, 40].Value.ToString().Trim());
                        }
                        else
                        {
                            examReportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 41].Value != null && !worksheet.Cells[row, 41].Value.ToString().Trim().IsEmpty())
                    {
                        examReportRegNumber = worksheet.Cells[row, 41].Value.ToString().Trim();
                    }

                    assetEvaluation = new()
                    {
                        RealEstateId = assetId,
                        EvaluatingOrgName = evaluatingOrgName,
                        ReportDate = reportDate,
                        ReportRegNumber = reportRegNumber,
                        MarketValue = marketValue,
                        ExaminingOrgName = ExaminingOrgName,
                        ExamReportDate = examReportDate,
                        ExamReportRegNumber = examReportRegNumber,
                        ReportStatus = reportStatus,
                        Confirmed = true,
                        StatusChangedDate = DateTime.Now.AddYears(1000)
                    };
                }

                else
                {
                    if (worksheet.Cells[row, 30].Value != null && worksheet.Cells[row, 30].Value.ToString().Trim().Equals("Ишончcиз"))
                    {
                        reportStatus = false;
                    }

                    if (worksheet.Cells[row, 23].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty())
                    {
                        evaluatingOrgName = worksheet.Cells[row, 23].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 24].Value.ToString().Trim(), out date))
                        {
                            reportDate = DateTime.Parse(worksheet.Cells[row, 24].Value.ToString().Trim());
                        }
                        else
                        {
                            reportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 25].Value != null && !worksheet.Cells[row, 25].Value.ToString().Trim().IsEmpty())
                    {
                        reportRegNumber = worksheet.Cells[row, 25].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 26].Value != null && !worksheet.Cells[row, 26].Value.ToString().Trim().IsEmpty())
                    {
                        if (float.TryParse(worksheet.Cells[row, 26].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            marketValue = worksheet.Cells[row, 26].Value.ToString().Trim();
                            marketValue = marketValue.Replace(',', '.');
                        }
                    }

                    if (worksheet.Cells[row, 27].Value != null && !worksheet.Cells[row, 27].Value.ToString().Trim().IsEmpty())
                    {
                        ExaminingOrgName = worksheet.Cells[row, 27].Value.ToString().Trim();
                    }


                    if (worksheet.Cells[row, 28].Value != null && !worksheet.Cells[row, 28].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 28].Value.ToString().Trim(), out date))
                        {
                            examReportDate = DateTime.Parse(worksheet.Cells[row, 28].Value.ToString().Trim());
                        }
                        else
                        {
                            examReportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 29].Value != null && !worksheet.Cells[row, 29].Value.ToString().Trim().IsEmpty())
                    {
                        examReportRegNumber = worksheet.Cells[row, 29].Value.ToString().Trim();
                    }

                    assetEvaluation = new()
                    {
                        RealEstateId = assetId,
                        EvaluatingOrgName = evaluatingOrgName,
                        ReportDate = reportDate,
                        ReportRegNumber = reportRegNumber,
                        MarketValue = marketValue,
                        ExaminingOrgName = ExaminingOrgName,
                        ExamReportDate = examReportDate,
                        ExamReportRegNumber = examReportRegNumber,
                        ReportStatus = reportStatus,
                        Confirmed = true,
                        StatusChangedDate = DateTime.Now.AddYears(1000)
                    };
                }

                await _context.AddAsync(assetEvaluation);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            if (target == 2)
            {
                var share = await _context.Shares.FindAsync(assetId);
                if (share == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган актив топилмади! Активтларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = share.BusinessEntityName;

                if (hasBlock2)
                {
                    if (worksheet.Cells[row, 42].Value != null && worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Ишончcиз"))
                    {
                        reportStatus = false;
                    }

                    if (worksheet.Cells[row, 23].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty())
                    {
                        evaluatingOrgName = worksheet.Cells[row, 23].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 24].Value.ToString().Trim(), out date))
                        {
                            reportDate = DateTime.Parse(worksheet.Cells[row, 24].Value.ToString().Trim());
                        }
                        else
                        {
                            reportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 25].Value != null && !worksheet.Cells[row, 25].Value.ToString().Trim().IsEmpty())
                    {
                        reportRegNumber = worksheet.Cells[row, 25].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 26].Value != null && !worksheet.Cells[row, 26].Value.ToString().Trim().IsEmpty())
                    {
                        if (float.TryParse(worksheet.Cells[row, 26].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            marketValue = worksheet.Cells[row, 26].Value.ToString().Trim();
                            marketValue = marketValue.Replace(',', '.');
                        }
                    }

                    if (worksheet.Cells[row, 27].Value != null && !worksheet.Cells[row, 27].Value.ToString().Trim().IsEmpty())
                    {
                        ExaminingOrgName = worksheet.Cells[row, 27].Value.ToString().Trim();
                    }


                    if (worksheet.Cells[row, 28].Value != null && !worksheet.Cells[row, 28].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 28].Value.ToString().Trim(), out date))
                        {
                            examReportDate = DateTime.Parse(worksheet.Cells[row, 28].Value.ToString().Trim());
                        }
                        else
                        {
                            examReportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 29].Value != null && !worksheet.Cells[row, 29].Value.ToString().Trim().IsEmpty())
                    {
                        examReportRegNumber = worksheet.Cells[row, 29].Value.ToString().Trim();
                    }

                    assetEvaluation = new()
                    {
                        ShareId = assetId,
                        EvaluatingOrgName = evaluatingOrgName,
                        ReportDate = reportDate,
                        ReportRegNumber = reportRegNumber,
                        MarketValue = marketValue,
                        ExaminingOrgName = ExaminingOrgName,
                        ExamReportDate = examReportDate,
                        ExamReportRegNumber = examReportRegNumber,
                        ReportStatus = reportStatus,
                        Confirmed = true,
                        StatusChangedDate = DateTime.Now.AddYears(1000)
                    };
                }

                else
                {
                    if (worksheet.Cells[row, 30].Value != null && worksheet.Cells[row, 30].Value.ToString().Trim().Equals("Ишончcиз"))
                    {
                        reportStatus = false;
                    }

                    if (worksheet.Cells[row, 23].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty())
                    {
                        evaluatingOrgName = worksheet.Cells[row, 23].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 24].Value.ToString().Trim(), out date))
                        {
                            reportDate = DateTime.Parse(worksheet.Cells[row, 24].Value.ToString().Trim());
                        }
                        else
                        {
                            reportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 25].Value != null && !worksheet.Cells[row, 25].Value.ToString().Trim().IsEmpty())
                    {
                        reportRegNumber = worksheet.Cells[row, 25].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 26].Value != null && !worksheet.Cells[row, 26].Value.ToString().Trim().IsEmpty())
                    {
                        if (float.TryParse(worksheet.Cells[row, 26].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            marketValue = worksheet.Cells[row, 26].Value.ToString().Trim();
                            marketValue = marketValue.Replace(',', '.');
                        }
                    }

                    if (worksheet.Cells[row, 27].Value != null && !worksheet.Cells[row, 27].Value.ToString().Trim().IsEmpty())
                    {
                        ExaminingOrgName = worksheet.Cells[row, 27].Value.ToString().Trim();
                    }


                    if (worksheet.Cells[row, 28].Value != null && !worksheet.Cells[row, 28].Value.ToString().Trim().IsEmpty())
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, 28].Value.ToString().Trim(), out date))
                        {
                            examReportDate = DateTime.Parse(worksheet.Cells[row, 28].Value.ToString().Trim());
                        }
                        else
                        {
                            examReportDate = DateTime.MinValue;
                        }
                    }

                    if (worksheet.Cells[row, 29].Value != null && !worksheet.Cells[row, 29].Value.ToString().Trim().IsEmpty())
                    {
                        examReportRegNumber = worksheet.Cells[row, 29].Value.ToString().Trim();
                    }

                    assetEvaluation = new()
                    {
                        ShareId = assetId,
                        EvaluatingOrgName = evaluatingOrgName,
                        ReportDate = reportDate,
                        ReportRegNumber = reportRegNumber,
                        MarketValue = marketValue,
                        ExaminingOrgName = ExaminingOrgName,
                        ExamReportDate = examReportDate,
                        ExamReportRegNumber = examReportRegNumber,
                        ReportStatus = reportStatus,
                        Confirmed = true,
                        StatusChangedDate = DateTime.Now.AddYears(1000)
                    };
                }

                await _context.AddAsync(assetEvaluation);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            result.Add("good");

            return result;


        }

        public async Task<List<string>> AddAuction(int row, ExcelWorksheet worksheet, int assetId, string status, int target)
        {

            List<string> result = new();

            string url = "#";
            DateTime date;
            float number;
            int exposureTime = 0;
            string tradingPlatformName = "";
            string activeValue = "0";
            DateTime biddingExposureDate = new();
            DateTime biddingHoldDate = new();

            if (worksheet.Cells[row, 32].Value == null || worksheet.Cells[row, 32].Value.ToString().Trim().IsEmpty())
            {
                result.Add("bad");
                result.Add(row + "-қаторда <<Активлар сони>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                return result;
            }

            if (worksheet.Cells[row, 35].Value == null || worksheet.Cells[row, 35].Value.ToString().Trim().IsEmpty())
            {
                result.Add("bad");
                result.Add(row + "-қаторда <<Активлар қиймати>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                return result;
            }

            if (worksheet.Cells[row, 31].Value != null && !worksheet.Cells[row, 31].Value.ToString().Trim().IsEmpty())
            {
                tradingPlatformName = worksheet.Cells[row, 31].Value.ToString().Trim();
            }

            if (worksheet.Cells[row, 37].Value != null && !worksheet.Cells[row, 37].Value.ToString().Trim().IsEmpty())
            {
                url = worksheet.Cells[row, 37].Value.ToString().Trim();
            }

            if (worksheet.Cells[row, 33].Value != null && !worksheet.Cells[row, 33].Value.ToString().Trim().Equals(""))
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 33].Value.ToString().Trim(), out date))
                {
                    result.Add("bad");
                    result.Add(row + "-қаторда <<Аукционга қўйилган сана>> { ой/кун/йил } форматида киритилмаган! ");

                    return result;
                }

                else
                {
                    biddingExposureDate = DateTime.Parse(worksheet.Cells[row, 33].Value.ToString().Trim());
                }
            }

            if (worksheet.Cells[row, 36].Value != null && !worksheet.Cells[row, 36].Value.ToString().Trim().Equals(""))
            {
                if (!DateTime.TryParse(worksheet.Cells[row, 36].Value.ToString().Trim(), out date))
                {
                    result.Add("bad");
                    result.Add(row + "-қаторда <<Аукцион ўтказиш санаси>> { ой/кун/йил } форматида киритилмаган! ");

                    return result;
                }

                else
                {
                    biddingHoldDate = DateTime.Parse(worksheet.Cells[row, 36].Value.ToString().Trim());
                }
            }

            if (worksheet.Cells[row, 34].Value != null && !worksheet.Cells[row, 34].Value.ToString().Trim().IsEmpty())
            {
                exposureTime = int.Parse(worksheet.Cells[row, 34].Value.ToString().Trim());
            }

            else if ((biddingHoldDate - biddingExposureDate).Days > 0)
            {
                exposureTime = (biddingHoldDate - biddingExposureDate).Days;
            }

            if (worksheet.Cells[row, 35].Value != null && !worksheet.Cells[row, 35].Value.ToString().Trim().IsEmpty())
            {
                
                if (float.TryParse(worksheet.Cells[row, 35].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                {
                    activeValue = worksheet.Cells[row, 35].Value.ToString().Trim();
                    activeValue = activeValue.Replace(',', '.');
                }
            }

            SubmissionOnBidding auction;

            string assetName;

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FindAsync(assetId);
                if (realEstate == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган объект топилмади! Объектларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = realEstate.RealEstateName;

                auction = new()
                {
                    RealEstateId = assetId,
                    TradingPlatformName = tradingPlatformName,
                    Url = url,
                    AmountOnBidding = Int32.Parse(worksheet.Cells[row, 32].Value.ToString().Trim()),
                    BiddingExposureDate = biddingExposureDate,
                    ExposureTime = exposureTime,
                    ActiveValue = activeValue,
                    BiddingHoldDate = biddingHoldDate,
                    AuctionCancelledDate = DateTime.Now.AddYears(1000),
                    Confirmed = true,
                    Status = status.Trim()

                };

                await _context.AddAsync(auction);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            if (target == 2)
            {
                var share = await _context.Shares.FindAsync(assetId);
                if (share == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган актив топилмади! Активтларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = share.BusinessEntityName;

                auction = new()
                {
                    ShareId = assetId,
                    TradingPlatformName = tradingPlatformName,
                    Url = url,
                    AmountOnBidding = Int32.Parse(worksheet.Cells[row, 32].Value.ToString().Trim()),
                    BiddingExposureDate = biddingExposureDate,
                    ExposureTime = exposureTime,
                    ActiveValue = activeValue,
                    BiddingHoldDate = biddingHoldDate,
                    AuctionCancelledDate = DateTime.Now.AddYears(1000),
                    Confirmed = true,
                    Status = status.Trim()
                };

                await _context.AddAsync(auction);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            result.Add("good");

            return result;


        }

        public async Task<List<string>> AddReduction(int row, ExcelWorksheet worksheet, int assetId, int target)
        {

            List<string> result = new();


            ReductionInAsset reductionInAsset;

            string assetName;
            string percentage = "";
            string amount = "0";
            string assetValueAfterDecline = "0";
            string governingBodyName = "";
            string solutionNumber = "";
            int numberOfSteps = 0;
            DateTime solutionDate = new();
            DateTime date;
            float number;

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FindAsync(assetId);
                if (realEstate == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган объект топилмади! Объектларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = realEstate.RealEstateName;

                if(worksheet.Cells[row, 38].Value != null && !worksheet.Cells[row, 38].Value.ToString().Trim().IsEmpty())
                {
                    governingBodyName = worksheet.Cells[row, 38].Value.ToString().Trim();
                }

                if (worksheet.Cells[row, 40].Value != null && !worksheet.Cells[row, 40].Value.ToString().Trim().IsEmpty())
                {
                    solutionNumber = worksheet.Cells[row, 40].Value.ToString().Trim();
                }

                if (worksheet.Cells[row, 39].Value != null && !worksheet.Cells[row, 39].Value.ToString().Trim().IsEmpty())
                {
                    if(!DateTime.TryParse(worksheet.Cells[row, 39].Value.ToString().Trim(), out date))
                    {
                        result.Add("bad");
                        result.Add($"{row}-қаторда қарор санаси {{ой/кун/йил}} форматида келтирилмаган! - Шу сабабли маълумотларни киритиш шу ерда тўхтатилди!");

                        return result;
                    }

                    else
                    {
                        solutionDate = DateTime.Parse(worksheet.Cells[row, 39].Value.ToString().Trim());
                    }
                        
                }

                if (worksheet.Cells[row, 41].Value != null && !worksheet.Cells[row, 41].Value.ToString().Trim().IsEmpty())
                {
                    percentage = worksheet.Cells[row, 41].Value.ToString().Trim();
                    percentage = percentage.Replace(',', '.');
                }

                if (worksheet.Cells[row, 42].Value != null && !worksheet.Cells[row, 42].Value.ToString().Trim().IsEmpty())
                {
                    
                    if (float.TryParse(worksheet.Cells[row, 42].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    {
                        amount = worksheet.Cells[row, 42].Value.ToString().Trim();
                        amount = amount.Replace(',', '.');
                    }
                }

                if (worksheet.Cells[row, 44].Value != null && !worksheet.Cells[row, 44].Value.ToString().Trim().IsEmpty())
                {
                    
                    if (float.TryParse(worksheet.Cells[row, 44].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    {
                        assetValueAfterDecline = worksheet.Cells[row, 44].Value.ToString().Trim();
                        assetValueAfterDecline = assetValueAfterDecline.Replace(',', '.');
                    }

                }

                if (worksheet.Cells[row, 43].Value != null && !worksheet.Cells[row, 43].Value.ToString().Trim().IsEmpty())
                {
                    numberOfSteps = int.Parse(worksheet.Cells[row, 43].Value.ToString().Trim());
                }
                reductionInAsset = new()
                {
                    RealEstateId = assetId,
                    GoverningBodyName = governingBodyName,
                    SolutionNumber = solutionNumber,
                    SolutionDate = solutionDate,
                    Percentage = percentage,
                    Amount = amount,
                    NumberOfSteps = numberOfSteps,
                    AssetValueAfterDecline = assetValueAfterDecline,
                    StatusChangedDate = DateTime.Now.AddYears(1000),
                    Confirmed = true,
                    Status = true

                };

                await _context.AddAsync(reductionInAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            if (target == 2)
            {
                var share = await _context.Shares.FindAsync(assetId);
                if (share == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган актив топилмади! Активтларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = share.BusinessEntityName;

                if (worksheet.Cells[row, 38].Value != null && !worksheet.Cells[row, 38].Value.ToString().Trim().IsEmpty())
                {
                    governingBodyName = worksheet.Cells[row, 38].Value.ToString().Trim();
                }

                if (worksheet.Cells[row, 40].Value != null && !worksheet.Cells[row, 40].Value.ToString().Trim().IsEmpty())
                {
                    solutionNumber = worksheet.Cells[row, 40].Value.ToString().Trim();
                }

                if (worksheet.Cells[row, 39].Value != null && !worksheet.Cells[row, 39].Value.ToString().Trim().IsEmpty())
                {
                    if (!DateTime.TryParse(worksheet.Cells[row, 39].Value.ToString().Trim(), out date))
                    {
                        result.Add("bad");
                        result.Add($"{row}-қаторда қарор санаси {{ой/кун/йил}} форматида келтирилмаган! - Шу сабабли маълумотларни киритиш шу ерда тўхтатилди!");

                        return result;
                    }

                    else
                    {
                        solutionDate = DateTime.Parse(worksheet.Cells[row, 39].Value.ToString().Trim());
                    }

                }

                if (worksheet.Cells[row, 41].Value != null && !worksheet.Cells[row, 41].Value.ToString().Trim().IsEmpty())
                {
                    percentage = worksheet.Cells[row, 41].Value.ToString().Trim();
                    percentage = percentage.Replace(',', '.');
                }

                if (worksheet.Cells[row, 42].Value != null && !worksheet.Cells[row, 42].Value.ToString().Trim().IsEmpty())
                {                   

                    if (float.TryParse(worksheet.Cells[row, 42].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    {
                        amount = worksheet.Cells[row, 42].Value.ToString().Trim();
                        amount = amount.Replace(',', '.');
                    }

                }

                if (worksheet.Cells[row, 44].Value != null && !worksheet.Cells[row, 44].Value.ToString().Trim().IsEmpty())
                {                  

                    if (float.TryParse(worksheet.Cells[row, 44].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                    {
                        assetValueAfterDecline = worksheet.Cells[row, 44].Value.ToString().Trim();
                        assetValueAfterDecline = assetValueAfterDecline.Replace(',', '.');
                    }
                }

                if (worksheet.Cells[row, 43].Value != null && !worksheet.Cells[row, 43].Value.ToString().Trim().IsEmpty())
                {
                    numberOfSteps = int.Parse(worksheet.Cells[row, 43].Value.ToString().Trim());
                }

                reductionInAsset = new()
                {
                    ShareId = assetId,
                    GoverningBodyName = governingBodyName,
                    SolutionNumber = solutionNumber,
                    SolutionDate = solutionDate,
                    Percentage = percentage,
                    Amount = amount,
                    NumberOfSteps = numberOfSteps,
                    AssetValueAfterDecline = assetValueAfterDecline,
                    StatusChangedDate = DateTime.Now.AddYears(1000),
                    Confirmed = true,
                    Status = true
                };

                await _context.AddAsync(reductionInAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            result.Add("good");

            return result;


        }

        public async Task<List<string>> AddOnetimePaymentAsset(int row, bool hasReduction, ExcelWorksheet worksheet, int assetId, int target)
        {

            List<string> result = new();


            OneTimePaymentAsset oneTimePaymentAsset = new();

            string assetName;

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FindAsync(assetId);
                if (realEstate == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган объект топилмади! Объектларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = realEstate.RealEstateName;

                if (hasReduction)
                {
                    oneTimePaymentAsset = new()
                    {
                        RealEstateId = assetId,
                        GoverningBodyName = worksheet.Cells[row, 45].Value.ToString().Trim(),
                        SolutionNumber = worksheet.Cells[row, 47].Value.ToString().Trim(),
                        SolutionDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim()),
                        BiddingDate = DateTime.Parse(worksheet.Cells[row, 48].Value.ToString().Trim()),
                        BiddingCancelledDate = DateTime.Now.AddYears(1000),
                        Status = "сотувда",
                        Confirmed = true,

                    };
                }

                else
                {
                    oneTimePaymentAsset = new()
                    {
                        RealEstateId = assetId,
                        GoverningBodyName = worksheet.Cells[row, 38].Value.ToString().Trim(),
                        SolutionNumber = worksheet.Cells[row, 40].Value.ToString().Trim(),
                        SolutionDate = DateTime.Parse(worksheet.Cells[row, 39].Value.ToString().Trim()),
                        BiddingDate = DateTime.Parse(worksheet.Cells[row, 41].Value.ToString().Trim()),
                        BiddingCancelledDate = DateTime.Now.AddYears(1000),
                        Status = "сотувда",
                        Confirmed = true,

                    };
                }


                await _context.AddAsync(oneTimePaymentAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            if (target == 2)
            {
                var share = await _context.Shares.FindAsync(assetId);
                if (share == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган актив топилмади! Активтларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = share.BusinessEntityName;

                if (hasReduction)
                {
                    oneTimePaymentAsset = new()
                    {
                        ShareId = assetId,
                        GoverningBodyName = worksheet.Cells[row, 45].Value.ToString().Trim(),
                        SolutionNumber = worksheet.Cells[row, 47].Value.ToString().Trim(),
                        SolutionDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim()),
                        BiddingDate = DateTime.Parse(worksheet.Cells[row, 48].Value.ToString().Trim()),
                        BiddingCancelledDate = DateTime.Now.AddYears(1000),
                        Status = "сотувда",
                        Confirmed = true,

                    };
                }

                else
                {
                    oneTimePaymentAsset = new()
                    {
                        ShareId = assetId,
                        GoverningBodyName = worksheet.Cells[row, 38].Value.ToString().Trim(),
                        SolutionNumber = worksheet.Cells[row, 40].Value.ToString().Trim(),
                        SolutionDate = DateTime.Parse(worksheet.Cells[row, 39].Value.ToString().Trim()),
                        BiddingDate = DateTime.Parse(worksheet.Cells[row, 41].Value.ToString().Trim()),
                        BiddingCancelledDate = DateTime.Now.AddYears(1000),
                        Status = "сотувда",
                        Confirmed = true,

                    };
                }

                await _context.AddAsync(oneTimePaymentAsset);
                await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

            }

            result.Add("good");
            result.Add(oneTimePaymentAsset.OneTimePaymentAssetId.ToString());

            return result;


        }

        public async Task<List<string>> AddInstallment(int row, bool hasEvaluation, ExcelWorksheet worksheet, int assetId, int target)
        {

            List<string> result = new();

            InstallmentAsset installmentAsset = new();

            string assetName;

            DateTime date;
            string governingBodyName = "";
            string solutionNumber = "";
            int installmentTime = 0;
            string actualInitPayment = "0";
            int paymentPeriodType = 0;
            string actualPayment = "0";
            string amountOfAssetSold = "0";

            DateTime solutionDate = new();
            DateTime biddingDate = new();
            DateTime agreementDate = new();
            DateTime actDate = new();
            float number;

            if (target == 1)
            {
                var realEstate = await _context.RealEstates.FindAsync(assetId);

                if (realEstate == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган объект топилмади! Объектларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = realEstate.RealEstateName;


                if (hasEvaluation)
                {
                    if (worksheet.Cells[row, 35].Value == null || worksheet.Cells[row, 35].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Актив харидорининг исми>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 36].Value == null || worksheet.Cells[row, 36].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Активни реализация қилиш суммаси>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    else
                    {
                        if (float.TryParse(worksheet.Cells[row, 36].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            amountOfAssetSold = worksheet.Cells[row, 36].Value.ToString().Trim();
                            amountOfAssetSold = amountOfAssetSold.Replace(',', '.');
                        }
                        else
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Активни реализация қилиш суммаси>> тўғри киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                            return result;
                        }
                    }

                    if (worksheet.Cells[row, 38].Value == null || worksheet.Cells[row, 38].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Шартнома рақами>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 31].Value != null && !worksheet.Cells[row, 31].Value.ToString().Trim().IsEmpty())
                    {
                        governingBodyName = worksheet.Cells[row, 31].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 33].Value != null && !worksheet.Cells[row, 33].Value.ToString().Trim().IsEmpty())
                    {
                        solutionNumber = worksheet.Cells[row, 33].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 32].Value != null && !worksheet.Cells[row, 32].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 32].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Қарор санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            solutionDate = DateTime.Parse(worksheet.Cells[row, 32].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 34].Value != null && !worksheet.Cells[row, 34].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 34].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Сотув санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            biddingDate = DateTime.Parse(worksheet.Cells[row, 34].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 37].Value != null && !worksheet.Cells[row, 37].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 37].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Шартнома санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                            return result;
                        }

                        else
                        {
                            agreementDate = DateTime.Parse(worksheet.Cells[row, 37].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 39].Value != null && !worksheet.Cells[row, 39].Value.ToString().Trim().IsEmpty())
                    {
                        installmentTime = int.Parse(worksheet.Cells[row, 39].Value.ToString().Trim());
                    }

                    if (worksheet.Cells[row, 41].Value != null && !worksheet.Cells[row, 41].Value.ToString().Trim().IsEmpty())
                    {                       

                        if (float.TryParse(worksheet.Cells[row, 41].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            actualInitPayment = worksheet.Cells[row, 41].Value.ToString().Trim();
                            actualInitPayment = actualInitPayment.Replace(',', '.');
                        }
                    }

                    if (worksheet.Cells[row, 42].Value != null && !worksheet.Cells[row, 42].Value.ToString().Trim().IsEmpty())
                    {
                        if (worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Ой"))
                        {
                            paymentPeriodType = 12;
                        }

                        else if (worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Квартал"))
                        {
                            paymentPeriodType = 4;
                        }

                        else if (worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Йил"))
                        {
                            paymentPeriodType = 1;
                        }

                    }

                    if (worksheet.Cells[row, 44].Value != null && !worksheet.Cells[row, 44].Value.ToString().Trim().IsEmpty())
                    {                       
                        if (float.TryParse(worksheet.Cells[row, 44].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            actualPayment = worksheet.Cells[row, 44].Value.ToString().Trim();
                            actualPayment = actualPayment.Replace(',', '.');
                        }
                    }

                    installmentAsset = new()
                    {
                        RealEstateId = assetId,
                        GoverningBodyName = governingBodyName,
                        SolutionNumber = solutionNumber,
                        SolutionDate = solutionDate,
                        BiddingDate = biddingDate,
                        AssetBuyerName = worksheet.Cells[row, 35].Value.ToString().Trim(),
                        AmountOfAssetSold = amountOfAssetSold,
                        AggreementDate = agreementDate,
                        AggreementNumber = worksheet.Cells[row, 38].Value.ToString().Trim(),
                        InstallmentTime = installmentTime,
                        ActualInitPayment = actualInitPayment,
                        PaymentPeriodType = paymentPeriodType,
                        ActualPayment = actualPayment,
                        ContractCancelledDate = DateTime.Now.AddYears(1000),
                        Status = 1,
                        Confirmed = true,

                    };

                    await _context.AddAsync(installmentAsset);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

                    if (worksheet.Cells[row, 46].Value != null && worksheet.Cells[row, 46].Value.ToString().Trim() != "" && worksheet.Cells[row, 47].Value != null && worksheet.Cells[row, 47].Value.ToString().Trim() != "")
                    {

                        if (!DateTime.TryParse(worksheet.Cells[row, 46].Value.ToString().Trim(), out date))
                        {

                            result.Add("bad");
                            result.Add(row + "-қаторда <<Акт санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            actDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim());
                        }


                        InstallmentStep2 installmentStep2 = new()
                        {
                            ActAndAssetDate = actDate,
                            ActAndAssetNumber = worksheet.Cells[row, 47].Value.ToString().Trim(),
                            InstallmentAssetId = installmentAsset.InstallmentAssetId,
                            Confirmed = true
                        };

                        await _context.AddAsync(installmentStep2);
                        await _context.SaveChangesAsync();

                        installmentAsset.InstallmentStep2Id = installmentStep2.InstallmentStep2Id;
                        installmentAsset.Status = 2;

                        await _context.SaveChangesAsync();
                    }
                }

                else
                {
                    if (worksheet.Cells[row, 27].Value == null || worksheet.Cells[row, 27].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Актив харидорининг исми>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 28].Value == null || worksheet.Cells[row, 28].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Активни реализация қилиш суммаси>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }
                    else
                    {
                        if (float.TryParse(worksheet.Cells[row, 28].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            amountOfAssetSold = worksheet.Cells[row, 28].Value.ToString().Trim();
                            amountOfAssetSold = amountOfAssetSold.Replace(',', '.');
                        }
                        else
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Активни реализация қилиш суммаси>> тўғри киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                            return result;
                        }
                    }

                    if (worksheet.Cells[row, 30].Value == null || worksheet.Cells[row, 30].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Шартнома рақами>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 23].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty())
                    {
                        governingBodyName = worksheet.Cells[row, 23].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 25].Value != null && !worksheet.Cells[row, 25].Value.ToString().Trim().IsEmpty())
                    {
                        solutionNumber = worksheet.Cells[row, 25].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 24].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Қарор санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            solutionDate = DateTime.Parse(worksheet.Cells[row, 24].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 26].Value != null && !worksheet.Cells[row, 26].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 26].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Сотув санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            biddingDate = DateTime.Parse(worksheet.Cells[row, 26].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 29].Value != null && !worksheet.Cells[row, 29].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 29].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Шартнома санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                            return result;
                        }

                        else
                        {
                            agreementDate = DateTime.Parse(worksheet.Cells[row, 29].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 31].Value != null && !worksheet.Cells[row, 31].Value.ToString().Trim().IsEmpty())
                    {
                        installmentTime = int.Parse(worksheet.Cells[row, 31].Value.ToString().Trim());
                    }

                    if (worksheet.Cells[row, 33].Value != null && !worksheet.Cells[row, 33].Value.ToString().Trim().IsEmpty())
                    {
                        
                        if (float.TryParse(worksheet.Cells[row, 33].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            actualInitPayment = worksheet.Cells[row, 33].Value.ToString().Trim();
                            actualInitPayment = actualInitPayment.Replace(',', '.');
                        }
                    }

                    if (worksheet.Cells[row, 34].Value != null && !worksheet.Cells[row, 34].Value.ToString().Trim().IsEmpty())
                    {
                        if (worksheet.Cells[row, 34].Value.ToString().Trim().Equals("Ой"))
                        {
                            paymentPeriodType = 12;
                        }

                        else if (worksheet.Cells[row, 34].Value.ToString().Trim().Equals("Квартал"))
                        {
                            paymentPeriodType = 4;
                        }

                        else if (worksheet.Cells[row, 34].Value.ToString().Trim().Equals("Йил"))
                        {
                            paymentPeriodType = 1;
                        }

                    }

                    if (worksheet.Cells[row, 36].Value != null && !worksheet.Cells[row, 36].Value.ToString().Trim().IsEmpty())
                    {                        

                        if (float.TryParse(worksheet.Cells[row, 36].Value.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out number))
                        {
                            actualPayment = worksheet.Cells[row, 36].Value.ToString().Trim();
                            actualPayment = actualPayment.Replace(',', '.');
                        }
                    }

                    installmentAsset = new()
                    {
                        RealEstateId = assetId,
                        GoverningBodyName = governingBodyName,
                        SolutionNumber = solutionNumber,
                        SolutionDate = solutionDate,
                        BiddingDate = biddingDate,
                        AssetBuyerName = worksheet.Cells[row, 27].Value.ToString().Trim(),
                        AmountOfAssetSold = amountOfAssetSold,
                        AggreementDate = agreementDate,
                        AggreementNumber = worksheet.Cells[row, 30].Value.ToString().Trim(),
                        InstallmentTime = installmentTime,
                        ActualInitPayment = actualInitPayment,
                        PaymentPeriodType = paymentPeriodType,
                        ActualPayment = actualPayment,
                        ContractCancelledDate = DateTime.Now.AddYears(1000),
                        Status = 1,
                        Confirmed = true,

                    };

                    await _context.AddAsync(installmentAsset);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

                    if (worksheet.Cells[row, 38].Value != null && worksheet.Cells[row, 38].Value.ToString().Trim() != "" && worksheet.Cells[row, 39].Value != null && worksheet.Cells[row, 39].Value.ToString().Trim() != "")
                    {

                        if (!DateTime.TryParse(worksheet.Cells[row, 38].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Акт санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;

                        }

                        else
                        {
                            actDate = DateTime.Parse(worksheet.Cells[row, 38].Value.ToString().Trim());
                        }


                        InstallmentStep2 installmentStep2 = new()
                        {
                            ActAndAssetDate = actDate,
                            ActAndAssetNumber = worksheet.Cells[row, 39].Value.ToString().Trim(),
                            InstallmentAssetId = installmentAsset.InstallmentAssetId,
                            Confirmed = true

                        };

                        await _context.AddAsync(installmentStep2);
                        await _context.SaveChangesAsync();

                        installmentAsset.InstallmentStep2Id = installmentStep2.InstallmentStep2Id;
                        installmentAsset.Status = 2;

                        await _context.SaveChangesAsync();
                    }
                }


            }

            if (target == 2)
            {
                var share = await _context.Shares.FindAsync(assetId);
                if (share == null)
                {
                    result.Add("bad");
                    result.Add("Тизимда хатолик юз берди - " + row + "- қатордаги киритилган актив топилмади! Активларни киритиш шу қаторда тўхтатилди!");
                    return result;
                }

                assetName = share.BusinessEntityName;

                if (hasEvaluation)
                {
                    if (worksheet.Cells[row, 35].Value == null || worksheet.Cells[row, 35].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Актив харидорининг исми>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 36].Value == null || worksheet.Cells[row, 36].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Активни реализация қилиш суммаси>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    else
                    {
                        amountOfAssetSold = worksheet.Cells[row, 36].Value.ToString().Trim();
                        amountOfAssetSold = amountOfAssetSold.Replace(',', '.');
                    }

                    if (worksheet.Cells[row, 38].Value == null || worksheet.Cells[row, 38].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Шартнома рақами>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 31].Value != null && !worksheet.Cells[row, 31].Value.ToString().Trim().IsEmpty())
                    {
                        governingBodyName = worksheet.Cells[row, 31].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 33].Value != null && !worksheet.Cells[row, 33].Value.ToString().Trim().IsEmpty())
                    {
                        solutionNumber = worksheet.Cells[row, 33].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 32].Value != null && !worksheet.Cells[row, 32].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 32].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Қарор санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            solutionDate = DateTime.Parse(worksheet.Cells[row, 32].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 34].Value != null && !worksheet.Cells[row, 34].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 34].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Сотув санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            biddingDate = DateTime.Parse(worksheet.Cells[row, 34].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 37].Value != null && !worksheet.Cells[row, 37].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 37].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Шартнома санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                            return result;
                        }

                        else
                        {
                            agreementDate = DateTime.Parse(worksheet.Cells[row, 37].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 39].Value != null && !worksheet.Cells[row, 39].Value.ToString().Trim().IsEmpty())
                    {
                        installmentTime = int.Parse(worksheet.Cells[row, 39].Value.ToString().Trim());
                    }

                    if (worksheet.Cells[row, 41].Value != null && !worksheet.Cells[row, 41].Value.ToString().Trim().IsEmpty())
                    {
                        actualInitPayment = worksheet.Cells[row, 41].Value.ToString().Trim();
                        actualInitPayment = actualInitPayment.Replace(',', '.');
                    }

                    if (worksheet.Cells[row, 42].Value != null && !worksheet.Cells[row, 42].Value.ToString().Trim().IsEmpty())
                    {
                        if (worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Ой"))
                        {
                            paymentPeriodType = 12;
                        }

                        else if (worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Квартал"))
                        {
                            paymentPeriodType = 4;
                        }

                        else if (worksheet.Cells[row, 42].Value.ToString().Trim().Equals("Йил"))
                        {
                            paymentPeriodType = 1;
                        }

                    }

                    if (worksheet.Cells[row, 44].Value != null && !worksheet.Cells[row, 44].Value.ToString().Trim().IsEmpty())
                    {
                        actualPayment = worksheet.Cells[row, 44].Value.ToString().Trim();
                        actualPayment = actualPayment.Replace(",", ".");
                    }

                    installmentAsset = new()
                    {
                        ShareId = assetId,
                        GoverningBodyName = governingBodyName,
                        SolutionNumber = solutionNumber,
                        SolutionDate = solutionDate,
                        BiddingDate = biddingDate,
                        AssetBuyerName = worksheet.Cells[row, 35].Value.ToString().Trim(),
                        AmountOfAssetSold = amountOfAssetSold,
                        AggreementDate = agreementDate,
                        AggreementNumber = worksheet.Cells[row, 38].Value.ToString().Trim(),
                        InstallmentTime = installmentTime,
                        ActualInitPayment = actualInitPayment,
                        PaymentPeriodType = paymentPeriodType,
                        ActualPayment = actualPayment,
                        ContractCancelledDate = DateTime.Now.AddYears(1000),
                        Status = 1,
                        Confirmed = true,

                    };

                    await _context.AddAsync(installmentAsset);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

                    if (worksheet.Cells[row, 46].Value != null && worksheet.Cells[row, 46].Value.ToString().Trim() != "" && worksheet.Cells[row, 47].Value != null && worksheet.Cells[row, 47].Value.ToString().Trim() != "")
                    {

                        if (!DateTime.TryParse(worksheet.Cells[row, 46].Value.ToString().Trim(), out date))
                        {

                            result.Add("bad");
                            result.Add(row + "-қаторда <<Акт санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            actDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim());
                        }


                        InstallmentStep2 installmentStep2 = new()
                        {
                            ActAndAssetDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim()),
                            ActAndAssetNumber = worksheet.Cells[row, 47].Value.ToString().Trim(),
                            InstallmentAssetId = installmentAsset.InstallmentAssetId,
                            Confirmed = true
                        };

                        await _context.AddAsync(installmentStep2);
                        await _context.SaveChangesAsync();

                        installmentAsset.InstallmentStep2Id = installmentStep2.InstallmentStep2Id;
                        installmentAsset.Status = 2;

                        await _context.SaveChangesAsync();
                    }
                }

                else
                {
                    if (worksheet.Cells[row, 27].Value == null || worksheet.Cells[row, 27].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Актив харидорининг исми>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 28].Value == null || worksheet.Cells[row, 28].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Активни реализация қилиш суммаси>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    else
                    {
                        amountOfAssetSold = worksheet.Cells[row, 28].Value.ToString().Trim();
                        amountOfAssetSold = amountOfAssetSold.Replace(',', '.');
                    }


                    if (worksheet.Cells[row, 30].Value == null || worksheet.Cells[row, 30].Value.ToString().Trim().IsEmpty())
                    {
                        result.Add("bad");
                        result.Add(row + "-қаторда <<Шартнома рақами>> киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                        return result;
                    }

                    if (worksheet.Cells[row, 23].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty())
                    {
                        governingBodyName = worksheet.Cells[row, 23].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 25].Value != null && !worksheet.Cells[row, 25].Value.ToString().Trim().IsEmpty())
                    {
                        solutionNumber = worksheet.Cells[row, 25].Value.ToString().Trim();
                    }

                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 24].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Қарор санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            solutionDate = DateTime.Parse(worksheet.Cells[row, 24].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 26].Value != null && !worksheet.Cells[row, 26].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 26].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Сотув санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            biddingDate = DateTime.Parse(worksheet.Cells[row, 26].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 29].Value != null && !worksheet.Cells[row, 29].Value.ToString().Trim().Equals(""))
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 29].Value.ToString().Trim(), out date))
                        {
                            result.Add("bad");
                            result.Add(row + "-қаторда <<Шартнома санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли, " + row + "-қаторга келиб объектларни киритиш тўхтади.");

                            return result;
                        }

                        else
                        {
                            agreementDate = DateTime.Parse(worksheet.Cells[row, 29].Value.ToString().Trim());
                        }
                    }

                    if (worksheet.Cells[row, 31].Value != null && !worksheet.Cells[row, 31].Value.ToString().Trim().IsEmpty())
                    {
                        installmentTime = int.Parse(worksheet.Cells[row, 31].Value.ToString().Trim());
                    }

                    if (worksheet.Cells[row, 33].Value != null && !worksheet.Cells[row, 33].Value.ToString().Trim().IsEmpty())
                    {
                        actualInitPayment = worksheet.Cells[row, 33].Value.ToString().Trim();
                        actualInitPayment = actualInitPayment.Replace(',', '.');
                    }

                    if (worksheet.Cells[row, 34].Value != null && !worksheet.Cells[row, 34].Value.ToString().Trim().IsEmpty())
                    {
                        if (worksheet.Cells[row, 34].Value.ToString().Trim().Equals("Ой"))
                        {
                            paymentPeriodType = 12;
                        }

                        else if (worksheet.Cells[row, 34].Value.ToString().Trim().Equals("Квартал"))
                        {
                            paymentPeriodType = 4;
                        }

                        else if (worksheet.Cells[row, 34].Value.ToString().Trim().Equals("Йил"))
                        {
                            paymentPeriodType = 1;
                        }

                    }

                    if (worksheet.Cells[row, 36].Value != null && !worksheet.Cells[row, 36].Value.ToString().Trim().IsEmpty())
                    {
                        actualPayment = worksheet.Cells[row, 36].Value.ToString().Trim();
                        actualPayment = actualPayment.Replace(',', '.');
                    }

                    installmentAsset = new()
                    {
                        ShareId = assetId,
                        GoverningBodyName = governingBodyName,
                        SolutionNumber = solutionNumber,
                        SolutionDate = solutionDate,
                        BiddingDate = biddingDate,
                        AssetBuyerName = worksheet.Cells[row, 27].Value.ToString().Trim(),
                        AmountOfAssetSold = amountOfAssetSold,
                        AggreementDate = agreementDate,
                        AggreementNumber = worksheet.Cells[row, 30].Value.ToString().Trim(),
                        InstallmentTime = installmentTime,
                        ActualInitPayment = actualInitPayment,
                        PaymentPeriodType = paymentPeriodType,
                        ActualPayment = actualPayment,
                        ContractCancelledDate = DateTime.Now.AddYears(1000),
                        Status = 1,
                        Confirmed = true,

                    };

                    await _context.AddAsync(installmentAsset);
                    await _context.SaveChangesAsync(_userManager.GetUserId(User), assetName);

                    if (worksheet.Cells[row, 46].Value != null && worksheet.Cells[row, 46].Value.ToString().Trim() != "" && worksheet.Cells[row, 47].Value != null && worksheet.Cells[row, 47].Value.ToString().Trim() != "")
                    {

                        if (!DateTime.TryParse(worksheet.Cells[row, 46].Value.ToString().Trim(), out date))
                        {

                            result.Add("bad");
                            result.Add(row + "-қаторда <<Акт санаси>> { ой/кун/йил } форматида киритилмаган! - Шу сабабли," + row + "-қаторга келиб объектларни киритиш тўхтади. ");

                            return result;
                        }

                        else
                        {
                            actDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim());
                        }


                        InstallmentStep2 installmentStep2 = new()
                        {
                            ActAndAssetDate = DateTime.Parse(worksheet.Cells[row, 46].Value.ToString().Trim()),
                            ActAndAssetNumber = worksheet.Cells[row, 47].Value.ToString().Trim(),
                            InstallmentAssetId = installmentAsset.InstallmentAssetId,
                            Confirmed = true
                        };

                        await _context.AddAsync(installmentStep2);
                        await _context.SaveChangesAsync();

                        installmentAsset.InstallmentStep2Id = installmentStep2.InstallmentStep2Id;
                        installmentAsset.Status = 2;

                        await _context.SaveChangesAsync();
                    }
                }

            }

            await MakeAssetOutOfAccount(assetId, target, actDate);

            result.Add("good");

            result.Add(installmentAsset.InstallmentAssetId.ToString());

            return result;

        }

        /*Definitions
          block1 - RealEstate/Share;
          block2 - TransferredAssets;
          block3 - AssetEvaluations;
          block4 - SubmissionOnBiddings;
          block5 - ReductionInAssets;
          block6 - OneTimePaymentAssets;
          block7 - InstallmentAssets;
          block8 - Users;
         */
        [HttpPost]
        public async Task<IActionResult> ExcelImport([FromForm] IFormFile templateFile,
                                                     [FromForm] string target, [FromForm] string block2, [FromForm] string block3, [FromForm] string block4,
                                                     [FromForm] string block5, [FromForm] string block6, [FromForm] string block7, [FromForm] string block8)
        {
            if (templateFile == null)
            {
                return Json(new { success = false, message = "Файлни юборишда хатолик юз берди!" });
            }

            if (target == null)
            {
                return Json(new { success = false, message = "Актив турини аниқлашда хатолик юз берди!" });
            }

            int _target = int.Parse(target);

            string fullName;
            string fullNameCheck;
            string email;
            string createdById;
            string role;
            string applicationUserId = "";
            bool isUserAvailable = false;
            int organizationId;
            string realEstateName = "";

            DateTime date;

            using (var stream = new MemoryStream())
            {
                await templateFile.CopyToAsync(stream);
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        int rowcount = 0;
                        int emptyRowCount = 0;
                        var start = worksheet.Dimension.Start;
                        var end = worksheet.Dimension.End;
                        int amount = 0;

                        for (int row = 3; row <= end.Row; row++)
                        {
                            string cellValue = worksheet.Cells[row, 1].Text;
                            if (cellValue.Equals(""))
                            {
                                emptyRowCount++;

                                if (emptyRowCount > 4)
                                {
                                    rowcount = row - 4;
                                    break;
                                }
                                
                            }
                            else 
                            {
                                emptyRowCount=0;                                
                            }

                            rowcount = row + 1;
                        }

                        if (rowcount < 4)
                        {
                            return Json(new { success = false, message = "Биринчи 5 қаторда маълумот мавжуд эмас - маълумот киритилмади!" });
                        }

                        for (int row = 3; row < rowcount; row++)
                        {
                            
                            string cellValue = worksheet.Cells[row, 1].Text;

                            if (cellValue.Equals(""))
                            {
                                continue;
                            }                     

                            if (block2 == null && block3 == null && block4 == null && block5 == null && block6 == null && block7 == null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }
                            }

                            if (block2 == null && block3 == null && block4 == null && block5 == null && block6 == null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 26].Value != null)
                                    email = worksheet.Cells[row, 26].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 23].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty())
                                {
                                    fullName = worksheet.Cells[row, 23].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                 return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                            isUserAvailable = false;

                            foreach (var item in userList)
                            {
                                var fName = item.FirstName.Trim().ToLower();
                                var lName = item.LastName.Trim().ToLower();
                                var mName = item.MiddleName.Trim().ToLower();

                                var fullName1 = lName + fName + mName;
                                var fullName2 = lName + mName + fName;
                                var fullName3 = mName + fName + lName;
                                var fullName4 = mName + lName + fName;
                                var fullName5 = fName + lName + mName;
                                var fullName6 = fName + mName + lName;

                                if (item.Email == email)
                                {
                                    isUserAvailable = true;
                                }


                                else if (fullName1.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName2.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName3.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName4.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName5.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName6.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                if (isUserAvailable)
                                {
                                    applicationUserId = item.Id;
                                    break;
                                }


                            }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else 
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }                                  

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 24].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 24].Value != null && !worksheet.Cells[row, 24].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 25].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }
                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }
                            }

                            if (block2 != null && block3 == null && block4 == null && block5 == null && block6 == null && block7 == null && block8 == null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                List<string> result = AddTransferredAsset(row, worksheet, applicationUserId, _target).Result;
                                if (result != null && result[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = result[1] });
                                }


                            }

                            if (block2 != null && block3 == null && block4 == null && block5 == null && block6 == null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 38].Value != null || !worksheet.Cells[row, 26].Value.Equals(""))
                                    email = worksheet.Cells[row, 38].Value.ToString().Trim();
                                else
                                    email = "";

                                if (worksheet.Cells[row, 35].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().Equals("")) { 
                                    fullName = worksheet.Cells[row, 35].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName + mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 36].Value != null && !worksheet.Cells[row, 36].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 36].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 37].Value != null && !worksheet.Cells[row, 37].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 37].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                List<string> transferresAssetResult = AddTransferredAsset(row, worksheet, applicationUserId, _target).Result;
                                if (transferresAssetResult != null && transferresAssetResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = transferresAssetResult[1] });
                                }
                            }

                            if (block2 != null && block3 != null && block4 == null && block5 == null && block6 == null && block7 == null && block8 == null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                List<string> result = AddTransferredAsset(row, worksheet, applicationUserId, _target).Result;
                                if (result[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = result[1] });
                                }
                                int assetId = Int32.Parse(result[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, true, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }



                            }

                            if (block2 != null && block3 != null && block4 == null && block5 == null && block6 == null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 46].Value != null || !worksheet.Cells[row, 26].Value.Equals(""))
                                    email = worksheet.Cells[row, 46].Value.ToString().Trim();
                                else
                                    email = "";

                                if (worksheet.Cells[row, 43].Value != null && !worksheet.Cells[row, 23].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 43].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName + mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 44].Value != null && !worksheet.Cells[row, 44].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 44].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 45].Value != null && !worksheet.Cells[row, 45].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 45].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }


                                List<string> transferredAssetResult = AddTransferredAsset(row, worksheet, applicationUserId, _target).Result;
                                if (transferredAssetResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = transferredAssetResult[1] });
                                }
                                int assetId = Int32.Parse(transferredAssetResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, true, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }



                            }

                            if (block2 == null && block3 != null && block4 == null && block5 == null && block6 == null && block7 == null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                            }

                            if (block2 == null && block3 != null && block4 == null && block5 == null && block6 == null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });
                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 34].Value != null)
                                    email = worksheet.Cells[row, 34].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 31].Value != null && !worksheet.Cells[row, 31].Value.ToString().Trim().Equals(""))
                                {
                                    fullName = worksheet.Cells[row, 31].Value.ToString().Trim();
                                    fullNameCheck = Regex.Replace(fullName.ToLower(), @"\s+", "");
                                }
                                    
                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName +mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 32].Value != null && !worksheet.Cells[row, 32].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 32].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 33].Value != null && !worksheet.Cells[row, 33].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 33].Value.ToString().Trim();
                                    }                                   

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 == null && block6 == null && block7 == null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "сотувда";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 == null && block6 == null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 41].Value != null)
                                    email = worksheet.Cells[row, 41].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 38].Value != null && !worksheet.Cells[row, 38].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 38].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName + mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {

                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }

                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;
                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 39].Value != null && !worksheet.Cells[row, 39].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 39].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 40].Value != null && !worksheet.Cells[row, 40].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 40].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "сотувда";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 != null && block6 == null && block7 == null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "Cотувда";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }

                                List<string> reducitonResult = AddReduction(row, worksheet, assetId, _target).Result;

                                if (reducitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = reducitonResult[1] });
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 != null && block6 == null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });
                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 48].Value != null)
                                    email = worksheet.Cells[row, 48].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 45].Value != null && !worksheet.Cells[row, 45].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 45].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName + mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 46].Value != null && !worksheet.Cells[row, 46].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 46].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 47].Value != null && !worksheet.Cells[row, 47].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 47].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "Сотувда";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }

                                List<string> reducitonResult = AddReduction(row, worksheet, assetId, _target).Result;

                                if (reducitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = reducitonResult[1] });
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 != null && block6 != null && block7 == null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "Cотилди";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }

                                List<string> reducitonResult = AddReduction(row, worksheet, assetId, _target).Result;

                                if (reducitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = reducitonResult[1] });
                                }

                                List<string> oneTimePaymentResult = AddOnetimePaymentAsset(row, true, worksheet, assetId, _target).Result;

                                if (oneTimePaymentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = oneTimePaymentResult[1] });
                                }

                                if (worksheet.Cells[row, 49].Value != null && worksheet.Cells[row, 49].Value.ToString().Trim() != "" && worksheet.Cells[row, 50].Value != null && worksheet.Cells[row, 50].Value.ToString().Trim() != "")
                                {
                                    DateTime agreementDate = new();
                                    string agreementNumber = "";
                                    string amountPayed = "";

                                    if (worksheet.Cells[row, 51].Value != null && worksheet.Cells[row, 51].Value.ToString().Trim() != "")
                                    {
                                        if (!DateTime.TryParse(worksheet.Cells[row, 54].Value.ToString().Trim(), out date))
                                        {

                                            return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                        }

                                        else
                                        {
                                            agreementDate = DateTime.Parse(worksheet.Cells[row, 51].Value.ToString().Trim());
                                        }
                                    }

                                    if (worksheet.Cells[row, 52].Value != null && worksheet.Cells[row, 52].Value.ToString().Trim() != "")
                                    {
                                        agreementNumber = worksheet.Cells[row, 52].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 53].Value != null && worksheet.Cells[row, 53].Value.ToString().Trim() != "")
                                    {
                                        amountPayed = worksheet.Cells[row, 53].Value.ToString().Trim();
                                        amountPayed = amountPayed.Replace(',', '.');
                                    }

                                    OneTimePaymentStep2 oneTimePaymentStep2 = new()
                                    {
                                        AssetBuyerName = worksheet.Cells[row, 49].Value.ToString().Trim(),
                                        AmountOfAssetSold = worksheet.Cells[row, 50].Value.ToString().Trim(),
                                        AggreementDate = agreementDate,
                                        AggreementNumber = agreementNumber,
                                        AmountPayed = amountPayed,
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep2);
                                    await _context.SaveChangesAsync();

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep2Id = oneTimePaymentStep2.OneTimePaymentStep2Id;
                                        oneTimePaymentAsset.Status = "шартнома";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }

                                if (worksheet.Cells[row, 54].Value != null && worksheet.Cells[row, 54].Value.ToString().Trim() != "" && worksheet.Cells[row, 55].Value != null && worksheet.Cells[row, 55].Value.ToString().Trim() != "")
                                {
                                    if (!DateTime.TryParse(worksheet.Cells[row, 54].Value.ToString().Trim(), out date))
                                    {

                                        return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                    }

                                    DateTime actDate = DateTime.Parse(worksheet.Cells[row, 54].Value.ToString().Trim());

                                    OneTimePaymentStep3 oneTimePaymentStep3 = new()
                                    {
                                        ActAndAssetDate = actDate,
                                        ActAndAssetNumber = worksheet.Cells[row, 55].Value.ToString().Trim(),
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep3);
                                    await _context.SaveChangesAsync();

                                    await MakeAssetOutOfAccount(assetId, _target, actDate);

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));

                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep3Id = oneTimePaymentStep3.OneTimePaymentStep3Id;
                                        oneTimePaymentAsset.Status = "акт";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 != null && block6 != null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });
                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 59].Value != null)
                                    email = worksheet.Cells[row, 59].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 56].Value != null && !worksheet.Cells[row, 56].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 56].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                            else
                                return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                            isUserAvailable = false;

                            foreach (var item in userList)
                            {
                                var fName = item.FirstName.Trim().ToLower();
                                var lName = item.LastName.Trim().ToLower();
                                var mName = item.MiddleName.Trim().ToLower();

                                var fullName1 = lName + fName + mName;
                                var fullName2 = lName + mName + fName;
                                var fullName3 = mName + fName + lName;
                                var fullName4 = mName + lName + fName;
                                var fullName5 = fName + lName + mName;
                                var fullName6 = fName + mName + lName;

                                if (item.Email == email)
                                {
                                    isUserAvailable = true;
                                }


                                else if (fullName1.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName2.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName3.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName4.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName5.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName6.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                if (isUserAvailable)
                                {
                                    applicationUserId = item.Id;
                                    break;
                                }


                            }

                            if (!isUserAvailable)
                            {
                                string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                string firstName = "";
                                string lastName = "";
                                string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 57].Value != null && !worksheet.Cells[row, 57].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 57].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 58].Value != null && !worksheet.Cells[row, 58].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 58].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "Cотилди";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }

                                List<string> reducitonResult = AddReduction(row, worksheet, assetId, _target).Result;

                                if (reducitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = reducitonResult[1] });
                                }

                                List<string> oneTimePaymentResult = AddOnetimePaymentAsset(row, true, worksheet, assetId, _target).Result;

                                if (oneTimePaymentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = oneTimePaymentResult[1] });
                                }

                                if (worksheet.Cells[row, 49].Value != null && worksheet.Cells[row, 49].Value.ToString().Trim() != "" && worksheet.Cells[row, 50].Value != null && worksheet.Cells[row, 50].Value.ToString().Trim() != "")
                                {
                                    DateTime agreementDate = new();
                                    string agreementNumber = "";
                                    string amountPayed = "";

                                    if (worksheet.Cells[row, 51].Value != null && worksheet.Cells[row, 51].Value.ToString().Trim() != "")
                                    {
                                        if (!DateTime.TryParse(worksheet.Cells[row, 54].Value.ToString().Trim(), out date))
                                        {

                                            return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                        }

                                        else
                                        {
                                            agreementDate = DateTime.Parse(worksheet.Cells[row, 51].Value.ToString().Trim());
                                        }
                                    }

                                    if (worksheet.Cells[row, 52].Value != null && worksheet.Cells[row, 52].Value.ToString().Trim() != "")
                                    {
                                        agreementNumber = worksheet.Cells[row, 52].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 53].Value != null && worksheet.Cells[row, 53].Value.ToString().Trim() != "")
                                    {
                                        amountPayed = worksheet.Cells[row, 53].Value.ToString().Trim();
                                        amountPayed = amountPayed.Replace(',', '.');
                                    }

                                    OneTimePaymentStep2 oneTimePaymentStep2 = new()
                                    {
                                        AssetBuyerName = worksheet.Cells[row, 49].Value.ToString().Trim(),
                                        AmountOfAssetSold = worksheet.Cells[row, 50].Value.ToString().Trim(),
                                        AggreementDate = agreementDate,
                                        AggreementNumber = agreementNumber,
                                        AmountPayed = amountPayed,
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep2);
                                    await _context.SaveChangesAsync();

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep2Id = oneTimePaymentStep2.OneTimePaymentStep2Id;
                                        oneTimePaymentAsset.Status = "шартнома";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }

                                if (worksheet.Cells[row, 54].Value != null && worksheet.Cells[row, 54].Value.ToString().Trim() != "" && worksheet.Cells[row, 55].Value != null && worksheet.Cells[row, 55].Value.ToString().Trim() != "")
                                {
                                    if (!DateTime.TryParse(worksheet.Cells[row, 54].Value.ToString().Trim(), out date))
                                    {

                                        return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                    }

                                    DateTime actDate = DateTime.Parse(worksheet.Cells[row, 54].Value.ToString().Trim());

                                    OneTimePaymentStep3 oneTimePaymentStep3 = new()
                                    {
                                        ActAndAssetDate = actDate,
                                        ActAndAssetNumber = worksheet.Cells[row, 55].Value.ToString().Trim(),
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep3);
                                    await _context.SaveChangesAsync();

                                    await MakeAssetOutOfAccount(assetId, _target, actDate);

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep3Id = oneTimePaymentStep3.OneTimePaymentStep3Id;
                                        oneTimePaymentAsset.Status = "акт";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 == null && block6 != null && block7 == null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "Сотилди";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }

                                List<string> oneTimePaymentResult = AddOnetimePaymentAsset(row, false, worksheet, assetId, _target).Result;

                                if (oneTimePaymentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = oneTimePaymentResult[1] });
                                }

                                if (worksheet.Cells[row, 42].Value != null && worksheet.Cells[row, 42].Value.ToString().Trim() != "" && worksheet.Cells[row, 43].Value != null && worksheet.Cells[row, 43].Value.ToString().Trim() != "")
                                {
                                    DateTime agreementDate = new();
                                    string agreementNumber = "";
                                    string amountPayed = "";

                                    if (worksheet.Cells[row, 44].Value != null && worksheet.Cells[row, 44].Value.ToString().Trim() != "")
                                    {
                                        if (!DateTime.TryParse(worksheet.Cells[row, 44].Value.ToString().Trim(), out date))
                                        {

                                            return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                        }

                                        else
                                        {
                                            agreementDate = DateTime.Parse(worksheet.Cells[row, 44].Value.ToString().Trim());
                                        }
                                    }

                                    if (worksheet.Cells[row, 45].Value != null && worksheet.Cells[row, 45].Value.ToString().Trim() != "")
                                    {
                                        agreementNumber = worksheet.Cells[row, 45].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 46].Value != null && worksheet.Cells[row, 46].Value.ToString().Trim() != "")
                                    {
                                        amountPayed = worksheet.Cells[row, 46].Value.ToString().Trim();
                                        amountPayed = amountPayed.Replace(',', '.');
                                    }

                                    OneTimePaymentStep2 oneTimePaymentStep2 = new()
                                    {
                                        AssetBuyerName = worksheet.Cells[row, 42].Value.ToString().Trim(),
                                        AmountOfAssetSold = worksheet.Cells[row, 43].Value.ToString().Trim(),
                                        AggreementDate = agreementDate,
                                        AggreementNumber = agreementNumber,
                                        AmountPayed = amountPayed,
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep2);
                                    await _context.SaveChangesAsync();

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep2Id = oneTimePaymentStep2.OneTimePaymentStep2Id;
                                        oneTimePaymentAsset.Status = "шартнома";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }

                                if (worksheet.Cells[row, 47].Value != null && worksheet.Cells[row, 47].Value.ToString().Trim() != "" && worksheet.Cells[row, 48].Value != null && worksheet.Cells[row, 48].Value.ToString().Trim() != "")
                                {
                                    if (!DateTime.TryParse(worksheet.Cells[row, 47].Value.ToString().Trim(), out date))
                                    {

                                        return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                    }

                                    DateTime actDate = DateTime.Parse(worksheet.Cells[row, 47].Value.ToString().Trim());

                                    OneTimePaymentStep3 oneTimePaymentStep3 = new()
                                    {
                                        ActAndAssetDate = actDate,
                                        ActAndAssetNumber = worksheet.Cells[row, 48].Value.ToString().Trim(),
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep3);
                                    await _context.SaveChangesAsync();

                                    await MakeAssetOutOfAccount(assetId, _target, actDate);

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep3Id = oneTimePaymentStep3.OneTimePaymentStep3Id;
                                        oneTimePaymentAsset.Status = "акт";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }
                            }

                            if (block2 == null && block3 != null && block4 != null && block5 == null && block6 != null && block7 == null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });
                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 52].Value != null)
                                    email = worksheet.Cells[row, 52].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 49].Value != null && !worksheet.Cells[row, 49].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 49].Value.ToString().Trim();
                                fullNameCheck = fullName.ToLower();
                                fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");   
                            }

                            else
                                return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                            isUserAvailable = false;

                            foreach (var item in userList)
                            {
                                var fName = item.FirstName.Trim().ToLower();
                                var lName = item.LastName.Trim().ToLower();
                                var mName = item.MiddleName.Trim().ToLower();

                                var fullName1 = lName + fName + mName;
                                var fullName2 = lName + mName + fName;
                                var fullName3 = mName + fName + lName;
                                var fullName4 = mName + lName + fName;
                                var fullName5 = fName + lName + mName;
                                var fullName6 = fName + mName + lName;

                                if (item.Email == email)
                                {
                                    isUserAvailable = true;
                                }


                                else if (fullName1.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName2.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName3.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName4.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName5.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                else if (fullName6.Equals(fullNameCheck))
                                {
                                    isUserAvailable = true;
                                }

                                if (isUserAvailable)
                                {
                                    applicationUserId = item.Id;
                                    break;
                                }


                            }

                            if (!isUserAvailable)
                            {
                                string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                string firstName = "";
                                string lastName = "";
                                string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 50].Value != null && !worksheet.Cells[row, 50].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 50].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 51].Value != null && !worksheet.Cells[row, 51].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 51].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                string status = "Сотилди";

                                List<string> aucitonResult = AddAuction(row, worksheet, assetId, status, _target).Result;

                                if (aucitonResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = aucitonResult[1] });
                                }


                                List<string> oneTimePaymentResult = AddOnetimePaymentAsset(row, false, worksheet, assetId, _target).Result;

                                if (oneTimePaymentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = oneTimePaymentResult[1] });
                                }


                                if (worksheet.Cells[row, 42].Value != null && worksheet.Cells[row, 42].Value.ToString().Trim() != "" && worksheet.Cells[row, 43].Value != null && worksheet.Cells[row, 43].Value.ToString().Trim() != "")
                                {

                                    DateTime agreementDate = new();
                                    string agreementNumber = "";
                                    string amountPayed = "";

                                    if (worksheet.Cells[row, 44].Value != null && worksheet.Cells[row, 44].Value.ToString().Trim() != "")
                                    {
                                        if (!DateTime.TryParse(worksheet.Cells[row, 44].Value.ToString().Trim(), out date))
                                        {

                                            return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                        }

                                        else
                                        {
                                            agreementDate = DateTime.Parse(worksheet.Cells[row, 44].Value.ToString().Trim());
                                        }
                                    }

                                    if (worksheet.Cells[row, 45].Value != null && worksheet.Cells[row, 45].Value.ToString().Trim() != "")
                                    {
                                        agreementNumber = worksheet.Cells[row, 45].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 46].Value != null && worksheet.Cells[row, 46].Value.ToString().Trim() != "")
                                    {
                                        amountPayed = worksheet.Cells[row, 46].Value.ToString().Trim();
                                        amountPayed = amountPayed.Replace(',', '.');
                                    }

                                    OneTimePaymentStep2 oneTimePaymentStep2 = new()

                                    {
                                        AssetBuyerName = worksheet.Cells[row, 42].Value.ToString().Trim(),
                                        AmountOfAssetSold = worksheet.Cells[row, 43].Value.ToString().Trim(),
                                        AggreementDate = agreementDate,
                                        AggreementNumber = agreementNumber,
                                        AmountPayed = amountPayed,
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep2);
                                    await _context.SaveChangesAsync();

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep2Id = oneTimePaymentStep2.OneTimePaymentStep2Id;
                                        oneTimePaymentAsset.Status = "шартнома";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }

                                if (worksheet.Cells[row, 47].Value != null && worksheet.Cells[row, 47].Value.ToString().Trim() != "" && worksheet.Cells[row, 48].Value != null && worksheet.Cells[row, 48].Value.ToString().Trim() != "")
                                {
                                    if (!DateTime.TryParse(worksheet.Cells[row, 47].Value.ToString().Trim(), out date))
                                    {

                                        return Json(new { success = false, message = row + "-қаторда <<акт санаси>> {ой/кун/йил} форматида киритилмаган. Шу сабабдан объектларни киритиш шу қаторда тўхтади!" });
                                    }

                                    DateTime actDate = DateTime.Parse(worksheet.Cells[row, 47].Value.ToString().Trim());

                                    OneTimePaymentStep3 oneTimePaymentStep3 = new()
                                    {
                                        ActAndAssetDate = actDate,
                                        ActAndAssetNumber = worksheet.Cells[row, 48].Value.ToString().Trim(),
                                        OneTimePaymentAssetId = int.Parse(oneTimePaymentResult[1]),
                                        Confirmed = true
                                    };

                                    await _context.AddAsync(oneTimePaymentStep3);
                                    await _context.SaveChangesAsync();

                                    await MakeAssetOutOfAccount(assetId, _target, actDate);

                                    var oneTimePaymentAsset = await _context.OneTimePaymentAssets.FindAsync(Int32.Parse(oneTimePaymentResult[1]));
                                    if (oneTimePaymentAsset != null)
                                    {
                                        oneTimePaymentAsset.OneTimePaymentStep3Id = oneTimePaymentStep3.OneTimePaymentStep3Id;
                                        oneTimePaymentAsset.Status = "акт";
                                    }

                                    else
                                    {
                                        return Json(new { success = false, message = row + "-қаторда бўлиб-бўлиб тўлаш маълумотларини киритишда хатолик юз берди!" });
                                    }

                                    await _context.SaveChangesAsync();
                                }
                            }

                            if (block2 == null && block3 != null && block4 == null && block5 == null && block6 == null && block7 != null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });
                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }

                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }


                                List<string> installmentResult = AddInstallment(row, true, worksheet, assetId, _target).Result;

                                if (installmentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = installmentResult[1] });
                                }

                            }

                            if (block2 == null && block3 != null && block4 == null && block5 == null && block6 == null && block7 != null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 51].Value != null)
                                    email = worksheet.Cells[row, 51].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 48].Value != null && !worksheet.Cells[row, 48].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 48].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName + mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 49].Value != null && !worksheet.Cells[row, 49].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 49].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 50].Value != null && !worksheet.Cells[row, 50].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 50].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);
                                List<string> evaluationResult = AddAssetEvaluation(row, false, worksheet, assetId, _target).Result;

                                if (evaluationResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = evaluationResult[1] });
                                }

                                List<string> installmentResult = AddInstallment(row, true, worksheet, assetId, _target).Result;

                                if (installmentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = installmentResult[1] });
                                }


                            }

                            if (block2 == null && block3 == null && block4 == null && block5 == null && block6 == null && block7 != null && block8 == null)
                            {

                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                organizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId);

                                if (user != null)
                                {
                                    applicationUserId = user.Id;
                                }
                                else
                                {
                                    realEstateName = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var organizationName = worksheet.Cells[row, 11].Value.ToString().Trim();

                                    return Json(new { success = false, message = realEstateName + " ни киритишни имкони бўлмади, чунки " + organizationName + " га боғланган фойдаланувчи топилмади! Активларни тизимга киритиш ушбу активга келиб тўхтади!" });

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);

                                List<string> installmentResult = AddInstallment(row, false, worksheet, assetId, _target).Result;

                                if (installmentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = installmentResult[1] });
                                }


                            }

                            if (block2 == null && block3 == null && block4 == null && block5 == null && block6 == null && block7 != null && block8 != null)
                            {
                                if (worksheet.Cells[row, 1].Value.ToString().Trim().Equals(""))
                                {
                                    return Json(new { success = false, message = row + "-қаторда актив номи киритилмаган!" });
                                }

                                if (worksheet.Cells[row, 12].Value.ToString().Trim().Equals("#N/A"))
                                {
                                    return Json(new { success = false, message = row + "-қаторда ташкилот номи киритилмаган!" });

                                }

                                var userList = _context.ApplicationUsers.ToList();

                                if (worksheet.Cells[row, 43].Value != null)
                                    email = worksheet.Cells[row, 43].Value.ToString().Trim();
                                else
                                    email = "";
                                if (worksheet.Cells[row, 40].Value != null && !worksheet.Cells[row, 40].Value.ToString().Trim().IsEmpty()) { 
                                    fullName = worksheet.Cells[row, 40].Value.ToString().Trim();
                                    fullNameCheck = fullName.ToLower();
                                    fullNameCheck = Regex.Replace(fullNameCheck, @"\s+", "");
                                }

                                else
                                    return Json(new { success = false, message = "Масъул шахс исм фамилияси келтирилмаган" });

                                isUserAvailable = false;

                                foreach (var item in userList)
                                {
                                    var fName = item.FirstName.Trim().ToLower();
                                    var lName = item.LastName.Trim().ToLower();
                                    var mName = item.MiddleName.Trim().ToLower();

                                    var fullName1 = lName + fName + mName;
                                    var fullName2 = lName + mName + fName;
                                    var fullName3 = mName + fName + lName;
                                    var fullName4 = mName + lName + fName;
                                    var fullName5 = fName + lName + mName;
                                    var fullName6 = fName + mName + lName;

                                    if (item.Email == email)
                                    {
                                        isUserAvailable = true;
                                    }


                                    else if (fullName1.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName2.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName3.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName4.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName5.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    else if (fullName6.Equals(fullNameCheck))
                                    {
                                        isUserAvailable = true;
                                    }

                                    if (isUserAvailable)
                                    {
                                        applicationUserId = item.Id;
                                        break;
                                    }


                                }

                                if (!isUserAvailable)
                                {
                                    string[] infos = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                                    string firstName = "";
                                    string lastName = "";
                                    string middleName = "";

                                    foreach (var item in infos)
                                    {
                                        if (item.Length > 3)
                                            if (item.Substring(item.Length - 2).ToLower().Equals("ов") || item.Substring(item.Length - 2).ToLower().Equals("ев") || item.Substring(item.Length - 3).ToLower().Equals("ева") || item.Substring(item.Length - 3).ToLower().Equals("ова"))
                                            {
                                                lastName = item;
                                            }
                                    }

                                    if (infos.Length == 1)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                        }
                                    }

                                    if (infos.Length == 2)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];
                                            lastName = infos[1];
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                        }
                                    }

                                    if (infos.Length == 3)
                                    {
                                        if (!infos[0].Equals(lastName))
                                        {
                                            firstName = infos[0];

                                            if (!infos[1].Equals(lastName))
                                            {
                                                middleName = infos[1];
                                                lastName = infos[2];

                                            }

                                            else
                                            {
                                                middleName = infos[2];
                                            }
                                        }

                                        else
                                        {
                                            firstName = infos[1];
                                            middleName = infos[2];
                                        }
                                    }

                                    if (email.Equals(""))
                                    {
                                        email = RandomEmail(7);
                                    }

                                    createdById = _userManager.GetUserId(User);
                                    role = DefaultRoles.Role_SimpleUser;

                                    string phoneNumber = "";
                                    string position = "";
                                    if (worksheet.Cells[row, 41].Value != null && !worksheet.Cells[row, 41].Value.ToString().Trim().IsEmpty())
                                    {
                                        position = worksheet.Cells[row, 41].Value.ToString().Trim();
                                    }

                                    if (worksheet.Cells[row, 42].Value != null && !worksheet.Cells[row, 42].Value.ToString().Trim().IsEmpty())
                                    {
                                        phoneNumber = "+998" + worksheet.Cells[row, 42].Value.ToString().Trim();
                                    }

                                    ApplicationUser user = new()
                                    {
                                        FirstName = firstName,
                                        LastName = lastName,
                                        MiddleName = middleName,
                                        Email = email,
                                        UserName = email,
                                        PhoneNumber = phoneNumber,
                                        Postion = position,
                                        Role = role,
                                        OrganizationId = Int32.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                        EmailConfirmed = true,
                                        CreatedById = createdById
                                    };

                                    var result = await CreateUser(user);

                                    if (result[0].Equals(""))
                                    {
                                        return Json(new { success = false, message = result[1] });
                                    }

                                    applicationUserId = result[0];

                                }

                                DateTime outDate = DateTime.Now.AddYears(1000);

                                var realEstateResult = await AddRealEstate(row, worksheet, applicationUserId, outDate);

                                if (realEstateResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = realEstateResult[1] });
                                }

                                int assetId = Int32.Parse(realEstateResult[1]);



                                List<string> installmentResult = AddInstallment(row, false, worksheet, assetId, _target).Result;

                                if (installmentResult[0].Equals("bad"))
                                {
                                    return Json(new { success = false, message = installmentResult[1] });
                                }


                            }

                            amount++;
                        }

                        return Json(new { success = true, message = $"{amount} та объект тизимга муваффақиятли киритилди!" });

                    }
                }

                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }


            }

        }


    }
}
