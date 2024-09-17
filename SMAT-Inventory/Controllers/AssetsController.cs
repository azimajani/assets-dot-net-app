using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SMAT_Inventory.Models;
using SMAT_Inventory.Dapper;
using Dapper;
using SMAT_Inventory;
using System.Globalization;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Reflection;
using OfficeOpenXml;


namespace SMAT_Inventory.Controllers
{
    public class AssetsController : Controller
    {
        // GET: Assets
        public ActionResult Index()
        {
            return View();
            
        }

        public ActionResult Login(LoginModel model)
        {
            //ViewBag.ReturnUrl = returnUrl;
            //GetWebSettings();
            //return View();

            LoginViewModel mode = new LoginViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                //LoginViewModel model = new LoginViewModel();
                //UserName = "Zeesshan";
                //Password = "123";
                dapper.AddUnicodeStringParameter("UserName", model.Email, 50);
                dapper.AddUnicodeStringParameter("Password", model.Password, 40);

                var result = dapper.query_multiple("dbo.UserAuth");
                if (result.hasRows() && model.Email!=null)
                {
                    mode.LoginList = result.Read<LoginModel>().ToList();
                    mode.SetModel();
                    if (mode.TotalCount > 0)
                    {
                        GetWebSettings();
                        return RedirectToAction("Dashboard","Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid username or password.";
                        return View();
                    }

                }
                else
                {
                    //ViewBag.ErrorMessage = "Invalid username or password.";
                    return View();
                }
            }

        }

