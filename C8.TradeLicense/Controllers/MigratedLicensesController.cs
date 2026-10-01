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
using System.Web.Script.Serialization;
using C8.TradeLicense.Keys;
using System.Net.Http;
using System.Text;
using System.Configuration;
using Newtonsoft.Json.Linq;
using PagedList;
using C8.TradeLicense.Helpers;

namespace C8.TradeLicense.Controllers
{
    public class MigratedLicensesController : Controller
    {
        private TradeLicenseDbContext db;
        private MigratedLicenseHelpers _migratedLicenseHelpers;
        private FileHelpers _fileHelpers;
        private MigratedLicenseViewModels tbs;
        private CesarDbContext core;
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

        // GET: MigratedLicenses
        public IdentityManager IdentityManager { get; set; }


        /// <summary>
        /// Gets or sets the SystemUser.
        /// </summary>
        public User Users { get; set; }


        public int UserId { get; set; }
        public MigratedLicensesController()
        {
            db = new TradeLicenseDbContext();
            _migratedLicenseHelpers = new MigratedLicenseHelpers(db);
            tbs = new MigratedLicenseViewModels();
            core = new CesarDbContext();
        }
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

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + ","
           + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Department Manager")]
        public ActionResult LicensePending(ListMigratedLicensesVM listMigratedLicensesVM)
        {

            Initialise();

            bool isAdmin = User.IsInRole(RoleKeys.SystemAdmin);
            int pendinglicense = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.LicensePending)?.StatusId ?? 0;
            int region = db.Regions.FirstOrDefault(x => x.RegionKey == TLKeys.metro_all)?.RegionId ?? 0;

            List<MigratedLicense> licenses = db.MigratedLicense.Include(l => l.MigratedBusiness)
               .Include(l => l.MigratedClient)
                    .Include(l => l.Region)
                    .Include(l => l.CreatedByUser)
                    .Include(l => l.LicenseType)
                    .Include(l => l.ModifiedByUser)
                    .Include(l => l.Status)
                    .Include(l => l.ItemType)
                    .Include(l => l.ItemSubCategory)
                    .Where(l => l.IsDeleted == false && l.IsActive == true)
                    .Where(l => (isAdmin || region == Users.Region || l.RegionId == Users.Region)
                    && (listMigratedLicensesVM.SelectedSearch != "ClientName" || (l.MigratedClient.Name.ToLower() + " " + l.MigratedClient.Surname.ToLower()).Contains(listMigratedLicensesVM.InputSearch.ToLower()))
                    && (listMigratedLicensesVM.SelectedSearch != "LicenseNumber" || l.LicenseNumber == listMigratedLicensesVM.InputSearch)
                    && (listMigratedLicensesVM.SelectedSearch != "BusinessName" || l.MigratedBusiness.ProposedTradeName.ToLower().Contains(listMigratedLicensesVM.InputSearch.ToLower()))
                    && (listMigratedLicensesVM.SelectedSearch != "Region" || l.Region.RegionName.ToLower().Contains(listMigratedLicensesVM.InputSearch.ToLower()))
                    && l.StatusId == pendinglicense)
                    .OrderBy(l => l.Status.StatusName).ToList();
            int pageSize = 5;
            int pageNumber = (listMigratedLicensesVM.Page ?? 1);

