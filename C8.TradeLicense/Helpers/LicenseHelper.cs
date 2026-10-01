using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Dtos;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using C8.TradeLicense.Models.enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Helpers
{
    public class LicenseHelper
    {
        private readonly TradeLicenseDbContext _db;
        private int EmailAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["EmailAccountId"]);
        public LicenseHelper(TradeLicenseDbContext db)
        {
            _db = db;
        }
        public LicenseHelper()
        {
            _db = new TradeLicenseDbContext();
        }

        public LicenseApplicationDetails GetRevokeLicenseVM(int licenseId, int userId, LicensesRevoked licensesRevoked)
        {
            try
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

                User UserId = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
                IEnumerable<LicensesRevoked> LicensesRevoked = _db.LicensesRevoked.Where(l => l.LicenseId == licenseId).ToList();

                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.LicenseDetails.LicenseType = license.LicenseType;
                licenseApplicationDetails.LicenseDetails.ItemType = license.ItemType;
                licenseApplicationDetails.LicensesRevokedList = LicensesRevoked;
                licenseApplicationDetails.LicensesRevokedDetails = LicensesRevoked.FirstOrDefault();
                licenseApplicationDetails.InspectionResponses = _db.InspectionResponse.Where(l => l.LicenseId == licenseId).Include(l => l.InspectionRequest).Include(l => l.InspectionRequest.Department).Include(l => l.InspectionRequest.Status).ToList();
                licenseApplicationDetails.DepartmentContacts = _db.DepartmentContacts.Include(l => l.User).ToList();
                licenseApplicationDetails.RegionDetails = license.Region;

                int Inspectordocument = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Inspectiondocument && dt.IsDeleted == false && dt.IsActive)
                                          .Select(d => d.DocumentTypeId)
                                          .FirstOrDefault();
                List<FileUpload> InspectorDocs = _db.FileUploads.Include(d => d.Document)
                                    .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == Inspectordocument && d.IsDeleted == false && d.IsActive).ToList();

                licenseApplicationDetails.InspectionRequestDocuments = InspectorDocs;
                licenseApplicationDetails.InspectionResponseDocuments = InspectorDocs;

                if (licensesRevoked != null)
                {
                    licenseApplicationDetails.LicensesRevokedDetails = new LicensesRevoked
                    {
                        Decision = licensesRevoked.Decision,
                        Comment = licensesRevoked.Comment,
                        DecisionDate = licensesRevoked.DecisionDate
                    };
                }
                return licenseApplicationDetails;
            }
            catch
            {
                return null;
            }
        }

        public LicenseApplicationDetails GetRevokeCancelReviewVM(int licenseId, int userId, RevokeCancelReview revokeCancelReview)
        {
            try
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

                User UserId = _db.Users.Where(u => u.UserId == userId).FirstOrDefault();
                IEnumerable<LicensesRevoked> LicensesRevoked = _db.LicensesRevoked.Where(l => l.LicenseId == licenseId).ToList();

                licenseApplicationDetails.UserDetails = UserId;
                licenseApplicationDetails.LicenseDetails = license;
                licenseApplicationDetails.BusinessDetails = license.Business;
                licenseApplicationDetails.CustomerDetails = license.Client;
                licenseApplicationDetails.LicensesRevokedList = LicensesRevoked;



                if (revokeCancelReview != null)
                {
                    licenseApplicationDetails.RevokeCancelReviewDetails = new RevokeCancelReview
                    {
                        Decision = revokeCancelReview.Decision,
                        Comment = revokeCancelReview.Comment,
                        DateReviewed = revokeCancelReview.DateReviewed
                    };
                }
                return licenseApplicationDetails;
            }
            catch
            {
                return null;
            }
        }

        public LicenseApplicationDetails GetAmendLicenseVM(int licenseId)
        {
            if (licenseId == null)
            {
                return null;
            }

            LicenseApplicationDetails LicenseApplicationDetails = new LicenseApplicationDetails();

            License license = _db.Licenses.Where(l => l.LicenseId == licenseId && l.IsDeleted == false && l.IsActive)
                .Include(l => l.Client)
                .Include(l => l.Business)
                .Include(l => l.LicenseType)
                .Include(l => l.Status)
                .Include(l => l.Region)
                .Include(l => l.ItemType)
                .Include(l => l.ItemSubCategory)
                .Include(l => l.ItemCondition).FirstOrDefault();


            if (license == null)
            {
                return null;
            }
            Amendment amendments = new Amendment();
            BusinessManger businessManger = new BusinessManger();
            Condition condition = new Condition();

            LicenseApplicationDetails.AmendmentDetails = amendments;
            LicenseApplicationDetails.BusinessManagerDetails = businessManger;
            LicenseApplicationDetails.ConditionDetails = condition;
            LicenseApplicationDetails.BusinessDetails = license.Business;
            LicenseApplicationDetails.LicenseDetails = license;
            LicenseApplicationDetails.CustomerDetails = license.Client;
            LicenseApplicationDetails.RegionDetails = license.Region;

            Business business = license.Business;

            #region initialize licenseTypeId


            int licenceTypeId = license.LicenseTypeId;

            #endregion

            SelectList customerTypes = new SelectList(_db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName");
            LicenseApplicationDetails.CustomerTypeList = customerTypes;

            SelectList nationalityList = new SelectList(_db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", LicenseApplicationDetails.CustomerDetails.Nationality);
            LicenseApplicationDetails.NationalityList = nationalityList;

            SelectList individualTypeList = new SelectList(_db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", LicenseApplicationDetails.CustomerDetails.Individual);
            LicenseApplicationDetails.IndividualTypeList = individualTypeList;

            SelectList licenseTypeList = new SelectList(_db.LicenseTypes.Where(i => i.IsDeleted == false && i.IsActive == true), "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            LicenseApplicationDetails.LicenseTypeList = licenseTypeList;

            SelectList itemTypeList = new SelectList(_db.ItemTypes.Where(i => i.IsDeleted == false && i.IsActive == true), "ItemTypeId", "ItemTypeName", license.ItemTypeId);
            LicenseApplicationDetails.ItemTypeList = itemTypeList;
            LicenseApplicationDetails.LicenseDetails.ItemTypeId = license.ItemTypeId;

            SelectList itemSubCategoryList = new SelectList(_db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
            LicenseApplicationDetails.ItemSubCategoryList = itemSubCategoryList;

            SelectList itemConditionList = new SelectList(_db.ItemConditions.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);
            LicenseApplicationDetails.ItemConditionList = itemConditionList;

            string regionName = _db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionId == license.RegionId).Select(s => s.RegionName).FirstOrDefault();
            LicenseApplicationDetails.RegionDetails.RegionName = regionName;

            SelectList businessList = new SelectList(_db.Businesses, "BusinessId", "ProposedTradeName");
            LicenseApplicationDetails.BusinessList = businessList;

            SelectList clientList = new SelectList(_db.Clients, "ClientId", "IdentityOrPassportNumber");
            LicenseApplicationDetails.ClientList = clientList;

            SelectList titleDeedTypeList = new SelectList(_db.TitleDeedTypes, "TitleDeedTypeId", "TitleDeedTypeName");
            LicenseApplicationDetails.TitleDeedTypeList = titleDeedTypeList;
            LicenseApplicationDetails.BusinessDetails.TitleDeedTypeId = license.Business.TitleDeedTypeId;
            LicenseApplicationDetails.BusinessDetails.BusinessId = business.BusinessId;
            LicenseApplicationDetails.LicenseDetails.LicenseTypeId = licenceTypeId;
            LicenseApplicationDetails.BusinessTypeList = new SelectList(_db.BusinessTypes.Where(i => i.IsDeleted == false && i.IsActive), "BusinessTypeId", "BusinessTypeName");
            LicenseApplicationDetails.OperationStructureTypeList = new SelectList(_db.OperationStructureTypes.Where(i => i.IsDeleted == false && i.IsActive), "OperationStructureTypeId", "OperationStructureTypeName");
            LicenseApplicationDetails.BusinessOperatorList = new SelectList(_db.BusinessOperators.Where(i => i.IsDeleted == false && i.IsActive), "BusinessOperatorId", "BusinessOperatorName");
            LicenseApplicationDetails.LicenseConditionList = new SelectList(_db.Conditions.Where(i => i.IsDeleted == false && i.IsActive), "ConditionId", "ConditionName");

            int businessId = Convert.ToInt32(business.BusinessId);

            BusinessManger businessManager = _db.BusinessManger.Where(l => l.BusinessId == businessId).Include(l => l.BusinessOperator).FirstOrDefault();
            LicenseApplicationConditions licenseCondition = _db.LicenseApplicationConditions.Include(l => l.Condition).FirstOrDefault(l => l.LicenseId == license.LicenseId);

            LicenseApplicationDetails.BusinessManagerDetails.BusinessOperatorId = businessManager.BusinessOperatorId;
            LicenseApplicationDetails.ConditionDetails.ConditionId = licenseCondition?.ConditionId ?? 0;

            LicenseApplicationDetails.BusinessMangers = _db.BusinessManger.Where(l => l.BusinessId == businessId).Include(l => l.BusinessOperator).ToList();
            LicenseApplicationDetails.BusinessEmployees = _db.BusinessEmployee.Where(l => l.BusinessId == businessId).ToList();
            LicenseApplicationDetails.LicenseApplicationConditions = _db.LicenseApplicationConditions.Where(l => l.LicenseId == license.LicenseId && l.IsDeleted == false && l.IsActive == true).ToList();

            DateTime zeroTime = new DateTime(1, 1, 1);
            DateTime issuedate = DateTime.Parse(license.LicenseIssueDateTime.ToString());
            DateTime currentdate = DateTime.Now.Date;
            TimeSpan span = currentdate - issuedate;

            if (span.Days > 1)
            {
                int years = (zeroTime + span).Year - 1;
            }

            int? ClientdocumentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();
            int? BussinessdocumentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();

            IEnumerable<FileUpload> clientDocs = _db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == ClientdocumentTypeId && d.IsDeleted == false && d.IsActive).ToList();
            IEnumerable<FileUpload> BussinessDocs = _db.FileUploads.Include(d => d.Document)
                                         .Where(d => d.ClientId == license.ClientId && d.Document.DocumentTypeId == BussinessdocumentTypeId && d.IsDeleted == false && d.IsActive)
                                         .ToList();
            List<Document> allDocs = _db.Documents.Where(d => d.DocumentTypeId == BussinessdocumentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            List<Document> docs = new List<Document>();

            LicenseApplicationDetails.ClientUploadList = clientDocs;
            LicenseApplicationDetails.BussinessUploadList = BussinessDocs;
            LicenseApplicationDetails.ClientOustandingDocuments = _db.Documents.Where(d => d.DocumentTypeId == ClientdocumentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            LicenseApplicationDetails.BussinessOustandingDocuments = (from d in allDocs
                                                                      where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                                      select d).ToList();
            return LicenseApplicationDetails;
        }
        public void AddRevokeCancellReview(int licenseId, int userId, RevokeCancelReview revokeCancelReview)
        {
            License license = _db.Licenses.FirstOrDefault(l => l.LicenseId == licenseId);
            List<LicensesRevoked> LicensesRevoked = _db.LicensesRevoked.Where(l => l.LicenseId == licenseId).ToList();
            revokeCancelReview.UserId = userId;
            revokeCancelReview.LicenseId = licenseId;
            _db.RevokeCancelReview.Add(revokeCancelReview);

            if (revokeCancelReview.Decision == TLKeys.Approve)
            {
                if (LicensesRevoked.LastOrDefault().Decision == TLKeys.RevokeLicense || LicensesRevoked.LastOrDefault().Decision == TLKeys.Cancel)
                {

                    license.IsDeleted = true;
                    license.IsActive = false;
                    if (LicensesRevoked.LastOrDefault().Decision == TLKeys.Cancel)
                    {
                        license.StatusId = _db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.LicenceApplicationCancelled)?.StatusId ?? 0;

                    }
                    else
                    {
                        license.StatusId = _db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.LicenseRevoked)?.StatusId ?? 0;
                    }
                }
                else
                {
                    license.StatusId = _db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.NotRenewed)?.StatusId ?? 0;
                }
                license.LicenseClosureDateTime = DateTime.Now;

            }
            else
            {
                license.StatusId = LicensesRevoked.LastOrDefault().PreviousStatusId;
            }
            _db.Entry(license).State = EntityState.Modified;
            _db.SaveChanges();
        }

        /// <summary>
        /// Generic helper to send an amendment review notification to users in a role.
        /// </summary>
        /// <param name="amendmentId">Amendment id to build the message (must resolve to a LicenseAmendment).</param>
        /// <param name="roleKey">RoleKey to filter recipients (e.g. RoleKeys.LicensingAdministrator).</param>
        /// <param name="controller">Optional controller used to generate an action link. If null no link will be included.</param>
        /// <param name="filterByRegion">If true restrict recipients to the amendment's license region (used for ChiefInspectors).</param>
        public void SendLicenseForReview(int amendmentId, string roleKey, Controller controller = null, bool filterByRegion = false)
        {
            // Load amendment with related license and business once
            LicenseAmendment license = _db.LicenseAmendments
                .Include(l => l.Business)
                .Include(l => l.License.Region)
                .Include(l => l.License)
                .FirstOrDefault(l => l.AmendmentId == amendmentId);

            if (license == null || license.License == null || license.Business == null)
                return;

            IQueryable<User> usersQuery = _db.Users.Where(c => c.Role == roleKey && c.IsActive);

            if (filterByRegion)
            {
                int? regionId = license.License.RegionId;
                var centralSouthRegion = _db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_CentralSouth);
                var nortWestRegion = _db.Regions.FirstOrDefault(r => r.RegionKey == TLKeys.metro_NorthWest);

                if (regionId.HasValue)
                {
                    switch (license.License.Region.RegionKey)
                    {
                        case TLKeys.metro_central:
                        case TLKeys.metro_south:
                            usersQuery = usersQuery.Where(d => d.IsDeleted == false && d.IsActive && (d.Region == regionId || d.Region == centralSouthRegion.RegionId));
                            break;
                        case TLKeys.metro_north:
                        case TLKeys.meto_west:
                            usersQuery = usersQuery.Where(d => d.IsDeleted == false && d.IsActive && (d.Region == regionId || d.Region == nortWestRegion.RegionId));
                            break;
                        default:
                            usersQuery = usersQuery.Where(d => d.IsDeleted == false && d.IsActive && d.Region == (int)regionId);
                            break;
                    }
                }

            }

            var recipients = usersQuery.ToList();
            if (!recipients.Any())
                return;

            string actionLink = string.Empty;
            if (controller != null)
            {
                try
                {
                    actionLink = controller.Url.Action("Review", "Amendment", new { id = license.AmendmentId }, controller.Request.Url.Scheme);
                }
                catch
                {
                    // ignore action link generation failure; still send email without the link
                    actionLink = string.Empty;
                }
            }

            string emailSubject = $"eThekwini Trade Licensing - Amendment for Feedback: {license.License.LicenseNumber}";
            var sb = new StringBuilder();
            sb.Append("<b>Application Awaiting Review. </b><br/><br/>");
            sb.AppendFormat("Business Name: {0}<br/><br/>", HttpUtility.HtmlEncode(license.Business.ProposedTradeName));
            if (!string.IsNullOrEmpty(actionLink))
                sb.AppendFormat("{0} to view further details.<br/><br/>", actionLink);
            sb.Append("Please do not respond to this email, as it is a system generated email.<br/><br/>");
            string emailBody = sb.ToString();

            foreach (var user in recipients)
            {
                try
                {
                    EmailHelper.SendEmail(EmailAccountId, user.EmailAddress, emailSubject, user.FullName, emailBody, user.UserId.ToString());
                }
                catch
                {
                    // TODO: log per-recipient send failure; do not stop other notifications
                    Console.WriteLine($"Failed to send email to {user.EmailAddress}");
                }
            }
        }

        // Thin wrappers kept for backward compatibility and clearer intent
        public void SendLicenseForAdministratorReview(int licenseId, Controller controller = null)
        {
            // original method located amendment by licenseId; keep same behaviour
            var amendment = _db.LicenseAmendments.FirstOrDefault(l => l.LicenseId == licenseId);
            if (amendment == null) return;
            SendLicenseForReview(amendment.AmendmentId, RoleKeys.LicensingAdministrator, controller, filterByRegion: false);
        }

        public void SendLicenseForChiefReview(int amendmentId, Controller controller = null)
        {
            SendLicenseForReview(amendmentId, RoleKeys.ChiefInspector, controller, filterByRegion: true);
        }

        public void SendLicenseForManagerReview(int amendmentId, Controller controller = null)
        {
            SendLicenseForReview(amendmentId, RoleKeys.LicensingManager, controller, filterByRegion: false);
        }
        public void SendLicenseAmendmentFeedbackEmail(int amendmentId, string feedback, Controller controller = null)
        {
            LicenseAmendment licenseAmendment = _db.LicenseAmendments.Include(l => l.Client).Include(l => l.Business).FirstOrDefault(l => l.AmendmentId == amendmentId);
            var user = _db.Clients.FirstOrDefault(u => u.ClientId == licenseAmendment.Client.ClientId);

            string actionLink = controller.Url.Action("Details", "License", new { id = licenseAmendment.LicenseId }, controller.Request.Url.Scheme);
            string emailSubject = $"eThekwini Trade Licensing - Amendment Feedback: {licenseAmendment.License.LicenseNumber}";
            string emailBody = $"<b>Your license amendment application has received feedback. </b><br/><br/> Business Name: {licenseAmendment.Business.ProposedTradeName}<br/><br/> Feedback: {feedback} <br/><br/>.<br/><br/> Please do not respond to this email, as it is a system generated email.<br/><br/>";
            EmailHelper.SendEmail(EmailAccountId, user.EmailAddress, emailSubject, $"{user.Fullname}", emailBody, user.ClientId.ToString());
        }

        public int AmendLicense(LicenseApplicationDto licenseApplication)
        {
            switch (licenseApplication.AmendmentType)
            {
                case AmendmentType.Normal:
                    {
                        LicenseAmendment licenseAmendment = new LicenseAmendment();

                        Client client = _db.Clients.FirstOrDefault(c => c.ClientId == licenseApplication.Client.ClientId);
                        client.Name = licenseApplication.Client.Name;
                        client.Surname = licenseApplication.Client.Surname;
                        client.ResidentialAddress1 = licenseApplication.Client.ResidentialAddress1;
                        client.ResidentialAddress2 = licenseApplication.Client.ResidentialAddress2;
                        client.ResidentialAddress3 = licenseApplication.Client.ResidentialAddress3;
                        client.ResidentialAddressCode = licenseApplication.Client.ResidentialAddressCode;
                        client.PostalAddress1 = licenseApplication.Client.PostalAddress1;
                        client.PostalAddress2 = licenseApplication.Client.PostalAddress2;
                        client.PostalAddress3 = licenseApplication.Client.PostalAddress3;
                        client.PostalAddressCode = licenseApplication.Client.PostalAddressCode;
                        client.IdentityOrPassportNumber = licenseApplication.Client.IdentityOrPassportNumber;
                        client.EmailAddress = licenseApplication.Client.EmailAddress;
                        client.CellphoneNumber = licenseApplication.Client.CellphoneNumber;
                        client.TelephoneNumber = licenseApplication.Client.TelephoneNumber;
                        client.Nationality = licenseApplication.Client.Nationality;
                        client.AltCellphoneNumber = licenseApplication.Client.AltCellphoneNumber;


                        License license = _db.Licenses.FirstOrDefault(l => l.LicenseId == licenseApplication.License.LicenseId);
                        license.LicenseTypeId = licenseApplication.License.LicenseTypeId;
                        license.ItemTypeId = licenseApplication.License.ItemTypeId;
                        license.ItemSubCategoryId = licenseApplication.License.ItemSubCategoryId;
                        license.ItemConditionId = licenseApplication.License.ItemConditionId;

                        Business business = _db.Businesses.FirstOrDefault(b => b.BusinessId == licenseApplication.Business.BusinessId);
                        business.ProposedTradeName = licenseApplication.Business.ProposedTradeName;
                        business.TitleDeedTypeId = licenseApplication.Business.TitleDeedTypeId;
                        business.PostalAddress1 = licenseApplication.Business.PostalAddress1;
                        business.PostalAddress2 = licenseApplication.Business.PostalAddress2;
                        business.PostalAddress3 = licenseApplication.Business.PostalAddress3;
                        business.PostalAddressCode = licenseApplication.Business.PostalAddressCode;
                        business.PostalAddress1 = licenseApplication.Business.PostalAddress1;
                        business.PostalAddress1 = licenseApplication.Business.PostalAddress1;
                        business.AltCellphoneNumber = licenseApplication.Business.AltCellphoneNumber;
                        business.BusinessTypeId = licenseApplication.Business.BusinessTypeId;
                        business.OperationStructureTypeId = licenseApplication.Business.OperationStructureTypeId;

                        //BusinessManger businessManger = _db.BusinessManger.FirstOrDefault(b => b.BusinessId == business.BusinessId);
                        //if (businessManger != null)
                        //{
                        //    businessManger.NameOfBusinessOperator = licenseApplication.Operator.NameOfBusinessOperator;
                        //    businessManger.OperatorIdentityOrPassportNumber = licenseApplication.Operator.OperatorIdentityOrPassportNumber;
                        //}

                        license.ClientId = client.ClientId;
                        licenseAmendment.LicenseId = license.LicenseId;
                        licenseAmendment.BusinessId = license.Business.BusinessId;

                        licenseAmendment.AmendmentType = licenseApplication.AmendmentType.ToString();

                        licenseAmendment.Business = license.Business;
                        licenseAmendment.License = license;
                        licenseAmendment.Client = client;
                        licenseAmendment.AmendmentDataJson = JsonConvert.SerializeObject(licenseAmendment);
                        licenseAmendment.RequestedDate = DateTime.Now;
                        licenseAmendment.StatusId = _db.Status.Where(s => s.StatusKey == StatusKeys.LicenseAmendmentRequested).Select(s => s.StatusId).FirstOrDefault();

                        _db.LicenseAmendments.Add(licenseAmendment);
                        _db.SaveChanges();

                        return licenseAmendment.AmendmentId;

                    }
                    break;
                case AmendmentType.Transfer:
                    {
                    }
                    break;
                case AmendmentType.Renewal:
                    {
                    }
                    break;
            }
            return 0;
        }

        public void AmendmentApproved(int LicenseAmendmentId)
        {
            LicenseAmendment licenseAmendment = _db.LicenseAmendments.FirstOrDefault(l => l.AmendmentId == LicenseAmendmentId);

            LicenseApplicationDto licenseApplication = JsonConvert.DeserializeObject<LicenseApplicationDto>(licenseAmendment.AmendmentDataJson);

            AmendmentType amendmentType = (AmendmentType)Enum.Parse(typeof(AmendmentType), licenseAmendment.AmendmentType);

            switch (amendmentType)
            {
                case AmendmentType.Normal:
                    {
                        License license = _db.Licenses.FirstOrDefault(l => l.LicenseId == licenseApplication.License.LicenseId);
                        license.LicenseIssueYear = DateTime.Now.Year;
                        license.LicenseIssueDateTime = DateTime.Now;
                        license.LicenseExpiryDate = DateTime.Now.AddYears(1);

                        Business business = _db.Businesses.FirstOrDefault(b => b.BusinessId == licenseApplication.Business.BusinessId);

                        business.ProposedTradeName = licenseApplication.Business.ProposedTradeName;
                        business.ResidentialAddress1 = licenseApplication.Business.ResidentialAddress1;
                        business.ResidentialAddress2 = licenseApplication.Business.ResidentialAddress2;
                        business.ResidentialAddress3 = licenseApplication.Business.ResidentialAddress3;
                        business.ResidentialAddressCode = licenseApplication.Business.ResidentialAddressCode;

                        business.PostalAddress1 = licenseApplication.Business.PostalAddress1;
                        business.PostalAddress2 = licenseApplication.Business.PostalAddress2;
                        business.PostalAddress3 = licenseApplication.Business.PostalAddress3;
                        business.PostalAddressCode = licenseApplication.Business.PostalAddressCode;

                        Client client = _db.Clients.FirstOrDefault(c => c.ClientId == licenseApplication.Client.ClientId);

                        client.Name = licenseApplication.Client.Name;
                        client.Surname = licenseApplication.Client.Surname;
                        client.IdentityOrPassportNumber = licenseApplication.Client.IdentityOrPassportNumber;
                        client.ResidentialAddress1 = licenseApplication.Client.ResidentialAddress1;
                        client.ResidentialAddress2 = licenseApplication.Client.ResidentialAddress2;
                        client.ResidentialAddress3 = licenseApplication.Client.ResidentialAddress3;
                        client.ResidentialAddressCode = licenseApplication.Client.ResidentialAddressCode;

                        licenseAmendment.StatusId = _db.Status.Where(s => s.StatusKey == StatusKeys.LicenseAmendmentApproved).Select(s => s.StatusId).FirstOrDefault();
                        licenseAmendment.ApprovedDate = DateTime.Now;

                        _db.Licenses.AddOrUpdate(license);
                        _db.Businesses.AddOrUpdate(business);
                        _db.Clients.AddOrUpdate(client);                 
                        _db.SaveChanges();

                        _db.LicenseAmendments.AddOrUpdate(licenseAmendment);
                        _db.SaveChanges();

                        List<FileUpload> fileUploads = _db.FileUploads.Where(f => f.referenceId == licenseAmendment.AmendmentId).ToList();
                        foreach (var file in fileUploads)
                        {
                            file.referenceId = business.BusinessId;
                            _db.FileUploads.AddOrUpdate(file);
                            _db.SaveChanges();
                        }
                    }
                    break;
            }
        }
    }
}