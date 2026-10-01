using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.DataAccessLayer;
using System.Globalization;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity;
using C8.TradeLicense.DAL;
using PagedList;
using C8.TradeLicense.Keys;
using C8.TradeLicense.DataAccessLayer.CesarDb;

namespace C8.TradeLicense.Controllers
{
    public class InspectionAppealsReviewsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private CesarDbContext core = new CesarDbContext();
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

        // GET: InspectionAppealsReviews
        [Authorize(Roles = "Licensing Manager" + "," + "Licensing Clerk" + "," + "System Admin")]
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

            int pendingReview = db.Status.Where(s => s.StatusKey == StatusKeys.AppealReview).Select(s => s.StatusId)
                    .FirstOrDefault();
         
            var Appeal = db.InspectionAppeal.Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.Status).Include(l => l.License.LicenseType).Include(l => l.License.Client).Where(l => l.StatusId == pendingReview && l.License.RegionId == Users.Region).OrderBy(l => l.Status.StatusName).ToList();
            if (User.IsInRole("System Admin"))
            {

               Appeal = db.InspectionAppeal.Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.Status).Include(l => l.License.LicenseType).Include(l => l.License.Client).Where(l => l.StatusId == pendingReview).OrderBy(l => l.Status.StatusName).ToList();
            }
            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        Appeal = Appeal.Where(l => (l.License.Client.Name.ToLower() + " " + l.License.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number
                        int n;
                        bool isNumeric = int.TryParse(inputSearch, out n);
                        if (isNumeric)
                        {
                            Appeal = Appeal.Where(l => l.License.LicenseId == Convert.ToInt32(inputSearch)).ToList();
                        }
                        else
                        {
                            Appeal = Appeal.Where(l => l.License.LicenseId == 0).ToList();
                        }

                        //licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        Appeal = Appeal.Where(l => l.License.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        Appeal = Appeal.Where(l => l.License.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
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

            ViewBag.LicenseCount = Appeal.Count();
            ViewBag.Licenses = Appeal;
            return View(Appeal.ToPagedList(pageNumber, pageSize));
        }

        // GET: InspectionAppealsReviews/Details/5
        [Authorize(Roles = "Licensing Manager" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            try
            {
                Initialise();
                var InspectionResponse = db.InspectionResponse.Where(l => l.InspectionRequestId == id).Select(s => s.InspectionResponseId).FirstOrDefault();


                var InspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponseId == InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.Status).Include(l => l.License).FirstOrDefault();



                if (InspectionAppeal == null)
                {
                    return HttpNotFound();
                }
                InspectionAppealsReviews apppealreview = db.InspectionAppealsReviews.Where(l => l.InspectionAppealId == InspectionAppeal.InspectionAppealId).FirstOrDefault();

                License license = db.Licenses.Where(l => l.LicenseId == InspectionAppeal.LicenseId)
               .Include(l => l.Client)
               .Include(l => l.Business)
               .Include(l => l.LicenseType)
               .Include(l => l.Status)
               .Include(l => l.Region)
               .Include(l => l.ItemType)
               .Include(l => l.ItemSubCategory)
               .Include(l => l.ItemCondition)



               .FirstOrDefault();
                var documentTypeId2 = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Refusalsign && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Appealdocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentTypeId == documentTypeId2 && d.IsActive == true && d.IsDeleted == false);
                int documentId1 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false).Select(s => s.DocumentId).FirstOrDefault();
                int documentId2 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId2 && d.IsActive == true && d.IsDeleted == false).Select(s => s.DocumentId).FirstOrDefault();
                var uploadedFiles = db.FileUploads.Where(f => f.ClientId == InspectionAppeal.License.ClientId && f.IsActive == true && f.IsDeleted == false && (f.DocumentId == documentId2 || f.DocumentId == documentId1)).Include(f => f.Document).ToList();
                var InspectionHistory = db.InspectionHistory.Where(l => l.InspectionRequestId == InspectionAppeal.InspectionResponse.InspectionRequestId).Include(l => l.Status).ToList();
                ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
                var UserId = db.Users.Where(u => u.UserId == apppealreview.UserId).FirstOrDefault();
                ViewData["BusinessData"] = license.Business;
                ViewData["ClientData"] = license.Client;
                ViewData["licenceData"] = license;
                ViewData["Documents"] = document;
                ViewData["UploadList"] = uploadedFiles;
                ViewData["Userdata"] = UserId;
                ViewData["AppealData"] = InspectionAppeal;
                ViewData["InspectionResponse"] = db.InspectionResponse.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
                ViewData["AppealsReviews"] = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                ViewData["adminpreviousReview"] = db.AdministratorReviews.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                ViewData["ChiefpreviousReview"] = db.ChiefReview.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                ViewData["ManagerpreviousReview"] = db.ManagerRevies.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
              
                ViewBag.InspectionHistory = InspectionHistory;

                var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                          .Select(d => d.DocumentTypeId)
                                                          .FirstOrDefault();
                var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == InspectionAppeal.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();





                ViewData["InspectorDocs"] = InspectorDocs;

                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.InspectionRequests = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");


                return View(apppealreview);
            }
            catch (Exception e)
            {
                return View("Error");
            }
       
        }

        // GET: InspectionAppealsReviews/Create
        [Authorize(Roles = "Licensing Manager" + "," + "Licensing Clerk" + "," + "System Admin")]
        public ActionResult Create(int id)
        {
            try
            {
                Initialise();

                LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
                InspectionAppeal InspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionAppealId == id).Include(l => l.InspectionResponse.InspectionRequest).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.Status).Include(l => l.License).FirstOrDefault();
                User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();

                if (InspectionAppeal == null)
                {
                    return HttpNotFound();
                }
                int? documentTypeId2 = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Refusalsign && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                int? documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Appealdocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                IQueryable<Document> document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentTypeId == documentTypeId2 && d.IsActive == true && d.IsDeleted == false);
                int? documentId1 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false).Select(s => s.DocumentId).FirstOrDefault();
                int? documentId2 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId2 && d.IsActive == true && d.IsDeleted == false).Select(s => s.DocumentId).FirstOrDefault();
                List<FileUpload> uploadedFiles = db.FileUploads.Where(f => f.ClientId == InspectionAppeal.License.ClientId && f.IsActive == true && f.IsDeleted == false && (f.DocumentId == documentId2 || f.DocumentId == documentId1)).Include(f => f.Document).ToList();
                List<InspectionHistory> InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == InspectionAppeal.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionAppeal.InspectionResponse.InspectionRequest.DepartmentId).ToList();
                        
                licenseApplicationDetails.ClientUploadList = uploadedFiles;
                licenseApplicationDetails.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();
                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.InspectionAppealDetails = InspectionAppeal;
                licenseApplicationDetails.InspectionAppealsReviews = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviewsDetails= db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.AdministratorReviews = db.AdministratorReviews.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.AdministratorReviewDetails = db.AdministratorReviews.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.ChiefReviewDetails = db.ChiefReview.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.ChiefReviews= db.ChiefReview.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ManagerReviews = db.ManagerRevies.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ManagerReviewDetails = db.ManagerRevies.Where(l => l.LicenseId == InspectionAppeal.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.InspectionHistorys = InspectionHistory;

                int? Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                          .Select(d => d.DocumentTypeId)
                                          .FirstOrDefault();

                List<FileUpload> InspectorDocs = db.FileUploads.Include(d => d.Document)
                                                .Where(d => d.ClientId == InspectionAppeal.License.ClientId &&
                                                d.Document.DocumentTypeId == Inspectordocument &&
                                                d.IsDeleted == false && d.IsActive).ToList();

                licenseApplicationDetails.InspectionRequestDocuments = InspectorDocs;
                licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;

                return View(licenseApplicationDetails);
            }
            catch (Exception e)
            {
                return View(TempData[LicenseApplicationDetails.ErrorKey]);
            }
        }

        // POST: InspectionAppealsReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Manager" + "," + "Licensingt Clerk" + "," + "System Admin")]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails , string AppealDateTime)
        {
            Initialise();
            InspectionAppealsReviews inspectionAppealsReviews = licenseApplicationDetails.InspectionAppealsReviewsDetails;
            var InspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionAppealId == inspectionAppealsReviews.InspectionAppealId)
                .Include(l => l.InspectionResponse.InspectionRequest).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.Status)
                .Include(l => l.License).FirstOrDefault();

            if (inspectionAppealsReviews.Decision != null) 
            {              
                InspectionAppeal.AppealDateTime =DateTime.Parse(AppealDateTime);
                if (inspectionAppealsReviews.Decision == TLKeys.Approve)
                {
                        InspectionAppeal.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AppealApproved).Select(s => s.StatusId).FirstOrDefault();
                        InspectionAppeal. InspectionResponse.InspectionRequest.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AppealApproved).Select(s => s.StatusId).FirstOrDefault();
                        db.Entry(InspectionAppeal).State = EntityState.Modified;
                        db.SaveChanges();
                        try
                        {
                            License license = db.Licenses.Where(l => l.LicenseId == InspectionAppeal.License.LicenseId)
                            .Include(l => l.Client)
                            .Include(l => l.Business)
                            .Include(l => l.LicenseType)
                            .Include(l => l.Status)
                            .Include(l => l.Region)
                            .Include(l => l.ItemType)
                            .Include(l => l.ItemSubCategory)
                            .Include(l => l.ItemCondition)
                            .FirstOrDefault();
                            #region Send Email To Clerk
                        var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                        var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                        var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;

                        var body = String.Empty;
                        var admins = db.Users.Where(c => c.Role == "Licensing Clerk" && c.Region == license.RegionId).ToList();

                        //LF2014a 
                        //User email helper to get well formatted email.
                        //var mail = new EmailHelper();

                        string actionLink = "<a href=" + Request.Url.Authority + ActionLinkKeys.ViewAppealDetails + InspectionAppeal.InspectionResponse.InspectionRequestId + ">Click here</a>";
                        foreach (var admin in admins)
                        {
                            if (template != null)
                            {
                                body = template.EmailBody;
                            }
                            //L.M.20150303a - Replace variables with actual email content
                            var user = admin;
                            body = body.Replace("#NAME#", user.FullName);
                            body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Manager for: </b><br/><br/> Business Name: " +
                               license.Business.ProposedTradeName + "<br/> " +
                                "<br/> "+ actionLink + " to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = user.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "eThekwini Trade Licensing - Appeal Reviewed:" + license.LicenseNumber,
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
                    InspectionAppeal.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AppealRejected).Select(s => s.StatusId).FirstOrDefault();
                    InspectionAppeal.InspectionResponse.InspectionRequest.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AppealRejected).Select(s => s.StatusId).FirstOrDefault();
                    db.Entry(InspectionAppeal).State = EntityState.Modified;
                    db.SaveChanges();
                    try
                    {
                        License license = db.Licenses.Where(l => l.LicenseId == InspectionAppeal.License.LicenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition).FirstOrDefault();
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
                            body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Manager for: </b><br/><br/> Business Name: " +
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
                                Subject = "eThekwini Trade Licensing - Appeal Rejected:" + license.LicenseNumber,
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
                db.InspectionAppealsReviews.Add(inspectionAppealsReviews);
                db.SaveChanges();                
                return RedirectToAction("Index", "InspectionAppeals");
            }

        



            if (InspectionAppeal == null)
            {
                return HttpNotFound();
            }

            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Appealdocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);

            var uploadedFiles = db.FileUploads.Where(f => f.ClientId == InspectionAppeal.License.ClientId && f.referenceId == InspectionAppeal.InspectionAppealId && f.IsActive == true && f.IsDeleted == false).Include(f => f.Document).ToList();
            var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == InspectionAppeal.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionAppeal.InspectionResponse.InspectionRequest.DepartmentId).ToList();
            var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                           .Select(d => d.DocumentTypeId)
                                           .FirstOrDefault();
            var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == InspectionAppeal.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();


            ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();


            ViewData["InspectorDocs"] = InspectorDocs;
            var UserId = db.Users.Where(u => u.UserId== Users.UserId).FirstOrDefault();
            ViewData["Documents"] = document;
            ViewData["UploadList"] = uploadedFiles;
            ViewData["Userdata"] = UserId;
            ViewData["AppealData"] = InspectionAppeal;
            ViewBag.InspectionHistory = InspectionHistory;


            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.CreatedByUserId);
            ViewBag.InspectionAppealId = new SelectList(db.InspectionAppeal, "InspectionAppealId", "IdentityOrPassportNumber", inspectionAppealsReviews.InspectionAppealId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.UserId);
            return View(inspectionAppealsReviews);
        }

        // GET: InspectionAppealsReviews/Edit/5
        [Authorize(Roles = "Licensing Manager" + "," + "Licensingt Clerk" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectionAppealsReviews inspectionAppealsReviews = db.InspectionAppealsReviews.Find(id);
            if (inspectionAppealsReviews == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.CreatedByUserId);
            ViewBag.InspectionAppealId = new SelectList(db.InspectionAppeal, "InspectionAppealId", "IdentityOrPassportNumber", inspectionAppealsReviews.InspectionAppealId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.UserId);
            return View(inspectionAppealsReviews);
        }

        // POST: InspectionAppealsReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Manager" + "," + "Licensingt Clerk" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "InspectionAppealsReviewId,InspectionAppealId,UserId,Decision,Comment,DateReviewed,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] InspectionAppealsReviews inspectionAppealsReviews)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectionAppealsReviews).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.CreatedByUserId);
            ViewBag.InspectionAppealId = new SelectList(db.InspectionAppeal, "InspectionAppealId", "IdentityOrPassportNumber", inspectionAppealsReviews.InspectionAppealId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", inspectionAppealsReviews.UserId);
            return View(inspectionAppealsReviews);
        }

        // GET: InspectionAppealsReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectionAppealsReviews inspectionAppealsReviews = db.InspectionAppealsReviews.Find(id);
            if (inspectionAppealsReviews == null)
            {
                return HttpNotFound();
            }
            return View(inspectionAppealsReviews);
        }

        // POST: InspectionAppealsReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectionAppealsReviews inspectionAppealsReviews = db.InspectionAppealsReviews.Find(id);
            db.InspectionAppealsReviews.Remove(inspectionAppealsReviews);
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
