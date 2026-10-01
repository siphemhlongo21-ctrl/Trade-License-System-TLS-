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
using C8.TradeLicense.ViewModels;

namespace C8.TradeLicense.Controllers
{
    public class DocumentCheckListController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        // GET: /DocumentCheckList/
        public ActionResult Index()
        {
            IQueryable<DocumentCheckList> documentCheck;
            //using (var db = new TradeLicenseDbContext())
            //{               
            documentCheck = db.DocumentCheckLists.Include(dc => dc.Document).Include(dc => dc.LicenseType).Include(dc => dc.Document.DocumentType);
            var documentCheckList = db.DocumentCheckLists.Where(c => c.IsActive && c.IsDeleted == false);
            ViewBag.DocumentListCount = documentCheckList.Count();
            //}
            return View(documentCheckList.ToList());
        }

        // GET: /DocumentCheckList/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentCheckList documentchecklist = db.DocumentCheckLists.Find(id);
            if (documentchecklist == null)
            {
                return HttpNotFound();
            }
            return View(documentchecklist);
        }

        // GET: /DocumentCheckList/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.DocumentId = new SelectList(db.Documents, "DocumentId", "DocumentName");
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            //List<SelectListItem> licenceType = new List<SelectListItem>();
            //ViewBag.LicenceType = new SelectList(db.LicenseTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false));
            //ViewBag.DocumentType = new SelectList(db.DocumentTypes.Where(c => c.IsActive == true).Where(c => c.IsDeleted == false));
            return View();
        }

        // POST: /DocumentCheckList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentCheckListId,LicenseTypeId,DocumentId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] DocumentCheckList documentchecklist)
        {
            if (ModelState.IsValid)
            {
                documentchecklist.IsDeleted = false;
                documentchecklist.IsActive = true;
                db.DocumentCheckLists.Add(documentchecklist);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", documentchecklist.CreatedByUserId);
            ViewBag.DocumentId = new SelectList(db.Documents, "DocumentId", "DocumentName",documentchecklist.DocumentId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", documentchecklist.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", documentchecklist.ModifiedByUserId);
            return View(documentchecklist);
        }

        // GET: /DocumentCheckList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentCheckList documentchecklist = db.DocumentCheckLists.Find(id);
            if (documentchecklist == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", documentchecklist.CreatedByUserId);
            ViewBag.DocumentId = new SelectList(db.Documents, "DocumentId", "DocumentName", documentchecklist.DocumentId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", documentchecklist.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", documentchecklist.ModifiedByUserId);
            return View(documentchecklist);
        }

        // POST: /DocumentCheckList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentCheckListId,LicenseTypeId,DocumentId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] DocumentCheckList documentchecklist)
        {
            if (ModelState.IsValid)
            {
                documentchecklist.IsActive = true;
                documentchecklist.IsDeleted = false;

                var local = db.Set<DocumentCheckList>()
                   .Local.FirstOrDefault(l => l.DocumentCheckListId == documentchecklist.DocumentCheckListId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(documentchecklist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", documentchecklist.CreatedByUserId);
            ViewBag.DocumentId = new SelectList(db.DocumentTypes, "DocumentId", "DocumentName", documentchecklist.DocumentId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", documentchecklist.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", documentchecklist.ModifiedByUserId);
            return View(documentchecklist);
        }

        // GET: /DocumentCheckList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentCheckList documentchecklist = db.DocumentCheckLists.Find(id);
            if (documentchecklist == null)
            {
                return HttpNotFound();
            }
            return View(documentchecklist);
        }

        // POST: /DocumentCheckList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentCheckList documentchecklist = db.DocumentCheckLists.Find(id);
            documentchecklist.IsActive = false;
            documentchecklist.IsDeleted = true;
            db.DocumentCheckLists.Remove(documentchecklist);
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
