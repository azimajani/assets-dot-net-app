﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
//using System.Web.UI.WebControls;

namespace SMAT_Inventory
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );
        }
    }
}