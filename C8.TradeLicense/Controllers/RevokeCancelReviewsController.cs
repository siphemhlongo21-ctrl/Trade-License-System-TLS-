using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Helpers;

namespace C8.TradeLicense.Controllers
{
    public class RevokeCancelReviewsController : Controller
    {
     
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private LicenseHelper _licenseHelper;
        public IdentityManager IdentityManager { get; set; }
        public RevokeCancelReviewsController()
        {
            _licenseHelper = new LicenseHelper(db);
        }

        /// <summary>
        /// Gets or sets the SystemUser.
        /// </summary>
        public User Users { get; set; }


        public int UserId { get; set; }

        /// <summary>
        /// The Initialise.
        /// </summary>
        /// 
       
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
      
        // GET: RevokeCancelReviews
        public ActionResult Index()
        {
            var revokeCancelReview = db.RevokeCancelReview.Include(r => r.CreatedByUser).Include(r => r.License).Include(r => r.ModifiedByUser).Include(r => r.User);
            return View(revokeCancelReview.ToList());
        }

        // GET: RevokeCancelReviews/Details/5
        public ActionResult Details(int? Id)
        {

            try
            {
                Initialise();

                LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

                License license = db.Licenses.Where(l => l.LicenseId == Id)
               .Include(l => l.Client)
               .Include(l => l.Business)
               .Include(l => l.LicenseType)
               .Include(l => l.Status)
               .Include(l => l.Region)
               .Include(l => l.ItemType)
               .Include(l => l.ItemSubCategory)
               .Include(l => l.ItemCondition)
               .FirstOrDefault();

                if (license == null)
                {
                    return HttpNotFound();
                }
          
                List<LicensesRevoked> LicensesRevoked = db.LicensesRevoked.Where(l => l.LicenseId == Id).ToList();     
                User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.LicensesRevokedList = LicensesRevoked;
                licenseApplicationDetails.AdministratorReviews = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.AdministratorReviewDetails = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.ChiefReviews = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ChiefReviewDetails = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.ManagerReviews = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ManagerReviewDetails = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.InspectionResponses = db.InspectionResponse.Where(l => l.LicenseId == Id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                licenseApplicationDetails.InspectionAppealsReviews = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviewsDetails = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();

                licenseApplicationDetails.RevokeCancelReviews = db.RevokeCancelReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();

                int Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                      .Select(d => d.DocumentTypeId)
                                      .FirstOrDefault();
                List<FileUpload> InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();





                licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;
                licenseApplicationDetails.InspectionRequestDocuments = InspectorDocs;
                licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
                licenseApplicationDetails.ItemTypeDetails = license.ItemType;
                licenseApplicationDetails.RegionDetails = license.Region;


                return View(licenseApplicationDetails);
            }
            catch (Exception e)
            {
                return View(TempData[LicenseApplicationDetails.ErrorKey]);
            }
        }

        // GET: RevokeCancelReviews/Create
        public ActionResult Create(int? Id)
        {
            Initialise();
                if (Id != null) {
                    return View(_licenseHelper.GetRevokeCancelReviewVM(Id.Value, Users.UserId, null));
                }
                else
                {
                    return View("Error");
                }              
        }

        // POST: RevokeCancelReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails)
        {          
            Initialise();
            int licenseId = licenseApplicationDetails?.LicenseDetails?.LicenseId ?? 0;
            int userId = licenseApplicationDetails?.UserDetails?.UserId ?? 0;
            RevokeCancelReview revokeCancelReview = licenseApplicationDetails.RevokeCancelReviewDetails;
            if (licenseId != 0 && revokeCancelReview != null)
            {
                _licenseHelper.AddRevokeCancellReview(licenseId, userId, revokeCancelReview);
                return RedirectToAction("Index", "ManagerReviews");
            }
            else
            {
                return View(_licenseHelper.GetRevokeCancelReviewVM(licenseId, Users.UserId, revokeCancelReview));
            }                
        }

        // GET: RevokeCancelReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevokeCancelReview revokeCancelReview = db.RevokeCancelReview.Find(id);
            if (revokeCancelReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", revokeCancelReview.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", revokeCancelReview.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", revokeCancelReview.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", revokeCancelReview.UserId);
            return View(revokeCancelReview);
        }

        // POST: RevokeCancelReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RevokeCancelReviewId,LicenseId,UserId,Decision,Comment,DateReviewed,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] RevokeCancelReview revokeCancelReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revokeCancelReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", revokeCancelReview.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", revokeCancelReview.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", revokeCancelReview.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", revokeCancelReview.UserId);
            return View(revokeCancelReview);
        }

        // GET: RevokeCancelReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevokeCancelReview revokeCancelReview = db.RevokeCancelReview.Find(id);
            if (revokeCancelReview == null)
            {
                return HttpNotFound();
            }
            return View(revokeCancelReview);
        }

        // POST: RevokeCancelReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RevokeCancelReview revokeCancelReview = db.RevokeCancelReview.Find(id);
            db.RevokeCancelReview.Remove(revokeCancelReview);
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
