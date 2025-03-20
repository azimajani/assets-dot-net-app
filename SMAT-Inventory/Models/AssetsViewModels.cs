using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;
using System.Data;

namespace SMAT_Inventory.Models
{

    public class zoneModel
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public int created_by { get; set; }
        public bool status { get; set; }
        public DateTime deleted_at { get; set; }
        public int Branches { get; set; }
        public string manager { get; set; }
        public string staffid { get; set; }
        public string mobile { get; set; }
        public string alternate_no { get; set; }
        public string email { get; set; }
        public int managerid { get; set; }
        public string staffcount { get; set; }

        public zoneModel()
        {
            this.id = 0;
            this.name = string.Empty;
            this.created_by = 0;
            this.status = false;
            this.deleted_at = DateTime.Now;
            this.Branches = 0;
            this.managerid = 0;
            this.staffcount = string.Empty;

        }
    }

    public class zoneViewModel
    {
        public List<zoneModel> zonelist { get; set; }
        public zoneModel zone { get; set; }
        public List<userModel> userlist { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public zoneViewModel()
        {
            this.zonelist = new List<zoneModel>();
            this.zone = new zoneModel();
            this.userlist= new List<userModel>();
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.zonelist.Count;
        }
    }


    public class WebSetting
    {
        public int id {get; set;}
        public string tag { get; set; }
        public string Value { get; set; }
        public string Created_by { get; set; }
        public int status { get; set; }

        public WebSetting()
        {
            this.id = 0;
            this.tag = string.Empty;
            this.Value = string.Empty;
            this.Created_by = string.Empty;
            this.status = 0;
        }
    }

    public class DMBranchModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string zone { get; set; }

        public DMBranchModel()
        {
            this.id = 0;
            this.code = string.Empty;
            this.name = string.Empty;
            this.zone = string.Empty;
        }
    }

    public class DMAssetModel
    {
        public int id { get; set; }
        public string zone { get; set; }
        public string branch { get; set; }
        public string Asset_id { get; set; }

        public DMAssetModel()
        {
            this.id = 0;
            this.zone = string.Empty;
            this.branch = string.Empty;
            this.Asset_id = string.Empty;
        }
    }

    public class DashBoardViewModel
    {
        public int zonecount { get; set; }
        public int branchcount { get; set; }
        public int assetscount { get; set; }
        public List<DMBranchModel> DMBranch { get; set; }
        public List<DMAssetModel> DMAsset { get; set; }
        public List<messagesModel> message { get; set; }
        public List<WebSetting> contactPERlist { get; set; }
        public List<WebSetting> contactSMAlist { get; set; }
        public DashBoardViewModel()
        {
            this.zonecount = 0;
            this.branchcount = 0;
            this.assetscount = 0;
            this.DMBranch = new List<DMBranchModel>();
            this.DMAsset = new List<DMAssetModel>();
            this.message = new List<messagesModel>();
        }
    }

    public class cityModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string status { get; set; }
        public DateTime deleted_at { get; set; }

        public cityModel()
        {
            this.id = 0;
            this.name = string.Empty;
            this.lat = 0;
            this.lng = 0;
            this.status = string.Empty;
            this.deleted_at = DateTime.Now;
        }
    }
    public class branchModel
    {
        public int id { get; set; }
        public int zone_id { get; set; }
        public string zone_name { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string phone3 { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public int city_id { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public string postalcode { get; set; }
        public string googleMapId { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool status { get; set; }
        public DateTime deleted_at { get; set; }
        public string city { get; set; }
        public string zone { get; set; }
        public string branchemail { get; set; }
        public int chkSUM { get; set; }

        public branchModel()
        {
            this.id = 0;
            this.zone_id = 0;
            this.zone_name = string.Empty;
            this.code = string.Empty;
            this.name = string.Empty;
            this.phone1 = string.Empty;
            this.phone2 = string.Empty;
            this.phone3 = string.Empty;
            this.email = string.Empty;
            this.address = string.Empty;
            this.city_id = 0;
            this.created_by = 0;
            this.updated_by = 0;
            this.postalcode = string.Empty;
            this.googleMapId = string.Empty;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.status = false;
            this.deleted_at = DateTime.Now;
            this.city = string.Empty;
            this.zone = string.Empty;
            this.branchemail = string.Empty;
            this.chkSUM = 1;
        }
    }

    public class branchViewModel
    {
        public List<branchModel> branchlist { get; set; }
        public branchModel branch { get; set; }
        public List<zoneModel> zonelist { get; set; }
        public List<cityModel> citylist { get; set; }
        public List<groupModel> grouplist { get; set; }
        public List<branchModelforgoogle> branchModelforgoogle { get; set; }
        public List<messagesModel> messageslist { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public branchViewModel()
        {
            this.branchlist = new List<branchModel>();
            this.branch = new branchModel();
            this.zonelist = new List<zoneModel>();
            this.citylist = new List<cityModel>();
            this.grouplist = new List<groupModel>();
            this.branchModelforgoogle = new List<branchModelforgoogle>();
            this.messageslist = new List<messagesModel>();
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.branchlist.Count;
        }
    }

    public class userModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime email_verified_at { get; set; }
        public string password { get; set; }
        public string image { get; set; }
        public string remember_token { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime deleted_at { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }
        public string CNIC { get; set; }
        public string address { get; set; }
        public int Active { get; set; }
        public DateTime DOB { get; set; }
        public string zones { get; set; }
        public int AssetTransfer_AddReviewer { get; set; }
        public int AssetTransfer_Approver { get; set; }
        public int AssetTransfer_Receiver { get; set; }
        public userModel()
        {
            this.id = 0;
            this.name = string.Empty;
            this.phone = string.Empty;
            this.email = string.Empty;
            this.email_verified_at = DateTime.Now;
            this.password = string.Empty;
            this.image = string.Empty;
            this.remember_token = string.Empty;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.deleted_at = DateTime.Now;
            this.role_id = 0;
            this.role_name = string.Empty;
            this.CNIC = string.Empty;
            this.address = string.Empty;
            this.Active = 0;
            this.DOB = DateTime.Now;
            this.zones = string.Empty;
            this.AssetTransfer_AddReviewer = 0;
            this.AssetTransfer_Approver = 0;
            this.AssetTransfer_Receiver = 0;
        }
    }

    public class roleModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public roleModel()
        {
            this.id = 0;
            this.name = string.Empty;
        }
    }

    public class UserViewModel
    {
        public List<userModel> userlist { get; set; }
        public userModel user { get; set; }
        public List<roleModel> rolelist { get; set; }
        public List<userpermissions> userpermissions { get; set; }
        public List<zoneModel> zones { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public UserViewModel()
        {
            this.userlist = new List<userModel>();
            this.user = new userModel();
            this.rolelist = new List<roleModel>();
            this.userpermissions = new List<userpermissions>();
            this.zones = new List<zoneModel>();
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.userlist.Count;
        }
    }

    public class groupModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string person_name { get; set; }
        public string person_designation { get; set; }
        public string person_number { get; set; }
        public string person_email { get; set; }
        public string branches { get; set; }
        public int city_id { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool status { get; set; }
        public DateTime deleted_at { get; set; }
        public int BranchesinGroup { get; set; }
        public string BranchesAll { get; set; }

        public groupModel()
        {
            this.id = 0;
            this.name = string.Empty;
            this.person_name = string.Empty;
            this.person_designation = string.Empty;
            this.person_number = string.Empty;
            this.person_email = string.Empty;
            this.branches = string.Empty;
            this.city_id = 0;
            this.created_by = 0;
            this.updated_by = 0;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.status = false;
            this.deleted_at = DateTime.Now;
            this.BranchesinGroup = 0;
            this.BranchesAll = string.Empty;
        }
    }

    public class groupViewModel
    {
        public List<groupModel> grouplist { get; set; }
        public groupModel group { get; set; }
        public List<cityModel> citylist { get; set; }
        public List<branchModel> branchlist { get; set; }
        public int zonecount { get; set; }
        public int branchcount { get; set; }
        public int groupcount { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public groupViewModel()
        {
            this.grouplist = new List<groupModel>();
            this.group = new groupModel();
            this.citylist = new List<cityModel>();
            this.branchlist = new List<branchModel>();
            this.TotalCount = 0;
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.grouplist.Count;
        }
    }

    public class country_currencies
    {
        public int id { get; set; }
        public string aplha_code2 { get; set; }
        public string aplha_code3 { get; set; }
        public int numeric_code { get; set; }
        public string country_name { get; set; }
        public string currency_name { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public string currency_column { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public bool status { get; set; }
        public DateTime deleted_at { get; set; }


        public country_currencies()
        {
            this.id = 0;
            this.aplha_code2 = string.Empty;
            this.aplha_code3 = string.Empty;
            this.numeric_code = 0;
            this.country_name = string.Empty;
            this.currency_name = string.Empty;
            this.currency_code = string.Empty;
            this.currency_symbol = string.Empty;
            this.currency_column = string.Empty;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.created_by = 0;
            this.updated_by = 0;
            this.status = false;
            this.deleted_at = DateTime.Now;
        }
    }
    public class settingsViewModel
    {
        public List<cityModel> city { get; set; }
        public List<country_currencies> country_Currencies { get; set; }
        public List<WebSetting> contactPERlist { get; set; }
        public List<WebSetting> contactSMAlist { get; set; }
        public settingsViewModel()
        {
            this.city = new List<cityModel>();
            this.country_Currencies = new List<country_currencies>();
            this.contactPERlist = new List<WebSetting>();
            this.contactSMAlist = new List<WebSetting>();
        }
    }

    public class reportlistModel
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string report_no { get; set; }
        public string report { get; set; }
        public int calculation { get; set; }
        public int scanned_times { get; set; }
        public DateTime created_at { get; set; }
        public int created_by { get; set; }
        public int status { get; set; }
        public DateTime deleted_at { get; set; }
        public string branchname { get; set; }
        public int zoneid { get; set; }
        public string zonename { get; set; }

        public reportlistModel()
        {
            id = 0;
            branch_id = 0;
            report_no = string.Empty;
            report = string.Empty;
            calculation = 0;
            scanned_times = 0;
            created_at = DateTime.Now;
            created_by = 0;
            status = 0;
            deleted_at = DateTime.Now;
            branchname = string.Empty;
            zoneid = 0;
            zonename = string.Empty;
        }

    }
    public class reportslsitViewModel
    {
        public List<zoneModel> zone { get; set; }
        public List<branchModel> branch { get; set; }
        public List<reportlistModel> reportlist { get; set; }
        public reportslsitViewModel()
        {
            this.zone = new List<zoneModel>();
            this.branch = new List<branchModel>();
            this.reportlist = new List<reportlistModel>();
        }
    }

    public class notificationModel
    {
        public Guid id { get; set; }
        public string type { get; set; }
        public string notifiable_type { get; set; }
        public int notifiable_id { get; set; }
        public int performed_by { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string data_value { get; set; }
        public string data { get; set; }
        public DateTime read_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string username { get; set; }

        public notificationModel()
        {
            this.id = Guid.NewGuid();
            this.type = string.Empty;
            this.notifiable_type = string.Empty;
            this.notifiable_id = 0;
            this.data = string.Empty;
            //this.read_at = null;
            //this.created_at = DateTime.Now;
            //this.updated_at = DateTime.Now;
        }
    }

    public class notificationViewModel
    {
        public List<notificationModel> notificationlist { get; set; }
        public notificationModel notification { get; set; }
        public List<notificationTypeModel> notificationTypelist { get; set; }
        public List<userModel> user { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public int performedBy { get; set; }
        public string notification_type { get; set; }
        public notificationViewModel()
        {
            this.notificationlist = new List<notificationModel>();
            this.notification = new notificationModel();
            this.user = new List<userModel>();
            this.notificationTypelist = new List<notificationTypeModel>();

        }
    }

    public class emailupdateViewModel
    {
        public int branchcount { get; set; }
        public int groupcount { get; set; }
        public List<branchModel> branch { get; set; }
        public List<groupModel> group { get; set; }
        public emailupdateViewModel()
        {
            this.branchcount = 0;
            this.groupcount = 0;
            this.branch = new List<branchModel>();
            this.group = new List<groupModel>();
        }
    }

    public class userpermissions
    {
        public int UserId { get; set; }
        public string Module { get; set; }
        public string AddPermission { get; set; }
        public string EditPermission { get; set; }
        public string ViewPermission { get; set; }
        public string DeletePermission { get; set; }

        public userpermissions()
        {
            this.UserId = 0;
            this.Module = string.Empty;
            this.AddPermission = string.Empty;
            this.EditPermission = string.Empty;
            this.ViewPermission = string.Empty;
            this.DeletePermission = string.Empty;
        }
    }

    public class scanfileModel
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string file_name { get; set; }
        public int calculation { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public int status { get; set; }
        public DateTime deleted_at { get; set; }

        public scanfileModel()
        {
            this.id = 0;
            this.branch_id = 0;
            this.file_name = string.Empty;
            this.calculation = 0;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.created_by = 0;
            this.updated_by = 0;
            this.status = 0;
            this.deleted_at = DateTime.Now;
        }
    }

    public class scanfileViewModel
    {
        public List<scanfileModel> scanfilelist = new List<scanfileModel>();
        public scanfileModel scanfile = new scanfileModel();

        public List<branchModel> branchlist = new List<branchModel>();
        public List<groupModel> grouplist = new List<groupModel>();
        public scanfileViewModel()
        {
            this.scanfilelist = new List<scanfileModel>();
            this.scanfile = new scanfileModel();
            this.branchlist = new List<branchModel>();
            this.grouplist = new List<groupModel>();
        }
    }

    public class assetsModelExport
    {
        public string tag_id { get; set; }
        public string asset_id { get; set; }
        public string asset_name { get; set; }
        public int invoice_cost { get; set; }
        public int total_cost { get; set; }
        public int salvage_value { get; set; }
        public DateTime date_of_purchase { get; set; }
        public string vendor_id { get; set; }
        public string vendor { get; set; }
        public string part_warranty { get; set; }
        public string service_warranty { get; set; }
        public string serial_no { get; set; }
        public string generic_name { get; set; }
        public string custom_field { get; set; }
        public int life_in_year { get; set; }
        public int dep_type { get; set; }
        public DateTime last_scan_date { get; set; }
        public int net_value { get; set; }
        public double remaining_years { get; set; }
        public double salvage { get; set; }
        public int last_year_bv { get; set; }
        public double current_dep { get; set; }
        public double total_dep { get; set; }
        public string location { get; set; }
        public string branch_code { get; set; }
        public string AssetOld_id { get; set; }
        public int Tagcount { get; set; }
        public int TagcountRemain { get; set; }

        public assetsModelExport()
        {
            this.tag_id = string.Empty;
            this.asset_id = string.Empty;
            this.asset_name = string.Empty;
            this.invoice_cost = 0;
            this.total_cost = 0;
            this.salvage_value = 0;
            this.date_of_purchase = DateTime.Now;
            this.vendor_id = string.Empty;
            this.vendor = string.Empty;
            this.part_warranty = string.Empty;
            this.service_warranty = string.Empty;
            this.serial_no = string.Empty;
            this.generic_name = string.Empty;
            this.custom_field = string.Empty;
            this.life_in_year = 0;
            this.dep_type = 0;
            this.last_scan_date = DateTime.Now;
            this.net_value = 0;
            this.remaining_years = 0;
            this.salvage = 0;
            this.last_year_bv = 0;
            this.current_dep = 0;
            this.total_dep = 0;
            this.location = string.Empty;
            this.branch_code = string.Empty;
        }
    }



    public class BranchModelExport
    {
        public string zone { get; set; }
        public string branch_code { get; set; }
        public string branch_name { get; set; }
        public string phone_no1 { get; set; }
        public string phone_no2 { get; set; }
        public string phone_no3 { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string postalcode { get; set; }
        public string googleMapId { get; set; }
        public string branchemail { get; set; }
        public int chkSUM { get; set; }

        public BranchModelExport()
        {
            this.zone = string.Empty;
            this.branch_code = string.Empty;
            this.branch_name = string.Empty;
            this.phone_no1 = string.Empty;
            this.phone_no2 = string.Empty;
            this.phone_no3 = string.Empty;
            this.email = string.Empty;
            this.address = string.Empty;
            this.city = string.Empty;
            this.postalcode = string.Empty;
            this.googleMapId = string.Empty;
            this.branchemail = string.Empty;
            this.chkSUM = 0;
        }

    }
    public class assetsModel
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string tag_id { get; set; }
        public string asset_id { get; set; }
        public string assetOld_id { get; set; }
        public string asset_name { get; set; }
        public int invoice_cost { get; set; }
        public int total_cost { get; set; }
        public int salvage_value { get; set; }
        public DateTime date_of_purchase { get; set; }
        public string vendor_id { get; set; }
        public string vendor { get; set; }
        public string part_warranty { get; set; }
        public string service_warranty { get; set; }
        public string serial_no { get; set; }
        public string generic_name { get; set; }
        public string custom_field { get; set; }
        public int life_in_year { get; set; }
        public int dep_type { get; set; }
        public DateTime last_scan_date { get; set; }
        public int net_value { get; set; }
        public int remaining_years { get; set; }
        public int salvage { get; set; }
        public int last_year_bv { get; set; }
        public double current_dep { get; set; }
        public double total_dep { get; set; }
        public string location { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public int status { get; set; }
        public DateTime deleted_at { get; set; }
        public int Tagcount { get; set; }
        public int TagcountRemain { get; set; }

        public assetsModel()
        {
            this.id = 0;
            this.branch_id = 0;
            this.tag_id = string.Empty;
            this.asset_id = string.Empty;
            this.assetOld_id = string.Empty;
            this.asset_name = string.Empty;
            this.invoice_cost = 0;
            this.total_cost = 0;
            this.salvage_value = 0;
            this.date_of_purchase = DateTime.Now;
            this.vendor_id = string.Empty;
            this.vendor = string.Empty;
            this.part_warranty = string.Empty;
            this.service_warranty = string.Empty;
            this.serial_no = string.Empty;
            this.generic_name = string.Empty;
            this.custom_field = string.Empty;
            this.life_in_year = 0;
            this.dep_type = 0;
            this.last_scan_date = DateTime.Now;
            this.net_value = 0;
            this.remaining_years = 0;
            this.salvage = 0;
            this.last_year_bv = 0;
            this.current_dep = 0;
            this.total_dep = 0;
            this.location = string.Empty;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.created_by = 0;
            this.updated_by = 0;
            this.status = 0;
            this.deleted_at = DateTime.Now;
            this.Tagcount = 0;
            this.TagcountRemain = 0;
        }
    }

    public class assetsViewModel
    {
        public List<assetsModel> assetlist { get; set; }
        public assetsModel asset { get; set; }
        public List<branchModel> branchlist { get; set; }
        public List<zoneModel> zonelist { get; set; }
        public List<groupModel> grouplist { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int assetcount { get; set; }
        public double currentbookvalue { get; set; }
        

        public assetsViewModel()
        {
            this.assetlist = new List<assetsModel>();
            this.asset = new assetsModel();
            this.branchlist = new List<branchModel>();
            this.zonelist = new List<zoneModel>();
            this.grouplist = new List<groupModel>();
            this.assetcount = 0;
            this.currentbookvalue = 0;
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.assetlist.Count;
        }
    }

    public class assetstransferModel
    {

        public int id { get; set; }
        public int AssetId { get; set; }
        public int frombranchcode { get; set; }
        public string fromlocation { get; set; }
        public int tobranchcode { get; set; }
        public string tolocation { get; set; }
        public DateTime created_at { get; set; }
        public int created_by { get; set; }
        public string Description { get; set; }
        public DateTime reviewed_at { get; set; }
        public DateTime approver_at { get; set; }
        public DateTime recceived_at { get; set; }
        public int reviewed_by { get; set; }
        public int approved_by { get; set; }
        public int received_by { get; set; }
        public string transferdetail { get; set; }
        public DateTime receiptprint_at { get; set; }
        public int receiptprint_by { get; set; }
        public int transstatus { get; set; }
        public string frombranchname { get; set; }
        public string tobranchname { get; set; }
        public string created_user { get; set; }
        public string reviewed_user { get; set; }
        public string approved_user { get; set; }
        public string received_user { get; set; }
        public string receiptprint_user { get; set; }
        public int deleted_by { get; set; }
        public DateTime deleted_at { get; set; }
        public string asset_id { get; set; }
        public string asset_name { get; set; }
        public string Status { get; set; }
        public string ReviewRemarks { get; set; }
        public string ApproveRemarks { get; set; }
        public assetstransferModel()
        {
            this.id = 0;
            this.AssetId = 0;
            this.frombranchcode = 0;
            this.fromlocation = string.Empty;
            this.tobranchcode = 0;
            this.tolocation = string.Empty;
            this.created_at = DateTime.Now;
	        this.created_by =0;
	        this.Description = string.Empty;
            this.reviewed_at = DateTime.Now;
            this.approver_at = DateTime.Now;
            this.recceived_at = DateTime.Now;
	        this.reviewed_by =0;
	        this.approved_by =0;
	        this.received_by =0;
	        this.transferdetail  = string.Empty;
	        this.receiptprint_at = DateTime.Now;
	        this.receiptprint_by =0;
	        this.transstatus =0;
            this.frombranchname = string.Empty;
            this.tobranchname = string.Empty;
            this.created_user = string.Empty;
            this.received_user = string.Empty;
            this.approved_user = string.Empty;
            this.received_user = string.Empty ;
            this.receiptprint_user = string.Empty ;
            this.deleted_by = 0;
            this.deleted_at = DateTime.Now;
            this.asset_id = string.Empty;
            this.asset_name = string.Empty;
            this.Status = string.Empty;
            this.ReviewRemarks = string.Empty;
            this.ApproveRemarks = string.Empty;
        }
    }

    public class assetstransferViewModel
    {
        public List<assetstransferModel> assettransferlist { get; set; }
        public assetstransferModel assettransfer { get; set; }
        public List<branchModel> branchlist { get; set; }
        public List<zoneModel> zonelist { get; set; }
        public List<groupModel> grouplist { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int assetcount { get; set; }
        public double currentbookvalue { get; set; }


        public assetstransferViewModel()
        {
            this.assettransferlist = new List<assetstransferModel>();
            this.assettransfer = new assetstransferModel();
            this.branchlist = new List<branchModel>();
            this.zonelist = new List<zoneModel>();
            this.grouplist = new List<groupModel>();
            this.assetcount = 0;
            this.currentbookvalue = 0;
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.assettransferlist.Count;
        }
    }
    public class AssetScannerFileModel
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string file_name { get; set; }
        public bool calculation { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public bool status { get; set; }
        public DateTime deleted_at { get; set; }

        public AssetScannerFileModel()
        {
            this.id = 0;
            this.branch_id = 0;
            this.file_name = string.Empty;
            this.calculation = false;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.created_by = 0;
            this.updated_by = 0;
            this.status = false;
            this.deleted_at = DateTime.Now;
        }


    }

    public class AssetScannerModel
    {
        public int id { get; set; }
        public string chk_sum_1 { get; set; }
        public int file_id { get; set; }
        public string tag_id { get; set; }
        public int pc { get; set; }
        public int count { get; set; }
        public int rssi { get; set; }
        public int riss { get; set; }
        public string chk_sum_2 { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public bool status { get; set; }
        public DateTime deleted_at { get; set; }

        public AssetScannerModel()
        {
            this.id = 0;
            this.chk_sum_1 = string.Empty;
            this.file_id = 0;
            this.tag_id = string.Empty;
            this.pc = 0;
            this.count = 0;
            this.rssi = 0;
            this.riss = 0;
            this.chk_sum_2 = string.Empty;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            this.created_by = 0;
            this.updated_by = 0;
            this.status = false;
            this.deleted_at = DateTime.Now;


        }
    }

    public class assetscanViewModel
    {
        public List<branchModel> branchlist { get; set; }
        public List<assetsModel> matched { get; set; }
        public List<assetsModel> unmatched { get; set; }
        public int count_totalRecords { get; set; }
        public int count_matchedRecords { get; set; }

        public assetscanViewModel()
        {
            this.branchlist = new List<branchModel>();
            this.matched = new List<assetsModel>();
            this.unmatched = new List<assetsModel>();
            this.count_totalRecords = 0;
            this.count_matchedRecords = 0;
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.count_totalRecords = this.matched.Count + this.unmatched.Count;
            this.count_matchedRecords = this.matched.Count;
        }

    }

    public class report_detailViewModel
    {
        public string title { get; set; }
        public DateTime date { get; set; }
        public List<dynamic> user { get; set; }
        public string company_name { get; set; }
        public string zone { get; set; }
        public List<dynamic> branches { get; set; }
        public string city { get; set; }
    }

    public class ReportDetailViewModel
    {
        public string title { get; set; }
        public string date { get; set; }
        public dynamic user { get; set; }
        public string company_name { get; set; }
        public string zone { get; set; }
        public string city { get; set; }
        public dynamic branches { get; set; }
        public dynamic matched_assets  { get; set; }
        public dynamic unmatched_assets { get; set; }
        public dynamic otherBranchAssets { get; set; }
        public string reporting_no { get; set; }
    }

    public class messagesModel
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public int active { get; set; }
        public int branch_id { get; set; }
        public int group_id { get; set; }
        public int zone_id { get; set; }
        public int city_id { get; set; }
        public string branchname { get; set; }
        public string groupname { get; set; }
        public string zonename { get; set; }
        public string cityname { get; set; }
        public string displaycolor { get; set; }

        public messagesModel()
        {
            this.id = 0;
            this.subject = string.Empty;
            this.message = string.Empty;
            this.startdate= DateTime.Now;
            this.enddate= DateTime.Now;
            this.Created_by = 0;
            this.Created_at = DateTime.Now;
            this.active = 0;
            this.branch_id = 0;
            this.zone_id = 0;
            this.group_id = 0;
            this.city_id = 0;
            this.branchname = string.Empty;
            this.zonename = string.Empty;
            this.groupname = string.Empty;
            this.cityname = string.Empty;
            this.displaycolor = string.Empty;
        }
        
    }

    public class messagesViewModel
    {
        public List<messagesModel> messagelist { get; set; }
        public List<branchModel> branchlist { get; set; }
        public List<zoneModel> zonelist { get; set; }
        public List<groupModel> grouplist { get; set; }
        public List<cityModel> citylist { get; set; }
        public messagesModel message { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        
        public messagesViewModel()
        {
            this.messagelist = new List<messagesModel>();
            this.message = new messagesModel();
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.messagelist.Count;
        }
    }

    public class branchModelforgoogle
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string Latitude { get; set; } // Ensure you have Latitude as a string or decimal
        public string Longitude { get; set; } // Ensure you have Longitude as a string or decimal
    }


    public class ReportingRequest
    {
        public int GroupId { get; set; }            // ID of the group (zone or branch)
        public string Group { get; set; }           // Name or identifier for the group
        public string View { get; set; }            // Action type, like "view_report"
        public string Download { get; set; }        // Action type, like "download_report"
        public DateTime? DateFrom { get; set; }     // Start date for the report period
        public DateTime? DateTo { get; set; }       // End date for the report period
        public string Calculation { get; set; }     // Whether calculations are needed ("yes" or "no")
        public string ReportType { get; set; }      // Type of report (optional, depending on the logic)

        // Add additional fields if necessary, depending on the form data structure
    }

    public class groupreportsummaryModel
    {
        public int branch_id { get; set; }
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public DateTime scan_date { get; set; }
        public int matched_count { get; set; }
        public int unmatched_count { get; set; }
        public int total_assets { get; set; }
        public int last_bv { get; set; }
        public int current_bv { get; set; }
        public int current_dep { get; set; }
        public int total_cost { get; set; }

    }

    public class groupreportsummaryViewModel
    {
        public List<groupreportsummaryModel> groupreportsummarylist { get; set; }
        public groupreportsummaryModel groupreportsummary { get; set; }
        public groupModel group {  get; set; }
        public int TotalCount { get; set; }
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public int calculation { get; set; }
        public groupreportsummaryViewModel()
        {
            this.groupreportsummarylist = new List<groupreportsummaryModel>();
            this.groupreportsummary = new groupreportsummaryModel();
            this.group = new groupModel();
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.groupreportsummarylist.Count;
        }
    }

    public class groupreportdetailModel
    {
        public int branch_id { get; set; }
        public string branchcode { get; set; }
        public string branchname { get; set; }
        public string asset_id { get; set; }
        public string asset_name { get; set; }
        public string invoice_cost { get; set; }
        public DateTime purchase_date { get; set; }
        public int total_cost { get; set; }
        public string location { get; set; }
        public string remaining_years { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public int dep_type { get; set; }

    }

    public class groupreportdetailViewModel
    {
        public List<groupreportdetailModel> groupreportdetaillist { get; set; }
        public List<groupreportdetailModel> groupreportdetaiunllist { get; set; }
        public groupreportdetailModel groupreportdetail { get; set; }
        public groupModel group { get; set; }
        public int TotalCount { get; set; }
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public int calculation { get; set; }
        public groupreportdetailViewModel()
        {
            this.groupreportdetaillist = new List<groupreportdetailModel>();
            this.groupreportdetaiunllist = new List<groupreportdetailModel>();
            this.groupreportdetail = new groupreportdetailModel();
            this.group = new groupModel();
        }

        public void SetModel()
        {
            this.SetCount();
        }

        private void SetCount()
        {
            this.TotalCount = this.groupreportdetaillist.Count;
        }
    }

    public class notificationTypeModel
    {
        public int id { get; set; }
        public string  name { get; set; }
        public DateTime deleted_at { get; set; }

        public notificationTypeModel()
        {
            this.id= 0;
            this.name = string.Empty;
            this.deleted_at = DateTime.Now;
        }
    }

    public class notificationTypeViewModel
    {
        public List<notificationTypeModel> notificationTypelist { get; set; }
        public notificationTypeModel notificationType { get; set; }

        public notificationTypeViewModel()
        {
            this.notificationTypelist = new List<notificationTypeModel>();
            this.notificationType = new notificationTypeModel();
        }
    }
}
