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
using Microsoft.AspNet.Identity;
using PagedList;
using Rotativa;
using C8.TradeLicense.Keys;

namespace C8.TradeLicense.Controllers
{
    public class InspectionRequestController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public IdentityManager IdentityManager { get; set; }


        /// <summary>
        /// Gets or sets the SystemUser.
        /// </summary>
        public User Users { get; set; }


        public int UserId { get; set; }

        /// <summary>
        /// The Initialise.
        /// </summary>
        private void Initialise()
        {
            using (var context = new TradeLicenseDbContext())
            {
                try
                {
                    IdentityManager = new IdentityManager(context);

                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        IdentityManager.CurrentUser(User);
                        Users = IdentityManager.CurrentUser(User);
                    }

                    if (Users != null)
                    {
                        Users =
                            context.Users.Where(o => o.UserId == Users.UserId)
                                .FirstOrDefault();



                    }


                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        #region InspectionRequest Index
        // GET: /InspectionRequest/
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            try
            {
                Initialise();
                //TG20150309a.
                //Resolved Paging issue by retaining search criteria's
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

                ViewBag.CurrentFilter = inputSearch;
                ViewBag.selecetedSearch = selectedSearch;

                var department = db.DepartmentContacts.FirstOrDefault(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false);
                List<InspectionRequest> inspectionrequests = new List<InspectionRequest>();

                if (department != null)
                {
                    inspectionrequests = db.InspectionRequests.Include(l => l.License).Include(l => l.License.Business).Include(l => l.Department).Include(l => l.LicenseType).Where(s => s.IsDeleted == false && s.IsActive == true && s.DepartmentId == department.DepartmentId).ToList();
                    ViewBag.DepartmentId = department.DepartmentId;
                    ViewBag.InspectionRequestCount = inspectionrequests.Count;
                    //return View(inspectionrequests.ToPagedList(pageNumber, itemsPerPage));
                }
                else if (User.IsInRole("Administrator")) //  
                {
                    inspectionrequests = db.InspectionRequests.Include(l => l.License).Include(l => l.License.Business).Include(l => l.Department).Include(l => l.LicenseType).Where(s => s.IsDeleted == false && s.IsActive == true && s.Status.StatusKey != StatusKeys.InspectionApproved).ToList();
                    ViewBag.InspectionRequestCount = inspectionrequests.Count;
                    //return View(inspectionrequests.ToPagedList(pageNumber, itemsPerPage));
                }

                if (selectedSearch != null)
                {
                    switch (selectedSearch)
                    {
                        case "ClientName":
                            //Search by client name
                            inspectionrequests = inspectionrequests.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "ReferenceNo":
                            //Search by reference number
                       
                                inspectionrequests = inspectionrequests.Where(l => l.License.LicenseNumber == inputSearch).ToList();
                                          
                            break;
                        case "BusinessName":
                            //Search by business name
                            inspectionrequests = inspectionrequests.Where(l => l.License.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                            break;
                        default:
                            inspectionrequests = inspectionrequests.ToList();
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
                ViewBag.InspectionRequestCount = inspectionrequests.Count;
                return View(inspectionrequests.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        



        #endregion

        #region InspectionRequest Details
        // GET: /InspectionRequest/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InspectionRequest inspectionrequest = db.InspectionRequests.Find(id);
                if (inspectionrequest == null)
                {
                    return HttpNotFound();
                }
                return View(inspectionrequest);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionRequest Create(GET)
        // GET: /InspectionRequest/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionRequest Create(POST)
        // POST: /InspectionRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "InspectionRequestId,OverrideOrderIndex,LicenseTypeId,DepartmentId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] InspectionRequest inspectionrequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inspectionrequest.IsActive = true;
                    inspectionrequest.IsDeleted = false;
                    inspectionrequest.IsLocked = false;
                    db.InspectionRequests.Add(inspectionrequest);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionrequest.CreatedByUserId);
                ViewBag.DocumentTypeId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", inspectionrequest.DepartmentId);
                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", inspectionrequest.LicenseTypeId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionrequest.ModifiedByUserId);
                return View(inspectionrequest);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        // GET: /InspectionResponse/ApproveLicense
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Licensing Clerk")]
        public ActionResult Outstanding(int? id)
        {
            try
            {


                var outstanding = db.InspectionRequests.Where(i=> i.LicenseId == id && i.StatusId !=18).Include(l => l.Department).Include(l => l.Client).Include(l => l.CreatedByUser).Include(l => l.LicenseType).Include(l => l.ModifiedByUser).Include(l => l.Status);
                
               





                ViewBag.LicenseInspectionCount = outstanding.Count();
                return View(outstanding);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        #region InspectionRequest Edit(GET)
        // GET: /InspectionRequest/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InspectionRequest inspectionrequest = db.InspectionRequests.Find(id);
                if (inspectionrequest == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionrequest.CreatedByUserId);
                ViewBag.DocumentTypeId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", inspectionrequest.DepartmentId);
                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", inspectionrequest.LicenseTypeId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionrequest.ModifiedByUserId);

                return View(inspectionrequest);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionRequest Edit(POST)
        // POST: /InspectionRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "InspectionRequestId,OverrideOrderIndex,LicenseTypeId,DepartmentId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] InspectionRequest inspectionrequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var local = db.Set<InspectionRequest>()
                    .Local.FirstOrDefault(l => l.InspectionRequestId == inspectionrequest.InspectionRequestId);

                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;
                    }

                    db.Entry(inspectionrequest).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionrequest.CreatedByUserId);
                ViewBag.DocumentTypeId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", inspectionrequest.DepartmentId);
                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", inspectionrequest.LicenseTypeId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionrequest.ModifiedByUserId);
                return View(inspectionrequest);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionRequest Delete(GET)
        // GET: /InspectionRequest/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "DepartmentInspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InspectionRequest inspectionrequest = db.InspectionRequests.Find(id);
                if (inspectionrequest == null)
                {
                    return HttpNotFound();
                }
                return View(inspectionrequest);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionRequest Delete(POST)
        // POST: /InspectionRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                InspectionRequest inspectionrequest = db.InspectionRequests.Find(id);
                var deleteInspection = db.InspectionRequests.Where(m => m.InspectionRequestId == id && m.IsActive == true && m.IsDeleted == false);
                inspectionrequest.IsDeleted = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Generate Inspection Report
        public ActionResult GenerateInspectionReport(int id)
        {
            try
            {
                InspectionRequest inspectionrequest = db.InspectionRequests.Find(id);
                var license = db.Licenses.Find(inspectionrequest.LicenseId);
                var licenseType = db.LicenseTypes.Find(license.LicenseTypeId);
                var business = db.Businesses.Find(license.BusinessId);
                var client = db.Clients.Find(license.ClientId);
                var licenseItemCategory = db.ItemTypes.Find(license.ItemTypeId);
                var department = db.Departments.Find(inspectionrequest.DepartmentId);

                ViewData["License"] = license;
                ViewData["LicenseType"] = licenseType;
                ViewData["Business"] = business;
                ViewData["Client"] = client;
                ViewData["ItemTypes"] = licenseItemCategory;

                ViewBag.Department = department.DepartmentName.ToUpper();
                ViewBag.LicenseTypeAmount = licenseType.Amount.ToString("0.00").Replace(".00","");
                ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");

                return new Rotativa.PartialViewAsPdf("_InspectionReport")
                {
                    FileName = "Inspection_Report.pdf"
                };
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Validate Authority
        /// <summary>
        /// Checks if currect have permissions to action 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "Licensing Clerk" + "," + "System Admin")]
        [HttpPost]
        public bool CanAction()
        {
            bool canAction = false;
            try
            {
                if ((User.IsInRole("Licensing Administrator")) || (User.IsInRole("Licensing Manager")) || (User.IsInRole("Licensing Clerk")))
                {
                    canAction = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return canAction;
        }

        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
