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

using CsvHelper;
using CsvHelper.Configuration;
using System.Web.Configuration;
using System.Security.Principal;
using System.Net.Mail;
using System.Net;
using System.Web.WebPages;
using Winnovative;



namespace SMAT_Inventory.Controllers
{

    public class SMAAPIController : Controller
    {

        private DAPPER_DATA_SERVICE dapper;
        public SMAAPIController()
        {
            dapper = new DAPPER_DATA_SERVICE();
        }


        [HttpPost]

        public ActionResult Scan(HttpPostedFileBase scannerFile, int branch = 0, int calculation = 0)
        {

            //scannerFile = "C:\\Work\\SMA-techno\\docs\\REPORT FORMATS\\SAMPLE SCAN FOR TESTING\\BR-204, 2023-02-12-04+02.csv";
            try
            {
                if (scannerFile == null || scannerFile.ContentLength == 0)
                {
                    return Json(new { success = false, message = "File not uploaded." });
                }
                if (branch == 0)
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
                string filePath = Path.Combine(Server.MapPath("~/app_Data/scan_files"), fileName);

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
                    calculation = true,
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
                    if (j == 0) { j++; continue; }
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
                                                   VALUES (" + file_id + ",'" + tag_id + "'," + pc + "," + count + "," + rssi + "," + riss + ",'" + created_by + "', getdate())";
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
                            NetValue = asset.net_value,
                            Salvage = asset.salvage_value
                        };

                        var depreciationCalculation = Depreciation(depreciationData);

                        if (depreciationCalculation.OkiTrans == 1)
                        {
                            asset.last_year_bv = asset.net_value;
                            asset.salvage = depreciationCalculation.Salvage;
                            asset.current_dep = depreciationCalculation.Depreciation;
                            asset.total_dep = depreciationCalculation.TotalDepreciation;
                            asset.net_value = depreciationCalculation.NetValue;
                            asset.last_scan_date = DateTime.Now;

                            string updateQuery = @"UPDATE Assets SET 
                                        Salvage = " + asset.salvage + ", current_dep = " + asset.current_dep + ", total_dep = " + asset.total_dep + "," +
                                            " net_value = " + asset.net_value + ", last_scan_date = '" + asset.last_scan_date + "', remaining_years = " + asset.remaining_years +
                                            " WHERE tag_id ='" + asset.tag_id + "'";

                            dapper.execute(updateQuery);

                            matchedAssetsFromScan.Add(asset);
                        }
                    }
                }
                // Further processing like generating report
                var reportNo = ScanDetailedReport(newFileId);
                //return Json(new { success = false, message = reportNo, code = 0 });

                // Send notifications, generate PDFs, etc.

