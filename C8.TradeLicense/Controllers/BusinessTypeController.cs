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
    public class BusinessTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: /BusinessType/
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Index()
        {
            ViewBag.BusinessTypeCount = db.BusinessTypes.Where(b => b.IsActive && b.IsDeleted == false).Count();
            var businesstypes = db.BusinessTypes.Include(b => b.CreatedByUser).Include(b => b.ModifiedByUser);
            return View(businesstypes.ToList().Where(s=>s.IsDeleted!=true));
        }
       // GET: /BusinessType/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessType businesstype = db.BusinessTypes.Find(id);
            if (businesstype == null)
            {
                return HttpNotFound();
            }
            return View(businesstype);
        }

        // GET: /BusinessType/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /BusinessType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create([Bind(Include="BusinessTypeId,BusinessTypeName,BusinessTypeDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] BusinessType businesstype)
        {
            if (ModelState.IsValid)
            {
                db.BusinessTypes.Add(businesstype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", businesstype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", businesstype.ModifiedByUserId);
            return View(businesstype);
        }

        // GET: /BusinessType/Edit/5
        //only admin users can edit
       // [Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessType businesstype = db.BusinessTypes.Find(id);
            if (businesstype == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", businesstype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", businesstype.ModifiedByUserId);
            return View(businesstype);
        }

        // POST: /BusinessType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include="BusinessTypeId,BusinessTypeName,BusinessTypeDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] BusinessType businesstype)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<BusinessType>().Local
                    .FirstOrDefault(l => l.BusinessTypeId == businesstype.BusinessTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(businesstype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", businesstype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", businesstype.ModifiedByUserId);
            return View(businesstype);
        }

        // GET: /BusinessType/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessType businesstype = db.BusinessTypes.Find(id);
            if (businesstype == null)
            {
                return HttpNotFound();
            }
            return View(businesstype);
        }

        // POST: /BusinessType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinessType businesstype = db.BusinessTypes.Find(id);
           // db.BusinessTypes.Remove(businesstype);
            businesstype.IsDeleted = true;
            db.Entry(businesstype).State = EntityState.Modified;

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
