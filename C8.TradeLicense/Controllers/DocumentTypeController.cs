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
    public class DocumentTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public DocumentTypeController()
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

        // GET: /DocumentType/
        public ActionResult Index()
        {
            var documentTypes = db.DocumentTypes.Where(d => d.IsActive && d.IsDeleted == false);
            ViewBag.DocumentTypeCount = documentTypes.Count();
            return View(documentTypes.ToList());
        }

        // GET: /DocumentType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentType documenttype = db.DocumentTypes.Find(id);
            if (documenttype == null)
            {
                return HttpNotFound();
            }
            return View(documenttype);
        }

        // GET: /DocumentType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /DocumentType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="DocumentTypeId,DocumentTypeName,DocumentKey,DocumentTypeDescription")] DocumentType documenttype)
        {
            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
            IdentityManager.CurrentUser(User);

            if (ModelState.IsValid)
            {
                documenttype.IsActive = true;
                documenttype.IsDeleted = false;
                documenttype.IsLocked = false;        
                db.DocumentTypes.Add(documenttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(documenttype);
        }

        // GET: /DocumentType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentType documenttype = db.DocumentTypes.Find(id);
            if (documenttype == null)
            {
                return HttpNotFound();
            }
            return View(documenttype);
        }

        // POST: /DocumentType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentTypeId,DocumentTypeName,DocumentTypeKey, DocumentTypeDescription")] DocumentType documenttype)
        {
            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
            IdentityManager.CurrentUser(User);
            documenttype.IsDeleted = false;
            documenttype.IsActive = true;
            documenttype.IsLocked = false;

            if (ModelState.IsValid)
            {
                var local = db.Set<DocumentType>()
                .Local.FirstOrDefault(l => l.DocumentTypeId == documenttype.DocumentTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(documenttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(documenttype);
        }

        // GET: /DocumentType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentType documenttype = db.DocumentTypes.Find(id);
            if (documenttype == null)
            {
                return HttpNotFound();
            }
            return View(documenttype);
        }

        // POST: /DocumentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
            IdentityManager.CurrentUser(User);

            DocumentType documenttype = db.DocumentTypes.Find(id);

            if(documenttype != null)
            {
                documenttype.IsDeleted = true;
                documenttype.IsActive = false;

                db.Entry(documenttype).State = EntityState.Modified;

                db.SaveChanges();
            }
            //db.DocumentTypes.Remove(documenttype);
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
