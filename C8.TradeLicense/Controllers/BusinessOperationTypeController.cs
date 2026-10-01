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
    public class BusinessOperationTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();


        [Authorize]
        // GET: /BusinessOperationType/
        // JK.20140726a - Add the Authorize with the respective roles to allow users to access the this function.
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Index()
        {
            ViewBag.BusinessOppTypeCount = db.BusinessOperationTypes.Where(b => b.IsActive && b.IsDeleted == false).Count();
            return View(db.BusinessOperationTypes.Where(b => b.IsActive && b.IsDeleted == false).ToList());
        }

        // GET: /BusinessOperationType/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessOperationType businessoperationtype = db.BusinessOperationTypes.Find(id);
            if (businessoperationtype == null)
            {
                return HttpNotFound();
            }
            return View(businessoperationtype);
        }

        // GET: /BusinessOperationType/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /BusinessOperationType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create([Bind(Include="BusinessOperationTypeId,BusinessOperationTypeName,BusinessOperationTypeDescription")] BusinessOperationType businessoperationtype)
        {
            if (ModelState.IsValid)
            {
                db.BusinessOperationTypes.Add(businessoperationtype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(businessoperationtype);
        }

        // GET: /BusinessOperationType/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessOperationType businessoperationtype = db.BusinessOperationTypes.Find(id);
            if (businessoperationtype == null)
            {
                return HttpNotFound();
            }
            return View(businessoperationtype);
        }

        // POST: /BusinessOperationType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include="BusinessOperationTypeId,BusinessOperationTypeName,BusinessOperationTypeDescription")] BusinessOperationType businessoperationtype)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<BusinessOperationType>().Local
                    .FirstOrDefault(l => l.BusinessOperationTypeId == businessoperationtype.BusinessOperationTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(businessoperationtype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(businessoperationtype);
        }

        // GET: /BusinessOperationType/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessOperationType businessoperationtype = db.BusinessOperationTypes.Find(id);
            if (businessoperationtype == null)
            {
                return HttpNotFound();
            }
            return View(businessoperationtype);
        }


        //The is no Active or Deleted 
        // POST: /BusinessOperationType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinessOperationType businessoperationtype = db.BusinessOperationTypes.Find(id);
            db.BusinessOperationTypes.Remove(businessoperationtype);
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
