using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using C8.TradeLicense.Helpers;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using License = C8.TradeLicense.Models.License;

namespace C8.TradeLicense.Controllers
{
    public class AdministratorReviewsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private AdministratorReviewHelpers _administratorReviewHelpers;
        private int EmailAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["EmailAccountId"]);
        private CesarDbContext core = new CesarDbContext();
        public IdentityManager IdentityManager { get; set; }

        public AdministratorReviewsController()
        {
            _administratorReviewHelpers = new AdministratorReviewHelpers(db);
        }
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

        // GET: AdministratorReviews
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
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

            int pendingReview = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).Select(s => s.StatusId)
                    .FirstOrDefault();
            int allregion = db.Regions.Where(s => s.RegionKey == TLKeys.metro_all).Select(s => s.RegionId)
                   .FirstOrDefault();
            int metro_NorthWest = db.Regions.Where(s => s.RegionKey == TLKeys.metro_NorthWest).Select(s => s.RegionId)
                .FirstOrDefault();
            int metro_CentralSouth = db.Regions.Where(s => s.RegionKey == TLKeys.metro_CentralSouth).Select(s => s.RegionId)
                .FirstOrDefault();


            var Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview && l.RegionId == Users.Region).OrderBy(l => l.Status.StatusName).ToList();
            if (Users.Region == allregion)
            {
                Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_CentralSouth)

            {
                int metro_south = db.Regions.Where(s => s.RegionKey == TLKeys.metro_south).Select(s => s.RegionId)
       .FirstOrDefault();
                int metro_central = db.Regions.Where(s => s.RegionKey == TLKeys.metro_central).Select(s => s.RegionId)
    .FirstOrDefault();
                Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview && (l.RegionId == metro_south || l.RegionId == metro_central)).OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_NorthWest)

            {
                int metro_north = db.Regions.Where(s => s.RegionKey == TLKeys.metro_north).Select(s => s.RegionId)
.FirstOrDefault();
                int meto_west = db.Regions.Where(s => s.RegionKey == TLKeys.meto_west).Select(s => s.RegionId)
    .FirstOrDefault();
                Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview && (l.RegionId == meto_west || l.RegionId == metro_north)).OrderBy(l => l.Status.StatusName).ToList();
            }

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

        // GET: AdministratorReviews/Details/5
        public ActionResult Details(int id)
        {
            Initialise();
            User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
            LicenseApplicationDetails licenseApplicationDetails = _administratorReviewHelpers.GetAdministratorCreateVM(id, UserId);
            return View(licenseApplicationDetails);
        }

        // GET: AdministratorReviews/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create(int id)
        {
            try
            {
                Initialise();
                // If not pending Administrator review, redirect to details page
                Status pendingAdministratorReview = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).FirstOrDefault();
                License license = db.Licenses.Where(l => l.LicenseId == id).FirstOrDefault();
                if (license.StatusId != pendingAdministratorReview.StatusId)
                {
                    return RedirectToAction("ApplicationTrackerDetails", "License", new { id });
                }

                User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                LicenseApplicationDetails licenseApplicationDetails = _administratorReviewHelpers.GetAdministratorCreateVM(id, UserId);
                licenseApplicationDetails.AdministratorReviewDetails = null;
                return View(licenseApplicationDetails);
            }
            catch (Exception e)
            {
                return View(TempData[LicenseApplicationDetails.ErrorKey]);
            }
        }

        // POST: AdministratorReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails, string Userid)
        {
            Initialise();
            AdministratorReview administratorReview = licenseApplicationDetails?.AdministratorReviewDetails ?? null;
            int licenseId = licenseApplicationDetails?.LicenseDetails?.LicenseId ?? 0;
            User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();



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
            if (ModelState.IsValid && administratorReview != null && license != null)
            {
                administratorReview.UserId = Int32.Parse(Userid);
                administratorReview.LicenseId = licenseId;

                administratorReview.IsActive = true;
                administratorReview.IsDeleted = false;
                administratorReview.IsLocked = false;
                db.AdministratorReviews.Add(administratorReview);
                db.SaveChanges();
                if (administratorReview.Decision == TLKeys.NotRecommended)
                {
                    license.StatusId =
db.Status.Where(s => s.StatusKey == StatusKeys.AdministratorRejected).Select(s => s.StatusId).
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
                            ;
                            //L.M.20150303a - Replace variables with actual email content
                            var user = admin;
                            body = body.Replace("#NAME#", user.FullName);
                            body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Administrator for: </b><br/><br/> Business Name: " +
                               license.Business.ProposedTradeName + "<br/> " +
                                "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = EmailAccountId,
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
    db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingChiefReview).Select(s => s.StatusId).
    FirstOrDefault();
                }
                try
                {
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                ///
                try
                {

                    #region Send Email To Cheif
                    var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                    var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                    var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                    var body = String.Empty;

                    var depContacts = new List<User>();
                    var centralSouthRegion = db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_CentralSouth);
                    var nortWestRegion = db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_NorthWest);
                    var central = db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_central);
                    var south = db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_south);

                    switch (license.Region.RegionKey)
                    {
                        case TLKeys.metro_central:
                        case TLKeys.metro_south:
                            depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && d.Role == RoleKeys.ChiefInspector && (d.Region == central.RegionId || d.Region == centralSouthRegion.RegionId)).ToList();
                            break;
                        case TLKeys.metro_north:
                        case TLKeys.meto_west:
                            depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && d.Role == RoleKeys.ChiefInspector && (d.Region == south.RegionId || d.Region == nortWestRegion.RegionId)).ToList();
                            break;
                        default:
                            depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && d.Role == RoleKeys.ChiefInspector && d.Region == license.RegionId).ToList();
                            break;
                    }

                    string actionLink = Url.Action("Create", "ChiefReviews", new { id = license.LicenseId }, Request.Url.Scheme);
                    foreach (var admin in depContacts)
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
                            "<br/>  <a href =" + actionLink + " > Click here </ a > to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = EmailAccountId,
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
                licenseApplicationDetails = _administratorReviewHelpers.GetAdministratorCreateVM(licenseId, UserId);
                return View(licenseApplicationDetails);
            }
        }

        // GET: AdministratorReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            Initialise();
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
                AdministratorReview AdministratorReview = db.AdministratorReviews.Where(l => l.LicenseId == id).FirstOrDefault();


                var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
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




                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.InspectionRequests = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");


                return View(AdministratorReview);
            }
            catch (Exception e)
            {
                return View("Error");
            }

        }

        // POST: AdministratorReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdministratorReviewId,LicenseId,UserId,Decision,Comment,DateReviewed,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] AdministratorReview administratorReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(administratorReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", administratorReview.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", administratorReview.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", administratorReview.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", administratorReview.UserId);
            return View(administratorReview);
        }

        // GET: AdministratorReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdministratorReview administratorReview = db.AdministratorReviews.Find(id);
            if (administratorReview == null)
            {
                return HttpNotFound();
            }
            return View(administratorReview);
        }

        // POST: AdministratorReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdministratorReview administratorReview = db.AdministratorReviews.Find(id);
            db.AdministratorReviews.Remove(administratorReview);
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
