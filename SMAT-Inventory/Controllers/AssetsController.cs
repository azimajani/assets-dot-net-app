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
using System.Xml.Linq;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

using System.Net.Http;
using Newtonsoft.Json.Linq;
using static QRCoder.PayloadGenerator;
using Microsoft.Ajax.Utilities;
using System.Security.Policy;
using OfficeOpenXml.Packaging.Ionic.Zlib;

namespace SMAT_Inventory.Controllers
{
    //[Authorize]
    public class AssetsController : Controller
    {
        
        public AssetsController()
        {
            if (GlobalVariables.AppUserId==0)
            {

            }

        }
        // GET: Assets
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult test()
        {
            return View();

        }

        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            //ViewBag.ReturnUrl = returnUrl;
            //GetWebSettings();
            //return View();
            GlobalVariables.AppUserId =0;
            GlobalVariables.AppUserName = "";
            GlobalVariables.AppUserEmail = "";
            GlobalVariables.remember_token = "";
            GlobalVariables.role = "";

            //User.Identity.IsAuthenticated
            //HttpCookie userSettingsCookieGet = Request.Cookies["SATUserSettings"];

            LoginViewModel mode = new LoginViewModel();
            if (model.logintype == "stafflogin")
            {
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
                        mode.LoginModel = result.Read<LoginModel>().FirstOrDefault();
                        mode.SetModel();
                        if (mode.LoginModel != null)
                        {
                            GlobalVariables.AppUserId = mode.LoginModel.id;
                            GlobalVariables.AppUserName = mode.LoginModel.Name;
                            GlobalVariables.AppUserEmail = mode.LoginModel.Email;
                            GlobalVariables.remember_token = mode.LoginModel.remember_token;
                            GlobalVariables.role = mode.LoginModel.role;
                            GlobalVariables.AppUserPassword = model.Password;
                            GlobalVariables.logintype = model.logintype;

                            GetWebSettings();
                            GetUserPermissions();
                            return RedirectToAction("Dashboard", "Home");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid username or password.";
                            return View("Login");
                        }

                    }
                    else
                    {
                        //ViewBag.ErrorMessage = "Invalid username or password.";
                        return View("Login");
                    }
                } 
            }
            else
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    //LoginViewModel model = new LoginViewModel();
                    //UserName = "Zeesshan";
                    //Password = "123";
                    dapper.AddUnicodeStringParameter("UserName", model.Email, 50);
                    dapper.AddUnicodeStringParameter("Password", model.Password, 40);

                    var result = dapper.query_multiple("dbo.UserAuth");
                    if (result.hasRows() && model.Email != null)
                    {
                        mode.LoginModel = result.Read<LoginModel>().FirstOrDefault();
                        mode.SetModel();
                        if (mode.LoginModel != null)
                        {
                            GlobalVariables.AppUserId = mode.LoginModel.id;
                            GlobalVariables.AppUserName = mode.LoginModel.Name;
                            GlobalVariables.AppUserEmail = mode.LoginModel.Email;
                            GlobalVariables.remember_token = mode.LoginModel.remember_token;
                            GlobalVariables.role = mode.LoginModel.role;
                            GlobalVariables.AppUserPassword = model.Password;
                            GlobalVariables.logintype = model.logintype;
                            GlobalVariables.AssetTransfer_AddReviewer = mode.LoginModel.AssetTransfer_AddReviewer;
                            GlobalVariables.AssetTransfer_Approver = mode.LoginModel.AssetTransfer_Approver;
                            GlobalVariables.AssetTransfer_Receiver = mode.LoginModel.AssetTransfer_Receiver;

                            GetWebSettings();
                            GetUserPermissions();
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

        }
        /*
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
                        GlobalVariables.AppUserId = mode.LoginModel.id;
                        GlobalVariables.AppUserName = mode.LoginModel.Name;
                        GlobalVariables.AppUserEmail = mode.LoginModel.Email;
                        GlobalVariables.remember_token = mode.LoginModel.remember_token;
                        GlobalVariables.role = mode.LoginModel.role;
                        GlobalVariables.AppUserPassword = model.Password;

                        GetWebSettings();
                        GetUserPermissions();
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid username or password.";
                        return View("Login");
                    }

                }
                else
                {
                    //ViewBag.ErrorMessage = "Invalid username or password.";
                    return View("Login");
                }
            }

        }
            */
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
        private void GetUserPermissions()
        {
            if (GlobalVariables.logintype == "stafflogin")
            {
                using (DAPPER_DATA_SERVICESMA dapperm = new DAPPER_DATA_SERVICESMA())
                {
                    try
                    {
                        GlobalVariables.GetUserPermissionsValue.Clear();
                        List<userpermissions> userpermissions = new List<userpermissions>();
                        dapperm.AddIntegerParameter("id", GlobalVariables.AppUserId);
                        userpermissions = dapperm.execute_stored_procedure<userpermissions>("dbo.UserPermissions");

                        GlobalVariables.GetUserPermissionsValue.AddRange(userpermissions);
                    }
                    catch (Exception ex)
                    {
                        //sysExceptions.logException(ex.Message, ex.StackTrace);
                    }
                }
            }
            else
            {
                using (DAPPER_DATA_SERVICE dappern = new DAPPER_DATA_SERVICE())
                {
                    try
                    {
                        GlobalVariables.GetUserPermissionsValue.Clear();
                        List<userpermissions> userpermissions = new List<userpermissions>();
                        dappern.AddIntegerParameter("id", GlobalVariables.AppUserId);
                        userpermissions = dappern.execute_stored_procedure<userpermissions>("dbo.UserPermissions");

                        GlobalVariables.GetUserPermissionsValue.AddRange(userpermissions);
                    }
                    catch (Exception ex)
                    {
                        //sysExceptions.logException(ex.Message, ex.StackTrace);
                    }
                }
            }
        }
        public ActionResult GetWebSettingsRefresh()
        {
            GetWebSettings();
            //GetUserPermissions();
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult zonelist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc", string searchTerm = "")
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
                dapper.AddUnicodeStringParameter("searchTerm", searchTerm, 200);

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
        public ActionResult zoneadd(int id = 0)
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
            try
            {
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

                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public ActionResult zonerestorenow(int id = 0)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.zonerestorenow");
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult branchlist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc", string searchTerm = "", int zone=0, int group=0, int city=0)
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
                dapper.AddUnicodeStringParameter("searchTerm", searchTerm, 200);
                dapper.AddIntegerParameter("zone", zone);
                dapper.AddIntegerParameter("group", group);
                dapper.AddIntegerParameter("city", city);

                var result = dapper.query_multiple("dbo.BranchListGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.TotalCount = result.Read<int>().SingleOrDefault();
                    Model.zonelist = result.Read<zoneModel>().ToList();
                    Model.grouplist = result.Read<groupModel>().ToList();
                    Model.citylist = result.Read<cityModel>().ToList();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;
            ViewBag.searchTerm= searchTerm;
            ViewBag.zone = zone;
            ViewBag.group = group;
            ViewBag.city = city;

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
                    Model.messageslist = result.Read<messagesModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult branchsave(branchModel item)
        {

            // Initialize the view model
            try
            {
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
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult notificationViewed(Guid Notificationid)
        {

            // Initialize the view model
            try
            {
                branchModel Test = new branchModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddGUIDParameter("Notificationid", Notificationid);
                    dapper.AddIntegerParameter("userid", GlobalVariables.AppUserId);

                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<branchModel>("dbo.notificationViewed").FirstOrDefault();
                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                    Model.zones = result.Read<zoneModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult staffuserlist(int page = 1, int pageSize = 10, string sortColumn = "Id", string sortOrder = "asc")
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
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
        public ActionResult staffuseradd(int id = 0)
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
            {
                dapper.AddIntegerParameter("id", id);
                var result = dapper.query_multiple("dbo.UserSingleGet");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.user = result.Read<userModel>().SingleOrDefault();
                    Model.rolelist = result.Read<roleModel>().ToList();
                    Model.zones = result.Read<zoneModel>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult userprofile(int id = 0)
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();
            if (GlobalVariables.logintype == "stafflogin")
            {
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
                {
                    dapper.AddIntegerParameter("id", id);
                    var result = dapper.query_multiple("dbo.UserSingleGet");

                    // Check if the result contains rows
                    if (result.hasRows())
                    {
                        Model.user = result.Read<userModel>().SingleOrDefault();
                        Model.rolelist = result.Read<roleModel>().ToList();
                        Model.zones = result.Read<zoneModel>().ToList();
                    }
                }
                // Return the view with the populated model
                return View(Model);
            }
            else
            {
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
                        Model.zones = result.Read<zoneModel>().ToList();
                    }
                }
                // Return the view with the populated model
                return View(Model);
            }
        }


        public ActionResult userActivate(int id = 0, string remember_token="")
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                dapper.AddUnicodeStringParameter("remember_token", remember_token,500);
                var result = dapper.query_multiple("dbo.userActivate");
                Model.user = result.Read<userModel>().SingleOrDefault();

                // Check if the result contains rows
                if (Model.user !=null)
                {
                    
                    if(Model.user.Active == 1)
                    {
                        ViewBag.usermessage = "User Already Activated!";
                        return View("usershowmessage");
                    }
                    else
                    {
                        return  RedirectToAction("userActiveChangePassword", "Assets", new { id = id , remember_token = remember_token }); 
                    }
                }
                else
                {
                    ViewBag.usermessage = "Invlid User contact to Admin";
                    return View("usershowmessage");
                }
            }
            // Return the view with the populated model
            //return View(Model);
        }


        public ActionResult staffuserActivate(int id = 0, string remember_token = "")
        {
            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
            {
                dapper.AddIntegerParameter("id", id);
                dapper.AddUnicodeStringParameter("remember_token", remember_token, 500);
                var result = dapper.query_multiple("dbo.userActivate");
                Model.user = result.Read<userModel>().SingleOrDefault();

                // Check if the result contains rows
                if (Model.user != null)
                {

                    if (Model.user.Active == 1)
                    {
                        ViewBag.usermessage = "User Already Activated!";
                        return View("usershowmessage");
                    }
                    else
                    {
                        return RedirectToAction("userActiveChangePassword", "Assets", new { id = id, remember_token = remember_token });
                    }
                }
                else
                {
                    ViewBag.usermessage = "Invlid User contact to Admin";
                    return View("usershowmessage");
                }
            }
            // Return the view with the populated model
            //return View(Model);
        }

        public ActionResult userActiveChangePassword(int id = 0, string remember_token="")
        {

            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                dapper.AddUnicodeStringParameter("remember_token", remember_token, 500);
                var result = dapper.query_multiple("dbo.userActivate");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.user = result.Read<userModel>().SingleOrDefault();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }


        public ActionResult userChangePassword(int id = 0, string remember_token = "")
        {
            /*
            // Initialize the view model
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", id);
                dapper.AddUnicodeStringParameter("remember_token", remember_token, 500);
                var result = dapper.query_multiple("dbo.userActivate");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.user = result.Read<userModel>().SingleOrDefault();
                }
            }
            // Return the view with the populated model
            return View(Model);*/
            return View();
        }

        public ActionResult usersavecchangepassword(int id = 0, string NewPassword = "")
        {
            try
            {
                // Initialize the view model
                UserViewModel Model = new UserViewModel();
                if (GlobalVariables.logintype == "stafflogin")
                {
                    // Create an instance of your Dapper data service
                    using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
                    {
                        dapper.AddIntegerParameter("id", id);
                        dapper.AddUnicodeStringParameter("NewPassword", NewPassword, 500);
                        var result = dapper.query_multiple("dbo.usersavecchangepassword");

                        if (result.hasRows())
                        {
                            Model.user = result.Read<userModel>().SingleOrDefault();

                            SMAAPIController smaApiController = new SMAAPIController();
                            var subject = GlobalVariables.GetWebSetting("User_Email_Subject", "").Replace("[Software Name]", GlobalVariables.GetWebSetting("company_name", ""));
                            var detail = GlobalVariables.GetWebSetting("User_Email_Detail", "");
                            detail = " Your user is " + Model.user.email + " / New password is " + NewPassword;
                            detail = detail + "<br> Use this like to login: <a href='" + Request.Url.Authority + "'>" + Request.Url.Authority + "</a>";
                            GlobalVariables.AppUserPassword = NewPassword;

                            smaApiController.SendEmail(Model.user.email, subject, detail, null);
                            return Json(new { success = true, message = "Password change Successfully - Check your email!" });
                        }
                    }
                }
                else
                {
                    // Create an instance of your Dapper data service
                    using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                    {
                        dapper.AddIntegerParameter("id", id);
                        dapper.AddUnicodeStringParameter("NewPassword", NewPassword, 500);
                        var result = dapper.query_multiple("dbo.usersavecchangepassword");

                        if (result.hasRows())
                        {
                            Model.user = result.Read<userModel>().SingleOrDefault();

                            SMAAPIController smaApiController = new SMAAPIController();
                            var subject = GlobalVariables.GetWebSetting("User_Email_Subject", "").Replace("[Software Name]", GlobalVariables.GetWebSetting("company_name", ""));
                            var detail = GlobalVariables.GetWebSetting("User_Email_Detail", "");
                            detail = " Your user is " + Model.user.email + " / New password is " + NewPassword;
                            detail = detail + "<br> Use this like to login: <a href='" + Request.Url.Authority + "'>" + Request.Url.Authority + "</a>";
                            GlobalVariables.AppUserPassword = NewPassword;

                            smaApiController.SendEmail(Model.user.email, subject, detail, null);
                            return Json(new { success = true, message = "Password change Successfully - Check your email!" });
                        }
                    }
                }
                // Return the view with the populated model
                //return View(Model);
                return Json(new { success = false, message = "Unable to Change Password" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult usersave(userModel item)
        {
            try
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
                    dapper.AddDateTimeParameter("DOB", item.DOB);
                    dapper.AddUnicodeStringParameter("zones", item.zones, 255);
                    dapper.AddIntegerParameter("AssetTransfer_AddReviewer", item.AssetTransfer_AddReviewer);
                    dapper.AddIntegerParameter("AssetTransfer_Approver", item.AssetTransfer_Approver);
                    dapper.AddIntegerParameter("AssetTransfer_Receiver", item.AssetTransfer_Receiver);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<userModel>("dbo.UserSave").FirstOrDefault();
                }
                // Return the view with the populated model
                if (item.id == 0)
                {
                    SMAAPIController smaApiController = new SMAAPIController();
                    var subject = GlobalVariables.GetWebSetting("User_Email_Subject", "").Replace("[Software Name]", GlobalVariables.GetWebSetting("company_name", ""));
                    var detail = GlobalVariables.GetWebSetting("User_Email_Detail", "");
                    detail = detail.Replace("[User Name]", item.name).Replace("[Software Name]", GlobalVariables.GetWebSetting("company_name", ""));
                    detail = detail.Replace("[User Active Link]", Request.Url.Authority+"/Assets/userActivate?id=" +item.id+"&remember_token="+item.remember_token);
                    smaApiController.SendEmail(item.email, subject, detail, null);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult staffusersave(userModel item)
        {
            try
            {
                userModel Test = new userModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
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
                    dapper.AddDateTimeParameter("DOB", item.DOB);
                    dapper.AddUnicodeStringParameter("zones", item.zones, 255);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<userModel>("dbo.UserSave").FirstOrDefault();
                }
                // Return the view with the populated model
                if (item.id == 0)
                {
                    SMAAPIController smaApiController = new SMAAPIController();
                    var subject = GlobalVariables.GetWebSetting("User_Email_Subject", "").Replace("[Software Name]", GlobalVariables.GetWebSetting("company_name", ""));
                    var detail = GlobalVariables.GetWebSetting("User_Email_Detail", "");
                    detail = detail.Replace("[User Name]", item.name).Replace("[Software Name]", GlobalVariables.GetWebSetting("company_name", ""));
                    detail = detail.Replace("[User Active Link]", Request.Url.Authority + "/Assets/staffuserActivate?id=" + item.id + "&remember_token=" + item.remember_token);
                    smaApiController.SendEmail(item.email, subject, detail, null);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult userpermission(int id = 0)
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

        public ActionResult staffuserpermission(int id = 0)
        {
            UserViewModel Model = new UserViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
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
                    dapper.AddIntegerParameter("UserIdby", GlobalVariables.AppUserId);

                    var result = dapper.query_multiple("dbo.UpdateUserPermissions");
                    //Test = dapper.execute_stored_procedure<userModel>("dbo.UserSave").FirstOrDefault();
                }
            }
            // Return the view with the populated model
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);



            //return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult staffUpdateUserPermissions(List<userpermissions> permissions)
        {

            userModel Test = new userModel();
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICESMA dapper = new DAPPER_DATA_SERVICESMA())
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
                    Model.zonecount = result.Read<int>().SingleOrDefault();
                    Model.branchcount = result.Read<int>().SingleOrDefault();
                    Model.groupcount = result.Read<int>().SingleOrDefault();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult groupsave(groupModel item)
        {

            // Initialize the view model
            try
            {
                groupModel Test = new groupModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("id", item.id);
                    dapper.AddUnicodeStringParameter("name", item.name, 255);
                    dapper.AddUnicodeStringParameter("person_name", item.person_name, 255);
                    dapper.AddUnicodeStringParameter("person_designation", item.person_designation, 255);
                    dapper.AddUnicodeStringParameter("person_email", item.person_email, 255);
                    dapper.AddUnicodeStringParameter("person_number", item.person_number, 255);
                    dapper.AddIntegerParameter("city_id", item.city_id);
                    dapper.AddUnicodeStringParameter("branches", item.branches, 255);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<groupModel>("dbo.GroupSave").FirstOrDefault();
                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                    Model.contactPERlist = result.Read<WebSetting>().ToList();
                    Model.contactSMAlist = result.Read<WebSetting>().ToList();
                }
            }
            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult reportslist(string fromdate = "", string todate = "", int zone_id = 0, int branch_id = 0)
        {

            // Initialize the view model
            reportslsitViewModel Model = new reportslsitViewModel();


            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zone_id", zone_id);
                dapper.AddIntegerParameter("branch_id", branch_id);
                dapper.AddStringParameter("fromdate", fromdate, 50);
                dapper.AddStringParameter("todate", todate, 50);
                var result = dapper.query_multiple("dbo.reportslist");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.zone = result.Read<zoneModel>().ToList();
                    Model.branch = result.Read<branchModel>().ToList();
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
        public ActionResult reportsscannedlist(string fromdate = "", string todate = "", int report_type = 0, int groupid = 0, int branchid = 0)
        {
            if (fromdate=="" ) { 
                fromdate = DateTime.Today.ToString("yyyy-MM-dd");
                todate = DateTime.Today.ToString("yyyy-MM-dd 23:59:59");
            }
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
        public ActionResult notification(int page = 1, int pageSize = 25, string sortColumn = "Id", string sortOrder = "desc", string datefrom = "", string dateto = "", int performedBy = 0, string notification_type = "")
        {
            if (datefrom == "") { datefrom = DateTime.Now.ToString("yyyy-MM-dd 00:00:00"); } else { datefrom = datefrom + " 00:00:00"; }
            if (dateto == "") { dateto = DateTime.Now.ToString("yyyy-MM-dd  23:59:59"); } else { dateto = dateto + " 23:59:59"; }
            if (notification_type == "All") { notification_type = ""; }
            // Initialize the view model
            notificationViewModel Model = new notificationViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                dapper.AddUnicodeStringParameter("datefrom", datefrom, 50);
                dapper.AddUnicodeStringParameter("dateto", dateto, 50);
                dapper.AddIntegerParameter("performed_by", performedBy);
                dapper.AddUnicodeStringParameter("name", notification_type, 50);
                dapper.AddIntegerParameter("userid", GlobalVariables.AppUserId);

                var result = dapper.query_multiple("dbo.notification");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.notificationlist = result.Read<notificationModel>().ToList();
                    Model.user = result.Read<userModel>().ToList();
                    Model.notificationTypelist = result.Read<notificationTypeModel>().ToList();
                }
            }

            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;
            Model.datefrom = datefrom.Substring(0,10);
            Model.dateto = dateto.Substring(0, 10);
            Model.performedBy = performedBy;
            Model.notification_type = notification_type;
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
        public ActionResult emailupdate(string GorB="", int itemid=0)
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
                    ViewBag.GorB = GorB;
                    ViewBag.itemid = itemid;
                }
            }
            return View(Model);
        }

        public ActionResult emailupdateSave(string GorB, int itemid, string email)
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

        public ActionResult emaildeleteSave(string GorB, int itemid, string email)
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
                Test = dapper.execute_stored_procedure<branchModel>("dbo.emaildeleteSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult emaildisableSave(string GorB, int itemid, string email)
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
                Test = dapper.execute_stored_procedure<branchModel>("dbo.emaildisableSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult emailenableSave(string GorB, int itemid, string email)
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
                Test = dapper.execute_stored_procedure<branchModel>("dbo.emailenableSave").FirstOrDefault();
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult assetslist(int page = 1, int pageSize = 25, string sortColumn = "Id", string sortOrder = "asc", string searchTerm = "", int zone=0, int branch=0, int group=0)
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
                dapper.AddUnicodeStringParameter("searchTerm", searchTerm, 200);
                dapper.AddIntegerParameter("zone", zone);
                dapper.AddIntegerParameter("branch", branch);
                dapper.AddIntegerParameter("group", group);

                var result = dapper.query_multiple("dbo.AssetsList");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.assetlist = result.Read<assetsModel>().ToList();
                    Model.assetcount = result.Read<int>().SingleOrDefault();
                    Model.currentbookvalue = result.Read<double>().SingleOrDefault();
                    Model.TotalCount = Model.assetcount;// result.Read<int>().SingleOrDefault();
                    Model.zonelist = result.Read<zoneModel>().ToList();
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.grouplist = result.Read<groupModel>().ToList();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;
            ViewBag.searchTerm = searchTerm;
            ViewBag.zone = zone;
            ViewBag.branch = branch;
            ViewBag.group = group;

            // Return the view with the populated model
            return View(Model);

        }
        public ActionResult assetstransferlist(int page = 1, int pageSize = 25, string sortColumn = "Id", string sortOrder = "asc",  int branch = 0, int AssetTransfer_AddReviewer=0, int AssetTransfer_Approver=0, int AssetTransfer_Receiver=0, int AssetTransfer_Deleted=0, int AssetTransfer_Completed=0)
        {
            /*0 added, 
            1 Added / waiting for reviewe(next 2),
            2 Reviewed / waiting approval(next 3),
            3 Approved / waiting for receiveing,
            4 Received,
            6 Cancel / Deleted,
            */

            // Initialize the view model
            assetstransferViewModel Model = new assetstransferViewModel();
            
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("PageNumber", page);
                dapper.AddIntegerParameter("PageSize", pageSize);
                dapper.AddUnicodeStringParameter("SortColumn", sortColumn, 50);
                dapper.AddUnicodeStringParameter("SortOrder", sortOrder, 40);
                dapper.AddIntegerParameter("branch", branch);
                dapper.AddIntegerParameter("AssetTransfer_AddReviewer", AssetTransfer_AddReviewer);
                dapper.AddIntegerParameter("AssetTransfer_Approver", AssetTransfer_Approver);
                dapper.AddIntegerParameter("AssetTransfer_Receiver", AssetTransfer_Receiver);
                dapper.AddIntegerParameter("AssetTransfer_Deleted", AssetTransfer_Deleted);
                dapper.AddIntegerParameter("AssetTransfer_Completed", AssetTransfer_Completed);
                dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);

                var result = dapper.query_multiple("dbo.assetstransferlist");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.assettransferlist = result.Read<assetstransferModel>().ToList();
                    Model.assetcount = result.Read<int>().SingleOrDefault();
                    Model.currentbookvalue = result.Read<double>().SingleOrDefault();
                    Model.TotalCount = Model.assetcount;// result.Read<int>().SingleOrDefault();
                    Model.branchlist = result.Read<branchModel>().ToList();
                }
            }
            // Set pagination details
            Model.PageSize = pageSize;
            Model.CurrentPage = page;
            Model.SortColumn = sortColumn;
            Model.SortOrder = sortOrder;
            
            ViewBag.branch = branch;
            ViewBag.AssetTransfer_AddReviewer = AssetTransfer_AddReviewer;
            ViewBag.AssetTransfer_Approver = AssetTransfer_Approver;
            ViewBag.AssetTransfer_Receiver = AssetTransfer_Receiver;
            ViewBag.AssetTransfer_Deleted = AssetTransfer_Deleted;
            ViewBag.AssetTransfer_Completed = AssetTransfer_Completed;
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


        public ActionResult assettransfer(string asset_id = "")
        {

            // Initialize the view model
            assetsViewModel Model = new assetsViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddUnicodeStringParameter("asset_id", asset_id, 200);
                var result = dapper.query_multiple("dbo.AssetSingleGetbyAsset_id");

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
            try
            {
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
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        public ActionResult assettransfersave(int AssetId,int frombranchcode, string fromlocation, int tobranchcode, string tolocation, string Description)
        {

            // Initialize the view model
            try
            {
                assetsModel Test = new assetsModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("AssetId", AssetId);
                    dapper.AddIntegerParameter("frombranchcode", frombranchcode);
                    dapper.AddUnicodeStringParameter("fromlocation", fromlocation, 255);
                    dapper.AddIntegerParameter("tobranchcode", tobranchcode);
                    dapper.AddUnicodeStringParameter("tolocation", tolocation, 255);
                    dapper.AddUnicodeStringParameter("Description", Description, 255);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);

                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<assetsModel>("dbo.assettransfersave").FirstOrDefault();
                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public ActionResult AddYear(int assetid, int addyear)
        {
            try 
            { 
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("assetid", assetid);
                    dapper.AddIntegerParameter("addyear", addyear);

                    //dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);

                    var result = dapper.query_multiple("dbo.AddYear");
                }
            }
            catch (Exception ex)
            {
                // Return the view with the populated model
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);

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
                return File(new UTF8Encoding().GetBytes(csv), "text/csv", "AssetExportToCSV.CSV");
            }
            catch (Exception ex)
            {
                // Handle exceptions as necessary
                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }


        public ActionResult BranchExportToCSV()
        {
            string storedProcedureName = "BranchListExport";
            string filename = "BranchExportToCSV.CSV";

            try
            {
                // Define the stored procedure name
                //string storedProcedureName = "AssetListExport";

                // Fetch the data using Dapper
                DataTable assetData = new DataTable();
                // = GetAssetData(storedProcedureName);
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var assetDataD = dapper.execute_stored_procedure<BranchModelExport>(storedProcedureName).ToList();
                    assetData = ConvertToDataTable(assetDataD);
                }

                // Convert the DataTable to CSV
                string csv = ConvertDataTableToCSV(assetData);

                // Return CSV file as download
                return File(new UTF8Encoding().GetBytes(csv), "text/csv", filename);
            }
            catch (Exception ex)
            {
                // Handle exceptions as necessary
                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }
        public ActionResult AssetsRollbackExportToCSV()
        {
            string storedProcedureName = "AssetsRollbackListExport";
            string filename = "AssetsRollbackExportToCSV.CSV";
            try
            {
                // Define the stored procedure name
                //string storedProcedureName = "AssetListExport";

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
                return File(new UTF8Encoding().GetBytes(csv), "text/csv", filename);
            }
            catch (Exception ex)
            {
                // Handle exceptions as necessary
                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }
        public ActionResult AssetsDeletedExportToCSV()
        {
            string storedProcedureName = "AssetsDeletedListExport";
            string filename = "AssetsDeletedExportToCSV.CSV";
            try
            {
                // Define the stored procedure name
                //string storedProcedureName = "AssetListExport";

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
                return File(new UTF8Encoding().GetBytes(csv), "text/csv", filename);
            }
            catch (Exception ex)
            {
                // Handle exceptions as necessary
                return new HttpStatusCodeResult(500, "Internal server error: " + ex.Message);
            }
        }
        public ActionResult BranchesDeletedExportToCSV()
        {
            string storedProcedureName = "BranchesDeletedListExport";
            string filename = "BranchesDeletedExportToCSV.CSV";
            try
            {
                // Define the stored procedure name
                //string storedProcedureName = "AssetListExport";

                // Fetch the data using Dapper
                DataTable assetData = new DataTable();
                // = GetAssetData(storedProcedureName);
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var assetDataD = dapper.execute_stored_procedure<BranchModelExport>(storedProcedureName).ToList();
                    assetData = ConvertToDataTable(assetDataD);
                }

                // Convert the DataTable to CSV
                string csv = ConvertDataTableToCSV(assetData);

                // Return CSV file as download
                return File(new UTF8Encoding().GetBytes(csv), "text/csv", filename);
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
        /*
        public ActionResult reportdetailed()
        {
            return View();
        }
        */
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

            // Read the CSV file
            var tagIdsFromCSV = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                int lineNumber = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (lineNumber == 0)
                    {
                        // Skip header row
                        lineNumber++;
                        continue;
                    }

                    var values = line.Split(','); // Assuming CSV columns are comma-separated
                    var tagId = values[0]; // Assuming tag_id is in the 3rd column (index 2)
                    tagIdsFromCSV.Add(tagId);
                    lineNumber++;
                }
            }

            // Remove duplicates
            tagIdsFromCSV = tagIdsFromCSV.Distinct().ToList();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                string tagIdsInClause = string.Join(",", tagIdsFromCSV.Select(t => $"'{t}'"));

                // Check if all tag_ids exist in the database
                string query = $@"
            SELECT tag_id 
            FROM (VALUES {string.Join(", ", tagIdsFromCSV.Select(t => $"('{t}')"))}) AS CSVTags(tag_id)
            EXCEPT
            SELECT tag_id FROM assets";

                var missingTagIds = dapper.ExecuteQuery<string>(query).ToList();

                if (missingTagIds.Any())
                {
                    return Json(new { success = false, message = "The following tag_ids are missing in the database: " + string.Join(", ", missingTagIds) });
                }

                assetsModel Test = new assetsModel();

                // If all tag_ids exist, proceed with update/insert process
                using (var reader = new StreamReader(filePath))
                {
                    int lineNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (lineNumber == 0)
                        {
                            // Skip header row
                            lineNumber++;
                            continue;
                        }

                        var values = line.Split(','); // Assuming CSV columns are comma-separated
                        DAPPER_DATA_SERVICE dapper2 = new DAPPER_DATA_SERVICE();
                        branchViewModel Model = new branchViewModel();
                        assetsViewModel Modelasset = new assetsViewModel();

                        dapper2.AddUnicodeStringParameter("code", values[24], 255);
                        dapper2.AddUnicodeStringParameter("tag_id", values[0], 255);
                        var result = dapper2.query_multiple("dbo.BranchCodeGet");

                        if (result.hasRows())
                        {
                            Model.branch = result.Read<branchModel>().SingleOrDefault();
                            Modelasset.asset = result.Read<assetsModel>().SingleOrDefault();
                        }

                        dapper.AddIntegerParameter("id", Modelasset.asset.id);
                        dapper.AddIntegerParameter("branch_id", Model.branch.id);
                        dapper.AddUnicodeStringParameter("tag_id", values[0], 255);
                        dapper.AddUnicodeStringParameter("asset_id", values[1], 255);
                        dapper.AddUnicodeStringParameter("asset_name", values[2], 255);
                        dapper.AddIntegerParameter("invoice_cost", Convert.ToInt32(values[3]));
                        dapper.AddIntegerParameter("total_cost", Convert.ToInt32(values[4]));
                        dapper.AddIntegerParameter("salvage_value", Convert.ToInt32(values[5]));
                        dapper.AddDateTimeParameter("date_of_purchase", Convert.ToDateTime(values[6]));
                        dapper.AddUnicodeStringParameter("vendor_id", values[7], 255);
                        dapper.AddUnicodeStringParameter("vendor", values[8], 255);
                        dapper.AddUnicodeStringParameter("part_warranty", values[9], 255);
                        dapper.AddUnicodeStringParameter("service_warranty", values[10], 255);
                        dapper.AddUnicodeStringParameter("serial_no", values[11], 255);
                        dapper.AddUnicodeStringParameter("generic_name", values[12], 255);
                        dapper.AddUnicodeStringParameter("custom_field", values[13], 255);
                        dapper.AddIntegerParameter("life_in_year", Convert.ToInt32(values[14]));
                        dapper.AddIntegerParameter("dep_type", Convert.ToInt32(values[15]));
                        dapper.AddIntegerParameter("net_value", Convert.ToInt32(values[17]));
                        dapper.AddDecimalParameter("remaining_years", Convert.ToDecimal(values[18]));
                        dapper.AddUnicodeStringParameter("location", values[23], 255);
                        dapper.AddUnicodeStringParameter("AssetOld_id", values[25], 255);
                        dapper.AddIntegerParameter("Tagcount", Convert.ToInt32(values[26]));
                        dapper.AddIntegerParameter("TagcountRemain", Convert.ToInt32(values[27]));

                        dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);


                        Test = dapper.execute_stored_procedure<assetsModel>("dbo.AssetSave_BulkUpdate").FirstOrDefault();
                        lineNumber++;
                    }
                }
            }

            return Json(new { success = true, data = new { message = "done" } });
        }

        public ActionResult assetbulkdelete()
        {
            return View();
        }
        public ActionResult DeleteAssetsFromExcel(HttpPostedFileBase scannerFile, string deleteType, string deleteAS)
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

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                // Load the CSV file
                var tagIdsFromCsv = new List<string>();
                using (var reader = new StreamReader(filePath))
                {
                    bool isHeader = true;
                    string tagId;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (isHeader)
                        {
                            isHeader = false; // Skip header
                            continue;
                        }
                        var values = line.Split(','); // Assuming CSV is comma-separated
                        tagId = "";
                        if (deleteAS == "tag_id")
                            {tagId = values[0];
                        }else if(deleteAS == "asset_id")
                            {tagId = values[1];
                        }
                        tagIdsFromCsv.Add(tagId);
                    }
                }

                // Remove duplicates
                tagIdsFromCsv = tagIdsFromCsv.Distinct().ToList();

                string tagIdsInClause = string.Join(",", tagIdsFromCsv.Select(t => $"'{t}'"));

                // Check if all tag_ids exist in the database
                string query = "";
                if (deleteAS == "tag_id")
                    {query = $@"
                        SELECT tag_id FROM (VALUES {string.Join(", ", tagIdsFromCsv.Select(t => $"('{t}')"))}) AS CsvTags(tag_id)
                        EXCEPT SELECT tag_id FROM assets";
                }else if (deleteAS == "asset_id")
                    {query = $@"
                        SELECT asset_id FROM (VALUES {string.Join(", ", tagIdsFromCsv.Select(t => $"('{t}')"))}) AS CsvTags(asset_id)
                        EXCEPT SELECT asset_id FROM assets";
                }

                var missingTagIds = dapper.ExecuteQuery<string>(query).ToList();

                if (missingTagIds.Any())
                {
                    // If any tag_id is missing, log the issue and stop the process
                    Console.WriteLine("The following tag_ids/AssetID are missing in the database: " + string.Join(", ", missingTagIds));
                    return Json(new { success = false, message = "The following tag_ids/AssetID are missing in the database: " + string.Join(", ", missingTagIds) });
                }

                assetsModel Test = new assetsModel();

                // If all tag_ids exist, proceed with update/insert process
                using (var reader = new StreamReader(filePath))
                {
                    bool isHeader = true;
                    string tagId = "";
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (isHeader)
                        {
                            isHeader = false; // Skip header
                            continue;
                        }
                        var values = line.Split(',');
                        if (deleteAS == "tag_id")
                            {tagId = values[0];
                        }else if (deleteAS == "asset_id")
                            {tagId = values[1];
                        }

                        /*DAPPER_DATA_SERVICE dapper2 = new DAPPER_DATA_SERVICE();
                        branchViewModel Model = new branchViewModel();
                        assetsViewModel Modelasset = new assetsViewModel();
                        dapper2.AddUnicodeStringParameter("code", values[1], 255); // Assuming code is in column 2
                        dapper2.AddUnicodeStringParameter("ID", tagId, 255); // Assuming tag_id is in column 3
                        var result = dapper2.query_multiple("dbo.BranchCodeGet");
                        if (result.hasRows())
                        {
                            Model.branch = result.Read<branchModel>().SingleOrDefault();
                            Modelasset.asset = result.Read<assetsModel>().SingleOrDefault();
                        }*/

                        dapper.AddUnicodeStringParameter("TTid", tagId,500);
                        dapper.AddUnicodeStringParameter("deleteAS", deleteAS,50);
                        dapper.AddUnicodeStringParameter("deleteType", deleteType, 50);
                        dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                        Test = dapper.execute_stored_procedure<assetsModel>("dbo.AssetBulkDelete").FirstOrDefault();
                    }
                }
            }

            return Json(new { success = true, data = new { message = "done" } });
        }
        public JsonResult GetBranchbyZone(int zoneId, string currentSortColumn ="code", string currentSortOrder="asc", string searchTerm="")
        {
            branchViewModel Model = new branchViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zoneid", zoneId);
                dapper.AddUnicodeStringParameter("currentSortColumn", currentSortColumn,255);
                dapper.AddUnicodeStringParameter("currentSortOrder", currentSortOrder,255);
                dapper.AddUnicodeStringParameter("searchTerm", searchTerm,255);
                Model.branchlist = dapper.execute_stored_procedure<branchModel>("dbo.GetBranchbyZone").ToList();
            }
                return Json(Model.branchlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffbyZone(int zoneId, string currentSortColumn = "id", string currentSortOrder = "asc", string searchTerm = "")
        {
            UserViewModel Model = new UserViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zoneid", zoneId);
                dapper.AddUnicodeStringParameter("currentSortColumn", currentSortColumn, 255);
                dapper.AddUnicodeStringParameter("currentSortOrder", currentSortOrder, 255);
                dapper.AddUnicodeStringParameter("searchTerm", searchTerm, 255);
                Model.userlist = dapper.execute_stored_procedure<userModel>("dbo.GetStaffbyZone").ToList();
            }
            return Json(Model.userlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManagerforZone(int Id)
        {
            UserViewModel Model = new UserViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", Id);
                Model.user = dapper.execute_stored_procedure<userModel>("dbo.GetManagerforZone").FirstOrDefault();
            }
            return Json(Model.user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetDetail(int Id)
        {
            assetsViewModel Model = new assetsViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("id", Id);
                Model.asset = dapper.execute_stored_procedure<assetsModel>("dbo.GetAssetDetail").FirstOrDefault();
            }
            return Json(Model.asset, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchbyGroup(int GroupId, string currentSortColumn = "code", string currentSortOrder = "asc")
        {
            branchViewModel Model = new branchViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("GroupId", GroupId);
                dapper.AddUnicodeStringParameter("currentSortColumn", currentSortColumn, 255);
                dapper.AddUnicodeStringParameter("currentSortOrder", currentSortOrder, 255);
                Model.branchlist = dapper.execute_stored_procedure<branchModel>("dbo.GetBranchbyGroup").ToList();
            }
            return Json(Model.branchlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssetTransferGet(int Id)
        {
            assetstransferViewModel Model = new assetstransferViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("Id", Id);
                Model.assettransfer = dapper.execute_stored_procedure<assetstransferModel>("dbo.AssetTransferGet").SingleOrDefault();
            }
            return Json(Model.assettransfer, JsonRequestBehavior.AllowGet);
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
                    Model.branchlist = result.Read<branchModel>().ToList();
                    Model.grouplist= result.Read<groupModel>().ToList();
                    Model.zonelist = result.Read<zoneModel>().ToList();
                    Model.citylist = result.Read<cityModel>().ToList();

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
                dapper.AddIntegerParameter("branch_id", item.branch_id);
                dapper.AddIntegerParameter("group_id", item.group_id);
                dapper.AddIntegerParameter("zone_id", item.zone_id);
                dapper.AddIntegerParameter("city_id", item.city_id);
                dapper.AddUnicodeStringParameter("displaycolor", item.displaycolor, 50);

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


        public ActionResult settingsave(string company_name, string company_address, string company_city, string company_currency, string primary_color)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddUnicodeStringParameter("company_name", company_name, 255);
                dapper.AddUnicodeStringParameter("company_address", company_address, 255);
                dapper.AddUnicodeStringParameter("company_city", company_city, 255);
                dapper.AddUnicodeStringParameter("company_currency", company_currency, 255);
                dapper.AddUnicodeStringParameter("primary_color", primary_color, 255);

                //dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);

                //var result = dapper.query_multiple("dbo.SettingSave");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        

        public ActionResult settingsavesub(string TagName, string TagValue)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddUnicodeStringParameter("TagName", TagName, 255);
                dapper.AddUnicodeStringParameter("TagValue", TagValue, 255);
                dapper.AddIntegerParameter("By", GlobalVariables.AppUserId);

                var result = dapper.query_multiple("dbo.SettingSaveSub");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        
        public ActionResult DeleteSettingTag(string TagName)
        {
            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddUnicodeStringParameter("TagName", TagName, 255);
                dapper.AddIntegerParameter("By", GlobalVariables.AppUserId);

                var result = dapper.query_multiple("dbo.DeleteSettingTag");
            }
            // Return the view with the populated model
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GoogleMapView(int zoneId=1)
        {

            branchViewModel Model = new branchViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("zoneid", zoneId);
                Model.branchModelforgoogle = dapper.execute_stored_procedure<branchModelforgoogle>("dbo.GetBranchbyGoogleMapId").ToList();
            }

            ViewBag.Branches = Model.branchModelforgoogle; // branches.Select(b => new { b.Latitude, b.Longitude }).ToList();
            return View();
        }


        public ActionResult DownloadSampleCsv(string fileName, string path)
        {
            var filePath = Server.MapPath(path+ fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            //string fileName = "sample-branch-file.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public ActionResult groupreportsummary(int group_id = 1, string datefrom="2024-01-01", string dateto="2024-10-01", int calculation=0)
        {
            if (datefrom == "") { datefrom = DateTime.Now.ToString("yyyy-MM-dd"); }
            if (dateto == "") { dateto = DateTime.Now.ToString("yyyy-MM-dd"); }
            // Initialize the view model
            groupreportsummaryViewModel Model = new groupreportsummaryViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("group_id", group_id);
                dapper.AddStringParameter("datefrom", datefrom,50);
                dapper.AddStringParameter("dateto", dateto,50);
                dapper.AddIntegerParameter("calculation", calculation);
                var result = dapper.query_multiple("dbo.groupreportsummary");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.groupreportsummarylist = result.Read<groupreportsummaryModel>().ToList();
                    Model.group = result.Read<groupModel>().SingleOrDefault();
                    Model.SetModel();
                    Model.datefrom = datefrom;
                    Model.dateto = dateto;
                    
                }
            }
            // Return the view with the populated model
            return View(Model);

        }

        public ActionResult groupreportdetail(int group_id = 1, string datefrom = "2024-01-01", string dateto = "2024-10-01", int calculation = 0)
        {
            if (datefrom == "") { datefrom = DateTime.Now.ToString("yyyy-MM-dd"); }
            if (dateto == "") { dateto = DateTime.Now.ToString("yyyy-MM-dd"); }
            // Initialize the view model
            groupreportdetailViewModel Model = new groupreportdetailViewModel();

            // Create an instance of your Dapper data service
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                dapper.AddIntegerParameter("group_id", group_id);
                dapper.AddStringParameter("datefrom", datefrom, 50);
                dapper.AddStringParameter("dateto", dateto, 50);
                dapper.AddIntegerParameter("calculation", calculation);
                var result = dapper.query_multiple("dbo.groupreportdetail");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.groupreportdetaillist = result.Read<groupreportdetailModel>().ToList();
                    Model.groupreportdetaiunllist = result.Read<groupreportdetailModel>().ToList();
                    Model.group = result.Read<groupModel>().SingleOrDefault();
                    Model.SetModel();
                    Model.datefrom = datefrom;
                    Model.dateto = dateto;

                }
            }
            // Return the view with the populated model
            return View(Model);

        }

        public class GeocodingService
        {
            private readonly string _googleApiKey = "AIzaSyCY2zdxcW1OrcOLRfxzej9Q9j6pepQG-6M"; // Replace with your API Key

            public async Task<(double Latitude, double Longitude)> GetCoordinatesFromAddressAsync(string address)
            {
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_googleApiKey}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(result);

                        if (json["status"].ToString() == "OK")
                        {
                            var location = json["results"][0]["geometry"]["location"];
                            double latitude = (double)location["lat"];
                            double longitude = (double)location["lng"];
                            return (latitude, longitude);
                        }
                        else
                        {
                            throw new Exception("Unable to get coordinates: " + json["status"]);
                        }
                    }
                    else
                    {
                        throw new Exception("Request failed with status code: " + response.StatusCode);
                    }
                }
            }
        }
        public async Task<ActionResult> GetBranchCoordinates(string branchAddress)
        {
            GeocodingService geocodingService = new GeocodingService();
            try
            {
                var (latitude, longitude) = await geocodingService.GetCoordinatesFromAddressAsync(branchAddress);
                // Use the latitude and longitude as needed, e.g., pass them to the view or save in the database
                ViewBag.Latitude = latitude;
                ViewBag.Longitude = longitude;
                return Json(new { success = true, message = latitude+", "+ longitude });
            }
            catch (Exception ex)
            {
                //ViewBag.Error = ex.Message;
                return Json(new { success = false, message = "Unable to find the address" });
            }
        }

        public async Task<string> GetBranchCoordinates2(string branchAddress)
        {
            GeocodingService geocodingService = new GeocodingService();
            try
            {
                var (latitude, longitude) = await geocodingService.GetCoordinatesFromAddressAsync(branchAddress);
                // Use the latitude and longitude as needed, e.g., pass them to the view or save in the database
                ViewBag.Latitude = latitude;
                ViewBag.Longitude = longitude;
                return  latitude + ", " + longitude;
            }
            catch (Exception ex)
            {
                //ViewBag.Error = ex.Message;
                return "";
            }
        }


        [HttpPost]
        public JsonResult UploadCompanyDetails()
        {

            try
            {
                var companyLogo = Request.Files["company_logo"];  // Access file input

                if (companyLogo != null && companyLogo.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/assets/images/company/");
                    string fileName = Path.GetFileName(companyLogo.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Save the logo file
                    string fullPath = Path.Combine(folderPath, fileName);
                    companyLogo.SaveAs(fullPath);

                    // Handle other form data
                    string companyName = Request.Form["company_name"];
                    string companyAddress = Request.Form["company_address"];
                    string email = Request.Form["contact_mobile"];
                    string city = Request.Form["company_city"];
                    // Process or save these details to the database as needed

                    using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                    {
                        string updateQuery = @"UPDATE web_settings SET value='assets/images/company/" + fileName + "' where tag='company_logo'";
                        dapper.execute(updateQuery);
                        GetWebSettings();
                    }
                    return Json(new { success = true, message = "Company Logo saved successfully." });
                }
                return Json(new { success = false, message = "No file uploaded." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UploadUserDetails()
        {

            try
            {
                var userPicture = Request.Files["user_picture"];  // Access file input

                if (userPicture != null && userPicture.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/assets/images/users/");
                    string fileName = Path.GetFileName(userPicture.FileName);
                    string userid = Request.Form["userid"];
                    string fileext = Path.GetExtension(userPicture.FileName);
                    string useridtmp = "00000" + userid;
                    fileName = useridtmp.Substring(useridtmp.Length - 5)+fileext;

                    // Ensure the folder exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Save the logo file
                    string fullPath = Path.Combine(folderPath, fileName);
                    userPicture.SaveAs(fullPath);

                    // Handle other form data
                    
                    // Process or save these details to the database as needed

                    using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                    {
                        string updateQuery = @"UPDATE users SET image='assets/images/users/" + fileName + "' where id="+userid;
                        dapper.execute(updateQuery);
                    }
                    return Json(new { success = true, message = "User Picture saved successfully." });
                }
                return Json(new { success = false, message = "No file uploaded." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ReloadBranches()
        {
            // Fetch updated branches from the database (replace this with your actual database query)
            //var branches = YourDatabaseService.GetBranches(); // Example: Use your data access method

            //return Json(branches, JsonRequestBehavior.AllowGet);
            branchViewModel Model = new branchViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {

                Model.branchlist = dapper.execute_stored_procedure<branchModel>("dbo.ReloadBranches").ToList();
            }

            return Json(Model.branchlist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReloadGroups()
        {
            // Fetch updated branches from the database (replace this with your actual database query)
            //var branches = YourDatabaseService.GetBranches(); // Example: Use your data access method

            //return Json(branches, JsonRequestBehavior.AllowGet);
            groupViewModel Model = new groupViewModel();

            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {

                Model.grouplist = dapper.execute_stored_procedure<groupModel>("dbo.ReloadGroups").ToList();
            }

            return Json(Model.grouplist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssetTransferSaveReview(int Id, string ReviewRemarks)
        {

            // Initialize the view model
            try
            {
                assetstransferModel Test = new assetstransferModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("Id", Id);
                    dapper.AddUnicodeStringParameter("ReviewRemarks", ReviewRemarks, 2000);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<assetstransferModel>("dbo.AssetTransferSaveReview").FirstOrDefault();
                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult AssetTransferSaveApprove(int Id, string ApproveRemarks)
        {

            // Initialize the view model
            try
            {
                assetstransferModel Test = new assetstransferModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("Id", Id);
                    dapper.AddUnicodeStringParameter("ApproveRemarks", ApproveRemarks, 2000);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<assetstransferModel>("dbo.AssetTransferSaveApprove").FirstOrDefault();
                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult AssetTransferSaveReceive(int Id, string ReceiveRemarks)
        {

            // Initialize the view model
            try
            {
                assetstransferModel Test = new assetstransferModel();
                // Create an instance of your Dapper data service
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("Id", Id);
                    dapper.AddUnicodeStringParameter("ReceiveRemarks", ReceiveRemarks, 2000);
                    dapper.AddIntegerParameter("UserId", GlobalVariables.AppUserId);
                    //var result = dapper.query_multiple("dbo.ZoneSave");
                    Test = dapper.execute_stored_procedure<assetstransferModel>("dbo.AssetTransferSaveReceive").FirstOrDefault();
                }
                // Return the view with the populated model
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}