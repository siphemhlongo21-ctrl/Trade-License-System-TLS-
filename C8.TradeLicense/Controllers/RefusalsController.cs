using C8.TradeLicense.DAL;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using C8.TradeLicense.Helpers;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using C8.TradeLicense.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Controllers
{
    [Authorize]
    public class RefusalsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private CesarDbContext core = new CesarDbContext();
        private RefusalHelpers _refusalHelpers;
        private FileHelpers _fileHelpers;
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

        public RefusalsController()
        {
            Initialise();
            _refusalHelpers = new RefusalHelpers(db);
            _fileHelpers = new FileHelpers(db);
        }
        // GET: Refusals
        public ActionResult Index(int LicenseID,string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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
                int AppealApprovedStatus = db.Status.Where(s => s.StatusKey == StatusKeys.AppealApproved).Select(s => s.StatusId)
           .FirstOrDefault();


                int inspectionOverdue = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.InspectionOverdue)?.StatusId ?? 0;
                int AppealReviewStatus = db.Status.Where(s => s.StatusKey == StatusKeys.AppealReview).Select(s => s.StatusId)
               .FirstOrDefault();

                int InspectionAppealedStatus = db.Status.Where(s => s.StatusKey == StatusKeys.InspectionAppealed).Select(s => s.StatusId)
               .FirstOrDefault();

                    int InspectionFailedStatus = db.Status.Where(s => s.StatusKey == StatusKeys.InspectionFailed).Select(s => s.StatusId)
                    .FirstOrDefault();
                    var InspectionRequests = db.InspectionRequests.Where(i => i.LicenseId == LicenseID && (i.StatusId == InspectionFailedStatus || i.StatusId == InspectionAppealedStatus || i.StatusId == AppealApprovedStatus || i.StatusId == AppealReviewStatus|| i.StatusId == inspectionOverdue)).Include(l => l.License).Include(l => l.License.Business).Include(l => l.Client).Include(l => l.CreatedByUser).Include(l => l.LicenseType).Include(l => l.ModifiedByUser).Include(l => l.Status).Include(l => l.Department).Include(l => l.License.Business).ToList();


                ;
                ViewData["Refusallist"] = db.Refusa.Where(L=> L.LicenseId == LicenseID).Include(l=> l.Status).ToList();


                if (selectedSearch != null)
                    {
                        switch (selectedSearch)
                        {
                            case "ClientName":
                            //Search by client name
                            InspectionRequests = InspectionRequests.Where(i => (i.Client.Name.ToLower() + " " + i.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                                break;
                            case "BusinessName":
                            //Search by business name
                            InspectionRequests = InspectionRequests.Where(i => i.License.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                                break;
                            default:
                            InspectionRequests = InspectionRequests.ToList();
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
                    
                    ViewBag.InspectionResponseCount = InspectionRequests.Count();

                    return View(InspectionRequests.ToPagedList(pageNumber, pageSize));
                
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: Refusals/Details/5
        public ActionResult Details(int id,int refusalId)
        {           
            LicenseApplicationDetails licenseApplicationDetails = _refusalHelpers.GetRefusal(id,refusalId);
            return View(licenseApplicationDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(LicenseApplicationDetails licenseApplicationDetails)
        {
            Refusal refusal = licenseApplicationDetails.RefusalDetails;
            if (ModelState.IsValid)
            {
                refusal.IsActive = true;
                refusal.IsDeleted = false;
                refusal.IsLocked = false;
                refusal.StatusId =
                           db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingRefusalReport).Select(s => s.StatusId).
                               FirstOrDefault();
                db.Entry(refusal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { refusal.LicenseId });
            }
            return RedirectToAction("Details",new { refusal.InspectionRequestId, refusal.RefusalId });
        }
        // GET: Refusals/Create

        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Licensing Clerk")]
        public ActionResult Create(int id)

        {
            LicenseApplicationDetails licenseApplicationDetails = _refusalHelpers.GetRefusal(id);
            return View(licenseApplicationDetails);
        }

        // POST: Refusals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Licensing Clerk")]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails, IEnumerable<HttpPostedFileBase> file)
        {
            Initialise();
            int inspectionRequestId = licenseApplicationDetails.InspectionRequestDetails?.InspectionRequestId ?? 0;
            int licenseId = licenseApplicationDetails.LicenseDetails?.LicenseId ?? 0;
            InspectionResponse InspectionResponses = db.InspectionResponse.Where(l => l.InspectionRequestId == inspectionRequestId)

      .Include(l => l.License)
      .Include(l => l.License.Region)
      .Include(l => l.License.ItemType)
      .Include(l => l.License.ItemSubCategory)
      .Include(l => l.License.ItemCondition)
          .Include(l => l.License.Business)
              .Include(l => l.License.Client)
      .Include(l => l.InspectionRequest)
      .Include(l => l.InspectionRequest.Department)
      .Include(l => l.InspectionRequest.Status)

      .FirstOrDefault();
            var DepartmentContact = db.DepartmentContacts.Where(d => d.DepartmentContactId == InspectionResponses.InspectionRequest.DepartmentContactId).Include(l => l.User).FirstOrDefault();
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Refusaldocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            if (file.First() == null)
            {
                TempData["Error"] = "Please Upload Refusal Report";
                licenseApplicationDetails = _refusalHelpers.GetRefusal(inspectionRequestId);
                return View(licenseApplicationDetails);
            }
            if (ModelState.IsValid)
            {
                Refusal refusal = new Refusal();
                refusal.InspectionRequestId = inspectionRequestId;
                refusal.InspectionResponseId = InspectionResponses.InspectionResponseId;
                refusal.LicenseId = licenseId;
                var document = db.Documents.Include(d => d.DocumentType).First(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);
                refusal.IsActive = true;
                refusal.IsDeleted = false;
                refusal.IsLocked = false;
                refusal.StatusId =
                           db.Status.Where(s => s.StatusKey == StatusKeys.RefusalReportUploaded).Select(s => s.StatusId). 
                               FirstOrDefault();
                db.Refusa.Add(refusal);
                db.SaveChanges();

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

                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(fileupload.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                        }

                        var Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

                        SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Refusals");
                        _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, InspectionResponses.License.Client.ClientId
                        , InspectionResponses.License.Client.Fullname, document.DocumentId, refusal.RefusalId, string.Empty, Users.Username);
                    }
                    ///
                    try
                    {

                        #region Send Email To Cheif
                        var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                        var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                        var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                        var body = String.Empty;
                        var admins = db.Users.Where(c => c.Role == "Chief Inspector" && c.Region == InspectionResponses.License.RegionId).ToList();

                      
                        if (InspectionResponses.License.Region.RegionKey == TLKeys.metro_south)
                        {
                            int metro_CentralSouth = db.Regions.Where(s => s.RegionKey == TLKeys.metro_CentralSouth).Select(s => s.RegionId)
                                .FirstOrDefault();
                            admins = db.Users.Where(c => c.Role == "Chief Inspector" && c.Region == metro_CentralSouth).ToList();
                        }
                        else if (InspectionResponses.License.Region.RegionKey == TLKeys.metro_central)
                        {
                            int metro_CentralSouth = db.Regions.Where(s => s.RegionKey == TLKeys.metro_CentralSouth).Select(s => s.RegionId)
                                .FirstOrDefault();
                            admins = db.Users.Where(c => c.Role == "Chief Inspector" && c.Region == metro_CentralSouth).ToList();
                        }

                        else if (InspectionResponses.License.Region.RegionKey == TLKeys.metro_north)
                        {
                            int metro_NorthWest = db.Regions.Where(s => s.RegionKey == TLKeys.metro_NorthWest).Select(s => s.RegionId)
     .FirstOrDefault();
                            admins = db.Users.Where(c => c.Role == "Chief Inspector" && c.Region == metro_NorthWest).ToList();
                        }

                        else if (InspectionResponses.License.Region.RegionKey == TLKeys.meto_west)
                        {
                            int metro_NorthWest = db.Regions.Where(s => s.RegionKey == TLKeys.metro_NorthWest).Select(s => s.RegionId)
     .FirstOrDefault();
                            admins = db.Users.Where(c => c.Role == "Chief Inspector" && c.Region == metro_NorthWest).ToList();
                        }

                        //LF2014a 
                        //User email helper to get well formatted email.
                        //var mail = new EmailHelper();

                        string actionLink = "<a href=" + Request.Url.Authority + ActionLinkKeys.ViewRefusal + refusal.InspectionRequestId + "?refusalId=" + refusal.RefusalId + ">Click here</a>";
                        foreach (var admin in admins)
                        {
                            if (template != null)
                            {
                                body = template.EmailBody;
                            }
                            //L.M.20150303a - Replace variables with actual email content
                            var user = admin;
                            body = body.Replace("#NAME#", user.FullName);
                            body = body.Replace("#BODYTEXT#", " <b>Refusal Letter Uploaded for: </b><br/><br/> Business Name: " +
                               InspectionResponses.License.Business.ProposedTradeName + "<br/> " +
                                actionLink + " to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = user.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "eThekwini Trade Licensing - Refusal Letter Uploaded:" + InspectionResponses.License.LicenseNumber,
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
                    return RedirectToAction("Index",new { LicenseID = InspectionResponses.LicenseId });
                }
            }

            licenseApplicationDetails = _refusalHelpers.GetRefusal(licenseApplicationDetails.InspectionRequestDetails.InspectionRequestId);
            return View(licenseApplicationDetails);
        }

        // GET: Refusals/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Licensing Clerk")]
        public ActionResult Edit(int id,int refusalId)
        {
            LicenseApplicationDetails licenseApplicationDetails = _refusalHelpers.GetRefusal(id, refusalId);
            return View(licenseApplicationDetails);
        }

        // POST: Refusals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Licensing Clerk")]
        public ActionResult Edit(LicenseApplicationDetails licenseApplicationDetails, IEnumerable<HttpPostedFileBase> file)
        {
            Initialise();
            Refusal refusal = licenseApplicationDetails.RefusalDetails;
            InspectionResponse InspectionResponses = db.InspectionResponse.Where(l => l.InspectionRequestId == refusal.InspectionRequestId)

     .Include(l => l.License)
     .Include(l => l.License.Region)
     .Include(l => l.License.ItemType)
     .Include(l => l.License.ItemSubCategory)
     .Include(l => l.License.ItemCondition)
         .Include(l => l.License.Business)
             .Include(l => l.License.Client)
     .Include(l => l.InspectionRequest)
     .Include(l => l.InspectionRequest.Department)
     .Include(l => l.InspectionRequest.Status)

     .FirstOrDefault();
            var DepartmentContact = db.DepartmentContacts.Where(d => d.DepartmentContactId == InspectionResponses.InspectionRequest.DepartmentContactId).Include(l => l.User).FirstOrDefault();
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == "Signed_Refusall_ Document" && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            if (file.First() == null)
            {

                TempData["Error"] = "Please Upload Signed Refusal Report";
            }

                if (ModelState.IsValid)
            {
                var document = db.Documents.Include(d => d.DocumentType).First(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);
                refusal.DateLog = DateTime.Now.Date;
                refusal.IsActive = true;
                refusal.IsDeleted = false;
                refusal.IsLocked = false;
                refusal.StatusId =
                           db.Status.Where(s => s.StatusKey == StatusKeys.SignedRefusalReportUploaded).Select(s => s.StatusId).
                               FirstOrDefault(); 
                db.Entry(refusal).State = EntityState.Modified;
                db.SaveChanges();
                AddDaysToDate AD = new AddDaysToDate();

                var InpectionRequest = db.InspectionRequests.Find(refusal.InspectionRequestId);
                var AppealFinalDateCalc = AD.AddDays(DateTime.Now.Date, 21);
                InpectionRequest.AppealFinalDate = DateTime.Parse(AppealFinalDateCalc.ToString());
                db.Entry(InpectionRequest).State = EntityState.Modified;
                db.SaveChanges();
                foreach (HttpPostedFileBase fileupload in file)
                {
                    if (fileupload != null)
                    {
                        IdentityManager identifyManager = new IdentityManager();
                        string currentUserId = User.Identity.GetUserId();
                        var user = identifyManager.CurrentUser(currentUserId);
                       
       
                        //Replaces spaces in file name with '_'
                   
                     

                        int position = fileupload.FileName.LastIndexOf(".", StringComparison.Ordinal);
                        string filename = fileupload.FileName.Substring(0, position);
                        string extension = fileupload.FileName.Substring(position + 1);

                        //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                        const string regExp = @"[^\w\d]";
                        string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                        string name = Regex.Replace(filename, regExp, "_");
                        string contentType = fileupload.ContentType;

                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(fileupload.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                        }
                    
                        SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Refusals");
                        _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, InspectionResponses.License.Client.ClientId
                        , InspectionResponses.License.Client.Fullname, document.DocumentId, refusal.RefusalId, string.Empty, Users.Username);
                    }
                }

                        return RedirectToAction("Index", new { refusal.LicenseId });
            }
         
                    return RedirectToAction("Edit", new { refusal.InspectionRequestId, refusal.RefusalId });
          
        }

        // GET: Refusals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Refusal refusal = db.Refusa.Find(id);
            if (refusal == null)
            {
                return HttpNotFound();
            }
            return View(refusal);
        }

        // POST: Refusals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Refusal refusal = db.Refusa.Find(id);
            db.Refusa.Remove(refusal);
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
