using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradingLicences.Controllers
{
    public class ErrorPageController : Controller
    {
        // GET: ErrorPage
        public ActionResult Error()
        {
            
            return View();
        }

        public ActionResult Loginerror()
        {

            return View();
        }
    }
}