using Microsoft.AspNet.Identity;
using SMAT_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMAT_Inventory.Models;


namespace SMAT_Inventory.Controllers
{
    public class ReportingController : Controller
    {/*
        [HttpPost]
        public ActionResult Reporting(ReportingRequest request)
        {
            if (request != null)
            {
                int groupId = request.GroupId;

                if (!string.IsNullOrEmpty(request.Group))
                {
                    if (request.View == "view_report" || request.Download == "download_report")
                    {
                        var group = GroupsModel.FirstOrDefault(g => g.Id == groupId);

                        var branches = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(group.Branches);

                        var branchesDetail = new Dictionary<int, branchModel>();

                        foreach (var branch in branches)
                        {
                            var branchDetail = branchModel.Include("Zone")
                                                          .Include("City")
                                                          .Where(b => b.Id == branch)
                                                          .ToList();

                            branchesDetail.Add(branch, new BranchDetail { BranchDetails = branchDetail });

                            var getScannedFile = request.DateFrom.HasValue && request.DateTo.HasValue
                                ? AssetScannerFileModel.Where(a => a.BranchId == branch &&
                                                                   a.CreatedAt >= request.DateFrom &&
                                                                   a.CreatedAt <= request.DateTo)
                                                       .OrderByDescending(a => a.Id)
                                : AssetScannerFileModel.Where(a => a.BranchId == branch)
                                                       .OrderByDescending(a => a.Id);

                            if (getScannedFile.Any())
                            {
                                var scannedFile = getScannedFile.First();

                                var collection = AssetScannerModel.Where(a => a.FileId == scannedFile.Id).ToList();

                                var assets = request.DateFrom.HasValue && request.DateTo.HasValue
                                    ? Assets.Where(a => a.BranchId == branch && a.CreatedAt < scannedFile.CreatedAt).ToList()
                                    : Assets.Where(a => a.BranchId == branch && a.CreatedAt < request.DateTo).ToList();

                                var assetDifference = assets.Select(a => a.TagId).Except(collection.Select(c => c.TagId));
                                var matchedAssets = assets.Select(a => a.TagId).Intersect(collection.Select(c => c.TagId));

                                var matchedAssetsFromScan = new List<Asset>();
                                var unmatchedAssetsFromScan = new List<Asset>();

                                foreach (var matched in matchedAssets)
                                {
                                    var assetFromScan = assets.First(a => a.TagId == matched);
                                    matchedAssetsFromScan.Add(assetFromScan);
                                }

                                foreach (var unmatched in assetDifference)
                                {
                                    var assetFromScan = assets.First(a => a.TagId == unmatched);
                                    unmatchedAssetsFromScan.Add(assetFromScan);
                                }

                                var matchedArray = new
                                {
                                    count = matchedAssets.Count(),
                                    last_bv = matchedAssetsFromScan.Sum(a => a.LastYearBV) ?? matchedAssetsFromScan.Sum(a => a.NetValue),
                                    current_bv = matchedAssetsFromScan.Sum(a => a.NetValue),
                                    current_dep = matchedAssetsFromScan.Sum(a => a.CurrentDep),
                                    total = matchedAssetsFromScan.Sum(a => a.TotalCost)
                                };

                                var unmatchedArray = new
                                {
                                    count = assetDifference.Count(),
                                    last_bv = unmatchedAssetsFromScan.Sum(a => a.LastYearBV) ?? unmatchedAssetsFromScan.Sum(a => a.NetValue),
                                    current_bv = unmatchedAssetsFromScan.Sum(a => a.NetValue),
                                    current_dep = unmatchedAssetsFromScan.Sum(a => a.CurrentDep),
                                    total = unmatchedAssetsFromScan.Sum(a => a.TotalCost)
                                };

                                branchesDetail[branch].Assets = new
                                {
                                    MatchedAssets = matchedAssetsFromScan,
                                    UnmatchedAssets = unmatchedAssetsFromScan,
                                    TotalRecords = collection.Count(),
                                    UnmatchedRecords = assetDifference.Count(),
                                    MatchedRecords = matchedAssets.Count(),
                                    Matched = matchedArray,
                                    Unmatched = unmatchedArray,
                                    Total = new
                                    {
                                        count = matchedAssets.Count() + assetDifference.Count(),
                                        last_bv = (matchedArray.last_bv + unmatchedArray.last_bv) > 0
                                            ? matchedArray.last_bv + unmatchedArray.last_bv
                                            : matchedArray.current_bv + unmatchedArray.current_bv,
                                        current_bv = matchedArray.current_bv + unmatchedArray.current_bv,
                                        current_dep = matchedArray.current_dep + unmatchedArray.current_dep,
                                        sum = matchedArray.total + unmatchedArray.total
                                    }
                                };

                                branchesDetail[branch].ScanDate = scannedFile.CreatedAt.ToString("dd/MM/yyyy");
                            }
                        }

                        var total = CalculateTotals(branchesDetail);

                        var currencyId = WebSettingsModel.FirstOrDefault(w => w.Tag == "company_currency")?.Value ?? "128";
                        var currency = CountryCurrency.Select(c => new { c.Id, c.CurrencySymbol }).FirstOrDefault(c => c.Id == int.Parse(currencyId));

                        var accountClosingDate = WebSettingsModel.FirstOrDefault(w => w.Tag == "account_closing_date")?.Value;
                        var startingPeriod = DateTime.ParseExact(request.DateFrom.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var endingPeriod = DateTime.ParseExact(request.DateTo.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var data = new
                        {
                            group = group.Name,
                            title = "Reporting",
                            date = DateTime.Now.ToString("MM/dd/yyyy"),
                            user = UserModel.FirstOrDefault(u => u.Id == User.Identity.GetUserId<int>()),
                            branches = branchesDetail,
                            total = total,
                            currency = currency.CurrencySymbol,
                            accounting_period = new { start = startingPeriod, end = endingPeriod },
                            calculation = request.Calculation == "yes"
                        };

                        var pdf = CreatePDF(request.ReportType, data);

                        if (request.View == "view_report")
                        {
                            return File(pdf, "application/pdf", "report.pdf");
                        }
                        else
                        {
                            return File(pdf, "application/pdf", "report.pdf", true);
                        }
                    }
                }
                else
                {
                    TempData["type"] = "error";
                    TempData["message"] = "Kindly select Zone and Branch!";
                    return RedirectToAction("Reports");
                }
            }

            return View();
        }

        private byte[] CreatePDF(string reportType, object data)
        {
            // Implement your PDF creation logic here (e.g., using Rotativa, iTextSharp, etc.)
            return new byte[0]; // Placeholder
        }

        private object CalculateTotals(Dictionary<int, BranchDetail> branchesDetail)
        {
            // Calculate the total assets, amounts, and other values.
            return new
            {
                assets = branchesDetail.Sum(b => b.Value.Assets?.TotalRecords ?? 0),
                amount = branchesDetail.Sum(b => b.Value.Assets?.Total.sum ?? 0),
                dep_amount = branchesDetail.Sum(b => b.Value.Assets?.Total.current_dep ?? 0),
                last_bv = branchesDetail.Sum(b => b.Value.Assets?.Total.last_bv ?? 0),
                bv_amount = branchesDetail.Sum(b => b.Value.Assets?.Total.current_bv ?? 0),
                branches = branchesDetail.Count,
                missingTotal = branchesDetail.Sum(b => b.Value.Assets?.Unmatched.count ?? 0),
                missingValue = branchesDetail.Sum(b => b.Value.Assets?.Unmatched.total ?? 0)
            };
        }*/
    }
}




