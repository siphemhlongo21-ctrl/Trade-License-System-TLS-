using C8.TradeLicense.Models.Interfaces;
using C8.TradeLicense.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Models
{
    public class LicenseApplicationDetails:IUIMessage
    {
        public License LicenseDetails { get; set; }
        public Business BusinessDetails { get; set; }
        public Client CustomerDetails { get; set; }
        public Amendment AmendmentDetails { get; set; }
        public Region RegionDetails { get; set; }
        public BusinessManger BusinessManagerDetails { get; set; }
        public Condition ConditionDetails { get; set; }
        public LicenseType LicenseTypeDetails { get; set; }
        public ItemType ItemTypeDetails { get; set; }
        public LicensesRevoked LicensesRevokedDetails { get; set; }
        public InspectionRequest InspectionRequestDetails { get; set; }
        public InspectionResponse InspectionResponseDetails { get; set; }
        public ChiefReview ChiefReviewDetails { get; set; }
        public ManagerReview ManagerReviewDetails { get; set; }
        public AdministratorReview AdministratorReviewDetails { get; set; }
        public User UserDetails { get; set; }
        public Refusal RefusalDetails { get; set; }
        public RevokeCancelReview RevokeCancelReviewDetails { get; set; }


        public InspectionAppeal InspectionAppealDetails { get; set; }
        public InspectionAppealsReviews InspectionAppealsReviewsDetails { get; set; }
        public DepartmentContact DepartmentContactDetails { get; set; }
        public Department DepartmentDetails { get; set; }

        public MigratedBusiness MigratedBusiness { get; set; }
        public MigratedLicense MigratedLicense { get; set; }
        public MigratedClient MigratedClient { get; set; }


        public SelectList CustomerTypeList { get; set; }
        public SelectList NationalityList { get; set; }
        public SelectList IndividualTypeList { get; set; }
        public SelectList LicenseTypeList { get; set; }
        public SelectList ItemTypeList { get; set; }
        public SelectList ItemSubCategoryList { get; set; }
        public SelectList ItemConditionList { get; set; }
        public SelectList BusinessList { get; set; }
        public SelectList ClientList { get; set; }
        public SelectList TitleDeedTypeList { get; set; }
        public SelectList BusinessTypeList { get; set; }
        public SelectList OperationStructureTypeList { get; set; }
        public SelectList BusinessOperatorList { get; set; }
        public SelectList LicenseConditionList { get; set; }
        public SelectList InspectorList { get; set; }
        public SelectList DepartmentList { get; set; }
        public SelectList Userlist { get; set; }
        public SelectList RegionList { get; set; }

        public SelectList StatusList { get; set; }

        public IEnumerable<Amendment> AmendmentList { get; set; }
        public IEnumerable<Business> ClientBusinessesList { get; set; }
        public IEnumerable<BusinessManger> BusinessMangers { get; set; }
        public IEnumerable<BusinessEmployee> BusinessEmployees { get; set; }
        public IEnumerable<LicenseApplicationConditions> LicenseApplicationConditions { get; set; }
        public IEnumerable<InspectionResponse> InspectionResponses { get; set; }
        public IEnumerable<FileUpload> BussinessUploadList { get; set; }
        public IEnumerable<FileUpload> InspectionResponseUploadList { get; set; }
        public IEnumerable<FileUpload> InspectionRequestUploadList { get; set; }
        public IEnumerable<Document> BussinessOustandingDocuments { get; set; }
        public IEnumerable<Document> BussinessDocuments { get; set; }

        public IEnumerable<Document> ClientDocuments { get; set; }
        public IEnumerable<Document> ClientOustandingDocuments { get; set; }
        public IEnumerable<FileUpload> ClientUploadList { get; set; }
        public IEnumerable<FileUpload> InspectionResponseDocuments { get; set; }
        public IEnumerable<FileUpload> InspectionRequestDocuments { get; set; }
        public IEnumerable<FileUpload> RefusalDocuments { get; set; }
        public IEnumerable<LicensesRevoked> LicensesRevokedList { get; set; }
        public IEnumerable<ChiefReview> ChiefReviews { get; set; }
        public IEnumerable<ManagerReview> ManagerReviews { get; set; }
        public IEnumerable<InspectionRequest> InspectionRequests { get; set; }
        public IEnumerable<Refusal> Refusals { get; set; }
        public IEnumerable< RevokeCancelReview> RevokeCancelReviews { get; set; }
        public IEnumerable<InspectionHistory> InspectionHistorys { get; set; }
        public IEnumerable<AdministratorReview> AdministratorReviews { get; set; }
        public IEnumerable<InspectionAppeal> InspectionAppeals { get; set; }
        public IEnumerable<InspectionAppealsReviews> InspectionAppealsReviews { get; set; }
        public IEnumerable<DepartmentContact> DepartmentContacts { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<PaymentLicense> PaymentLicenses { get; set; }
        public IPagedList<License> DeletedLicensePageList { get; set; }

        public IEnumerable<BusinessOperator> BusinessOperators { get; set; }
        public DepartmentCirculationHistoryVM DepartmentCirculationHistoryVM { get; set; }
        public string PayinslipTableData { get; set; }
        public string Screen { get; set; }
        public int DeletedLicenseCount { get; set; }
        public string CurrentFilter { get; set; }
        public string Exits  { get; set; }
        public string SelectedSearch { get; set; }
        public bool IsPrincipal { get; set; }
        public string PayinslipTblData { get; set; }
        public IPagedList<InspectionRequest> IPagedInspection { get; set; }

        public const string SuccessKey = "Success";
        public const string InfoKey = "Info";
        public const string ErrorKey = "Error";
        public const string NoRecordKey = "norecord";
        public string Message { get; set; }
        public string MessageType { get; set; }

    }
}
