using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using C8.TradeLicense.Keys;
using C8.TradeLicense.ViewModels;
using System.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using C8.TradeLicense.Helpers;

namespace C8.TradeLicense.Controllers
{
    public class InspectionResponseController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private CesarDbContext core = new CesarDbContext();
        private FileHelpers _fileHelpers;

        public InspectionResponseController()
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


        #region InspectionResponse Index
        // GET: /InspectionResponse/
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Department Clerk" + "," + "Licensing Clerk")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            Initialise();
            try
            {
                if ((User.IsInRole("Licensing Administrator")) || (User.IsInRole("Licensing Manager")) || (User.IsInRole("System Admin")) || (User.IsInRole("Licensing Clerk")))
                {
                    return RedirectToAction("LicensePendingResponses", "InspectionResponse");
                }
                else
                {
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

                    DepartmentContact department = db.DepartmentContacts?.FirstOrDefault(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false);

                    if (department != null)
                    {
                        List<InspectionRequest> inspectionresponses = db.InspectionRequests?.
                                             Where(l => l.DepartmentId == department.DepartmentId &&
                                             (l.Status.StatusKey == StatusKeys.PendingLicenseInspection ||
                                              l.Status.StatusKey == StatusKeys.ResponsePending ||
                                              l.Status.StatusKey == StatusKeys.InspectionApproved) && l.IsActive)
                                             .Include(l => l.Department)
                                             .Include(l => l.License.Business)
                                             .Include(l => l.License.Region)
                                             .Include(l => l.Status)
                                             .Include(l => l.LicenseType)
                                             .Include(l => l.Client)
                                             .Include(l => l.License).ToList();

                        if (User.IsInRole("Department Inspector"))
                        {
                            inspectionresponses = db.InspectionRequests?.
                                                  Where(l => l.DepartmentContactId == department.DepartmentContactId &&
                                                  (l.Status.StatusKey == StatusKeys.PendingLicenseInspection ||
                                                  l.Status.StatusKey == StatusKeys.InspectionFailed ||
                                                  l.Status.StatusKey == StatusKeys.InspectionApproved) && l.IsActive)
                                                  .Include(l => l.Department)
                                                  .Include(l => l.License.Business)
                                                  .Include(l => l.License.Region)
                                                  .Include(l => l.Status)
                                                  .Include(l => l.LicenseType)
                                                  .Include(l => l.Client)
                                                  .Include(l => l.License).ToList();
                        }


                        bool userIsPrincipal = false;

                        if ((!User.IsInRole("System Admin")))
                        {
                            if (Users != null)
                            {
                                userIsPrincipal = db.DepartmentContacts.First(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false).IsPrinciple;
                            }
                        }
                        else
                        {
                            userIsPrincipal = true;

                        }

                        if (selectedSearch != null)
                        {
                            switch (selectedSearch)
                            {
                                case "ClientName":
                                    //Search by client name
                                    inspectionresponses = inspectionresponses.Where(i => (i.License.Client.Name.ToLower() + " " + i.License.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                                    break;
                                case "BusinessName":
                                    //Search by business name
                                    inspectionresponses = inspectionresponses.Where(i => i.License.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                                    break;
                                default:
                                    inspectionresponses = inspectionresponses.ToList();
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

                        LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
                        licenseApplicationDetails.CurrentFilter = inputSearch;
                        licenseApplicationDetails.SelectedSearch = selectedSearch;
                        licenseApplicationDetails.IsPrincipal = userIsPrincipal;
                        licenseApplicationDetails.InspectionRequests = inspectionresponses;

                        IPagedList<InspectionRequest> ipagedInspection = inspectionresponses.ToPagedList(pageNumber, pageSize);
                        licenseApplicationDetails.IPagedInspection = ipagedInspection;


                        return View(licenseApplicationDetails);
                    }

                    TempData[LicenseApplicationDetails.ErrorKey] = "Not Associated to a Department";
                    return RedirectToAction("Index", "Home");
                }

            }

            catch (Exception)
            {
                return View("Error");
            }


        }

        #endregion


        #region InspectionResponse Details
        // GET: /InspectionResponse/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Department Clerk" + "," + "Licensing Clerk")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InspectionResponse inspectionresponse = db.InspectionResponse.Find(id);
                var license = db.Licenses.Find(inspectionresponse.LicenseId);
                var uploadedFiles = db.FileUploads.Include(f => f.Document).Where(f => f.referenceId == inspectionresponse.InspectionResponseId && f.IsActive == true && f.IsDeleted == false).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "inspection_document" && s.IsActive == true && s.IsDeleted == false).Select(s => s.DocumentTypeId).FirstOrDefault());
                ViewData["UploadList"] = uploadedFiles;
                ViewBag.ClientName = db.Clients.Find(license.ClientId).Name;
                ViewBag.ModifiedBy = db.Users.Find(inspectionresponse.CreatedByUserId).FullName;

                if (inspectionresponse == null)
                {
                    return HttpNotFound();
                }
                return View(inspectionresponse);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionResponse Create(GET)
        // GET: /InspectionResponse/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Department Clerk" + "," + "Licensing Clerk")]
        public ActionResult Create(int? id)
        {
            try
            {
                Initialise();
                var inspectionReq = db.InspectionRequests.Where(l => l.InspectionRequestId == id).Include(l => l.License).Include(l => l.License.Client).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.ItemType).Include(l => l.License.ItemSubCategory).Include(l => l.License.ItemCondition).Include(l => l.License.Status).Include(l => l.Department).Include(l => l.Status).FirstOrDefault();
                // If not pending inspection or response pending redirect to index
                if (db.InspectionResponse.Any(i => i.InspectionRequestId == id && i.IsActive && !i.IsDeleted))
                    return RedirectToAction("ApplicationTrackerDetails", "License", new {id= inspectionReq.LicenseId });

                var DepartmentContact = db.DepartmentContacts.Where(d => d.DepartmentContactId == inspectionReq.DepartmentContactId).Include(l => l.User).FirstOrDefault();
                var Requestdocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
                var UploadedDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == inspectionReq.ClientId && d.referenceId == id && d.Document.DocumentTypeId == Requestdocument && d.IsDeleted == false && d.IsActive).ToList();
                var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.RequestId = inspectionReq.InspectionRequestId;
                ViewBag.LicenseId = inspectionReq.LicenseId;
                ViewBag.LicenseApplicationDate = db.Licenses.Find(inspectionReq.LicenseId).ApplicationDateTime;
                ViewBag.ExpiryDate = inspectionReq.SlaExpiryDate;
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.RequestActive = inspectionReq.IsActive;
                var StatusTypeId = db.StatusTypes.Where(l => l.StatusTypeName == TLKeys.InspectionStatus).Select(s => s.StatusTypeId).FirstOrDefault();
                ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == StatusTypeId && s.StatusName != StatusKeys.InspectionPending && s.IsActive == true && s.IsDeleted == false).OrderBy(c => c.StatusName), "StatusId", "StatusName");
                ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(l => l.CreatedByUser).Include(d => d.DocumentType);
                ViewData["DepartmentContactdata"] = DepartmentContact;
                ViewData["BusinessData"] = inspectionReq.License.Business;
                ViewData["ClientData"] = inspectionReq.License.Client;
                ViewData["LicenseInfo"] = inspectionReq.License;
                ViewData["InspectionRequestsData"] = inspectionReq;
                ViewData["inspectionUploadList"] = UploadedDocs;
                ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
                var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == inspectionReq.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == inspectionReq.DepartmentId).ToList();
                var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                          .Select(d => d.DocumentTypeId)
                                          .FirstOrDefault();
                var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == inspectionReq.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();

                ViewData["InspectorDocs"] = InspectorDocs;
                ViewData["InspectionHistoryData"] = InspectionHistory;
                if (User.IsInRole("Department Clerk"))
                {
                    InspectionResponse InspectionResponse = new InspectionResponse();
                    InspectionResponse.CapturedByClerk = true;

                    return View(InspectionResponse);

                }
                else
                {
                    return View();
                }

            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionResponse Create(POST)
        // POST: /InspectionResponse/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Department Clerk" + "," + "Licensing Clerk")]
        public ActionResult Create([Bind(Include = "InspectionResponseId,InspectionRequestId,InspectionDateTime,LicenseId,InspectionFailureCount,Comment,StatusId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,NonComplianceDateTime,ComplianceDateTime,CapturedByInspector,AdhocInspector")] InspectionResponse inspectionresponse, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                Initialise();
                #region  Email 
                var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                var body = String.Empty;
                #endregion Email 

                var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                var inspectionRequest = db.InspectionRequests.Where(l => l.InspectionRequestId == inspectionresponse.InspectionRequestId).Include(l => l.Department).Include(l => l.License).Include(l => l.License.Client).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.ItemType).Include(l => l.License.ItemSubCategory).Include(l => l.License.ItemCondition).Include(l => l.License.Status);

                if (ModelState.IsValid)
                {

                    var document = db.Documents.Include(d => d.DocumentType).First(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);
                    var status = db.Status.Find(inspectionresponse.StatusId).StatusName;

                    var license = db.Licenses.Find(inspectionRequest.FirstOrDefault().LicenseId);


                    if (User.IsInRole("Licensing Clerk"))
                    {

                        inspectionresponse.CapturedByClerk = true;
                        inspectionresponse.Clerk = Users.FullName;


                    }
                    if (User.IsInRole("Department Clerk"))
                    {

                        inspectionresponse.CapturedByClerk = true;
                        inspectionresponse.Clerk = Users.FullName;


                    }
                    inspectionresponse.IsActive = true;
                    inspectionresponse.IsDeleted = false;
                    inspectionresponse.IsLocked = false;

                    InspectionRequest InspectionRequest = db.InspectionRequests.Find(inspectionresponse.InspectionRequestId);


                    db.InspectionResponse.Add(inspectionresponse);

                    inspectionresponse.IsDeleted = false;
                    inspectionresponse.IsLocked = false;
                    inspectionRequest.OrderByDescending(o => o.InspectionRequestId).FirstOrDefault().StatusId = inspectionresponse.StatusId;

                    InspectionRequest.IsActive = false;
                    InspectionRequest.IsDeleted = true;
                    db.Entry(InspectionRequest).State = EntityState.Modified;
                    db.SaveChanges();

                    ///History for appeals
                    var requestid = inspectionresponse.InspectionRequestId;
                    try
                    {

                        inspectionresponse = db.InspectionResponse.Where(r => r.InspectionRequestId == requestid).Include(l => l.Status).FirstOrDefault();
                        InspectionHistory inspectionHistory = new InspectionHistory();
                        inspectionHistory.AdhocInspector = inspectionresponse.AdhocInspector;
                        inspectionHistory.CapturedByClerk = inspectionresponse.CapturedByClerk;
                        inspectionHistory.Clerk = inspectionresponse.Clerk;
                        inspectionHistory.Comment = inspectionresponse.Comment;

                        inspectionHistory.LicenseId = inspectionresponse.LicenseId;
                        inspectionHistory.InspectionRequestId = inspectionresponse.InspectionRequestId;

                        inspectionHistory.StatusId = inspectionresponse.StatusId;
                        inspectionHistory.InspectionResponseId = inspectionresponse.InspectionResponseId;
                        inspectionHistory.InspectionDateTime = inspectionresponse.InspectionDateTime;
                        db.InspectionHistory.Add(inspectionHistory);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Index");
                        //return View("Error");
                    }
                    if (status == TLKeys.InspectionFailed)
                    {
                        inspectionresponse.InspectionFailureCount = inspectionresponse.InspectionFailureCount + 1;
                        InspectionRequest.StatusId = inspectionresponse.StatusId;

                        license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.RefusalInProgress).Select(s => s.StatusId).FirstOrDefault();
                        db.Entry(license).State = EntityState.Modified;
                        db.SaveChanges();
                        ///
                        try
                        {
                            License licence = db.Licenses.Where(l => l.LicenseId == license.LicenseId)
                            .Include(l => l.Client)
                            .Include(l => l.Business)
                            .Include(l => l.LicenseType)
                            .Include(l => l.Status)
                            .Include(l => l.Region)
                            .Include(l => l.ItemType)
                            .Include(l => l.ItemSubCategory)
                            .Include(l => l.ItemCondition)


                            .FirstOrDefault();
                            #region Send Email To Admin
                            body = String.Empty;

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
                                body = body.Replace("#BODYTEXT#", " <b>Application has been actioned for: </b><br/><br/> Business Name: " +
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
                                    Subject = "eThekwini Trade Licensing -" + " Inspection failed:" + license.LicenseNumber,
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
                        InspectionRequest.StatusId = inspectionresponse.StatusId;
                        var inspectionApproved = db.Status.Where(l => l.StatusKey == StatusKeys.InspectionApproved).Select(s => s.StatusId).FirstOrDefault();                            ///checks if any inspection isn't approved
                        var inspectionNotapproved = db.InspectionRequests.Where(i => i.StatusId != inspectionApproved && i.LicenseId == license.LicenseId && i.IsDeleted != true).Count();

                        if (inspectionNotapproved == 0)
                        {
                            ///if the last inspection Request has been approved status of application changes to License Pending 


                            license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).Select(s => s.StatusId).FirstOrDefault();
                            db.Entry(license).State = EntityState.Modified;
                            db.SaveChanges();


                            ///
                            try
                            {
                                License licence = db.Licenses.Where(l => l.LicenseId == license.LicenseId)
                            .Include(l => l.Client)
                            .Include(l => l.Business)
                            .Include(l => l.LicenseType)
                            .Include(l => l.Status)
                            .Include(l => l.Region)
                            .Include(l => l.ItemType)
                            .Include(l => l.ItemSubCategory)
                            .Include(l => l.ItemCondition)


                            .FirstOrDefault();



                                var admins = db.Users.Where(c => c.Role == "Licensing Administrator" && c.IsActive).ToList();
                                //LF2014a 
                                //User email helper to get well formatted email.
                                //var mail = new EmailHelper();

                                string actionLink = "<a href=" + Request.Url.Authority + ActionLinkKeys.AdministratorReviewsCreate + licence.LicenseId + ">Click here</a>";
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
                                        "<br/> " + actionLink + " to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
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


                        }
                    }
                    //// send mail to department manager
                    #region Construct emails
                    applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                    template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                    body = string.Empty;

                    if (template != null)
                    {
                        body = template.EmailBody;
                    }


                    #region DepartmentManager
                    License licence2 = db.Licenses.Where(l => l.LicenseId == license.LicenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition)


.FirstOrDefault();



                    var depContacts = db.DepartmentContacts.Where(d => d.DepartmentId == inspectionRequest.FirstOrDefault().DepartmentId && d.IsDeleted == false && d.IsActive && d.RoleName == "Department Manager")
                                                           .Include(u => u.User).ToList();

                    var department = db.Departments.Where(d => d.DepartmentId == inspectionRequest.FirstOrDefault().DepartmentId).FirstOrDefault();

                    if (depContacts.Count > 0)
                    {
                        foreach (var departmentContact in depContacts)
                        {

                            if (template != null)
                            {
                                body = template.EmailBody;
                            }

                            if (departmentContact != null)
                            {
                                //L.M.20150303a - Replace variables with actual email content
                                body = body.Replace("#NAME#", departmentContact.User.FullName);
                                body = body.Replace("#BODYTEXT#", " <b>Application Inspection has been Processed for: </b><br/><br/> Business Name: " +
                               licence2.Business.ProposedTradeName + "<br/> " +
                                "Inspection processed by: " + Users.FullName + "<br/> " +

                                " Inspectors Response: " + inspectionresponse.Status.StatusName + "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not response to this email, as it is a system generated email.<br/>");

                                var email = new tb_EmailQueue
                                {
                                    QueueDateTime = DateTime.Now,
                                    ApplicationId = applicationId,
                                    EmailAccountId = 5,//Hard coded
                                    ToList = departmentContact.User.EmailAddress,
                                    CcList = null,
                                    BccList = null,
                                    Subject = "eThekwini Trade Licensing-" + department.DepartmentName + " Inspection Response." + licence2.LicenseNumber,
                                    Body = body,
                                    IsHtml = true,
                                    FailureCount = 0,
                                    ReferenceId = licence2.Client.IdentityOrPassportNumber,
                                    HasAttachments = false
                                };

                                core.tb_EmailQueue.Add(email);
                                core.SaveChanges();

                            }


                        }

                    }

                    #endregion DepartmentManager mail
                    #endregion email


                    foreach (HttpPostedFileBase fileupload in file)
                    {
                        if (fileupload != null)
                        {
                            IdentityManager identifyManager = new IdentityManager();
                            string currentUserId = User.Identity.GetUserId();
                            var user = identifyManager.CurrentUser(currentUserId);
                            var userid = user.UserId; ;

                            //Replaces spaces in file name with '_'



                            int position = fileupload.FileName.LastIndexOf(".", StringComparison.Ordinal);
                            string filename = fileupload.FileName.Substring(0, position);
                            string extension = fileupload.FileName.Substring(position + 1);

                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(fileupload.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                            }
                            SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Inspection Response");
                            _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, license.Client.ClientId
                            , license.Client.Fullname, document.DocumentId, inspectionresponse.InspectionResponseId, string.Empty, Users.Username);
                        }
                    }
                    TempData["successsful"] = "The inspections was captured successful";

                    return RedirectToAction("Index");
                }
                else
                {

                    var DepartmentContact = db.DepartmentContacts.Where(d => d.DepartmentContactId == inspectionRequest.FirstOrDefault().DepartmentContactId).Include(l => l.User).FirstOrDefault();


                    ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                    ViewBag.RequestId = inspectionRequest.FirstOrDefault().InspectionRequestId;
                    ViewBag.LicenseId = inspectionRequest.FirstOrDefault().LicenseId;
                    ViewBag.LicenseApplicationDate = db.Licenses.Find(inspectionRequest.FirstOrDefault().LicenseId).ApplicationDateTime;
                    ViewBag.ExpiryDate = inspectionRequest.FirstOrDefault().SlaExpiryDate;
                    ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                    ViewBag.RequestActive = inspectionRequest.FirstOrDefault().IsActive;
                    ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 4 && s.StatusName != "Inspection Pending" && s.IsActive == true && s.IsDeleted == false).OrderBy(c => c.StatusName), "StatusId", "StatusName");
                    var Requestdocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                     .Select(d => d.DocumentTypeId)
                                     .FirstOrDefault();
                    var UploadedDocs = db.FileUploads.Include(d => d.Document)
                                        .Where(d => d.ClientId == inspectionRequest.FirstOrDefault().License.ClientId && d.referenceId == inspectionresponse.InspectionRequestId && d.Document.DocumentTypeId == Requestdocument && d.IsDeleted == false && d.IsActive).ToList();


                    ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType);
                    ViewData["DepartmentContactdata"] = DepartmentContact;
                    ViewData["BusinessData"] = inspectionRequest.FirstOrDefault().License.Business;
                    ViewData["ClientData"] = inspectionRequest.FirstOrDefault().License.Client;
                    ViewData["licenceData"] = inspectionRequest.FirstOrDefault().License;
                    ViewData["InspectionRequestsData"] = inspectionRequest.FirstOrDefault();
                    ViewData["inspectionUploadList"] = UploadedDocs;
                    ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
                    var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == inspectionRequest.FirstOrDefault().License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == inspectionRequest.FirstOrDefault().DepartmentId).ToList();

                    ViewData["InspectionHistoryData"] = InspectionHistory;
                    var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                     .Select(d => d.DocumentTypeId)
                                     .FirstOrDefault();
                    var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                        .Where(d => d.ClientId == inspectionRequest.FirstOrDefault().License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();
                    return View();
                }


            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
                //return View("Error");
            }
        }



        #region Licenses Pending Responses
        // GET: /InspectionResponse/LicensePendingResponses
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Licensing Clerk" + "," + "Department Clerk")]
        public ActionResult LicensePendingResponses(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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




                var licenses = db.Licenses.Include(l => l.Business).Include(l => l.Client).Include(l => l.CreatedByUser).Include(l => l.LicenseType).Include(l => l.ModifiedByUser).Include(l => l.Status);
                List<License> pendingLicenseResponses = new List<License>();

                if ((User.IsInRole("System Admin")))
                {
                    pendingLicenseResponses = licenses.Where(c => c.IsDeleted == false && c.IsActive == true && c.Status.StatusKey == StatusKeys.PendingLicenseInspection || c.Status.StatusKey == StatusKeys.RefusalInProgress).ToList();
                }
                else if ((User.IsInRole("Licensing Administrator")) || (User.IsInRole("Licensing Clerk")))
                {

                    pendingLicenseResponses = licenses.Where(c => c.IsDeleted == false && c.IsActive == true && (c.Status.StatusKey == StatusKeys.PendingLicenseInspection || c.Status.StatusKey == StatusKeys.RefusalInProgress || c.Status.StatusKey == StatusKeys.ResponsePending) && c.RegionId == Users.Region).ToList();
                }
                else
                {
                    pendingLicenseResponses = licenses.Where(c => c.IsDeleted == false && c.IsActive == true && c.Status.StatusKey == StatusKeys.LicenseAwaitingRecommendation).ToList();
                }

                if (selectedSearch != null)
                {
                    switch (selectedSearch)
                    {
                        case "ClientName":
                            //Search by client name
                            pendingLicenseResponses = pendingLicenseResponses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "ReferenceNo":
                            //Search by reference number
                            pendingLicenseResponses = pendingLicenseResponses.Where(l => l.LicenseNumber == inputSearch.ToUpper()).ToList();

                            break;
                        case "BusinessName":
                            //Search by business name
                            pendingLicenseResponses = pendingLicenseResponses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
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
                ViewBag.PendingLicenseResponses = pendingLicenseResponses.Count;
                return View(pendingLicenseResponses.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion



        #region Approve License(GET)
        // GET: /InspectionResponse/ApproveLicense
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult ApproveLicense(int? id)
        {
            try
            {
                var inspectionresponse = db.InspectionResponse.Include(i => i.InspectionRequest).Include(i => i.InspectionRequest.Client).Include(i => i.InspectionRequest.Department).Include(i => i.License).Include(i => i.Status);

                if (User.IsInRole("Administrator"))
                {
                    inspectionresponse = inspectionresponse.Where(s => s.IsDeleted == false && s.IsActive == true && s.LicenseId == id);
                    var inspectionRequests = db.InspectionRequests.Where(i => i.IsActive == true && i.IsDeleted == false && i.LicenseId == id).Select(i => new { i.DepartmentId, i.Department });
                    ViewBag.FailedInspections = inspectionresponse.Where(i => i.Status.StatusKey == "InspectionFailed" && i.IsActive == true && i.IsDeleted == false).Select(i => new { i.LicenseId, i.License, i.StatusId, i.Status, i.InspectionRequestId, i.InspectionRequest.Department }).ToList();
                    ViewBag.OutstandingResponses = inspectionRequests.ToList();
                }
                else
                {
                    inspectionresponse = inspectionresponse.Where(s => s.IsDeleted == true && s.IsActive == false && s.LicenseId == id);
                }

                ViewBag.LicenseInspectionResponseCount = inspectionresponse.Where(i => i.IsActive == true && i.IsDeleted == false).ToList().Count;
                return View(inspectionresponse.ToList());
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Approve License(POST)
        // POST: /InspectionResponse/ApproveLicense
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult ApproveLicense(int id)
        {
            Initialise();
            try
            {
                if (ModelState.IsValid)
                {
                    var license = db.Licenses.Find(id);
                    int approvedLicenseStatusId;

                    if (User.IsInRole("Licensing Administrator") || User.IsInRole("System Admin"))
                    {
                        approvedLicenseStatusId = db.Status.First(d => d.StatusKey == StatusKeys.LicenseAwaitingRecommendation && d.IsActive == true && d.IsDeleted == false).StatusId;
                        int approvedInspectionStatusId = db.Status.First(d => d.StatusKey == StatusKeys.InspectionApproved && d.IsActive == true && d.IsDeleted == false).StatusId;
                        var inspectionResponses = db.InspectionResponse.Where(d => d.LicenseId == id && d.IsActive == true && d.IsDeleted == false);
                        var inspectionRequests = db.InspectionRequests.Where(d => d.LicenseId == id && d.IsActive == true && d.IsDeleted == false);

                        foreach (var inspection in inspectionResponses)
                        {
                            inspection.ComplianceDateTime = DateTime.Now;
                            inspection.IsActive = false;
                            inspection.IsDeleted = true;
                            db.Entry(inspection).State = EntityState.Modified;
                        }

                        foreach (var request in inspectionRequests)
                        {
                            request.StatusId = approvedInspectionStatusId;
                            db.Entry(request).State = EntityState.Modified;
                        }

                        #region license manager email notification
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TradeLicenseDbContext()));
                        var roleManagerUser = roleManager.FindByName("Licensing Manager").Users.FirstOrDefault();
                        var licensingManager = IdentityManager.CurrentUser(roleManagerUser.UserId);

                        if (roleManagerUser != null)
                        {
                            var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                            var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                            var body = string.Empty;
                            var client = db.Clients.Find(license.ClientId);
                            var business = db.Businesses.Find(license.BusinessId);

                            if (template != null)
                            {
                                body = template.EmailBody;
                            }

                            //L.M.20150303a - Replace variables with actual email content
                            body = body.Replace("#NAME#", licensingManager.FullName);
                            body = body.Replace("#BODYTEXT#", "<b>A License is awaiting final approval. See Details Below </b><br/>"
                                + " Client Name: " + client.Fullname + "<br/> Business Name: " + business.ProposedTradeName);

                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = licensingManager.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "eThekwini Trade Licensing- License Approval Notification.",
                                Body = body,
                                IsHtml = true,
                                FailureCount = 0,
                                ReferenceId = licensingManager.UserId.ToString(),
                                HasAttachments = false
                            };

                            core.tb_EmailQueue.Add(email);
                            core.SaveChanges();
                        }

                        #endregion license manager
                    }
                    else
                    {
                        approvedLicenseStatusId = db.Status.First(d => d.StatusKey == "LicenseApprovedAwaitingCollection" && d.IsActive == true && d.IsDeleted == false).StatusId;
                        #region client email notification
                        var client = db.Clients.Find(license.ClientId);
                        var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                        var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.EmailTemplateKey == "tls_client_email_template"
                            && t.IsDeleted == false && t.IsActive);
                        var body = string.Empty;

                        if (template != null)
                        {
                            body = template.EmailBody;
                        }

                        //L.M.20150303a - Replace variables with actual email content
                        body = body.Replace("#NAME#", client.Fullname);
                        body = body.Replace("#BODYTEXT#", "Your License has been approved and awaiting collection.");

                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = 5,//Hard coded
                            ToList = client.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "eThekwini Trade Licensing- License Collection Notification.",
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = client.IdentityOrPassportNumber,
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                        core.SaveChanges();

                        #endregion client mail
                    }

                    license.StatusId = approvedLicenseStatusId;
                    license.NotificationUpdatedDateTime = DateTime.Now;
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionresponse.CreatedByUserId);
                //ViewBag.RequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "InspectionRequestId", inspectionresponse.InspectionRequestId);
                //ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseId", inspectionresponse.LicenseId);
                //ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionresponse.ModifiedByUserId);
                //ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 4).OrderBy(c => c.StatusName), "StatusId", "StatusName");
                return RedirectToAction("LicensePendingResponses", "InspectionResponse");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionResponse Edit(GET)
        // GET: /InspectionResponse/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Licensing Clerk" + "," + "Department Clerk")]
        public ActionResult Edit(int? id)
        {

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var inspectionReq = db.InspectionRequests.Where(l => l.InspectionRequestId == id).Include(l => l.Status).Include(l => l.License).Include(l => l.License.Client).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.ItemType).Include(l => l.License.ItemSubCategory).Include(l => l.License.ItemCondition).Include(l => l.License.Status).Include(l => l.Department).FirstOrDefault();
                var DepartmentContact = db.DepartmentContacts.Where(d => d.DepartmentContactId == inspectionReq.DepartmentContactId).Include(l => l.User).FirstOrDefault();
                var responseid = db.InspectionResponse.Where(i => i.InspectionRequestId == id).Select(s => s.InspectionResponseId).FirstOrDefault();
                InspectionResponse inspectionresponse = db.InspectionResponse.Find(responseid);
                var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType);

                var uploadedFiles = db.FileUploads.Where(f => f.ClientId == inspectionReq.License.ClientId && f.referenceId == inspectionresponse.InspectionResponseId && f.IsActive == true && f.IsDeleted == false).Include(f => f.Document).ToList();
                if (inspectionresponse == null)
                {
                    return HttpNotFound();
                }
                var Requestdocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                      .Select(d => d.DocumentTypeId)
                                      .FirstOrDefault();
                var UploadedDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == inspectionReq.License.ClientId && d.referenceId == id && d.Document.DocumentTypeId == Requestdocument && d.IsDeleted == false && d.IsActive).ToList();
                ViewData["DepartmentContactdata"] = DepartmentContact;
                ViewData["BusinessData"] = inspectionReq.License.Business;
                ViewData["ClientData"] = inspectionReq.License.Client;
                ViewData["licenceData"] = inspectionReq.License;
                ViewData["InspectionRequestsData"] = inspectionReq;
                ViewData["InspectionResponseData"] = inspectionresponse;
                ViewData["Documents"] = document;
                ViewData["UploadList"] = uploadedFiles;
                ViewData["inspectionUploadList"] = UploadedDocs;
                ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionresponse.CreatedByUserId);
                //ViewBag.RequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "InspectionRequestId", inspectionresponse.RequestId);
                //ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseId", inspectionresponse.LicenseId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionresponse.ModifiedByUserId);
                ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 4 && s.StatusName != "Inspection Pending" && s.IsActive == true && s.IsDeleted == false).OrderBy(c => c.StatusName), "StatusId", "StatusName", inspectionresponse.StatusId);

                var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == inspectionReq.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == inspectionReq.DepartmentId).ToList();

                ViewData["InspectionHistoryData"] = InspectionHistory;
                var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
                var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == inspectionReq.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();
                ViewData["InspectorDocs"] = InspectorDocs;
                if (User.IsInRole("Licensing Clerk"))
                {

                    inspectionresponse.CapturedByClerk = true;


                }


                return View(inspectionresponse);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionResponse Edit(POST)
        // POST: /InspectionResponse/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Licensing Clerk" + "," + "Department Clerk")]
        public ActionResult Edit([Bind(Include = "InspectionResponseId,InspectionRequestId,InspectionDateTime,InspectionFailureCount,LicenseId,Comment,StatusId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,NonComplianceDateTime,ComplianceDateTime")] InspectionResponse inspectionresponse, IEnumerable<HttpPostedFileBase> file)
        {
            Initialise();
            try
            {
                var inspectionRequest = db.InspectionRequests.Where(L => L.InspectionRequestId == inspectionresponse.InspectionRequestId).Include(L => L.Department);
                var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();

                var document = db.Documents.Include(d => d.DocumentType).First(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);
                var uploadedFiles = db.FileUploads.Include(f => f.Document).Where(f => f.referenceId == inspectionresponse.InspectionResponseId && f.IsActive == true && f.IsDeleted == false).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "inspection_document" && s.IsActive == true && s.IsDeleted == false).Select(s => s.DocumentTypeId).FirstOrDefault());

                if (ModelState.IsValid)
                {

                    var status = db.Status.Find(inspectionresponse.StatusId).StatusName;
                    var department = db.Departments.Find(inspectionRequest.FirstOrDefault().DepartmentId);
                    var license = db.Licenses.Where(l => l.LicenseId == inspectionresponse.LicenseId).Include(l => l.Client).FirstOrDefault();

                    if (User.IsInRole("Licensing Clerk"))
                    {

                        inspectionresponse.CapturedByClerk = true;



                    }
                    if (User.IsInRole("Department Clerk"))
                    {

                        inspectionresponse.CapturedByClerk = true;



                    }

                    inspectionresponse.IsActive = true;
                    inspectionresponse.IsDeleted = false;
                    inspectionresponse.IsLocked = false;
                    InspectionRequest InspectionRequest = db.InspectionRequests.Find(inspectionresponse.InspectionRequestId);
                    if (status == TLKeys.InspectionFailed)
                    {
                        inspectionresponse.InspectionFailureCount = inspectionresponse.InspectionFailureCount + 1;
                        InspectionRequest.StatusId = inspectionresponse.StatusId;

                        license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.RefusalInProgress).Select(s => s.StatusId).FirstOrDefault();
                        db.Entry(license).State = EntityState.Modified;
                        db.SaveChanges();
                        ///
                        try
                        {
                            License licence = db.Licenses.Where(l => l.LicenseId == license.LicenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition)


.FirstOrDefault();
                            #region Send Email To Admin
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
                                body = body.Replace("#BODYTEXT#", " <b>Application has been actioned for: </b><br/><br/> Business Name: " +
                                   license.Business.ProposedTradeName + "<br/> " +
                                    "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not response to this email, as it is a system generated email.<br/><br/>");
                                var email = new tb_EmailQueue
                                {
                                    QueueDateTime = DateTime.Now,
                                    ApplicationId = applicationId,
                                    EmailAccountId = 5,//Hard coded
                                    ToList = user.EmailAddress,
                                    CcList = null,
                                    BccList = null,
                                    Subject = "eThekwini Trade Licensing -" + " Inspection failed:" + license.LicenseNumber,
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
                        InspectionRequest.StatusId = inspectionresponse.StatusId;
                        var NotapprovedStatus = db.Status.Where(l => l.StatusKey == StatusKeys.InspectionApproved).Select(s => s.StatusId).FirstOrDefault();                            ///checks if any inspection isn't approved
                        var inspectionNotapproved = db.InspectionRequests.Where(i => i.StatusId != NotapprovedStatus && i.LicenseId == license.LicenseId).Count();

                        if (inspectionNotapproved == 0)
                        {
                            ///if the last inspection Request has been approved status of application changes to License Pending 


                            license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).Select(s => s.StatusId).FirstOrDefault();
                            db.Entry(license).State = EntityState.Modified;
                            db.SaveChanges();


                            ///
                            try
                            {
                                License licence = db.Licenses.Where(l => l.LicenseId == license.LicenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition)


.FirstOrDefault();
                                #region Send Email To Admin
                                var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                                var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                                var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                                var body = String.Empty;


                                var admins = db.Users.Where(c => c.Role == "Licensing Administrator" && c.Region == license.RegionId).ToList();
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
                                    body = body.Replace("#BODYTEXT#", " <b>Application Awaiting Review. </b><br/><br/> Business Name: " +
                                       license.Business.ProposedTradeName + "<br/> " +
                                        "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not response to this email, as it is a system generated email.<br/><br/>");
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

                        }
                    }


                    inspectionresponse.IsDeleted = false;
                    inspectionresponse.IsLocked = false;


                    InspectionRequest.IsActive = false;
                    InspectionRequest.IsDeleted = true;
                    db.Entry(inspectionresponse).State = EntityState.Modified;


                    db.SaveChanges();
                    var local = db.Set<InspectionResponse>()
                    .Local.FirstOrDefault(l => l.InspectionResponseId == inspectionresponse.InspectionResponseId);

                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;
                    }

                    db.Entry(InspectionRequest).State = EntityState.Modified;



                    //if (inspectionresponse.InspectionFailureCount == 2)
                    //{
                    //    var licenseToTerminate = db.Licenses.Find(inspectionresponse.LicenseId);
                    //    var inspectionRequests = db.InspectionRequests.Where(i => i.LicenseId == inspectionresponse.LicenseId && i.IsActive == true && i.IsDeleted == false);
                    //    var inspectionResponses = db.InspectionResponse.Where(i => i.LicenseId == inspectionresponse.LicenseId && i.IsActive == true && i.IsDeleted == false);
                    //    int terminateLicenseStatusId = db.Status.First(d => d.StatusKey == "LicenseApplicationTerminated" && d.IsActive == true && d.IsDeleted == false).StatusId;

                    //    licenseToTerminate.StatusId = terminateLicenseStatusId;
                    //    licenseToTerminate.IsActive = false;
                    //    licenseToTerminate.IsDeleted = true;
                    //    db.Entry(licenseToTerminate).State = EntityState.Modified;

                    //    foreach (var inspection in inspectionResponses)
                    //    {
                    //        inspection.IsActive = false;
                    //        inspection.IsDeleted = true;
                    //        inspection.StatusId = terminateLicenseStatusId;
                    //        db.Entry(inspection).State = EntityState.Modified;
                    //    }

                    //    foreach (var request in inspectionRequests)
                    //    {
                    //        request.IsActive = false;
                    //        request.IsDeleted = true;
                    //        request.StatusId = terminateLicenseStatusId;
                    //        db.Entry(request).State = EntityState.Modified;
                    //    }
                    //}

                    db.SaveChanges();

                    ///History for appeals

                    try
                    {


                        InspectionHistory inspectionHistory = new InspectionHistory();
                        inspectionHistory.AdhocInspector = inspectionresponse.AdhocInspector;
                        inspectionHistory.CapturedByClerk = inspectionresponse.CapturedByClerk;
                        inspectionHistory.Comment = inspectionresponse.Comment;

                        inspectionHistory.LicenseId = inspectionresponse.LicenseId;
                        inspectionHistory.InspectionRequestId = inspectionresponse.InspectionRequestId;

                        inspectionHistory.StatusId = inspectionresponse.StatusId;
                        inspectionHistory.InspectionResponseId = inspectionresponse.InspectionResponseId;
                        inspectionHistory.InspectionDateTime = inspectionresponse.InspectionDateTime;
                        db.InspectionHistory.Add(inspectionHistory);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Index");
                        //return View("Error");
                    }




                    if (file.Any())
                    {
                        foreach (HttpPostedFileBase fileupload in file)
                        {
                            if (fileupload != null)
                            {


                                //Replaces spaces in file name with '_'

                                int fileLen = fileupload.ContentLength;
                                byte[] fileData = null;
                                using (var binaryReader = new BinaryReader(fileupload.InputStream))
                                {
                                    fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                                }

                                int position = fileupload.FileName.LastIndexOf(".", StringComparison.Ordinal);
                                string filename = fileupload.FileName.Substring(0, position);
                                string extension = fileupload.FileName.Substring(position + 1);

                                //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                                const string regExp = @"[^\w\d]";
                                string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                                string name = Regex.Replace(filename, regExp, "_");
                                string contentType = fileupload.ContentType;

                                SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Inspection Edit");
                                _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, license.Client.ClientId
                                , license.Client.Fullname, document.DocumentId, inspectionresponse.InspectionResponseId, string.Empty, Users.Username);
                            }
                        }
                    }

                    return RedirectToAction("Index");
                }


                ViewData["UploadList"] = uploadedFiles;
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionresponse.CreatedByUserId);
                ViewBag.RequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "InspectionRequestId", inspectionresponse.InspectionRequestId);
                ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseId", inspectionresponse.LicenseId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", inspectionresponse.ModifiedByUserId);
                ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 4 && s.StatusName != "Inspection Pending" && s.IsActive == true && s.IsDeleted == false).OrderBy(c => c.StatusName), "StatusId", "StatusName");
                var inspectionReq = db.InspectionRequests.Where(l => l.InspectionRequestId == inspectionresponse.InspectionRequestId).Include(l => l.Status).Include(l => l.License).Include(l => l.License.Client).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.ItemType).Include(l => l.License.ItemSubCategory).Include(l => l.License.ItemCondition).Include(l => l.License.Status).Include(l => l.Department).FirstOrDefault();
                var DepartmentContact = db.DepartmentContacts.Where(d => d.DepartmentContactId == inspectionReq.DepartmentContactId).Include(l => l.User).FirstOrDefault();
                var responseid = db.InspectionResponse.Where(i => i.InspectionRequestId == inspectionresponse.InspectionRequestId).Select(s => s.InspectionResponseId).FirstOrDefault();
                ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
                var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == inspectionReq.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == inspectionReq.DepartmentId).ToList();

                ViewData["InspectionHistoryData"] = InspectionHistory;
                var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
                var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == inspectionRequest.FirstOrDefault().License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();
                ViewData["InspectorDocs"] = InspectorDocs;

                if (inspectionresponse == null)
                {
                    return HttpNotFound();
                }
                var Requestdocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
                var UploadedDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == inspectionReq.ClientId && d.referenceId == inspectionresponse.InspectionRequestId && d.Document.DocumentTypeId == Requestdocument && d.IsDeleted == false && d.IsActive).ToList();

                ViewData["DepartmentContactdata"] = DepartmentContact;
                ViewData["BusinessData"] = inspectionReq.License.Business;
                ViewData["ClientData"] = inspectionReq.License.Client;
                ViewData["licenceData"] = inspectionReq.License;
                ViewData["InspectionRequestsData"] = inspectionReq;
                ViewData["Documents"] = document;
                ViewData["UploadList"] = uploadedFiles;
                ViewData["inspectionUploadList"] = UploadedDocs;


                if (User.IsInRole("Licensing Clerk"))
                {

                    inspectionresponse.CapturedByClerk = true;


                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //return View("Error");
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region InspectionResponse Delete(GET)
        // GET: /InspectionResponse/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InspectionResponse inspectionresponse = db.InspectionResponse.Find(id);
                if (inspectionresponse == null)
                {
                    return HttpNotFound();
                }
                return View(inspectionresponse);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region InspectionResponse Delete(POST)
        // POST: /InspectionResponse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                InspectionResponse inspectionresponse = db.InspectionResponse.Find(id);
                db.InspectionResponse.Remove(inspectionresponse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
                //return View("Error");
            }
        }

        #endregion

        #region Download Inspection Report
        //Downloads file from directory using the fileuploadId and filename
        //TG20141106a.


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
