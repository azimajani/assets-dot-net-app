using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMAT_Inventory.Models;
using SMAT_Inventory.Dapper;

namespace SMAT_Inventory.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Dashboard()
        {
            DashBoardViewModel Model = new DashBoardViewModel();
            using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
            {
                var result = dapper.query_multiple("dbo.DashBoard");

                // Check if the result contains rows
                if (result.hasRows())
                {
                    Model.zonecount = result.Read<int>().SingleOrDefault();
                    Model.branchcount = result.Read<int>().SingleOrDefault();
                    Model.assetscount = result.Read<int>().SingleOrDefault();
                    Model.DMBranch = result.Read<DMBranchModel>().ToList();
                    Model.DMAsset = result.Read<DMAssetModel>().ToList();
                    Model.message = result.Read<messagesModel>().ToList();
                }
            }
                return View(Model);
        }
    }
}