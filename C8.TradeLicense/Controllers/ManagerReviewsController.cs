using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using C8.TradeLicense.Helpers;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using C8.TradeLicense.ViewModels;
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
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using License = C8.TradeLicense.Models.License;

namespace C8.TradeLicense.Controllers
{
    [Authorize]
    public class ManagerReviewsController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private CesarDbContext core = new CesarDbContext();
        private ManagerReviewHelpers _managerReviewHelpers;
        private int EmailAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["EmailAccountId"]);
        public ManagerReviewsController()
        {
            _managerReviewHelpers = new ManagerReviewHelpers(db);
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

        // GET: ManagerReviews
        [Authorize(Roles = "Licensing Manager" + "," + "Licensingt Clerk" + "," + "System Admin")]
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

            int pendingReview = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerReview).Select(s => s.StatusId)
                    .FirstOrDefault();
            int otherreview = db.Status.Where(s => s.StatusKey == StatusKeys.RevokeCancelAwaitingManagerReview).Select(s => s.StatusId)
                    .FirstOrDefault();
            int allregion = db.Regions.Where(s => s.RegionKey == TLKeys.metro_all).Select(s => s.RegionId)
             .FirstOrDefault();
            int metro_NorthWest = db.Regions.Where(s => s.RegionKey == TLKeys.metro_NorthWest).Select(s => s.RegionId)
                .FirstOrDefault();
            int metro_CentralSouth = db.Regions.Where(s => s.RegionKey == TLKeys.metro_CentralSouth).Select(s => s.RegionId)
                .FirstOrDefault();
            var Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && (l.StatusId == pendingReview || l.StatusId == otherreview) && l.RegionId == Users.Region).OrderBy(l => l.Status.StatusName).ToList();
            if (Users.Region == allregion)
            {
                Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && (l.StatusId == pendingReview || l.StatusId == otherreview)).OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_CentralSouth)

            {
                int metro_south = db.Regions.Where(s => s.RegionKey == TLKeys.metro_south).Select(s => s.RegionId)
       .FirstOrDefault();
                int metro_central = db.Regions.Where(s => s.RegionKey == TLKeys.metro_central).Select(s => s.RegionId)
    .FirstOrDefault();
                Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && (l.StatusId == pendingReview || l.StatusId == otherreview) && (l.RegionId == metro_central || l.RegionId == metro_south)).OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_NorthWest)

            {
                int metro_north = db.Regions.Where(s => s.RegionKey == TLKeys.metro_north).Select(s => s.RegionId)
.FirstOrDefault();
                int meto_west = db.Regions.Where(s => s.RegionKey == TLKeys.meto_west).Select(s => s.RegionId)
    .FirstOrDefault();
                Licenses = db.Licenses.Include(l => l.Business).Include(l => l.Region).Include(l => l.Status).Include(l => l.LicenseType).Include(l => l.Client).Where(l => l.IsDeleted == false && l.IsActive == true && (l.StatusId == pendingReview || l.StatusId == otherreview) && (l.RegionId == meto_west || l.RegionId == metro_north)).OrderBy(l => l.Status.StatusName).ToList();
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

        [Authorize(Roles = "Department Manager" + "," + "Department Clerk" + "," + "System Admin" + "," + "Licensing Manager" + "," + "Licensing Clerk")]
        public ActionResult ViewDetails(int id)
        {
            try
            {
                Initialise();
                return View(_managerReviewHelpers.GetViewDetailsVM(id));
            }
            catch (Exception e)
            {
                return View(TempData[LicenseApplicationDetails.ErrorKey]);
            }
        }

        [Authorize(Roles = "Department Manager" + "," + "Department Clerk" + "," + "System Admin" + "," + "Licensing Manager" + "," + "Licensing Clerk")]
        public ActionResult InspectorsResponseIndex(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            Initialise();
            var department = db.DepartmentContacts.FirstOrDefault(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false);

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

            int InspectionPending = db.Status.Where(s => s.StatusKey == StatusKeys.PendingLicenseInspection).Select(s => s.StatusId)
                    .FirstOrDefault();
            int AwaitingManagerResponse = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerResponse).Select(s => s.StatusId)
                   .FirstOrDefault();
            int allregion = db.Regions.Where(s => s.RegionKey == TLKeys.metro_all).Select(s => s.RegionId)
             .FirstOrDefault();
            var InspectionRequest = db.InspectionRequests.Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsActive == false).OrderBy(l => l.License.Status.StatusName).ToList();

            if ((User.IsInRole("System Admin")))
            {
                if (department != null)
                {
                    InspectionRequest = db.InspectionRequests.Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.License.Status.StatusKey != StatusKeys.PendingLicenseInspection && l.License.Status.StatusKey != StatusKeys.AwaitingManagerResponse && l.IsActive == false && l.IsDeleted == true && l.License.Status.StatusKey != StatusKeys.LicenceApplicationCancelled && l.License.Status.StatusKey != StatusKeys.AppealAbandoned).OrderBy(l => l.License.Status.StatusName).OrderBy(l => l.License.Status.StatusName).ToList();

                }
            }
            else if (Users.Region == allregion)
            {
                var departmentlist = db.DepartmentContacts.Where(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false).ToList();
                if (departmentlist.Count > 0)
                {
                    InspectionRequest = new List<Models.InspectionRequest>();
                    foreach (var dept in departmentlist)
                    {


                        var data = db.InspectionRequests.Where(l => l.DepartmentId == dept.DepartmentId).Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.License.Status.StatusKey != StatusKeys.PendingLicenseInspection && l.License.Status.StatusKey != StatusKeys.AwaitingManagerResponse && l.IsActive == false && l.IsDeleted == true && l.License.Status.StatusKey != StatusKeys.LicenceApplicationCancelled && l.License.Status.StatusKey != StatusKeys.AppealAbandoned).OrderBy(l => l.License.Status.StatusName).ToList();
                        foreach (var item in data)
                        {
                            InspectionRequest.Add(item);
                        }
                    }
                }
            }

            var DepartmentContacts = db.DepartmentContacts.Where(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false).Select(s => s.DepartmentContactId).FirstOrDefault();

            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        InspectionRequest = InspectionRequest.Where(l => (l.License.Client.Name.ToLower() + " " + l.License.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number

                        InspectionRequest = InspectionRequest.Where(l => l.License.LicenseNumber == inputSearch).ToList();


                        //licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        InspectionRequest = InspectionRequest.Where(l => l.License.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        InspectionRequest = InspectionRequest.Where(l => l.License.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                }
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == InspectionPending).OrderBy(c => c.StatusName), "StatusId", "StatusName");

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

            ViewBag.LicenseCount = InspectionRequest.Count();
            ViewBag.Licenses = InspectionRequest;
            return View(InspectionRequest.ToPagedList(pageNumber, pageSize));
        }
        [Authorize(Roles = "Department Manager" + "," + "Department Clerk" + "," + "System Admin" + "," + "Licensing Manager" + "," + "Licensing Clerk" + "," + "Chief Inspector")]
        public ActionResult DepartmentManagerIndex(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            Initialise();

            var department = db.DepartmentContacts.FirstOrDefault(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false);

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
            int pendinginspection = db.Status.Where(s => s.StatusKey == StatusKeys.PendingLicenseInspection).Select(s => s.StatusId)
                  .FirstOrDefault();
            int pendingReview = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerResponse).Select(s => s.StatusId)
                    .FirstOrDefault();
            int allregion = db.Regions.Where(s => s.RegionKey == TLKeys.metro_all).Select(s => s.RegionId)
             .FirstOrDefault();
            var InspectionRequest = db.InspectionRequests.Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.License.Status.StatusName).ToList();

            if ((User.IsInRole("System Admin")))
            {

                InspectionRequest = db.InspectionRequests.Include(l => l.Department).Include(l => l.Status).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && (l.StatusId == pendingReview) || (l.StatusId == pendinginspection)).OrderBy(l => l.License.Status.StatusName).ToList();


            }
            else if (Users.Region == allregion)
            {
                var departmentlist = db.DepartmentContacts.Where(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false).ToList();
                if (departmentlist.Count > 0)
                {
                    InspectionRequest = new List<Models.InspectionRequest>();
                    foreach (var dept in departmentlist)
                    {


                        var data = db.InspectionRequests.Where(l => l.DepartmentId == dept.DepartmentId).Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.License.Status.StatusName).ToList();
                        foreach (var item in data)
                        {
                            InspectionRequest.Add(item);
                        }
                    }
                }
            }
            else
            {
                if (department != null)
                {
                    InspectionRequest = db.InspectionRequests.Where(l => l.DepartmentId == department.DepartmentId).Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.License.Status.StatusName).ToList();
                }
                else if (Users.Role.Contains("Chief Inspector"))
                {
                    var getregion = db.Regions.Where(r => r.RegionId == Users.Region && r.IsActive == true && r.IsDeleted == false).FirstOrDefault();
                    var getdepartment = db.Departments.Where(d => d.DepartmentName.StartsWith("Business Licensing") && d.IsActive == true && d.IsDeleted == false && d.RegionId == Users.Region).FirstOrDefault();
                    if (getregion.RegionKey == TLKeys.metro_CentralSouth && getdepartment == null)
                    {
                        var region = db.Regions.Where(r => r.RegionKey == TLKeys.metro_central || r.RegionKey == TLKeys.metro_south && r.IsActive == true && r.IsDeleted == false).ToArray();
                        var central = region[0].RegionId;
                        var south = region[1].RegionId;
                        var Business = db.Departments.Where(d => d.DepartmentName.Contains("Business") && d.IsActive == true && d.IsDeleted == false).ToList();
                        var departmentlist = Business.Where(d => d.RegionId == central || d.RegionId == south).ToList();
                        if (departmentlist.Count > 0)
                        {
                            InspectionRequest = new List<Models.InspectionRequest>();
                            foreach (var dept in departmentlist)
                            {


                                var data = db.InspectionRequests.Where(l => l.DepartmentId == dept.DepartmentId).Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.License.Status.StatusName).ToList();
                                foreach (var item in data)
                                {
                                    InspectionRequest.Add(item);
                                }
                            }
                        }

                    }
                    else if (getregion.RegionKey == TLKeys.metro_NorthWest && getdepartment == null)
                    {
                        var region = db.Regions.Where(r => r.RegionKey == TLKeys.metro_north || r.RegionKey == TLKeys.meto_west && r.IsActive == true && r.IsDeleted == false).ToArray();
                        var north = region[0].RegionId;
                        var west = region[1].RegionId;
                        var Business = db.Departments.Where(d => d.DepartmentName.Contains("Business") && d.IsActive == true && d.IsDeleted == false).ToList();
                        var departmentlist = Business.Where(d => d.RegionId == north || d.RegionId == west).ToList();
                        if (departmentlist.Count > 0)
                        {
                            InspectionRequest = new List<Models.InspectionRequest>();
                            foreach (var dept in departmentlist)
                            {


                                var data = db.InspectionRequests.Where(l => l.DepartmentId == dept.DepartmentId).Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.License.Status.StatusName).ToList();
                                foreach (var item in data)
                                {
                                    InspectionRequest.Add(item);
                                }
                            }
                        }

                    }
                    else
                    {
                        InspectionRequest = db.InspectionRequests.Where(l => l.DepartmentId == getdepartment.DepartmentId).Include(l => l.Department).Include(l => l.License.Business).Include(l => l.License.Region).Include(l => l.License.Status).Include(l => l.LicenseType).Include(l => l.Client).Include(l => l.License).Where(l => l.IsDeleted == false && l.IsActive == true && l.StatusId == pendingReview).OrderBy(l => l.License.Status.StatusName).ToList();
                    }


                }
            }

            var DepartmentContacts = db.DepartmentContacts.Where(d => d.UserId == Users.UserId && d.IsActive == true && d.IsDeleted == false).Select(s => s.DepartmentContactId).FirstOrDefault();

            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        InspectionRequest = InspectionRequest.Where(l => (l.License.Client.Name.ToLower() + " " + l.License.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number

                        InspectionRequest = InspectionRequest.Where(l => l.License.LicenseNumber == inputSearch).ToList();


                        //licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        InspectionRequest = InspectionRequest.Where(l => l.License.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        InspectionRequest = InspectionRequest.Where(l => l.License.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
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

            ViewBag.LicenseCount = InspectionRequest.Count();
            ViewBag.Licenses = InspectionRequest;
            return View(InspectionRequest.ToPagedList(pageNumber, pageSize));
        }

        // GET: ManagerReviews/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
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
                ManagerReview managerReview = db.ManagerRevies.Where(l => l.LicenseId == id).OrderByDescending(o => o.ManagerReviewId).FirstOrDefault();

                User UserId = db.Users.Where(u => u.UserId == managerReview.UserId).FirstOrDefault();
                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
                licenseApplicationDetails.ItemTypeDetails = license.ItemType;
                licenseApplicationDetails.RegionDetails = license.Region;
                licenseApplicationDetails.ChiefReviews = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ManagerReviews = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ManagerReviewDetails = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.InspectionRequests = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
                licenseApplicationDetails.InspectionResponses = db.InspectionResponse.Where(l => l.LicenseId == id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                licenseApplicationDetails.AdministratorReviews = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviews = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviewsDetails = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();

                int Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                          .Select(d => d.DocumentTypeId)
                                          .FirstOrDefault();
                List<FileUpload> InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();

                licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;
                int InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                       .Select(d => d.DocumentTypeId)
                                       .FirstOrDefault();
                licenseApplicationDetails.InspectionRequestUploadList = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId && d.IsDeleted == false && d.IsActive).ToList();
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
        // GET: DepartmentManager/Create
        public ActionResult DepartmentManagerReassign(int? Id, int? DepartmentId)
        {
            try
            {



                var inspectorList = db.DepartmentContacts.Include(l => l.User).Where(l => l.IsDeleted == false && l.IsActive == true && l.DepartmentId == DepartmentId && l.RoleName == "Department Inspector").ToList();
                var inspectionRequest = db.InspectionRequests.Where(l => l.IsDeleted == false && l.IsActive == true && l.DepartmentId == DepartmentId && l.InspectionRequestId == Id).Include(l => l.Department).Include(l => l.Status).FirstOrDefault();


                License license = db.Licenses.Where(l => l.LicenseId == inspectionRequest.LicenseId)
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

                var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
                var UploadedDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.referenceId == Id && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive).ToList();
                ViewData["BusinessData"] = license.Business;
                ViewData["ClientData"] = license.Client;
                ViewData["licenceData"] = license;
                ViewData["InspectionRequest"] = inspectionRequest;
                ViewData["LicenseType"] = license.LicenseType;
                ViewData["ItemType"] = license.ItemType;
                ViewData["Region"] = license.Region;
                ViewData["UploadList"] = UploadedDocs;

                List<SelectListItem> inspectorlist = new List<SelectListItem>();
                foreach (var item in inspectorList)
                {
                    inspectorlist.Add(new SelectListItem() { Value = item.DepartmentContactId.ToString(), Text = item.User.FullName });
                }

                ViewBag.inspector = new SelectList(inspectorlist, "Value", "Text", inspectionRequest.DepartmentContactId);

                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.InspectionRequests = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");


                return View();
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        // POST: DepartmentManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmentManagerReassign([Bind(Include = "ManagerReviewId,UserId,Decision,Comment,DateReviewed,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,InspectionRequestId")] ManagerReview managerReview, int InspectionRequestId, string Inspector, string Userid)
        {


            var InspectionRequest = db.InspectionRequests.Where(l => l.IsDeleted == false && l.IsActive == true && l.InspectionRequestId == InspectionRequestId).Include(l => l.License).FirstOrDefault();



            if (ModelState.IsValid)
            {



                InspectionRequest.DepartmentContactId = Int32.Parse(Inspector);
                InspectionRequest.StatusId =
db.Status.Where(s => s.StatusKey == StatusKeys.PendingLicenseInspection).Select(s => s.StatusId).
FirstOrDefault();

                db.Entry(InspectionRequest).State = EntityState.Modified;
                db.SaveChanges();

                //var InspectionRequestCount = db.InspectionRequests.Where(l => l.IsDeleted == false && l.IsActive == true && l.LicenseId == InspectionRequest.LicenseId && l.StatusId == 28).Count();
                //if (InspectionRequestCount == 0)
                //{
                //                InspectionRequest.License.StatusId =
                //db.Status.Where(s => s.StatusKey == StatusKeys.PendingLicenseInspection).Select(s => s.StatusId).
                //FirstOrDefault();
                //                db.Entry(InspectionRequest).State = EntityState.Modified;
                //                db.SaveChanges();
                //}

                //Sends out emails to  and inspectors departments.
                #region Construct emails
                var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                var body = string.Empty;


                #region inspectors

                License licence = db.Licenses.Where(l => l.LicenseId == InspectionRequest.LicenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition)


.FirstOrDefault();
                var depContacts = db.DepartmentContacts.Where(d => d.DepartmentContactId == InspectionRequest.DepartmentContactId && d.IsDeleted == false && d.IsActive == true)
                                                       .Include(u => u.User).FirstOrDefault();

                var departmentinfo = db.Departments.Where(d => d.DepartmentId == depContacts.DepartmentId).FirstOrDefault();
                var client = db.Clients.Where(w => w.ClientId == InspectionRequest.License.ClientId).FirstOrDefault();
                if (depContacts != null)
                {


                    if (template != null)
                    {
                        body = template.EmailBody;
                    }

                    if (depContacts != null)
                    {
                        //L.M.20150303a - Replace variables with actual email content
                        body = body.Replace("#NAME#", depContacts.User.FullName);
                        body = body.Replace("#BODYTEXT#", " <b>An application has been reassigned to you for Inspection. </b><br/><br/> Business Name: " +
                           licence.Business.ProposedTradeName + "<br/> " +
                            " License Type: " + licence.LicenseType.LicenseTypeName + "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/>");

                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = EmailAccountId,
                            ToList = depContacts.User.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "eThekwini Trade Licensing- " + departmentinfo.DepartmentName + " Inspection Request." + licence.LicenseNumber,
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = client.IdentityOrPassportNumber,
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                        core.SaveChanges();




                    }


                }
                #endregion inspectors mail
                #endregion email
                return RedirectToAction("DepartmentManagerIndex");
            }
            var inspectorList = db.DepartmentContacts.Include(l => l.User).Where(l => l.IsDeleted == false && l.IsActive == true && l.DepartmentId == InspectionRequest.DepartmentId).ToList();



            var license = db.Licenses.Where(l => l.LicenseId == InspectionRequest.LicenseId)
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
            Initialise();

            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                        .Select(d => d.DocumentTypeId)
                                        .FirstOrDefault();
            var UploadedDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == license.ClientId && d.referenceId == InspectionRequestId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive).ToList();
            var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
            ViewData["Userdata"] = UserId;
            ViewData["BusinessData"] = license.Business;
            ViewData["ClientData"] = license.Client;
            ViewData["licenceData"] = license;
            ViewData["InspectionRequest"] = InspectionRequest;
            ViewData["LicenseType"] = license.LicenseType;
            ViewData["ItemType"] = license.ItemType;
            ViewData["Region"] = license.Region;
            ViewData["UploadList"] = UploadedDocs;

            List<SelectListItem> inspectorlist = new List<SelectListItem>();
            foreach (var item in inspectorList)
            {
                inspectorlist.Add(new SelectListItem() { Value = item.DepartmentContactId.ToString(), Text = item.User.FullName });
            }

            ViewBag.inspector = new SelectList(inspectorlist, "Value", "Text");
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", managerReview.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.UserId);
            return View(managerReview);
        }

        // GET: DepartmentManager/Create
        public ActionResult DepartmentManagerCreate(int? Id, int? DepartmentId)
        {
            try
            {
                LicenseApplicationDetails licenseApplicationDetails = _managerReviewHelpers.GetDepartmentManagerCreateVM(Id, DepartmentId);
                return View(licenseApplicationDetails);
            }
            catch (Exception e)
            {
                return View(TempData[LicenseApplicationDetails.ErrorKey]);
            }
        }

        // POST: DepartmentManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmentManagerCreate(LicenseApplicationDetails licenseApplicationDetails)
        {
            int departmentContact = licenseApplicationDetails?.InspectionRequestDetails?.DepartmentContactId ?? 0;
            int inspectionRequestId = licenseApplicationDetails?.InspectionRequestDetails?.InspectionRequestId ?? 0;

            if (ModelState.IsValid && departmentContact != 0 && inspectionRequestId != 0)
            {
                _managerReviewHelpers.AssignInspector(inspectionRequestId, departmentContact);
                #region Construct emails
                var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                var body = string.Empty;
                #region inspectors
                var InspectionRequest = db.InspectionRequests.Where(l => l.IsDeleted == false && l.IsActive == true && l.InspectionRequestId == inspectionRequestId).Include(l => l.License).FirstOrDefault();


                License licence = db.Licenses.Where(l => l.LicenseId == InspectionRequest.LicenseId)
.Include(l => l.Client)
.Include(l => l.Business)
.Include(l => l.LicenseType)
.Include(l => l.Status)
.Include(l => l.Region)
.Include(l => l.ItemType)
.Include(l => l.ItemSubCategory)
.Include(l => l.ItemCondition)


.FirstOrDefault();
                var depContacts = db.DepartmentContacts.Where(d => d.DepartmentContactId == InspectionRequest.DepartmentContactId && d.IsDeleted == false && d.IsActive == true)
                                                       .Include(u => u.User).FirstOrDefault();

                var departmentinfo = db.Departments.Where(d => d.DepartmentId == depContacts.DepartmentId).FirstOrDefault();
                var client = db.Clients.Where(w => w.ClientId == InspectionRequest.License.ClientId).FirstOrDefault();
                string actionLink = Url.Action("Create", "InspectionResponse", new { id = InspectionRequest.InspectionRequestId }, Request.Url.Scheme);
                if (depContacts != null)
                {
                    if (depContacts != null)
                    {
                        if (template != null)
                        {
                            body = template.EmailBody;
                        }
                        //L.M.20150303a - Replace variables with actual email content
                        body = body.Replace("#NAME#", depContacts.User.FullName);
                        body = body.Replace("#BODYTEXT#", " <b>An application has been assigned to you for Inspection. </b><br/><br/> Business Name: " +
                           licence.Business.ProposedTradeName + "<br/> " +
                            " License Type: " + licence.LicenseType.LicenseTypeName + "<br/> <a href=" + actionLink + ">Click here</a>" + " to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/>");

                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = EmailAccountId,
                            ToList = depContacts.User.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "eThekwini Trade Licensing- " + departmentinfo.DepartmentName + " Inspection Request." + licence.LicenseNumber,
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = client.IdentityOrPassportNumber,
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                        core.SaveChanges();




                    }


                }
                #endregion inspectors mail
                #endregion email
                TempData["Success"] = "License assigned to inspector";
                return RedirectToAction("DepartmentManagerIndex");
            }
            int departmentId = db.InspectionRequests.FirstOrDefault(l => l.IsDeleted == false && l.IsActive == true && l.InspectionRequestId == inspectionRequestId)
                ?.DepartmentId ?? 0;
            licenseApplicationDetails = _managerReviewHelpers.GetDepartmentManagerCreateVM(inspectionRequestId, departmentId);
            return View(licenseApplicationDetails);
        }


        // GET: ManagerReviews/Create
        [Authorize]
        public ActionResult Create(int? Id)
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
                //If not pending Manager Review, redirect to details.
                Status pendingManagerReview = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.AwaitingManagerReview);
                if (pendingManagerReview != null && license.StatusId != pendingManagerReview.StatusId)
                    return RedirectToAction("ApplicationTrackerDetails", "License", new { id = Id });

                User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.RegionDetails = license.Region;
                licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
                licenseApplicationDetails.ItemTypeDetails = license.ItemType;
                licenseApplicationDetails.ChiefReviews = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.ManagerReviews = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionResponses = db.InspectionResponse.Where(l => l.LicenseId == Id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                licenseApplicationDetails.InspectionRequests = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
                licenseApplicationDetails.AdministratorReviews = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.InspectionAppealsReviewsDetails = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();
                licenseApplicationDetails.InspectionAppealsReviews = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                licenseApplicationDetails.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();

                int? Inspectordocument = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                              .Select(d => d.DocumentTypeId)
                                              .FirstOrDefault();

                List<FileUpload> InspectorDocs = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId &&
                                    d.Document.DocumentTypeId == Inspectordocument &&
                                    d.IsDeleted == false &&
                                    d.IsActive).ToList();

                int? InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                    .Select(d => d.DocumentTypeId)
                                    .FirstOrDefault();

                licenseApplicationDetails.InspectionRequestUploadList = db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId &&
                                    d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId &&
                                    d.IsDeleted == false &&
                                    d.IsActive).ToList();

                licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;
                licenseApplicationDetails.InspectionResponseUploadList = InspectorDocs;
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

        // POST: ManagerReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails, string Userid)
        {

            Initialise();
            if (licenseApplicationDetails == null || licenseApplicationDetails.ManagerReviewDetails == null)
            {
                return HttpNotFound("Invalid LicenseApplicationDetails or ManagerReviewDetails.");
            }
            ManagerReview managerReview = licenseApplicationDetails?.ManagerReviewDetails ?? null;
            int licenseId = licenseApplicationDetails?.LicenseDetails?.LicenseId ?? 0;
            User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
            #region Send Email 
            var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
            var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
            var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
            var body = String.Empty;

            #endregion Send Email

            License license = new License();
            license = db.Licenses.Where(l => l.LicenseId == licenseId)
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
                return HttpNotFound("License not found.");
            }


            if (ModelState.IsValid)
            {


                managerReview.UserId = Int32.Parse(Userid);
                managerReview.LicenseId = licenseId;
                managerReview.IsActive = true;
                managerReview.IsDeleted = false;
                managerReview.IsLocked = false;

                db.ManagerRevies.Add(managerReview);
                db.SaveChanges();

                try
                {
                    if (managerReview.Decision == TLKeys.Reject)
                    {
                        license.StatusId =
    db.Status.Where(s => s.StatusKey == StatusKeys.ManagerRejected).Select(s => s.StatusId).
    FirstOrDefault();

                    }
                    else
                    {


                        DateTime dateToAddDays = DateTime.Now;
                        DateTime DueDate = dateToAddDays.Date.AddYears(1);
                        license.LicenseExpiryDate = DueDate;

                        license.LicenseIssueDateTime = DateTime.Now;


                        license.StatusId =
        db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId).
        FirstOrDefault();
                    }
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch { }





                try
                {

                    #region Send Email To Clerk

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
                        body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Manager for: </b><br/><br/> Business Name: " +
                           license.Business.ProposedTradeName + "<br/> " +
                             "Managers Decision: " +
                           managerReview.Decision + "<br/> " +
                             "Managers Comment: " +
                           managerReview.Comment + "<br/> " +
                            "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = EmailAccountId,
                            ToList = user.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "eThekwini Trade Licensing - Application: " + managerReview.Decision + "License Number " + license.LicenseNumber,
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = user.UserId.ToString(),
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                    }

                    core.SaveChanges();
                    #endregion Send Email To Clerk
                    var inspectionResponse = db.InspectionResponse.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).Include(l => l.InspectionRequest).ToList();
                    #region Send Email To DepartmentManagers
                    foreach (var response in inspectionResponse)
                    {
                        body = String.Empty;
                        var DepartmentManagers = db.DepartmentContacts.Where(d => d.DepartmentId == response.InspectionRequest.DepartmentId && d.IsDeleted == false && d.IsActive && d.RoleName == "Department Manager")
                                                              .Include(u => u.User).ToList();

                        if (DepartmentManagers.Count > 0)
                        {

                            //LF2014a 
                            //User email helper to get well formatted email.
                            //var mail = new EmailHelper();


                            foreach (var admin in DepartmentManagers)
                            {
                                if (template != null)
                                {
                                    body = template.EmailBody;
                                }
                                //L.M.20150303a - Replace variables with actual email content
                                var user = admin.User;
                                body = body.Replace("#NAME#", user.FullName);
                                body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Manager for: </b><br/><br/> Business Name: " +
                                   license.Business.ProposedTradeName + "<br/> " +
                                     "Managers Decision: " +
                                   managerReview.Decision + "<br/> " +
                                     "Managers Comment: " +
                                   managerReview.Comment + "<br/> " +
                                    "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                                var email = new tb_EmailQueue
                                {
                                    QueueDateTime = DateTime.Now,
                                    ApplicationId = applicationId,
                                    EmailAccountId = EmailAccountId,
                                    ToList = user.EmailAddress,
                                    CcList = null,
                                    BccList = null,
                                    Subject = "eThekwini Trade Licensing - Application: " + managerReview.Decision + "License Number " + license.LicenseNumber,
                                    Body = body,
                                    IsHtml = true,
                                    FailureCount = 0,
                                    ReferenceId = user.UserId.ToString(),
                                    HasAttachments = false
                                };

                                core.tb_EmailQueue.Add(email);
                            }

                            core.SaveChanges();
                        }
                    }
                    #endregion Send Email To DepartmentManagers
                    #region Send Email To DepartmentInspector
                    foreach (var response in inspectionResponse)
                    {
                        body = String.Empty;
                        var Inspector = db.DepartmentContacts.Where(d => d.DepartmentContactId == response.InspectionRequest.DepartmentContactId && d.IsDeleted == false)
                                                              .Include(u => u.User).ToList();

                        if (Inspector.Count > 0)
                        {

                            //LF2014a 
                            //User email helper to get well formatted email.
                            //var mail = new EmailHelper();


                            foreach (var admin in Inspector)
                            {
                                if (template != null)
                                {
                                    body = template.EmailBody;
                                }
                                //L.M.20150303a - Replace variables with actual email content
                                var user = admin.User;
                                body = body.Replace("#NAME#", user.FullName);
                                body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Manager for: </b><br/><br/> Business Name: " +
                                   license.Business.ProposedTradeName + "<br/> " +
                                     "Managers Decision: " +
                                   managerReview.Decision + "<br/> " +
                                     "Managers Comment: " +
                                   managerReview.Comment + "<br/> " +
                                    "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                                var email = new tb_EmailQueue
                                {
                                    QueueDateTime = DateTime.Now,
                                    ApplicationId = applicationId,
                                    EmailAccountId = EmailAccountId,
                                    ToList = user.EmailAddress,
                                    CcList = null,
                                    BccList = null,
                                    Subject = "eThekwini Trade Licensing - Application: " + managerReview.Decision + "License Number " + license.LicenseNumber,
                                    Body = body,
                                    IsHtml = true,
                                    FailureCount = 0,
                                    ReferenceId = user.UserId.ToString(),
                                    HasAttachments = false
                                };

                                core.tb_EmailQueue.Add(email);
                            }

                            core.SaveChanges();
                        }
                    }
                    #endregion Send Email To DepartmentInspector

                    #region Send Email To Licence Administrators
                    //L.M.20150303a - Replace variables with actual email content
                    var users = db.Users.Where(u => u.Role == "Licensing Administrator" && u.IsActive && u.IsDeleted == false);

                    foreach (var admin in users)
                    {
                        body = template.EmailBody;
                        body = body.Replace("#NAME#", admin.FullName);
                        body = body.Replace("#BODYTEXT#", " <b>Application has been actioned by Licensing Manager for: </b><br/><br/> Business Name: " +
                           license.Business.ProposedTradeName + "<br/> " +
                             "Managers Decision: " +
                           managerReview.Decision + "<br/> " +
                             "Managers Comment: " +
                           managerReview.Comment + "<br/> " +
                            "<br/> Log onto the Trade Licensing application to view further details.<br/> Please do not respond to this email, as it is a system generated email.<br/><br/>");
                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = EmailAccountId,
                            ToList = admin.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "eThekwini Trade Licensing - Application: " + managerReview.Decision + "License Number " + license.LicenseNumber,
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = admin.UserId.ToString(),
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                        core.SaveChanges();
                    }
                    #endregion
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

                //var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                ViewData["Userdata"] = UserId;
                ViewData["BusinessData"] = license.Business;
                ViewData["ClientData"] = license.Client;
                ViewData["licenceData"] = license;
                ViewData["AppealsReviews"] = db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["adminpreviousReview"] = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["ChiefpreviousReview"] = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["ManagerpreviousReview"] = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
                ViewData["InspectionResponse"] = db.InspectionResponse.Where(l => l.LicenseId == managerReview.LicenseId).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
                ViewData["DepartmentRequest"] = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
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


                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.InspectionRequests = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                return View(managerReview);
            }
        }

        // GET: ManagerReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManagerReview managerReview = db.ManagerRevies.Find(id);
            if (managerReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", managerReview.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.UserId);
            return View(managerReview);
        }

        // POST: ManagerReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ManagerReviewId,LicenseId,UserId,Decision,Comment,DateReviewed,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] ManagerReview managerReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(managerReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.CreatedByUserId);
            ViewBag.LicenseId = new SelectList(db.Licenses, "LicenseId", "LicenseNumber", managerReview.LicenseId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.ModifiedByUserId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", managerReview.UserId);
            return View(managerReview);
        }

        // GET: ManagerReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManagerReview managerReview = db.ManagerRevies.Find(id);
            if (managerReview == null)
            {
                return HttpNotFound();
            }
            return View(managerReview);
        }

        // POST: ManagerReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ManagerReview managerReview = db.ManagerRevies.Find(id);
            db.ManagerRevies.Remove(managerReview);
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
