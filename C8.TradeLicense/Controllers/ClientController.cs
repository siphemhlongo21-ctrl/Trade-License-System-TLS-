using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using User = C8.TradeLicense.Models.User;
using C8.TradeLicense.Keys;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Data.Entity;
using System.Text;
using C8.TradeLicense.ViewModels;
using Newtonsoft.Json.Linq;
using PagedList;
using C8.TradeLicense.Helpers;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace C8.TradeLicense.Controllers
{
    public class ClientController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private FileHelpers _fileHelpers;
        // private C8_Common_CoreDataContext coreDataContext = new C8_Common_CoreDataContext();

        public ClientController()
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

        // GET: /Client/
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {

            ViewData["ClientBusinessesList"] = db.Businesses.ToList();
            var template =
                new StreamReader(Server.MapPath("~/Views/Shared/_EmailTemplatePartial.cshtml"));
            var stringTemplate = template.ReadToEnd();
            //EmailHelper.ConstructEmailBody("test", "test");
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
            }

            ViewBag.CurrentFilter = inputSearch;
            ViewBag.selecetedSearch = selectedSearch;

            var clients = db.Clients.Include(c => c.CreatedByUser)
                                   .Include(c => c.ModifiedByUser)
                                   //.Include(c => c.ClientUser)
                                   .Include(c => c.ClientStatus)
                                   .Where(c => c.IsDeleted == false && c.IsActive)
                                   .ToList();
            if (selectedSearch != null)
            {
                switch (selectedSearch)
                {
                    case "ClientName":
                        clients = clients.Where(l => (l.Name.ToLower() + " " + l.Surname.ToLower()).Contains(inputSearch.ToLower())).ToList();
                        break;
                    case "IDPassportNumber":
                        //Search by reference number
                        clients = clients.Where(l => l.IdentityOrPassportNumber == inputSearch).ToList();
                        break;
                }
            }

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.ClientCount = clients.Count;
            return View(clients.ToPagedList(pageNumber, pageSize));
            //return View(clients.ToList());
        }


        /// <summary>
        /// Search client based in search criteria 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="SearchCriteria"></param>
        /// <param name="inputSearch"></param>
        /// <returns></returns>
        ///   POST: /Client/
        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Index(int? page, string SearchCriteria, string inputSearch)
        {

         
            List<Client> searchResults = null;
            var clients = db.Clients.Include(c => c.CreatedByUser)
                                    .Include(c => c.ModifiedByUser)
                                    .Include(c => c.Name)
                                    .Include(c=> c.Surname)
                                    .Include(c => c.ClientStatus)
                                    .Where(c => c.IsDeleted == false && c.IsActive)
                                    .ToList();

            if (SearchCriteria == "Id")
            {
                //Search by Id No or passport
                var results = db.Clients.Include(c => c.CreatedByUser).Include(c => c.ModifiedByUser).
                    Include(c => c.ClientStatus).Where(c => c.IdentityOrPassportNumber == inputSearch && c.IsDeleted == false && c.IsActive).ToList();
                searchResults = results;
            }
            else
            {
                //Search by client name
                var results = db.Clients
                              .Include(c => c.CreatedByUser)
                              .Include(c => c.ModifiedByUser)
                              //.Include(c => c.ClientUser)
                              .Include(c => c.ClientStatus)
                              .Where(f => (f.Name + " " + f.Surname).Contains(inputSearch) && f.IsDeleted == false && f.IsActive).ToList();

                searchResults = results; //.Where(c => c.Fullname.Contains(inputSearch) && c.IsDeleted == false && c.IsActive);
            }

            if (searchResults.Count <= 0)
            {
                TempData["Error"] = "Client not found! Please try different search criteria.";
            }

            ViewBag.CanAction = CanAction();
            // LM.20141110a - Set parameters for paging
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ////

            return View(searchResults.Count > 0 ? searchResults.ToPagedList(pageNumber, pageSize) : clients.ToPagedList(pageNumber, pageSize));
            //return View(clients.ToList());
        }

        // GET: /Client/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();

            Client client = db.Clients.Find(id);

            if (client == null)
            {
                return HttpNotFound();
            }

            // LM 20141029a - Gets list of required documents based on the document type key.
            int? documentTypeId = db.DocumentTypes?
                                 .Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument)
                                 .Select(d => d.DocumentTypeId).FirstOrDefault();

            List<Document> documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Include(d => d.DocumentType).ToList();
           
            List<FileUpload> clientDocs = db.FileUploads.Include(d => d.Document)
                                         .Where(d => d.ClientId == id && d.referenceId == id && d.Document.DocumentTypeId == documentTypeId)
                                         .ToList();

            List<Document> clientDocuments = (from d in documentType
                                              where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                              select d).ToList();

            // LM.20141031 - Get Outstanding documents for client.
            List<Document> clientOustandingDocuments = (from d in documentType
                                                        where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                        select d).ToList();

            IEnumerable<Business> businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client).Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == id).ToList();       

            licenseApplicationDetails.ClientOustandingDocuments = clientOustandingDocuments;         
            licenseApplicationDetails.CustomerDetails = client;       
            licenseApplicationDetails.ClientBusinessesList = businesses;
            // LM.20141029 - Gets list of required documents based on the document type key
            licenseApplicationDetails.ClientDocuments = clientDocuments;
            licenseApplicationDetails.ClientUploadList= clientDocs;

            return View(licenseApplicationDetails);
        }

        // GET: /Client/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Create()
        {
      
            // LM-20141029a Gets list of required documents based on the document type key.
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();

            ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).ToList()
                                    .Where(d => d.IsActive && d.IsDeleted == false);
            ViewData["Client"] = ViewData["Client"];
            ViewBag.CustomerType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName");
            ViewBag.Nationality = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName");
            ViewBag.IndividualType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName");

            ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town"});
            ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town" });
            ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" });
            ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" });

            //var test = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).ToList();
            TempData["Success"] = null;
            TempData["Error"] = null;

            return View(new Client());
        }

        // POST: /Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "ClientId,IdentityOrPassportNumber,Name,Surname,ResidentialAddress1,ResidentialAddress2,ResidentialAddress3,ResidentialAddressCode,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,TelephoneNumber,CellphoneNumber,AltCellphoneNumber,FaxNumber,EmailAddress,UserId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,NationalityOther,ExpiryPermit,CustomerType,RegNumber,BusinessName,SameAs ")] Client client, IEnumerable<HttpPostedFileBase> files, string CommentData, string Nationality, string IndividualType, string TelephoneNumberCode, string TelephoneNumber, string FaxNumbercode, string FaxNumber, string CustomerType)
        {
            Initialise();
            client.Nationality = Nationality;
            client.Individual = IndividualType;
            client.CustomerType = CustomerType;
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            bool isRSA = false;
            bool isForeign = false;

            // LM.20141029a - Gets list of required documents based on the document type key
            List<Document> docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentKey != "Other" && d.DocumentKey != TLKeys.Events_Management && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            var clientDocs = db.FileUploads.Where(f => f.ClientId == client.ClientId && f.referenceId == client.ClientId && f.Document.DocumentTypeId == documentTypeId).Include(d => d.Document.DocumentType).ToList();
            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                                     ).ToList();

            var idNumber = "";
            if (client.IdentityOrPassportNumber != null)
            {
                idNumber = db.Clients.Where(c => c.IdentityOrPassportNumber == client.IdentityOrPassportNumber).Select(c => c.IdentityOrPassportNumber).FirstOrDefault();
            }
            else
            {
                idNumber = db.Clients.Where(c => c.RegNumber == client.RegNumber).Select(c => c.RegNumber).FirstOrDefault();
            }

            #region IdValidation
            if (CustomerType == "Business")
            {
                isRSA = true;
            }
            else
            {
                if (Nationality == "South African National")
                {
                    var rsaid = Regex.Match(client.IdentityOrPassportNumber, @"(?<Year>[0-9][0-9])(?<Month>([0][1-9])|([1][0-2]))(?<Day>([0-2][0-9])|([3][0-1]))(?<Gender>[0-9])(?<Series>[0-9]{3})(?<Citizenship>[0-9])(?<Uniform>[0-9])(?<Control>[0-9])");

                    if (rsaid.Success)
                    {
                        isRSA = true;
                    }
                }
                else
                {
                    isForeign = true;
                }
            }
            #endregion

            if (isRSA == true || isForeign == true)
            {
                if (ModelState.IsValid)
                {
                    if (idNumber == null) // No duplicates
                    {
                        client.IsActive = true;
                        client.IsDeleted = false;
                        client.IsLocked = false;

                        client.FaxNumber = FaxNumbercode + FaxNumber;
                        client.TelephoneNumber = TelephoneNumberCode + TelephoneNumber;
                        client.ClientStatusId = db.Status.Where(s => s.StatusKey == StatusKeys.PendingClientClearance).Select(s => s.StatusId).FirstOrDefault();

                        db.Clients.Add(client);
                        db.SaveChanges();
                        var clientUser = db.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);

                        #region Upload
                        if (files != null && files.Any())
                        {
                            int counter = 0;
                            foreach (HttpPostedFileBase file in files)
                            {
                                if (file != null)
                                {
                                    int fileLen = file.ContentLength;
                                    byte[] fileData = null;
                                    using (var binaryReader = new BinaryReader(file.InputStream))
                                    {
                                        fileData = binaryReader.ReadBytes(file.ContentLength);
                                    }


                                    var Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

                                    var comment = "";
                                    if (CommentData != null)
                                    {
                                        if (CommentData != "")
                                        {
                                            char[] spearator = { '*' };
                                            String[] Commentlist = CommentData.Split(spearator, StringSplitOptions.None);
                                            Commentlist = Commentlist.Where(t => t != ",").ToArray();

                                            if (Commentlist[counter].ToString() != "N/A")
                                            {
                                                comment = Commentlist[counter].ToString();
                                            }
                                        }
                                    }


                                    SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, file.FileName, "Client");
                                    _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, client.ClientId
                                    , client.Fullname, Uploaddoc[counter].DocumentId, client.ClientId, comment, Users.Username);
                                }
                                counter++;
                            }
                            ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId);
                        }
                        else
                        {
                            TempData["Error"] = "Supporting Documents required!.";
                            client = clientUser;
                            ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId);
                        }
                        #endregion

                        clientDocs = db.FileUploads.Where(f => f.ClientId == client.ClientId && f.referenceId == client.ClientId && f.Document.DocumentTypeId == documentTypeId).ToList();

                        if (clientDocs.Count() == docs.Count())
                        {
                            clientUser.ClientStatusId = db.Status.Where(s => s.StatusKey == StatusKeys.ClientApproved).Select(s => s.StatusId).FirstOrDefault();
                            db.Entry(clientUser).State = EntityState.Modified;
                            db.SaveChanges();

                            ViewData["Documents"] = (from d in docs
                                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                     select d).ToList();

                            ViewData["ClientData"] = client;
                            TempData["Success"] = "Saved Successfully!";
                        }
                        else
                        {
                            OustandingDocuments = (from d in docs
                                                   where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                   select d.DocumentName
                                              ).ToList();

                            ViewBag.OustandingDocuments = OustandingDocuments;
                            ViewData["ClientData"] = client;
                            TempData["Success"] = "Client Saved, following documents outstanding.";
                            ViewData["Documents"] = (from d in docs
                                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                     select d).ToList();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).ToList();
                        TempData["Error"] = "Customer exists!";
                        ViewData["Documents"] = (from d in docs
                                                 where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                 select d).ToList();
                    }
                }
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                TempData["Error"] = "Not a valid Id Number!";
                ViewData["Documents"] = (from d in docs
                                         where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                         select d).ToList();
            }

            docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            ViewData["Documents"] = (from d in docs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();
            ViewData["Client"] = client;
            ViewData["UploadList"] = clientDocs;

            if (ModelState.IsValid == false || idNumber != null || isRSA == false || isForeign == false)
            {
                ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town", client.PostalAddress3 }, client.PostalAddress3);
                ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town", client.ResidentialAddress3 }, client.ResidentialAddress3);
                ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area", client.PostalAddress2 }, client.PostalAddress2);
                ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area", client.ResidentialAddress2 }, client.ResidentialAddress2);

                ViewBag.CustomerType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName", client.CustomerType);
                ViewBag.Nationality = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", client.Nationality);
                ViewBag.IndividualType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", client.Individual);
                return View(client);
            }

            return RedirectToAction("Edit", new { id = client.ClientId });
        }






        // GET: /Client/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {           
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LicenseApplicationDetails licenseApplicationDetails = new LicenseApplicationDetails();
            Client client = db.Clients.Find(id);

            if (client == null)
            {
                return HttpNotFound();
            }

        
            int? documentTypeId = db.DocumentTypes?
                               .Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument)
                               .Select(d => d.DocumentTypeId).FirstOrDefault();

            List<Document> documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
           
            List<FileUpload> clientDocs = db.FileUploads.Include(d => d.Document)
                                          .Where(d => d.ClientId == id && d.referenceId == id && d.Document.DocumentTypeId == documentTypeId)
                                          .ToList();

            List<Document> clientDocuments = (from d in documentType
                                              where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                              select d).ToList();

            List<Business> businesses = db.Businesses.Include(b => b.ItemType)
                                     .Include(b => b.BusinessType)
                                     .Include(b => b.Client).Include(s => s.BusinessStatus)
                                     .Include(b => b.CreatedByUser)
                                     .Include(b => b.ModifiedByUser)
                                     .Include(b => b.OperationStructureType)
                                     .Include(b => b.TitleDeedType)                               
                                     .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == id).ToList();
         
            // LM.20141031 - Get Outstanding documents for client.
            List<Document> docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentKey != "Other" && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
           
            IEnumerable<Document> clientOustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d
                              ).ToList();

            licenseApplicationDetails.CustomerTypeList = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName", client.CustomerType);
            licenseApplicationDetails.NationalityList = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", client.Nationality);
            licenseApplicationDetails.IndividualTypeList = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", client.Individual);
            licenseApplicationDetails.ClientOustandingDocuments = clientOustandingDocuments;
            licenseApplicationDetails.CustomerDetails = client;
            licenseApplicationDetails.ClientBusinessesList = businesses;
            // LM.20141029 - Gets list of required documents based on the document type key
            licenseApplicationDetails.ClientDocuments = clientDocuments;

            licenseApplicationDetails.ClientUploadList = clientDocs;

            return View(licenseApplicationDetails);
        }

        // POST: /Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "ClientId,ClientUserId, ClientStatusId,IdentityOrPassportNumber,Name,Surname,ResidentialAddress1,ResidentialAddress2,ResidentialAddress3,ResidentialAddressCode,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,TelephoneNumber,CellphoneNumber,AltCellphoneNumber,FaxNumber,EmailAddress,UserId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,NationalityOther,ExpiryPermit,CustomerType,RegNumber,BusinessName,SameAs ")] string Nationality, string IndividualType, string CustomerType, IEnumerable<HttpPostedFileBase> files, string CommentData, LicenseApplicationDetails licenseApplicationDetails)
        {
            Initialise();
            int DocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            List<Document> docs = db.Documents.Where(d => d.DocumentTypeId == DocumentTypeId && d.DocumentKey != "Other" && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

            List<Business> businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client)
                                          .Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == licenseApplicationDetails.CustomerDetails.ClientId).ToList();

            List<FileUpload> clientDocs = db.FileUploads.Where(d => d.ClientId == licenseApplicationDetails.CustomerDetails.ClientId && d.referenceId == licenseApplicationDetails.CustomerDetails.ClientId && d.Document.DocumentTypeId == DocumentTypeId)
                                           .Include(d => d.Document)
                                           .ToList();


            clientDocs = db.FileUploads.Where(d => d.ClientId == licenseApplicationDetails.CustomerDetails.ClientId && d.referenceId == licenseApplicationDetails.CustomerDetails.ClientId && d.Document.DocumentTypeId == DocumentTypeId).Include(d => d.Document).ToList();

            List<Document> docsupload = db.Documents.Where(d => d.DocumentTypeId == DocumentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

            List<Document> uploadedDocs = (from d in docsupload
                                           where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                           select d).ToList();

            List<Document> clientOustandingDocuments = (from d in docs
                                                        where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                        select d).ToList();


            if (ModelState.IsValid)
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.

                if (files != null && files.Any())
                {
                    //TG20141028 - FileUpload
                    int counter = 0;
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            string CREATION_DATE = "";
                            string fileUrl = "";
                            int fileLen = file.ContentLength;
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(file.ContentLength);
                            }
                            List<Document> Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == DocumentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                           
                            string comment = "";
                            if (CommentData != null)
                            {
                                if (CommentData != "")
                                {
                                    char[] spearator = { '*' };

                                    string[] Commentlist = CommentData.Split(spearator, StringSplitOptions.None);
                                    Commentlist = Commentlist.Where(t => t != ",").ToArray();

                                    if (Commentlist[counter].ToString() != "N/A")
                                    {
                                        comment = Commentlist[counter].ToString();
                                    }
                                }

                            }
                            SharePointDocument sharePointDocument = _fileHelpers.UploadFileToSharepoint(fileData, file.FileName, "Client");
                            _fileHelpers.SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, licenseApplicationDetails.CustomerDetails.ClientId
                            , licenseApplicationDetails.CustomerDetails.Fullname, Uploaddoc[counter].DocumentId, licenseApplicationDetails.CustomerDetails.ClientId, comment, Users.Username);
                            counter++;

                        }
                        else
                        {
                            counter++;
                        }


                    }

                }


                clientDocs = db.FileUploads.Where(d => d.ClientId == licenseApplicationDetails.CustomerDetails.ClientId && d.referenceId == licenseApplicationDetails.CustomerDetails.ClientId && d.Document.DocumentTypeId == DocumentTypeId).Include(d => d.Document).ToList();


                if (clientDocs.Count() >= docs.Count())
                {
                    licenseApplicationDetails.CustomerDetails.ClientStatusId =
                    db.Status.Where(s => s.StatusKey == StatusKeys.ClientApproved).Select(s => s.StatusId).
                        FirstOrDefault();

                    licenseApplicationDetails.CustomerDetails = licenseApplicationDetails.CustomerDetails;
                    TempData[LicenseApplicationDetails.SuccessKey] = "Updated Successfully!";
                }
                else
                {

                    licenseApplicationDetails.ClientOustandingDocuments = clientOustandingDocuments;
                    licenseApplicationDetails.ClientUploadList = clientDocs;
                    licenseApplicationDetails.ClientDocuments = (from d in docs
                                                                 where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                                 select d).ToList();

                    TempData[LicenseApplicationDetails.SuccessKey] = "Updated Successfully!";
                }

                Client local = db.Set<Client>().Local.FirstOrDefault(l => l.ClientId == licenseApplicationDetails.CustomerDetails.ClientId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                licenseApplicationDetails.CustomerDetails.Nationality = Nationality;
                licenseApplicationDetails.CustomerDetails.CustomerType = CustomerType;
                licenseApplicationDetails.CustomerDetails.Individual = IndividualType;

                Client client = licenseApplicationDetails.CustomerDetails;
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();

                licenseApplicationDetails.ClientOustandingDocuments = clientOustandingDocuments;
                licenseApplicationDetails.ClientUploadList = clientDocs;
                licenseApplicationDetails.ClientDocuments = (from d in docs
                                                             where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                             select d).ToList();

            }
            /// exclude other - to be done
            // LM 20141029a - Gets list of required documents based on the document type key.
            licenseApplicationDetails.ClientDocuments = (from d in docs
                                                         where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                         select d).ToList(); //db.Documents.Where(d => d.DocumentTypeId == documentTypeId);

            licenseApplicationDetails.ClientBusinessesList = businesses;
            licenseApplicationDetails.ClientUploadList = clientDocs;

            licenseApplicationDetails.CustomerTypeList = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName", licenseApplicationDetails.CustomerDetails.CustomerType);
            licenseApplicationDetails.NationalityList = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", licenseApplicationDetails.CustomerDetails.Nationality);
            licenseApplicationDetails.IndividualTypeList = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", licenseApplicationDetails.CustomerDetails.Individual);


            return View(licenseApplicationDetails);
        }


        // GET: /Client/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: /Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Manager" + "," + "System Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);

            if (client != null)
            {
                client.IsActive = false;
                client.IsDeleted = true;
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();

                // LM.20141110a - Delete all businesses for the client
                var clientbusinesses = db.Businesses.Where(b => b.ClientId == client.ClientId).ToList();
                if (clientbusinesses.Count > 0)
                {
                    foreach (var business in clientbusinesses)
                    {
                        business.IsActive = false;
                        business.IsDeleted = true;
                        db.Entry(business).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            //db.Clients.Remove(client);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Redirects to create client bussiness view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult RegisterClientBusiness()
        {

            var clientData = (Client)ViewData["ClientData"];

            if (null != clientData)
            {
                return RedirectToAction("Create", "Business", new { id = clientData.ClientId });

            }
            else
            {
                TempData["Error"] = "Please register client first!";
                return View(clientData);
            }

        }



        /// <param name="id">File Id</param>
        /// <param name="file"> File name</param>
        ///  <param name="clientId"> File name</param>
        /// <returns></returns>
        public void Download(string filename, int formid, int docid)
        {
            HttpWebResponse response = null;
            Encoding enc = System.Text.Encoding.GetEncoding(1252);
         
            var serviceUrl = ConfigurationManager.AppSettings["serviceUrl"];
            var folderName = filename + formid + docid;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{  \"Search_Request\": {    \"FolderName\": \"" + folderName + "\"  }}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                response = (HttpWebResponse)httpWebRequest.GetResponse();
                var messages = "";
                if (response != null)
                {
                    StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc);
                    messages = loResponseStream.ReadToEnd();
                    //GenerateQR(messages, AccountNo);
                }

               
            }
          
            catch (System.Web.Services.Protocols.SoapException ex)
                {
                    throw ex;
                }
            }
          

        
        /// <summary>
        /// Checks for if client exists in the database
        /// </summary>
        /// <param name="idNo">Client ID/Passport Number</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsClientIdNumberExist(string idNo)
        {
            var idNumber =
                    db.Clients.Where(c => c.IdentityOrPassportNumber == idNo).Select(c => c.IdentityOrPassportNumber).
                        First();

            return Json(idNumber == null);

        }

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
