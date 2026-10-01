using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace C8.TradeLicense.Controllers
{
    public class LicenseDocumentController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        //
        // GET: /LicenseDocument/
        public ActionResult Index()
        {
            using (var db = new TradeLicenseDbContext())
            {
                var model = new LicenseDocuments 
                { 
                    LicenseTypeDetails = db.LicenseTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false).ToList(),
                    DocumentTypeDetails = db.DocumentTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false).ToList(),                  

                };

                return View(model);
            }
        }
    }
}