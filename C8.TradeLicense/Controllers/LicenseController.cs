using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using System.Web.Mvc;
using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using C8.TradeLicense.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Web.Script.Serialization;
using C8.TradeLicense.DAL;
using C8.TradeLicense.Keys;
using Microsoft.Ajax.Utilities;
using System.Configuration;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using C8.TradeLicense.Helpers;
using System.Data.Entity.Migrations;
using C8.TradeLicense.Dtos;
using C8.TradeLicense.Models.enums;

namespace C8.TradeLicense.Controllers
{
    public class LicenseController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private LicenseHelper _licenseHelper;
        private FileHelpers _fileHelpers;
        private CesarDbContext core = new CesarDbContext();
        private const int PendingInspectionSlaNumDays = 90; // Entire license application process SLA
        public int EmailAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["EmailAccountId"]);
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

        public LicenseController()
        {
            // JK.20140906a - Instantiate the IdentityManager in the contructor and pass the DbContext.
            IdentityManager = new IdentityManager(db);
            _licenseHelper = new LicenseHelper(db);
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

            try
            {
                IdentityManager = new IdentityManager(db);

                if (User != null && User.Identity.IsAuthenticated)
                {
                    IdentityManager.CurrentUser(User);
                    Users = IdentityManager.CurrentUser(User);
                }

                if (Users != null)
                {
                    Users =
                        db.Users.Where(o => o.UserId == Users.UserId)
                            .FirstOrDefault();



                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #region Pending License Display Methods
        // GET: /License/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
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

            int pendingInspection = db.Status.Where(s => s.StatusKey == StatusKeys.LicensePending).Select(s => s.StatusId)
                    .FirstOrDefault();


            var licenses = db.Licenses.Include(l => l.Business)
                     .Include(l => l.Client)
                     .Include(l => l.Region)
                     .Include(l => l.CreatedByUser)
                     .Include(l => l.LicenseType)
                     .Include(l => l.ModifiedByUser)
                     .Include(l => l.Status)
                     .Include(l => l.ItemType)
                     .Include(l => l.ItemSubCategory)
                     .Where(l => l.IsDeleted == false && l.IsActive == true)
                     .Where(l => l.StatusId == pendingInspection && l.RegionId == Users.Region)
                     .OrderBy(l => l.Status.StatusName).ToList();
            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
            {
                licenses = db.Licenses.Include(l => l.Business)
                  .Include(l => l.Client)
                  .Include(l => l.Region)
                  .Include(l => l.CreatedByUser)
                  .Include(l => l.LicenseType)
                  .Include(l => l.ModifiedByUser)
                  .Include(l => l.Status)
                  .Include(l => l.ItemType)
                  .Include(l => l.ItemSubCategory)
                  .Where(l => l.IsDeleted == false && l.IsActive == true)
                  .Where(l => l.StatusId == pendingInspection)
                  .OrderBy(l => l.Status.StatusName).ToList();
            }

            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number

                        licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();


                        //licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                }
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == pendingInspection).OrderBy(c => c.StatusName), "StatusId", "StatusName");

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

            ViewBag.LicenseCount = licenses.Count;
            ViewBag.Licenses = licenses;
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }


        [Authorize(Roles = "Licensing Administrator" + "," + "Department Manager" + "," + "Department Clerk" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult Refusal(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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


            int pendingInspection = db.Status.Where(s => s.StatusKey == StatusKeys.RefusalInProgress).Select(s => s.StatusId)
                    .FirstOrDefault();
            int allregion = db.Regions.Where(s => s.RegionKey == TLKeys.metro_all).Select(s => s.RegionId)
              .FirstOrDefault();
            int metro_NorthWest = db.Regions.Where(s => s.RegionKey == TLKeys.metro_NorthWest).Select(s => s.RegionId)
                .FirstOrDefault();
            int metro_CentralSouth = db.Regions.Where(s => s.RegionKey == TLKeys.metro_CentralSouth).Select(s => s.RegionId)
                .FirstOrDefault();

            var licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                .Include(l => l.Region)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(l => l.IsDeleted == false && l.IsActive == true)
                .Where(l => l.StatusId == pendingInspection && l.RegionId == Users.Region)
                .OrderBy(l => l.Status.StatusName).ToList();
            if (Users.Region == allregion)
            {

                licenses = db.Licenses.Include(l => l.Business)
                   .Include(l => l.Client)
                   //.Include(l => l.Client.ClientUser)
                   .Include(l => l.Region)
                   .Include(l => l.CreatedByUser)
                   .Include(l => l.LicenseType)
                   .Include(l => l.ModifiedByUser)
                   .Include(l => l.Status)
                   .Include(l => l.ItemType)
                   .Include(l => l.ItemSubCategory)
                   .Where(l => l.IsDeleted == false && l.IsActive == true)
                   .Where(l => l.StatusId == pendingInspection)
                   .OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_CentralSouth)

            {
                int metro_south = db.Regions.Where(s => s.RegionKey == TLKeys.metro_south).Select(s => s.RegionId)
       .FirstOrDefault();
                int metro_central = db.Regions.Where(s => s.RegionKey == TLKeys.metro_central).Select(s => s.RegionId)
    .FirstOrDefault();

                licenses = db.Licenses.Include(l => l.Business)
               .Include(l => l.Client)
               //.Include(l => l.Client.ClientUser)
               .Include(l => l.Region)
               .Include(l => l.CreatedByUser)
               .Include(l => l.LicenseType)
               .Include(l => l.ModifiedByUser)
               .Include(l => l.Status)
               .Include(l => l.ItemType)
               .Include(l => l.ItemSubCategory)
               .Where(l => l.IsDeleted == false && l.IsActive == true)
               .Where(l => l.StatusId == pendingInspection && (l.RegionId == metro_south || l.RegionId == metro_central))
               .OrderBy(l => l.Status.StatusName).ToList();
            }
            else if (Users.Region == metro_NorthWest)

            {
                int metro_north = db.Regions.Where(s => s.RegionKey == TLKeys.metro_north).Select(s => s.RegionId)
.FirstOrDefault();
                int meto_west = db.Regions.Where(s => s.RegionKey == TLKeys.meto_west).Select(s => s.RegionId)
    .FirstOrDefault();

                licenses = db.Licenses.Include(l => l.Business)
               .Include(l => l.Client)
               //.Include(l => l.Client.ClientUser)
               .Include(l => l.Region)
               .Include(l => l.CreatedByUser)
               .Include(l => l.LicenseType)
               .Include(l => l.ModifiedByUser)
               .Include(l => l.Status)
               .Include(l => l.ItemType)
               .Include(l => l.ItemSubCategory)
               .Where(l => l.IsDeleted == false && l.IsActive == true)
               .Where(l => l.StatusId == pendingInspection && (l.RegionId == meto_west || l.RegionId == metro_north))
               .OrderBy(l => l.Status.StatusName).ToList();
            }

            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number

                        licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();



                        //licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                }
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == pendingInspection).OrderBy(c => c.StatusName), "StatusId", "StatusName");


            int pageSize = 5;
            int pageNumber = (page ?? 1);

            ViewBag.LicenseCount = licenses.Count();
            ViewBag.Licenses = licenses;
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }
        #region IndexPost
        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Index(int? Page_No, string StatusId, string SearchCriteria, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            Initialise();
            ViewBag.CurrentSortOrder = Sorting_Order;
            //ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            //ViewBag.SortingDate = Sorting_Order == "Date_Enroll" ? "Date_Description" : "Date";
            if (inputSearch != null)
            {
                Page_No = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.FilterValue = inputSearch;
            //ViewBag.currentFilter = inputSearch;
            //Gets all the licenses that are awaiting documentation
            //TG2014a.   
            int pendingDocumention = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDocumentationPending).Select(s => s.StatusId)
                     .FirstOrDefault();
            int pendingInspection = db.Status.Where(s => s.StatusKey == StatusKeys.PendingLicenseInspection).Select(s => s.StatusId)
                    .FirstOrDefault();

            var licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                .Include(l => l.Region)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(l => l.IsDeleted == false && l.IsActive == true)
                .Where(l => l.StatusId == pendingInspection && l.RegionId == Users.Region)
                .OrderBy(l => l.Status.StatusName).ToList();
            if (User.IsInRole("System Admin"))
            {
                licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                .Include(l => l.Region)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(l => l.IsDeleted == false && l.IsActive == true)
                .Where(l => l.StatusId == pendingInspection)
                .OrderBy(l => l.Status.StatusName).ToList();
            }

            #region Search Filters
            if (!String.IsNullOrEmpty(StatusId))
            {
                var searchStatus = db.Status.Find(Convert.ToInt32(StatusId));
                int status = db.Status.Where(s => s.StatusId == searchStatus.StatusId).Select(s => s.StatusId)
                  .FirstOrDefault();
                licenses = licenses.Where(l => l.StatusId == status)
               .OrderBy(l => l.Status.StatusName).ToList();
                #region oldCode
                // licenses = db.Licenses.Include(l => l.Business)
                // .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                //.Include(l => l.Region)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.LicenseType)
                //.Include(l => l.ModifiedByUser)
                //.Include(l => l.Status)
                //.Include(l => l.ItemType)
                //.Include(l => l.ItemSubCategory)
                //.Where(l => l.IsDeleted == false && l.IsActive == true)
                //.Where(l => l.StatusId == status)
                //.OrderBy(l => l.Status.StatusName).ToList();
                #endregion
            }

            //LM.20150209a 
            if (SearchCriteria == "BusinessName")
            {
                //Search by business name
                licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
            }
            else if (SearchCriteria == "ReferenceNo")
            {
                //Search by reference number
                licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();
            }
            else if (SearchCriteria == "ClientName")
            {
                //Search by client name
                licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
            }
            #endregion

            if (licenses.Count <= 0)
            {
                TempData["Error"] = "No records to display.";
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == pendingDocumention || s.StatusId == pendingInspection).OrderBy(c => c.StatusName), "StatusId", "StatusName");

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                Page_No = 1;
            }
            int pageSize = 5;
            int pageNumber = (Page_No ?? 1);
            ViewBag.LicenseCount = licenses.Count;
            //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }
        #endregion
        #endregion

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Abandoned(int? Page_No, string StatusId, string SearchCriteria, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            Initialise();
            ViewBag.CurrentSortOrder = Sorting_Order;
            //ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            //ViewBag.SortingDate = Sorting_Order == "Date_Enroll" ? "Date_Description" : "Date";
            if (inputSearch != null)
            {
                Page_No = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.FilterValue = inputSearch;
            //ViewBag.currentFilter = inputSearch;
            //Gets all the licenses that are awaiting documentation
            //TG2014a.   
            int pendingDocumention = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDocumentationPending).Select(s => s.StatusId)
                     .FirstOrDefault();
            int pendingInspection = db.Status.Where(s => s.StatusKey == StatusKeys.AppealAbandoned).Select(s => s.StatusId)
                    .FirstOrDefault();




            var licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                .Include(l => l.Region)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(l => l.IsDeleted == false && l.IsActive == true && l.RegionId == Users.Region)
                .Where(l => l.StatusId == pendingInspection)
                .OrderBy(l => l.Status.StatusName).ToList();
            if (User.IsInRole("System Admin"))
            {


                licenses = db.Licenses.Include(l => l.Business)
                   .Include(l => l.Client)
                   //.Include(l => l.Client.ClientUser)
                   .Include(l => l.Region)
                   .Include(l => l.CreatedByUser)
                   .Include(l => l.LicenseType)
                   .Include(l => l.ModifiedByUser)
                   .Include(l => l.Status)
                   .Include(l => l.ItemType)
                   .Include(l => l.ItemSubCategory)
                   .Where(l => l.IsDeleted == false && l.IsActive == true)
                   .Where(l => l.StatusId == pendingInspection)
                   .OrderBy(l => l.Status.StatusName).ToList();
            }



            #region Search Filters
            if (!String.IsNullOrEmpty(StatusId))
            {
                var searchStatus = db.Status.Find(Convert.ToInt32(StatusId));
                int status = db.Status.Where(s => s.StatusId == searchStatus.StatusId).Select(s => s.StatusId)
                  .FirstOrDefault();
                licenses = licenses.Where(l => l.StatusId == status)
               .OrderBy(l => l.Status.StatusName).ToList();
                #region oldCode
                // licenses = db.Licenses.Include(l => l.Business)
                // .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                //.Include(l => l.Region)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.LicenseType)
                //.Include(l => l.ModifiedByUser)
                //.Include(l => l.Status)
                //.Include(l => l.ItemType)
                //.Include(l => l.ItemSubCategory)
                //.Where(l => l.IsDeleted == false && l.IsActive == true)
                //.Where(l => l.StatusId == status)
                //.OrderBy(l => l.Status.StatusName).ToList();
                #endregion
            }

            //LM.20150209a 
            if (SearchCriteria == "BusinessName")
            {
                //Search by business name
                licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
            }
            else if (SearchCriteria == "ReferenceNo")
            {
                //Search by reference number
                licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();
            }
            else if (SearchCriteria == "ClientName")
            {
                //Search by client name
                licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
            }
            #endregion

            if (licenses.Count <= 0)
            {
                TempData["Error"] = "No records to display.";
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == pendingDocumention || s.StatusId == pendingInspection).OrderBy(c => c.StatusName), "StatusId", "StatusName");

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                Page_No = 1;
            }
            int pageSize = 5;
            int pageNumber = (Page_No ?? 1);
            ViewBag.LicenseCount = licenses.Count;
            //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult RejectedApplications(int? Page_No, string StatusId, string SearchCriteria, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            Initialise();
            ViewBag.CurrentSortOrder = Sorting_Order;
            //ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            //ViewBag.SortingDate = Sorting_Order == "Date_Enroll" ? "Date_Description" : "Date";
            if (inputSearch != null)
            {
                Page_No = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.FilterValue = inputSearch;
            //ViewBag.currentFilter = inputSearch;
            //Gets all the licenses that are awaiting documentation
            //TG2014a.   
            int ManagerRejected = db.Status.Where(s => s.StatusKey == StatusKeys.ManagerRejected).Select(s => s.StatusId)
                     .FirstOrDefault();
            int AppealRejected = db.Status.Where(s => s.StatusKey == StatusKeys.AppealRejected).Select(s => s.StatusId)
                    .FirstOrDefault();
            int ChiefRejected = db.Status.Where(s => s.StatusKey == StatusKeys.ChiefRejected).Select(s => s.StatusId)
                   .FirstOrDefault();
            int AdministratorRejected = db.Status.Where(s => s.StatusKey == StatusKeys.AdministratorRejected).Select(s => s.StatusId)
                  .FirstOrDefault();



            var licenses = db.InspectionRequests.Include(l => l.License.Business)
                .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                .Include(l => l.License.Region)
                .Include(l => l.CreatedByUser)
                .Include(l => l.License)
                 .Include(l => l.License.Status)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.Department)
                .Where(l => l.License.RegionId == Users.Region)
                .OrderBy(l => l.Status.StatusName).DistinctBy(x => x.LicenseId).ToList();
            if (User.IsInRole("System Admin"))
            {


                licenses = db.InspectionRequests.Include(l => l.License.Business)
                    .Include(l => l.Client)
                    //.Include(l => l.Client.ClientUser)
                    .Include(l => l.License.Region)
                    .Include(l => l.CreatedByUser)
                    .Include(l => l.License)
                     .Include(l => l.License.Status)
                    .Include(l => l.ModifiedByUser)
                    .Include(l => l.Status)
                    .Include(l => l.Department)
                    .OrderBy(l => l.Status.StatusName).DistinctBy(x => x.LicenseId).ToList();
            }



            licenses = licenses.Where(l => l.StatusId == AppealRejected || l.License.StatusId == AdministratorRejected || l.License.StatusId == ManagerRejected || l.License.StatusId == ChiefRejected).ToList();



            #region Search Filters
            if (!String.IsNullOrEmpty(StatusId))
            {
                var searchStatus = db.Status.Find(Convert.ToInt32(StatusId));
                int status = db.Status.Where(s => s.StatusId == searchStatus.StatusId).Select(s => s.StatusId)
                  .FirstOrDefault();
                licenses = licenses.Where(l => l.StatusId == status)
               .OrderBy(l => l.Status.StatusName).ToList();
                #region oldCode
                // licenses = db.Licenses.Include(l => l.Business)
                // .Include(l => l.Client)
                //.Include(l => l.Client.ClientUser)
                //.Include(l => l.Region)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.LicenseType)
                //.Include(l => l.ModifiedByUser)
                //.Include(l => l.Status)
                //.Include(l => l.ItemType)
                //.Include(l => l.ItemSubCategory)
                //.Where(l => l.IsDeleted == false && l.IsActive == true)
                //.Where(l => l.StatusId == status)
                //.OrderBy(l => l.Status.StatusName).ToList();
                #endregion
            }

            //LM.20150209a 
            if (SearchCriteria == "BusinessName")
            {
                //Search by business name
                licenses = licenses.Where(l => l.License.Business.ProposedTradeName.Contains(inputSearch)).ToList();
            }
            else if (SearchCriteria == "ReferenceNo")
            {
                //Search by reference number
                licenses = licenses.Where(l => l.License.LicenseNumber == inputSearch).ToList();
            }
            else if (SearchCriteria == "ClientName")
            {
                //Search by client name
                licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
            }
            #endregion

            if (licenses.Count <= 0)
            {
                TempData["Error"] = "No records to display.";
            }

            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusId == AdministratorRejected || s.StatusId == ManagerRejected || s.StatusId == AppealRejected || s.StatusId == ChiefRejected).OrderBy(c => c.StatusName), "StatusId", "StatusName");

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                Page_No = 1;
            }
            int pageSize = 5;
            int pageNumber = (Page_No ?? 1);
            ViewBag.LicenseCount = licenses.Count;
            //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }

        #region  License Status Change Methods

        /// <summary>
        ///  License renewals
        /// </summary>

        //public ActionResult PendingrenewStatus(int id)
        //{

        //    var license = db.Licenses.Find(id);
        //    license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.PendingRenewal).Select(s => s.StatusId).FirstOrDefault();
        //    db.Entry(license).State = EntityState.Modified;
        //    db.SaveChanges();
        //    TempData["Success"] = "License is Pending Renewal";
        //    return RedirectToAction("ApprovedLicenses");


        //}
        //public void checkexpiryDate(List<License> license)
        //{
        //    DateTime currentDate = DateTime.Now.Date;
        //    foreach (var items in license)
        //    {
        //        if (currentDate > items.LicenseExpiryDate)
        //        {
        //            items.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.NotRenewed).Select(s => s.StatusId).FirstOrDefault();
        //            db.Entry(items).State = EntityState.Modified;
        //            db.SaveChanges();

        //        }

        //    }

        //}
        public ActionResult renewedStatus(string id)
        {
            Initialise();
            try
            {
                if (id != null)
                {
                    int licenceID = Convert.ToInt32(id);


                    var license = db.Licenses.Where(x => x.LicenseId == licenceID).Include(x => x.Status).FirstOrDefault();


                    DateTime dateToAddDays = DateTime.Now;
                    DateTime DueDate = dateToAddDays.Date.AddYears(1);

                    RenewalHistory renewalHistory = new RenewalHistory();
                    try
                    {
                        renewalHistory.LicenseId = license.LicenseId;
                        renewalHistory.OldRenewalDateTime = license.LicenseExpiryDate;
                        renewalHistory.NewRenewalDateTime = DueDate;
                        renewalHistory.IsActive = true;
                        db.RenewalHistory.Add(renewalHistory);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                        return Json("Error");
                    }
                    license.LicenseExpiryDate = DueDate;
                    license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId).FirstOrDefault();
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();


                    return Json("Success");

                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;

            }

            return Json("Error");
        }
        #endregion
        #region Approved License Methods
        //Returns all the Approved Licenses
        //Moved from License Approval Controller
        //TG20141029a.
        [Authorize(Roles = "Licensing Administrator" + "," + "Department Manager" + "," + "Department Clerk" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult ApprovedLicenses(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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
            IdentityManager identifyManager = new IdentityManager();
            string currentUserId = User.Identity.GetUserId();
            var user = identifyManager.CurrentUser(currentUserId);
            var licenses = db.Licenses.Include(l => l.Business)
     .Include(l => l.Region)
     .Include(l => l.Client)
     .Include(l => l.CreatedByUser)
     .Include(l => l.LicenseType)
     .Include(l => l.ModifiedByUser)
     .Include(l => l.Status)
     .Include(l => l.ItemType)
     .Include(l => l.ItemSubCategory)
     .Where(c => c.IsDeleted == false && c.IsActive && c.RegionId == Users.Region).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId)
         .FirstOrDefault()).ToList();
            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))

            {
                licenses = db.Licenses.Include(l => l.Business)
          .Include(l => l.Region)
          .Include(l => l.Client)
          .Include(l => l.CreatedByUser)
          .Include(l => l.LicenseType)
          .Include(l => l.ModifiedByUser)
          .Include(l => l.Status)
          .Include(l => l.ItemType)
          .Include(l => l.ItemSubCategory)
          .Where(c => c.IsDeleted == false && c.IsActive).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId)
              .FirstOrDefault()).ToList();
            }



            //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
            ViewBag.CanAction = CanAction();

            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "LicenseNo":
                        //Search by license number
                        licenses = licenses.Where(l => l.LicenseNumber == inputSearch)
                        .ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    default:
                        licenses = licenses.ToList();
                        break;
                }
            }
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //TempData["Error"] = null;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.ApprovedLicenseCount = licenses.Count;
            return View(licenses.ToPagedList(pageNumber, pageSize));
            //return View(licenses);

        }
        #region ApprovedPost
        //Returns all Searched Approved Licenses
        //Moved from License Approval Controller
        //TG20141029a.
        //[HttpPost]
        //[Authorize(Roles = "Administrator" + "," + "Clerks" + "," + "Licensing Manager")]
        //public ActionResult ApprovedLicenses(int? page, string SearchCriteria, string inputSearch)
        //{
        //    int approvedLicenses = db.Status.Where(s => s.StatusKey == "LicenseApproved").Select(s => s.StatusId)
        //            .FirstOrDefault();
        //    List<License> searchResults = null;

        //    var licenses = db.Licenses.Include(l => l.Business)
        //        .Include(l => l.Region)
        //        .Include(l => l.Client)
        //        .Include(l => l.CreatedByUser)
        //        .Include(l => l.LicenseType)
        //        .Include(l => l.ModifiedByUser)
        //        .Include(l => l.Status)
        //        .Include(l => l.ItemType)
        //        .Include(l => l.ItemSubCategory)
        //        .Where(c => c.IsDeleted == false && c.IsActive).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseApproved").Select(s => s.StatusId)
        //            .FirstOrDefault()).ToList();

        //    if (SearchCriteria == "BusinessName")
        //    {
        //        //Search by business name
        //        licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
        //        #region OldCode
        //        //    db.Licenses.Include(c => c.CreatedByUser)
        //        //    .Include(l => l.Business)
        //        //     .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //    .Where(l => l.Business.ProposedTradeName.Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //        //    .Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }
        //    else if (SearchCriteria == "ClientName")
        //    {
        //        //Search by client name
        //        licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
        //        #region oldCode
        //        //var results = db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //        // .Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
        //    }
        //    else if (SearchCriteria == "LicenseNumber")
        //    {
        //        licenses = licenses.Where(l => (l.LicenseNumber.Contains(inputSearch))).ToList();
        //        #region oldCode
        //        //var results = 
        //        //     db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.LicenseNumber.Contains(inputSearch)) && l.IsDeleted == false && l.IsActive)
        //        //.Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }

        //    else
        //    {
        //        searchResults = licenses;
        //    }

        //    if (searchResults.Count <= 0 && SearchCriteria != null)
        //    {
        //        TempData["Error"] = "License not found! Please try different search criteria.";
        //    }
        //    //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
        //    ViewBag.CanAction = CanAction();
        //    // LM.20141110a - Set parameters for paging
        //    if (Request.HttpMethod != "GET")
        //    {
        //        page = 1;
        //    }
        //    //TempData["Error"] = null;
        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);
        //    ViewBag.ApprovedLicenseCount = licenses.Count;
        //    return View(searchResults.ToPagedList(pageNumber, pageSize));
        //    // return View(licenses.ToPagedList(pageNumber, pageSize));
        //    //return View(licenses);

        //}
        #endregion
        #endregion

        #region Expired Licenses for renewal
        /// <summary>
        /// TG20150219a.
        /// Returns all expired licenses.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>


        //Returns all Searched Expired Licenses      
        //TG20150219a.
        //[HttpPost]
        //[Authorize(Roles = "Administrator" + "," + "Clerks" + "," + "Licensing Manager")]
        //public ActionResult LicenseRenewal(int? page, string SearchCriteria, string inputSearch)
        //{
        //    int expiredLicenses = db.Status.Where(s => s.StatusKey == "LicenseExpired").Select(s => s.StatusId)
        //            .FirstOrDefault();
        //    List<License> searchResults = null;

        //    var licenses = db.Licenses.Include(l => l.Business)
        //        .Include(l => l.Region)
        //        .Include(l => l.Client)
        //        .Include(l => l.CreatedByUser)
        //        .Include(l => l.LicenseType)
        //        .Include(l => l.ModifiedByUser)
        //        .Include(l => l.Status)
        //        .Include(l => l.ItemType)
        //        .Include(l => l.ItemSubCategory)
        //        .Where(c => c.IsDeleted == false && c.IsActive).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseExpired").Select(s => s.StatusId)
        //            .FirstOrDefault()).ToList();

        //    if (SearchCriteria == "BusinessName")
        //    {
        //        //Search by business name
        //        licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
        //        #region OldCode
        //        //    db.Licenses.Include(c => c.CreatedByUser)
        //        //    .Include(l => l.Business)
        //        //     .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //    .Where(l => l.Business.ProposedTradeName.Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //        //    .Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }
        //    else if (SearchCriteria == "ClientName")
        //    {
        //        //Search by client name
        //        licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
        //        #region oldCode
        //        //var results = db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //        // .Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
        //    }
        //    else if (SearchCriteria == "LicenseNumber")
        //    {
        //        licenses = licenses.Where(l => (l.LicenseNumber.Contains(inputSearch))).ToList();
        //        #region oldCode
        //        //var results = 
        //        //     db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.LicenseNumber.Contains(inputSearch)) && l.IsDeleted == false && l.IsActive)
        //        //.Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }
        //    else if (SearchCriteria == "ReferenceNumber")
        //    {
        //        licenses = licenses.Where(l => (l.LicenseId == Convert.ToInt32(inputSearch))).ToList();
        //        #region oldCode
        //        //var results = 
        //        //     db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.LicenseNumber.Contains(inputSearch)) && l.IsDeleted == false && l.IsActive)
        //        //.Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }

        //    else
        //    {
        //        searchResults = licenses;
        //    }

        //    if (searchResults.Count <= 0 && SearchCriteria != null)
        //    {
        //        TempData["Error"] = "License not found! Please try different search criteria.";
        //    }
        //    //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
        //    ViewBag.CanAction = CanAction();
        //    // LM.20141110a - Set parameters for paging
        //    if (Request.HttpMethod != "GET")
        //    {
        //        page = 1;
        //    }
        //    //TempData["Error"] = null;
        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);
        //    ViewBag.LicenseRenewal = licenses.Count;
        //    return View(searchResults.ToPagedList(pageNumber, pageSize));
        //    // return View(licenses.ToPagedList(pageNumber, pageSize));
        //    //return View(licenses);

        //}
        /// <summary>
        /// TG20140519a.
        /// Renews a license.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        #endregion

        #region Terminated Licenses

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager"
    + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult TerminatedLicenses(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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

            var licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Region)
                .Include(l => l.Client)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(c => c.IsDeleted == true && c.IsActive == false && c.RegionId == Users.Region).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApplicationTerminated).Select(s => s.StatusId)
                    .FirstOrDefault()).ToList();

            if (User.IsInRole("System Admin"))
            {
                licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Region)
                .Include(l => l.Client)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApplicationTerminated).Select(s => s.StatusId)
                    .FirstOrDefault()).ToList();

            }


            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number
                        int n;
                        bool isNumeric = int.TryParse(inputSearch, out n);
                        if (isNumeric)
                        {
                            licenses = licenses.Where(l => l.LicenseId == Convert.ToInt32(inputSearch)).ToList();
                        }
                        else
                        {
                            licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        }
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        //Search by Region
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    default:
                        licenses = licenses.ToList();
                        break;
                }
            }

            //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //TempData["Error"] = null;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.TerminatedLicenses = licenses.Count;
            return View(licenses.ToPagedList(pageNumber, pageSize));
            //return View(licenses);

        }
        #region Terminated Post
        //Returns all Searched Expired Licenses      
        //TG20150219a.
        //[HttpPost]
        //[Authorize(Roles = "Administrator" + "," + "Clerks" + "," + "Licensing Manager")]
        //public ActionResult TerminatedLicenses(int? page, string SearchCriteria, string inputSearch)
        //{
        //    //int terminatedLicenses = db.Status.Where(s => s.StatusKey == "LicenseApplicationTerminated").Select(s => s.StatusId)
        //    //        .FirstOrDefault();
        //    List<License> searchResults = null;

        //    var licenses = db.Licenses.Include(l => l.Business)
        //        .Include(l => l.Region)
        //        .Include(l => l.Client)
        //        .Include(l => l.CreatedByUser)
        //        .Include(l => l.LicenseType)
        //        .Include(l => l.ModifiedByUser)
        //        .Include(l => l.Status)
        //        .Include(l => l.ItemType)
        //        .Include(l => l.ItemSubCategory)
        //        .Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseApplicationTerminated").Select(s => s.StatusId)
        //            .FirstOrDefault()).ToList();

        //    if (SearchCriteria == "BusinessName")
        //    {
        //        //Search by business name
        //        licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
        //        #region OldCode
        //        //    db.Licenses.Include(c => c.CreatedByUser)
        //        //    .Include(l => l.Business)
        //        //     .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //    .Where(l => l.Business.ProposedTradeName.Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //        //    .Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }
        //    else if (SearchCriteria == "ClientName")
        //    {
        //        //Search by client name
        //        licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
        //        #region oldCode
        //        //var results = db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //        // .Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
        //    }
        //    else if (SearchCriteria == "ReferenceNo")
        //    {
        //        int refno = Convert.ToInt32(inputSearch);
        //        licenses = licenses.Where(l => (l.LicenseId == refno)).ToList();
        //        #region oldCode
        //        //var results = 
        //        //     db.Licenses
        //        //              .Include(l => l.Region)
        //        //.Include(l => l.Client)
        //        //.Include(l => l.CreatedByUser)
        //        //.Include(l => l.LicenseType)
        //        //.Include(l => l.ModifiedByUser)
        //        //.Include(l => l.Status)
        //        //.Include(l => l.ItemType)
        //        //.Include(l => l.ItemSubCategory)
        //        //.Where(l => (l.LicenseNumber.Contains(inputSearch)) && l.IsDeleted == false && l.IsActive)
        //        //.Where(l => l.StatusId == approvedLicenses).ToList();
        //        #endregion
        //        searchResults = licenses;
        //    }

        //    else
        //    {
        //        searchResults = licenses;
        //    }

        //    if (searchResults.Count <= 0 && SearchCriteria != null)
        //    {
        //        TempData["Error"] = "License not found! Please try different search criteria.";
        //    }
        //    //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
        //    ViewBag.CanAction = CanAction();
        //    // LM.20141110a - Set parameters for paging
        //    if (Request.HttpMethod != "GET")
        //    {
        //        page = 1;
        //    }
        //    //TempData["Error"] = null;
        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);
        //    ViewBag.LicenseRenewal = licenses.Count;
        //    return View(searchResults.ToPagedList(pageNumber, pageSize));
        //    // return View(licenses.ToPagedList(pageNumber, pageSize));
        //    //return View(licenses);

        //}
        #endregion
        #endregion

        #region License Collection Methods
        //Gets the licenses awaiting collection.
        //TG20141029a
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult NotRenewed(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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


            var licenses = db.Licenses.Include(l => l.Business)
              .Include(l => l.Client)
              .Include(l => l.CreatedByUser)
              .Include(l => l.LicenseType)
              .Include(l => l.Region)
              .Include(l => l.ModifiedByUser)
              .Include(l => l.Status)
              .Where(l => l.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.NotRenewed).Select(s => s.StatusId).FirstOrDefault())
              .Where(l => l.IsDeleted == false && l.IsActive == true && l.RegionId == Users.Region)
              .OrderBy(l => l.LicenseType.LicenseTypeName).ToList();
            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
            {
                licenses = db.Licenses.Include(l => l.Business)
              .Include(l => l.Client)
              .Include(l => l.CreatedByUser)
              .Include(l => l.LicenseType)
              .Include(l => l.Region)
              .Include(l => l.ModifiedByUser)
              .Include(l => l.Status)
              .Where(l => l.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.NotRenewed).Select(s => s.StatusId).FirstOrDefault())
              .Where(l => l.IsDeleted == false && l.IsActive == true)
              .OrderBy(l => l.LicenseType.LicenseTypeName).ToList();
            }


            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        //Search by client name
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number
                        int n;

                        licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();


                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "Region":
                        //Search by Region
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    default:
                        licenses = licenses.ToList();
                        break;
                }
            }

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.LicenseCollectionCount = licenses.ToList().Count;
            return View(licenses.ToPagedList(pageNumber, pageSize));
            //return View(licenses);

        }
        #region CollectionPost
        //Gets the licenses awaiting collection.
        //TG20141029a
        //[HttpPost]
        //[Authorize(Roles = "Administrator" + "," + "Clerks" + "," + "Licensing Manager")]
        //public ActionResult LicenseCollection(int? page, string SearchCriteria, string inputSearch)
        //{
        //    int approvedLicenses = db.Status.Where(s => s.StatusKey == "LicenseApprovedAwaitingCollection").Select(s => s.StatusId)
        //            .FirstOrDefault();
        //    List<License> searchResults = null;

        //    var licenses = db.Licenses.Include(l => l.Business)
        //        .Include(l => l.Region)
        //        .Include(l => l.Client)
        //        .Include(l => l.CreatedByUser)
        //        .Include(l => l.LicenseType)
        //        .Include(l => l.ModifiedByUser)
        //        .Include(l => l.Status)
        //        .Include(l => l.ItemType)
        //        .Include(l => l.ItemSubCategory)
        //        .Where(c => c.IsDeleted == false && c.IsActive).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseApprovedAwaitingCollection").Select(s => s.StatusId)
        //            .FirstOrDefault()).ToList();

        //    if (SearchCriteria == "BusinessName")
        //    {
        //        //Search by business name
        //        var results = db.Licenses.Include(c => c.CreatedByUser)
        //            .Include(l => l.Business)
        //             .Include(l => l.Region)
        //        .Include(l => l.Client)
        //        .Include(l => l.CreatedByUser)
        //        .Include(l => l.LicenseType)
        //        .Include(l => l.ModifiedByUser)
        //        .Include(l => l.Status)
        //        .Include(l => l.ItemType)
        //        .Include(l => l.ItemSubCategory)
        //            .Where(l => l.Business.ProposedTradeName.Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //            .Where(l => l.StatusId == approvedLicenses).ToList();
        //        searchResults = results;
        //    }
        //    else if (SearchCriteria == "ClientName")
        //    {
        //        //Search by client name
        //        var results = db.Licenses
        //                      .Include(l => l.Region)
        //        .Include(l => l.Client)
        //        .Include(l => l.CreatedByUser)
        //        .Include(l => l.LicenseType)
        //        .Include(l => l.ModifiedByUser)
        //        .Include(l => l.Status)
        //        .Include(l => l.ItemType)
        //        .Include(l => l.ItemSubCategory)
        //        .Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch) && l.IsDeleted == false && l.IsActive)
        //         .Where(l => l.StatusId == approvedLicenses).ToList();

        //        searchResults = results; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
        //    }
        //    else if (SearchCriteria == "ReferenceNo")
        //    {
        //        int refno = Convert.ToInt32(inputSearch);
        //        var results = db.Licenses
        //                     .Include(l => l.Region)
        //       .Include(l => l.Client)
        //       .Include(l => l.CreatedByUser)
        //       .Include(l => l.LicenseType)
        //       .Include(l => l.ModifiedByUser)
        //       .Include(l => l.Status)
        //       .Include(l => l.ItemType)
        //       .Include(l => l.ItemSubCategory)
        //       .Where(l => l.LicenseId == refno && l.IsDeleted == false && l.IsActive)
        //        .Where(l => l.StatusId == approvedLicenses).ToList();

        //        searchResults = results;
        //    }
        //    else
        //    {
        //        searchResults = licenses;
        //    }

        //    if (searchResults.Count <= 0)
        //    {
        //        TempData["Error"] = "License not found! Please try different search criteria.";
        //    }
        //    //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
        //    ViewBag.CanAction = CanAction();
        //    // LM.20141110a - Set parameters for paging
        //    if (Request.HttpMethod != "GET")
        //    {
        //        page = 1;
        //    }
        //    //TempData["Error"] = null;
        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);
        //    return View(searchResults.ToPagedList(pageNumber, pageSize));
        //    // return View(licenses.ToPagedList(pageNumber, pageSize));
        //    //return View(licenses);

        //}
        #endregion
        //TG20141125a
        //Once the license is picked up it is moved the 'Approved License' page
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult collectLicense(int id)// int id, 
        {
            string prefix = "";
            License license = db.Licenses.Find(id);
            try
            {

                #region RegionCode
                if (license.RegionId == db.Regions.Where(r => r.RegionKey == "metro_central").Select(r => r.RegionId).
                        FirstOrDefault())
                {
                    prefix = "BO";
                }
                else if (license.RegionId == db.Regions.Where(r => r.RegionKey == "meto_west").Select(r => r.RegionId).
                        FirstOrDefault())
                {
                    prefix = "BW";
                }
                else if (license.RegionId == db.Regions.Where(r => r.RegionKey == "metro_north").Select(r => r.RegionId).
                        FirstOrDefault())
                {
                    prefix = "DO";
                }
                else if (license.RegionId == db.Regions.Where(r => r.RegionKey == "metro_south").Select(r => r.RegionId).
                        FirstOrDefault())
                {
                    prefix = "BLS";
                }

                if (license.LicenseTypeId == db.LicenseTypes.Where(l => l.LicenseTypeKey == "accommodation").Select(l => l.LicenseTypeId).
                        FirstOrDefault())
                {
                    prefix = "AEO";
                }
                #endregion

                license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId).
                        FirstOrDefault();
                license.LicenseCollectedDateTime = DateTime.Now;
                license.LicenseIssueDateTime = DateTime.Now;
                license.LicenseIssueYear = DateTime.Now.Year;
                license.LicenseExpiryDate = DateTime.Now.AddYears(1);
                license.LicenseNumber = prefix + DateTime.Now.Year + license.BusinessId;
                license.IsActive = true;
                license.IsDeleted = false;
                license.IsLocked = false;
                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "License Issued, Available to print.";
            }
            catch (Exception x)
            {

                throw x;
            }
            return RedirectToAction("Details", new { id = license.LicenseId });//return RedirectToAction("ApprovedLicenses");
        }
        #endregion

        #region Revoke/Cancel/Decline/Delete License Methods
        //Gets the Declined/Rejected licenses.
        //TG20141029a
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult DeclinedLicenses(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            Initialise();
            int declinedLicenses = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDeclined).Select(s => s.StatusId)
                     .FirstOrDefault();
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

            var licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Region)
                .Include(l => l.Client)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(c => c.IsDeleted == true && c.IsActive == false && c.RegionId == Users.Region).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDeclined).Select(s => s.StatusId)
                    .FirstOrDefault()).ToList()
                ;

            if (User.IsInRole("System Admin"))
            {
                licenses = db.Licenses.Include(l => l.Business)
                     .Include(l => l.Region)
                     .Include(l => l.Client)
                     .Include(l => l.CreatedByUser)
                     .Include(l => l.LicenseType)
                     .Include(l => l.ModifiedByUser)
                     .Include(l => l.Status)
                     .Include(l => l.ItemType)
                     .Include(l => l.ItemSubCategory)
                     .Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDeclined).Select(s => s.StatusId)
                         .FirstOrDefault()).ToList()
                     ;
            }

            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        //search by client name
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).Where(l => l.StatusId == declinedLicenses).Where(l => l.IsDeleted == true && l.IsActive == false).ToList();
                        break;
                    case "ReferenceNo":
                        //Search by reference number
                        int n;
                        bool isNumeric = int.TryParse(inputSearch, out n);
                        if (isNumeric)
                        {
                            licenses = licenses.Where(l => l.LicenseId == Convert.ToInt32(inputSearch)).Where(l => l.StatusId == declinedLicenses).Where(l => l.IsDeleted == true && l.IsActive == false).ToList();
                        }
                        else
                        {
                            licenses = licenses.Where(l => l.LicenseId == 0).ToList();
                        }
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).Where(l => l.StatusId == declinedLicenses).Where(l => l.IsDeleted == true && l.IsActive == false).ToList();
                        break;
                    case "Region":
                        //Search by Region
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).Where(l => l.StatusId == declinedLicenses).Where(l => l.IsDeleted == true && l.IsActive == false).ToList();
                        break;
                    default:
                        licenses = licenses.ToList();
                        break;
                }
            }
            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //TempData["Error"] = null;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.DeclinedLicenseCount = licenses.Count;
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }

        //Gets all the revoked licenses
        //TG20141029a
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult RevokedLicenses(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            Initialise();
            int RevokedLicenses = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseRevoked).Select(s => s.StatusId)
                     .FirstOrDefault();
            int CancelLicenses = db.Status.Where(s => s.StatusKey == StatusKeys.LicenceApplicationCancelled).Select(s => s.StatusId)
                  .FirstOrDefault();
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

            var licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Region)
                .Include(l => l.Client)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.Status)
                .Where(c => c.RegionId == Users.Region && (c.StatusId == RevokedLicenses || c.StatusId == CancelLicenses)).ToList();
            ;
            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
            {
                licenses = db.Licenses.Include(l => l.Business)
             .Include(l => l.Region)
             .Include(l => l.Client)
             .Include(l => l.CreatedByUser)
             .Include(l => l.LicenseType)
             .Include(l => l.ModifiedByUser)
             .Include(l => l.Status)
             .Include(l => l.Status)
             .Where(c => c.StatusId == RevokedLicenses || c.StatusId == CancelLicenses).ToList();
            }
            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        //Search by client name
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).Where(l => l.StatusId == RevokedLicenses || l.StatusId == CancelLicenses).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).Where(l => l.StatusId == RevokedLicenses || l.StatusId == CancelLicenses).ToList();
                        break;
                    case "LicenseNumber":
                        licenses = licenses.Where(l => (l.LicenseNumber == inputSearch))
                        .Where(l => l.StatusId == RevokedLicenses || l.StatusId == CancelLicenses).ToList();
                        break;
                    case "Region":
                        //Search by Region
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).Where(l => l.StatusId == RevokedLicenses || l.StatusId == CancelLicenses).ToList();
                        break;
                    default:
                        licenses = licenses.ToList();
                        break;
                }
            }
            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //TempData["Error"] = null;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.RevokedLicense = licenses.Count;
            return View(licenses.ToPagedList(pageNumber, pageSize));

        }
        #region RevokePost
        //Gets all the searched revoked licenses
        //TG20141029a
        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult RevokedLicenses(int? page, string SearchCriteria, string inputSearch)
        {
            Initialise();
            int RevokedLicenses = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseRevoked).Select(s => s.StatusId)
                     .FirstOrDefault();
            int CancelLicenses = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseCanceled).Select(s => s.StatusId)
                    .FirstOrDefault();
            List<License> searchResults = null;

            var licenses = db.Licenses.Include(l => l.Business)
               .Include(l => l.Region)
               .Include(l => l.Client)
               .Include(l => l.CreatedByUser)
               .Include(l => l.LicenseType)
               .Include(l => l.ModifiedByUser)
               .Include(l => l.Status)
               .Include(l => l.ItemType)
               .Include(l => l.ItemSubCategory)
               .Where(c => c.IsDeleted == true && c.IsActive == false && c.RegionId == Users.Region).Where(c => c.StatusId == db.Status.Where(s => (s.StatusKey == StatusKeys.LicenseRevoked) || (s.StatusKey == StatusKeys.LicenseCanceled)).Select(s => s.StatusId)
                   .FirstOrDefault()).ToList();
            if (User.IsInRole("System Admin"))
            {
                licenses = db.Licenses.Include(l => l.Business)
               .Include(l => l.Region)
               .Include(l => l.Client)
               .Include(l => l.CreatedByUser)
               .Include(l => l.LicenseType)
               .Include(l => l.ModifiedByUser)
               .Include(l => l.Status)
               .Include(l => l.ItemType)
               .Include(l => l.ItemSubCategory)
               .Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => (s.StatusKey == StatusKeys.LicenseRevoked) || (s.StatusKey == StatusKeys.LicenseCanceled)).Select(s => s.StatusId)
                   .FirstOrDefault()).ToList();
            }


            if (SearchCriteria == "BusinessName")
            {
                //Search by business name
                licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch) && l.IsDeleted == true && l.IsActive == false)
                    .Where(l => (l.StatusId == RevokedLicenses) || (l.StatusId == CancelLicenses)).ToList();
                #region oldCode
                //var results = db.Licenses.Include(c => c.CreatedByUser)
                //    .Include(l => l.Business)
                //     .Include(l => l.Region)
                //.Include(l => l.Client)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.LicenseType)
                //.Include(l => l.ModifiedByUser)
                //.Include(l => l.Status)
                //.Include(l => l.ItemType)
                //.Include(l => l.ItemSubCategory)
                //    .Where(l => l.Business.ProposedTradeName.Contains(inputSearch) && l.IsDeleted == true && l.IsActive == false)
                //    .Where(l => l.StatusId == RevokedLicenses).ToList();
                #endregion
                searchResults = licenses;
            }
            else if (SearchCriteria == "ClientName")
            {
                //Search by client name
                #region oldCode
                //var results = db.Licenses
                //              .Include(l => l.Region)
                //.Include(l => l.Client)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.LicenseType)
                //.Include(l => l.ModifiedByUser)
                //.Include(l => l.Status)
                //.Include(l => l.ItemType)
                //.Include(l => l.ItemSubCategory)
                #endregion
                licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch) && l.IsDeleted == true && l.IsActive == false)
                   .Where(l => (l.StatusId == RevokedLicenses) || (l.StatusId == CancelLicenses)).ToList();

                searchResults = licenses; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
            }
            else if (SearchCriteria == "LicenseNumber")
            {
                #region oldCode
                // var results = db.Licenses
                //              .Include(l => l.Region)
                //.Include(l => l.Client)
                //.Include(l => l.CreatedByUser)
                //.Include(l => l.LicenseType)
                //.Include(l => l.ModifiedByUser)
                //.Include(l => l.Status)
                //.Include(l => l.ItemType)
                //.Include(l => l.ItemSubCategory)
                #endregion
                licenses = licenses.Where(l => l.LicenseNumber.Contains(inputSearch) && l.IsDeleted == true && l.IsActive == false)
               .Where(l => (l.StatusId == RevokedLicenses) || (l.StatusId == CancelLicenses)).ToList();
                searchResults = licenses;
            }
            else if (SearchCriteria == "ReferenceNumber")
            {
                licenses = licenses.Where(l => (l.LicenseId == Convert.ToInt32(inputSearch)) && l.IsDeleted == true && l.IsActive == false)
               .Where(l => (l.StatusId == RevokedLicenses) || (l.StatusId == CancelLicenses)).ToList();
                searchResults = licenses;
            }
            else if (SearchCriteria == "Region")
            {
                //Search by Region
                licenses = licenses.Where(l => l.Region.RegionName.Contains(inputSearch) && l.IsDeleted == true && l.IsActive == false)
                        .Where(l => (l.StatusId == RevokedLicenses) || (l.StatusId == CancelLicenses)).ToList();
                searchResults = licenses; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
            }


            else
            {
                searchResults = licenses;
            }

            if (searchResults.Count <= 0)
            {
                TempData["Error"] = "License not found! Please try different search criteria.";
            }
            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //TempData["Error"] = null;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(searchResults.ToPagedList(pageNumber, pageSize));

        }
        #endregion
        //TG20141125a
        //Revokes the approved license.
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult RevokeLicence(int id)
        {

            Initialise();
            LicenseApplicationDetails licenseApplicationDetails = _licenseHelper.GetRevokeLicenseVM(id, UserId, null);
            if (licenseApplicationDetails != null)
            {
                return View(licenseApplicationDetails);
            }
            else
            {
                TempData["Error"] = "License invalid!";
                return RedirectToAction("Refusal");
            }
        }
        //Revokes the approved license.
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult revokeLicence(LicenseApplicationDetails licenseApplicationDetails)
        {
            Initialise();

            try
            {
                int licenseId = licenseApplicationDetails.LicenseDetails?.LicenseId ?? 0;
                LicensesRevoked licensesRevoked = licenseApplicationDetails.LicensesRevokedDetails;
                licensesRevoked.LicenseId = licenseId;
                License license = db.Licenses.Where(l => l.LicenseId == licenseId).Include(l => l.Business).FirstOrDefault();
                if (ModelState.IsValid && licenseId != 0)
                {
                    licensesRevoked.PreviousStatusId = license.StatusId;

                    db.LicensesRevoked.Add(licensesRevoked);
                    db.SaveChanges();


                    license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.RevokeCancelAwaitingManagerReview).Select(s => s.StatusId).
                            FirstOrDefault();
                    //license.LicenseClosureDateTime = DateTime.Now;
                    //license.IsDeleted = true;
                    //license.IsActive = false;
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();
                    #region Send Email To Manager
                    try
                    {
                        var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                        var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                        var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                        var body = String.Empty;
                        var admins = db.Users.Where(c => c.Role == "Licensing Manager").ToList();

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
                            body = body.Replace("#BODYTEXT#", " <b>Application Revoked,Cancelled or Not Renewed Awaiting Review. </b><br/><br/> Business Name: " +
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
                                Subject = "eThekwini Trade Licensing - Application for Revoked,Cancelled or Not Renewed:" + license.LicenseNumber,
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
                    licenseApplicationDetails = _licenseHelper.GetRevokeLicenseVM(licenseId, UserId, licensesRevoked);
                    return View(licenseApplicationDetails);
                }
            }
            catch (Exception x)
            {

                throw x;


            }
            return RedirectToAction("ApprovedLicenses");
        }

        // GET: /License/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Deleted(int? page, string StatusId, string SearchCriteria, string inputSearch, string CurrentFilter, string Sorting_Order)
        {
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
            Initialise();

            if (inputSearch != null)
            {
                page = 1;
            }
            else
            {
                inputSearch = CurrentFilter;
            }

            licenseApplicationDetails.CurrentFilter = inputSearch;

            //Gets all the licenses that are awaiting documentation
            //TG2014a.   
            int pendingDocumention = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDocumentationPending).Select(s => s.StatusId)
                     .FirstOrDefault();

            List<License> licenses = db.Licenses.Include(l => l.Business)
                .Include(l => l.Client)
                .Include(l => l.Region)
                .Include(l => l.CreatedByUser)
                .Include(l => l.LicenseType)
                .Include(l => l.ModifiedByUser)
                .Include(l => l.Status)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Where(l => l.IsDeleted == true && l.IsActive == false && l.RegionId == Users.Region)
                .OrderBy(l => l.Status.StatusName).ToList();

            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
            {
                licenses = db.Licenses.Include(l => l.Business)
                   .Include(l => l.Client)
                   .Include(l => l.Region)
                   .Include(l => l.CreatedByUser)
                   .Include(l => l.LicenseType)
                   .Include(l => l.ModifiedByUser)
                   .Include(l => l.Status)
                   .Include(l => l.ItemType)
                   .Include(l => l.ItemSubCategory)
                   .Where(l => l.IsDeleted == true && l.IsActive == false)
                   .OrderBy(l => l.Status.StatusName).ToList();
            }


            #region Search Filters
            if (!String.IsNullOrEmpty(StatusId))
            {
                Status searchStatus = db.Status.Find(Convert.ToInt32(StatusId));

                int status = db.Status.Where(s => s.StatusId == searchStatus.StatusId)
                                      .Select(s => s.StatusId)
                                      .FirstOrDefault();

                licenses = licenses.Where(l => l.StatusId == status)
                                   .OrderBy(l => l.Status.StatusName).ToList();

            }

            //LM.20150209a 
            if (SearchCriteria == "BusinessName")
            {
                //Search by business name
                licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
            }
            else if (SearchCriteria == "ReferenceNo")
            {
                //Search by reference number
                licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();
            }
            else if (SearchCriteria == "ClientName")
            {
                //Search by client name
                licenses = licenses.Where(l => (l.Client.Name + " " + l.Client.Surname).Contains(inputSearch)).ToList();
            }
            else if (SearchCriteria == "Region")
            {
                //Search by client name
                licenses = licenses.Where(l => (l.Region.RegionName).Contains(inputSearch)).ToList();
            }
            #endregion

            if (licenses.Count <= 0)
            {
                TempData[LicenseApplicationDetails.ErrorKey] = "No records to display.";
            }

            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            licenseApplicationDetails.DeletedLicenseCount = licenses.Count;
            licenseApplicationDetails.DeletedLicensePageList = licenses.ToPagedList(pageNumber, pageSize);

            return View(licenseApplicationDetails);
        }

        [Authorize(Roles = RoleKeys.SystemAdmin + "," + RoleKeys.LicensingManager + "," + RoleKeys.LicensingAdministrator)]
        [HttpPost]

        public ActionResult CancelLicense(string id)
        {
            string message = "";
            string licensenumber = "";
            try
            {
                int Id = Convert.ToInt32(id);
                License license = db.Licenses.Find(Id);
                license.IsActive = false;
                license.IsDeleted = true;
                license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseDeleted).Select(s => s.StatusId).FirstOrDefault();
                licensenumber = license.LicenseNumber;

                //Flag all inspections as deleted.
                List<InspectionRequest> inspectionRequests = db.InspectionRequests.Where(i => i.LicenseId == Id).ToList();
                foreach (var inspectionRequest in inspectionRequests)
                {
                    inspectionRequest.IsActive = false;
                    inspectionRequest.IsDeleted = true;
                    inspectionRequest.ModifiedDateTime = DateTime.Now;
                    db.InspectionRequests.AddOrUpdate(inspectionRequest);
                }
                List<InspectionResponse> inspectionResponses = db.InspectionResponse.Where(i => i.LicenseId == Id).ToList();
                foreach (var inspectionResponse in inspectionResponses)
                {
                    inspectionResponse.IsActive = false;
                    inspectionResponse.IsDeleted = true;
                    inspectionResponse.ModifiedDateTime = DateTime.Now;
                    db.InspectionResponse.AddOrUpdate(inspectionResponse);
                }
                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();
                TempData[LicenseApplicationDetails.ErrorKey] = "License Application " + licensenumber + " Deleted.";
                return Json(new { success = true, msg = message, licensenum = licensenumber });
            }
            catch (Exception e)
            {
                return Json(new { success = false, msg = message, licensenum = licensenumber });
            }

        }
        // POST: /License/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {

            License license = db.Licenses.Find(id);
            license.StatusId =
                         db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApplicationTerminated).Select(s => s.StatusId).FirstOrDefault();
            license.IsActive = false;
            license.IsDeleted = true;
            db.Entry(license).State = EntityState.Modified;
            //db.Licenses.Remove(license);
            db.SaveChanges();
            TempData["Error"] = "License Application terminated.";
            return RedirectToAction("Details", new { id = license.LicenseId });//return RedirectToAction("ApprovedLicenses");

        }
        #endregion
        public ActionResult Details(int? Id, string Screen)
        {
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            Initialise();
            try
            {

                License license = null;
                if (Id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                license = db.Licenses.Where(l => l.LicenseId == Id)
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

                licenseApplicationDetails.LicenseDetails = license;
                Business business = db.Businesses.Find(license.BusinessId);
                if (null != business)
                {

                    User UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                    licenseApplicationDetails.UserDetails = UserId;
                    licenseApplicationDetails.BusinessDetails = business;
                    licenseApplicationDetails.CustomerDetails = license.Client;
                    licenseApplicationDetails.LicenseDetails = license;
                    licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
                    licenseApplicationDetails.ItemTypeDetails = license.ItemType;
                    licenseApplicationDetails.InspectionResponses = db.InspectionResponse.Where(l => l.LicenseId == Id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                    licenseApplicationDetails.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();
                    licenseApplicationDetails.RegionDetails = license.Region;

                    if (license.StatusId != db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApplicationTerminated).Select(s => s.StatusId).FirstOrDefault())
                    {
                        // Gets list of required documents based on the document type key
                        int documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.license_doc).Select(d => d.DocumentTypeId).FirstOrDefault();
                        licenseApplicationDetails.BussinessDocuments = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType);
                        //var clientDocs = db.FileUploads.Where(d => d.ClientId == business.ClientId).Include(d => d.Document).ToList();
                        IQueryable<FileUpload> uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == DocumentTypeKeys.license_doc).Select(s => s.DocumentTypeId).FirstOrDefault());

                        List<Document> documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).ToList();
                        List<DocumentCheckList> docs1 = db.DocumentCheckLists.Where(dc => dc.LicenseTypeId == license.LicenseTypeId).Include(dc => dc.Document).ToList();


                        List<Document> OustandingDocuments = (from d in docs1
                                                              where !(from f in uploadedFiles select f.DocumentId).Contains(d.DocumentId)
                                                              select d.Document).ToList();

                        licenseApplicationDetails.BussinessDocuments = OustandingDocuments;

                        licenseApplicationDetails.BussinessUploadList = uploadedFiles;
                    }
                }

                licenseApplicationDetails.Screen = Screen;

                return View(licenseApplicationDetails);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        public ActionResult Uploadlicense(int? Id, string Screen)
        {
            Initialise();
            try
            {

                License license = null;
                if (Id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                license = db.Licenses.Where(l => l.LicenseId == Id)
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
                var business = db.Businesses.Find(license.BusinessId);
                if (null != business)
                {

                    var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                    ViewData["Userdata"] = UserId;
                    ViewData["BusinessData"] = license.Business;
                    ViewData["ClientData"] = license.Client;
                    ViewData["licenceData"] = license;

                    ViewData["InspectionResponse"] = db.InspectionResponse.Where(l => l.LicenseId == Id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                    ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
                    ViewData["LicenseType"] = license.LicenseType;
                    ViewData["ItemType"] = license.ItemType;
                    ViewData["Region"] = license.Region;
                    if (license.StatusId != db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApplicationTerminated).Select(s => s.StatusId).FirstOrDefault())
                    {
                        // Gets list of required documents based on the document type key
                        var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.license_doc).Select(d => d.DocumentTypeId).FirstOrDefault();
                        ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType);
                        //var clientDocs = db.FileUploads.Where(d => d.ClientId == business.ClientId).Include(d => d.Document).ToList();
                        var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == DocumentTypeKeys.license_doc).Select(s => s.DocumentTypeId).FirstOrDefault());

                        var documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).ToList();
                        List<DocumentCheckList> docs1 = db.DocumentCheckLists.Where(dc => dc.LicenseTypeId == license.LicenseTypeId).Include(dc => dc.Document).ToList();


                        var OustandingDocuments = (from d in docs1
                                                   where !(from f in uploadedFiles select f.DocumentId).Contains(d.DocumentId)
                                                   select d.Document.DocumentName).ToList();
                        ViewBag.OustandingDocuments = OustandingDocuments;
                        ViewData["UploadList"] = uploadedFiles;
                    }
                }






                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.InspectionRequests = new SelectList(db.InspectionRequests, "InspectionRequestId", "RefNumber");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.Screen = Screen;

                return View(license);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Uploadlicense(License license, IEnumerable<HttpPostedFileBase> file)
        {
            Initialise();
            license = db.Licenses.Where(l => l.LicenseId == license.LicenseId)
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
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.license_doc && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            if (file.First() == null)
            {

                TempData["Error"] = "Please Upload License document";
                return RedirectToAction("Uploadlicense", new { Id = license.LicenseId });
            }
            var document = db.Documents.Include(d => d.DocumentType).First(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);
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


                    int fileLen = fileupload.ContentLength;
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(fileupload.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(fileupload.ContentLength);
                    }
                    try
                    {
                        SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "License print out");
                        _fileHelpers.SaveFile(uploadName, sharePointDocument.DocumentUrl, license.Client.ClientId
                        , license.Client.Fullname, document.DocumentId, license.LicenseId, string.Empty, user.Username);
                    }
                    catch (System.Web.Services.Protocols.SoapException ex)
                    {
                        throw ex;
                    }
                }
            }
            return RedirectToAction("Details", "License", new { id = license.LicenseId });
        }
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult LicenseRenewal(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
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

            var licenses = db.Licenses.Include(l => l.Business)
             .Include(l => l.Region)
             .Include(l => l.Client)
             .Include(l => l.CreatedByUser)
             .Include(l => l.LicenseType)
             .Include(l => l.ModifiedByUser)
             .Include(l => l.Status)
             .Include(l => l.ItemType)
             .Include(l => l.ItemSubCategory)
             .Where(c => c.IsDeleted == false && c.IsActive == true && c.RegionId == Users.Region).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.PendingRenewal).Select(s => s.StatusId)
                 .FirstOrDefault()).ToList();
            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
            {
                licenses = db.Licenses.Include(l => l.Business)
             .Include(l => l.Region)
             .Include(l => l.Client)
             .Include(l => l.CreatedByUser)
             .Include(l => l.LicenseType)
             .Include(l => l.ModifiedByUser)
             .Include(l => l.Status)
             .Include(l => l.ItemType)
             .Include(l => l.ItemSubCategory)
             .Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.PendingRenewal).Select(s => s.StatusId)
                 .FirstOrDefault()).ToList();
            }



            //var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());
            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        licenses = licenses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        licenses = licenses.Where(l => l.Business.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "LicenseNumber":
                        licenses = licenses.Where(l => (l.LicenseNumber == inputSearch)).ToList();
                        break;
                    case "Region":
                        //Search by Region
                        licenses = licenses.Where(l => l.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                    default:
                        licenses = licenses.ToList();
                        break;
                }
            }
            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //TempData["Error"] = null;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.LicenseRenewal = licenses.Count;
            return View(licenses.ToPagedList(pageNumber, pageSize));
            //return View(licenses);

        }
        public ActionResult Prepopulatedropdown(string selectedType)
        {
            try
            {
                int ItemTypeId = Int32.Parse(selectedType);
                var ItemtypeKey = db.ItemTypes.Where(i => i.ItemTypeId == ItemTypeId && i.IsDeleted == false && i.IsActive == true).Select(s => s.ItemTypeKey).FirstOrDefault();
                var LicenseType = db.LicenseTypes.Where(i => i.LicenseTypeKey == ItemtypeKey).FirstOrDefault();

                var ItemSubCategory = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == ItemTypeId && i.IsDeleted == false && i.IsActive == true), "ItemSubCategoryId", "ItemSubCategoryName");
                var ItemCondition = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName");
                return Json(new
                {
                    success = true,
                    ItemCondition = ItemCondition,
                    ItemSubCategory = ItemSubCategory,
                    LicenseType = LicenseType.LicenseTypeId,

                    JsonRequestBehavior.AllowGet

                });

            }

            catch (Exception e)
            {
                var em = e.Message;
                return Json(new { success = false });
            }
        }
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult LicenseAmendment(int? id)
        {
            Initialise();
            LicenseApplicationDetails LicenseApplicationDetails = _licenseHelper.GetAmendLicenseVM(id.Value);
            TempData[LicenseApplicationDetails.SuccessKey] = null;
            TempData[LicenseApplicationDetails.InfoKey] = null;
            TempData[LicenseApplicationDetails.ErrorKey] = null;
            return View(LicenseApplicationDetails);
        }


        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LicenseAmendment(LicenseApplicationDetails license, IEnumerable<HttpPostedFileBase> files, string DepartmentTableData, string RegionId, string RegionId2, string CustomerType, string IndividualType, string Nationality, string LicenseTypedp, string ItemTypeIddp, string ItemSubCategoryId, string ItemConditionId, string LicenseTypedp2, string ItemTypeIddp2, string ItemSubCategoryId2, string ItemConditionId2, string CustomerType2, string IndividualType2, string Nationality2, string PayinslipNumberData, string BusinessPostalAddress3, string BusinessPostalAddress2, string BusinessResidentialAddress2, string BusinessResidentialAddress3, string OperatorTableData, string EmployeeTableData, BusinessManger BusinessManger, BusinessEmployee BusinessEmployee, string EditOperatorTableData, string EditEmployeeTableData)
        {
            int error = 0;
            Initialise();

            char[] spearator = { ',' };
            var validPayInSlip = true;
            String[] PayinslipNumberslist = PayinslipNumberData.Split(spearator, StringSplitOptions.None);

            bool inspectionsCompleteCheck = false;

            license.LicenseDetails.IsActive = true;
            if (PayinslipNumberData == "" || PayinslipNumberData == null)
            {
                /////Payment is required for all
                TempData["Error"] = "Please query a Pay in Slip Number";
            }
            foreach (var value in PayinslipNumberslist)
            {
                if (value == "required")
                {
                    validPayInSlip = false;
                    break;
                }
            }
            if (validPayInSlip == true)
            {
                if (license.AmendmentDetails.AmendmentType == "Normal Amendment")
                {
                    try
                    {
                        int fileUploaded = 0;
                        var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
                        var docs2 = new List<Document>();
                        var itemtypeval = db.ItemTypes.Where(c => c.ItemTypeId == license.BusinessDetails.ItemTypeId).Select(s => s.ItemTypeName).FirstOrDefault();
                        if (itemtypeval == "Item2")
                        {
                            docs2 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType)
                            .ToList();
                        }
                        else
                        {
                            docs2 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false
                            && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management).Include(d => d.DocumentType).ToList();
                        }

                        var clientDocs2 = db.FileUploads.Include(d => d.Document)
                                                       .Where(d => d.ClientId == license.BusinessDetails.ClientId && d.referenceId == license.BusinessDetails.BusinessId && d.Document.DocumentTypeId == documentTypeId)
                                                       .ToList();
                        var OustandingDocuments = (from d in docs2
                                                   where !(from f in clientDocs2 select f.DocumentId).Contains(d.DocumentId)
                                                   select d.DocumentName
                                                      ).ToList();


                        #region Upload
                        if (files != null && files.Any())
                        {
                            //TG20141028 - FileUpload
                            license.BusinessDetails.IsActive = true;
                            license.CustomerDetails.IsActive = true;
                            license.LicenseDetails.IsActive = true;
                            db.SaveAmendments(license.AmendmentDetails.AmendmentType, license.LicenseDetails.LicenseId);
                            LicenseApplicationDto licenseApplication = new LicenseApplicationDto
                            {
                                License = license.LicenseDetails,
                                Business = license.BusinessDetails,
                                Client = license.CustomerDetails,
                                AmendmentType = AmendmentType.Normal
                            };
                            int amendmentId = new LicenseHelper().AmendLicense(licenseApplication);
                            new LicenseHelper().SendLicenseForAdministratorReview(license.LicenseDetails.LicenseId, this);

                            int counter = 0;

                            foreach (HttpPostedFileBase file in files)
                            {

                                if (file != null)
                                {

                                    int fileLen = file.ContentLength;
                                    byte[] fileData = null;
                                    using (var binaryReader = new BinaryReader(file.InputStream))
                                    {
                                        fileData = binaryReader.ReadBytes(file.ContentLength);
                                    }
                                    var fname = file.FileName;
                                    SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fname, "Business amendment");
                                    _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, license.BusinessDetails.ClientId
                                    , license.CustomerDetails.Fullname, docs2[0].DocumentId, amendmentId, string.Empty, Users.Username);
                                    fileUploaded = 1;
                                    counter++;
                                }
                                else
                                {
                                    counter++;
                                }

                            }

                        }

                        #endregion Sharepoint File upload
                        clientDocs2 = db.FileUploads.Include(d => d.Document)
                                   .Where(d => d.ClientId == license.BusinessDetails.ClientId && d.referenceId == license.BusinessDetails.BusinessId && d.Document.DocumentTypeId == documentTypeId)
                                   .ToList();

                        if (fileUploaded == 1)
                        {
                            license.LicenseDetails.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).Select(s => s.StatusId).
                                    FirstOrDefault();                      
                            // Manager
                            char[] spearator2 = { '*' };
                            if (OperatorTableData != "")
                            {
                                try
                                {
                                    String[] OperatorTableDatalist = OperatorTableData.Split(spearator2, StringSplitOptions.None);
                                    OperatorTableDatalist = OperatorTableDatalist.Where(t => t != "").ToArray();
                                    foreach (var item in OperatorTableDatalist)
                                    {
                                        char[] separator = { ',' };
                                        String[] Data = item.Split(separator, StringSplitOptions.None);
                                        Data = Data.Where(t => t != "" && t != " ").ToArray();
                                        BusinessManger.BusinessId = Int32.Parse(license.BusinessDetails.BusinessId.ToString());
                                        BusinessManger.BusinessOperatorId = Int32.Parse(Data[0]);
                                        BusinessManger.NameOfBusinessOperator = Data[1];
                                        BusinessManger.OperatorIdentityOrPassportNumber = Data[2];
                                        BusinessManger.OperatorResidentialAddress1 = Data[3];
                                        BusinessManger.OperatorResidentialAddress3 = Data[4];
                                        BusinessManger.OperatorResidentialAddress2 = Data[5];
                                        BusinessManger.OperatorResidentialAddressCode = Int32.Parse(Data[6]);
                                        BusinessManger.IsActive = true;
                                        BusinessManger.IsDeleted = false;
                                        BusinessManger.IsLocked = false;
                                        db.BusinessManger.Add(BusinessManger);
                                        db.SaveChanges();
                                    }
                                }
                                catch (Exception x)
                                {
                                }
                            }
                            //edit manager
                            if (EditOperatorTableData != "")
                            {

                                String[] OperatorTableDatalist = EditOperatorTableData.Split(spearator2, StringSplitOptions.None);
                                OperatorTableDatalist = OperatorTableDatalist.Where(t => t != "").ToArray();
                                foreach (var item in OperatorTableDatalist)
                                {
                                    char[] separator = { ',' };
                                    String[] Data = item.Split(separator, StringSplitOptions.None);
                                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                                    BusinessManger BusinessMangeredit = db.BusinessManger.Find(Int32.Parse(Data[0]));
                                    BusinessMangeredit.BusinessOperatorId = Int32.Parse(Data[1]);
                                    BusinessMangeredit.NameOfBusinessOperator = Data[2];
                                    BusinessMangeredit.OperatorIdentityOrPassportNumber = Data[3];
                                    BusinessMangeredit.OperatorResidentialAddress1 = Data[4];
                                    BusinessMangeredit.OperatorResidentialAddress3 = Data[5];
                                    BusinessMangeredit.OperatorResidentialAddress2 = Data[6];
                                    BusinessMangeredit.OperatorResidentialAddressCode = Int32.Parse(Data[7]);

                                    db.Entry(BusinessMangeredit).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            //Employee
                            if (EmployeeTableData != "")
                            {
                                String[] EmployeeTableDatalist = EmployeeTableData.Split(spearator2, StringSplitOptions.None);
                                EmployeeTableDatalist = EmployeeTableDatalist.Where(t => t != "").ToArray();
                                foreach (var item in EmployeeTableDatalist)
                                {
                                    char[] separator = { ',' };
                                    String[] Data = item.Split(separator, StringSplitOptions.None);
                                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                                    BusinessEmployee.BusinessId = license.BusinessDetails.BusinessId;
                                    BusinessEmployee.NameOfEmployer = Data[0];
                                    BusinessEmployee.EmployerResidentialAddress1 = Data[1];
                                    BusinessEmployee.EmployerResidentialAddress3 = Data[2];
                                    BusinessEmployee.EmployerResidentialAddress2 = Data[3];
                                    BusinessEmployee.EmployerResidentialAddressCode = Int32.Parse(Data[4]);
                                    BusinessEmployee.IsActive = true;
                                    BusinessEmployee.IsDeleted = false;
                                    BusinessEmployee.IsLocked = false;
                                    db.BusinessEmployee.Add(BusinessEmployee);
                                    db.SaveChanges();
                                }
                            }
                            //edit Employee
                            if (EditEmployeeTableData != "")
                            {
                                String[] EmployeeTableDatalist = EditEmployeeTableData.Split(spearator2, StringSplitOptions.None);
                                EmployeeTableDatalist = EmployeeTableDatalist.Where(t => t != "").ToArray();
                                foreach (var item in EmployeeTableDatalist)
                                {
                                    char[] separator = { ',' };
                                    String[] Data = item.Split(separator, StringSplitOptions.None);
                                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                                    BusinessEmployee BusinessEmployeeedit = db.BusinessEmployee.Find(Int32.Parse(Data[0]));
                                    BusinessEmployeeedit.NameOfEmployer = Data[1];
                                    BusinessEmployeeedit.EmployerResidentialAddress1 = Data[2];
                                    BusinessEmployeeedit.EmployerResidentialAddress3 = Data[3];
                                    BusinessEmployeeedit.EmployerResidentialAddress2 = Data[4];
                                    BusinessEmployeeedit.EmployerResidentialAddressCode = Int32.Parse(Data[5]);

                                    db.Entry(BusinessEmployeeedit).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        //else
                        //{
                        //    error = 1;
                        //    TempData["Error"] = " Please upload documents.";
                        //}
                    }
                    catch (Exception x)
                    {

                        throw x;
                    }
                }

                else if (license.AmendmentDetails.AmendmentType == "Relief Amendment")
                {
                    if (PayinslipNumberData == "" || PayinslipNumberData == null)
                    {

                        TempData["Error"] = "Please query a Pay in Slip Number";

                        return RedirectToAction("LicenseAmendment", new { id = license.LicenseDetails.LicenseId });
                    }

                    try
                    {


                        license.LicenseDetails.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.LicensePending).Select(s => s.StatusId).
                                FirstOrDefault();




                        var local = db.Set<License>()
                 .Local.FirstOrDefault(l => l.LicenseId == license.LicenseDetails.LicenseId);

                        if (local != null)
                        {
                            db.Entry(local).State = EntityState.Detached;
                        }


                        license.BusinessDetails.IsActive = true;
                        license.CustomerDetails.IsActive = true;
                        license.LicenseDetails.IsActive = true;
                        db.Entry(license.LicenseDetails).State = EntityState.Modified;
                        db.Entry(license.BusinessDetails).State = EntityState.Modified;
                        db.Entry(license.CustomerDetails).State = EntityState.Modified;
                        db.SaveAmendments(license.AmendmentDetails.AmendmentType, license.LicenseDetails.LicenseId);
                    }
                    catch (Exception x)
                    {

                        throw x;
                    }
                }
                if (license.AmendmentDetails.AmendmentType == "Transfer Amendment")
                {



                    try
                    {
                        var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
                        var docs2 = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentKey != "Other" && d.DocumentKey != TLKeys.Events_Management && d.DocumentKey != TLKeys.fingerprint_Verification && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                        var clientDocs2 = db.FileUploads.Where(d => d.ClientId == license.CustomerDetails.ClientId && d.referenceId == license.CustomerDetails.ClientId && d.Document.DocumentTypeId == documentTypeId)
                                      .Include(d => d.Document).Include(d => d.Document.DocumentType)
                                      .ToList();
                        var docsupload = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();


                        var oustandingDocuments = (from d in docs2
                                                   where !(from f in clientDocs2 select f.DocumentId).Contains(d.DocumentId)
                                                   select d.DocumentName).ToList();
                        int fileUploaded = 0;
                        int counter = 0;
                        if (files != null && files.Any())
                        {
                            //TG20141028 - FileUpload

                            ;
                            foreach (HttpPostedFileBase file in files)
                            {
                                if (file != null)
                                {
                                    int fileLen = file.ContentLength;
                                    byte[] fileData = null;
                                    using (var binaryReader = new BinaryReader(file.InputStream))
                                    {
                                        fileData = binaryReader.ReadBytes(file.ContentLength);
                                    }
                                    SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, file.FileName, "client amendment");
                                    _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, license.CustomerDetails.ClientId
                                        , license.CustomerDetails.Fullname, docsupload[counter].DocumentId, license.CustomerDetails.ClientId, string.Empty, Users.Username);
                                    fileUploaded++;
                                    counter++;
                                }
                                else
                                {
                                    counter++;
                                }
                            }
                        }
                        if (fileUploaded >= 3)
                        {
                            int years = 0;
                            DateTime zeroTime = new DateTime(1, 1, 1);
                            var issuedate = DateTime.Parse(license.LicenseDetails.LicenseIssueDateTime.ToString());
                            var currentdate = DateTime.Now.Date;
                            TimeSpan span = currentdate - issuedate;
                            if (span.Days > 120)
                            {
                                years = (zeroTime + span).Year - 1;
                            }

                            //       if (years > 0)
                            //       {

                            //           license.LicenseDetails.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.LicensePending).Select(s => s.StatusId).
                            //         FirstOrDefault();

                            //       }
                            //       else
                            //       {
                            //           license.LicenseDetails.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).Select(s => s.StatusId).
                            // FirstOrDefault();
                            //       }


                            //       var local = db.Set<License>()
                            //.Local.FirstOrDefault(l => l.LicenseId == license.LicenseDetails.LicenseId);

                            //       if (local != null)
                            //       {
                            //           db.Entry(local).State = EntityState.Detached;
                            //       }
                            license.BusinessDetails.IsActive = true;
                            license.CustomerDetails.IsActive = true;
                            license.LicenseDetails.IsActive = true;
                            //L.M.20260526 - License Information to be updated only after final approval.
                            //db.Entry(license.LicenseDetails).State = EntityState.Modified;
                            //db.Entry(license.BusinessDetails).State = EntityState.Modified;
                            //db.Entry(license.CustomerDetails).State = EntityState.Modified;
                            db.SaveAmendments(license.AmendmentDetails.AmendmentType, license.LicenseDetails.LicenseId);
                        }
                        else
                        {
                            error = 1;
                            TempData["Error"] = " Please upload required documents.";
                        }

                    }




                    catch (Exception x)
                    {

                        throw x;
                    }
                }
                if (error == 0)
                {

                    char[] separator = { '*' };

                    String[] TableDatalist = PayinslipNumberData.Split(separator, StringSplitOptions.None);
                    TableDatalist = TableDatalist.Where(t => t != "").ToArray();
                    PaymentLicense paymentlicense = new PaymentLicense();

                    foreach (var item in TableDatalist)
                    {


                        var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();



                        char[] sep = { ',' };
                        String[] Data = item.Split(sep, StringSplitOptions.None);
                        Data = Data.Where(t => t != "" && t != " ").ToArray();

                        paymentlicense.LicenseId = license.LicenseDetails.LicenseId;
                        paymentlicense.PAYINSLIP_NO = Data[0].ToString();
                        paymentlicense.CUST_ACCT_NO = Data[1].ToString();
                        paymentlicense.SERVICE_UNIT = Data[2].ToString();
                        paymentlicense.REQUEST_NO = Data[3].ToString();
                        if (Data[4].ToString() != "N/A")
                        {
                            paymentlicense.PAYINSLIP_AMOUNT = Convert.ToDouble(Data[4].ToString());
                        }
                        else
                        {
                            paymentlicense.PAYINSLIP_AMOUNT = 0;
                        }
                        if (Data[5].ToString() != "N/A")
                        {
                            paymentlicense.ALLOCATED_AMOUNT = Convert.ToDouble(Data[5].ToString());
                        }

                        if (Data[6].ToString() != "N/A")
                        {
                            paymentlicense.PAID_AMOUNT = Convert.ToDouble(Data[6].ToString());
                        }
                        if (Data[7].ToString() != "N/A")
                        {
                            paymentlicense.PAID_DATE = Data[7].ToString();
                        }

                        if (Data[8].ToString() != "N/A")
                        {
                            paymentlicense.PAYINSLIP_DATE = Data[8].ToString();
                        }
                        if (Data[9].ToString() != "N/A")
                        {
                            paymentlicense.BALANCE_UNALLOCATED_AMOUNT = Convert.ToDouble(Data[9].ToString());
                        }


                        paymentlicense.CreatedDateTime = DateTime.Now.Date;

                        if (UserId != null)
                        {
                            paymentlicense.CreatedByUserId = Int32.Parse(UserId.UserId.ToString());
                        }

                        db.PaymentLicense.Add(paymentlicense);
                        db.SaveChanges();

                    }
                    TempData["Success"] = "Saved Successfully!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {

                TempData["Error"] = "Please provide values for mandatory Pay in Slip fields";
            }

            ViewBag.Exits = "0";
            return View(_licenseHelper.GetAmendLicenseVM(license.LicenseDetails.LicenseId));
        }

        [HttpPost]
        public ActionResult LicenseAmendmentAddCondition(int licenseId, int conditionId)
        {
            // Link Condition with License
            if (conditionId != 0)
            {
                string conditionName, msg = string.Empty;
                List<LicenseApplicationConditions> licenseApplicationConditions = new List<LicenseApplicationConditions>();
                LicenseApplicationConditions LicenseApplicationConditions = new LicenseApplicationConditions();
                conditionName = db.Conditions.Where(l => l.ConditionId == conditionId).Select(c => c.ConditionName).FirstOrDefault();
                bool islicenseConditionsExit = db.LicenseApplicationConditions.Where(l => l.ConditionId == conditionId &&
                                                                                     l.LicenseId == licenseId &&
                                                                                     l.IsDeleted == false &&
                                                                                     l.IsActive
                                                                                     ).ToList().Count > 0;

                //only link if condition is not linked already
                if (!islicenseConditionsExit)
                {
                    LicenseApplicationConditions.LicenseId = licenseId;
                    LicenseApplicationConditions.ConditionId = conditionId;
                    LicenseApplicationConditions.ConditionName = conditionName;
                    LicenseApplicationConditions.IsActive = true;
                    LicenseApplicationConditions.IsDeleted = false;
                    LicenseApplicationConditions.IsLocked = false;
                    LicenseApplicationConditions.IsSelected = true;

                    db.LicenseApplicationConditions.Add(LicenseApplicationConditions);
                    db.SaveChanges();

                }
                else
                {
                    msg = "exit";
                }

                licenseApplicationConditions = db.LicenseApplicationConditions.Where(l => l.LicenseId == licenseId
                                                                                    && l.IsDeleted == false
                                                                                    && l.IsActive == true).ToList();

                return Json(new { success = true, msg = msg, licenseApplicationConditions = licenseApplicationConditions });
            }

            return Json(new { success = false });

        }

        [HttpPost]
        public ActionResult LicenseAmendmentEditCondition(int licenseApplicationConditionsId, int licenseId, int conditionId)
        {
            //Update linked condition
            if (licenseApplicationConditionsId != 0)
            {
                string conditionName = string.Empty;
                List<LicenseApplicationConditions> licenseApplicationConditions = new List<LicenseApplicationConditions>();
                conditionName = db.Conditions.Where(l => l.ConditionId == conditionId).Select(c => c.ConditionName).FirstOrDefault();

                LicenseApplicationConditions LicenseApplicationConditionsedit = db.LicenseApplicationConditions.Find(licenseApplicationConditionsId);
                LicenseApplicationConditionsedit.ConditionId = conditionId;
                LicenseApplicationConditionsedit.ConditionName = conditionName;
                LicenseApplicationConditionsedit.IsActive = true;
                LicenseApplicationConditionsedit.IsDeleted = false;
                LicenseApplicationConditionsedit.IsLocked = false;
                LicenseApplicationConditionsedit.IsSelected = true;

                db.Entry(LicenseApplicationConditionsedit).State = EntityState.Modified;
                db.SaveChanges();

                licenseApplicationConditions = db.LicenseApplicationConditions.Where(l => l.LicenseId == licenseId
                                                                                     && l.IsDeleted == false
                                                                                     && l.IsActive == true).ToList();

                return Json(new { success = true, msg = "", licenseApplicationConditions = licenseApplicationConditions });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult LicenseAmendmentRemoveCondition(int licenseApplicationConditionsId, int conditionId)
        {   //Remove linked condition
            List<LicenseApplicationConditions> licenseApplicationConditions = new List<LicenseApplicationConditions>();

            int licenseId = db.LicenseApplicationConditions.Where(l => l.LicenseApplicationConditionsId == licenseApplicationConditionsId).Select(c => c.LicenseId).FirstOrDefault();
            LicenseApplicationConditions LicenseApplicationConditionsedit = db.LicenseApplicationConditions.Find(licenseApplicationConditionsId);
            LicenseApplicationConditionsedit.IsDeleted = true;
            db.Entry(LicenseApplicationConditionsedit).State = EntityState.Modified;
            db.SaveChanges();

            licenseApplicationConditions = db.LicenseApplicationConditions.Where(l => l.LicenseId == licenseId
                                                                                && l.IsDeleted == false
                                                                                && l.IsActive == true).ToList();

            return Json(new { success = true, msg = "", licenseApplicationConditions = licenseApplicationConditions });
        }


        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {

            TempData["Success"] = null;
            TempData["Info"] = null;
            TempData["Error"] = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
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

            var business = license.Business;

            if (null != business)
            {
                ViewData["BusinessData"] = business;
                ViewData["ClientData"] = license.Client;
            }
            #region initialize licenseTypeId


            int licenceTypeId = license.LicenseTypeId;

            #endregion
            ViewData["LicenseInfo"] = license;

            ViewData["LicenseType"] = db.LicenseTypes.Find(licenceTypeId);
            ViewData["ItemType"] = db.ItemTypes.Find(business.ItemTypeId);
            ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
            ViewBag.ItemConditionId = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);
            ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionId == Users.Region && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
            if (User.IsInRole("System Admin"))
            {
                ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber");
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.LicenseTypeId = licenceTypeId;
            ViewBag.BusinessID = business.BusinessId;
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 1).OrderBy(c => c.StatusName), "StatusId", "StatusName");


            var Userlist = new SelectList(db.Users.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "FirstName");
            ViewBag.Userlist = JsonConvert.SerializeObject(Userlist);


            ViewBag.payment = db.PaymentLicense.Where(l => l.LicenseId == license.LicenseId);







            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();

            var allDocs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            var docs = new List<Document>();

            if (business.ItemType.ItemTypeName == "Item 2")
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType)
                    .ToList();
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false
                    && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management).Include(d => d.DocumentType).ToList();
            }

            var clientDocs = db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive)
                                          .ToList();

            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                                         ).ToList();

            ViewData["Documents"] = (from d in allDocs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();
            ViewData["UploadList"] = clientDocs;
            ViewBag.OustandingDocuments = OustandingDocuments;

            //Ends






            return View(license);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "LicenseId,LicenseTypeId,ClientId,BusinessId,ApplicationDateTime,StatusId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime, RegionId, ItemTypeId,ItemSubCategoryId,ItemConditionId")] License license, IEnumerable<HttpPostedFileBase> files, string DepartmentTableData, string ddlregion)
        {
            var StatusKey = db.Status.Where(s => s.StatusId == license.StatusId).Select(l => l.StatusKey).FirstOrDefault();

            var oldlicense = db.Licenses.Find(license.LicenseId);

            Initialise();

            if (ModelState.IsValid)
            {
                license.ApplicationDateTime = oldlicense.ApplicationDateTime;
                license.LicenseNumber = oldlicense.LicenseNumber;
                license.LicenseIssueDateTime = oldlicense.LicenseIssueDateTime;
                license.LicenseClosureDateTime = oldlicense.LicenseClosureDateTime;
                license.LicenseCollectedDateTime = oldlicense.LicenseCollectedDateTime;
                if (StatusKey == StatusKeys.LicenseRenewal)
                {
                    try
                    {

                        license.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId).
                                FirstOrDefault();
                        license.LicenseRenewalDateTime = DateTime.Now;
                        license.LicenseExpiryDate = DateTime.Now.Date.AddYears(1);
                        license.IsDeleted = false;
                        license.IsActive = true;


                        var local = db.Set<License>()
                 .Local.FirstOrDefault(l => l.LicenseId == license.LicenseId);

                        if (local != null)
                        {
                            db.Entry(local).State = EntityState.Detached;
                        }

                        db.Entry(license).State = EntityState.Modified;

                        ViewData["BusinessData"] = db.Businesses.Find(license.BusinessId); ;
                        ViewData["ClientData"] = db.Clients.Find(license.ClientId);
                        TempData["Success"] = "License Renewed, Available to print.";
                        db.SaveChanges();

                    }
                    catch (Exception x)
                    {

                        throw x;
                    }
                }
                else
                {


                    license.IsActive = true;
                    license.IsDeleted = false;
                    license.IsDeleted = false;
                    TempData["Success"] = "License Application saved";

                    var local = db.Set<License>()
                   .Local.FirstOrDefault(l => l.LicenseId == license.LicenseId);

                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;
                    }

                    db.Entry(license).State = EntityState.Modified;

                    ViewData["BusinessData"] = db.Businesses.Find(license.BusinessId); ;
                    ViewData["ClientData"] = db.Clients.Find(license.ClientId);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = license.LicenseId });

                }




                //Sends out emails to client and respective departments.
                //#region Construct emails
                //#region client

                //var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                //var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                //var body = string.Empty;

                //if (template != null)
                //{
                //    body = template.EmailBody;
                //}

                ////L.M.20150303a - Replace variables with actual email content
                //body = body.Replace("#NAME#", client.Fullname);
                //body = body.Replace("#BODYTEXT#", "Your application for business license has been processed and sent for inspections." + Convert.ToDateTime(license.LicenseExpiryDate).ToShortDateString());

                //var email = new tb_EmailQueue
                //{
                //    QueueDateTime = DateTime.Now,
                //    ApplicationId = applicationId,
                //    EmailAccountId = 6,//Hard coded
                //    ToList = client.EmailAddress,
                //    CcList = null,
                //    BccList = null,
                //    Subject = "TLS: License Registration",
                //    Body = body,
                //    IsHtml = true,
                //    FailureCount = 0,
                //    ReferenceId = client.IdentityOrPassportNumber,
                //    HasAttachments = false
                //};

                //core.tb_EmailQueue.Add(email);
                //core.SaveChanges();

                //#endregion client mail
                //#region inspectors

                //foreach (var dep in dept)
                //{
                //    Department dep1 = dep;
                //    var depContacts = db.DepartmentContacts.Where(d => d.DepartmentId == dep.DepartmentId && d.IsDeleted == false && d.IsActive)
                //                                           .Include(u => u.User)
                //                                           .ToList();

                //    if (depContacts.Count > 0)
                //    {
                //        foreach (var departmentContact in depContacts)
                //        {
                //            if (departmentContact.IsPrinciple == false)
                //            {
                //                var chiefInspector = db.DepartmentContacts.Include(d => d.User).FirstOrDefault(c => c.IsDeleted == false && c.IsActive && c.DepartmentContactId == departmentContact.DepartmentHeadContactId);
                //                //body =
                //                //    string.Format("Dear " + departmentContact.User.FullName +
                //                //                  "<br/>You have an outstanding inspection, please review. <br/> Regards<br/><b> Trade License Administrator<b>");

                //                if (chiefInspector != null)
                //                {
                //                    //L.M.20150303a - Replace variables with actual email content
                //                    body = body.Replace("#NAME#", departmentContact.User.FullName);
                //                    body = body.Replace("#BODYTEXT#", "You have an outstanding inspection, please logon to the system and review.");

                //                    email = new tb_EmailQueue
                //                    {
                //                        QueueDateTime = DateTime.Now,
                //                        ApplicationId = applicationId,
                //                        EmailAccountId = 6,//Hard coded
                //                        ToList = departmentContact.User.EmailAddress,
                //                        CcList = null,
                //                        BccList = null,
                //                        Subject = "TLS: " + dep.DepartmentName + " Inspection Request.",
                //                        Body = body,
                //                        IsHtml = true,
                //                        FailureCount = 0,
                //                        ReferenceId = client.IdentityOrPassportNumber,
                //                        HasAttachments = false
                //                    };

                //                    core.tb_EmailQueue.Add(email);
                //                    core.SaveChanges();

                //                }

                //            }
                //        }

                //    }
                //}
                //#endregion inspectors mail
                //#endregion email

            }
            else
            {


                ViewData["LicenseType"] = db.LicenseTypes.Find(license.LicenseTypeId);
                ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
                ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionId == Users.Region && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
                if (User.IsInRole("System Admin"))
                {
                    ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
                }
                ViewBag.ItemConditionId = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);

                ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
                ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.CreatedByUserId);
                ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName), "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.ModifiedByUserId);
                ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 1).OrderBy(s => s.StatusType.StatusTypeName), "StatusId", "StatusName", license.StatusId);
                ViewData["LicenseType"] = license.LicenseType;
                ViewData["ItemType"] = license.ItemType;
                ViewData["Region"] = license.Region;

                ViewData["licenceData"] = license;
                ViewBag.reference = license.LicenseId;




                ViewBag.payment = db.payments.ToList();
                return RedirectToAction("Edit", new { id = license.LicenseId });
            }
            return RedirectToAction("Edit", new { id = license.LicenseId });
        }
        #region License Details Method
        // GET: /License/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager"
            + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin")]
        public ActionResult lDetails(int? id)
        {
            Initialise();
            License license = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            license = db.Licenses.Where(l => l.LicenseId == id)
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
            var business = db.Businesses.Find(license.BusinessId);
            if (null != business)
            {

                var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();
                ViewData["Userdata"] = UserId;
                ViewData["BusinessData"] = license.Business;
                ViewData["ClientData"] = license.Client;
                ViewData["licenceData"] = license;

                ViewData["InspectionResponse"] = db.InspectionResponse.Where(l => l.LicenseId == id).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
                ViewData["LicenseType"] = license.LicenseType;
                ViewData["ItemType"] = license.ItemType;
                ViewData["Region"] = license.Region;

                if (license.StatusId != db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApplicationTerminated).Select(s => s.StatusId).FirstOrDefault())
                {
                    // Gets list of required documents based on the document type key
                    var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.licensedocument).Select(d => d.DocumentTypeId).FirstOrDefault();
                    ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType);
                    //var clientDocs = db.FileUploads.Where(d => d.ClientId == business.ClientId).Include(d => d.Document).ToList();
                    var uploadedFiles = db.FileUploads.Where(f => f.referenceId == license.LicenseId).Where(f => f.Document.DocumentTypeId == db.DocumentTypes.Where(s => s.DocumentTypeKey == "license_document").Select(s => s.DocumentTypeId).FirstOrDefault());

                    var documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Include(d => d.DocumentType).ToList();
                    List<DocumentCheckList> docs1 = db.DocumentCheckLists.Where(dc => dc.LicenseTypeId == license.LicenseTypeId).Include(dc => dc.Document).ToList();


                    var OustandingDocuments = (from d in docs1
                                               where !(from f in uploadedFiles select f.DocumentId).Contains(d.DocumentId)
                                               select d.Document.DocumentName).ToList();
                    ViewBag.OustandingDocuments = OustandingDocuments;
                    ViewData["UploadList"] = uploadedFiles;
                }
            }
            return View(license);
        }
        #endregion

        [HttpPost]
        public ActionResult LoadPayInSlip(string licenseId)
        {
            if (licenseId != null)
            {
                try
                {

                    int licenceID = Int32.Parse(licenseId);


                    var licensePayment = db.PaymentLicense.Where(p => p.LicenseId == licenceID).ToList();

                    Payment[] Paymentarray = new Payment[licensePayment.Count()];




                    int index = 0;
                    if (licenseId != null)
                    {
                        foreach (var record in licensePayment)
                        {

                            if (record.LicenseId == licenceID)
                            {
                                Payment dr = new Payment();
                                dr.PAYINSLIP_NO = record.PAYINSLIP_NO;
                                dr.CUST_ACCT_NO = record.CUST_ACCT_NO;
                                dr.ALLOCATED_AMOUNT = record.ALLOCATED_AMOUNT;
                                dr.BALANCE_UNALLOCATED_AMOUNT = record.BALANCE_UNALLOCATED_AMOUNT;
                                dr.PAID_AMOUNT = record.PAID_AMOUNT;
                                dr.PAID_DATE = record.PAID_DATE;
                                dr.PAYINSLIP_AMOUNT = record.PAYINSLIP_AMOUNT;
                                dr.PAYINSLIP_DATE = record.PAYINSLIP_DATE;
                                dr.REQUEST_NO = record.REQUEST_NO;
                                dr.SERVICE_UNIT = record.SERVICE_UNIT;
                                Paymentarray[index] = dr;
                                index++;
                            }

                        }
                    }

                    string result = javaScriptSerializer.Serialize(Paymentarray);
                    return Json(result);
                }




                catch (Exception ex)
                {
                    var error = ex.Message;
                }
            }
            return Json("Error");

        }
        [HttpPost]
        public ActionResult PayInSlipCheck(string PayinslipNumbers, string licenseId)
        {
            try
            {
                char[] spearator = { ',' };
                int paymentCount = 0;
                var message = "";
                var licensenumber = "";
                String[] PayinslipNumberslist = PayinslipNumbers.Split(spearator, StringSplitOptions.None);
                int licenceID = 0;
                if (licenseId != null)
                {
                    licenceID = Int32.Parse(licenseId);
                }
                var licensePayment = db.PaymentLicense.ToList();
                var Payment = db.payments.ToList();

                foreach (var item in PayinslipNumberslist)
                {
                    foreach (var payinslip in licensePayment)
                    {
                        if (payinslip.PAYINSLIP_NO == item)
                        {
                            message = "exits";
                            licensenumber = db.Licenses.Where(l => l.LicenseId == payinslip.LicenseId).Select(s => s.LicenseNumber).FirstOrDefault();
                        }
                    }

                }


                return Json(new { success = true, msg = message, licensenum = licensenumber });

            }




            catch (Exception ex)
            {
                var error = ex.Message;
            }

            return Json("Error");
        }
        [HttpPost]
        public ActionResult QueryPayInSlip(string PayinslipNumbers, string licenseId)
        {
            try
            {
                int licenceID = 0;
                if (licenseId != null)
                {
                    licenceID = Int32.Parse(licenseId);

                }



                ViewBag.Exits = "0";
                char[] spearator = { ',' };


                String[] PayinslipNumberslist = PayinslipNumbers.Split(spearator, StringSplitOptions.None);


                var Payment = db.payments.ToList();
                var licensePayment = db.PaymentLicense.ToList();

                int paymentCount = 0;

                foreach (var item in PayinslipNumberslist)
                {
                    foreach (var payinslip in Payment)
                    {
                        if (payinslip.PAYINSLIP_NO == item)
                        {
                            paymentCount++;

                        }

                    }


                }
                if (paymentCount == PayinslipNumberslist.Count())
                {
                    foreach (var item in PayinslipNumberslist)
                    {
                        foreach (var payinslip in licensePayment)
                        {

                            if (payinslip.LicenseId == licenceID)
                            {
                                paymentCount++;
                            }

                        }
                    }
                    Payment[] Paymentarray = new Payment[paymentCount];




                    int index = 0;

                    foreach (var item in Payment)
                    {
                        foreach (var nums in PayinslipNumberslist)
                        {
                            if (item.PAYINSLIP_NO == nums)
                            {
                                Payment dr = new Payment();
                                dr.PAYINSLIP_NO = item.PAYINSLIP_NO;
                                dr.CUST_ACCT_NO = item.CUST_ACCT_NO;
                                dr.ALLOCATED_AMOUNT = item.ALLOCATED_AMOUNT;
                                dr.BALANCE_UNALLOCATED_AMOUNT = item.BALANCE_UNALLOCATED_AMOUNT;
                                dr.PAID_AMOUNT = item.PAID_AMOUNT;
                                dr.PAID_DATE = item.PAID_DATE;
                                dr.PAYINSLIP_AMOUNT = item.PAYINSLIP_AMOUNT;
                                dr.PAYINSLIP_DATE = item.PAYINSLIP_DATE;
                                dr.REQUEST_NO = item.REQUEST_NO;
                                dr.SERVICE_UNIT = item.SERVICE_UNIT;
                                Paymentarray[index] = dr;
                                index++;
                            }
                        }
                    }
                    if (licenseId != null)
                    {
                        foreach (var record in licensePayment)
                        {

                            if (record.LicenseId == licenceID)
                            {
                                Payment dr = new Payment();
                                dr.PAYINSLIP_NO = record.PAYINSLIP_NO;
                                dr.CUST_ACCT_NO = record.CUST_ACCT_NO;
                                dr.ALLOCATED_AMOUNT = record.ALLOCATED_AMOUNT;
                                dr.BALANCE_UNALLOCATED_AMOUNT = record.BALANCE_UNALLOCATED_AMOUNT;
                                dr.PAID_AMOUNT = record.PAID_AMOUNT;
                                dr.PAID_DATE = record.PAID_DATE;
                                dr.PAYINSLIP_AMOUNT = record.PAYINSLIP_AMOUNT;
                                dr.PAYINSLIP_DATE = record.PAYINSLIP_DATE;
                                dr.REQUEST_NO = record.REQUEST_NO;
                                dr.SERVICE_UNIT = record.SERVICE_UNIT;
                                Paymentarray[index] = dr;
                                index++;
                            }

                        }
                    }

                    string result = javaScriptSerializer.Serialize(Paymentarray);
                    return Json(result);
                }


            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }

            return Json("Error");

        }
        public ActionResult ApplicationTrackerDetails(int? Id)
        {
            Initialise();
            try
            {
                AdministratorReviewHelpers administratorReviewHelpers = new AdministratorReviewHelpers();
                return View(administratorReviewHelpers.GetAdministratorCreateVM(Id.Value, Users));
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult LicenseRenewalHistory(int? Page, string StatusId, string selectedSearch, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            Initialise();
            if (inputSearch != null)
            {
                Page = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.CurrentSortOrder = Sorting_Order;

            ViewBag.CurrentFilter = inputSearch;
            ViewBag.selecetedSearch = selectedSearch;

            ViewBag.FilterValue = inputSearch;






            #region Search Filters
            if (selectedSearch != null)
            {
                var licenses = db.RenewalHistory.Include(l => l.License)
  .Include(l => l.License.Client)
.Include(l => l.License.Business)
  .Include(l => l.License.Region)
  .Include(l => l.CreatedByUser)
  .Include(l => l.License.LicenseType)
  .Include(l => l.ModifiedByUser)


  .Where(l => l.IsDeleted == false && l.IsActive == true && l.License.RegionId == Users.Region).ToList();

                if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
                {


                    licenses = db.RenewalHistory.Include(l => l.License)
.Include(l => l.License.Client)
.Include(l => l.License.Business)
//.Include(l => l.Client.ClientUser)
.Include(l => l.License.Region)
.Include(l => l.CreatedByUser)
.Include(l => l.License.LicenseType)
.Include(l => l.ModifiedByUser)
            .Where(l => l.IsDeleted == false && l.IsActive == true).ToList();
                }
                //LM.20150209a 
                if (selectedSearch == "BusinessName")
                {
                    //Search by business name
                    licenses = licenses.Where(l => l.License.Business.ProposedTradeName.Contains(inputSearch)).ToList();
                }
                else if (selectedSearch == "ReferenceNo")
                {
                    //Search by reference number
                    licenses = licenses.Where(l => l.License.LicenseNumber == inputSearch).ToList();
                }
                else if (selectedSearch == "Customer")
                {
                    //Search by client name
                    licenses = licenses.Where(l => (l.License.Client.Name + " " + l.License.Client.Surname).Contains(inputSearch)).ToList();
                }
                #endregion

                if (licenses.Count <= 0)
                {
                    TempData["Error"] = "No records to display.";
                }

                ViewBag.CanAction = CanAction();
                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    Page = 1;
                }
                int pageSize = 5;
                int pageNumber = (Page ?? 1);
                ViewBag.LicenseCount = licenses.Count;
                ViewBag.showTable = "1";
                //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
                return View(licenses.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                ViewBag.showTable = "0";
                var licenses = db.RenewalHistory.Where(l => l.IsLocked == true).ToList();

                ViewBag.CanAction = CanAction();
                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    Page = 1;
                }
                int pageSize = 5;
                int pageNumber = (Page ?? 1);
                ViewBag.LicenseCount = licenses.Count;
                //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
                return View(licenses.ToPagedList(pageNumber, pageSize));

            }



        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Department Manager" + "," + "Department Clerk" + "," + "Department Inspector" + "," + "Chief Inspector")]
        public ActionResult ApplicationTracker(int? Page, string StatusId, string selectedSearch, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            Initialise();
            if (inputSearch != null)
            {
                Page = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.CurrentFilter = inputSearch;
            ViewBag.selecetedSearch = selectedSearch;


            ViewBag.FilterValue = inputSearch;






            #region Search Filters
            if (selectedSearch != null)
            {
                var licenses = db.Licenses.Include(l => l.Business)
  .Include(l => l.Client)
  //.Include(l => l.Client.ClientUser)
  .Include(l => l.Region)
  .Include(l => l.CreatedByUser)
  .Include(l => l.LicenseType)
  .Include(l => l.ModifiedByUser)
  .Include(l => l.Status)
  .Include(l => l.ItemType)
  .Include(l => l.ItemSubCategory)
  .Where(l => l.IsDeleted == false && l.IsActive == true && l.RegionId == Users.Region)

  .OrderBy(l => l.Status.StatusName).ToList();
                if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
                {


                    licenses = db.Licenses.Include(l => l.Business)
                       .Include(l => l.Client)
                       //.Include(l => l.Client.ClientUser)
                       .Include(l => l.Region)
                       .Include(l => l.CreatedByUser)
                       .Include(l => l.LicenseType)
                       .Include(l => l.ModifiedByUser)
                       .Include(l => l.Status)
                       .Include(l => l.ItemType)
                       .Include(l => l.ItemSubCategory)
                       .Where(l => l.IsDeleted == false && l.IsActive == true)

                       .OrderBy(l => l.Status.StatusName).ToList();
                }
                //LM.20150209a 
                if (selectedSearch == "BusinessName")
                {
                    //Search by business name
                    licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
                }
                else if (selectedSearch == "ReferenceNo")
                {
                    //Search by reference number
                    licenses = licenses.Where(l => l.LicenseNumber == inputSearch).ToList();
                }
                else if (selectedSearch == "ClientName")
                {
                    //Search by client name
                    licenses = licenses.Where(l => (l.Client.Fullname).Contains(inputSearch)).ToList();
                }
                #endregion

                if (licenses.Count <= 0)
                {
                    TempData["Error"] = "No records to display.";
                }

                ViewBag.CanAction = CanAction();
                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    Page = 1;
                }
                int pageSize = 5;
                int pageNumber = (Page ?? 1);
                ViewBag.LicenseCount = licenses.Count;
                ViewBag.showTable = "1";
                //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
                return View(licenses.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                ViewBag.showTable = "0";
                var licenses = db.Licenses.Where(l => l.IsLocked == true).OrderBy(l => l.Status.StatusName).ToList();

                ViewBag.CanAction = CanAction();
                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    Page = 1;
                }
                int pageSize = 5;
                int pageNumber = (Page ?? 1);
                ViewBag.LicenseCount = licenses.Count;
                //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
                return View(licenses.ToPagedList(pageNumber, pageSize));

            }



        }
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Department Manager" + "," + "Department Clerk" + "," + "Department Inspector" + "," + "Chief Inspector")]
        public ActionResult DepartmentTracker(int? Page, string StatusId, string selectedSearch, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            Initialise();
            if (inputSearch != null)
            {
                Page = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.CurrentFilter = inputSearch;
            ViewBag.selecetedSearch = selectedSearch;


            ViewBag.FilterValue = inputSearch;






            #region Search Filters
            ViewData["InspectionResponsedata"] = db.InspectionResponse.Include(l => l.InspectionRequest).ToList();
            ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
            var licenses = db.InspectionRequests.Include(l => l.License).Include(l => l.Department)
  .Include(l => l.License.Client)
  .Include(l => l.License.Business)
  .Include(l => l.License.Region)
  .Include(l => l.CreatedByUser)
  .Include(l => l.License.LicenseType)
  .Include(l => l.ModifiedByUser)
  .Include(l => l.Status)
  .Include(l => l.License.ItemType)
  .Include(l => l.License.ItemSubCategory)

  .Where(l => l.IsDeleted == false && l.License.RegionId == Users.Region && l.License.Status.StatusKey == StatusKeys.PendingLicenseInspection || l.License.Status.StatusKey == StatusKeys.AwaitingManagerResponse)

  .OrderBy(l => l.Status.StatusName).ToList();
            if (User.IsInRole("System Admin") || User.IsInRole("Licensing Manager") || User.IsInRole("Licensing Administrator"))
            {


                licenses = db.InspectionRequests.Include(l => l.License).Include(l => l.Department)
.Include(l => l.License.Client)
.Include(l => l.License.Business)
.Include(l => l.License.Region)
.Include(l => l.CreatedByUser)
.Include(l => l.License.LicenseType)
.Include(l => l.ModifiedByUser)
.Include(l => l.Status)
.Include(l => l.License.ItemType)
.Include(l => l.License.ItemSubCategory)

.Where(l => l.IsDeleted == false && (l.License.Status.StatusKey == StatusKeys.PendingLicenseInspection || l.License.Status.StatusKey == StatusKeys.AwaitingManagerResponse))
.OrderBy(l => l.Status.StatusName).ToList();

            }
            if (selectedSearch != null)
            {
                //LM.20150209a 
                if (selectedSearch == "BusinessName")
                {
                    //Search by business name
                    licenses = licenses.Where(l => l.License.Business.ProposedTradeName.Contains(inputSearch)).ToList();
                }
                else if (selectedSearch == "ReferenceNo")
                {
                    //Search by reference number
                    licenses = licenses.Where(l => l.License.LicenseNumber == inputSearch).ToList();
                }
                else if (selectedSearch == "Department")
                {
                    //Search by client name
                    licenses = licenses.Where(l => l.Department.DepartmentName.Contains(inputSearch)).ToList();
                }
                #endregion

                if (licenses.Count <= 0)
                {
                    TempData["Error"] = "No records to display.";
                }
            }
            Status awaingDepartmentManagerStatus = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.AwaitingManagerResponse);

            foreach (var inspection in licenses)
                inspection.IsLocked = inspection.StatusId != awaingDepartmentManagerStatus.StatusId;

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                Page = 1;
            }
            int pageSize = 5;
            int pageNumber = (Page ?? 1);
            ViewBag.LicenseCount = licenses.Count;
            ViewBag.showTable = "1";
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ReCirculatePartial(int inspectionRequestId)
        {
            var inspection = db.InspectionRequests
                .FirstOrDefault(x => x.InspectionRequestId == inspectionRequestId);

            if (inspection == null)
            {
                return HttpNotFound();
            }

            var vm = new ReCirculateVM
            {
                InspectionRequestId = inspection.InspectionRequestId,
                CurrentInspectorId = inspection.DepartmentContactId,
                Inspectors = GetInspectors()
            };

            return PartialView("_ReCirculatePartial", vm);
        }

        private List<SelectListItem> GetInspectors()
        {
            return db.DepartmentContacts
                .Include(d => d._DepartmentId)
                .Where(x => x.IsPrinciple && (x.RoleName.Equals(RoleKeys.DepartmentManager) || x.RoleName.Equals(RoleKeys.ChiefInspector)))
                .Include(x => x.User)
                 .Select(x => new SelectListItem
                 {
                     Value = x.DepartmentContactId.ToString(),
                     Text = x.User.FirstName + " " + x.User.LastName + " (" + x._DepartmentId.DepartmentName + ")"
                 }).ToList();
        }

        [HttpPost]
        public ActionResult SaveReCirculation(ReCirculateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Inspectors = GetInspectors();

                return PartialView("_ReCirculatePartial", vm);
            }

            var inspection = db.InspectionRequests
                .FirstOrDefault(x =>
                    x.InspectionRequestId == vm.InspectionRequestId);

            if (inspection == null)
            {
                return HttpNotFound();
            }
            DepartmentContact departmentContact = db.DepartmentContacts.FirstOrDefault(d => d.DepartmentContactId == vm.NewInspectorId);
            if (departmentContact == null) return HttpNotFound();

            LicenseCirculationHelper.ReCirculateLicense(vm.InspectionRequestId, departmentContact.DepartmentId, vm.Reason, this);

            return Json(new
            {
                success = true
            });
        }
        public ActionResult LicenseDocUploads(int? licenseId, string Screen)
        {
            var license = db.Licenses.Where(l => l.LicenseId == licenseId).FirstOrDefault();


            var clientDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.Clientdocument && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();
            var BussinessDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();
            var InspectionRequestDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();
            var InspectionDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();
            var RefusalDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.Refusaldocument && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();
            var RefusalsignDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.Refusalsign && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();
            var AppealDocumentTypes = db.DocumentTypes.Where(r => r.DocumentTypeKey == DocumentTypeKeys.Appealdocument && r.IsDeleted == false && r.IsActive == true).Select(s => s.DocumentTypeId).FirstOrDefault();

            var clientDocs = db.FileUploads.Include(d => d.Document)
                                   .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == clientDocumentTypes && d.IsDeleted == false && d.IsActive == true && d.referenceId == license.ClientId).ToList();
            var BussinessDocs = db.FileUploads.Include(d => d.Document)
                                   .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == BussinessDocumentTypes && d.IsDeleted == false && d.IsActive == true && d.referenceId == license.BusinessId).ToList();
            var InspectionRequestDocs = db.FileUploads.Include(d => d.Document)
                                  .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == InspectionRequestDocumentTypes && d.IsDeleted == false && d.IsActive == true).ToList();

            var InspectionDocs = db.FileUploads.Include(d => d.Document)
                                  .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == InspectionDocumentTypes && d.IsDeleted == false && d.IsActive == true).ToList();
            var RefusalDocs = db.FileUploads.Include(d => d.Document)
                                 .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == RefusalDocumentTypes && d.IsDeleted == false && d.IsActive == true).ToList();
            var RefusalsignDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == RefusalsignDocumentTypes && d.IsDeleted == false && d.IsActive == true).ToList();
            var AppealDocs = db.FileUploads.Include(d => d.Document)
                               .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == AppealDocumentTypes && d.IsDeleted == false && d.IsActive == true).ToList();

            ViewData["RefusalDocs"] = RefusalDocs;
            ViewData["clientDocs"] = clientDocs;
            ViewData["InspectionDocs"] = InspectionDocs;
            ViewData["BussinessDocs"] = BussinessDocs;
            ViewData["RefusalsignDocs"] = RefusalsignDocs;
            ViewData["InspectionRequestDocs"] = InspectionRequestDocs;
            ViewData["AppealDocs"] = AppealDocs;
            ViewData["InspectionData"] = db.InspectionResponse.Where(l => l.LicenseId == licenseId).Include(l => l.InspectionRequest.Department).ToList();
            ViewData["InspectionRequestData"] = db.InspectionRequests.Where(l => l.LicenseId == licenseId).Include(l => l.Department).ToList();
            ViewData["RefusalData"] = db.Refusa.Where(l => l.LicenseId == licenseId).ToList();
            ViewData["AppealData"] = db.InspectionAppeal.Where(l => l.LicenseId == licenseId).ToList();
            ViewBag.Screen = Screen;
            return View();
        }
        #region License Create methods




        [Authorize(Roles = "Licensing Administrator,Licensing Clerk,Licensing Manager,System Admin")]
        public ActionResult Create(int? id)
        {
            Initialise();
            ViewBag.Exits = "0";

            var business = db.Businesses.Where(b => b.BusinessId == id)
                .Include(b => b.Client)
                .Include(b => b.ItemType)
                .FirstOrDefault();

            if (business == null)
            {
                return HttpNotFound();
            }

            ViewData["BusinessData"] = business;
            ViewData["ClientData"] = business.Client;

            int licenceTypeId = 0;

            #region initialize licenseTypeId
            if (business.ItemTypeId == db.ItemTypes.Where(it => it.ItemTypeKey == "sale_supply").Select(it => it.ItemTypeId).FirstOrDefault())
            {
                licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "sale_supply").Select(lt => lt.LicenseTypeId).FirstOrDefault();
            }
            else if (business.ItemTypeId == db.ItemTypes.Where(it => it.ItemTypeKey == "health_entertainment").Select(it => it.ItemTypeId).FirstOrDefault())
            {
                licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "health_entertainment").Select(lt => lt.LicenseTypeId).FirstOrDefault();
            }
            else if (business.ItemTypeId == db.ItemTypes.Where(it => it.ItemTypeKey == "hawking").Select(it => it.ItemTypeId).FirstOrDefault())
            {
                licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "hawking").Select(lt => lt.LicenseTypeId).FirstOrDefault();
            }
            else
            {
                licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "accommodation").Select(lt => lt.LicenseTypeId).FirstOrDefault();
            }
            #endregion

            // === Dropdown Lists for License Type and Item Type ===
            ViewBag.LicenseTypeId = new SelectList(
                db.LicenseTypes.Where(lt => lt.IsDeleted == false && lt.IsActive == true)
                               .OrderBy(lt => lt.LicenseTypeName),
                "LicenseTypeId", "LicenseTypeName", licenceTypeId);

            ViewBag.ItemTypeId = new SelectList(
                db.ItemTypes.Where(it => it.IsDeleted == false && it.IsActive == true)
                            .OrderBy(it => it.ItemTypeName),
                "ItemTypeId", "ItemTypeName", business.ItemTypeId);

            // Keep your original ViewData (still used in view)
            ViewData["LicenseType"] = db.LicenseTypes.Find(licenceTypeId);
            ViewData["ItemType"] = db.ItemTypes.Find(business.ItemTypeId);

            // Other dropdowns
            ViewBag.ItemSubCategoryId = new SelectList(
                db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId
                                             && i.IsDeleted == false
                                             && i.IsActive == true
                                             && i.ItemSubCategoryName != "Migrated Item Sub-Category"),
                "ItemSubCategoryId", "ItemSubCategoryName");

            ViewBag.ItemConditionId = new SelectList(
                db.ItemConditions.Where(i => i.ItemTypeId == business.ItemTypeId
                                          && i.IsDeleted == false
                                          && i.IsActive),
                "ItemConditionId", "ItemConditionName");

            ViewBag.RegionId = new SelectList(
                db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"),
                "RegionId", "RegionName");

            if (User.IsInRole("System Admin"))
            {
                ViewBag.RegionId = new SelectList(
                    db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"),
                    "RegionId", "RegionName");
            }

            // Other ViewBags
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber");
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");

            ViewBag.BusinessID = business.BusinessId;
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 1).OrderBy(c => c.StatusName), "StatusId", "StatusName");

            // Department tab
            ViewBag.currentdate = DateTime.Now.ToShortDateString();
            ViewBag.DepartmentList = new SelectList(db.Departments.Where(i => i.IsDeleted == false && i.IsActive == true), "DepartmentId", "DepartmentName");

            var DepartmentUser = new SelectList(db.DepartmentContacts.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "DepartmentId");
            ViewBag.DepartmentUser = JsonConvert.SerializeObject(DepartmentUser);

            var Userlist = new SelectList(db.Users.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "FirstName");
            ViewBag.Userlist = JsonConvert.SerializeObject(Userlist);

            ViewBag.Decision = new SelectList(new[] { "Approve", "Reject" });
            ViewBag.payment = db.PaymentLicense.Where(l => l.LicenseId == 0);

            TempData["Success"] = null;
            TempData["Error"] = null;
            TempData["Info"] = null;

            // Document Logic (unchanged)
            Business business1 = db.Businesses.Include(b => b.BusinessStatus)
                                         .Include(b => b.BusinessType)
                                         .Include(b => b.OperationStructureType)
                                         .Include(b => b.TitleDeedType)
                                         .Include(b => b.ItemType)
                                         .FirstOrDefault(b => b.BusinessId == id);

            if (business1 == null)
            {
                return HttpNotFound();
            }

            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument
                                                           && dt.IsDeleted == false && dt.IsActive)
                                                 .Select(d => d.DocumentTypeId)
                                                 .FirstOrDefault();

            var allDocs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId)
                                      .Where(d => d.IsActive && d.IsDeleted == false)
                                      .Include(d => d.DocumentType).ToList();

            var docs = new List<Document>();

            if (business1.ItemType.ItemTypeName == "Item 2")
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId)
                                   .Where(d => d.IsActive == true && d.IsDeleted == false)
                                   .Include(d => d.DocumentType).ToList();
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId)
                                   .Where(d => d.IsActive && d.IsDeleted == false
                                       && d.DocumentKey != TLKeys.fingerprint_Verification
                                       && d.DocumentKey != TLKeys.Events_Management)
                                   .Include(d => d.DocumentType).ToList();
            }

            var clientDocs = db.FileUploads.Include(d => d.Document)
                                           .Where(d => d.ClientId == business1.ClientId
                                                    && d.referenceId == business1.BusinessId
                                                    && d.Document.DocumentTypeId == documentTypeId
                                                    && d.IsDeleted == false && d.IsActive)
                                           .ToList();

            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName).ToList();

            ViewData["Documents"] = (from d in allDocs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();

            ViewData["UploadList"] = clientDocs;
            ViewBag.OustandingDocuments = OustandingDocuments;

            return View();
        }


        public JsonResult GetSubCategoriesByItemType(int itemTypeId)
        {
            var data = db.ItemSubCategories
                .Where(i => i.ItemTypeId == itemTypeId
                         && i.IsDeleted == false
                         && i.IsActive == true
                         && i.ItemSubCategoryName != "Migrated Item Sub-Category")
                .OrderBy(i => i.ItemSubCategoryName)
                .Select(i => new { Value = i.ItemSubCategoryId, Text = i.ItemSubCategoryName })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemConditionsByItemType(int itemTypeId)
        {
            var data = db.ItemConditions
                .Where(i => i.ItemTypeId == itemTypeId
                         && i.IsDeleted == false
                         && i.IsActive == true)
                .OrderBy(i => i.ItemConditionName)
                .Select(i => new { Value = i.ItemConditionId, Text = i.ItemConditionName })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }





        //// GET: /License/Create
        //[Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        //public ActionResult Create(int? id)
        //{
        //    Initialise();
        //    ViewBag.Exits = "0";
        //    var business = db.Businesses.Where(b => b.BusinessId == id)
        //        .Include(b => b.Client)
        //        .Include(b => b.ItemType)
        //        .FirstOrDefault();
        //    int licenceTypeId = 0;

        //    if (null != business)
        //    {

        //        ViewData["BusinessData"] = business;
        //        ViewData["ClientData"] = business.Client;
        //        //ViewBag.ClientId = client.ClientId; 
        //    }


        //    #region initialize licenseTypeId

        //    if (business.ItemTypeId == db.ItemTypes.Where(it => it.ItemTypeKey == "sale_supply").Select(it => it.ItemTypeId).FirstOrDefault())
        //    {
        //        licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "sale_supply").Select(lt => lt.LicenseTypeId).FirstOrDefault();
        //    }
        //    else if (business.ItemTypeId == db.ItemTypes.Where(it => it.ItemTypeKey == "health_entertainment").Select(it => it.ItemTypeId).FirstOrDefault())
        //    {
        //        licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "health_entertainment").Select(lt => lt.LicenseTypeId).FirstOrDefault();
        //    }
        //    else if (business.ItemTypeId == db.ItemTypes.Where(it => it.ItemTypeKey == "hawking").Select(it => it.ItemTypeId).FirstOrDefault())
        //    {
        //        licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "hawking").Select(lt => lt.LicenseTypeId).FirstOrDefault();
        //    }
        //    else
        //    {
        //        licenceTypeId = db.LicenseTypes.Where(lt => lt.LicenseTypeKey == "accommodation").Select(lt => lt.LicenseTypeId).FirstOrDefault();
        //    }
        //    #endregion

        //    // === NEW: Populate Dropdown Lists ===
        //    ViewBag.LicenseTypeId = new SelectList(
        //        db.LicenseTypes.Where(lt => lt.IsDeleted == false && lt.IsActive == true),
        //        "LicenseTypeId", "LicenseTypeName", licenceTypeId);   // pre-select

        //    ViewBag.ItemTypeId = new SelectList(
        //        db.ItemTypes.Where(it => it.IsDeleted == false && it.IsActive == true),
        //        "ItemTypeId", "ItemTypeName", business.ItemTypeId);   // pre-select

        //    ViewData["LicenseType"] = db.LicenseTypes.Find(licenceTypeId);
        //    ViewData["ItemType"] = db.ItemTypes.Find(business.ItemTypeId);
        //    ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName");
        //    ViewBag.ItemConditionId = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName");

        //    //ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionId == Users.Region &&  r.RegionKey != "metro_all"), "RegionId", "RegionName");
        //    ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"), "RegionId", "RegionName");
        //    if (User.IsInRole("System Admin"))
        //    {
        //        ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"), "RegionId", "RegionName");
        //    }
        //    ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName");
        //    ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber");
        //    ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
        //    ViewBag.LicenseTypeId = licenceTypeId;
        //    ViewBag.BusinessID = business.BusinessId;
        //    ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
        //    ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 1).OrderBy(c => c.StatusName), "StatusId", "StatusName");
        //    ////Department tab
        //    ViewBag.currentdate = DateTime.Now.ToShortDateString();
        //    ViewBag.DepartmentList = new SelectList(db.Departments.Where(i => i.IsDeleted == false && i.IsActive == true), "DepartmentId", "DepartmentName");
        //    var DepartmentUser = new SelectList(db.DepartmentContacts.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "DepartmentId");
        //    ViewBag.DepartmentUser = JsonConvert.SerializeObject(DepartmentUser);

        //    var Userlist = new SelectList(db.Users.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "FirstName");
        //    ViewBag.Userlist = JsonConvert.SerializeObject(Userlist);
        //    ViewBag.Decision = new SelectList(new[] { "Approve", "Reject" });

        //    ViewBag.payment = db.PaymentLicense.Where(l => l.LicenseId == 0);
        //    TempData["Success"] = null;
        //    TempData["Error"] = null;
        //    TempData["Info"] = null;






        //    //AJC 2019/12/02 Retrieves list of uploaded and outstanding documents for specific customer
        //    //Starts
        //    Business business1 = db.Businesses.Include(b => b.BusinessStatus)
        //                                 .Include(b => b.BusinessType)
        //                                 .Include(b => b.OperationStructureType)
        //                                 .Include(b => b.TitleDeedType)

        //                                 .Include(b => b.ItemType)
        //                                 .FirstOrDefault(b => b.BusinessId == id);


        //    if (business1 == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
        //                                        .Select(d => d.DocumentTypeId)
        //                                        .FirstOrDefault();

        //    var allDocs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
        //    var docs = new List<Document>();

        //    if (business1.ItemType.ItemTypeName == "Item 2")
        //    {
        //        docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType)
        //            .ToList();
        //    }
        //    else
        //    {
        //        docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false
        //            && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management).Include(d => d.DocumentType).ToList();
        //    }

        //    var clientDocs = db.FileUploads.Include(d => d.Document)
        //                                  .Where(d => d.ClientId == business1.ClientId && d.referenceId == business1.BusinessId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive)
        //                                  .ToList();

        //    var OustandingDocuments = (from d in docs
        //                               where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
        //                               select d.DocumentName
        //                                 ).ToList();

        //    ViewData["Documents"] = (from d in allDocs
        //                             where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
        //                             select d).ToList();
        //    ViewData["UploadList"] = clientDocs;
        //    ViewBag.OustandingDocuments = OustandingDocuments;

        //    //Ends






        //    return View();
        //}












        /// captures data in InspectionRequest table 
        /// 

        [HttpPost]

        public JsonResult SaveDepartments(string TableData)
        {
            char[] spearator = { '*' };


            // Using the Method 
            String[] TableDatalist = TableData.Split(spearator, StringSplitOptions.None);
            TableDatalist = TableDatalist.Where(t => t != "").ToArray();
            foreach (var item in TableDatalist)
            {
                char[] separator = { ',' };
                String[] Data = item.Split(separator, StringSplitOptions.None);
                Data = Data.Where(t => t != "").ToArray();
                var r = Data[0];
            }



            return Json("output", JsonRequestBehavior.AllowGet);
        }


        // POST: /License/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]

        public ActionResult Create([Bind(Include = "LicenseId,LicenseTypeId,LicenseTypeName,ClientId,BusinessId,ApplicationDateTime,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,RequiredDocuments,RegionId,ItemTypeId,ItemSubCategoryId,ItemConditionId")] License license, IEnumerable<HttpPostedFileBase> file, string ddlregion,
            string BusinessID, string PayinslipNumberData, string OtherItemSubCategory)// HttpPostedFileBase[] files)
        {
            Initialise();
            char[] spearator = { ',' };
            var validPayInSlip = true;
            String[] PayinslipNumberslist = PayinslipNumberData.Split(spearator, StringSplitOptions.None);

            var licenseEntry = db.Licenses.Where(lE => lE.BusinessId == license.BusinessId && lE.IsDeleted == false && lE.ItemSubCategoryId == license.ItemSubCategoryId && lE.IsActive == true).Select(l => l.BusinessId).FirstOrDefault();
            var itemCondition = db.ItemConditions.Where(i => i.ItemTypeId == license.ItemTypeId).Where(i => i.IsActive == true && i.IsDeleted == false).FirstOrDefault();

            if (PayinslipNumberData == "default" || PayinslipNumberData == null)
            {
                var LicenseId = db.Licenses.Where(lE => lE.BusinessId == license.BusinessId && lE.IsDeleted == false && lE.IsActive == true).Select(l => l.LicenseId).FirstOrDefault();
                license.LicenseId = LicenseId;
                //TempData["Error"] = "Please query a Pay in Slip Number";
            }
            else if (ModelState.IsValid)
            {

                //foreach (var value in PayinslipNumberslist)
                //{


                //    if (value == "required")
                //    {

                //        validPayInSlip = false;
                //        break;
                //    }

                //}
                if (licenseEntry == null)
                {
                    //Saves License
                    //TG20141001.

                    var client = db.Clients.Find(license.ClientId);


                    if (license.LicenseNumber == "" || license.LicenseNumber == null)
                    {
                        if (validPayInSlip == true)
                        {

                            if (license.RegionId != null)
                            {
                                RegionPrefix RP = new RegionPrefix();
                                var region = db.Regions.Where(r => r.RegionId == license.RegionId).FirstOrDefault();
                                //if (userRegion == 7)
                                //{
                                //    region.RegionName = "Central";
                                //}



                                license.LicenseNumber = RP.GetRegionPrefix(2, region.RegionName.ToString());
                            }

                            if (!string.IsNullOrEmpty(OtherItemSubCategory))
                            {
                                ItemSubCategory itemSubCategory = new ItemSubCategory()
                                {
                                    ItemSubCategoryName = OtherItemSubCategory
                                ,
                                    ItemTypeId = license.ItemTypeId,
                                    ItemSubCategoryDescription = OtherItemSubCategory,
                                    IsActive = true,
                                    IsDeleted = false,
                                    IsLocked = false,
                                };
                                db.ItemSubCategories.Add(itemSubCategory);
                                license.ItemSubCategoryId = itemSubCategory.ItemSubCategoryId;
                            }

                            license.LicenseTypeId = license.LicenseTypeId;
                            license.ItemConditionId = license.ItemConditionId;
                            license.RegionId = license.RegionId;
                            license.BusinessId = Int32.Parse(BusinessID);

                            license.ItemTypeId = license.ItemTypeId;

                            license.IsActive = true;
                            license.IsDeleted = false;
                            license.IsLocked = false;
                            license.migratedLicense = false;
                            license.ApplicationDateTime = DateTime.Now;
                            var business = db.Businesses.Where(b => b.BusinessId == license.BusinessId && b.IsDeleted == false && b.IsActive).Include(b => b.Client).Include(b => b.ItemType).FirstOrDefault();
                            //license.ItemTypeId = business.ItemTypeId;
                            var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;

                            if (license.ItemTypeId <= 0)
                            {
                                license.ItemTypeId = business.ItemTypeId;
                            }

                            var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.EmailTemplateKey == "tls_client_email_template"
                                && t.IsDeleted == false && t.IsActive);
                            var body = string.Empty;

                            if (template != null)
                            {
                                body = template.EmailBody;
                            }

                            bool inspectionsCompleteCheck = false;
                            if (inspectionsCompleteCheck)
                            {
                                var approvedLicenseStatusId = db.Status.First(d => d.StatusKey == StatusKeys.LicenseApprovedAwaitingCollection && d.IsActive == true && d.IsDeleted == false).StatusId;

                                //L.M.20150303a - Replace variables with actual email content
                                body = body.Replace("#NAME#", client.Fullname);
                                body = body.Replace("#BODYTEXT#", "Your License has been renewed and awaiting collection.");

                                var email = new tb_EmailQueue
                                {
                                    QueueDateTime = DateTime.Now,
                                    ApplicationId = applicationId,
                                    EmailAccountId = EmailAccountId,
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

                                license.StatusId = approvedLicenseStatusId;
                                license.NotificationUpdatedDateTime = DateTime.Now;
                                db.Licenses.Add(license);
                                db.SaveChanges();

                                TempData["Success"] = "License Application approved. Awaiting client collection";
                            }
                            else
                            {

                                license.StatusId =
                                db.Status.Where(s => s.StatusKey == StatusKeys.LicensePending).Select(s => s.StatusId).FirstOrDefault();
                                db.Licenses.Add(license);
                                db.SaveChanges();

                                //License Sent for inspection.
                                //TG20141013. 

                                //foreach (var dep in dept)
                                //{

                                if (client.EmailAddress != null)
                                {
                                    // TODO: Email Service
                                    #region Construct emails
                                    #region client


                                    //L.M.20150303a - Replace variables with actual email content
                                    body = body.Replace("#NAME#", client.Fullname);
                                    body = body.Replace("#BODYTEXT#", "Your license application for " + business.ProposedTradeName + " has been processed .");
                                    var email = new tb_EmailQueue
                                    {
                                        QueueDateTime = DateTime.Now,
                                        ApplicationId = applicationId,
                                        EmailAccountId = EmailAccountId,//Hard coded
                                        ToList = client.EmailAddress,
                                        CcList = null,
                                        BccList = null,
                                        Subject = "eThekwini Trade Licensing- License Registration",
                                        Body = body,
                                        IsHtml = true,
                                        FailureCount = 0,
                                        ReferenceId = client.IdentityOrPassportNumber,
                                        HasAttachments = false
                                    };

                                    core.tb_EmailQueue.Add(email);
                                    core.SaveChanges();

                                    #endregion client mail

                                    #endregion email
                                }

                                TempData["Success"] = "License Application saved, Inspections to follow.";
                            }



                            char[] separator = { '*' };
                            #region PayIn Slip Implementation
                            String[] TableDatalist = PayinslipNumberData.Split(separator, StringSplitOptions.None);
                            TableDatalist = TableDatalist.Where(t => t != "").ToArray();
                            PaymentLicense paymentlicense = new PaymentLicense();

                            foreach (var item in TableDatalist)
                            {
                                int licenseId = db.Licenses.Where(l => l.BusinessId == license.BusinessId).Select(s => s.LicenseId).FirstOrDefault();

                                var UserId = db.Users.Where(u => u.UserId == Users.UserId).FirstOrDefault();



                                char[] sep = { ',' };
                                String[] Data = item.Split(sep, StringSplitOptions.None);
                                Data = Data.Where(t => t != "" && t != " ").ToArray();

                                paymentlicense.LicenseId = licenseId;
                                paymentlicense.PAYINSLIP_NO = Data[0].ToString();
                                paymentlicense.CUST_ACCT_NO = Data[1].ToString();
                                paymentlicense.SERVICE_UNIT = Data[2].ToString();
                                paymentlicense.REQUEST_NO = Data[3].ToString();

                                if (Data[4].ToString() != "N/A" && !Data[4].Equals(TLKeys.NoProvided))
                                {
                                    paymentlicense.PAYINSLIP_AMOUNT = Convert.ToDouble(Data[4].ToString());
                                }
                                else
                                {
                                    paymentlicense.PAYINSLIP_AMOUNT = 0;
                                }
                                if (Data[5].ToString() != "N/A" && !Data[5].Equals(TLKeys.NoProvided))
                                {
                                    paymentlicense.ALLOCATED_AMOUNT = Convert.ToDouble(Data[5].ToString());
                                }

                                if (Data[6].ToString() != "N/A" && !Data[6].Equals(TLKeys.NoProvided))
                                {
                                    paymentlicense.PAID_AMOUNT = Convert.ToDouble(Data[6].ToString());
                                }
                                if (Data[7].ToString() != "N/A" && !Data[7].Equals(TLKeys.NoProvided))
                                {
                                    paymentlicense.PAID_DATE = Data[7].ToString();
                                }

                                if (Data[8].ToString() != "N/A" && !Data[8].Equals(TLKeys.NoProvided))
                                {
                                    paymentlicense.PAYINSLIP_DATE = Data[8].ToString();
                                }
                                if (Data[9].ToString() != "N/A" && !Data[9].Equals(TLKeys.NoProvided))
                                {
                                    paymentlicense.BALANCE_UNALLOCATED_AMOUNT = Convert.ToDouble(Data[9].ToString());
                                }


                                paymentlicense.CreatedDateTime = DateTime.Now.Date;

                                if (UserId != null)
                                {
                                    paymentlicense.CreatedByUserId = Int32.Parse(UserId.UserId.ToString());
                                }

                                db.PaymentLicense.Add(paymentlicense);
                                db.SaveChanges();

                            }
                            #endregion
                        }
                        else
                        {
                            var LicenseId = db.Licenses.Where(lE => lE.BusinessId == license.BusinessId && lE.IsDeleted == false && lE.IsActive == true).Select(l => l.LicenseId).FirstOrDefault();
                            license.LicenseId = LicenseId;
                            TempData["Error"] = "Please provide values for mandatory Pay in Slip fields";
                        }

                    }
                    else
                    {
                        var LicenseId = db.Licenses.Where(lE => lE.BusinessId == license.BusinessId && lE.IsDeleted == false && lE.IsActive == true).Select(l => l.LicenseId).FirstOrDefault();
                        license.LicenseId = LicenseId;
                        TempData["Error"] = "Invalid Pay in Slip Number";
                    }
                }
                else
                {
                    var LicenseId = db.Licenses.Where(lE => lE.BusinessId == license.BusinessId && lE.IsDeleted == false && lE.IsActive == true).Select(l => l.LicenseId).FirstOrDefault();
                    license.LicenseId = LicenseId;
                    TempData["Error"] = "License Exists!";
                }

            }

            //AJC 2019/12/02 Retrieves list of uploaded and outstanding documents for specific customer
            //Starts
            Business business1 = db.Businesses.Include(b => b.BusinessStatus)
                                         .Include(b => b.BusinessType)
                                         .Include(b => b.OperationStructureType)
                                         .Include(b => b.TitleDeedType)

                                         .Include(b => b.ItemType)
                                         .FirstOrDefault(b => b.BusinessId == license.BusinessId);

            ViewData["BusinessData"] = db.Businesses.Find(license.BusinessId);
            ViewData["ClientData"] = db.Clients.Find(license.ClientId);
            ViewData["LicenseType"] = db.LicenseTypes.Find(license.LicenseTypeId);
            ViewData["ItemType"] = db.ItemTypes.Find(license.ItemTypeId);
            ViewBag.BusinessID = license.BusinessId;

            ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionId == Users.Region), "RegionId", "RegionName", license.RegionId);
            if (User.IsInRole("System Admin"))
            {
                ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true), "RegionId", "RegionName", license.RegionId);
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.CreatedByUserId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName), "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);

            ViewBag.ItemConditionId = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);

            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.ModifiedByUserId);
            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 1).OrderBy(c => c.StatusName), "StatusId", "StatusName", license.StatusId);
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();
            var clientDocs = db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == license.ClientId && d.referenceId == license.BusinessId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive)
                                          .ToList();

            ViewData["UploadList"] = clientDocs;
            if (business1 == null)
            {
                return HttpNotFound();
            }



            var allDocs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            var docs = new List<Document>();

            if (business1.ItemType.ItemTypeName == "Item 2")
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType)
                    .ToList();
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false
                    && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management).Include(d => d.DocumentType).ToList();
            }



            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                                         ).ToList();

            ViewData["Documents"] = (from d in allDocs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();
            ViewData["UploadList"] = clientDocs;
            ViewBag.OustandingDocuments = OustandingDocuments;
            if (ModelState.IsValid == false)
            {
                return View(license);
            }
            //L.M - 20211116 - Disabled payin slip validation. Decision made by business
            //else if(PayinslipNumberData == "" || PayinslipNumberData == null)
            //{
            //    return View(license);
            //}
            //else if(validPayInSlip == false)
            //{
            //    return View(license);
            //}
            else
            {
                return RedirectToAction("Index");
            }

        }
        #endregion

        #region License Edit Methods

        [HttpPost]
        public ActionResult PrepopulatInspector(int? departmentID)
        {
            try
            {

                var departmentcon = db.DepartmentContacts.Where(i => i.IsDeleted == false && i.IsActive == true && i.DepartmentId == departmentID).Include(l => l.User).ToList();
                List<SelectListItem> Userlist = new List<SelectListItem>();
                if (departmentcon != null)
                {
                    foreach (var item in departmentcon)
                    {

                        Userlist.Add(new SelectListItem() { Value = item.UserId.ToString(), Text = item.User.FirstName });

                    }
                }



                return Json(Userlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var em = e.Message;
                return Json(new { success = false });
            }
        }
        // GET: /License/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Departmentcirculation(int? id)
        {
            TempData[LicenseApplicationDetails.SuccessKey] = null;
            TempData[LicenseApplicationDetails.InfoKey] = null;
            TempData[LicenseApplicationDetails.ErrorKey] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DepartmentCirculationVM departmentCirculationVM = new DepartmentCirculationVM() { DepartmentCirculationHistoryVM = new DepartmentCirculationHistoryVM() };

            License license = db.Licenses.Where(l => l.LicenseId == id)
                                .Include(l => l.Client)
                                .Include(l => l.Business)
                                .Include(l => l.Business.ItemType)
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

            Business business = license.Business;
            int businessId = Convert.ToInt32(business.BusinessId);

            if (null != business)
            {
                departmentCirculationVM.BusinessDetails = business;
                departmentCirculationVM.CustomerDetails = license.Client;
            }

            int documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument)
                                .Select(d => d.DocumentTypeId)
                                .FirstOrDefault();

            List<FileUpload> clientDocs = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == license.ClientId && d.referenceId == license.BusinessId && d.Document.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false)
                                .ToList();

            departmentCirculationVM.LicenseDetails = license;
            departmentCirculationVM.ClientUploadList = clientDocs;
            departmentCirculationVM.LicenseDetails.LicenseType = license.LicenseType;
            departmentCirculationVM.LicenseDetails.ItemType = license.ItemType;
            //licenseApplicationDetails.RegionDetails = license.Region;
            departmentCirculationVM.DepartmentCirculationHistoryVM.InspectionRequests = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
            departmentCirculationVM.DepartmentCirculationHistoryVM.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();
            //licenseApplicationDetails.BusinessMangers = db.BusinessManger.Where(l => l.BusinessId == businessId).Include(l => l.BusinessOperator).ToList();
            //licenseApplicationDetails.BusinessEmployees = db.BusinessEmployee.Where(l => l.BusinessId == businessId).ToList();

            //licenseApplicationDetails.ItemSubCategoryList = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category").OrderBy(i => i.ItemSubCategoryName), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
            //licenseApplicationDetails.RegionList = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true).OrderBy(r => r.RegionName), "RegionId", "RegionName", license.RegionId);
            //licenseApplicationDetails.ItemConditionList = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);
            //licenseApplicationDetails.BusinessList = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
            //licenseApplicationDetails.ClientList = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
            //licenseApplicationDetails.LicenseDetails.StatusId = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerResponse).Select(s => s.StatusId).FirstOrDefault();
            //licenseApplicationDetails.LicenseTypeList = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName), "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            departmentCirculationVM.PaymentLicenses = db.PaymentLicense.Where(l => l.LicenseId == license.LicenseId).ToList();
            Region centralregion = db.Regions.Where(r => r.RegionKey == TLKeys.metro_central).FirstOrDefault();


            ViewBag.DepartmentList = new SelectList(db.Departments.Where(i => i.IsDeleted == false && i.IsActive == true).OrderBy(x => x.DepartmentName), "DepartmentId", "DepartmentName");


            // licenseApplicationDetails.DepartmentDetails = db.Departments.OrderBy(x => x.DepartmentName).Where(i => i.IsDeleted == false && i.IsActive == true && i.RegionId == license.RegionId).FirstOrDefault();
            //licenseApplicationDetails.Userlist = new SelectList(db.Users.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "FirstName");

            List<Document> allDocs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            List<Document> docs = new List<Document>();

            if (business.ItemType.ItemTypeName == "Item 2")
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType)
                    .ToList();
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId)
                                   .Where(d => d.IsActive &&
                                    d.IsDeleted == false &&
                                    d.DocumentKey != TLKeys.fingerprint_Verification &&
                                    d.DocumentKey != TLKeys.Events_Management)
                                   .Include(d => d.DocumentType)
                                   .ToList();
            }

            List<Document> clientOustandingDocuments = (from d in docs
                                                        where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                        select d).ToList();

            //licenseApplicationDetails.BussinessDocuments = (from d in allDocs
            //                                                where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
            //                                                select d).ToList();

            departmentCirculationVM.ClientOustandingDocuments = clientOustandingDocuments;

            ////Ends
            int InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();

            departmentCirculationVM.DepartmentCirculationHistoryVM.InspectionRequestUploadList = db.FileUploads.Include(d => d.Document)
                                                                .Where(d => d.ClientId == license.ClientId &&
                                                                d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId &&
                                                                d.IsDeleted == false && d.IsActive).ToList();

            return View(departmentCirculationVM);
        }

        // POST: /License/Edit/5ss
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Departmentcirculation(DepartmentCirculationVM departmentCirculationVM, IEnumerable<HttpPostedFileBase> files, string DepartmentTableData, string ddlregion)
        {
            Initialise();
            License license = db.Licenses.FirstOrDefault(x => x.LicenseId == departmentCirculationVM.LicenseDetails.LicenseId);
            int awaitingManagerResponse = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.AwaitingManagerResponse)?.StatusId ?? 0;
            var townPlanningDept = db.Departments.Where(tp => tp.IsActive == true && tp.IsDeleted == false).Where(tp => tp.RegionId == 1).Where(tp => tp.DepartmentDescription.Contains("Town Planning")).FirstOrDefault();
            var allRegions = db.Regions.Where(r => r.IsActive == true && r.IsDeleted == false).Where(r => r.RegionKey.Contains("metro_all")).FirstOrDefault();

            var client = db.Clients.Find(departmentCirculationVM.CustomerDetails.ClientId);

            if (DepartmentTableData == "")
            {
                TempData["Error"] = "Please add a Department";
            }
            else
            {



                var activeinspectionrequest = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId && (l.IsActive == true || l.IsDeleted == false)).ToList();
                if (activeinspectionrequest.Count > 0)
                {
                    foreach (var item in activeinspectionrequest)
                    {
                        InspectionRequest Inspr = db.InspectionRequests.Find(item.InspectionRequestId);
                        Inspr.IsActive = false;
                        Inspr.IsDeleted = true;
                        db.Entry(Inspr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var activeinspectionresponse = db.InspectionResponse.Where(l => l.LicenseId == license.LicenseId && (l.IsActive == true || l.IsDeleted == false)).ToList();
                if (activeinspectionresponse.Count > 0)
                {
                    foreach (var item in activeinspectionresponse)
                    {
                        InspectionResponse Inspresponse = db.InspectionResponse.Find(item.InspectionResponseId);
                        Inspresponse.IsActive = false;
                        Inspresponse.IsDeleted = true;
                        db.Entry(Inspresponse).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                InspectionRequest req = new InspectionRequest();

                char[] spearator = { '*' };


                // Using the Method 
                String[] TableDatalist = DepartmentTableData.Split(spearator, StringSplitOptions.None);
                TableDatalist = TableDatalist.Where(t => t != "").ToArray();
                foreach (var item in TableDatalist)
                {
                    char[] separator = { ',' };
                    String[] Data = item.Split(separator, StringSplitOptions.None);
                    Data = Data.Where(t => t != "" && t != " ").ToArray();

                    req.LicenseId = license.LicenseId;
                    req.LicenseTypeId = license.LicenseTypeId;
                    req.RefNumber = Data[0];


                    req.DepartmentId = Int32.Parse(Data[2]);
                    req.ClientId = (int)license.ClientId;
                    //req.StatusId = Int32.Parse(Data[3]);
                    req.StatusId = awaitingManagerResponse;
                    if (Data[4] != "N/A")
                    {
                        req.Comment = Data[4];
                    }

                    req.SlaExpiryDate = DateTime.Now.AddDays(PendingInspectionSlaNumDays).Date; // LM.20150729a - Keep inspection request open.
                    req.AllocatedDate = DateTime.Now;
                    req.IsActive = true;
                    req.IsDeleted = false;
                    req.IsLocked = false;
                    db.InspectionRequests.Add(req);
                    db.SaveChanges();

                    var inspectionrequestID = db.InspectionRequests.OrderByDescending(s => s.InspectionRequestId).FirstOrDefault();
                    if (Data[5] != "N/A")
                    {
                        var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
                        var document = db.Documents.Include(d => d.DocumentType).First(d => d.DocumentTypeId == documentTypeId && d.IsActive == true && d.IsDeleted == false);
                        foreach (HttpPostedFileBase fileupload in files)
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
                                if (filename == Data[5])
                                {
                                    SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, fileupload.FileName, "Department Circulation");
                                    _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, client.ClientId
                                    , client.Fullname, document.DocumentId, inspectionrequestID.InspectionRequestId, string.Empty, Users.Username);
                                }

                            }
                        }
                    }
                }

                int ManagerRejected = db.Status.Where(s => s.StatusKey == StatusKeys.ManagerRejected).Select(s => s.StatusId)
.FirstOrDefault();

                int ChiefRejected = db.Status.Where(s => s.StatusKey == StatusKeys.ChiefRejected).Select(s => s.StatusId)
                       .FirstOrDefault();
                int AdministratorRejected = db.Status.Where(s => s.StatusKey == StatusKeys.AdministratorRejected).Select(s => s.StatusId)
                      .FirstOrDefault();

                var lic = db.Licenses.Where(l => l.LicenseId == license.LicenseId && (l.StatusId == AdministratorRejected || l.StatusId == ManagerRejected || l.StatusId == ChiefRejected));
                if (lic.Count() > 0)
                {
                    int statusId = lic.Select(s => s.StatusId).FirstOrDefault();

                    if (ManagerRejected == statusId)
                    {
                        AdministratorReview AdministratorReview = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).FirstOrDefault();
                        AdministratorReview.IsActive = false;
                        db.Entry(AdministratorReview).State = EntityState.Modified;

                        ChiefReview ChiefReview = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).FirstOrDefault();
                        ChiefReview.IsActive = false;
                        db.Entry(ChiefReview).State = EntityState.Modified;

                        ManagerReview managereview = db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).FirstOrDefault();
                        managereview.IsActive = false;
                        db.Entry(managereview).State = EntityState.Modified;

                    }

                    if (ChiefRejected == statusId)
                    {
                        AdministratorReview AdministratorReview = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).FirstOrDefault();
                        AdministratorReview.IsActive = false;
                        db.Entry(AdministratorReview).State = EntityState.Modified;

                        ChiefReview ChiefReview = db.ChiefReview.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).FirstOrDefault();
                        ChiefReview.IsActive = false;
                        db.Entry(ChiefReview).State = EntityState.Modified;

                    }

                    if (AdministratorRejected == statusId)
                    {
                        AdministratorReview AdministratorReview = db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId && l.IsActive == true).FirstOrDefault();
                        AdministratorReview.IsActive = false;
                        db.Entry(AdministratorReview).State = EntityState.Modified;

                    }
                }


                license.IsActive = true;
                license.IsDeleted = false;
                license.IsDeleted = false;
                license.StatusId =
                db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerResponse).Select(s => s.StatusId).
                    FirstOrDefault();



                var local = db.Set<License>()
                .Local.FirstOrDefault(l => l.LicenseId == license.LicenseId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }



                ViewData["BusinessData"] = db.Businesses.Find(license.BusinessId); ;
                ViewData["ClientData"] = db.Clients.Find(license.ClientId);
                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();



                //Sends out emails to client and respective departments.
                #region Construct emails
                var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                var body = string.Empty;

                if (template != null)
                {
                    body = template.EmailBody;
                }

                #region client


                //    //L.M.20150303a - Replace variables with actual email content
                if (!string.IsNullOrEmpty(client.EmailAddress))
                {
                    body = body.Replace("#NAME#", client.Fullname);
                    body = body.Replace("#BODYTEXT#", "Your application for business license has been processed and sent for inspections." + Convert.ToDateTime(license.LicenseExpiryDate).ToShortDateString());

                    var email = new tb_EmailQueue
                    {
                        QueueDateTime = DateTime.Now,
                        ApplicationId = applicationId,
                        EmailAccountId = 6,//Hard coded
                        ToList = client.EmailAddress,
                        CcList = null,
                        BccList = null,
                        Subject = "eThekwini Trade Licensing- License Registration",
                        Body = body,
                        IsHtml = true,
                        FailureCount = 0,
                        ReferenceId = client.IdentityOrPassportNumber,
                        HasAttachments = false
                    };

                    core.tb_EmailQueue.Add(email);
                    core.SaveChanges();
                }
                #endregion client mail
                #region DepartmentManager
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
                var inspection = db.InspectionRequests.Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.LicenseId == license.LicenseId).ToList();
                foreach (var dep in inspection)
                {
                    string actionLink = Url.Action("DepartmentManagerCreate", "ManagerReviews", new { id = dep.InspectionRequestId, dep.DepartmentId }, Request.Url.Scheme);

                    var department = db.Departments.Where(d => d.DepartmentId == dep.DepartmentId).Include(l => l.Region).FirstOrDefault();
                    var centralSouthRegion = db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_CentralSouth);
                    var nortWestRegion = db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_NorthWest);

                    if (department.DepartmentName.Contains("Business Licensing"))
                    {
                        var depContacts = new List<User>();
                        switch (licence.Region.RegionKey)
                        {
                            case TLKeys.metro_central:
                            case TLKeys.metro_south:
                                depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && (d.Role == RoleKeys.ChiefInspector || d.Role == RoleKeys.DepartmentClerk) && (d.Region == department.RegionId || d.Region == centralSouthRegion.RegionId)).ToList();
                                break;
                            case TLKeys.metro_north:
                            case TLKeys.meto_west:
                                depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && (d.Role == RoleKeys.ChiefInspector || d.Role == RoleKeys.DepartmentClerk) && (d.Region == department.RegionId || d.Region == nortWestRegion.RegionId)).ToList();
                                break;
                            default:
                                depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && (d.Role == RoleKeys.ChiefInspector || d.Role == RoleKeys.DepartmentClerk) && d.Region == department.RegionId).ToList();
                                break;
                        }
                        if (depContacts.Count == 0)
                        {
                            var region = db.Regions.Where(r => r.RegionKey == TLKeys.metro_all).FirstOrDefault();
                            depContacts = db.Users.Where(d => d.IsDeleted == false && d.IsActive && (d.Role == RoleKeys.ChiefInspector || d.Role == RoleKeys.DepartmentClerk) && d.Region == region.RegionId).ToList();
                        }

                        if (depContacts.Count > 0)
                        {
                            foreach (var departmentContact in depContacts)
                            {
                                body = template.EmailBody;

                                string link = AppDomain.CurrentDomain.BaseDirectory;
                                if (departmentContact != null)
                                {
                                    //L.M.20150303a - Replace variables with actual email content
                                    body = body.Replace("#NAME#", departmentContact.FullName);
                                    body = body.Replace("#BODYTEXT#", " <b>Application has been actioned for. </b><br/><br/> Business Name: " +
                                   licence.Business.ProposedTradeName + "<br/> " +
                                    " License Type: " + licence.LicenseType.LicenseTypeName + "<br/> <a href=" + actionLink + ">Click here</a>" + " to view further details.<br/> Please do not response to this email, as it is a system generated email.<br/>");

                                    var email = new tb_EmailQueue
                                    {
                                        QueueDateTime = DateTime.Now,
                                        ApplicationId = applicationId,
                                        EmailAccountId = EmailAccountId,//Hard coded
                                        ToList = departmentContact.EmailAddress,
                                        CcList = null,
                                        BccList = null,
                                        Subject = "eThekwini Trade Licensing-" + department.DepartmentName + " Inspection Request." + licence.LicenseNumber,
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

                        }
                    }
                    else
                    {
                        var depContacts = db.DepartmentContacts.Where(d => d.DepartmentId == dep.DepartmentId && d.IsDeleted == false && d.IsActive && (d.RoleName == RoleKeys.DepartmentManager || d.RoleName == RoleKeys.DepartmentClerk))
                                           .Include(u => u.User).ToList();

                        if (depContacts.Count > 0)
                        {
                            foreach (var departmentContact in depContacts)
                            {
                                body = template.EmailBody;


                                if (departmentContact != null)
                                {
                                    //L.M.20150303a - Replace variables with actual email content
                                    body = body.Replace("#NAME#", departmentContact.User.FullName);
                                    body = body.Replace("#BODYTEXT#", " <b>Application has been actioned for. </b><br/><br/> Business Name: " +
                                   licence.Business.ProposedTradeName + "<br/> " +
                                    " License Type: " + licence.LicenseType.LicenseTypeName + "<br/> " + "<a href=" + actionLink + ">Click here</a>" + " to view further details.<br/> Please do not response to this email, as it is a system generated email.<br/>");

                                    var email = new tb_EmailQueue
                                    {
                                        QueueDateTime = DateTime.Now,
                                        ApplicationId = applicationId,
                                        EmailAccountId = EmailAccountId,
                                        ToList = departmentContact.User.EmailAddress,
                                        CcList = null,
                                        BccList = null,
                                        Subject = "eThekwini Trade Licensing-" + department.DepartmentName + " Inspection Request." + licence.LicenseNumber,
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

                        }
                    }





                }
                #endregion inspectors mail
                #endregion email
                TempData["Success"] = "License Application saved, Inspections to follow.";

            }


            ViewData["LicenseType"] = db.LicenseTypes.Find(license.LicenseTypeId);
            ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
            ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true), "RegionId", "RegionName", license.RegionId);
            ViewBag.ItemConditionId = new SelectList(db.ItemConditions.Where(i => i.ItemTypeId == license.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);
            ViewData["BusinessData"] = db.Businesses.Find(license.BusinessId);
            ViewData["ClientData"] = db.Clients.Find(license.ClientId);
            ViewData["DepartmentContactdata"] = db.DepartmentContacts.Include(l => l.User).ToList();
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "ProposedTradeName", license.BusinessId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "IdentityOrPassportNumber", license.ClientId);
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.CreatedByUserId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes.Where(c => c.IsDeleted == false && c.IsActive == true).OrderBy(c => c.LicenseTypeName), "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", license.ModifiedByUserId);
            ViewBag.StatusId = new SelectList(db.Status.Where(s => s.StatusTypeId == 1).OrderBy(s => s.StatusType.StatusTypeName), "StatusId", "StatusName", license.StatusId);
            ViewData["LicenseType"] = license.LicenseType;
            ViewData["ItemType"] = license.ItemType;
            ViewData["Region"] = license.Region;
            ViewData["DepartmentRequest"] = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
            var InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                    .Select(d => d.DocumentTypeId)
                                    .FirstOrDefault();
            ViewData["inspectionUploadList"] = db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId && d.IsDeleted == false && d.IsActive).ToList();
            var docTypeId =
   db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument)
       .Select(d => d.DocumentTypeId)
       .FirstOrDefault();
            var clientDocs =
                db.FileUploads.Include(d => d.Document)
                    .Where(d => d.ClientId == license.ClientId && d.referenceId == license.BusinessId && d.Document.DocumentTypeId == docTypeId).Where(d => d.IsActive == true && d.IsDeleted == false)
                    .ToList();
            ViewData["UploadList"] = clientDocs;

            ViewData["licenceData"] = license;
            ViewBag.reference = license.LicenseId;


            ////Department tab
            ViewBag.currentdate = DateTime.Now.ToShortDateString();
            ViewBag.currentdate = DateTime.Now.ToShortDateString();
            var centralregion = db.Regions.Where(r => r.RegionKey == TLKeys.metro_central).FirstOrDefault();
            if (license.RegionId == centralregion.RegionId)
            {



                ViewBag.DepartmentList = new SelectList(db.Departments.OrderBy(x => x.DepartmentName).Where(i => i.IsDeleted == false && i.IsActive == true && i.RegionId == license.RegionId || i.DepartmentKey == DepartmentKeys.Health_North_1 || i.DepartmentKey == DepartmentKeys.Health_South_6 || i.DepartmentKey == DepartmentKeys.Health_West_7), "DepartmentId", "DepartmentName");
            }
            else
            {
                ViewBag.DepartmentList = new SelectList(db.Departments.Where(i => i.IsDeleted == false && i.IsActive == true && i.RegionId == license.RegionId), "DepartmentId", "DepartmentName");
            }


            ViewBag.Userlist = new SelectList(db.Users.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "FirstName");
            ViewBag.statusnum = db.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerResponse).Select(s => s.StatusId).FirstOrDefault();
            ViewBag.payment = db.PaymentLicense.Where(l => l.LicenseId == license.LicenseId).ToList();

            if (DepartmentTableData == "")

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

                List<Document> docs = new List<Document>();
                var business = db.Businesses.FirstOrDefault(x => x.BusinessId == licence.BusinessId);
                int documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument)
                               .Select(d => d.DocumentTypeId)
                               .FirstOrDefault();
                if (business.ItemType.ItemTypeName == "Item 2")
                {
                    docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType)
                        .ToList();
                }
                else
                {
                    docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId)
                                       .Where(d => d.IsActive &&
                                        d.IsDeleted == false &&
                                        d.DocumentKey != TLKeys.fingerprint_Verification &&
                                        d.DocumentKey != TLKeys.Events_Management)
                                       .Include(d => d.DocumentType)
                                       .ToList();
                }
                ViewData["licenceData"] = licence;
                List<Document> clientOustandingDocuments = (from d in docs
                                                            where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                            select d).ToList();

                departmentCirculationVM.ClientOustandingDocuments = clientOustandingDocuments;

                ////Ends
                InspectoionrequestdocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                                   .Select(d => d.DocumentTypeId)
                                                   .FirstOrDefault();

                departmentCirculationVM.DepartmentCirculationHistoryVM = new DepartmentCirculationHistoryVM();
                departmentCirculationVM.DepartmentCirculationHistoryVM.InspectionRequestUploadList = db.FileUploads.Include(d => d.Document)
                                                                    .Where(d => d.ClientId == license.ClientId &&
                                                                    d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId &&
                                                                    d.IsDeleted == false && d.IsActive).ToList();
                List<FileUpload> clientDocsUpload = db.FileUploads.Include(d => d.Document)
                             .Where(d => d.ClientId == license.ClientId && d.referenceId == license.BusinessId && d.Document.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false)
                             .ToList();

                departmentCirculationVM.ClientUploadList = clientDocsUpload;

                license = db.Licenses
                          .Include(l => l.Client)
                          .Include(l => l.Business)
                          .Include(l => l.Business.ItemType)
                          .Include(l => l.LicenseType)
                          .Include(l => l.Status)
                          .Include(l => l.Region)
                          .Include(l => l.ItemType)
                          .Include(l => l.ItemSubCategory)
                          .Include(l => l.ItemCondition)
                          .FirstOrDefault(x => x.LicenseId == departmentCirculationVM.LicenseDetails.LicenseId);
                departmentCirculationVM.LicenseDetails = license;
                departmentCirculationVM.PaymentLicenses = db.PaymentLicense.Where(l => l.LicenseId == license.LicenseId).ToList();
                departmentCirculationVM.DepartmentCirculationHistoryVM.InspectionRequests = db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();
                departmentCirculationVM.DepartmentCirculationHistoryVM.DepartmentContacts = db.DepartmentContacts.Include(l => l.User).ToList();

                return View(departmentCirculationVM);
            }


            return RedirectToAction("Details", new { id = license.LicenseId });
        }
        #endregion

        #region Generate License PDF Method

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult GeneratePDF(int id)
        {
            //gets license info           
            License license = db.Licenses.Where(l => l.LicenseId == id)
                .Include(l => l.LicenseType)
                .Include(l => l.ItemSubCategory)
                .Include(l => l.Client)
                .Include(l => l.ItemCondition)
                .Include(l => l.ItemType)
                .FirstOrDefault();

            ViewData["License"] = license;

            ViewData["Conditions"] = db.LicenseApplicationConditions
                .Where(c => c.IsActive == true && c.IsDeleted == false && c.LicenseId == id)
                .ToList();

            //Gets Business Info         
            ViewData["Business"] = db.Businesses
                .Where(b => b.BusinessId == license.BusinessId)
                .Include(b => b.TitleDeedType)
                .FirstOrDefault();

            ViewData["Manager"] = db.BusinessManger
                .Where(b => b.BusinessId == license.BusinessId)
                .Include(b => b.BusinessOperator)
                .ToList();

            if (license.LicenseTypeId == db.LicenseTypes
                .Where(l => l.LicenseTypeKey == "accommodation")
                .Select(l => l.LicenseTypeId)
                .FirstOrDefault())
            {
                return new Rotativa.PartialViewAsPdf("_AccomodationTemplate")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                    CustomSwitches =
          "--print-media-type " +
          "--disable-smart-shrinking " +
          "--margin-top 10 " +
          "--margin-bottom 10 " +
          "--margin-left 10 " +
          "--margin-right 10"
                };
            }
            else
            {
                return new Rotativa.PartialViewAsPdf("_ItemLicenseTemplate")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                    CustomSwitches =
          "--print-media-type " +
          "--disable-smart-shrinking " +
          "--margin-top 10 " +
          "--margin-bottom 10 " +
          "--margin-left 10 " +
          "--margin-right 10"
                };
            }
        }












        //[Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        //public ActionResult GeneratePDF(int id)
        //{
        //    //gets license info           
        //    License license = db.Licenses.Where(l => l.LicenseId == id).Include(l => l.LicenseType).Include(l => l.ItemSubCategory).Include(l => l.Client).Include(l => l.ItemCondition).Include(l => l.ItemType).FirstOrDefault();
        //    ViewData["License"] = license;

        //    ViewData["Conditions"] = db.LicenseApplicationConditions.Where(c => c.IsActive == true && c.IsDeleted == false && c.LicenseId == id).ToList();

        //    //Gets Business Info         
        //    ViewData["Business"] = db.Businesses.Where(b => b.BusinessId == license.BusinessId).Include(b => b.TitleDeedType).FirstOrDefault();
        //    ViewData["Manager"] = db.BusinessManger.Where(b => b.BusinessId == license.BusinessId).Include(b => b.BusinessOperator).ToList();
        //    if (license.LicenseTypeId == db.LicenseTypes.Where(l => l.LicenseTypeKey == "accommodation").Select(l => l.LicenseTypeId).
        //                FirstOrDefault())
        //    {
        //        return new Rotativa.PartialViewAsPdf("_AccomodationTemplate")
        //        {
        //            PageSize = Rotativa.Options.Size.A4,
        //            PageMargins = new Rotativa.Options.Margins
        //            {
        //                Top = 15,
        //                Bottom = 25,
        //                Left = 15,
        //                Right = 15
        //            }
        //        };
        //    }
        //    else
        //    {
        //        return new Rotativa.PartialViewAsPdf("_ItemLicenseTemplate")
        //        {
        //            PageSize = Rotativa.Options.Size.A4,
        //            PageMargins = new Rotativa.Options.Margins
        //            {
        //                Top = 15,
        //                Bottom = 25,
        //                Left = 15,
        //                Right = 15
        //            }
        //        };
        //    }

        //}
        #endregion

        #region Paging Method
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
                if ((User.IsInRole(RoleKeys.SystemAdmin)) || (User.IsInRole("Licensing Administrator")) || (User.IsInRole("Licensing Manager")) || (User.IsInRole("Licensing Clerk")))
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
