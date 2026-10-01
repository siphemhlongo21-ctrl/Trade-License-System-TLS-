using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Helpers
{
    public class RefusalHelpers
    {
        private readonly TradeLicenseDbContext _db;
        public RefusalHelpers(TradeLicenseDbContext db)
        {
            _db = db;
        }

        public LicenseApplicationDetails GetRefusal(int ispectionRequestId){
             LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
        InspectionResponse InspectionResponses = _db.InspectionResponse.Where(l => l.InspectionRequestId == ispectionRequestId)
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

        DepartmentContact DepartmentContact = _db.DepartmentContacts.Where(d => d.DepartmentContactId == InspectionResponses.InspectionRequest.DepartmentContactId).Include(l => l.User).FirstOrDefault();

            if (InspectionResponses == null)
            {
                return null;
            }
    int Requestdocument = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                 .Select(d => d.DocumentTypeId)
                                 .FirstOrDefault();
    List<FileUpload> UploadedDocs = _db.FileUploads.Include(d => d.Document)
                        .Where(d => d.ClientId == InspectionResponses.License.ClientId && d.referenceId == ispectionRequestId && d.Document.DocumentTypeId == Requestdocument && d.IsDeleted == false && d.IsActive).ToList();
    int documentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Refusaldocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
    IEnumerable<InspectionHistory> InspectionHistory = _db.InspectionHistory.Where(l => l.LicenseId == InspectionResponses.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionResponses.InspectionRequest.DepartmentId).ToList();


    licenseApplicationDetails.LicenseDetails = InspectionResponses.License;
            licenseApplicationDetails.InspectionRequestDetails = InspectionResponses.InspectionRequest;
            licenseApplicationDetails.InspectionResponseDetails = InspectionResponses;
        
            licenseApplicationDetails.BussinessDocuments = _db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType);
    licenseApplicationDetails.InspectionHistorys = InspectionHistory;
            licenseApplicationDetails.BusinessDetails = InspectionResponses.License.Business;
            licenseApplicationDetails.CustomerDetails = InspectionResponses.License.Client;

           licenseApplicationDetails.DepartmentContactDetails = DepartmentContact;
           licenseApplicationDetails.ClientUploadList = UploadedDocs;
           licenseApplicationDetails.InspectionRequestUploadList = UploadedDocs;
            licenseApplicationDetails.DepartmentContacts = _db.DepartmentContacts.Include(l => l.User).ToList();

    int Inspectordocument = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                              .Select(d => d.DocumentTypeId)
                                              .FirstOrDefault();
    List<FileUpload> InspectorDocs = _db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == InspectionResponses.License.Client.ClientId &&
                                    d.Document.DocumentTypeId == Inspectordocument &&
                                    d.IsDeleted == false &&
                                    d.IsActive).ToList();

    licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;
            licenseApplicationDetails.InspectionRequestDocuments = InspectorDocs;

            licenseApplicationDetails.InspectionResponseUploadList = InspectorDocs;
            return licenseApplicationDetails;
        }
        public LicenseApplicationDetails GetRefusal(int ispectionRequestId, int refusalId)
        {
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
            InspectionResponse InspectionResponses = _db.InspectionResponse.Where(l => l.InspectionRequestId == ispectionRequestId)
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

            DepartmentContact DepartmentContact = _db.DepartmentContacts.Where(d => d.DepartmentContactId == InspectionResponses.InspectionRequest.DepartmentContactId).Include(l => l.User).FirstOrDefault();

            if (InspectionResponses == null)
            {
                return null;
            }
            int Requestdocument = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.InspectionRequestdocument && dt.IsDeleted == false && dt.IsActive)
                                         .Select(d => d.DocumentTypeId)
                                         .FirstOrDefault();
            List<FileUpload> UploadedDocs = _db.FileUploads.Include(d => d.Document)
                                .Where(d => d.ClientId == InspectionResponses.License.ClientId && d.referenceId == ispectionRequestId && d.Document.DocumentTypeId == Requestdocument && d.IsDeleted == false && d.IsActive).ToList();
            int documentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Refusaldocument && dt.IsActive == true && dt.IsDeleted == false).Select(d => d.DocumentTypeId).FirstOrDefault();
            IEnumerable<InspectionHistory> InspectionHistory = _db.InspectionHistory.Where(l => l.LicenseId == InspectionResponses.License.LicenseId).Include(l => l.Status).Include(l => l.InspectionRequest).Where(r => r.InspectionRequest.DepartmentId == InspectionResponses.InspectionRequest.DepartmentId).ToList();


            licenseApplicationDetails.LicenseDetails = InspectionResponses.License;
            licenseApplicationDetails.InspectionRequestDetails = InspectionResponses.InspectionRequest;
            licenseApplicationDetails.InspectionResponseDetails = InspectionResponses;

            licenseApplicationDetails.BussinessDocuments = _db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType);
            licenseApplicationDetails.InspectionHistorys = InspectionHistory;
            licenseApplicationDetails.BusinessDetails = InspectionResponses.License.Business;
            licenseApplicationDetails.CustomerDetails = InspectionResponses.License.Client;

            licenseApplicationDetails.DepartmentContactDetails = DepartmentContact;
            licenseApplicationDetails.ClientUploadList = UploadedDocs;
            licenseApplicationDetails.InspectionRequestUploadList = UploadedDocs;
            licenseApplicationDetails.DepartmentContacts = _db.DepartmentContacts.Include(l => l.User).ToList();
            licenseApplicationDetails.RefusalDetails = _db.Refusa.FirstOrDefault(x=>x.RefusalId == refusalId);
            licenseApplicationDetails.RefusalDocuments = _db.FileUploads.Where(f => f.ClientId == InspectionResponses.License.ClientId 
            && f.referenceId == refusalId && f.IsActive == true && f.IsDeleted == false)
                .Include(f => f.Document).Include(f => f.CreatedByUser).ToList();
            int Inspectordocument = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                                      .Select(d => d.DocumentTypeId)
                                                      .FirstOrDefault();
            List<FileUpload> InspectorDocs = _db.FileUploads.Include(d => d.Document)
                                            .Where(d => d.ClientId == InspectionResponses.License.Client.ClientId &&
                                            d.Document.DocumentTypeId == Inspectordocument &&
                                            d.IsDeleted == false &&
                                            d.IsActive).ToList();

            licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;
            licenseApplicationDetails.InspectionRequestDocuments = InspectorDocs;

            licenseApplicationDetails.InspectionResponseUploadList = InspectorDocs;
            return licenseApplicationDetails;
        }
    }
}