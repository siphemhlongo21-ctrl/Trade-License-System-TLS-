using C8.TradeLicense.DAL;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Helpers
{
    public class ManagerReviewHelpers
    {
        private readonly TradeLicenseDbContext _db;
        public ManagerReviewHelpers(TradeLicenseDbContext db)
        {
            _db = db;
        }
        public ManagerReviewHelpers()
        {
            _db = new TradeLicenseDbContext();
        }
        public LicenseApplicationDetails GetDepartmentManagerCreateVM(int? inspectionRequestId, int? DepartmentId)
        {
            try
            {
                LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
                List<DepartmentContact> inspectorList = _db.DepartmentContacts.Include(l => l.User).Where(l => l.IsDeleted == false && l.IsActive == true && l.DepartmentId == DepartmentId && l.RoleName == "Department Inspector").ToList();
                InspectionRequest inspectionRequest = _db.InspectionRequests.Where(l => l.IsDeleted == false && l.IsActive == true && l.DepartmentId == DepartmentId && l.InspectionRequestId == inspectionRequestId).Include(l => l.Department).Include(l => l.Status).FirstOrDefault();

                License license = _db.Licenses.Where(l => l.LicenseId == inspectionRequest.LicenseId)
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
                    return null;
                }

                int documentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
                List<FileUpload> UploadedDocs = _db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.referenceId == inspectionRequestId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive).ToList();

                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.InspectionRequestDetails = inspectionRequest;
                licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
                licenseApplicationDetails.ItemTypeDetails = license.ItemType;
                licenseApplicationDetails.ClientUploadList = UploadedDocs;
                licenseApplicationDetails.RegionDetails = license.Region;

                List<SelectListItem> inspectorlist = new List<SelectListItem>();
                foreach (var item in inspectorList)
                {
                    inspectorlist.Add(new SelectListItem() { Value = item.DepartmentContactId.ToString(), Text = item.User.FullName });
                }

                SelectList inspectorslist = new SelectList(inspectorlist, "Value", "Text");
                licenseApplicationDetails.InspectorList = inspectorslist;
                return licenseApplicationDetails;
            }
            catch
            {
                return null;
            }
        }
        public LicenseApplicationDetails GetViewDetailsVM(int inspectionId)
        {
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            InspectionResponse InspectionResponse = _db.InspectionResponse.Where(l => l.InspectionRequestId == inspectionId)
           .Include(l => l.License)
           .Include(l => l.License.Client)
           .Include(l => l.License.Business)
           .Include(l => l.License.LicenseType)
           .Include(l => l.Status)
           .Include(l => l.License.Region)
           .Include(l => l.License.ItemType)
           .Include(l => l.License.ItemSubCategory)
           .Include(l => l.License.ItemCondition)
           .Include(l => l.InspectionRequest)
           .Include(l => l.InspectionRequest.Department)
           .FirstOrDefault();

            if (InspectionResponse == null)
            {
                return null;
            }
            licenseApplicationDetails.InspectionAppealsReviews = _db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == InspectionResponse.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.DepartmentContacts = _db.DepartmentContacts.Include(l => l.User).ToList();
            licenseApplicationDetails.AdministratorReviews = _db.AdministratorReviews.Where(l => l.LicenseId == InspectionResponse.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.ChiefReviews = _db.ChiefReview.Where(l => l.LicenseId == InspectionResponse.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.ManagerReviews = _db.ManagerRevies.Where(l => l.LicenseId == InspectionResponse.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.LicenseDetails = InspectionResponse.License;
            licenseApplicationDetails.BusinessDetails = InspectionResponse.License.Business;
            licenseApplicationDetails.CustomerDetails = InspectionResponse.License.Client;
            licenseApplicationDetails.LicenseTypeDetails = InspectionResponse.License.LicenseType;
            licenseApplicationDetails.ItemTypeDetails = InspectionResponse.License.ItemType;
            licenseApplicationDetails.RegionDetails = InspectionResponse.License.Region;
            licenseApplicationDetails.InspectionResponses = _db.InspectionResponse.Where(l => l.LicenseId == InspectionResponse.License.LicenseId && l.IsActive == true)
                .Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Status).Include(l => l.InspectionRequest.Department).ToList();
          return licenseApplicationDetails;
        }
        public void AssignInspector(int? inspectionRequestId,int departmentContact)
        {
            int NoInspectionResponseRefusalDays = _db.NumOfDays.FirstOrDefault(x => x.Name == 
                NumberOfDaysKeys.NoInspectionResponseRefusalDays && x.IsActive)?.NumberOfDays??0;
            var InspectionRequest = _db.InspectionRequests.FirstOrDefault(l => l.IsDeleted == false && l.IsActive == true && l.InspectionRequestId == inspectionRequestId);
            if (InspectionRequest != null)
            {
                InspectionRequest.AllocatedDate = DateTime.Now;
                AddDaysToDate AD = new AddDaysToDate();
                InspectionRequest.InspectionFinalDate = DateTime.Parse(AD.AddDays(DateTime.Now.Date, NoInspectionResponseRefusalDays));
                InspectionRequest.DepartmentContactId = departmentContact;
                InspectionRequest.StatusId =
                _db.Status.Where(s => s.StatusKey == StatusKeys.PendingLicenseInspection).Select(s => s.StatusId).
                FirstOrDefault();
                _db.Entry(InspectionRequest).State = EntityState.Modified;
                _db.SaveChanges();
            }            
        }
    }
}