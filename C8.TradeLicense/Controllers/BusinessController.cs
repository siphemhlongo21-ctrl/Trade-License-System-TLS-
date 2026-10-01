using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.ViewModels;
using C8.TradeLicense.Keys;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using PagedList;
using C8.TradeLicense.Helpers;
using System.Web.Services.Description;

namespace C8.TradeLicense.Controllers
{
    public class BusinessController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private FileHelpers _fileHelpers;
        public BusinessController()
        {
            // JK.20140906a - Instantiate the IdentityManager in the contructor and pass the DbContext.
            IdentityManager = new IdentityManager(db);
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

        // GET: /Business/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            var documentTypeId =
            db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument)
                .Select(d => d.DocumentTypeId)
                .FirstOrDefault();
            var doc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.DocumentKey != DocumentTypeKeys.partnershipdocument);
            int count = doc.Count();
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

            var businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client)
                                          .Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                     
                                          .Where(c => c.IsDeleted == false && c.IsActive == true && c.ItemType.IsDeleted == false && c.ItemType.IsActive) // && c.BusinessStatusId == db.Status.Where(s => s.StatusKey == "BusinessApproved").Select(s => s.StatusId)
                                          .ToList();

            var clients = db.Clients.Include(c => c.CreatedByUser)
                                    .Include(c => c.ModifiedByUser)
                                  
                                    .Include(c => c.ClientStatus)
                                    .Where(c => c.IsDeleted == false && c.IsActive == true)
                                    .ToList();
            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        businesses = businesses.Where(l => (l.Client.Name.ToLower() + " " + l.Client.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "IDPassportNumber":
                        //Search by reference number
                        businesses = businesses.Where(l => l.Client.IdentityOrPassportNumber == inputSearch).ToList();
                        break;
                    case "BusinessName":
                        //Search by business name
                        businesses = businesses.Where(l => l.ProposedTradeName.ToLower().Contains(inputSearch.ToLower())).ToList();
                        break;
                }
            }

            ViewBag.CanAction = CanAction();
            ViewData["RegisteredClients"] = clients;

            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.BusinessCount = businesses.Count;
            return View(businesses.ToPagedList(pageNumber, pageSize));
            //return View(businesses.ToList());
        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        [HttpPost]
        public ActionResult _BusinessPartial(int id)
        {
            var businesses = db.Businesses.Where(c => c.ClientId == id).ToList();

            return PartialView("_ClientBusinessPartial", businesses);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        [HttpPost]
        public ActionResult Index(int? page, string dropdownClient)
        {

            var clientId = Convert.ToInt32(dropdownClient);
            var client = db.Clients
                                  .Where(c => c.ClientId == clientId && c.IsDeleted == false && c.IsActive)
                                  .FirstOrDefault();

            if (null != client)
            {
                ViewData["ClientData"] = client;
            }

            var businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client)
                                          .Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                        
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ItemType.IsDeleted == false && c.ItemType.IsActive) // && c.BusinessStatusId == db.Status.Where(s => s.StatusKey == "BusinessApproved").Select(s => s.StatusId)
                                          .ToList();

            var clients = db.Clients.Include(c => c.CreatedByUser)
                                    .Include(c => c.ModifiedByUser)
                                    //.Include(c => c.ClientUser)
                                    .Include(c => c.ClientStatus)
                                    .Where(c => c.IsDeleted == false && c.IsActive)
                                    .ToList();



            ViewBag.CanAction = CanAction();
            ViewData["RegisteredClients"] = clients;

            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {

                page = 1;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(businesses.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Business/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Business business = db.Businesses.Include(b => b.BusinessStatus)
                                             .Include(b => b.BusinessType)
                                             .Include(b => b.OperationStructureType)
                                             .Include(b => b.TitleDeedType)
                                         
                                             .Include(b => b.ItemType)
                                             .FirstOrDefault(b => b.BusinessId == id);

            if (business == null)
            {
                return HttpNotFound();
            }

            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
                                                .Select(d => d.DocumentTypeId)
                                                .FirstOrDefault();
            var docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive == true).Include(d => d.DocumentType)
                                   .ToList();
            var clientDocs = db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive == true)
                                          .ToList();
            var license = db.Licenses.Where(l => l.BusinessId == business.BusinessId && l.IsDeleted == false && l.IsActive == true)
                                     .Include(l => l.LicenseType)
                                     .FirstOrDefault();

            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                                         ).ToList();

            ViewData["Documents"] = (from d in docs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();// db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive);
            int businessId = Convert.ToInt32(business.BusinessId);



            ViewData["BusinessManger"] = db.BusinessManger.Where(l => l.BusinessId == businessId).Include(l => l.BusinessOperator).ToList();

            ViewData["BusinessEmployee"] = db.BusinessEmployee.Where(l => l.BusinessId == businessId).ToList();
            ViewData["UploadList"] = clientDocs;
            ViewBag.OustandingDocuments = OustandingDocuments;
            //ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(i => i.IsDeleted == false && i.IsActive), "ItemTypeId", "ItemTypeName", business.ItemTypeId);
            //ViewBag.BusinessTypeId = new SelectList(db.BusinessTypes.Where(b => b.IsDeleted == false && b.IsActive), "BusinessTypeId", "BusinessTypeName", business.BusinessTypeId);
            //ViewBag.TitleDeedTypeId = new SelectList(db.TitleDeedTypes.Where(t => t.IsDeleted == false && t.IsActive), "TitleDeedTypeId", "TitleDeedTypeName", business.TitleDeedTypeId);
            //ViewBag.OperationStructureTypeId = new SelectList(db.OperationStructureTypes.Where(o => o.IsDeleted == false && o.IsActive), "OperationStructureTypeId", "OperationStructureTypeName", business.OperationStructureTypeId);
            //ViewBag.BusinessOperatorId = new SelectList(db.BusinessOperators.Where(bo => bo.IsDeleted == false && bo.IsActive), "BusinessOperatorId", "BusinessOperatorName", business.BusinessOperatorId);

            return View(business);
        }

        public FileResult DownloadFile(string FileName)
        {
            return File("~/Documents/" + FileName, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        // GET: /Business/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Create(int? page)
        {
            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            //Get route value
            string roateValue = (string)this.RouteData.Values["id"];
            int id = Convert.ToInt32(roateValue);
            Client client = db.Clients.Find(id);

            List<Business> businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client)
                                          .Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)                                                                           
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == id && c.ItemType.IsDeleted == false && c.ItemType.IsActive == true)
                                          .ToList();

            if (null != client)
            {               
                licenseApplicationDetails.CustomerDetails = client;
            }

            // Gets list of required documents based on the document type key
            int documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument)
                                .Select(d => d.DocumentTypeId)
                                .FirstOrDefault();

            IEnumerable<FileUpload> clientDocs = db.FileUploads.Include(d => d.Document)
                                                 .Where(d => d.ClientId == id && d.Document.DocumentTypeId == documentTypeId)
                                                 .Where(d => d.IsActive == true && d.IsDeleted == false)
                                                 .ToList();
     
            List<Document> documents = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).
                                                    Where(d => d.IsActive == true && d.IsDeleted == false).ToList();

            licenseApplicationDetails.BussinessDocuments = documents;
            licenseApplicationDetails.ClientUploadList = clientDocs;
            licenseApplicationDetails.BusinessMangers= db.BusinessManger.Where(l => l.IsLocked == true)
                                                                        .Include(l => l.BusinessOperator).ToList();
            licenseApplicationDetails.BusinessEmployees = db.BusinessEmployee.Where(l => l.IsLocked == true).ToList();

            SelectList itemTypeList = new SelectList(db.ItemTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "ItemTypeId", "ItemTypeName");
            licenseApplicationDetails.ItemTypeList = itemTypeList;

            SelectList itemSubCategoryList = new SelectList(db.ItemSubCategories.ToList().Where(i => i.IsDeleted == false && i.IsActive), "ItemSubCategoryId", "ItemSubCategoryName");
            licenseApplicationDetails.ItemSubCategoryList = itemSubCategoryList;

            SelectList businessTypeList = new SelectList(db.BusinessTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessTypeId", "BusinessTypeName");
            licenseApplicationDetails.BusinessTypeList = businessTypeList;

            SelectList titleDeedTypeList = new SelectList(db.TitleDeedTypes, "TitleDeedTypeId", "TitleDeedTypeName");
            licenseApplicationDetails.TitleDeedTypeList = titleDeedTypeList;

            SelectList operationStructureTypeList = new SelectList(db.OperationStructureTypes.Where(i => i.IsDeleted == false && i.IsActive), "OperationStructureTypeId", "OperationStructureTypeName");
            licenseApplicationDetails.OperationStructureTypeList = operationStructureTypeList;        

            licenseApplicationDetails.BusinessOperatorList = new SelectList(db.BusinessOperators.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessOperatorId", "BusinessOperatorName");

            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
        
            int pageNumber = (page ?? 1);

            licenseApplicationDetails.ClientBusinessesList = businesses;

            if (businesses.Count() <= 0)
            {
                TempData[LicenseApplicationDetails.NoRecordKey] = "No related Businesses to customer found.";
            }

            return View(licenseApplicationDetails);
        }
        public JsonResult checkBName(string BName)
        {
            var exist = false;
           var findname= db.Businesses.Where(b => b.ProposedTradeName == BName).Select(b => b.ProposedTradeName).FirstOrDefault();
            if (findname != null) {
                exist = true;
            }
            return new JsonResult { Data = exist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
         }
        // POST: /Business/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Create(LicenseApplicationDetails licenseApplicationDetails, IEnumerable<HttpPostedFileBase> files, /*bool IsSelfEmployed*/ int? page, string itemTypeValue, string TelephoneNumberCode, string TelephoneNumber, string FaxNumbercode, string FaxNumber, string OperatorTableData,string EmployeeTableData)
        {
            Initialise();
            Business business = licenseApplicationDetails.BusinessDetails;
            business.ClientId = licenseApplicationDetails.CustomerDetails.ClientId;
       
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            var docs = new List<Document>();

            if (itemTypeValue == "Item2")
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d=> d.DocumentType)
                    .ToList();            
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false
                    && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management).Include(d => d.DocumentType).ToList();
            }

            //List<DocumentCheckList> docs1 = db.DocumentCheckLists.Where(dc => dc.LicenseTypeId == license.LicenseTypeId).Include(dc => dc.Document).ToList();
            var client = db.Clients.Find(business.ClientId);
            var businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client)
                                          .Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                    
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == business.ClientId && c.ItemType.IsDeleted == false && c.ItemType.IsActive == true)
                                          .ToList();
            var clientDocs = db.FileUploads.Include(d => d.Document).Include(d => d.Document.DocumentType)
                                           .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId)
                                           .ToList();

            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                                          ).ToList();

            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            int  duplicatebusinsses = 0;
            
           
           
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                //IdentityManager.CurrentUser(User);

                //Validates duplicates business names
                var tradeName =
                    db.Businesses.Where(b => b.ProposedTradeName == business.ProposedTradeName).Select(
                        b => b.ProposedTradeName).FirstOrDefault();

                if (tradeName == null)
                {
                    if (OperatorTableData != "")
                    {
                        try
                        {
                            business.TelephoneNumber = TelephoneNumberCode + TelephoneNumber;
                            business.FaxNumber = FaxNumbercode + FaxNumber;
                            business.IsActive = true;
                            business.IsDeleted = false;
                            business.IsLocked = false;

                          
                            business.BusinessStatusId =
                                db.Status.Where(s => s.StatusKey == StatusKeys.PendingBusinessClearance).Select(s => s.StatusId).FirstOrDefault();

                            db.Businesses.Add(business);
                            db.SaveChanges();



                            char[] spearator = { '*' };

                            BusinessManger BusinessManger = new BusinessManger();
                            // Manger
                            String[] OperatorTableDatalist = OperatorTableData.Split(spearator, StringSplitOptions.None);
                            OperatorTableDatalist = OperatorTableDatalist.Where(t => t != "").ToArray();
                            foreach (var item in OperatorTableDatalist)
                            {
                                char[] separator = { ',' };
                                String[] Data = item.Split(separator, StringSplitOptions.None);
                                Data = Data.Where(t => t != "" && t != " ").ToArray();
                                BusinessManger.BusinessId = business.BusinessId;
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
                            //Employee
                            BusinessEmployee BusinessEmployee = new BusinessEmployee();
                            if (EmployeeTableData != null)
                            {
                                String[] EmployeeTableDatalist = EmployeeTableData.Split(spearator, StringSplitOptions.None);
                                EmployeeTableDatalist = EmployeeTableDatalist.Where(t => t != "").ToArray();
                                foreach (var item in EmployeeTableDatalist)
                                {
                                    char[] separator = { ',' };
                                    String[] Data = item.Split(separator, StringSplitOptions.None);
                                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                                    BusinessEmployee.BusinessId = business.BusinessId;
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
                        }
                        catch (Exception e)
                        {
                            var error = e.InnerException;
                        }


                        if (files != null)
                        {


                            if (files.Any())
                            {
                                //TG20141028 - FileUpload
                                int counter = 0;
                              
                                foreach (HttpPostedFileBase file in files)
                                {

                                    if (file != null)
                                    {
                                        var length = file.ContentLength;
                                        var bytesP = new byte[length];
                                        file.InputStream.Read(bytesP, 0, length);
                                        Byte[] bytes = bytesP;
                                        var Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

                                        SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(bytes, file.FileName, "Business");
                                        _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, client.ClientId
                                        , client.Fullname, Uploaddoc[counter].DocumentId, business.BusinessId, string.Empty, Users.Username);

                                        counter++;
                                    }
                                    else
                                    {
                                        counter++;
                                    }

                                }
                              

                            }
                        }

                        business.BusinessStatusId = db.Status.Where(s => s.StatusKey == StatusKeys.PendingBusinessClearance)
                                                                 .Select(s => s.StatusId)
                                                                 .FirstOrDefault();
                        clientDocs = db.FileUploads.Include(d => d.Document)
                                                   .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId)
                                                   .ToList();

                        if (docs.Count() == clientDocs.Count())
                        {
                            business.BusinessStatusId = db.Status.Where(s => s.StatusKey == StatusKeys.BusinessApproved)
                                                                 .Select(s => s.StatusId)
                                                                 .FirstOrDefault();

                            db.Entry(business).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["Success"] = "Saved Successfully!";
                        }
                
                        else
                        {
                            OustandingDocuments = (from d in docs
                                                   where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                   select d.DocumentName
                                               ).ToList();

                            ViewBag.OustandingDocuments = OustandingDocuments;
                            TempData["Success"] = "Business Saved. Outstanding document";
                        }
                        //TempData["Success"] = "Saved Successfully!";
                    }
                    else
                    {
                        duplicatebusinsses = 1;
                        TempData["Error"] = "Business Operator is Required";
                    }
                }
                else
                {
                    duplicatebusinsses = 1;
                    if (EmployeeTableData != null) {
                        ViewBag.EmployeeTableData = EmployeeTableData;

                    }
                    if (OperatorTableData != null) {
                        ViewBag.OperatorTableData = OperatorTableData;
                    }
                    TempData["Error"] = "There is already a registered business with the same name on the system!";
                }
            }
            if (duplicatebusinsses == 1)
            {             
                if (null != client)
                {
                    licenseApplicationDetails.CustomerDetails = client;
                }


                List<Document> documents = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).
                                                        Where(d => d.IsActive == true && d.IsDeleted == false).ToList();

                licenseApplicationDetails.BussinessDocuments = documents;
                licenseApplicationDetails.ClientUploadList = clientDocs;
                licenseApplicationDetails.BusinessMangers = db.BusinessManger.Where(l => l.IsLocked == true)
                                                                            .Include(l => l.BusinessOperator).ToList();
                licenseApplicationDetails.BusinessEmployees = db.BusinessEmployee.Where(l => l.IsLocked == true).ToList();

                SelectList itemTypeList = new SelectList(db.ItemTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "ItemTypeId", "ItemTypeName");
                licenseApplicationDetails.ItemTypeList = itemTypeList;

                SelectList itemSubCategoryList = new SelectList(db.ItemSubCategories.ToList().Where(i => i.IsDeleted == false && i.IsActive), "ItemSubCategoryId", "ItemSubCategoryName");
                licenseApplicationDetails.ItemSubCategoryList = itemSubCategoryList;

                SelectList businessTypeList = new SelectList(db.BusinessTypes.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessTypeId", "BusinessTypeName");
                licenseApplicationDetails.BusinessTypeList = businessTypeList;

                SelectList titleDeedTypeList = new SelectList(db.TitleDeedTypes, "TitleDeedTypeId", "TitleDeedTypeName");
                licenseApplicationDetails.TitleDeedTypeList = titleDeedTypeList;

                SelectList operationStructureTypeList = new SelectList(db.OperationStructureTypes.Where(i => i.IsDeleted == false && i.IsActive), "OperationStructureTypeId", "OperationStructureTypeName");
                licenseApplicationDetails.OperationStructureTypeList = operationStructureTypeList;

                licenseApplicationDetails.BusinessOperatorList = new SelectList(db.BusinessOperators.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessOperatorId", "BusinessOperatorName");

                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    page = 1;
                }

                 pageNumber = (page ?? 1);

                licenseApplicationDetails.ClientBusinessesList = businesses;

                if (businesses.Count() <= 0)
                {
                    TempData[LicenseApplicationDetails.NoRecordKey] = "No related Businesses to customer found.";
                }

                return View(licenseApplicationDetails);
            }

            return RedirectToAction("Index");
    

            }

        // GET: /Business/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            Business business = db.Businesses.Include(b => b.BusinessStatus)
                
                                            .Include(b => b.BusinessType)
                                            .Include(b => b.OperationStructureType)
                                            .Include(b => b.TitleDeedType)                                     
                                            .Include(b => b.ItemType)
                                            .FirstOrDefault(b => b.BusinessId == id);

            if (business == null)
            {
                return HttpNotFound();
            }
            //Get route value

            int businessId = Convert.ToInt32(business.BusinessId);
            Client client = db.Clients.Find(business.ClientId);

            if (null != client)
            {
               licenseApplicationDetails.CustomerDetails = client;

            }

            int? documentTypeId = db.DocumentTypes?
                                 .Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument && dt.IsDeleted == false && dt.IsActive)
                                 .Select(d => d.DocumentTypeId)
                                 .FirstOrDefault();

            List<Document> allDocs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
        
            List<Document> docs = new List<Document>();

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

            List<FileUpload> clientDocs = db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId && d.IsDeleted == false && d.IsActive)
                                          .ToList();

            License license = db.Licenses.Where(l => l.BusinessId == business.BusinessId && l.IsDeleted == false && l.IsActive)
                                         .Include(l => l.LicenseType)
                                         .FirstOrDefault();

            if (license != null)
            {
                licenseApplicationDetails.LicenseDetails = license;
            }

            List<Document> clientOustandingDocuments = (from d in docs
                                                        where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                        select d).ToList();
            List<Document> businessDocs = (from d in allDocs
                                where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                select d).ToList();

            licenseApplicationDetails.BussinessDocuments = businessDocs;
            licenseApplicationDetails.ClientUploadList = clientDocs;         
            licenseApplicationDetails.BusinessDetails = business;
            licenseApplicationDetails.BusinessMangers = db.BusinessManger.Where(l => l.BusinessId == businessId).Include(l => l.BusinessOperator).ToList();
            licenseApplicationDetails.BusinessEmployees = db.BusinessEmployee.Where(l => l.BusinessId == businessId).ToList();
            licenseApplicationDetails.ClientOustandingDocuments = clientOustandingDocuments;
            licenseApplicationDetails.BusinessTypeList = new SelectList(db.BusinessTypes.Where(b => b.IsDeleted == false && b.IsActive), "BusinessTypeId", "BusinessTypeName", business.BusinessTypeId);
            licenseApplicationDetails.OperationStructureTypeList = new SelectList(db.OperationStructureTypes.Where(o => o.IsDeleted == false && o.IsActive), "OperationStructureTypeId", "OperationStructureTypeName", business.OperationStructureTypeId);
            licenseApplicationDetails.BusinessOperatorList = new SelectList(db.BusinessOperators.ToList().Where(i => i.IsDeleted == false && i.IsActive), "BusinessOperatorId", "BusinessOperatorName");

            SelectList itemTypeList = new SelectList(db.ItemTypes.Where(i => i.IsDeleted == false && i.IsActive), "ItemTypeId", "ItemTypeName", business.ItemTypeId);
            licenseApplicationDetails.ItemTypeList = itemTypeList;

            SelectList itemSubCategoryList = new SelectList(db.ItemSubCategories.Where(i => i.ItemTypeId == business.ItemTypeId && i.IsDeleted == false && i.IsActive == true), "ItemSubCategoryId", "ItemSubCategoryName", business.ItemTypeId);
            licenseApplicationDetails.ItemSubCategoryList = itemSubCategoryList;

            SelectList titleDeedTypeList = new SelectList(db.TitleDeedTypes.Where(t => t.IsDeleted == false && t.IsActive), "TitleDeedTypeId", "TitleDeedTypeName", business.TitleDeedTypeId);
            licenseApplicationDetails.TitleDeedTypeList = titleDeedTypeList;
          
            return View(licenseApplicationDetails);
        }

        // POST: /Business/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit(LicenseApplicationDetails licenseApplicationDetails, IEnumerable<HttpPostedFileBase> files, string itemTypeValue, string CommentData, string OperatorTableData, string EmployeeTableData, string EditOperatorTableData, string EditEmployeeTableData)
        {
            Initialise();
            char[] spearator = { '*' };
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            var docs = new List<Document>();
            Business business = licenseApplicationDetails.BusinessDetails;
            business.Client = null;
            business.ItemType = null;

            if (itemTypeValue == "Item2")
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive == true && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Where(d => d.IsActive && d.IsDeleted == false
                && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management).Include(d => d.DocumentType).ToList();
            }

            var client = db.Clients.Find(business.ClientId);
            var clientDocs = db.FileUploads.Include(d => d.Document).Include(d => d.Document.DocumentType)
                                           .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId)
                                           .ToList();
            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName).ToList();

            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser. 

            #region Upload
            if (files != null && files.Any())
            {
                int counter = 0;
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null)
                    {
                        var length = file.ContentLength;
                        var bytesP = new byte[length];
                        file.InputStream.Read(bytesP, 0, length);
                        Byte[] bytes = bytesP;
                        var Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                        string comment = "";

                        if (CommentData != null && CommentData != "")
                        {
                            char[] spearator2 = { '*' };
                            String[] Commentlist = CommentData.Split(spearator2, StringSplitOptions.None);
                            Commentlist = Commentlist.Where(t => t != ",").ToArray();

                            if (Commentlist[counter].ToString() != "N/A")
                            {
                                comment = Commentlist[counter].ToString();
                            }
                        }
                        SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(bytes, file.FileName, "Business");
                        _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, client.ClientId
                        , client.Fullname, Uploaddoc[counter].DocumentId, business.BusinessId, comment, Users.Username);
                        counter++;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
            #endregion

            clientDocs = db.FileUploads.Include(d => d.Document)
                                      .Where(d => d.ClientId == business.ClientId && d.referenceId == business.BusinessId && d.Document.DocumentTypeId == documentTypeId)
                                      .ToList();

            if (clientDocs.Count() >= docs.Count())
            {
                business.BusinessStatusId = db.Status.Where(s => s.StatusKey == StatusKeys.BusinessApproved)
                                                     .Select(s => s.StatusId)
                                                     .FirstOrDefault();
                TempData["Success"] = "Saved Successfully!";
            }
            else
            {
                OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName).ToList();

                ViewBag.OustandingDocuments = OustandingDocuments;
                TempData["Success"] = "Business Saved, following documents outstanding.";
            }

            var local = db.Set<Business>().Local.FirstOrDefault(l => l.BusinessId == business.BusinessId);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            db.Entry(business).State = EntityState.Modified;
            db.SaveChanges();

            // Manager
            if (OperatorTableData != "")
            {
                BusinessManger BusinessManger = new BusinessManger();
                String[] OperatorTableDatalist = OperatorTableData.Split(spearator, StringSplitOptions.None);
                OperatorTableDatalist = OperatorTableDatalist.Where(t => t != "").ToArray();
                foreach (var item in OperatorTableDatalist)
                {
                    char[] separator = { ',' };
                    String[] Data = item.Split(separator, StringSplitOptions.None);
                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                    BusinessManger.BusinessId = business.BusinessId;
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

            // Edit Manager
            if (EditOperatorTableData != "")
            {
                String[] OperatorTableDatalist = EditOperatorTableData.Split(spearator, StringSplitOptions.None);
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

            // Employee
            if (EmployeeTableData != "")
            {
                BusinessEmployee BusinessEmployee = new BusinessEmployee();
                String[] EmployeeTableDatalist = EmployeeTableData.Split(spearator, StringSplitOptions.None);
                EmployeeTableDatalist = EmployeeTableDatalist.Where(t => t != "").ToArray();
                foreach (var item in EmployeeTableDatalist)
                {
                    char[] separator = { ',' };
                    String[] Data = item.Split(separator, StringSplitOptions.None);
                    Data = Data.Where(t => t != "" && t != " ").ToArray();
                    BusinessEmployee.BusinessId = business.BusinessId;
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

            // Edit Employee
            if (EditEmployeeTableData != "")
            {
                String[] EmployeeTableDatalist = EditEmployeeTableData.Split(spearator, StringSplitOptions.None);
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

            ViewData["Documents"] = (from d in docs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();
            ViewData["UploadList"] = clientDocs;
            return RedirectToAction("Index");
        }

        // GET: /Business/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Business business = db.Businesses.Where(b => b.BusinessId == id)
                .Include(c => c.Client)
                .Include(o => o.ItemType)
                .Include(b => b.BusinessType)
                .Include(t => t.TitleDeedType)
                .Include(b => b.OperationStructureType)
                .FirstOrDefault();

            if (business == null)
            {
                return HttpNotFound();
            }
            return View(business);
        }

        // POST: /Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Business business = db.Businesses.Find(id);

                if (business != null)
                {
                    business.IsActive = false;
                    business.IsDeleted = true;
                    db.Entry(business).State = EntityState.Modified;

                    // LM.20141110a - Delete all licenses related to this business
                    var licenses = db.Licenses.Where(l => l.BusinessId == business.BusinessId).ToList();
                    if (licenses.Count > 0)
                    {
                        foreach (var license in licenses)
                        {
                            license.IsActive = false;
                            license.IsDeleted = true;
                            db.Entry(license).State = EntityState.Modified;

                            // LM.20141110a - Delete inspections related to the license
                            var inspectionRequests = db.InspectionRequests.Where(i => i.LicenseId == license.LicenseId).ToList();
                            var inspectionResponses = db.InspectionResponse.Where(i => i.LicenseId == license.LicenseId).ToList();
                            if (inspectionRequests.Count > 0)
                            {
                                foreach (var inspectionRequest in inspectionRequests)
                                {
                                    inspectionRequest.IsActive = false;
                                    inspectionRequest.IsDeleted = true;
                                    db.Entry(inspectionRequest).State = EntityState.Modified;
                                }
                            }

                            if (inspectionResponses.Count > 0)
                            {
                                foreach (var inspectionResponse in inspectionResponses)
                                {
                                    inspectionResponse.IsActive = false;
                                    inspectionResponse.IsDeleted = true;
                                    db.Entry(inspectionResponse).State = EntityState.Modified;
                                }
                            }
                        }


                    }

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return View("Error");
            }

            //db.Businesses.Remove(business);

            return RedirectToAction("Index");
        }

        /// <param name="id">File Id</param>
        /// <param name="file"> File name</param>
        ///  <param name="clientId"> File name</param>
        /// <returns></returns>
 
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
                if ((User.IsInRole("Licensing Administrator")) || (User.IsInRole("Licensing Manager")))
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

        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult DeleteFile(int? id, int businessId)
        {
            FileUpload uploads = db.FileUploads.Find(id);

            if (uploads != null)
            {
                uploads.IsDeleted = true;
                uploads.IsActive = false;
                db.Entry(uploads).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details", businessId);
        }

        public ActionResult ReloadItemSubCategory(int itemTypeId)
        {


           var subCategories = db.ItemSubCategories.ToList().Where(i => i.IsDeleted == false
                                                                            && i.IsActive
                                                                            && i.ItemTypeId == itemTypeId);

            return Json(subCategories, JsonRequestBehavior.AllowGet);
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
