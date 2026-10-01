using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Helpers
{
    public class AdministratorReviewHelpers
    {
        private readonly TradeLicenseDbContext _db;
        public AdministratorReviewHelpers(TradeLicenseDbContext db)
        {
            _db = db;
        }
        public AdministratorReviewHelpers()
        {
            _db = new TradeLicenseDbContext();
        }

        public LicenseApplicationDetails GetAdministratorCreateVM(int licenseId, User UserId)
        {
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            License license = _db.Licenses.Where(l => l.LicenseId == licenseId)
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


            licenseApplicationDetails.InspectionAppealsReviews = _db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.InspectionAppealsReviewsDetails = _db.InspectionAppealsReviews.Include(l => l.InspectionAppeal).Where(l => l.InspectionAppeal.LicenseId == license.LicenseId).Include(l => l.User).FirstOrDefault();

            licenseApplicationDetails.AdministratorReviews = _db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.AdministratorReviewDetails = _db.AdministratorReviews.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).OrderByDescending(o => o.AdministratorReviewId).FirstOrDefault();
            licenseApplicationDetails.ChiefReviews = _db.ChiefReview.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();
            licenseApplicationDetails.ManagerReviews = _db.ManagerRevies.Where(l => l.LicenseId == license.LicenseId).Include(l => l.User).ToList();

            licenseApplicationDetails.UserDetails = UserId;
            licenseApplicationDetails.BusinessDetails = license.Business;
            licenseApplicationDetails.CustomerDetails = license.Client;
            licenseApplicationDetails.LicenseDetails = license;
            licenseApplicationDetails.InspectionRequests = _db.InspectionRequests.Where(l => l.LicenseId == license.LicenseId).Include(i => i.Department).Include(i => i.Status).ToList();

            licenseApplicationDetails.InspectionResponses = _db.InspectionResponse.Where(l => l.LicenseId == licenseId).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
            licenseApplicationDetails.DepartmentContacts = _db.DepartmentContacts.Include(l => l.User).ToList();
            licenseApplicationDetails.LicenseTypeDetails = license.LicenseType;
            licenseApplicationDetails.ItemTypeDetails = license.ItemType;
            licenseApplicationDetails.RegionDetails = license.Region;
            licenseApplicationDetails.DepartmentContacts = _db.DepartmentContacts.Include(l => l.User).ToList();

            int? Inspectordocument = _db.DocumentTypes?
                                    .Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                    .Select(d => d.DocumentTypeId)
                                    .FirstOrDefault();

            List<FileUpload> InspectorDocs = _db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();

            licenseApplicationDetails.InspectionRequestDocuments = InspectorDocs;
            licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;
            int InspectoionrequestdocumentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                                   .Select(d => d.DocumentTypeId)
                                                   .FirstOrDefault();
            licenseApplicationDetails.InspectionRequestUploadList = _db.FileUploads.Include(d => d.Document)
                                                                    .Where(d => d.ClientId == license.ClientId &&
                                                                    d.Document.DocumentTypeId == InspectoionrequestdocumentTypeId &&
                                                                    d.IsDeleted == false && d.IsActive).ToList();
            licenseApplicationDetails.DepartmentCirculationHistoryVM = new ViewModels.DepartmentCirculationHistoryVM
            {
                DepartmentContacts = licenseApplicationDetails.DepartmentContacts,
                InspectionRequests = licenseApplicationDetails.InspectionRequests,
                InspectionRequestUploadList = licenseApplicationDetails.InspectionRequestUploadList
            };
            licenseApplicationDetails.AmendmentList = _db.Amendments.Include(x => x.CreatedByUser).Where(x => x.IsActive && x.LicenseId == license.LicenseId).ToList();
            return licenseApplicationDetails;
        }
    }
}