using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Globalization;
using System.IO;
using System.Text;
using SMAT_Inventory.Models;
using SMAT_Inventory.Dapper;
using Dapper;
//using YourNamespace.Models; // Import your models
//using YourNamespace.Data; // Import your DbContext
using OfficeOpenXml; //Assuming you are using EPPlus for Excel operations
using System.Data;
using System.Data.SqlClient;
using SMAT_Inventory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Routing;
using DinkToPdf;
using DinkToPdf.Contracts;


namespace SMAT_Inventory.Controllers
{

    public class APIController : Controller
    {
       
        private readonly DAPPER_DATA_SERVICE dapper;
        public APIController()
        {
            dapper = new DAPPER_DATA_SERVICE();
        }


        [HttpPost]

        public ActionResult Scan(HttpPostedFileBase scannerFile, int branch=0, int calculation=0)
        {
            //scannerFile = "C:\\Work\\SMA-techno\\docs\\REPORT FORMATS\\SAMPLE SCAN FOR TESTING\\BR-204, 2023-02-12-04+02.csv";
            try
            {
                if (scannerFile == null || scannerFile.ContentLength == 0)
                {
                    return Json(new { success = false, message = "File not uploaded." });
                }
                if(branch==0)
                {
                    return Json(new { success = false, message = "No Branch selected..." });
                }

                branchViewModel modi = new branchViewModel();
                using (DAPPER_DATA_SERVICE dapper2 = new DAPPER_DATA_SERVICE())
                {
                    dapper2.AddIntegerParameter("branch_id", branch);
                    var result = dapper2.query_multiple("dbo.[GetBranchbyBranch_id]");

                    // Check if the result contains rows
                    if (result.hasRows())
                    {
                        modi.branch = result.Read<branchModel>().FirstOrDefault();
                    }
                }

                string fileName = modi.branch.code + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + calculation.ToString() + "_S";

                //string fileName = DateTime.Now.Ticks + "-file.csv";
                string filePath = Path.Combine(Server.MapPath("~/App_Data/scan_files"), fileName);

                // Save the uploaded file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    scannerFile.InputStream.CopyTo(fileStream);
                }

                // Insert file details into the database
                var assetsFile = new AssetScannerFileModel
                {
                    branch_id = branch,
                    file_name = fileName,
                    calculation =true,
                    created_by = GetAuthenticatedUserId(),
                    created_at = DateTime.Now
                };

                string insertQuery = @"INSERT INTO Asset_Scanner_File (branch_id, file_name, calculation, created_by, created_at) 
                                   VALUES (@branch_id, @file_name, @calculation, @created_by, @created_at);
                                   SELECT CAST(SCOPE_IDENTITY() as int);";
                int newFileId = dapper.ExecuteQuery<int>(insertQuery, assetsFile).Single();

                // Reading CSV file using EPPlus or any library you prefer
                var importData = new List<List<string>>();
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        importData.Add(values.ToList());
                    }
                }

                var assets = dapper.ExecuteQuery<assetsModel>("SELECT * FROM Assets WHERE branch_id = @branch_id", new { branch_id = branch });
                var differentTagId = new List<string>();
                int j = 0;
                foreach (var record in importData)
                {
                    if (record.Count < 6) continue; // Assuming there are at least 6 columns in the CSV
                    if (j == 0) { j++;  continue; }
                    //var scannedAsset = new AssetScannerModel
                    //{
                    int file_id = newFileId;
                    string tag_id = record[1].Replace(" ", "").Replace("\"", "").Trim();
                    int pc = int.Parse(record[2].Replace(" ", "").Replace("\"", "").Trim());
                    int count = int.Parse(record[3].Replace(" ", "").Replace("\"", "").Trim());
                    int rssi = int.Parse(record[4].Replace(" ", "").Replace("\"", "").Trim());
                    int riss = (int)Math.Round(double.Parse(record[5]));
                    int created_by = GetAuthenticatedUserId();
                    //};

                    string insertScannedAssetQuery = @"INSERT INTO assets_scanned (file_id, tag_id, pc, count, rssi, riss, created_by, created_at) 
                                                   VALUES (" + file_id + ",'" + tag_id + "'," + pc + "," + count + "," + rssi + "," + riss + ",'" + created_by +"', getdate())";
                    dapper.execute(insertScannedAssetQuery);

                    differentTagId.Add(tag_id);
                }

