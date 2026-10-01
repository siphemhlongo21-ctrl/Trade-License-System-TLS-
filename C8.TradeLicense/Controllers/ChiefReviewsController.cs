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
using PagedList;
using Microsoft.AspNet.Identity;
using C8.TradeLicense.Keys;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using Microsoft.AspNet.Identity.EntityFramework;

namespace C8.TradeLicense.Controllers
{
    public class ChiefReviewsController : Controller
    {

        private CesarDbContext core = new CesarDbContext();
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
        // GET: ChiefReviews

        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin" + "," + "Chief Inspector")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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
                //
            }

            ViewBag.CurrentFilter = inputSearch;
            ViewBag.selecetedSearch = selectedSearch;

            int pendingReview = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingChiefReview).Select(s => s.StatusId)
                    .FirstOrDefault();

            var Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.Status.StatusName).ToList();
            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        Licenses = Licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number

                        Licenses = Licenses.Where(l => l.LicenseNumber == inputSearch).ToList();


                        //licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        Licenses = Licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        Licenses = Licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                }
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == pendingReview).OrderBy(c => c.StatusName), "StatusId", "StatusName");

            #region CanAction
            //ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            //if (Request.HttpMethod != "GET")
            //{
            //    Page_No = 1;
            //}
            #endregion
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.LicenseCount = Licenses.Count();
            ViewBag.Licenses = Licenses;
            return View(Licenses.ToPagedList(pageNumber, pageSize));
        }

        // GET: ChiefReviews/Details/5
        public ActionResult Details(int? id)
        {
            try
            {

                License license = db.Licenses.Where(l => l.LicenseId == id)
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
                ChiefReview ChiefReview = db.ChiefReview.Where(l => l.LicenseId == id).OrderByDescending(o => o.ChiefReviewId).FirstOrDefault();

                var UserId = db.Users.Where(u => u.UserId == ChiefReview.UserId).FirstOrDefault();
                ViewData["Userdata"] = UserId;
                ViewData["BusinessData"] = license.Business;
                ViewData["ClientData"] = license.Client;
                ViewData["licenceData"] = license;
                ViewData["AppealsReviews"] = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["adminpreviousReview"] = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["ChiefpreviousReview"] = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["ManagerpreviousReview"] = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["InspectionResponse"] = db.InspectionResponse.Where(l => l.LicenseId == id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
                ViewData["LicenseType"] = license.LicenseType;
                ViewData["ItemType"] = license.ItemType;
                ViewData["Region"] = license.Region;
                ViewData["DepartmentRequest"] = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();



                var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                          .Select(d => d.DocumentTypeId)
                                                          .FirstOrDefault();
                var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();


                ViewData["InspectorDocs"] = InspectorDocs;
                var InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                    .Select(d => d.DocumentTypeId)
                                    .FirstOrDefault();
                ViewData["inspectionUploadList"] = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId && d.IsDeleted == false && d.IsActive).ToList();

                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.InspectionRequests = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");


                return View(ChiefReview);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        // GET: ChiefReviews/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin" + "," + "Chief Inspector")]
        public ActionResult Create(int id)
        {
            try
            {
                Initialise();

                License license = db.Licenses.Where(l => l.LicenseId == id)
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
                //If not pending Chief review, redirect to details page
                Status pendingChiefReviewStatus = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingChiefReview).FirstOrDefault();
                if (pendingChiefReviewStatus != null && license.StatusId != pendingChiefReviewStatus.StatusId)
                    return RedirectToAction("ApplicationTrackerDetails", "License", new { id });

                LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
                User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();

                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
                licenseApplicationDetails.ItemTypeDetails = license.ItemType;
                licenseApplicationDetails.RegionDetails = license.Region;

                licenseApplicationDetails.InspectionResponses = db.InspectionResponse.Where(l => l.LicenseId == id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                licenseApplicationDetails.ChiefReviews = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviews = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviewsDetails = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.AdministratorReviews = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.AdministratorReviewDetails = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.ManagerReviews = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionRequests = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
                licenseApplicationDetails.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();


                int? Inspectordocument = db.DocumentTypes?
                                       .Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument &&
                                       dt.IsDeleted == false && dt.IsActive)
                                       .Select(d => d.DocumentTypeId)
                                       .FirstOrDefault();

                IEnumerable<FileUpload> InspectorDocs = db.FileUploads.Include(d => d.Document)
                                                        .Where(d => d.ClientId == license.ClientId &&
                                                        d.Document.DocumentTypeId == Inspectordocument &&
                                                        d.IsDeleted == false && d.IsActive).ToList();

                licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;

                int? InspectoionrequestdocumentTypeId = db.DocumentTypes?
                                                        .Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument &&
                                                        dt.IsDeleted == false && dt.IsActive)
                                                        .Select(d => d.DocumentTypeId)
                                                        .FirstOrDefault();

                licenseApplicationDetails.InspectionRequestUploadList = db.FileUploads.Include(d => d.Document)
                                                                         .Where(d => d.ClientId == license.ClientId &&
                                                                         d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId &&
                                                                         d.IsDeleted == false &&
                                                                         d.IsActive).ToList();
                licenseApplicationDetails.DepartmentCirculationHistoryVM = new ViewModels.DepartmentCirculationHistoryVM
                {
                    DepartmentContacts = licenseApplicationDetails.DepartmentContacts,
                    InspectionRequests = licenseApplicationDetails.InspectionRequests,
                    InspectionRequestUploadList = licenseApplicationDetails.InspectionRequestUploadList
                };
                return View(licenseApplicationDetails);
            }
            catch (Exception e)
            {
                return View(TempData[LicenseApplicationDetails.ErrorKey]);
            }
        }


        // POST: ChiefReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin" + "," + "Chief Inspector")]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails, string Userid)
        {
            Initialise();
            ChiefReview chiefReview = licenseApplicationDetails?.ChiefReviewDetails ?? null;
            int licenseId = licenseApplicationDetails?.LicenseDetails?.LicenseId ?? 0;
            // int licenseId = licenseApplicationDetails?.LicenseDetails?.LicenseId ?? 0;
            License license = db.Licenses.Where(l => l.LicenseId == licenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition)
.FirstOrDefault();
            if (ModelState.IsValid)
            {
                chiefReview.UserId = Int32.Parse(Userid);
                chiefReview.LicenseId = licenseId;

                chiefReview.IsActive = true;
                chiefReview.IsDeleted = false;
                chiefReview.IsLocked = false;
                db.ChiefReview.Add(chiefReview);
                db.SaveChanges();
                if (chiefReview.Decision == TLKeys.NotRecommended)
                {
                    license.StatusId =
db.Status.Where(s => s.StatusKey == StatusKeys.ChiefRejected).Select(s => s.StatusId).
FirstOrDefault();
                    ///
                    try
                    {

                        #region Send Email To Clerk
                        var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                        var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                        var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                        var body = String.Empty;
                        var admins = db.Users.Where(c => c.Role == "Licensing Clerk" && c.Region == license.RegionId).ToList();

                        //LF2014a 
                        //User email helper to get well formatted email.
                        //var mail = new EmailHelper();


                        foreach (var admin in admins)
                        {
                            if (template != null)
                            {
                                body = template.EmailBody;
                            }
                            //L.M.20150303a - Replace variables with actual email content
                            var user = admin;
                            body = body.Replace("#NAME#", user.FullName);
                            body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Chief Inspector for: </b><br/><br/> Business Name: " +
                               license.Business.ProposedTradeName + "<br/> " +
                                "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = user.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "eThekwini Trade Licensing - Application Rejected:" + license.LicenseNumber,
                                Body = body,
                                IsHtml = true,
                                FailureCount = 0,
                                ReferenceId = user.UserId.ToString(),
                                HasAttachments = false
                            };

                            core.tb_EmailQueue.Add(email);
                        }

                        core.SaveChanges();
                        #endregion Send Email To Admin
                    }
                    catch (Exception e)
                    {
                        var error = e.InnerException;
                    }
                }
                else
                {
                    license.StatusId =
    db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerReview).Select(s => s.StatusId).
    FirstOrDefault();
                }
                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();

                ///
                try
                {

                    #region Send Email To Manager
                    var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                    var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                    var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                    var body = String.Empty;
                    var admins = db.Users.Where(c => c.Role == "Licensing Manager" && c.IsActive).ToList();

                    //LF2014a 
                    //User email helper to get well formatted email.
                    //var mail = new EmailHelper();

                    string actionLink = "<a href=" + Request.Url.Authority + ActionLinkKeys.DepartmentManagerFinalCreate + license.LicenseId + ">Click here</a>";
                    foreach (var admin in admins)
                    {
                        if (template != null)
                        {
                            body = template.EmailBody;
                        }
                        //L.M.20150303a - Replace variables with actual email content
                        var user = admin;
                        body = body.Replace("#NAME#", user.FullName);
                        body = body.Replace("#BODYTEXT#", " <b>Application Awaiting Review. </b><br/><br/> Business Name: " +
                           license.Business.ProposedTradeName + "<br/> " +
                            "<br/> " + actionLink + " to Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = 5,//Hard coded
                            ToList = user.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "eThekwini Trade Licensing - Application for Feedback:" + license.LicenseNumber,
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = user.UserId.ToString(),
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                    }

                    core.SaveChanges();
                    #endregion Send Email To Admin
                }
                catch (Exception e)
                {
                    var error = e.InnerException;
                }
                return RedirectToAction("Index");
            }
            else
            {

                if (license == null)
                {
                    return HttpNotFound();
                }


                var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                ViewData["Userdata"] = UserId;
                ViewData["BusinessData"] = license.Business;
                ViewData["ClientData"] = license.Client;
                ViewData["licenceData"] = license;
                ViewData["DepartmentRequest"] = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
                ViewData["AppealsReviews"] = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["adminpreviousReview"] = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["ChiefpreviousReview"] = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["ManagerpreviousReview"] = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["InspectionResponse"] = db.InspectionResponse.Where(l => l.LicenseId == license.LicenseId).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
                ViewData["LicenseType"] = license.LicenseType;
                ViewData["ItemType"] = license.ItemType;
                ViewData["Region"] = license.Region;

                var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                          .Select(d => d.DocumentTypeId)
                                                          .FirstOrDefault();
                var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();

                ViewData["InspectorDocs"] = InspectorDocs;
                var InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                    .Select(d => d.DocumentTypeId)
                                    .FirstOrDefault();
                ViewData["inspectionUploadList"] = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId && d.IsDeleted == false && d.IsActive).ToList();
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", chiefReview.CreatedByUserId);
                ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", chiefReview.LicenseId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", chiefReview.ModifiedByUserId);
                ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", chiefReview.UserId);
                return View(chiefReview);
            }
        }

        // GET: ChiefReviews/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin" + "," + "Chief Inspector")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiefReview chiefReview = db.ChiefReview.Find(id);
            if (chiefReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", chiefReview.LicenseId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", chiefReview.UserId);
            return View(chiefReview);
        }

        // POST: ChiefReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin" + "," + "Chief Inspector")]
        public ActionResult Edit([Bind(Include = "ChiefReviewId,LicenseId,UserId,Decision,Comment,DateReviewed")] ChiefReview chiefReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiefReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", chiefReview.LicenseId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", chiefReview.UserId);
            return View(chiefReview);
        }

        // GET: ChiefReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiefReview chiefReview = db.ChiefReview.Find(id);
            if (chiefReview == null)
            {
                return HttpNotFound();
            }
            return View(chiefReview);
        }

        // POST: ChiefReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiefReview chiefReview = db.ChiefReview.Find(id);
            db.ChiefReview.Remove(chiefReview);
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