        public ActionResult stafflogin(LoginModel model)
        {
            //ViewBag.ReturnUrl = returnUrl;
            //GetWebSettings();
            //return View();

            LoginViewModel mode = new LoginViewModel();

            using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
            {
                //LoginViewModel model = new LoginViewModel();
                //UserName = "Zeesshan";
                //Password = "123";
                dapper.AddUnicodeStringParameter("UserName", model.Email, 50);
                dapper.AddUnicodeStringParameter("Password", model.Password, 40);

                var result = dapper.query_multiple("dbo.UserAuth");
                if (result.hasRows() && model.Email != null)
                {
                    mode.LoginList = result.Read<LoginModel>().ToList();
                    mode.SetModel();
                    if (mode.TotalCount > 0)
                    {
                        GetWebSettings();
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid username or password.";
                        return View();
                    }

                }
                else
                {
                    //ViewBag.ErrorMessage = "Invalid username or password.";
                    return View();
                }
            }

        }
        private void GetWebSettings()
        {
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                try
                {
                    GlobalVariables.GetWebSettingValue.Clear();
                    List<WebSetting> WebSetting = new List<WebSetting>();
                    WebSetting = dapper.execute_stored_procedure<WebSetting>("dbo.GetWebSetting");

                    GlobalVariables.GetWebSettingValue.AddRange(WebSetting);
                }
                catch (Exception ex)
                {
                    //sysExceptions.logException(ex.Message, ex.StackTrace);
                }
            }
        }
        public ActionResult zonelist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            zoneViewModel Model = new zoneViewModel();
            
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.ZoneListGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.zonelist = result.Read<zoneModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);

            /*zoneViewModel Model = new zoneViewModel();
            DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE();
            var result = dapper.query_multiple("dbo.GetAllUsers");
            if (result.hasRows())
            {
                Model.zonelist = dapper.execute_stored_procedure<zoneModel>("dbo.ZoneListGet").ToList();
            }
            Model.SetModel();
            return View(Model);*/
        }
        public ActionResult zoneadd(int id=0)
        {

            // Initialize the view model
            zoneViewModel Model = new zoneViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zoneid", id);
                var result = dapper.query_multiple("dbo.ZoneSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.zone = result.Read<zoneModel>().SingleOrDefault();
                    Model.userlist = result.Read<userModel>().ToList();
                    Model.TotalCount = Model.zone.Branches;
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult zoneasave(zoneModel item)
        {

            // Initialize the view model

            zoneModel Test = new zoneModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zoneid", item.id);
                dapper.AddUnicodeStringParameter("zonename", item.name, 255);
                dapper.AddIntegerParameter("managerid", item.managerid);
                dapper.AddUnicodeStringParameter("staffid", item.staffid, 255);
                dapper.AddUnicodeStringParameter("email", item.email, 255);
                dapper.AddUnicodeStringParameter("mobile", item.mobile, 255);
                dapper.AddUnicodeStringParameter("alternate_no", item.alternate_no, 255);
                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                //var result = dapper.query_multiple("dbo.ZoneSave");
                Test = dapper.execute_stored_procedure<zoneModel>("dbo.ZoneSave").FirstOrDefault();
                //Test = dapper.execute_stored_procedure<zoneModel>("ZoneSave").FirstOrDefault();

                // Check if the result contains rows
                /*if (result.hasRows())
                {
                    Model.zone = result.Read<zoneModel>().SingleOrDefault();
                    Model.TotalCount = Model.zone.Branches;
                }*/
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult zonedelete(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.zonedelete");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult zonerestore(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            zoneViewModel Model = new zoneViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.ZoneRestore");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.zonelist = result.Read<zoneModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);

            /*zoneViewModel Model = new zoneViewModel();
            DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE();
            var result = dapper.query_multiple("dbo.GetAllUsers");
            if (result.hasRows())
            {
                Model.zonelist = dapper.execute_stored_procedure<zoneModel>("dbo.ZoneListGet").ToList();
            }
            Model.SetModel();
            return View(Model);*/
        }
        public ActionResult zonerestorenow(int id=0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.zonerestorenow");
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult branchlist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {
            // Initialize the view model
            branchViewModel Model = new branchViewModel();
            
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.BranchListGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult branchadd(int id = 0)
        {

            // Initialize the view model
            branchViewModel Model = new branchViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.BranchSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.branch = result.Read<branchModel>().SingleOrDefault();
                    Model.zonelist = result.Read<zoneModel>().ToList();
                    Model.citylist = result.Read<cityModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult branchsave(branchModel item)
        {

            // Initialize the view model

            branchModel Test = new branchModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", item.id);
                dapper.AddIntegerParameter("zone_id", item.zone_id);
                dapper.AddUnicodeStringParameter("code", item.code, 255);
                dapper.AddUnicodeStringParameter("name", item.name, 255);
                dapper.AddUnicodeStringParameter("phone1", item.phone1, 255);
                dapper.AddUnicodeStringParameter("phone2", item.phone2, 255);
                dapper.AddUnicodeStringParameter("phone3", item.phone3, 255);
                dapper.AddUnicodeStringParameter("email", item.email, 255);
                dapper.AddUnicodeStringParameter("branchemail", item.branchemail, 255);
                dapper.AddUnicodeStringParameter("address", item.address, 255);
                dapper.AddIntegerParameter("city_id", item.city_id);
                dapper.AddUnicodeStringParameter("postalcode", item.postalcode, 255);
                dapper.AddUnicodeStringParameter("googleMapId", item.googleMapId, 255);
                dapper.AddIntegerParameter("chkSUM", item.chkSUM);

                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                //var result = dapper.query_multiple("dbo.ZoneSave");
                Test = dapper.execute_stored_procedure<branchModel>("dbo.BranchSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult branchdelete(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.branchdelete");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult branchpermanentlyelete(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.branchpermanentlyelete");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult branchrestore(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            branchViewModel Model = new branchViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.BranchRestore");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult branchrestorenow(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.branchrestorenow");
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult userlist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.UserListGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.userlist = result.Read<userModel>().ToList();
                    Model.rolelist = result.Read<roleModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult useradd(int id = 0)
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.UserSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.user = result.Read<userModel>().SingleOrDefault();
                    Model.rolelist = result.Read<roleModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult usersave(userModel item)
        {
            userModel Test = new userModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", item.id);
                dapper.AddIntegerParameter("role_id", item.role_id);
                dapper.AddUnicodeStringParameter("name", item.name, 255);
                dapper.AddUnicodeStringParameter("email", item.email, 255);
                dapper.AddUnicodeStringParameter("phone", item.phone, 255);
                dapper.AddUnicodeStringParameter("address", item.address, 255);
                dapper.AddUnicodeStringParameter("CNIC", item.CNIC, 255);
                dapper.AddUnicodeStringParameter("password", item.password, 255);
                dapper.AddIntegerParameter("Active", item.Active);
                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                //var result = dapper.query_multiple("dbo.ZoneSave");
                Test = dapper.execute_stored_procedure<userModel>("dbo.UserSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult userpermission(int id =0)
        {
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.UserPermissions");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.userpermissions = result.Read<userpermissions>().ToList();
                    Model.user = result.Read<userModel>().SingleOrDefault();
                }
            }
            // Return the view with the populated model
            return View(Model);
        }
        [HttpPost]
        public ActionResult UpdateUserPermissions(List<userpermissions> permissions)
        {
            
            userModel Test = new userModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                foreach (var permission in permissions)
                {

                    dapper.AddIntegerParameter("UserId", permission.UserId);
                    dapper.AddUnicodeStringParameter("Module", permission.Module, 50);
                    dapper.AddUnicodeStringParameter("AddPermission", permission.AddPermission, 20);
                    dapper.AddUnicodeStringParameter("EditPermission", permission.EditPermission, 20);
                    dapper.AddUnicodeStringParameter("ViewPermission", permission.ViewPermission, 20);
                    dapper.AddUnicodeStringParameter("DeletePermission", permission.DeletePermission, 20);

                    var result = dapper.query_multiple("dbo.UpdateUserPermissions");
                    //Test = dapper.execute_stored_procedure<userModel>("dbo.UserSave").FirstOrDefault();
                }
            }
            // Return the view with the populated model
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);



            //return Json(new { success = true });
        }

        public ActionResult grouplist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {
            // Initialize the view model
            groupViewModel Model = new groupViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.GroupListGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.grouplist = result.Read<groupModel>().ToList();
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.citylist = result.Read<cityModel>().ToList();
                    Model.zonecount = result.Read<int>().SingleOrDefault();
                    Model.branchcount = result.Read<int>().SingleOrDefault();
                    Model.groupcount = result.Read<int>().SingleOrDefault();
                    Model.TotalCount = Model.groupcount;
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult grouprestore(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {
            // Initialize the view model
            groupViewModel Model = new groupViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.GroupRestore");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.grouplist = result.Read<groupModel>().ToList();
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.citylist = result.Read<cityModel>().ToList();
                    Model.zonecount = result.Read<int>().SingleOrDefault();
                    Model.branchcount = result.Read<int>().SingleOrDefault();
                    Model.groupcount = result.Read<int>().SingleOrDefault();
                    Model.TotalCount = Model.groupcount;
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult grouprestorenow(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.grouprestorenow");
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult groupadd(int id = 0)
        {

            // Initialize the view model
            groupViewModel Model = new groupViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.GroupSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.group = result.Read<groupModel>().SingleOrDefault();
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.citylist = result.Read<cityModel>().ToList();
                    Model.zonecount= result.Read<int>().SingleOrDefault();
                    Model.branchcount= result.Read<int>().SingleOrDefault();
                    Model.groupcount = result.Read<int>().SingleOrDefault();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult groupdelete(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.groupdelete");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult settings()
        {

            // Initialize the view model
            settingsViewModel Model = new settingsViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                var result = dapper.query_multiple("dbo.settings");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.city = result.Read<cityModel>().ToList();
                    Model.country_Currencies = result.Read<country_currencies>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult reportslist( string fromdate="", string todate="", int zone_id = 0, int branch_id = 0)
        {

            // Initialize the view model
            reportslsitViewModel Model = new reportslsitViewModel();


            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zone_id", zone_id);
                dapper.AddIntegerParameter("branch_id", branch_id);
                dapper.AddStringParameter("fromdate", fromdate,50);
                dapper.AddStringParameter("todate", todate,50);
                var result = dapper.query_multiple("dbo.reportslist");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.zone = result.Read<zoneModel>().ToList();
                    Model.branch= result.Read<branchModel>().ToList();
                    Model.reportlist = result.Read<reportlistModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult reportscustomlist(string fromdate = "", string todate = "", int report_type = 0)
        {
            // Initialize the view model
            reportslsitViewModel Model = new reportslsitViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("report_type", report_type);
                dapper.AddStringParameter("fromdate", fromdate, 50);
                dapper.AddStringParameter("todate", todate, 50);
                var result = dapper.query_multiple("dbo.reportscustomlist");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.reportlist = result.Read<reportlistModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult reportsscannedlist(string fromdate = "", string todate = "", int report_type = 0, int groupid=0, int branchid=0)
        {
            // Initialize the view model
            scanfileViewModel Model = new scanfileViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("report_type", report_type);
                dapper.AddStringParameter("fromdate", fromdate, 50);
                dapper.AddStringParameter("todate", todate, 50);
                dapper.AddIntegerParameter("groupid", groupid);
                dapper.AddIntegerParameter("branchid", branchid);
                var result = dapper.query_multiple("dbo.reportsscannedlist");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.scanfilelist = result.Read<scanfileModel>().ToList();
                    Model.grouplist = result.Read<groupModel>().ToList();
                    Model.branchlist = result.Read<branchModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult notification()
        {

            // Initialize the view model
            notificationViewModel Model = new notificationViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                var result = dapper.query_multiple("dbo.notification");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.notificationlist = result.Read<notificationModel>().ToList();
                    Model.user = result.Read<userModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult groupreporting()
        {

            // Initialize the view model
            groupViewModel Model = new groupViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                var result = dapper.query_multiple("dbo.groupreporting");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.grouplist = result.Read<groupModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult emailupdate()
        {
            emailupdateViewModel Model = new emailupdateViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                var result = dapper.query_multiple("dbo.emailupdate");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.branchcount = result.Read<int>().FirstOrDefault();
                    Model.groupcount = result.Read<int>().FirstOrDefault();
                    Model.branch = result.Read<branchModel>().ToList();
                    Model.group = result.Read<groupModel>().ToList();
                }
            }
            return View(Model);
        }
        public ActionResult emailupdateSave(string GorB, int itemid, string email )
        {

            // Initialize the view model

            branchModel Test = new branchModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            { 
                dapper.AddUnicodeStringParameter("GorB", GorB, 20);
                dapper.AddUnicodeStringParameter("email", email, 255);
                dapper.AddIntegerParameter("itemid", itemid);
                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                //var result = dapper.query_multiple("dbo.ZoneSave");
                Test = dapper.execute_stored_procedure<branchModel>("dbo.emailupdateSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult assetslist(int page = 1, int pageSize = 25, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            assetsViewModel Model = new assetsViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.AssetsList");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.assetlist = result.Read<assetsModel>().ToList();
                    Model.assetcount = result.Read<int>().SingleOrDefault();
                    Model.currentbookvalue = result.Read<double>().SingleOrDefault();
                    Model.TotalCount = Model.assetcount;// result.Read<int>().SingleOrDefault();

                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult assetadd(int id = 0)
        {

            // Initialize the view model
            assetsViewModel Model = new assetsViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.AssetSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.asset = result.Read<assetsModel>().SingleOrDefault();
                    Model.branchlist = result.Read<branchModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult assetsave(assetsModel item)
        {

            // Initialize the view model

            assetsModel Test = new assetsModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", item.id);
                dapper.AddIntegerParameter("branch_id", item.branch_id);
                dapper.AddUnicodeStringParameter("tag_id", item.tag_id, 255);
                dapper.AddUnicodeStringParameter("asset_id", item.asset_id, 255);
                dapper.AddUnicodeStringParameter("assetOld_id", item.assetOld_id, 255);
                dapper.AddUnicodeStringParameter("asset_name", item.asset_name, 255);
                dapper.AddIntegerParameter("invoice_cost", item.invoice_cost);
                dapper.AddIntegerParameter("total_cost", item.total_cost);
                dapper.AddDateTimeParameter("date_of_purchase", item.date_of_purchase);
                dapper.AddUnicodeStringParameter("vendor_id", item.vendor_id, 255);
                dapper.AddUnicodeStringParameter("vendor", item.vendor, 255);
                dapper.AddUnicodeStringParameter("part_warranty", item.part_warranty, 255);
                dapper.AddUnicodeStringParameter("service_warranty", item.service_warranty, 255);
                dapper.AddUnicodeStringParameter("serial_no", item.serial_no, 255);
                dapper.AddIntegerParameter("dep_type", item.dep_type);
                dapper.AddIntegerParameter("net_value", item.net_value);
                dapper.AddDecimalParameter("remaining_years", (decimal)item.remaining_years);
                dapper.AddIntegerParameter("life_in_year", item.life_in_year);
                dapper.AddIntegerParameter("salvage_value", item.salvage_value);
                dapper.AddUnicodeStringParameter("generic_name", item.generic_name, 255);
                dapper.AddUnicodeStringParameter("custom_field", item.custom_field, 255);
                dapper.AddIntegerParameter("Tagcount", item.Tagcount);
                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);

                //var result = dapper.query_multiple("dbo.ZoneSave");
                Test = dapper.execute_stored_procedure<assetsModel>("dbo.assetsave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult assetdelete(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.assetdelete");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult assetpermanentlydelete(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.assetpermanentlydelete");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult assetrestore(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            assetsViewModel Model = new assetsViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.AssetRestore");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.assetlist = result.Read<assetsModel>().ToList();
                    Model.assetcount = result.Read<int>().SingleOrDefault();
                    Model.currentbookvalue = result.Read<double>().SingleOrDefault();
                    Model.TotalCount = Model.assetcount;// result.Read<int>().SingleOrDefault();

                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Return the view with the populated model
            return View(Model);
        }
        public ActionResult assetrestorenow(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.assetrestorenow");
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult assetrollback(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            assetsViewModel Model = new assetsViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.AssetRollback");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.assetlist = result.Read<assetsModel>().ToList();
                    Model.assetcount = result.Read<int>().SingleOrDefault();
                    Model.currentbookvalue = result.Read<double>().SingleOrDefault();
                    Model.TotalCount = Model.assetcount;// result.Read<int>().SingleOrDefault();

                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Return the view with the populated model
            return View(Model);
        }

        // Action method to export data to CSV
        public ActionResult AssetExportToCSV()
        {
            try
            {
                // Define the stored procedure name
                string storedProcedureName = "AssetListExport";

                // Fetch the data using Dapper
                DataTable assetData = new DataTable();
                // = GetAssetData(storedProcedureName);
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var assetDataD = dapper.execute_stored_procedure<assetsModelExport>(storedProcedureName).ToList();
                    assetData = ConvertToDataTable(assetDataD);
                }

                // Convert the DataTable to CSV
                string csv = ConvertDataTableToCSV(assetData);

                // Return CSV file as download
                return File(new UTF8Encoding().GetBytes(csv), "text/csv", "AssetExportToCSV.csv");
            }
            catch (Exception ex)
            {
                // Handle exceptions as necessary
                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }
        // Method to convert DataTable to CSV format
        private string ConvertDataTableToCSV(DataTable dataTable)
        {
            StringBuilder csvBuilder = new StringBuilder();

            // Add the header row
            foreach (DataColumn column in dataTable.Columns)
            {
                csvBuilder.Append(column.ColumnName + ",");
            }

            csvBuilder.AppendLine();

            // Add the data rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    csvBuilder.Append(row[column].ToString().Replace(",", ";") + ",");
                }

                csvBuilder.AppendLine();
            }

            return csvBuilder.ToString();
        }
        // Helper method to convert List<T> to DataTable
        private DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get all the properties of the generic type class
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Define the columns based on the properties
            foreach (PropertyInfo prop in properties)
            {
                Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, propType);
            }

            // Populate the rows with data
            foreach (T item in items)
            {
                var values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public ActionResult assetscan()
        {
            // Initialize the view model
            assetscanViewModel Model = new assetscanViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                var result = dapper.query_multiple("dbo.assetscan");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.branchlist = result.Read<branchModel>().ToList();
                }
            }
            Model.SetModel();
            // Return the view with the populated model
            return View(Model);
        }

        public ActionResult reportdetailed()
        {
            return View();
        }

        public ActionResult assetbulkupdate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateAssetsFromExcel(HttpPostedFileBase scannerFile)
        { 
            if (scannerFile == null || scannerFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "File not uploaded." });
            }

            string fileName = DateTime.Now.Ticks + "-file.csv";
            string filePath = Path.Combine(Server.MapPath("~/App_Data/Asset_Update_Add"), fileName);

            // Save the uploaded file to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                scannerFile.InputStream.CopyTo(fileStream);
            }

                // Enable ExcelPackage to use EPPlus (requires setting the license context)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                // Load the Excel file
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming data is on the first sheet
                    var rowCount = worksheet.Dimension.Rows;

                    // Extract all tag_ids from the Excel file
                    var tagIdsFromExcel = new List<string>();
                    for (int row = 2; row <= rowCount; row++) // Skip header row (starts at row 2)
                    {
                        var tagId = worksheet.Cells[row, 3].GetValue<string>(); // Assuming tag_id is in column 2
                        tagIdsFromExcel.Add(tagId);
                    }

                    // Remove duplicates
                    tagIdsFromExcel = tagIdsFromExcel.Distinct().ToList();

                    string tagIdsInClause = string.Join(",", tagIdsFromExcel.Select(t => $"'{t}'"));

                    // Check if all tag_ids exist in the database
                    string query = $@"
                SELECT tag_id 
                FROM (VALUES {string.Join(", ", tagIdsFromExcel.Select(t => $"('{t}')"))}) AS ExcelTags(tag_id)
                EXCEPT
                SELECT tag_id FROM assets";

                    var missingTagIds = dapper.ExecuteQuery<string>(query).ToList();

                    if (missingTagIds.Any())
                    {
                        // If any tag_id is missing, log the issue and stop the process
                        Console.WriteLine("The following tag_ids are missing in the database: " + string.Join(", ", missingTagIds));
                        return Json(new { success = false, message = "The following tag_ids are missing in the database: " + string.Join(", ", missingTagIds) });
                    }

                     assetsModel Test = new assetsModel();

                    // If all tag_ids exist, proceed with update/insert process
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DAPPER_DATA_SERVICE dapper2 = new DAPPER_DATA_SERVICE();
                        branchViewModel Model = new branchViewModel();
                        assetsViewModel Modelasset = new assetsViewModel();
                        dapper2.AddUnicodeStringParameter("code", worksheet.Cells[row, 2].GetValue<string>(), 255);
                        dapper2.AddUnicodeStringParameter("tag_id", worksheet.Cells[row, 3].GetValue<string>(), 255);
                        var result = dapper2.query_multiple("dbo.BranchCodeGet");
                        if (result.hasRows())
                        {
                            Model.branch = result.Read<branchModel>().SingleOrDefault();
                            Modelasset.asset = result.Read<assetsModel>().SingleOrDefault();
                        }


                        dapper.AddIntegerParameter("id", Modelasset.asset.id);
                        dapper.AddIntegerParameter("branch_id", Model.branch.id);
                        dapper.AddUnicodeStringParameter("tag_id", worksheet.Cells[row, 3].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("asset_id", worksheet.Cells[row, 4].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("asset_name", worksheet.Cells[row, 5].GetValue<string>(), 255);
                        dapper.AddIntegerParameter("invoice_cost", Convert.ToInt32(worksheet.Cells[row, 6].Value));
                        dapper.AddIntegerParameter("total_cost", Convert.ToInt32(worksheet.Cells[row, 7].Value));
                        dapper.AddIntegerParameter("salvage_value", Convert.ToInt32(worksheet.Cells[row, 8].Value));
                        dapper.AddDateTimeParameter("date_of_purchase", worksheet.Cells[row, 9].GetValue<DateTime>());
                        dapper.AddUnicodeStringParameter("vendor_id", worksheet.Cells[row, 10].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("vendor", worksheet.Cells[row, 11].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("part_warranty", worksheet.Cells[row, 12].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("service_warranty", worksheet.Cells[row, 13].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("serial_no", worksheet.Cells[row, 14].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("generic_name", worksheet.Cells[row, 15].GetValue<string>(), 255);
                        dapper.AddUnicodeStringParameter("custom_field", worksheet.Cells[row, 16].GetValue<string>(), 255);
                        dapper.AddIntegerParameter("life_in_year", Convert.ToInt32(worksheet.Cells[row, 17].Value));
                        dapper.AddIntegerParameter("dep_type", Convert.ToInt32(worksheet.Cells[row, 18].Value));
                        dapper.AddIntegerParameter("net_value", Convert.ToInt32(worksheet.Cells[row, 20].Value));
                        dapper.AddDecimalParameter("remaining_years", Convert.ToDecimal(worksheet.Cells[row, 21].Value));
                        dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    
                        Test = dapper.execute_stored_procedure<assetsModel>("dbo.assetsave").FirstOrDefault();
                    }
                }
            }
            return Json(new { success = true, data = new { message = "done" } });
            //return Json("nothing...");
        }

        public ActionResult assetbulkdelete()
        {
            return View();
        }
        public ActionResult DeleteAssetsFromExcel(HttpPostedFileBase scannerFile)
        {
            if (scannerFile == null || scannerFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "File not uploaded." });
            }

            string fileName = DateTime.Now.Ticks + "-file.csv";
            string filePath = Path.Combine(Server.MapPath("~/App_Data/Asset_Update_Add"), fileName);

            // Save the uploaded file to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                scannerFile.InputStream.CopyTo(fileStream);
            }

            // Enable ExcelPackage to use EPPlus (requires setting the license context)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                // Load the Excel file
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assuming data is on the first sheet
                    var rowCount = worksheet.Dimension.Rows;

                    // Extract all tag_ids from the Excel file
                    var tagIdsFromExcel = new List<string>();
                    for (int row = 2; row <= rowCount; row++) // Skip header row (starts at row 2)
                    {
                        var tagId = worksheet.Cells[row, 3].GetValue<string>(); // Assuming tag_id is in column 2
                        tagIdsFromExcel.Add(tagId);
                    }

                    // Remove duplicates
                    tagIdsFromExcel = tagIdsFromExcel.Distinct().ToList();

                    string tagIdsInClause = string.Join(",", tagIdsFromExcel.Select(t => $"'{t}'"));

                    // Check if all tag_ids exist in the database
                    string query = $@"
                SELECT tag_id 
                FROM (VALUES {string.Join(", ", tagIdsFromExcel.Select(t => $"('{t}')"))}) AS ExcelTags(tag_id)
                EXCEPT
                SELECT tag_id FROM assets";

                    var missingTagIds = dapper.ExecuteQuery<string>(query).ToList();

                    if (missingTagIds.Any())
                    {
                        // If any tag_id is missing, log the issue and stop the process
                        Console.WriteLine("The following tag_ids are missing in the database: " + string.Join(", ", missingTagIds));
                        return Json(new { success = false, message = "The following tag_ids are missing in the database: " + string.Join(", ", missingTagIds) });
                    }

                    assetsModel Test = new assetsModel();

                    // If all tag_ids exist, proceed with update/insert process
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DAPPER_DATA_SERVICE dapper2 = new DAPPER_DATA_SERVICE();
                        branchViewModel Model = new branchViewModel();
                        assetsViewModel Modelasset = new assetsViewModel();
                        dapper2.AddUnicodeStringParameter("code", worksheet.Cells[row, 2].GetValue<string>(), 255);
                        dapper2.AddUnicodeStringParameter("tag_id", worksheet.Cells[row, 3].GetValue<string>(), 255);
                        var result = dapper2.query_multiple("dbo.BranchCodeGet");
                        if (result.hasRows())
                        {
                            Model.branch = result.Read<branchModel>().SingleOrDefault();
                            Modelasset.asset = result.Read<assetsModel>().SingleOrDefault();
                        }


                        dapper.AddIntegerParameter("id", Modelasset.asset.id);
                        

                        Test = dapper.execute_stored_procedure<assetsModel>("dbo.AssetDelete").FirstOrDefault();
                    }
                }
            }
            return Json(new { success = true, data = new { message = "done" } });
            //return Json("nothing...");
        }
        public JsonResult GetBranchbyZone(int zoneId)
        {
            branchViewModel Model = new branchViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zoneid", zoneId);
                Model.branchlist = dapper.execute_stored_procedure<branchModel>("dbo.GetBranchbyZone").ToList();
            }
                return Json(Model.branchlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchbyGroup(int GroupId)
        {
            branchViewModel Model = new branchViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("GroupId", GroupId);
                Model.branchlist = dapper.execute_stored_procedure<branchModel>("dbo.GetBranchbyGroup").ToList();
            }
            return Json(Model.branchlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult messagelist(int page = 1, int pageSize = 10, string sortColumn = "startdate", string sortOrder = "desc")
        {

            // Initialize the view model
            messagesViewModel Model = new messagesViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                var result = dapper.query_multiple("dbo.MessageListGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.messagelist = result.Read<messagesModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;

            // Finalize the model if necessary
            //Model.SetModel();

            // Return the view with the populated model
            return View(Model);

            /*zoneViewModel Model = new zoneViewModel();
            DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE();
            var result = dapper.query_multiple("dbo.GetAllUsers");
            if (result.hasRows())
            {
                Model.zonelist = dapper.execute_stored_procedure<zoneModel>("dbo.ZoneListGet").ToList();
            }
            Model.SetModel();
            return View(Model);*/
        }

        public ActionResult messageadd(int id = 0)
        {

            // Initialize the view model
            messagesViewModel Model = new messagesViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.MessageSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.message = result.Read<messagesModel>().SingleOrDefault();

                    Model.TotalCount = 1;
                }
            }
            // Return the view with the populated model
            return View(Model);

        }

        public ActionResult messagesave(messagesModel item)
        {

            // Initialize the view model

            messagesModel Test = new messagesModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", item.id);
                dapper.AddUnicodeStringParameter("subject", item.subject, 255);
                dapper.AddUnicodeStringParameter("message", item.message, 255);
                dapper.AddDateTimeParameter("startdate", item.startdate);
                dapper.AddDateTimeParameter("enddate", item.enddate);

                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                //var result = dapper.query_multiple("dbo.ZoneSave");
                Test = dapper.execute_stored_procedure<messagesModel>("dbo.MessageSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult messageview(int id = 0)
        {

            // Initialize the view model
            messagesViewModel Model = new messagesViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.MessageSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.message = result.Read<messagesModel>().SingleOrDefault();

                    Model.TotalCount = 1;
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
    }
}