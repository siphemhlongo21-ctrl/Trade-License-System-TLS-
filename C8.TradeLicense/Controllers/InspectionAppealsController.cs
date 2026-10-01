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
using C8.TradeLicense.Keys;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using C8.TradeLicense.ViewModels;
using PagedList;
using Microsoft.AspNet.Identity;
using C8.TradeLicense.Helpers;

namespace C8.TradeLicense.Controllers
{
    [Authorize]
    public class InspectionAppealsController : Controller
    {
        private CesarDbContext core = new CesarDbContext();
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private FileHelpers _fileHelpers;
        private RefusalHelpers _refusalHelpers;
        public InspectionAppealsController()
        {
            _fileHelpers = new FileHelpers(db);
            _refusalHelpers = new RefusalHelpers(db);
        }
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

        // GET: InspectionAppeals
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "Licensing Clerk" + "," + "System Admin")]
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



            int allregion = db.Regions.Where(s => s.RegionKey == TLKeys.metro_all).Select(s => s.RegionId)
                   .FirstOrDefault();
            int metro_NorthWest = db.Regions.Where(s => s.RegionKey == TLKeys.metro_NorthWest).Select(s => s.RegionId)
                .FirstOrDefault();
            int metro_CentralSouth = db.Regions.Where(s => s.RegionKey == TLKeys.metro_CentralSouth).Select(s => s.RegionId)
                .FirstOrDefault();

            var Appeal = db.InspectionAppeal.Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.Status).Include(l => l.License.LicenseType).Include(l => l.License.Client).Where(l => l.StatusId == pendingReview && l.License.RegionId == Users.Region).OrderBy(l => l.Status.StatusName).ToList();

            if (Users.Region == allregion)
            {
                Appeal = db.InspectionAppeal.Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.Status).Include(l => l.License.LicenseType).Include(l => l.License.Client).Where(l => l.StatusId == pendingReview).OrderBy(l => l.Status.StatusName).ToList();
            }

            else if (Users.Region == metro_CentralSouth)

            {
                int metro_south = db.Regions.Where(s => s.RegionKey == TLKeys.metro_south).Select(s => s.RegionId)
       .FirstOrDefault();
                int metro_central = db.Regions.Where(s => s.RegionKey == TLKeys.metro_central).Select(s => s.RegionId)
    .FirstOrDefault();
                Appeal = db.InspectionAppeal.Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.Status).Include(l => l.License.LicenseType).Include(l => l.License.Client).Where(l => l.StatusId == pendingReview && (l.License.RegionId == metro_central || l.License.RegionId == metro_south)).OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_NorthWest)

            {
                int metro_north = db.Regions.Where(s => s.RegionKey == TLKeys.metro_north).Select(s => s.RegionId)
.FirstOrDefault();
                int meto_west = db.Regions.Where(s => s.RegionKey == TLKeys.meto_west).Select(s => s.RegionId)
    .FirstOrDefault();
                Appeal = db.InspectionAppeal.Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.Status).Include(l => l.License.LicenseType).Include(l => l.License.Client).Where(l => l.StatusId == pendingReview && (l.License.RegionId == meto_west || l.License.RegionId == metro_north)).OrderBy(l => l.Status.StatusName).ToList();
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

                        Appeal = Appeal.Where(l => l.License.LicenseNumber == inputSearch).ToList();


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

        // GET: InspectionAppeals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectionAppeal inspectionAppeal = db.InspectionAppeal.Find(id);
            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            return View(inspectionAppeal);
        }

        // GET: InspectionAppeals/Create
        public ActionResult Create(int InspectionRequestId)
        {
            Initialise();
            LicenseApplicationDetails licenseApplicationDetails = _refusalHelpers.GetRefusal(InspectionRequestId);
            return View(licenseApplicationDetails);
        }





        // POST: InspectionAppeals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails)
        {
            Initialise();
            InspectionAppeal inspectionAppeal = licenseApplicationDetails.InspectionAppealDetails;
            int inspectionRequestId = inspectionAppeal.InspectionResponse.InspectionRequestId;
            inspectionAppeal.InspectionResponse = null;
            inspectionAppeal.License = null;
            var InspectionResponse = db.InspectionResponse.Where(l => l.InspectionResponseId == inspectionAppeal.InspectionResponseId).Include(l => l.InspectionRequest).Include(l => l.License).FirstOrDefault();
            if (inspectionAppeal.AppealDate != null)
            {
                DateTime appealD = DateTime.Parse(inspectionAppeal.AppealDate.ToString());
                inspectionAppeal.AppealDate = appealD;
            }
            if (ModelState.IsValid)
            {
                    inspectionAppeal.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.InspectionAppealed).Select(s => s.StatusId).FirstOrDefault();
                    db.InspectionAppeal.Add(inspectionAppeal);
                    db.SaveChanges();
                    InspectionResponse.InspectionRequest.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.InspectionAppealed).Select(s => s.StatusId).FirstOrDefault();
                    db.Entry(InspectionResponse).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Refusal", "License");                                
            }
            licenseApplicationDetails = _refusalHelpers.GetRefusal(inspectionRequestId);
            return View(licenseApplicationDetails);
        }

        // GET: InspectionAppeals/Edit/5
        public ActionResult Edit(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var inspectionAppealcount = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == id).Count();

            InspectionAppeal inspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == id).Include(l => l.License).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).FirstOrDefault();

            if (inspectionAppealcount > 1)
            {
                inspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == id).Include(l => l.License).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).OrderByDescending(l => l.InspectionAppealId).FirstOrDefault();
            }

            InspectionResponse InspectionResponse = db.InspectionResponse.Find(inspectionAppeal.InspectionResponseId);






            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            ViewBag.Nationality = new SelectList(new[] { "South African National", "Foreign National", "Asylum Seeker", "Refugee", "Other" }, inspectionAppeal.Nationality);

            ViewBag.IndividualType = new SelectList(new[] { "Agent", "Owner", "Proxy" }, inspectionAppeal.Individual);

            ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town" }, inspectionAppeal.PostalAddress3);
            ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town" }, inspectionAppeal.ResidentialAddress3);
            ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" }, inspectionAppeal.PostalAddress2);
            ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" }, inspectionAppeal.ResidentialAddress2);
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Appealdocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentName == DocumentTypeKeys.AppealName && d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType);

            var uploadedFiles = db.FileUploads.Where(f => f.ClientId == inspectionAppeal.License.ClientId && f.referenceId == inspectionAppeal.InspectionAppealId && f.IsActive == true && f.IsDeleted == false).Include(f => f.Document).ToList();
            var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == InspectionResponse.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionResponse.InspectionRequest.DepartmentId).ToList();
            ViewData["licenceData"] = inspectionAppeal.License;
            ViewData["InspectionRequestData"] = inspectionAppeal.InspectionResponse.InspectionRequest;
            ViewData["InspectionResponsesData"] = inspectionAppeal.InspectionResponse;
            ViewData["inspectionAppealData"] = inspectionAppeal;
            ViewData["Documents"] = document;
            ViewData["UploadList"] = uploadedFiles;
            var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                     .Select(d => d.DocumentTypeId)
                                                     .FirstOrDefault();
            var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == inspectionAppeal.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();

            ViewData["InspectorDocs"] = InspectorDocs;
            ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
            ViewBag.InspectionHistory = InspectionHistory;
            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            ViewBag.InspectionRequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber", inspectionAppeal.InspectionResponse.InspectionRequestId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", inspectionAppeal.LicenseId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", inspectionAppeal.StatusId);
            return View(inspectionAppeal);
        }

        // POST: InspectionAppeals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InspectionAppealId,InspectionRequestId,InspectionResponseId,LicenseId,IdentityOrPassportNumber,Name,Surname,Individual,Nationality,NationalityOther,ExpiryPermit,ResidentialAddress1,ResidentialAddress2,ResidentialAddress3,ResidentialAddressCode,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,TelephoneNumber,CellphoneNumber,FaxNumber,EmailAddress,Reason,AppealDateTime,AppealDate,StatusId,AppealDocDate,SameAs")] InspectionAppeal inspectionAppeal, string DocumentTableData, IEnumerable<HttpPostedFileBase> file, string Nationality)
        {
            Initialise();
            var InspectionResponse = db.InspectionResponse.Where(l => l.InspectionResponseId == inspectionAppeal.InspectionResponseId).Include(l => l.InspectionRequest).FirstOrDefault();
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == "Appeal_document" && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType);
            inspectionAppeal.Nationality = Nationality;
            if (file == null)
            {

                TempData["Error"] = "Please Upload Appeal Refusal Letter";

            }
            else
            {
                if (inspectionAppeal.AppealDate != null)
                {
                    DateTime appealD = DateTime.Parse(inspectionAppeal.AppealDate.ToString());
                    inspectionAppeal.AppealDate = appealD;
                }
                if (ModelState.IsValid)
                {
                    inspectionAppeal.AppealDocDate = DateTime.Now;
                    inspectionAppeal.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AppealReview).Select(s => s.StatusId).FirstOrDefault();

                    db.Entry(inspectionAppeal).State = EntityState.Modified;
                    db.SaveChanges();

                    InspectionResponse.InspectionRequest.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AppealReview).Select(s => s.StatusId).FirstOrDefault();
                    db.Entry(InspectionResponse).State = EntityState.Modified;
                    db.SaveChanges();



                    var status = db.Status.Find(inspectionAppeal.StatusId).StatusName;
                    var InspectionRequest = db.InspectionRequests.Find(inspectionAppeal.InspectionResponse.InspectionRequestId);
                    var department = db.Departments.Find(InspectionRequest.DepartmentId);
                    var license = db.Licenses.Find(inspectionAppeal.LicenseId);
                    var business = db.Businesses.Find(license.BusinessId);
                    var client = db.Clients.Find(license.ClientId);
                    foreach (HttpPostedFileBase fileupload in file)
                    {
                        if (fileupload != null)
                        {

                            //Replaces spaces in file name with '_'



                            int position = fileupload.FileName.LastIndexOf(".", StringComparison.Ordinal);
                            string filename = fileupload.FileName.Substring(0, position);
                            string extension = fileupload.FileName.Substring(position + 1);

                            //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                            const string regExp = @"[^\w\d]";
                            string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                            string name = Regex.Replace(filename, regExp, "_");
                            string contentType = fileupload.ContentType;
                            IdentityManager identifyManager = new IdentityManager();
                            string currentUserId = User.Identity.GetUserId();
                            var user = identifyManager.CurrentUser(currentUserId);
                            var uploadby = user.Username;
                            var CREATION_DATE = "";
                            var fileUrl = "";
                            int fileLen = fileupload.ContentLength;
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(fileupload.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                            }
                            SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Appeals");
                            _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, client.ClientId
                            , client.Fullname, document.First().DocumentId, inspectionAppeal.InspectionAppealId, string.Empty, Users.Username);
                        }
                    }
                    ///
                    try
                    {
                        license = db.Licenses.Where(l => l.LicenseId == inspectionAppeal.LicenseId)
             .Include(l => l.Client)
             .Include(l => l.Business)
             .Include(l => l.LicenseType)
             .Include(l => l.Status)
             .Include(l => l.Region)
             .Include(l => l.ItemType)
             .Include(l => l.ItemSubCategory)
             .Include(l => l.ItemCondition).FirstOrDefault();
                        #region Send Email To Manager
                        var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                        var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                        var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                        var body = String.Empty;
                        var admins = db.Users.Where(c => c.Role == "Licensing Manager").ToList();

                        //LF2014a 
                        //User email helper to get well formatted email.
                        //var mail = new EmailHelper();

                        string actionLink = "<a href=" + Request.Url.Authority + ActionLinkKeys.ReviewInspectionAppeal + inspectionAppeal.InspectionAppealId + ">Click here</a>";
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
                                "<br/> "+actionLink+" to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = user.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "eThekwini Trade Licensing - Application Appeal Feedback:" + license.LicenseNumber,
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
                    return RedirectToAction("Refusal", "License");
                }
            }
            inspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == InspectionResponse.InspectionRequestId).Include(l => l.License).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).FirstOrDefault();
            ViewBag.Nationality = new SelectList(new[] { "South African National", "Foreign National", "Asylum Seeker", "Refugee", "Other" });

            ViewBag.IndividualType = new SelectList(new[] { "Agent", "Owner", "Proxy" }, inspectionAppeal.Individual);

            ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town" }, inspectionAppeal.PostalAddress3);
            ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town" }, inspectionAppeal.ResidentialAddress3);
            ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" }, inspectionAppeal.PostalAddress2);
            ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" }, inspectionAppeal.ResidentialAddress2);
            var uploadedFiles = db.FileUploads.Where(f => f.ClientId == inspectionAppeal.License.ClientId && f.referenceId == inspectionAppeal.InspectionAppealId && f.IsActive == true && f.IsDeleted == false).Include(f => f.Document).ToList();
            var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == InspectionResponse.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionResponse.InspectionRequest.DepartmentId).ToList();
            ViewData["licenceData"] = inspectionAppeal.License;
            ViewData["inspectionAppealData"] = inspectionAppeal;
            ViewData["InspectionRequestData"] = inspectionAppeal.InspectionResponse.InspectionRequest;
            ViewData["InspectionResponsesData"] = inspectionAppeal.InspectionResponse;
            ViewData["Documents"] = document;
            ViewData["UploadList"] = uploadedFiles;
            ViewBag.InspectionHistory = InspectionHistory;
            var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                   .Select(d => d.DocumentTypeId)
                                                   .FirstOrDefault();
            var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == inspectionAppeal.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();
            ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
            ViewData["InspectorDocs"] = InspectorDocs;
            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            ViewBag.InspectionRequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber", InspectionResponse.InspectionRequestId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", inspectionAppeal.LicenseId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", inspectionAppeal.StatusId);
            return View(inspectionAppeal);

        }

        public ActionResult ViewEdit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var inspectionAppealcount = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == id).Count();

            InspectionAppeal inspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == id).Include(l => l.License).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).FirstOrDefault();

            if (inspectionAppealcount > 1)
            {
                inspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == id).Include(l => l.License).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).OrderByDescending(l => l.InspectionAppealId).FirstOrDefault();
            }

            InspectionResponse InspectionResponse = db.InspectionResponse.Find(inspectionAppeal.InspectionResponseId);



            var t = inspectionAppeal.AppealDateTime;


            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == "Appeal_document" && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType);

            var uploadedFiles = db.FileUploads.Where(f => f.ClientId == inspectionAppeal.License.ClientId && f.referenceId == inspectionAppeal.InspectionAppealId && f.IsActive == true && f.IsDeleted == false).Include(f => f.Document).ToList();
            var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == InspectionResponse.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionResponse.InspectionRequest.DepartmentId).ToList();
            ViewData["licenceData"] = inspectionAppeal.License;
            ViewData["InspectionRequestData"] = inspectionAppeal.InspectionResponse.InspectionRequest;
            ViewData["InspectionResponsesData"] = inspectionAppeal.InspectionResponse;
            ViewData["AppealsReviews"] = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == inspectionAppeal.LicenseId).Include(l => l.User).ToList();
            ViewData["Documents"] = document;
            ViewData["UploadList"] = uploadedFiles;
            ViewBag.InspectionHistory = InspectionHistory;
            var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                   .Select(d => d.DocumentTypeId)
                                                   .FirstOrDefault();
            var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == inspectionAppeal.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();
            ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
            ViewData["InspectorDocs"] = InspectorDocs;
            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            ViewBag.InspectionRequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber", inspectionAppeal.InspectionResponse.InspectionRequestId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", inspectionAppeal.LicenseId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", inspectionAppeal.StatusId);
            return View(inspectionAppeal);
        }

        // POST: InspectionAppeals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewEdit([Bind(Include = "InspectionAppealId,InspectionRequestId,InspectionResponseId,LicenseId,IdentityOrPassportNumber,Name,Surname,Individual,Nationality,NationalityOther,ExpiryPermit,ResidentialAddress1,ResidentialAddress2,ResidentialAddress3,ResidentialAddressCode,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,TelephoneNumber,CellphoneNumber,FaxNumber,EmailAddress,Reason,AppealDateTime,AppealDate,StatusId,AppealDocDate")] InspectionAppeal inspectionAppeal, string DocumentTableData, IEnumerable<HttpPostedFileBase> files)
        {
            Initialise();
            var UserId = db.Users.Where(u => u.UserId== Users.UserId).FirstOrDefault();
            var InspectionResponse = db.InspectionResponse.Where(l => l.InspectionResponseId == inspectionAppeal.InspectionResponseId).Include(l => l.InspectionRequest).FirstOrDefault();
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Appealdocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            var document = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType);
            var license = db.Licenses.Find(inspectionAppeal.LicenseId);
            if (files == null)
            {

                TempData["Error"] = "Please Upload Appeal Supporting Documents";

            }
            else if (DocumentTableData =="")
            {
                TempData["Error"] = "something went wrong Please try again";
            }
            else if (ModelState.IsValid)


            {
                Document Doc = new Document();

                char[] spearator = { '*' };
                String[] TableDatalist = DocumentTableData.Split(spearator, StringSplitOptions.None);
                TableDatalist = TableDatalist.Where(t => t != "").ToArray();
                foreach (var item in TableDatalist)
                {
                    char[] separator = { ',' };
                    String[] Data = item.Split(separator, StringSplitOptions.None);
                    Data = Data.Where(t => t != "" && t != " ").ToArray();

                    Doc.DocumentName = Data[0];
                    Doc.DocumentDescription = "Documment uploaded to support an Appeal this document is not pre predetermined";
                    Doc.DocumentKey = "Appeal_Support_Doc";

                    Doc.DocumentTypeId = documentTypeId;

                    Doc.IsActive = true;
                    Doc.IsDeleted = false;
                    Doc.IsLocked = false;

                    Doc.CreatedByUserId = UserId.UserId;
                    Doc.CreatedDateTime = DateTime.Now.Date;



                    db.Documents.Add(Doc);
                    db.SaveChanges();
                    inspectionAppeal.AppealDocDate = DateTime.Now;
                }

                inspectionAppeal.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).Select(s => s.StatusId).FirstOrDefault();

                db.Entry(inspectionAppeal).State = EntityState.Modified;
                db.SaveChanges();

                InspectionResponse.InspectionRequest.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.InspectionApproved).Select(s => s.StatusId).FirstOrDefault();
                db.Entry(InspectionResponse).State = EntityState.Modified;
                db.SaveChanges();



                var status = db.Status.Find(inspectionAppeal.StatusId).StatusName;
                var InspectionRequest = db.InspectionRequests.Find(inspectionAppeal.InspectionResponse.InspectionRequestId);
                var department = db.Departments.Find(InspectionRequest.DepartmentId);

                var business = db.Businesses.Find(license.BusinessId);
                var client = db.Clients.Find(license.ClientId);
                foreach (HttpPostedFileBase fileupload in files)
                {
                    var documentnew = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentKey == "Appeal_Support_Doc" && d.DocumentName == Doc.DocumentName && d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                    if (fileupload != null)
                    {
                        foreach (var doc in documentnew)
                        {

                            //Replaces spaces in file name with '_'
                            var CREATION_DATE = "";
                            var fileUrl = "";
                            int fileLen = fileupload.ContentLength;
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(fileupload.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                            }

                            var fname = fileupload.FileName;
                            var r = "";
                            var Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

                            int position = fileupload.FileName.LastIndexOf(".", StringComparison.Ordinal);
                            string filename = fileupload.FileName.Substring(0, position);
                            string extension = fileupload.FileName.Substring(position + 1);

                            //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                            const string regExp = @"[^\w\d]";
                            string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                            string name = Regex.Replace(filename, regExp, "_");
                            string contentType = fileupload.ContentType;
                            IdentityManager identifyManager = new IdentityManager();
                            string currentUserId = User.Identity.GetUserId();
                            var user = identifyManager.CurrentUser(currentUserId);
                            var uploadby = user.Username;


                            SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Appeals supporting doc");
                            _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, client.ClientId
                            , client.Fullname, doc.DocumentId, inspectionAppeal.InspectionAppealId, string.Empty, Users.Username);
                        }
                    }
                }
                var NotapprovedStatus = db.Status.Where(l => l.StatusKey == StatusKeys.InspectionApproved).Select(s => s.StatusId).FirstOrDefault();                            ///checks if any inspection isn't approved
                var inspectionNotapproved = db.InspectionRequests.Where(i => i.StatusId != NotapprovedStatus && i.LicenseId == license.LicenseId && i.IsActive == true).Count();

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


                        var admins = db.Users.Where(c => c.Role == "Licensing Administrator").ToList();
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
                            body = body.Replace("#BODYTEXT#", " <b>Application Appealed - Awaiting Review. </b><br/><br/> Business Name: " +
                               license.Business.ProposedTradeName + "<br/> " +
                                "<br/> "+actionLink+" to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = user.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "eThekwini Trade Licensing - Application for Appeal Review:" + license.LicenseNumber,
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
                return RedirectToAction("Index");
            }


            inspectionAppeal = db.InspectionAppeal.Where(l => l.InspectionResponse.InspectionRequestId == InspectionResponse.InspectionRequestId).Include(l => l.License).Include(l => l.InspectionResponse).Include(l => l.InspectionResponse.InspectionRequest).FirstOrDefault();

            var uploadedFiles = db.FileUploads.Where(f => f.ClientId == inspectionAppeal.License.ClientId && f.referenceId == inspectionAppeal.InspectionAppealId && f.IsActive == true && f.IsDeleted == false).Include(f => f.Document).ToList();
            var InspectionHistory = db.InspectionHistory.Where(l => l.LicenseId == InspectionResponse.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionResponse.InspectionRequest.DepartmentId).ToList();
            ViewData["licenceData"] = inspectionAppeal.License;
            ViewData["InspectionRequestData"] = inspectionAppeal.InspectionResponse.InspectionRequest;
            ViewData["InspectionResponsesData"] = inspectionAppeal.InspectionResponse;
            ViewData["AppealsReviews"] = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == inspectionAppeal.LicenseId).Include(l => l.User).ToList();
            ViewData["Documents"] = document;
            ViewData["UploadList"] = uploadedFiles;
            ViewBag.InspectionHistory = InspectionHistory;
            var Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                   .Select(d => d.DocumentTypeId)
                                                   .FirstOrDefault();
            var InspectorDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == inspectionAppeal.License.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();
            ViewData["DepartmentContact"] = db.DepartmentContacts.Include(l => l.User).ToList();
            ViewData["InspectorDocs"] = InspectorDocs;
            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            ViewBag.InspectionRequestId = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber", InspectionResponse.InspectionRequestId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", inspectionAppeal.LicenseId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", inspectionAppeal.StatusId);
            return View(inspectionAppeal);

        }


        // GET: InspectionAppeals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectionAppeal inspectionAppeal = db.InspectionAppeal.Find(id);
            if (inspectionAppeal == null)
            {
                return HttpNotFound();
            }
            return View(inspectionAppeal);
        }

        // POST: InspectionAppeals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectionAppeal inspectionAppeal = db.InspectionAppeal.Find(id);
            db.InspectionAppeal.Remove(inspectionAppeal);
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
