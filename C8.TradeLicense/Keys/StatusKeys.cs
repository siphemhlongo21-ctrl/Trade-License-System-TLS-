using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C8.TradeLicense.Keys
{
    public class StatusKeys
    {
        //Trade License
        public const string LicenseApproved = "LicenseApproved";
   
        public const string LicenseDocumentationPending = "LicenseDocumentationPending";
        public const string LicenseExpired = "LicenseExpired";


        public const string LicenseApplicationTerminated = "LicenseApplicationTerminated";
        public const string LicenseApprovedAwaitingCollection = "LicenseApprovedAwaitingCollection";
        public const string LicenseDeclined = "LicenseDeclined";
        public const string LicenseRevoked = "LicenseRevoked";
        public const string LicenceApplicationCancelled = "LicenceApplicationCancelled";


        public const string AwaitingManagerReview = "AwaitingManagerReview";
        public const string RevokeCancelAwaitingManagerReview = "RevokeCancelAwaitingManagerReview";
        public const string AwaitingManagerResponse = "AwaitingManagerResponse";
        public const string ManagerRejected = "ManagerRejected";


        public const string AwaitingAdministratorReview = "AwaitingAdministratorReview";
        public const string AdministratorRejected = "AdministratorRejected";
        public const string AwaitingChiefReview = "AwaitingChiefReview";
        public const string ChiefRejected = "ChiefRejected";


        public const string AppealReview = "AppealReview";
        public const string InspectionAppealed = "InspectionAppealed";
        public const string InspectionApproved = "InspectionApproved";
        public const string ResponsePending = "ResponsePending";
        public const string LicenseAwaitingRecommendation = "LicenseAwaitingRecommendation";
        public const string PendingLicenseInspection = "PendingLicenseInspection";
        public const string AppealApproved = "AppealApproved";
        public const string AppealRejected = "AppealRejected";
        public const string RefusalInProgress = "RefusalInProgress";
        public const string InspectionPending = "Inspection Pending";
        public const string InspectionOverdue = "InspectionOverdue";

        public const string PendingBusinessClearance = "PendingBusinessClearance";
        public const string BusinessApproved = "BusinessApproved";
        public const string PendingClientClearance = "PendingClientClearance";


        public const string ClientApproved = "ClientApproved";

        public const string AwaitingPayment = "AwaitingPayment";
        public const string LicensePending = "LicensePending";
        public const string PendingRefusal = "PendingRefusal";
        public const string InspectionFailed = "InspectionFailed";
      

        public const string AwaitingRefusalReport = "AwaitingRefusalReport";
        public const string RefusalReportUploaded = "RefusalReportUploaded";
        public const string SignedRefusalReportUploaded = "SignedRefusalReportUploaded";
        public const string LicenseCanceled = "LicenseCanceled";
        public const string AppealAbandoned = "AppealAbandoned";

  

        public const string LicenseRenewal = "LicenseRenewal";
        public const string PendingRenewal = "PendingRenewal";
        public const string NotRenewed = "NotRenewed";
        public const string LicenseDeleted = "LicenseDeleted";

        public const string LicenseSuspended = "LicenseSuspended";
        public const string LicenseAmendmentRequested = "LicenseAmendmentRequested"; 
        public const string LicenseAmendmentRejected = "LicenseAmendmentRejected";
        public const string LicenseAmendmentApproved = "LicenseAmendmentApproved";
    }
}