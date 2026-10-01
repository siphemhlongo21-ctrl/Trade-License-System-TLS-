using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;

namespace C8.TradeLicense.Controllers
{
    [Authorize(Roles =RoleKeys.SystemAdmin)]
    public class AppSettingsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: AppSettings
        public ActionResult Index()
        {
            var appSettings = db.AppSettings.Include(a => a.CreatedByUser).Include(a => a.ModifiedByUser);
            return View(appSettings.ToList());
        }

        // GET: AppSettings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppSetting appSetting = db.AppSettings.Find(id);
            if (appSetting == null)
            {
                return HttpNotFound();
            }
            return View(appSetting);
        }

        // GET: AppSettings/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: AppSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Key,Value,Description,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] AppSetting appSetting)
        {
            if (ModelState.IsValid)
            {
                db.AppSettings.Add(appSetting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", appSetting.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", appSetting.ModifiedByUserId);
            return View(appSetting);
        }

        // GET: AppSettings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppSetting appSetting = db.AppSettings.Find(id);
            if (appSetting == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", appSetting.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", appSetting.ModifiedByUserId);
            return View(appSetting);
        }

        // POST: AppSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Key,Value,Description,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] AppSetting appSetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appSetting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", appSetting.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", appSetting.ModifiedByUserId);
            return View(appSetting);
        }

        // GET: AppSettings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppSetting appSetting = db.AppSettings.Find(id);
            if (appSetting == null)
            {
                return HttpNotFound();
            }
            return View(appSetting);
        }

        // POST: AppSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppSetting appSetting = db.AppSettings.Find(id);
            db.AppSettings.Remove(appSetting);
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
