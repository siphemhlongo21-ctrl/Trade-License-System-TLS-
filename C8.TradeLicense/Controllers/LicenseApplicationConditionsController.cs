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

namespace C8.TradeLicense.Controllers
{
    public class LicenseApplicationConditionsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: LicenseApplicationConditions
        public ActionResult Index()
        {
            var licenseApplicationConditions = db.LicenseApplicationConditions.Include(l => l.Condition).Include(l => l.CreatedByUser).Include(l => l.License).Include(l => l.ModifiedByUser);
            return View(licenseApplicationConditions.ToList());
        }

        // GET: LicenseApplicationConditions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseApplicationConditions licenseApplicationConditions = db.LicenseApplicationConditions.Find(id);
            if (licenseApplicationConditions == null)
            {
                return HttpNotFound();
            }
            return View(licenseApplicationConditions);
        }
        public ActionResult AddChecklistItem(string itemName, string itemID, string LicenseId)
        {
            string result = "";
            try
            {
           
                if (LicenseId != null)
                {
                    int ID = Convert.ToInt32(LicenseId);
                    int ConditionId = Convert.ToInt32(itemID);
                    var getLACID = db.LicenseApplicationConditions.Where(x => x.ConditionId == ConditionId && x.LicenseId == ID).FirstOrDefault();
                    if (getLACID == null)
                    {
                        LicenseApplicationConditions CheckList = new LicenseApplicationConditions();
                        CheckList.LicenseId = ID;
                        CheckList.ConditionName = itemName;
                        CheckList.IsSelected = true;
                        CheckList.ConditionId = ConditionId;
                        CheckList.IsActive = true;
                        CheckList.IsDeleted = false;
                        db.LicenseApplicationConditions.Add(CheckList);
                        db.SaveChanges();
                    }
                    else
                    {
                       
                        LicenseApplicationConditions LAC = db.LicenseApplicationConditions.Find(getLACID.LicenseApplicationConditionsId);
                        LAC.IsActive = true;
                        LAC.IsDeleted = false;
                        LAC.IsSelected = true;
                        db.Entry(LAC).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    result = "Success";

                }
              
          
            }
            catch (Exception e)
            {
                result = "Error";
                var ee = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveChecklistItem(string itemID, string LicenseId)
        {
            string result = "";
            try
            {

                if (itemID != null)
                {
                    int ID = Convert.ToInt32(LicenseId);
                    int ConditionId = Convert.ToInt32(itemID);
                    var getLACID = db.LicenseApplicationConditions.Where(x => x.ConditionId == ConditionId && x.LicenseId == ID).Select(s => s.LicenseApplicationConditionsId).FirstOrDefault();
                    LicenseApplicationConditions LAC = db.LicenseApplicationConditions.Find(getLACID);
                    LAC.IsActive = false;
                    LAC.IsDeleted = true;
                    LAC.IsSelected = false;
                    db.Entry(LAC).State = EntityState.Modified;
                    db.SaveChanges();
                    result = "Success";
                }
            }
            catch (Exception e)
            {
                result = "Error";
                var ee = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: LicenseApplicationConditions/Create
        public ActionResult Create(int?id)
        {
            var licenseApplicationConditions = db.LicenseApplicationConditions.Where(l=> l.LicenseId == id).Include(l => l.Condition).Include(l => l.CreatedByUser).Include(l => l.License).Include(l => l.ModifiedByUser);
            ViewData["License"] = db.Licenses.Find(id);
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditionId", "ConditionName");
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View(licenseApplicationConditions.ToList());
        }

        // POST: LicenseApplicationConditions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LicenseApplicationConditionsId,LicenseId,ConditionId,ConditionName,IsSelected,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] LicenseApplicationConditions licenseApplicationConditions)
        {
            if (ModelState.IsValid)
            {
                db.LicenseApplicationConditions.Add(licenseApplicationConditions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditionId", "ConditionName", licenseApplicationConditions.ConditionId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", licenseApplicationConditions.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", licenseApplicationConditions.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", licenseApplicationConditions.ModifiedByUserId);
            return View(licenseApplicationConditions);
        }

        // GET: LicenseApplicationConditions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseApplicationConditions licenseApplicationConditions = db.LicenseApplicationConditions.Find(id);
            if (licenseApplicationConditions == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditionId", "ConditionName", licenseApplicationConditions.ConditionId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", licenseApplicationConditions.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", licenseApplicationConditions.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", licenseApplicationConditions.ModifiedByUserId);
            return View(licenseApplicationConditions);
        }

        // POST: LicenseApplicationConditions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LicenseApplicationConditionsId,LicenseId,ConditionId,ConditionName,IsSelected,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] LicenseApplicationConditions licenseApplicationConditions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(licenseApplicationConditions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditionId", "ConditionName", licenseApplicationConditions.ConditionId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", licenseApplicationConditions.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", licenseApplicationConditions.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", licenseApplicationConditions.ModifiedByUserId);
            return View(licenseApplicationConditions);
        }

        // GET: LicenseApplicationConditions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenseApplicationConditions licenseApplicationConditions = db.LicenseApplicationConditions.Find(id);
            if (licenseApplicationConditions == null)
            {
                return HttpNotFound();
            }
            return View(licenseApplicationConditions);
        }

        // POST: LicenseApplicationConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LicenseApplicationConditions licenseApplicationConditions = db.LicenseApplicationConditions.Find(id);
            db.LicenseApplicationConditions.Remove(licenseApplicationConditions);
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
