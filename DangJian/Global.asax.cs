﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DangJian
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
           // Exception exception = Server.GetLastError();
           // Response.Clear();
           // HttpException httpException = exception as HttpException;
           // RouteData routeData = new RouteData();
           // routeData.Values.Add("controller", "Account");
           // routeData.Values.Add("action", "Index");
            
           // // Clear the error on server.
           // Server.ClearError();
           // // Call target Controller and pass the routeData.
           // IController errorController = new DangJian.Controllers.AccountController();
           // errorController.Execute(new RequestContext(
           //new HttpContextWrapper(Context), routeData));
        }
    }
}