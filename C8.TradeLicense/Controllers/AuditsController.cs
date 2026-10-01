using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Models;
using PagedList;

namespace C8.TradeLicense.Controllers
{
    public class AuditsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: Audits
        [Authorize(Roles = "Licensing Administrator" +  "," + "System Admin")]
        public ActionResult Index(int? page, string inputSearch)
        {
            List<Audit> searchResults = null;
            var audit = db.Audit.ToList();
            if (inputSearch != null)
            {
                ViewBag.inputSearch = inputSearch;
            var LicenseId = db.Licenses.Where(l => l.LicenseNumber == inputSearch).Select(s => s.LicenseId).FirstOrDefault();
           
                var results = db.Audit.Where(x=> x.PrimaryKey == LicenseId || x.LicenseId == LicenseId).ToList();
                searchResults = results;
            }
            else
            {
                var results = db.Audit.ToList();
                searchResults = results;
            }

           
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(searchResults.Count > 0 ? searchResults.ToPagedList(pageNumber, pageSize) : audit.ToPagedList(pageNumber, pageSize));
        }

        // GET: Audits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        // GET: Audits/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Audits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AuditId,Action,PrimaryKey,LicenseId,TableName,ColumnName,OriginalValue,CurrentValue,AuditByUserId,AuditDateTime")] Audit audit)
        {
            if (ModelState.IsValid)
            {
                db.Audit.Add(audit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(audit);
        }

        // GET: Audits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        // POST: Audits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AuditId,Action,PrimaryKey,LicenseId,TableName,ColumnName,OriginalValue,CurrentValue,AuditByUserId,AuditDateTime")] Audit audit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(audit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(audit);
        }

        // GET: Audits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        // POST: Audits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Audit audit = db.Audit.Find(id);
            db.Audit.Remove(audit);
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
