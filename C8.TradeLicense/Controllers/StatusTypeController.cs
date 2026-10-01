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
    public class StatusTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public StatusTypeController()
        {

            IdentityManager = new IdentityManager(db);
        }
        public IdentityManager IdentityManager { get; set; }
        // GET: /StatusType/
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Index()
        {
            var statustypes = db.StatusTypes.Where(c => c.IsActive && c.IsDeleted == false);
            ViewBag.StatusTypeCount = statustypes.Count();
            return View(statustypes.ToList());
        }

        // GET: /StatusType/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusType statustype = db.StatusTypes.Find(id);
            if (statustype == null)
            {
                return HttpNotFound();
            }
            return View(statustype);
        }

        // GET: /StatusType/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /StatusType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "StatusTypeId,StatusTypeName,StatusTypeDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] StatusType statustype)
        {
            if (ModelState.IsValid)
            {
                var statusTypes = db.StatusTypes.Where(d => d.IsDeleted == false && d.StatusTypeName.Replace(" ", String.Empty).Equals(statustype.StatusTypeName.Replace(" ", String.Empty), StringComparison.InvariantCultureIgnoreCase));

                if (statusTypes.Count() == 0 && statustype.StatusTypeName !=  null)
                {
                    IdentityManager.CurrentUser(User);
                    statustype.IsActive = true;
                    statustype.IsDeleted = false;
                    statustype.IsLocked = false;
                    db.StatusTypes.Add(statustype);
                    db.SaveChanges();
                    TempData["Success"] = "Status Type saved Successfully!";

                    //return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Please enter data!";

                    //ModelState.AddModelError(string.Empty, "Status Type already exists");
                }
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", statustype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", statustype.ModifiedByUserId);
            return View(statustype);
        }

        // GET: /StatusType/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusType statustype = db.StatusTypes.Find(id);
            if (statustype == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", statustype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", statustype.ModifiedByUserId);
            return View(statustype);
        }

        // POST: /StatusType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "StatusTypeId,StatusTypeName,StatusTypeDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] StatusType statustype)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                statustype.IsActive = true;
                statustype.IsDeleted = false;
                statustype.IsLocked = false;
                statustype.CreatedDateTime = statustype.CreatedDateTime;

                var local = db.Set<StatusType>()
                .Local.FirstOrDefault(l => l.StatusTypeId == statustype.StatusTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(statustype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", statustype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", statustype.ModifiedByUserId);
            return View(statustype);
        }

        // GET: /StatusType/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StatusType statustype = db.StatusTypes.Find(id);
            if (statustype == null)
            {
                return HttpNotFound();
            }
            return View(statustype);
        }

        // POST: /StatusType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            IdentityManager.CurrentUser(User);
            //StatusType statustype = db.StatusTypes.Find(id);
            //statustype.IsActive = false;
            //statustype.IsDeleted = true;
            //statustype.IsLocked = false;
            //db.StatusTypes.Remove(statustype);
            //db.SaveChanges();
            //return RedirectToAction("Index");

            StatusType statustype = db.StatusTypes.Find(id);
            statustype.IsActive = false;
            statustype.IsDeleted = true;

            statustype.CreatedDateTime = DateTime.Now;
            db.Entry(statustype).State = EntityState.Modified;
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
