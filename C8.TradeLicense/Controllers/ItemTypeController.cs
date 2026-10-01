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
    public class ItemTypeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public ItemTypeController()
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
        // GET: /ItmeType/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Index()
        {
            var itemtypes = db.ItemTypes.Include(i => i.CreatedByUser).Include(i => i.ModifiedByUser).Where(i => i.IsDeleted == false && i.IsActive);
            ViewBag.ItemType = itemtypes.Count();
            return View(itemtypes.ToList());
        }

        // GET: /ItmeType/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemtype = db.ItemTypes.Find(id);
            if (itemtype == null)
            {
                return HttpNotFound();
            }
            return View(itemtype);
        }

        // GET: /ItmeType/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create()
        {
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /ItmeType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "ItemTypeId,ItemTypeName,ItemTypeDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,ItemTypeKey")] ItemType itemtype)
        {
            if (ModelState.IsValid)
            {

                var itemTypeName =
                    db.ItemTypes.Where(i => i.ItemTypeName == itemtype.ItemTypeName).Select(
                        i => i.ItemTypeName).FirstOrDefault();
                if (itemTypeName == null)
                {
                    IdentityManager.CurrentUser(User);
                    itemtype.IsDeleted = false;
                    itemtype.IsActive = true;
                    itemtype.IsLocked = false;
                    db.ItemTypes.Add(itemtype);
                    db.SaveChanges();
                    TempData["Success"] = "Item Type Saved.";
                }
                else
                {
                    TempData["Error"] = "Item Type Exists.";
                }

            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemtype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemtype.ModifiedByUserId);
            return View(itemtype);//return RedirectToAction("Index");  //
        }

        // GET: /ItmeType/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemtype = db.ItemTypes.Find(id);
            if (itemtype == null)
            {
                return HttpNotFound();
            }
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemtype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemtype.ModifiedByUserId);
            return View(itemtype);
        }

        // POST: /ItmeType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "ItemTypeId,ItemTypeName,ItemTypeDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,ItemTypeKey")] ItemType itemtype)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                itemtype.IsDeleted = false;
                itemtype.IsActive = true;
                itemtype.IsLocked = false;
                //db.ItemTypes.Add(itemtype);

                var local = db.Set<ItemType>()
                .Local.FirstOrDefault(l => l.ItemTypeId == itemtype.ItemTypeId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(itemtype).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Info"] = "Item Type Update.";
                //return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemtype.CreatedByUserId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemtype.ModifiedByUserId);
            return View(itemtype); //return RedirectToAction("Index");//
        }

        // GET: /ItmeType/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemtype = db.ItemTypes.Find(id);
            if (itemtype == null)
            {
                return HttpNotFound();
            }
            return View(itemtype);
        }

        // POST: /ItmeType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemType itemtype = db.ItemTypes.Find(id);
            IdentityManager.CurrentUser(User);
            itemtype.IsActive = false;
            itemtype.IsDeleted = true;
            db.Entry(itemtype).State = EntityState.Modified;
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
