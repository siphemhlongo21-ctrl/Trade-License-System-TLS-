using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using PagedList;

namespace C8.TradeLicense.Controllers
{
    public class HomeController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private const int PendingInspectionSlaNumDays = 90; // Entire license application process SLA

        //
        // GET: /Home/
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
          
   
            
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

            int pendingInspection = db.Status.Where(s => s.StatusKey == "PendingLicenseInspection").Select(s => s.StatusId)
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
                .Where(l => l.StatusId == pendingInspection)
                .OrderBy(l => l.Status.StatusName).ToList();
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


            var RevokedLicenses = db.Licenses.Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseRevoked).Select(s => s.StatusId).FirstOrDefault()).ToList();
            var Approvedlicenses = db.Licenses.Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.LicenseApproved).Select(s => s.StatusId).FirstOrDefault()).ToList();
            var PendingRenewal = db.Licenses.Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.PendingRenewal).Select(s => s.StatusId).FirstOrDefault()).ToList();
            var NotRenewed = db.Licenses.Where(c => c.IsDeleted == false && c.IsActive == true).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == StatusKeys.NotRenewed).Select(s => s.StatusId).FirstOrDefault()).ToList();

            ViewBag.Revoked = RevokedLicenses.Count();
            ViewBag.Approved = Approvedlicenses.Count();
            ViewBag.Renewal = PendingRenewal.Count();
            ViewBag.NotRenewed = NotRenewed.Count();



            ViewBag.LicenseCount = licenses.Count;
            ViewBag.Licenses = licenses;
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager")]
        public ActionResult Index(int? Page_No, string StatusId, string SearchCriteria, string inputSearch, string Filter_Value, string Sorting_Order)
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            if (inputSearch != null)
            {
                Page_No = 1;
            }
            else
            {
                inputSearch = Filter_Value;

            }
            ViewBag.FilterValue = inputSearch;
            //TG2014a.   
            int pendingDocumention = db.Status.Where(s => s.StatusKey == "LicenseDocumentationPending").Select(s => s.StatusId)
                     .FirstOrDefault();
            int pendingInspection = db.Status.Where(s => s.StatusKey == "PendingLicenseInspection").Select(s => s.StatusId)
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
                .Where(l => l.StatusId == pendingInspection)
                .OrderBy(l => l.Status.StatusName).ToList();

            #region Search Filters
            if (!String.IsNullOrEmpty(StatusId))
            {
                var searchStatus = db.Status.Find(Convert.ToInt32(StatusId));
                int status = db.Status.Where(s => s.StatusId == searchStatus.StatusId).Select(s => s.StatusId)
                  .FirstOrDefault();
                licenses = licenses.Where(l => l.StatusId == status)
               .OrderBy(l => l.Status.StatusName).ToList();
            }

            if (SearchCriteria == "BusinessName")
            {
                //Search by business name
                licenses = licenses.Where(l => l.Business.ProposedTradeName.Contains(inputSearch)).ToList();
            }
            else if (SearchCriteria == "ReferenceNo")
            {
                //Search by reference number
                licenses = licenses.Where(l => l.LicenseId == Convert.ToInt32(inputSearch)).ToList();
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

    
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                Page_No = 1;
            }
            int pageSize = 5;
            int pageNumber = (Page_No ?? 1);
            var RevokedLicenses = db.Licenses.Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseDeclined").Select(s => s.StatusId).FirstOrDefault()).ToList();
            var Approvedlicenses = db.Licenses.Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseApproved").Select(s => s.StatusId).FirstOrDefault()).ToList();
            var AwaitingCollectionlicenses = db.Licenses.Where(c => c.IsDeleted == true && c.IsActive == false).Where(c => c.StatusId == db.Status.Where(s => s.StatusKey == "LicenseApprovedAwaitingCollection").Select(s => s.StatusId).FirstOrDefault()).ToList();

            ViewBag.Revoked = RevokedLicenses.Count();
            ViewBag.Approved = Approvedlicenses.Count();
            ViewBag.Collection = AwaitingCollectionlicenses.Count();

            ViewBag.LicenseCount = licenses.Count;
            //ViewData["PendingLicenseInpection"] = licensesPendingInspection.ToPagedList(pageNumber, pageSize);
            return View(licenses.ToPagedList(pageNumber, pageSize));
        }
    }
}