                var collection = differentTagId.Distinct().ToList();
                var matchedAssets = assets.Select(a => a.tag_id).Intersect(collection).ToList();

                var matchedAssetsFromScan = new List<assetsModel>();
                var unmatchedAssetsFromScan = new List<assetsModel>();

                foreach (var matched in matchedAssets)
                {
                    var assetsFromScan = assets.Where(a => a.tag_id == matched).ToList();
                    if (assetsFromScan.Any())
                    {
                        var asset = assetsFromScan.First();

                        var depreciationData = new
                        {
                            TotalCost = asset.total_cost,
                            LifeInYear = asset.life_in_year,
                            DateOfPurchase = asset.date_of_purchase,
                            DepType = asset.dep_type,
                            RemainingYears = asset.remaining_years,
                            NetValue = asset.net_value
                        };

                        var depreciationCalculation = Depreciation(depreciationData);

                        asset.last_year_bv = asset.net_value;
                        asset.salvage = depreciationCalculation.Salvage;
                        asset.current_dep = depreciationCalculation.Depreciation;
                        asset.total_dep = depreciationCalculation.TotalDepreciation;
                        asset.net_value = depreciationCalculation.NetValue;
                        asset.last_scan_date = DateTime.Now;

                        string updateQuery = @"UPDATE Assets SET 
                                        Salvage = "+ asset.salvage + ", current_dep = " + asset.current_dep + ", total_dep = " + asset.total_dep + ","+
                                        " net_value = " + asset.net_value + ", last_scan_date = '" + asset.last_scan_date+ "', remaining_years = " + asset.remaining_years + 
                                        " WHERE tag_id ='"+ asset.tag_id+"'";

                        dapper.execute(updateQuery);

                        matchedAssetsFromScan.Add(asset);
                    }
                }

                // Further processing like generating report
                var reportNo = ScanDetailedReport(newFileId);

                // Send notifications, generate PDFs, etc.

