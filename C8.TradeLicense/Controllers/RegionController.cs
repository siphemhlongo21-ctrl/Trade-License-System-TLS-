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

namespace C8.TradeLicense.Controllers
{
    public class RegionController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public RegionController()
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
        // GET: /Region/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk"+"," + "System Admin")]
        public ActionResult Index()
        {
            var regions = db.Regions.Include(r => r.CreatedByUser).Include(r => r.ModifiedByUser).Where(r => r.IsActive && r.IsDeleted == false);
            ViewBag.RegionCount = regions.Count();
            return View(regions.ToList());
        }

        // GET: /Region/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // GET: /Region/Create
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

        // POST: /Region/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "RegionId,RegionName,RegionKey,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Region region)
        {
            if (ModelState.IsValid)
            {
                var regionName =
                  db.Regions.Where(r => r.RegionName == region.RegionName).Select(
                      r => r.RegionName).FirstOrDefault();
                if (regionName == null)
                {
                    IdentityManager.CurrentUser(User);
                    region.IsDeleted = false;
                    region.IsActive = true;
                    region.IsLocked = false;
                    db.Regions.Add(region);
                    db.SaveChanges();
                    TempData["Success"] = "Region Saved.";
                }
                else
                {
                    TempData["Error"] = "Region Exists.";
                }

            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", region.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", region.ModifiedByUserId);
            return View(region);
        }

        // GET: /Region/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", region.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", region.ModifiedByUserId);
            return View(region);
        }

        // POST: /Region/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RegionId,RegionName,RegionKey,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Region region)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                region.IsDeleted = false;
                region.IsActive = true;
                region.IsLocked = false;

                var local = db.Set<Region>()
                .Local.FirstOrDefault(l => l.RegionId == region.RegionId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Info"] = "Region Updated.";
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", region.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", region.ModifiedByUserId);
            return View(region);
        }

        // GET: /Region/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        // POST: /Region/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Region region = db.Regions.Find(id);
            IdentityManager.CurrentUser(User);
            region.IsActive = false;
            region.IsDeleted = true;
            db.Entry(region).State = EntityState.Modified;
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
