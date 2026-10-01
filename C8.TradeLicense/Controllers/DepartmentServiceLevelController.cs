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
    public class DepartmentServiceLevelController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public DepartmentServiceLevelController()
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

        // GET: /DepartmentServiceLevel/
        public ActionResult Index()
        {
            var departmentservicelevelagreements = db.DepartmentServiceLevelAgreements.Include(d => d.CreatedByUser).Include(d => d.Department).Include(d => d.ModifiedByUser).Include(d => d.ServiceLevelAgreement);
            return View(departmentservicelevelagreements.ToList());
        }

        // GET: /DepartmentServiceLevel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentServiceLevelAgreement departmentservicelevelagreement = db.DepartmentServiceLevelAgreements.Find(id);
            if (departmentservicelevelagreement == null)
            {
                return HttpNotFound();
            }
            return View(departmentservicelevelagreement);
        }

        // GET: /DepartmentServiceLevel/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.SlaId = new SelectList(db.ServiceLevelAgreements, "SlaId", "SlaKey");
            return View();
        }

        // POST: /DepartmentServiceLevel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="DepartmentSlaId,DepartmentId,SlaId,SlaDays,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] DepartmentServiceLevelAgreement departmentservicelevelagreement)
        {
            if (ModelState.IsValid)
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                IdentityManager.CurrentUser(User);

                departmentservicelevelagreement.IsActive = true;
                departmentservicelevelagreement.IsDeleted = false;
                departmentservicelevelagreement.IsLocked = false;

                db.DepartmentServiceLevelAgreements.Add(departmentservicelevelagreement);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentservicelevelagreement.CreatedByUserId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentservicelevelagreement.DepartmentId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentservicelevelagreement.ModifiedByUserId);
            ViewBag.SlaId = new SelectList(db.ServiceLevelAgreements, "SlaId", "SlaKey", departmentservicelevelagreement.SlaId);
            return View(departmentservicelevelagreement);
        }

        // GET: /DepartmentServiceLevel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentServiceLevelAgreement departmentservicelevelagreement = db.DepartmentServiceLevelAgreements.Find(id);
            if (departmentservicelevelagreement == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentservicelevelagreement.CreatedByUserId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentservicelevelagreement.DepartmentId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentservicelevelagreement.ModifiedByUserId);
            ViewBag.SlaId = new SelectList(db.ServiceLevelAgreements, "SlaId", "SlaKey", departmentservicelevelagreement.SlaId);
            return View(departmentservicelevelagreement);
        }

        // POST: /DepartmentServiceLevel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="DepartmentSlaId,DepartmentId,SlaId,SlaDays,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] DepartmentServiceLevelAgreement departmentservicelevelagreement)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<DepartmentServiceLevelAgreement>()
                    .Local.FirstOrDefault(l => l.DepartmentSlaId == departmentservicelevelagreement.DepartmentSlaId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(departmentservicelevelagreement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentservicelevelagreement.CreatedByUserId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentservicelevelagreement.DepartmentId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentservicelevelagreement.ModifiedByUserId);
            ViewBag.SlaId = new SelectList(db.ServiceLevelAgreements, "SlaId", "SlaKey", departmentservicelevelagreement.SlaId);
            return View(departmentservicelevelagreement);
        }

        // GET: /DepartmentServiceLevel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentServiceLevelAgreement departmentservicelevelagreement = db.DepartmentServiceLevelAgreements.Find(id);
            if (departmentservicelevelagreement == null)
            {
                return HttpNotFound();
            }
            return View(departmentservicelevelagreement);
        }

        // POST: /DepartmentServiceLevel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DepartmentServiceLevelAgreement departmentservicelevelagreement = db.DepartmentServiceLevelAgreements.Find(id);
            db.DepartmentServiceLevelAgreements.Remove(departmentservicelevelagreement);
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