                return Json(new { success = true, data = new { url = Url.Content("~/app_data/reports/" + reportNo + ".pdf"),fileName= reportNo + ".pdf" } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, code = ex.HResult });
            }
        }

        private int GetAuthenticatedUserId()
        {
            // Implement logic to get authenticated user ID
            return GlobalVariables.AppUserId; // Placeholder
        }

        private dynamic Depreciation(dynamic data)
        {
            // Implement your depreciation calculation logic
            return new { Salvage = 0, Depreciation = 0, TotalDepreciation = 0, NetValue = 0 };
        }

       

        private void NotifyAdmin(dynamic notificationData)
        {
            // Implement your logic to notify the admin
        }

        public ActionResult DownloadFile(string fileName)
        {
            // Define the path to the file
            string filePath = Path.Combine(Server.MapPath("~/app_data/reports"), fileName);

            // Check if file exists
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("File not found");
            }

            // Return the file to the user
            return File(filePath, "application/pdf", fileName);
        }


        public string ScanDetailedReport(int scanned_file_id)
        {
            // Fetch the scanned file details
            var get_scanned_file = dapper.ExecuteQuery<dynamic>(
                "SELECT * FROM Asset_Scanner_File WHERE id = @scanned_file_id", new { scanned_file_id }).FirstOrDefault();

            if (get_scanned_file != null)
            {
                // Fetch collection details
                var collection = dapper.ExecuteQuery<dynamic>(
                    "SELECT * FROM assets_scanned WHERE file_id = @file_id", new { file_id = get_scanned_file.id });

                // Fetch branch details with joins
                var branch = dapper.ExecuteQuery<dynamic>(
                    @"SELECT branch.id,branch.zone_id,branch.code,branch.name,branch.phone1,branch.phone2,branch.phone3,branch.email,branch.address,branch.city_id,branch.created_by,branch.updated_by,branch.postalcode,branch.googleMapId,branch.created_at,branch.updated_at,branch.status,branch.deleted_at, cities.name as city, zone.name as zone 
                      FROM branch 
                      JOIN zone ON zone.id = branch.zone_id 
                      JOIN cities ON cities.id = branch.city_id 
                      WHERE branch.id = @branch_id AND branch.status = 1",
                      new { branch_id = get_scanned_file.branch_id }).FirstOrDefault();

                var branches_detail = new List<dynamic> { branch };

                // Fetch assets
                var assets = dapper.ExecuteQuery<assetsModel>(
                    "SELECT * FROM Assets WHERE branch_id = @branch_id AND created_at < @created_at",
                    new { branch_id = branch.id, created_at = get_scanned_file.created_at });

                // Compute asset differences and matches
                var assetDifference = assets.Select(a => a.tag_id).Except(collection.Select(c => c.tag_id));
                var matchedAssets = assets.Select(a => a.tag_id).Intersect(collection.Select(c => c.tag_id));

                var sameBranchAssets = assetDifference.Union(matchedAssets);
                var otherThanBranch = collection.Select(c => c.tag_id).Except(sameBranchAssets);

                var matchedAssetsFromScan = new List<assetsModel>();
                var unmatchedAssetsFromScan = new List<assetsModel>();
                var otherBranchFromScan = new List<assetsModel>();
                var otherTagsFromScan = new List<dynamic>();

                foreach (var matched in matchedAssets)
                {
                    assetsModel AssetsFromScan = assets.FirstOrDefault(a => a.tag_id == matched);
                    matchedAssetsFromScan.Add(AssetsFromScan);
                }

                foreach (var unmatched in assetDifference)
                {
                    var AssetsFromScan = assets.FirstOrDefault(a => a.tag_id == unmatched);
                    unmatchedAssetsFromScan.Add(AssetsFromScan);
                }

                foreach (var otherBranch in otherThanBranch)
                {
                        var AssetsFromScan = dapper.ExecuteQuery<assetsModel>(
                        "SELECT * FROM Assets WHERE tag_id = @tag_id AND created_at < @created_at",
                        new { tag_id = otherBranch, created_at = get_scanned_file.created_at }).FirstOrDefault();

                    if (AssetsFromScan!=null)
                    {
                        otherBranchFromScan.Add(AssetsFromScan);
                    }
                    else
                    {
                        otherTagsFromScan.Add(otherBranch);
                    }
                }

                var createdAt = DateTime.Parse(get_scanned_file.created_at.ToString());
                var scanned_date = createdAt.ToString("ddMMyyyy");

                var branchesDetail = branches_detail.FirstOrDefault();
                branchesDetail.assets = new
                {
                    matched_assets = matchedAssetsFromScan,
                    unmatched_assets = unmatchedAssetsFromScan,
                    otherBranchAssets = otherBranchFromScan,
                    otherTags = otherTagsFromScan,
                    totalRecords = collection.Count(),
                    unmatchedRecords = assetDifference.Count(),
                    matchedRecords = matchedAssets.Count(),
                    reporting_no = $"{branch.code}-{get_scanned_file.id + 1000}-{scanned_date}{(get_scanned_file.calculation == 1 ? "-C" : "-R")}"
                };

                var company_name = dapper.ExecuteQuery<dynamic>(
                    "SELECT * FROM Web_Settings WHERE tag = 'company_name'").FirstOrDefault();

                var data = new ReportDetailViewModel
                {
                    title = "Reporting",
                    date = DateTime.Now.ToString("dd-MM-yyyy"),
                    user = dapper.ExecuteQuery<dynamic>(
                        "SELECT * FROM Users WHERE id = @id", new { id = GetAuthenticatedUserId() }).FirstOrDefault(),
                    company_name = company_name.name,
                    zone = branch.zone,
                    branches = branchesDetail,
                    matched_assets = matchedAssetsFromScan,
                    unmatched_assets = unmatchedAssetsFromScan,
                    otherBranchAssets = otherBranchFromScan,
                    city = branch.city,
                    reporting_no = $"{branch.code}-{get_scanned_file.id + 1000}-{scanned_date}{(get_scanned_file.calculation == 1 ? "-C" : "-R")}"
                };

                string reportView = get_scanned_file.calculation == 1 ? "reportdetailed" : "Assets/reportreview";
                //var pdf = GeneratePdf(reportView, data);  // Assume you have a method to generate a PDF from a view
                var pdfGenerator = new PdfGenerator();
                var pdf = pdfGenerator.GeneratePdf(reportView, data, ControllerContext);

                //string reportPath = $"~/app_data/reports/{branches_detail.FirstOrDefault().assets.reporting_no}.pdf";
                string reportPath = Path.Combine(Server.MapPath("~/App_Data/reports"), branches_detail.FirstOrDefault().assets.reporting_no + ".pdf");

                System.IO.File.WriteAllBytes(reportPath, pdf);

                var reports = new
                {
                    branch_id = branch.id,
                    report_no = branches_detail.FirstOrDefault().assets.reporting_no,
                    report = $"{branches_detail.FirstOrDefault().assets.reporting_no}.pdf",
                    calculation = 1,
                    created_by = GetAuthenticatedUserId()
                };

                dapper.ExecuteQuery<dynamic>("INSERT INTO Reports (branch_id, report_no, report, calculation, created_by, created_at) VALUES (@branch_id, @report_no, @report, @calculation, @created_by, getdate())", reports);

                var reporting_no = branches_detail.FirstOrDefault().assets.reporting_no;

                SendEmail(reporting_no, branch.email, pdf);  // Assume you have a method to send emails

                return reporting_no;
            }
            else
            {
                return null;
            }
        }

        /*private byte[] GeneratePdf(string viewName, object model, ControllerContext ControllerContext)
        {
            //var model = new YourViewModel(); // Populate with actual data
            byte[] pdfBytes = PdfGenerator.GeneratePdf(viewName, model, ControllerContext);
            return pdfBytes;  //File(pdfBytes, "application/pdf", "Report.pdf");
        }*/

        private void SendEmail(string reporting_no, string emails, byte[] pdf)
        {
            // Implement your email sending logic here
        }




    }