            listMigratedLicensesVM.LicenseCount = licenses.Count();
            listMigratedLicensesVM.MigratedLicenses = licenses.ToPagedList(pageNumber, pageSize);
            return View(listMigratedLicensesVM);
        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + ","
          + "Department Inspector" + "," + "Chief Inspector" + "," + "System Admin" + "," + "Department Manager")]
        public ActionResult LicenseApproved(ListMigratedLicensesVM listMigratedLicensesVM)
        {

            Initialise();


            bool isAdmin = User.IsInRole(RoleKeys.SystemAdmin);
            int licenseApproved = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.LicenseApproved)?.StatusId ?? 0;
            int region = db.Regions.FirstOrDefault(x => x.RegionKey == TLKeys.metro_all)?.RegionId ?? 0;

            List<MigratedLicense> licenses = db.MigratedLicense.Include(l => l.MigratedBusiness)
               .Include(l => l.MigratedClient)
                    .Include(l => l.Region)
                    .Include(l => l.CreatedByUser)
                    .Include(l => l.LicenseType)
                    .Include(l => l.ModifiedByUser)
                    .Include(l => l.Status)
                    .Include(l => l.ItemType)
                    .Include(l => l.ItemSubCategory)    
                    .Where(l => l.IsDeleted == false && l.IsActive == true)
                    .Where(l => (isAdmin || l.RegionId == Users.Region)
                    && (listMigratedLicensesVM.SelectedSearch != "ClientName" || (l.MigratedClient.Name.ToLower() + " " + l.MigratedClient.Surname.ToLower()).Contains(listMigratedLicensesVM.InputSearch.ToLower()))
                    && (listMigratedLicensesVM.SelectedSearch != "LicenseNumber" || l.LicenseNumber == listMigratedLicensesVM.InputSearch)
                    && (listMigratedLicensesVM.SelectedSearch != "BusinessName" || l.MigratedBusiness.ProposedTradeName.ToLower().Contains(listMigratedLicensesVM.InputSearch.ToLower()))
                    && (listMigratedLicensesVM.SelectedSearch != "Region" || l.Region.RegionName.ToLower().Contains(listMigratedLicensesVM.InputSearch.ToLower()))
                    && l.StatusId == licenseApproved)
                    .OrderBy(l => l.Status.StatusName).ToList();
            int pageSize = 5;
            int pageNumber = (listMigratedLicensesVM.Page ?? 1);

            listMigratedLicensesVM.LicenseCount = licenses.Count();
            listMigratedLicensesVM.MigratedLicenses = licenses.ToPagedList(pageNumber, pageSize);
            return View(listMigratedLicensesVM);
        }


        // GET: MigratedLicenses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MigratedLicense migratedLicense = db.MigratedLicense.Find(id);
            if (migratedLicense == null)
            {
                return HttpNotFound();
            }
            return View(migratedLicense);
        }

        // GET: MigratedLicenses/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName");
            ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories, "ItemSubCategoryId", "ItemSubCategoryName");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName");
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName");
            ViewBag.MBusinessId = new SelectList(db.MigratedBusiness, "MBusinessId", "ProposedTradeName");
            ViewBag.MClientId = new SelectList(db.MigratedClient, "MClientId", "IdentityOrPassportNumber");
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "RegionName");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName");
            return View();
        }

        // POST: MigratedLicenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MLicenseId,LicenseTypeId,MClientId,ItemTypeId,ItemSubCategoryId,MBusinessId,LicenseNumber,RegionId,LicenseIssueYear,ApplicationDateTime,LicenseIssueDateTime,NotificationUpdatedDateTime,LicenseExpiryDate,StatusId,LicenseClosureDateTime,LicenseCollectedDateTime,LicenseRenewalDateTime,ItemConditionId,ConditionReason,ConditionComment,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime")] MigratedLicense migratedLicense)
        {
            if (ModelState.IsValid)
            {
                db.MigratedLicense.Add(migratedLicense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", migratedLicense.CreatedByUserId);
            ViewBag.ItemConditionId = new SelectList(db.ItemConditions, "ItemConditionId", "ItemConditionName", migratedLicense.ItemConditionId);
            ViewBag.ItemSubCategoryId = new SelectList(db.ItemSubCategories, "ItemSubCategoryId", "ItemSubCategoryName", migratedLicense.ItemSubCategoryId);
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "ItemTypeId", "ItemTypeName", migratedLicense.ItemTypeId);
            ViewBag.LicenseTypeId = new SelectList(db.LicenseTypes, "LicenseTypeId", "LicenseTypeName", migratedLicense.LicenseTypeId);
            ViewBag.MBusinessId = new SelectList(db.MigratedBusiness, "MBusinessId", "ProposedTradeName", migratedLicense.MBusinessId);
            ViewBag.MClientId = new SelectList(db.MigratedClient, "MClientId", "IdentityOrPassportNumber", migratedLicense.MClientId);
            ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", migratedLicense.ModifiedByUserId);
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "RegionName", migratedLicense.Region);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "StatusName", migratedLicense.StatusId);
            return View(migratedLicense);
        }

        // GET: MigratedLicenses/Edit/5
        public ActionResult Edit(int? id)
        {
            Initialise();
            return View(_migratedLicenseHelpers.GetEditLicenseVM(id.Value, User.IsInRole(RoleKeys.SystemAdmin), Users.Region, string.Empty, string.Empty));
        }

        // POST: MigratedLicenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LicenseApplicationDetails licenseApplicationDetails, IEnumerable<HttpPostedFileBase> files,
            string CustomerDetailsisSame,string BusinessDetailsisSame, string OperatorTableData,
             string EmployeeTableData, BusinessEmployee BusinessEmployee
            ,string clientComment,string BussinessComment)
        {
            Initialise();
            
            string filesStatus = _migratedLicenseHelpers.CheckFilesUploaded(files);
            
            if (filesStatus != ViewSettingsKeys.Success)
            {
                return View(_migratedLicenseHelpers.GetEditLicenseVM(licenseApplicationDetails.MigratedLicense.MLicenseId
                    , User.IsInRole(RoleKeys.SystemAdmin), Users.Region, filesStatus, ViewSettingsKeys.Error));
            }
            if (OperatorTableData != null)
            {
                var approvedstatus = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.LicenseApproved)?.StatusId ?? 0;
                _migratedLicenseHelpers.ProcessMigrated(licenseApplicationDetails, files,
                    CustomerDetailsisSame, BusinessDetailsisSame, OperatorTableData,
                    EmployeeTableData, BusinessEmployee,clientComment, BussinessComment
                    ,Users.UserId, Users.Username, approvedstatus);

                if (licenseApplicationDetails.MigratedLicense.StatusId == approvedstatus)
                {
                    return View("LicenseApproved",new ListMigratedLicensesVM { Message = "Saved Successfully!" , MessageType = ViewSettingsKeys.Success});
                }
                else
                {
                    return View("LicensePending", new ListMigratedLicensesVM { Message = "Saved Successfully!", MessageType = ViewSettingsKeys.Success });
                }

            }
            return View(_migratedLicenseHelpers.GetEditLicenseVM(licenseApplicationDetails.MigratedLicense.MLicenseId
               ,User.IsInRole(RoleKeys.SystemAdmin), Users.Region, "Something went wrong.", ViewSettingsKeys.Error));
        }

        // GET: MigratedLicenses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MigratedLicense migratedLicense = db.MigratedLicense.Find(id);
            if (migratedLicense == null)
            {
                return HttpNotFound();
            }
            return View(migratedLicense);
        }

        // POST: MigratedLicenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MigratedLicense migratedLicense = db.MigratedLicense.Find(id);
            db.MigratedLicense.Remove(migratedLicense);
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
