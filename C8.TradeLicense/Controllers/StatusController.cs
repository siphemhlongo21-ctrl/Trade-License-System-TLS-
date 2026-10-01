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

namespace C8.TradeLicense.Controllers
{
    public class StatusController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public StatusController()
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

        // GET: /Status/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Index()
        {
            var status = db.Status.Include(s => s.CreatedByUser).Include(s => s.ModifiedByUser).Include(s => s.StatusType)
                .Where(s => s.IsActive && s.IsDeleted == false);
            ViewBag.StatusCount = status.Count();
            return View(status.ToList().OrderBy(s => s.StatusType.StatusTypeName));
        }

        // GET: /Status/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        // GET: /Status/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create()
        {
            // User test = new User();
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.StatusTypeId = new SelectList(db.StatusTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false), "StatusTypeId", "StatusTypeName");
            return View();
        }

        // POST: /Status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "StatusId, StatusTypeID,StatusName,StatusDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,StatusKey")] Status status)
        {
            if (ModelState.IsValid)
            {

                var statusName =
             db.Status.Where(s => s.StatusName == status.StatusName).Select(
                 s => s.StatusName).FirstOrDefault();
                if (statusName == null)
                {
                    // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                    IdentityManager.CurrentUser(User);
                    status.IsActive = true;
                    status.IsDeleted = false;
                    status.IsLocked = false;
                    db.Status.Add(status);
                    db.SaveChanges();
                    TempData["Success"] = "Status saved Successfully!";
                }
                else
                {
                    TempData["Error"] = "Status Exists.";
                }


            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";

            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", status.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", status.ModifiedByUserId);
            ViewBag.StatusTypeId = new SelectList(db.StatusTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false), "StatusTypeId", "StatusTypeName");

            return View(status);
        }

        // GET: /Status/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", status.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", status.ModifiedByUserId);
            ViewBag.StatusTypeId = new SelectList(db.StatusTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false), "StatusTypeId", "StatusTypeName", status.StatusTypeId);

            return View(status);
        }

        // POST: /Status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "StatusId, StatusTypeID,StatusName,StatusDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,StatusKey")] Status status)
        {
            if (ModelState.IsValid)
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                IdentityManager.CurrentUser(User);
                status.IsActive = true;
                status.IsDeleted = false;
                status.IsLocked = false;

                var local = db.Set<Status>()
                .Local.FirstOrDefault(l => l.StatusId == status.StatusId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Info"] = "Status Updated Successfully!";
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", status.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", status.ModifiedByUserId);
            ViewBag.StatusTypeId = new SelectList(db.StatusTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false), "StatusTypeId", "StatusTypeName", status.StatusTypeId);

            return View(status);
        }

        // GET: /Status/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Status status = db.Status.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        // POST: /Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
            IdentityManager.CurrentUser(User);
            Status status = db.Status.Find(id);
            status.IsDeleted = true;
            db.Entry(status).State = EntityState.Modified;
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
