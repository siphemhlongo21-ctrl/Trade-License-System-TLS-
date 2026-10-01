using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using C8.TradeLicense.Models;


namespace C8.TradeLicense
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes( RouteTable.Routes );
            BundleConfig.RegisterBundles( BundleTable.Bundles );
        }
        protected void Application_Error()
        {

            HttpContext httpContext = HttpContext.Current;
            Exception exception = Server.GetLastError();
            if (!exception.Message.Contains("The provided anti-forgery token was meant for a different claims-based user than the current user."))
            {
                httpContext.Response.Redirect("~/ErrorPage/Error");
            }


            if (exception.Message.Contains("The provided anti-forgery token was meant for a different claims-based user than the current user."))
            {

                httpContext.Response.Redirect("~/ErrorPage/Loginerror");
            }
            else if (exception.Message.Contains("The required anti-forgery form field \"__RequestVerificationToken\" is not present."))
            {
                httpContext.Response.Redirect("~/Home/Index");
            }
            else
            {
                var t = exception.Message;
                httpContext.Response.Redirect("~/ErrorPage/Error");
            }






        }
    }
}
