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
using C8.TradeLicense.Helpers;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace C8.TradeLicense.Controllers
{
    public class DocumentController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        FileHelpers _fileHelpers;

        public DocumentController()
        {
            // JK.20140906a - Instantiate the IdentityManager in the contructor and pass the DbContext.
            IdentityManager = new IdentityManager(db);
            _fileHelpers = new FileHelpers(db);
        }

        /// <summary>
        /// JK.20140906a - Create a Identity Manager property to be used in any function. Instantiate in the contructor.
        /// Gets or sets the identity manager.
        /// </summary>
        /// <value>
        /// The identity manager.
        /// </value>
        public IdentityManager IdentityManager { get; set; }
        // GET: /Document/
        public ActionResult Index()
        {
            var documents = db.Documents.Include(d => d.CreatedByUser).Include(d => d.DocumentType).Include(d => d.ModifiedByUser).OrderBy(d => d.DocumentType.DocumentTypeName)
                .Where(d => d.IsActive && d.IsDeleted == false);
            ViewBag.DocumentCount = documents.Count();
            return View(documents.ToList());
        }
        public async Task<ActionResult> Download(string fileLink)
        {
            SharePointDocument doc = await _fileHelpers.DownloadFile(fileLink);
            if (string.IsNullOrEmpty(doc.FileName))
            {
                return null;
            }
            string mimeType = MimeMapping.GetMimeMapping(doc.FileName);
            return File(doc.FileContent, mimeType);
        }
        // GET: /Document/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: /Document/Create
        public ActionResult Create()
        {
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.DocumentTypeId = db.DocumentTypes.ToList();
            //new SelectList(db.DocumentTypes, "DocumentTypeId", "DocumentTypeName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /Document/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocumentId,DocumentName,DocumentDescription,DocumentKey,DocumentTypeId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Document document)
        {
            if (ModelState.IsValid)
            {
                var documentName =
          db.Documents.Where(s => s.DocumentName == document.DocumentName).Select(
              s => s.DocumentName).FirstOrDefault();
                if (documentName == null)
                {
                    IdentityManager.CurrentUser(User);
                    document.IsActive = true;
                    document.IsDeleted = false;
                    document.IsLocked = false;
                    db.Documents.Add(document);
                    db.SaveChanges();
                    TempData["Success"] = "Document saved Successfully!";
                }
                else
                {
                    TempData["Error"] = "Document Exists!.";
                }
                //return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", document.CreatedByUserId);
            ViewBag.DocumentTypeId = db.DocumentTypes.ToList();
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", document.ModifiedByUserId);
            return View(document);
        }

        // GET: /Document/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", document.CreatedByUserId);
            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "DocumentTypeId", "DocumentTypeName", document.DocumentTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", document.ModifiedByUserId);
            return View(document);
        }

        // POST: /Document/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocumentId,DocumentName,DocumentDescription,DocumentKey,DocumentTypeId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] Document document)
        {
            if (ModelState.IsValid)
            {
                var local = db.Set<Document>()
                  .Local.FirstOrDefault(l => l.DocumentId == document.DocumentId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", document.CreatedByUserId);
            ViewBag.DocumentTypeId = new SelectList(db.DocumentTypes, "DocumentTypeId", "DocumentTypeName", document.DocumentTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", document.ModifiedByUserId);
            return View(document);
        }

        // GET: /Document/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: /Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
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
