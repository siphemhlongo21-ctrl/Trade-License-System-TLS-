using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using Elmah;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Helpers
{
    public class MigratedLicenseHelpers
    {
        private readonly TradeLicenseDbContext _db;
        private readonly FileHelpers _fileHelpers;
        public MigratedLicenseHelpers(TradeLicenseDbContext db)
        {
            _db = db;
            _fileHelpers = new FileHelpers(db);
        }
        public MigratedLicenseHelpers()
        {
            _db = new TradeLicenseDbContext();
            _fileHelpers = new FileHelpers(_db);
        }

        public LicenseApplicationDetails GetEditLicenseVM(int id, bool isAdmin, int regionId, string message, string messageType)
        {
            if (id ==0)
            {
                return null;
            }
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            licenseApplicationDetails.Exits = "0";


            MigratedLicense license = _db.MigratedLicense.Where(l => l.MLicenseId == id)
                                    .Include(l => l.MigratedClient)
                                    .Include(l => l.MigratedBusiness)
                                    .Include(l => l.MigratedBusiness.BusinessOperator)
                                    .Include(l => l.LicenseType)
                                    .Include(l => l.Status)
                                    .Include(l => l.Status.StatusType)
                                    .Include(l => l.Region)
                                    .Include(l => l.ItemType)
                                    .Include(l => l.ItemSubCategory)
                                    .Include(l => l.ItemCondition)
                                    .FirstOrDefault();

            if (license == null)
            {
                return null;
            }


            licenseApplicationDetails.MigratedLicense = license;
            licenseApplicationDetails.MigratedBusiness = license.MigratedBusiness;
            licenseApplicationDetails.MigratedClient = license.MigratedClient;



            MigratedBusiness business = license.MigratedBusiness;

            if (null != business)
            {
                licenseApplicationDetails.MigratedBusiness = business;
                licenseApplicationDetails.MigratedClient = license.MigratedClient;
            }


            licenseApplicationDetails.MigratedLicense = license;
            int? region = _db.Regions?
                .Where(x => x.RegionKey == TLKeys.metro_all)
                .FirstOrDefault().RegionId;

            licenseApplicationDetails.LicenseTypeDetails = _db.LicenseTypes.Find(license.LicenseTypeId);
            licenseApplicationDetails.ItemTypeDetails = _db.ItemTypes.Find(business.ItemTypeId);
            /////
            int businessId = Convert.ToInt32(business.MBusinessId);
            List<BusinessManger> BusinessMangerId = _db.BusinessManger.Where(b => b.BusinessId == businessId).ToList();

            licenseApplicationDetails.BusinessMangers = _db.BusinessManger.Where(l => l.BusinessMangerId == BusinessMangerId.FirstOrDefault().BusinessMangerId).Include(l => l.BusinessOperator).ToList();
            List<BusinessEmployee> BusinessEmployeeId = _db.BusinessEmployee.Where(b => b.BusinessId == businessId).ToList();
            licenseApplicationDetails.BusinessEmployees = _db.BusinessEmployee.Where(l => l.BusinessEmployeeId == BusinessEmployeeId.FirstOrDefault().BusinessEmployeeId).ToList();

            licenseApplicationDetails.TitleDeedTypeList = new SelectList(_db.TitleDeedTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "TitleDeedTypeId", "TitleDeedTypeName");
            licenseApplicationDetails.LicenseTypeList = new SelectList(_db.LicenseTypes.Where(i => i.IsDeleted == false && i.IsActive == true), "LicenseTypeId", "LicenseTypeName", license.LicenseTypeId);
            licenseApplicationDetails.ItemTypeList = new SelectList(_db.ItemTypes.Where(i => i.IsDeleted == false && i.IsActive == true), "ItemTypeId", "ItemTypeName", license.ItemTypeId);
            if (license.ItemSubCategory != null)
            {
                if (license.ItemSubCategory.ItemSubCategoryName != "Migrated Item Sub-Category")
                {
                    licenseApplicationDetails.ItemSubCategoryList = new SelectList(_db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
                }
                else
                {
                    licenseApplicationDetails.ItemSubCategoryList = new SelectList(_db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
                }
            }
            else
            {
                licenseApplicationDetails.ItemSubCategoryList = new SelectList(_db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true && i.ItemSubCategoryName != "Migrated Item Sub-Category"), "ItemSubCategoryId", "ItemSubCategoryName", license.ItemSubCategoryId);
            }
            licenseApplicationDetails.ItemConditionList = new SelectList(_db.ItemConditions.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive), "ItemConditionId", "ItemConditionName", license.ItemConditionId);

            licenseApplicationDetails.RegionList = new SelectList(_db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionId == regionId && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
            if (isAdmin)
            {
                licenseApplicationDetails.RegionList = new SelectList(_db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
            }
            if (regionId == region)
            {
                licenseApplicationDetails.RegionList = new SelectList(_db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true && r.RegionKey != "metro_all"), "RegionId", "RegionName", license.RegionId);
            }

            licenseApplicationDetails.BusinessList = new SelectList(_db.Businesses, "BusinessId", "ProposedTradeName");
            licenseApplicationDetails.ClientList = new SelectList(_db.Clients, "ClientId", "IdentityOrPassportNumber");
            licenseApplicationDetails.Userlist = new SelectList(_db.Users, "UserId", "FirstName");
            licenseApplicationDetails.MigratedLicense.LicenseTypeId = license.LicenseTypeId;
            licenseApplicationDetails.MigratedBusiness.MBusinessId = business.MBusinessId;
            licenseApplicationDetails.Userlist = new SelectList(_db.Users, "UserId", "FirstName");

            List<SelectListItem> statusSelectList = new List<SelectListItem>();

            IOrderedQueryable<Status> statusList = _db.Status.Where(s => s.StatusType.StatusTypeName == "Licence Status" &&
                                                (s.StatusKey == StatusKeys.LicensePending ||
                                                s.StatusKey == StatusKeys.LicenseApproved ||
                                                s.StatusKey == StatusKeys.AppealAbandoned)).OrderBy(c => c.StatusName);
            foreach (var item in statusList)
            {
                string text = item.StatusKey == StatusKeys.LicensePending
                            ? item.StatusName + " (Department circulation)"
                            : item.StatusName;

                statusSelectList.Add(new SelectListItem
                {
                    Value = item.StatusId.ToString(),
                    Text = text,
                    Selected = license.StatusId == item.StatusId
                });

            }

            licenseApplicationDetails.StatusList = new SelectList(statusSelectList, nameof(SelectListItem.Value), nameof(SelectListItem.Text), license.StatusId);
            licenseApplicationDetails.CustomerTypeList = new SelectList(_db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName", licenseApplicationDetails.MigratedClient.CustomerType);
            licenseApplicationDetails.NationalityList = new SelectList(_db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", licenseApplicationDetails.MigratedClient.Nationality);
            licenseApplicationDetails.IndividualTypeList = new SelectList(_db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", licenseApplicationDetails.MigratedClient.Individual);
            licenseApplicationDetails.BusinessTypeList = new SelectList(_db.BusinessTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessTypeId", "BusinessTypeName", licenseApplicationDetails.MigratedBusiness.BusinessType);


            //////
            licenseApplicationDetails.OperationStructureTypeList = new SelectList(_db.OperationStructureTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "OperationStructureTypeId", "OperationStructureTypeName");
            licenseApplicationDetails.BusinessOperators = _db.BusinessOperators.ToList().Where(i => i.IsDeleted == false && i.IsActive);
            licenseApplicationDetails.BusinessOperatorList = new SelectList(_db.BusinessOperators.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessOperatorId", "BusinessOperatorName");

            SelectList Userlist = new SelectList(_db.Users.Where(i => i.IsDeleted == false && i.IsActive == true), "UserId", "FirstName");

            licenseApplicationDetails.Userlist = Userlist;

            int? ClientdocumentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();

            int? BussinessdocumentTypeId = _db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();

            List<FileUpload> clientDocs = _db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == license.MClientId && d.Document.DocumentTypeId == ClientdocumentTypeId && d.IsDeleted == false && d.IsActive).ToList();
            List<FileUpload> BussinessDocs = _db.FileUploads.Include(d => d.Document)
                                         .Where(d => d.ClientId == license.MClientId && d.Document.DocumentTypeId == BussinessdocumentTypeId && d.IsDeleted == false && d.IsActive)
                                         .ToList();
            List<Document> allDocs = _db.Documents.Where(d => d.DocumentTypeId == BussinessdocumentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            List<Document> docs = new List<Document>();

            licenseApplicationDetails.ClientUploadList = clientDocs;
            licenseApplicationDetails.BussinessUploadList = BussinessDocs;
            licenseApplicationDetails.BussinessOustandingDocuments = (from d in allDocs
                                                                      where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                                      select d).ToList();

            licenseApplicationDetails.ClientOustandingDocuments = _db.Documents.Where(d => d.DocumentTypeId == ClientdocumentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            licenseApplicationDetails.BussinessDocuments = (from d in allDocs
                                                            where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                            select d).ToList();
            licenseApplicationDetails.Message = message;
            licenseApplicationDetails.MessageType = messageType;
            return licenseApplicationDetails;
        }
        public string CheckFilesUploaded(IEnumerable<HttpPostedFileBase> files)
        {
            return ViewSettingsKeys.Success;
            if (files != null && files.Any())
            {
                ///check reqiured docs are uploaded
                int count = 0;
                foreach (var file in files)
                {
                    if (count == 0)
                    {
                        if (file == null)
                        {
                            return "Please Upload Required Documents";
                        }
                    }
                    if (count == 1)
                    {
                        if (file == null)
                        {
                            return "Please Upload Required Documents";
                        }
                    }
                    if (count == 2)
                    {
                        if (file == null)
                        {
                            return "Please Upload Required Documents";
                        }
                    }
                    if (count == 6)
                    {
                        if (file == null)
                        {
                            return "Please Upload Required Documents";
                        }
                    }
                    count++;
                }
            }
            return ViewSettingsKeys.Success;
        }

        public int CreateBusinessFromMigrated(MigratedBusiness migratedBusiness, int clientId, string BusinessDetailsisSame)
        {
            int businessstatus = _db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.BusinessApproved)?.StatusId ?? 0;
            Business Business = new Business();
            Business.ClientId = clientId;

            Business.BusinessTypeId = Convert.ToInt32(migratedBusiness.BusinessTypeId);
            Business.CellphoneNumber = migratedBusiness.CellphoneNumber;
            Business.AltCellphoneNumber = migratedBusiness.AltCellphoneNumber;
            Business.FaxNumber = migratedBusiness.FaxNumber;

            Business.RatesAccountNumber = migratedBusiness.RatesAccountNumber;
            Business.ProposedTradeName = migratedBusiness.ProposedTradeName;
            Business.TelephoneNumber = migratedBusiness.TelephoneNumber;
            Business.ResidentialShopOrUnitNumber = migratedBusiness.ResidentialShopOrUnitNumber;
            Business.OperationStructureTypeId = Convert.ToInt32(migratedBusiness.OperationStructureTypeId);
            Business.TitleDeedTypeId = Convert.ToInt32(migratedBusiness.TitleDeedTypeId);
            Business.ItemTypeId = migratedBusiness.ItemTypeId.Value;


            Business.KnownAs = migratedBusiness.KnownAs;
            Business.PostalAddress1 = migratedBusiness.PostalAddress1;
            Business.PostalAddress2 = migratedBusiness.PostalAddress2;
            Business.PostalAddress3 = migratedBusiness.PostalAddress3;
            Business.PostalAddressCode = Convert.ToInt32(migratedBusiness.PostalAddressCode);
            Business.ResidentialAddress1 = migratedBusiness.ResidentialAddress1;
            Business.ResidentialAddress2 = migratedBusiness.ResidentialAddress2;
            Business.ResidentialAddress3 = migratedBusiness.ResidentialAddress3;
            Business.ResidentialAddressCode = Convert.ToInt32(migratedBusiness.ResidentialAddressCode);

            Business.SameAs = Convert.ToBoolean(BusinessDetailsisSame);
            Business.BusinessStatusId = businessstatus;
            Business.IsActive = true;
            _db.Businesses.Add(Business);
            _db.SaveChanges();
            return Business.BusinessId;
        }

        public int CreateClientFromMigrated(MigratedClient migratedClient, string CustomerDetailsisSame)
        {
            int clientstatus = _db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.ClientApproved)?.StatusId ?? 0;
            Client Customer = new Client();
            Customer.CustomerType = migratedClient.CustomerType;
            Customer.Individual = migratedClient.Individual;
            Customer.Nationality = migratedClient.Nationality;
            Customer.NationalityOther = migratedClient.NationalityOther;
            Customer.BusinessName = migratedClient.BusinessName;
            Customer.RegNumber = migratedClient.RegNumber;
            Customer.IdentityOrPassportNumber = migratedClient.IdentityOrPassportNumber;
            Customer.PostalAddress1 = migratedClient.PostalAddress1;
            Customer.PostalAddress2 = migratedClient.PostalAddress2;
            Customer.PostalAddress3 = migratedClient.PostalAddress3;
            Customer.PostalAddressCode = Convert.ToInt32(migratedClient.PostalAddressCode);
            Customer.SameAs = Convert.ToBoolean(CustomerDetailsisSame);
            Customer.ResidentialAddress1 = migratedClient.ResidentialAddress1;
            Customer.ResidentialAddress2 = migratedClient.ResidentialAddress2;
            Customer.ResidentialAddress3 = migratedClient.ResidentialAddress3;
            Customer.ResidentialAddressCode = Convert.ToInt32(migratedClient.ResidentialAddressCode);
            Customer.TelephoneNumber = migratedClient.TelephoneNumber;
            Customer.CellphoneNumber = migratedClient.CellphoneNumber;
            Customer.AltCellphoneNumber = migratedClient.AltCellphoneNumber;
            Customer.EmailAddress = migratedClient.EmailAddress;
            Customer.Name = migratedClient.Name;
            Customer.Surname = migratedClient.Surname;
            Customer.ClientStatusId = clientstatus;
            Customer.IsActive = true;
            _db.Clients.Add(Customer);
            int count = _db.SaveChanges();
            return Customer.ClientId;
        }
        public void AddBusinessManager(string OperatorTableData, int businessId)
        {
            char[] spearator = { '*' };
            String[] OperatorTableDatalist = OperatorTableData.Split(spearator, StringSplitOptions.None);
            OperatorTableDatalist = OperatorTableDatalist.Where(t => t != "").ToArray();
            foreach (var item in OperatorTableDatalist)
            {
                char[] separator = { ',' };
                String[] Data = item.Split(separator, StringSplitOptions.None);
                Data = Data.Where(t => t != "" && t != " ").ToArray();
                BusinessManger BusinessManger = new BusinessManger();
                BusinessManger.BusinessId = businessId;
                BusinessManger.BusinessOperatorId = Int32.Parse(Data[0]);
                BusinessManger.NameOfBusinessOperator = Data[1];
                BusinessManger.OperatorIdentityOrPassportNumber = Data[2];
                BusinessManger.OperatorResidentialAddress1 = Data[3];
                BusinessManger.OperatorResidentialAddress3 = Data[4];
                BusinessManger.OperatorResidentialAddress2 = Data[5];
                bool isIntString = Data[6].All(char.IsDigit);
                if (isIntString == true)
                {
                    BusinessManger.OperatorResidentialAddressCode = Int32.Parse(Data[6]);
                }
                else
                {
                    BusinessManger.OperatorResidentialAddressCode = Int32.Parse("0000");

                }
                BusinessManger.IsActive = true;
                BusinessManger.IsDeleted = false;
                BusinessManger.IsLocked = false;
                _db.BusinessManger.Add(BusinessManger);
                _db.SaveChanges();
            }
        }

        public void AddEmployeedDetails(string EmployeeTableData, int businessId)
        {
            if (EmployeeTableData != null)
            {
                char[] spearator = { '*' };
                String[] EmployeeTableDatalist = EmployeeTableData.Split(spearator, StringSplitOptions.None);
                EmployeeTableDatalist = EmployeeTableDatalist.Where(t => t != "").ToArray();
                foreach (var item in EmployeeTableDatalist)
                {
                    char[] separator = { ',' };
                    String[] Data = item.Split(separator, StringSplitOptions.None);
                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                    BusinessEmployee BusinessEmployee = new BusinessEmployee();
                    BusinessEmployee.BusinessId = businessId;
                    BusinessEmployee.NameOfEmployer = Data[0];
                    BusinessEmployee.EmployerResidentialAddress1 = Data[1];
                    BusinessEmployee.EmployerResidentialAddress3 = Data[2];
                    BusinessEmployee.EmployerResidentialAddress2 = Data[3];
                    bool isIntString = Data[4].All(char.IsDigit);
                    if (isIntString == true)
                    {
                        BusinessEmployee.EmployerResidentialAddressCode = Int32.Parse(Data[4]);
                    }
                    else
                    {
                        BusinessEmployee.EmployerResidentialAddressCode = Int32.Parse("0000");
                    }
                    BusinessEmployee.IsActive = true;
                    BusinessEmployee.IsDeleted = false;
                    BusinessEmployee.IsLocked = false;
                    _db.BusinessEmployee.Add(BusinessEmployee);
                    _db.SaveChanges();
                }
            }
        }
        public int CreateLicenseFromMigrated(MigratedLicense migratedLicense, int clientId, int businessId, int approvedstatus)
        {
            License Licence = new License();
            Licence.migratedLicense = true;
            Licence.ClientId = clientId;
            Licence.BusinessId = businessId;
            Licence.StatusId = migratedLicense.StatusId.Value;
            Licence.IsActive = true;
            Licence.IsDeleted = false;
            Licence.IsLocked = false;
            if (migratedLicense.ApplicationDateTime == null)
            {
                Licence.ApplicationDateTime = DateTime.Now;
            }
            else
            {
                Licence.ApplicationDateTime = migratedLicense.ApplicationDateTime;
            }
            if (migratedLicense.LicenseIssueYear != null)
            {
                Licence.LicenseIssueYear = Convert.ToInt32(migratedLicense.LicenseIssueYear);
            }
            if (migratedLicense.NotificationUpdatedDateTime != null)
            {
                Licence.NotificationUpdatedDateTime = migratedLicense.NotificationUpdatedDateTime;
            }
            if (migratedLicense.LicenseExpiryDate != null)
            {
                Licence.LicenseExpiryDate = migratedLicense.LicenseExpiryDate;
            }
            if (migratedLicense.LicenseIssueDateTime != null)
            {
                Licence.LicenseIssueDateTime = migratedLicense.LicenseIssueDateTime;
            }
            if (Licence.StatusId == approvedstatus)
            {
                if (migratedLicense.LicenseExpiryDate == null && migratedLicense.LicenseIssueDateTime != null)
                {
                    Licence.LicenseExpiryDate = (DateTime)migratedLicense.LicenseIssueDateTime.Value.AddYears(1);
                }
                if (migratedLicense.LicenseIssueDateTime == null)
                {
                    Licence.LicenseIssueDateTime = DateTime.Now;
                }
            }
            Licence.ItemConditionId = migratedLicense.ItemConditionId.Value;
            Licence.ItemSubCategoryId = migratedLicense.ItemSubCategoryId;
            Licence.ItemTypeId = migratedLicense.ItemTypeId;
            Licence.LicenseTypeId = migratedLicense.LicenseTypeId.Value;
            Licence.LicenseNumber = migratedLicense.LicenseNumber;
            Licence.IsActive = true;
            Licence.RegionId = migratedLicense.RegionId;
            _db.Licenses.Add(Licence);
            _db.SaveChanges();
            return Licence.LicenseId;
        }

        public void AddPayment(string PayinslipTblData, int licenseId)
        {
            ///payment details
            PaymentLicense payment = new PaymentLicense();


            char[] spearator = { '*' };

            if (string.IsNullOrEmpty(PayinslipTblData)) return;

            String[] TableDatalist = PayinslipTblData.Split(spearator, StringSplitOptions.None);
            TableDatalist = TableDatalist.Where(t => t != "").ToArray();
            foreach (var item in TableDatalist)
            {
                char[] separator = { ',' };
                String[] Data = item.Split(separator, StringSplitOptions.None);
                Data = Data.Where(t => t != "" && t != " ").ToArray();

                payment.LicenseId = licenseId;
                payment.PAYINSLIP_NO = Data[0].ToString();
                payment.CUST_ACCT_NO = Data[1].ToString();
                payment.SERVICE_UNIT = Data[2].ToString();
                payment.REQUEST_NO = Data[3].ToString();
                if (Data[4].ToString() != "N/A")
                {
                    payment.PAYINSLIP_AMOUNT = Convert.ToDouble(Data[4].ToString());
                }
                else
                {
                    payment.PAYINSLIP_AMOUNT = 0;
                }
                if (Data[5].ToString() != "N/A")
                {
                    payment.ALLOCATED_AMOUNT = Convert.ToDouble(Data[5].ToString());
                }

                if (Data[6].ToString() != "N/A")
                {
                    payment.PAID_AMOUNT = Convert.ToDouble(Data[6].ToString());
                }
                if (Data[7].ToString() != "N/A")
                {
                    payment.PAID_DATE = Data[7].ToString();
                }

                if (Data[8].ToString() != "N/A")
                {
                    payment.PAYINSLIP_DATE = Data[8].ToString();
                }
                if (Data[9].ToString() != "N/A")
                {
                    payment.BALANCE_UNALLOCATED_AMOUNT = Convert.ToDouble(Data[9].ToString());
                }
                _db.PaymentLicense.Add(payment);
                _db.SaveChanges();
            }
        }

        public void DeleteMigratedRecords(MigratedBusiness migratedBusiness, MigratedClient migratedClient, MigratedLicense migratedLicense)
        {
            migratedBusiness.IsDeleted = true;
            migratedClient.IsDeleted = true;
            migratedLicense.IsDeleted = true;
            migratedBusiness.IsActive = false;
            migratedClient.IsActive = false;
            migratedLicense.IsActive = false;
            _db.Entry(migratedLicense).State = EntityState.Modified;
            _db.Entry(migratedBusiness).State = EntityState.Modified;
            _db.Entry(migratedClient).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void ProcessMigrated(LicenseApplicationDetails licenseApplicationDetails, IEnumerable<HttpPostedFileBase> files,
            string CustomerDetailsisSame, string BusinessDetailsisSame, string OperatorTableData,
            string EmployeeTableData, BusinessEmployee BusinessEmployee
            , string clientComment, string BussinessComment, int userId, string username, int approvedstatus)
        {
            int doccount = 0;
            int clientId = 0;
            int businessId = 0;
            int licenseId = 0;

            clientId = CreateClientFromMigrated(licenseApplicationDetails.MigratedClient, CustomerDetailsisSame);
            if (clientId == 0) throw new Exception("Failed to created customer entity");

            _fileHelpers.SaveClientFiles(files, clientComment, clientId,
                licenseApplicationDetails.MigratedClient.Fullname, username, userId);
            businessId = CreateBusinessFromMigrated(licenseApplicationDetails.MigratedBusiness, clientId, BusinessDetailsisSame);
            AddBusinessManager(OperatorTableData, businessId);
            AddEmployeedDetails(EmployeeTableData, businessId);
            _fileHelpers.SaveBusinessFiles(files, BussinessComment, clientId, businessId, licenseApplicationDetails.MigratedClient.Fullname,
                username, userId, licenseApplicationDetails.MigratedBusiness.ItemTypeId);
            licenseId = CreateLicenseFromMigrated(licenseApplicationDetails.MigratedLicense
                , clientId, businessId, approvedstatus);
            AddPayment(licenseApplicationDetails.PayinslipTblData, licenseId);
            DeleteMigratedRecords(licenseApplicationDetails.MigratedBusiness, licenseApplicationDetails.MigratedClient,
                licenseApplicationDetails.MigratedLicense);
        }
    }
}