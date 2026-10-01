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
     [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
    public class OperationStructureTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public OperationStructureTypeController()
        {
            // JK.20140906a - Instantiate the IdentityManager in the contructor and pass the DbContext.
            IdentityManager = new IdentityManager(db);
        }

        public IdentityManager IdentityManager { get; set; }

        // GET: /OperationStructureType/      
        public ActionResult Index()
        {
            var operationStructureTypes = db.OperationStructureTypes.Where(o => o.IsActive && o.IsDeleted == false);
            ViewBag.OperationTypeCount = operationStructureTypes.Count();
            return View(operationStructureTypes.ToList());
        }

        // GET: /OperationStructureType/Details/5
         
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationStructureType operationstructuretype = db.OperationStructureTypes.Find(id);
            if (operationstructuretype == null)
            {
                return HttpNotFound();
            }
            return View(operationstructuretype);
        }

        // GET: /OperationStructureType/Create
       
        public ActionResult Create()
        {
            return View();
        }

        // POST: /OperationStructureType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="OperationStructureTypeId,OperationStructureTypeName,OperationStructureTypeDescription")] OperationStructureType operationstructuretype)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                db.OperationStructureTypes.Add(operationstructuretype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(operationstructuretype);
        }

        // GET: /OperationStructureType/Edit/5
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationStructureType operationstructuretype = db.OperationStructureTypes.Find(id);
            if (operationstructuretype == null)
            {
                return HttpNotFound();
            }
            return View(operationstructuretype);
        }

        // POST: /OperationStructureType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="OperationStructureTypeId,OperationStructureTypeName,OperationStructureTypeDescription")] OperationStructureType operationstructuretype)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                operationstructuretype.IsDeleted = false;
                operationstructuretype.IsActive = true;
                operationstructuretype.IsLocked = false;

                var local = db.Set<OperationStructureType>()
                .Local.FirstOrDefault(l => l.OperationStructureTypeId == operationstructuretype.OperationStructureTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(operationstructuretype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(operationstructuretype);
        }

        // GET: /OperationStructureType/Delete/5
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationStructureType operationstructuretype = db.OperationStructureTypes.Find(id);
            if (operationstructuretype == null)
            {
                return HttpNotFound();
            }
            return View(operationstructuretype);
        }

        // POST: /OperationStructureType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IdentityManager.CurrentUser(User);
            OperationStructureType operationstructuretype = db.OperationStructureTypes.Find(id);
            db.OperationStructureTypes.Remove(operationstructuretype);
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
