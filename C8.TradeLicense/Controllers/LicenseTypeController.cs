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
    public class LicenseTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public LicenseTypeController()
        {
            // JK.20140906a - Instantiate the IdentityManager in the contructor and pass the DbContext.
            IdentityManager = new IdentityManager(db);
        }
        /// <summary>
        /// JK.20140906a - Create a Identity Manager property to be used in any function. Instantiate in the contructor.
        /// Gets or sets the identity manager.
        /// </summary>
        /// <value>
        /// The identity manager.
        /// </value>
        public IdentityManager IdentityManager { get; set; }

        /// <summary>
        /// Gets or sets the user manager.
        /// JK.20140903a - Implement for getting the user details.
        /// Remember to include the using "Microsoft.AspNet.Identity"
        /// </summary>
        /// <value>
        /// The user manager.
        /// </value>
        protected UserManager<ApplicationUser> UserManager { get; set; }

        // GET: /LicenseType/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Index()
        {
            //var licensetypes = db.LicenseTypes.Include(l => l.CreatedByUser).Include(l => l.ModifiedByUser); 
            var licensetypes = db.LicenseTypes.Where(c => c.IsActive == true && c.IsDeleted == false);
            ViewBag.LicenseTypeCount = licensetypes.Count();
            return View(licensetypes.ToList());
        }

        // GET: /LicenseType/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var documentCheck = (from d in db.DocumentCheckLists where d.LicenseType.LicenseTypeId == id select d).ToList();

            if (null != documentCheck)
            {
                ViewData["DocumentCheckData"] = documentCheck;
            }

            LicenseType licensetype = db.LicenseTypes.Find(id);
            if (licensetype == null)
            {
                return HttpNotFound();
            }
            return View(licensetype);
        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var documentCheck = db.DocumentCheckLists.Where(c => c.LicenseTypeId == id); //(from d in db.DocumentCheckLists where d.LicenseType.LicenseTypeId == id select d);
            //DocumentCheckList documentCheck = db.DocumentCheckLists.Find(id);
            if (documentCheck == null)
            {
                return HttpNotFound();
            }
            return View(documentCheck);
        }
        // GET: /LicenseType/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create()
        {
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /LicenseType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "LicenseTypeId,LicenseTypeName,LicenseTypeDescription,IsActive,IsDeleted,IsLocked,LicenseTypeKey,Amount")] LicenseType licensetype)
        {
            if (ModelState.IsValid)
            {
                var licenseTypeName =
                   db.LicenseTypes.Where(l => l.LicenseTypeName == licensetype.LicenseTypeName).Select(
                       l => l.LicenseTypeName).FirstOrDefault();
                if (licenseTypeName == null)
                {
                    IdentityManager.CurrentUser(User);
                    licensetype.IsActive = true;
                    licensetype.IsDeleted = false;
                    licensetype.IsLocked = false;
                    db.LicenseTypes.Add(licensetype);
                    db.SaveChanges();
                    TempData["Success"] = "License Type Saved.";
                }
                else
                {
                    TempData["Error"] = "License Type Exists.";
                }
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", licensetype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", licensetype.ModifiedByUserId);
            return View(licensetype);
        }

        // GET: /LicenseType/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseType licensetype = db.LicenseTypes.Find(id);
            if (licensetype == null)
            {
                return HttpNotFound();
            }
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", licensetype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", licensetype.ModifiedByUserId);
            return View(licensetype);
        }

        // POST: /LicenseType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "LicenseTypeId,LicenseTypeName,LicenseTypeDescription,IsActive,IsDeleted,IsLocked,LicenseTypeKey,Amount")] LicenseType licensetype)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                licensetype.IsActive = true;
                licensetype.IsDeleted = false;
                licensetype.IsLocked = false;

                var local = db.Set<LicenseType>()
                .Local.FirstOrDefault(l => l.LicenseTypeId == licensetype.LicenseTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(licensetype).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Info"] = "License Type Updated.";
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", licensetype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", licensetype.ModifiedByUserId);
            return View(licensetype);
        }

        // GET: /LicenseType/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseType licensetype = db.LicenseTypes.Find(id);
            if (licensetype == null)
            {
                return HttpNotFound();
            }
            return View(licensetype);
        }

        // POST: /LicenseType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            LicenseType licensetype = db.LicenseTypes.Find(id);
            licensetype.IsActive = false;
            licensetype.IsDeleted = true;

            licensetype.CreatedDateTime = DateTime.Now;
            db.Entry(licensetype).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
