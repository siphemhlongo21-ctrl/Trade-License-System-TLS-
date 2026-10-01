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
    public class ItemSubCategoryController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public ItemSubCategoryController()
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

        // GET: /ItemSubCategory/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Index()
        {
            var itemsubcategories = db.ItemSubCategories.Include(i => i.CreatedByUser).Include(i => i.ItemType).Include(i => i.ModifiedByUser).Where(i => i.IsDeleted == false && i.IsActive).OrderBy(i => i.ItemType.ItemTypeName);
            ViewBag.ItemSubCategoryCount = itemsubcategories.Count();
            return View(itemsubcategories.ToList());
        }

        // GET: /ItemSubCategory/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemSubCategory itemsubcategory = db.ItemSubCategories.Find(id);
            if (itemsubcategory == null)
            {
                return HttpNotFound();
            }
            return View(itemsubcategory);
        }

        // GET: /ItemSubCategory/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create()
        {
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(it => it.IsDeleted == false && it.IsActive == true), "ItemTypeId", "ItemTypeName");//db.ItemTypes.ToList(); //new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: /ItemSubCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemSubCategoryId,ItemTypeId,ItemSubCategoryName,ItemSubCategoryDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] ItemSubCategory itemsubcategory)
        {
            if (ModelState.IsValid)
            {
                var itemSubName =
                    db.ItemSubCategories.Where(i => i.ItemSubCategoryName == itemsubcategory.ItemSubCategoryName).Select(
                        i => i.ItemSubCategoryName).FirstOrDefault();
                if (itemSubName == null)
                {
                    IdentityManager.CurrentUser(User);
                    itemsubcategory.IsDeleted = false;
                    itemsubcategory.IsActive = true;
                    itemsubcategory.IsLocked = false;
                    db.ItemSubCategories.Add(itemsubcategory);
                    db.SaveChanges();
                    TempData["Success"] = "Item Sub-Category Saved.";
                }
                else
                {
                    TempData["Error"] = "ItemSub-Category already Exists.";
                }
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemsubcategory.CreatedByUserId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(it => it.IsDeleted == false && it.IsActive == true), "ItemTypeId", "ItemTypeName");//db.ItemTypes.ToList(); //new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemsubcategory.ModifiedByUserId);
            return View(itemsubcategory);//return RedirectToAction("Index");//
        }

        // GET: /ItemSubCategory/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemSubCategory itemsubcategory = db.ItemSubCategories.Find(id);
            if (itemsubcategory == null)
            {
                return HttpNotFound();
            }
            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemsubcategory.CreatedByUserId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName", itemsubcategory.ItemTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemsubcategory.ModifiedByUserId);
            return View(itemsubcategory);
        }

        // POST: /ItemSubCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemSubCategoryId,ItemTypeId,ItemSubCategoryName,ItemSubCategoryDescription,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] ItemSubCategory itemsubcategory)
        {
            if (ModelState.IsValid)
            {
                IdentityManager.CurrentUser(User);
                itemsubcategory.IsDeleted = false;
                itemsubcategory.IsActive = true;
                itemsubcategory.IsLocked = false;

                var local = db.Set<ItemSubCategory>()
                .Local.FirstOrDefault(l => l.ItemSubCategoryId == itemsubcategory.ItemSubCategoryId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(itemsubcategory).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Info"] = "Item Sub-Category Update.";
            }
            else
            {
                TempData["Error"] = "Please Fill in missing information.";
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemsubcategory.CreatedByUserId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName", itemsubcategory.ItemTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", itemsubcategory.ModifiedByUserId);
            return View(itemsubcategory);//RedirectToAction("Index"); //
        }

        // GET: /ItemSubCategory/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemSubCategory itemsubcategory = db.ItemSubCategories.Find(id);
            if (itemsubcategory == null)
            {
                return HttpNotFound();
            }
            return View(itemsubcategory);
        }

        // POST: /ItemSubCategory/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "System Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemSubCategory itemsubcategory = db.ItemSubCategories.Find(id);
            IdentityManager.CurrentUser(User);
            itemsubcategory.IsActive = false;
            itemsubcategory.IsDeleted = true;
            db.Entry(itemsubcategory).State = EntityState.Modified;
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
