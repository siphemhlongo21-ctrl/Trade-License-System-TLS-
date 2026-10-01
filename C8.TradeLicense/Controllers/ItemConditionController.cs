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
    public class ItemConditionController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: /ItemCondition/
        public ActionResult Index()
        {
            var itemconditions = db.ItemConditions.Include(i => i.CreatedByUser).Include(i => i.ItemType).Include(i => i.ModifiedByUser)
                .Where(i => i.IsActive && i.IsDeleted == false);
            ViewBag.ItemConditionCount = itemconditions.Count();
            return View(itemconditions.ToList());
        }

        // GET: /ItemCondition/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemCondition itemcondition = db.ItemConditions.Find(id);
            if (itemcondition == null)
            {
                return HttpNotFound();
            }
            return View(itemcondition);
        }

        // GET: /ItemCondition/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /ItemCondition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ItemConditionId,ItemTypeId,ItemConditionName,ItemConditionDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] ItemCondition itemcondition)
        {
            if (ModelState.IsValid)
            {
                db.ItemConditions.Add(itemcondition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemcondition.CreatedByUserId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName", itemcondition.ItemTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemcondition.ModifiedByUserId);
            return View(itemcondition);
        }

        // GET: /ItemCondition/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemCondition itemcondition = db.ItemConditions.Find(id);
            if (itemcondition == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemcondition.CreatedByUserId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName", itemcondition.ItemTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemcondition.ModifiedByUserId);
            return View(itemcondition);
        }

        // POST: /ItemCondition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ItemConditionId,ItemTypeId,ItemConditionName,ItemConditionDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] ItemCondition itemcondition)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<ItemCondition>()
                .Local.FirstOrDefault(l => l.ItemConditionId == itemcondition.ItemConditionId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(itemcondition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemcondition.CreatedByUserId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName", itemcondition.ItemTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemcondition.ModifiedByUserId);
            return View(itemcondition);
        }

        // GET: /ItemCondition/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemCondition itemcondition = db.ItemConditions.Find(id);
            if (itemcondition == null)
            {
                return HttpNotFound();
            }
            return View(itemcondition);
        }

        // POST: /ItemCondition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemCondition itemcondition = db.ItemConditions.Find(id);
            db.ItemConditions.Remove(itemcondition);
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