public class PdfGenerator
    {
        private static IConverter _converter;

        public PdfGenerator()
        {
            //var context = new CustomAssemblyLoadContext();
            _converter = new SynchronizedConverter(new PdfTools());
        }

        public static byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var pdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    Out = null // Set to null to get the result as a byte array
                },
                Objects = {
                new ObjectSettings
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            return _converter.Convert(pdfDocument);
        }


        public byte[] GeneratePdf(string viewName, object model, ControllerContext ControllerContext)
        {
            // Render the HTML content from the view
            string htmlContent = RenderViewToString(viewName, model, ControllerContext);

            // Generate PDF from HTML content
            PdfGenerator pdfGenerator = new PdfGenerator();
            byte[] pdfBytes = GeneratePdfFromHtml(htmlContent);

            // Return the PDF as a file download
            return pdfBytes; //File(pdfBytes, "application/pdf", "GeneratedDocument.pdf");
        }


        private static string RenderViewToString(string viewName, object model, ControllerContext controllerContext)
        {
            // Set the model for the view
            controllerContext.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                // Find the view using FindView instead of FindPartialView
                var viewResult = ViewEngines.Engines.FindView(controllerContext, viewName, null);

                // Check if the view was found
                if (viewResult.View == null)
                {
                    throw new FileNotFoundException($"View '{viewName}' could not be found. Make sure the view exists in the correct location.");
                }

                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);

                // Render the view to the StringWriter
                viewResult.View.Render(viewContext, stringWriter);

                // Release the view after rendering
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);

                return stringWriter.GetStringBuilder().ToString();
            }
        }

    }


}



