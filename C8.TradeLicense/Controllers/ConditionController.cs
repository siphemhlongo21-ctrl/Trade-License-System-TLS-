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
    public class ConditionController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: /Condition/
        public ActionResult Index(string Screen,int? Id)
        {
            if(Id != null)
            {
                ViewBag.Screen = Screen;
                ViewBag.Id = Id;
            }
            var conditions = db.Conditions.Include(c => c.CreatedByUser).Include(c => c.ModifiedByUser)
                .Where(c => c.IsActive && c.IsDeleted == false);
            ViewBag.ConditionCount = conditions.Count();
        
            return View(conditions.ToList());

        }

        // GET: /Condition/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // GET: /Condition/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }
    
        // POST: /Condition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ConditionId,ItemConditionId,ConditionName,ConditionDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Condition condition)
        {
            if (ModelState.IsValid)
            {
                var checkConditions = db.Conditions.Where(x => x.ConditionName == condition.ConditionName).FirstOrDefault();
                if (checkConditions == null)
                { 
                    condition.IsActive = true;
                condition.IsDeleted = false;
                db.Conditions.Add(condition);
                db.SaveChanges();
            }
                else
                {

                    Condition con = db.Conditions.Find(checkConditions.ConditionId);
                    con.IsActive = true;
                    con.IsDeleted = false;
                    
                    db.Entry(con).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.CreatedByUserId);
            //ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName", condition.ItemConditionId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.ModifiedByUserId);
            return View(condition);
        }
        public ActionResult AddCondition(int? id)
        {
            
                string CheckList = "";
            if (id == null)
            {
                //Condition condition = db.Conditions.ToList();
            }
            else          
            {
                var LAC = db.LicenseApplicationConditions.Where(l=> l.LicenseId == id).Include(l=> l.Condition).ToList();
                    foreach (var item in LAC)
                {
                    CheckList += item.Condition.ConditionName + ",";
                }
                //List Start



                var GetConditiontList = db.Conditions.ToList();
                LicenseApplicationConditions[] LicenseApplicationConditionsList = new LicenseApplicationConditions[GetConditiontList.Count()];
                int count = 0;

                foreach (var item in GetConditiontList)
                {
                    LicenseApplicationConditions temp = new LicenseApplicationConditions();
                    if (CheckList.IndexOf(item.ConditionName) > -1)
                    {
                        //temp.IsSelected = true;
                    }
                    else
                    {
                        //temp.IsSelected = false;
                    }
                    temp.ConditionId = item.ConditionId;
                    temp.Condition.ConditionName = item.ConditionName;
                    LicenseApplicationConditionsList[count] = temp;
                    count++;
                    /////// 
                }

                //GetDepartmentList End
                //GetApplicationChecklist Start


                ViewBag.DeptCheckList = CheckList;
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.CreatedByUserId);
            //ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName", condition.ItemConditionId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.ModifiedByUserId);
            return View(condition);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCondition([Bind(Include = "ConditionId,ItemConditionId,ConditionName,ConditionDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Condition condition)
        {
            if (ModelState.IsValid)
            {
                db.Conditions.Add(condition);
                db.SaveChanges();
                return RedirectToAction("AddCondition");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.CreatedByUserId);
            //ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName", condition.ItemConditionId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.ModifiedByUserId);
            return View(condition);
        }
  
        // GET: /Condition/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.CreatedByUserId);
            //ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName", condition.ItemConditionId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.ModifiedByUserId);
            return View(condition);
        }

        // POST: /Condition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ConditionId,ItemConditionId,ConditionName,ConditionDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Condition condition)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<Condition>().Local
                    .FirstOrDefault(l => l.ConditionId == condition.ConditionId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                condition.IsActive = true;
                db.Entry(condition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.CreatedByUserId);
            //ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName", condition.ItemConditionId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", condition.ModifiedByUserId);
            return View(condition);
        }

        // GET: /Condition/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Condition condition = db.Conditions.Find(id);
            if (condition == null)
            {
                return HttpNotFound();
            }
            return View(condition);
        }

        // POST: /Condition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Condition condition = db.Conditions.Find(id);
            condition.IsActive = false;
            condition.IsDeleted = true;
           
            db.Entry(condition).State = EntityState.Modified;
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
