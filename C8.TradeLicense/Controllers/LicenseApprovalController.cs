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
    public class LicenseApprovalController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: /LicenseApproval/
        public ActionResult Index()
        {
            var licenses = db.Licenses.Include(l => l.Business).Include(l => l.Client).Include(l => l.CreatedByUser).Include(l => l.LicenseType).Include(l => l.ModifiedByUser).Include(l => l.Status);
            return View(licenses.Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.StatusId == db.Status.Where(s => s.StatusName == "LicenseApproved").Select(s => s.StatusId).FirstOrDefault()).ToList());
        }

        // GET: /LicenseApproval/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        // GET: /LicenseApproval/Create
        public ActionResult Create()
        {
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber");
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName");
            return View();
        }

        // POST: /LicenseApproval/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="LicenseId,LicenseTypeId,ClientId,BusinessId,ApplicationDateTime,LicenseIssueDateTime,NotificationUpdatedDateTime,StatusId,ClientInformationClearance,BusinessInformationClearance,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] License license)
        {
            if (ModelState.IsValid)
            {
                db.Licenses.Add(license);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.CreatedByUserId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.ModifiedByUserId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", license.StatusId);
            return View(license);
        }

        // GET: /LicenseApproval/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.CreatedByUserId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.ModifiedByUserId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", license.StatusId);
            return View(license);
        }

        // POST: /LicenseApproval/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="LicenseId,LicenseTypeId,ClientId,BusinessId,ApplicationDateTime,LicenseIssueDateTime,NotificationUpdatedDateTime,StatusId,ClientInformationClearance,BusinessInformationClearance,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] License license)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<License>()
               .Local.FirstOrDefault(l => l.LicenseId == license.LicenseId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.CreatedByUserId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.ModifiedByUserId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", license.StatusId);
            return View(license);
        }

        // GET: /LicenseApproval/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        // POST: /LicenseApproval/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            License license = db.Licenses.Find(id);
            db.Licenses.Remove(license);
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
