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
using System.Data.Entity.Infrastructure;

namespace C8.TradeLicense.Controllers
{
    public class TitleDeedTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public TitleDeedTypeController()
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

        // Add the Authorize with the respective roles to allow users to access the this function
         [Authorize]
        // GET: /TitleDeedType/
        public ActionResult Index()
        {
            var titledeedtypes = db.TitleDeedTypes.Include(t => t.CreatedByUser).Include(t => t.ModifiedByUser)
                .Where(t => t.IsActive && t.IsDeleted == false);
            ViewBag.TitleDeedCount = titledeedtypes.Count();
            return View(titledeedtypes.ToList()); // where clause to return the records that IsDeleted!=true 
        }

        // GET: /TitleDeedType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TitleDeedType titledeedtype = db.TitleDeedTypes.Find(id);
            if (titledeedtype == null)
            {
                return HttpNotFound();
            }
            return View(titledeedtype);
        }

        // GET: /TitleDeedType/Create
        public ActionResult Create()
        {
            //ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            //ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /TitleDeedType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TitleDeedTypeId,TitleDeedTypeName,TitleDeedDescription")] TitleDeedType titledeedtype)
        {

            if (ModelState.IsValid)
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                IdentityManager.CurrentUser(User);

                titledeedtype.IsActive = true;
                titledeedtype.IsDeleted = false;
                titledeedtype.IsLocked = false;

                db.TitleDeedTypes.Add(titledeedtype);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", titledeedtype.CreatedByUserId);
            //ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", titledeedtype.ModifiedByUserId);
            return View(titledeedtype);
        }

        // GET: /TitleDeedType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TitleDeedType titledeedtype = db.TitleDeedTypes.Find(id);
            if (titledeedtype == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", titledeedtype.CreatedByUserId);
            //ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", titledeedtype.ModifiedByUserId);
            return View(titledeedtype);
        }

        // POST: /TitleDeedType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TitleDeedTypeId,TitleDeedTypeName,TitleDeedDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] TitleDeedType titledeedtype)
        {
            if (ModelState.IsValid)
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                IdentityManager.CurrentUser(User);

                var local = db.Set<TitleDeedType>()
                .Local.FirstOrDefault(l => l.TitleDeedTypeId == titledeedtype.TitleDeedTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(titledeedtype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", titledeedtype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", titledeedtype.ModifiedByUserId);
            return View(titledeedtype);
        }

        // GET: /TitleDeedType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TitleDeedType titledeedtype = db.TitleDeedTypes.Find(id);
            if (titledeedtype == null)
            {
                return HttpNotFound();
            }
            return View(titledeedtype);
        }


        // Set the Value of to true , to show it has been deleted 
        // POST: /TitleDeedType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
            IdentityManager.CurrentUser(User);
            
            TitleDeedType titledeedtype = db.TitleDeedTypes.Find(id);
            if(titledeedtype != null)
            {
                titledeedtype.IsDeleted = true;
                db.Entry(titledeedtype).State = EntityState.Modified;
                db.SaveChanges();
            }
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