                return Json(new { success = true, data = new { url = Url.Content("~/app_data/reports/" + reportNo + ".pdf"), fileName = reportNo + ".pdf" } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "ehehhe" + ex, code = ex.HResult });
            }
        }

        private int GetAuthenticatedUserId()
        {
            // Implement logic to get authenticated user ID
            return GlobalVariables.AppUserId; // Placeholder
        }

        private dynamic Depreciation(dynamic data)
        {
            int Straight_line_rate = 0, Double_Declining_Rate = 0;
            int DepreciationV = 0, NetValueV = data.NetValue;
            int remainyearsum =(data.RemainingYears * (data.RemainingYears + 1))/2;
            try
            {
                if (data.DepType == 1)
                {
                    Straight_line_rate = (1 / data.LifeInYear);
                    Double_Declining_Rate = 2 * Straight_line_rate;
                    DepreciationV = Double_Declining_Rate * (data.NetValue);
                    NetValueV = data.NetValue - DepreciationV;
                }
                if (data.DepType == 2)
                {
                    DepreciationV = ((data.NetValue - data.Salvage) / data.LifeInYear);
                }
                if (data.DepType == 3)
                {
                    DepreciationV = ((remainyearsum - data.Salvage) * (data.net_NetValuevalue - data.Salvage));
                }
                // Implement your depreciation calculation logic
                return new { Salvage = data.Salvage, Depreciation = DepreciationV, TotalDepreciation = DepreciationV, NetValue = NetValueV, OkiTrans=1 };
            }
            catch (Exception ex)
            {
                return new { Salvage = 0, Depreciation = 0, TotalDepreciation = 0, NetValue = 0, OkiTrans=0 };

            }
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


        public JsonResult CheckFileExists(string fileName)
        {
            var filePath = Path.Combine(Server.MapPath("~/app_data/reports/"), fileName);
            bool exists = System.IO.File.Exists(filePath);
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
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

                    if (AssetsFromScan != null)
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


                string reportPath0 = Server.MapPath("~/App_Data/reports");

                LogErrorToFile("path: " + reportPath0);
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

                //SendEmail(reporting_no, branch.email, pdf);  // Assume you have a method to send emails

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

        //public string SendEmail(string toEmail, string subject, string message, string fromEmail, string fromEmailPassword, string smtpServer, int smtpPort, string EnableSsl, string attachmentPath = null)
        public string SendEmail(string toEmail, string subject, string message, string attachmentPath = null)
        {
            try
            {
                //WriteToEvenLog("Application", "in send email 1");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(GlobalVariables.GetWebSetting("fromEmail", ""));
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = message; //message;
                mail.IsBodyHtml = true;

                // Add attachment if provided
                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    Attachment attachment = new Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);
                }

                SmtpClient smtpClient = new SmtpClient(GlobalVariables.GetWebSetting("smtpServer", ""), Convert.ToInt32(GlobalVariables.GetWebSetting("smtpPort", "")));
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(GlobalVariables.GetWebSetting("fromEmail", ""), GlobalVariables.GetWebSetting("fromEmailPassword", ""));

                smtpClient.EnableSsl = false;
                if (GlobalVariables.GetWebSetting("EnableSsl", "false") == "true") { smtpClient.EnableSsl = true; }


                smtpClient.Send(mail);

                return "sended";
            }

            catch (Exception ex)
            {
                return "Error sending eMail (1.1): " + ex.Message;
                // Handle the exception or log the error
            }
        }



        [HttpPost]
        public ActionResult UploadBranchData(HttpPostedFileBase file)
        {
            ViewBag.Error = "";
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.Error = "Please upload a valid CSV file.";
                return Json(new { success = false, message = ViewBag.Error });
            }
            AssetsController AssetsController = new AssetsController();
             
            var branches = new List<branchModel>();
            using (var reader = new StreamReader(file.InputStream))
            {
                var csvReader = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                });

                var records = csvReader.GetRecords<dynamic>().ToList();

                try
                {
                    using (var transaction = dapper.BeginTransaction())
                    {
                        foreach (var record in records)
                        {
                            string tmpgoogleMapId = "";
                            tmpgoogleMapId = record.googleMapId;
                            /*if (record.googleMapId == "")
                            {
                                tmpgoogleMapId = AssetsController.GetBranchCoordinates2(record.address);
                            }
                            else
                            {
                                tmpgoogleMapId = record.googleMapId;
                            }*/

                            // Extract data from the CSV file
                            string zoneName = record.zone;
                            string cityName = record.city;
                            string branchName = record.branch_name;
                            string code = record.branch_code;
                            string phone1 = record.phone_no1;
                            string phone2 = record.phone_no2;
                            string phone3 = record.phone_no3;
                            string email = record.email;
                            string address = record.address;
                            string postalCode = record.postalcode;
                            string googleMapId = tmpgoogleMapId;
                            string branchemail = record.branchemail;
                            string chkSum = record.chkSum;

                            // Check if zone exists
                            int zoneId = dapper.ExecuteQueryint("SELECT id FROM zone WHERE name = @Name", new { Name = zoneName });
                            if (zoneId == 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"Zone '{zoneName}' does not exist.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }

                            // Check if city exists
                            int cityId = dapper.ExecuteQueryint("SELECT id FROM cities WHERE name = @Name", new { Name = cityName });
                            if (cityId == 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"City '{cityName}' does not exist.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }

                            // Check for duplicate branch name
                            int existingBranch = dapper.ExecuteQueryint("SELECT id FROM branch WHERE name = @Name", new { Name = branchName });
                            if (existingBranch > 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"Branch '{branchName}' already exists.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }
                            // Check for duplicate branch name
                            int existingBranchcode = dapper.ExecuteQueryint("SELECT id FROM branch WHERE code = @Code", new { Code = code });
                            if (existingBranchcode > 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"Branch Code '{code}' already exists.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }
                            // Insert into branch table
                            var ii = dapper.ExecuteQueryint(@"INSERT INTO branch (zone_id, city_id, code, name, phone1, phone2, phone3, email, address, postalcode, googleMapId, created_by, status, branchemail, chkSUM) 
                                        VALUES (@ZoneId, @CityId, @Code, @Name, @Phone1, @Phone2, @Phone3, @Email, @Address, @PostalCode, @GoogleMapId, @CreatedBy, @Status, @Branchemail, @ChkSUM)",
                                            new
                                            {
                                                ZoneId = zoneId,
                                                CityId = cityId,
                                                Code = code,
                                                Name = branchName,
                                                Phone1 = phone1,
                                                Phone2 = phone2,
                                                Phone3 = phone3,
                                                Email = email,
                                                Address = address,
                                                PostalCode = postalCode,
                                                GoogleMapId = googleMapId,
                                                CreatedBy = GlobalVariables.AppUserId, // Replace with actual user ID
                                                Status = 1, // Default status as active
                                                Branchemail = branchemail,
                                                ChkSUM = chkSum
                                            });
                        }

                        transaction.Commit();
                        return Json(new { success = true, message = "All branches uploaded successfully!" });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error occurred: " + ex.Message;
                }
            }

            return Json(new { success = false, message = ViewBag.Error });
        }

        [HttpPost]
        public ActionResult UploadAssetData(HttpPostedFileBase file)
        {
            ViewBag.Error = "";
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.Error = "Please upload a valid CSV file.";
                return Json(new { success = false, message = ViewBag.Error });
            }

            var branches = new List<assetsModel>();
            using (var reader = new StreamReader(file.InputStream))
            {
                var csvReader = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                });

                var records = csvReader.GetRecords<dynamic>().ToList();

                try
                {
                    using (var transaction = dapper.BeginTransaction())
                    {
                        foreach (var record in records)
                        {
                            // Extract data from the CSV file
                            string tag_id = record.tag_id;
                            string asset_id = record.asset_id;
                            string asset_name = record.asset_name;
                            string invoice_cost = record.invoice_cost;
                            string total_cost = record.total_cost;
                            string salvage_value = record.salvage_value;
                            string date_of_purchase = record.date_of_purchase;
                            string vendor_id = record.vendor_id;
                            string vendor = record.vendor;
                            string part_warranty = record.part_warranty;
                            string service_warranty = record.service_warranty;
                            string serial_no = record.serial_no;
                            string generic_name = record.generic_name;
                            string custom_field = record.custom_field;
                            string life_in_year = record.life_in_year;
                            string dep_type = record.dep_type;
                            string last_scan_date = record.last_scan_date;
                            string net_value = record.net_value;
                            string remaining_years = record.remaining_years;
                            string salvage = record.salvage;
                            string last_year_bv = record.last_year_bv;
                            string current_dep = record.current_dep;
                            string total_dep = record.total_dep;
                            string location = record.location;
                            string branch_code = record.branch_code;
                            string AssetOld_id = record.AssetOld_id;
                            string Tagcount = record.Tagcount;
                            string TagcountRemain = record.TagcountRemain;

                            // Check if Branch exists
                            int branchcode = dapper.ExecuteQueryint("SELECT id FROM branch WHERE code = @Code", new { Code = branch_code });
                            if (branchcode == 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"Branch Code '{branch_code}' does not exist.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }

                            // Check for duplicate Tag ID
                            int AssetIdtag = dapper.ExecuteQueryint("SELECT id FROM assets WHERE tag_id = @Tag_id", new { Tag_id = tag_id });
                            if (AssetIdtag > 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"Tag ID '{tag_id}' already exists.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }
                            // Check for duplicate Asset ID
                            int AssetIdasset = dapper.ExecuteQueryint("SELECT id FROM assets WHERE asset_id = @Asset_id", new { Asset_id = asset_id });
                            if (AssetIdasset > 0)
                            {
                                transaction.Rollback();
                                ViewBag.Error = $"Asset ID '{asset_id}' already exists.";
                                return Json(new { success = false, message = ViewBag.Error });
                            }


                            // Insert into branch table
                            var ii = dapper.ExecuteQueryint(@"INSERT INTO assets (tag_id, asset_id, asset_name, invoice_cost, total_cost, salvage_value, date_of_purchase, vendor_id, vendor, part_warranty, service_warranty, serial_no, generic_name, custom_field, life_in_year, dep_type, last_scan_date, net_value, remaining_years, salvage, last_year_bv, current_dep, total_dep, location, branch_id, created_by, created_at,AssetOld_id, Tagcount, TagcountRemain) 
                                        VALUES (@TAG_ID, @ASSET_ID, @ASSET_NAME, @INVOICE_COST, @TOTAL_COST, @SALVAGE_VALUE, @DATE_OF_PURCHASE, @VENDOR_ID, @VENDOR, @PART_WARRANTY, @SERVICE_WARRANTY, @SERIAL_NO, @GENERIC_NAME, @CUSTOM_FIELD, @LIFE_IN_YEAR, @DEP_TYPE, @LAST_SCAN_DATE, @NET_VALUE, @REMAINING_YEARS, @SALVAGE, @LAST_YEAR_BV, @CURRENT_DEP, @TOTAL_DEP, @LOCATION, @BRANCH_ID, @CREATED_BY, @CREATED_AT,@ASSETOLD_ID, @TAGCOUNT, @TAGCOUNTREMAIN)",
                                            new
                                            {
                                                TAG_ID = tag_id,
                                                ASSET_ID = asset_id,
                                                ASSET_NAME = asset_name,
                                                INVOICE_COST = invoice_cost,
                                                TOTAL_COST = total_cost,
                                                SALVAGE_VALUE = salvage_value,
                                                DATE_OF_PURCHASE = date_of_purchase,
                                                VENDOR_ID = vendor_id,
                                                VENDOR = vendor,
                                                PART_WARRANTY = part_warranty,
                                                SERVICE_WARRANTY = service_warranty,
                                                SERIAL_NO = serial_no,
                                                GENERIC_NAME = generic_name,
                                                CUSTOM_FIELD = custom_field,
                                                LIFE_IN_YEAR = life_in_year,
                                                DEP_TYPE = dep_type,
                                                LAST_SCAN_DATE = last_scan_date,
                                                NET_VALUE = net_value,
                                                REMAINING_YEARS = remaining_years,
                                                SALVAGE = salvage,
                                                LAST_YEAR_BV = last_year_bv,
                                                CURRENT_DEP = current_dep,
                                                TOTAL_DEP = total_dep,
                                                LOCATION = location,
                                                BRANCH_ID = branchcode,
                                                CREATED_BY = GlobalVariables.AppUserId,
                                                CREATED_AT = DateTime.Now,
                                                ASSETOLD_ID = AssetOld_id,
                                                TAGCOUNT = Tagcount,
                                                TAGCOUNTREMAIN = TagcountRemain,
                                            });
                        }

                        transaction.Commit();
                        return Json(new { success = true, message = "All Assets uploaded successfully!" });
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error occurred: " + ex.Message;
                }
            }

            return Json(new { success = false, message = ViewBag.Error });
        }


        private static void LogErrorToFile(string errorMessage)
        {
            string logFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/logs/error_log.txt");
            if (!string.IsNullOrEmpty(logFilePath))
            {
                // Ensure directory exists
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Write error to the file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + errorMessage);
                }
            }
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

            _converter.Error += (sender, args) =>
            {
                LogErrorToFile("DinkToPdf Error: " + args.Message);
            };
            try
            {
                var pdfDocument = new HtmlToPdfDocument()
                {
                    GlobalSettings = new GlobalSettings
                    {
                        //WkhtmltopdfPath = @"~\",
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
            catch (Exception ex)
            {
                // Log error to a file on the server
                LogErrorToFile("Error: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                throw;
            }
        }

        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            try
            { 
                // Initialize the HTML to PDF converter
                var htmlToPdfConverter = new HtmlToPdfConverter();

                // Optional: Configure PDF settings
                htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 10;
                htmlToPdfConverter.PdfDocumentOptions.RightMargin = 10;
                htmlToPdfConverter.PdfDocumentOptions.TopMargin = 10;
                htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 10;

                // Convert HTML content to PDF as a byte array
                byte[] pdfBytes = htmlToPdfConverter.ConvertHtml(htmlContent, "");
                return pdfBytes;
            }
            catch (Exception ex)
            {
                // Log error to a file on the server
                LogErrorToFile("Error: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                throw;
            }
            
        }

        private static void LogErrorToFile(string errorMessage)
        {
            string logFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/logs/error_log.txt");
            if (!string.IsNullOrEmpty(logFilePath))
            {
                // Ensure directory exists
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Write error to the file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + errorMessage);
                }
            }
        }

        public byte[] GeneratePdf(string viewName, object model, ControllerContext ControllerContext)
        {
            // Render the HTML content from the view
            string htmlContent = RenderViewToString(viewName, model, ControllerContext);

            // Generate PDF from HTML content
            PdfGenerator pdfGenerator = new PdfGenerator();
            //byte[] pdfBytes = GeneratePdfFromHtml(htmlContent);
            byte[] pdfBytes = ConvertHtmlToPdf(htmlContent);
            
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



