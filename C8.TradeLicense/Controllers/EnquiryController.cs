using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity;
using C8.TradeLicense.DataAccessLayer;

namespace C8.TradeLicense.Controllers
{
    public class EnquiryController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        public EnquiryController()
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

        // GET: /Enquiry/
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            try
            {
                if (inputSearch != null)
                {
                    page = 1;
                    searchCriteria = selectedSearch;
                }
                else
                {
                    inputSearch = currentFilter;
                    if (currentFilter != null)
                    {
                        searchCriteria = selectedSearch;
                    }
                }

                int departmentID = DepartmentID();
                List<Enquiry> enquiries = new List<Enquiry>();
                enquiries = db.Enquiries.Where(e => e.IsActive == true && e.IsDeleted == false && e.DepartmentId == departmentID)
                    .Include(e => e.CreatedByUser)
                    .Include(e => e.ModifiedByUser)
                    .Include(e => e.LicenseType)
                    .Include(e => e.Department).ToList();

                if (selectedSearch != null)
                {
                    switch (selectedSearch)
                    {
                        case "ClientName":
                            //Search by client name
                            enquiries = enquiries.Where(i => (i.ClientName.ToLower()).Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "BusinessName":
                            //Search by business name
                            enquiries = enquiries.Where(i => i.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                            break;
                        default:
                            enquiries = enquiries.ToList();
                            break;
                    }
                }

                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    page = 1;
                }

                int pageNumber = page ?? 1;
                int pageSize = 5;
                ViewBag.CurrentFilter = inputSearch;
                ViewBag.selecetedSearch = selectedSearch;
                ViewBag.EnquiriesCount = enquiries.Count;
                //ViewBag.DepartmentId = department.DepartmentId;
                return View(enquiries.ToPagedList(pageNumber,pageSize));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: /Enquiry/Details/5
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Details(int id)
        {
            try
            {
                int deptId = DepartmentID();
                Enquiry enquiry = null;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                enquiry = db.Enquiries.Where(e => e.EnquiryId == id && e.DepartmentId == deptId)
                    .Where(e => e.IsActive == true && e.IsDeleted == false)
                    .Include(e => e.CreatedByUser)
                    .Include(e => e.ModifiedByUser)
                    .Include(e => e.LicenseType)
                    .Include(e => e.Department)
                    .FirstOrDefault();
                //enquiry = db.Enquiries.Where(e => e.EnquiryId == id).Where(e => e.IsActive == true && e.IsDeleted == false).Include(e => e.CreatedByUser).Include(e => e.ModifiedByUser).Include(e => e.LicenseType);
                if (enquiry == null)
                {
                    return HttpNotFound();
                }
                return View(enquiry);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: /Enquiry/Create
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Create()
        {
            try
            {
                TempData["Success"] = null;
                TempData["Error"] = null;

                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName), "LicenseTypeId", "LicenseTypeName");
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // POST: /Enquiry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "EnquiryId,ClientName,ProposedTradeName,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,EnquiryReport,InspectorsReport,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,LicenseTypeId")] Enquiry enquiry)
        {
            try
            {
                int deptId = DepartmentID();
                if (ModelState.IsValid)
                {

                    IdentityManager.CurrentUser(User);
                    enquiry.DepartmentId = deptId;
                    enquiry.IsActive = true;
                    enquiry.IsDeleted = false;
                    enquiry.IsLocked = false;
                    db.Enquiries.Add(enquiry);
                    db.SaveChanges();

                    TempData["Success"] = "Enquiry saved Successfully!";
                }
                else
                {
                    TempData["Error"] = "Please enter data!";
                }

                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName), "LicenseTypeId", "LicenseTypeName", enquiry.LicenseTypeId);
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", enquiry.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", enquiry.ModifiedByUserId);
                return View(enquiry);
            }
            catch(Exception)
            {
                return View("Error");
            }
        }

        // GET: /Enquiry/Edit/5
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            try
            {
                TempData["Success"] = null;
                TempData["Error"] = null;
                int deptId = DepartmentID();
                Enquiry enquiry = null;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                enquiry = db.Enquiries.Where(e => e.EnquiryId == id && e.DepartmentId == deptId)
                   .Where(e => e.IsActive == true && e.IsDeleted == false)
                   .Include(e => e.CreatedByUser)
                   .Include(e => e.ModifiedByUser)
                   .Include(e => e.LicenseType)
                   .Include(e => e.Department)
                   .FirstOrDefault();
                if (enquiry == null)
                {
                    return HttpNotFound();
                }
                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName)
                    , "LicenseTypeId", "LicenseTypeName", enquiry.LicenseTypeId);
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", enquiry.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", enquiry.ModifiedByUserId);
                ViewBag.EnquiryId = id;
                return View(enquiry);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // POST: /Enquiry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "EnquiryId,ClientName,ProposedTradeName,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,EnquiryReport,InspectorsReport,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,LicenseTypeId")] Enquiry enquiry)
        {
            try
            {
                int deptId = DepartmentID();

                if (ModelState.IsValid)
                {
                    IdentityManager.CurrentUser(User);
                    enquiry.IsActive = true;
                    enquiry.IsDeleted = false;
                    enquiry.IsLocked = false;
                    enquiry.DepartmentId = deptId;

                    var local = db.Set<Enquiry>()
                    .Local.FirstOrDefault(l => l.EnquiryId == enquiry.EnquiryId);

                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;
                    }

                    db.Entry(enquiry).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Enquiry Successfully Updated!";
                }
                else
                {
                    TempData["Error"] = "Please enter data!";
                }

                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName),
                    "LicenseTypeId", "LicenseTypeName", enquiry.LicenseTypeId);
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", enquiry.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", enquiry.ModifiedByUserId);
                return View(enquiry);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: /Enquiry/Delete/5
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            try
            {
                TempData["Error"] = null;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Enquiry enquiry = db.Enquiries.Find(id);
                if (enquiry == null)
                {
                    return HttpNotFound();
                }
                return View(enquiry);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // POST: /Enquiry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Enquiry enquiry = db.Enquiries.Find(id);
                IdentityManager.CurrentUser(User);
                enquiry.IsActive = false;
                enquiry.IsDeleted = true;
                enquiry.IsLocked = false;
                db.Entry(enquiry).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Error"] = "Enquiry Successfully Removed!";
                //return RedirectToAction("Index");
                return View(enquiry);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        /// <summary>
        /// Returns the DepartmentId the logged in user is assigned too
        /// TG20150302
        /// </summary>
        /// <returns></returns>
        public int DepartmentID()
        {
            IdentityManager identifyManager = new IdentityManager();
            var currentUserGuidId = User.Identity.GetUserId();
            var currentUser = identifyManager.CurrentUser(currentUserGuidId);
            var department = db.DepartmentContacts.FirstOrDefault(d => d.UserId == currentUser.UserId);

            return department.DepartmentId;
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
