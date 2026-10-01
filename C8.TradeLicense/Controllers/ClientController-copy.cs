using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.TradeLicenseWebService;
using PagedList;
using User = C8.TradeLicense.Models.User;
using C8.TradeLicense.Keys;
using System.Collections;

namespace C8.TradeLicense.Controllers
{
    public class ClientController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        // private C8_Common_CoreDataContext coreDataContext = new C8_Common_CoreDataContext();

        public ClientController()
        {
            // JK.20140906a - Instantiate the IdentityManager in the contructor and pass the DbContext.
            IdentityManager = new IdentityManager(db);
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
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            // LM 20141029a - Gets list of required documents based on the document type key.
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            var documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId).Include(d => d.DocumentType).ToList();
            var clientDocs =
                db.FileUploads.Include(d => d.Document)
                    .Where(d => d.ClientId == id && d.referenceId == id && d.Document.DocumentTypeId == documentTypeId)
                    .ToList();

            var businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client).Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                     
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == id).ToList();

            // LM.20141031 - Get Outstanding documents for client.
            var OustandingDocuments = (from d in documentType
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                              ).ToList();

            ViewBag.OustandingDocuments = OustandingDocuments;
            ViewData["ClientData"] = client;
            ViewData["ClientBusinessesList"] = businesses;
            // LM.20141029 - Gets list of required documents based on the document type key
            ViewData["Documents"] = (from d in documentType
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();
            ViewData["UploadList"] = clientDocs;

            return View(client);
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
        public ActionResult Create([Bind(Include = "ClientId,IdentityOrPassportNumber,Name,Surname,ResidentialAddress1,ResidentialAddress2,ResidentialAddress3,ResidentialAddressCode,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,TelephoneNumber,CellphoneNumber,AltCellphoneNumber,FaxNumber,EmailAddress,UserId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,NationalityOther,ExpiryPermit,CustomerType,RegNumber,BusinessName,SameAs ")] Client client, IEnumerable<HttpPostedFileBase> files,string CommentData, string Nationality,string IndividualType,string TelephoneNumberCode, string TelephoneNumber, string FaxNumbercode, string FaxNumber,string CustomerType)
        {
       
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
                idNumber =
                      db.Clients.Where(c => c.IdentityOrPassportNumber == client.IdentityOrPassportNumber).Select(
                          c => c.IdentityOrPassportNumber).FirstOrDefault();
            }
            else
            {
                 idNumber =
                   db.Clients.Where(c => c.RegNumber== client.RegNumber).Select(
                       c => c.RegNumber).FirstOrDefault();
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
            if (isRSA==true || isForeign==true)
            {
                if (ModelState.IsValid)
                {
                    // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                    IdentityManager.CurrentUser(User);
                    //Check For Duplicate Entries 


                    if (idNumber == null) // No duplicates
                    {
                        //var user = new User()
                        //    {
                        //        FirstName = client.Name,
                        //        LastName = client.Surname,
                        //        Username = "",
                        //        Password = "",
                        //        EmailAddress = client.EmailAddress,
                        //        Mobile =  client.CellphoneNumber,
                        //        LandLine = client.TelephoneNumber ?? string.Empty,                               
                        //        Fax = client.FaxNumber ?? string.Empty,
                        //        IpAddress = "",
                        //        IsActive = true,
                        //        IsDeleted = false,
                        //        IsLocked = false
                        //    };

                        //db.Users.Add(user);
                        //db.SaveChanges();

                        // LM.20141029 - Get inserted primary key from Users table
                        //User u = new User();
                        //u = db.Users.Find(user.UserId);

                        //client.ClientUserId = u.UserId;
                        
                        client.IsActive = true;
                        client.IsDeleted = false;
                        client.IsLocked = false;
                        
                        client.FaxNumber = FaxNumbercode + FaxNumber;
                        client.TelephoneNumber = TelephoneNumberCode + TelephoneNumber;
                        client.ClientStatusId =
                            db.Status.Where(s => s.StatusKey == StatusKeys.PendingClientClearance).Select(s => s.StatusId).
                                FirstOrDefault();

                        db.Clients.Add(client);
                        db.SaveChanges();
                        var clientUser = db.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);

                        #region Upload
                        if (files.Any())
                        {

                            //L.M 20141118a - FileUpload (Saving to sharepoint)
                            int counter = 0;
                            var service = new SharepointETMSoapClient();
                            //service.Open();
                            foreach (HttpPostedFileBase file in files)
                            {
                                if (file != null)
                                {
                                  
                                    var fileUrl = "http://10.10.9.71:168/Documents/Trade License/Clients/";
                                    int fileLen = file.ContentLength;
                                    byte[] fileData = null;
                                    using (var binaryReader = new BinaryReader(file.InputStream))
                                    {
                                        fileData = binaryReader.ReadBytes(file.ContentLength);
                                    }

                                    int position = file.FileName.LastIndexOf(".", StringComparison.Ordinal);
                                    string filename = file.FileName.Substring(0, position);
                                    string extension = file.FileName.Substring(position + 1);

                                    //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                                    const string regExp = @"[^\w\d]";
                                    string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                                    string name = Regex.Replace(filename, regExp, "_");
                                    string contentType = file.ContentType;
                                   var Uploaddoc = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                                    var comment = "";
                                    if (CommentData != null )
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
                                    try
                                    {
                                        fileUrl = fileUrl + uploadName;

                                        FileUpload fileupload = new FileUpload
                                        {
                                            FileName = uploadName,
                                            FilePath = fileUrl,
                                            ClientId = client.ClientId,
                                            Comments = comment,
                                            ClientName = client.Fullname,
                                            DocumentId = Uploaddoc[counter].DocumentId,
                                            referenceId = client.ClientId,
                                            IsActive = true,
                                            IsDeleted = false,
                                            IsLocked = false
                                        };
                                        db.FileUploads.Add(fileupload);
                                        db.SaveChanges();

                                        var lastFileUploadId = fileupload.FileUploadId;

                                        //Generate Unique Document ID or use primary key from database
                                        string UniqueID = client.Name + "_" + client.Surname + "_" + lastFileUploadId;
                                        string trade = "Trade License" + " " + "-" + " " + DateTime.Now.Year;
                                        string refNo = client.ClientId.ToString();
                                        const string des = "Client Document";
                                        const string application = @"Trade License\Clients";
                                        string metadata = "Title:" + trade + ";ReferenceNo:" + refNo + ";Description:" + des + ";Application:" + application;
                                        //string metadata = "Title:" + trade + ";ReferenceNo:" + refNo + ";Description:" + des;

                                        // Send posted file to SharePoint
                                        var status = service.Upload(name, UniqueID, extension, fileData, metadata);                                     

                                    }
                                    catch (System.Web.Services.Protocols.SoapException ex)
                                    {
                                        throw ex;
                                    }
                                    counter++;

                                }
                                else
                                {
                                    counter++;
                                }

                            }
                            service.Close();
                            // LM.20141029 - Gets list of required documents based on the document type key
                            ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId);
                        }
                        else
                        {
                            TempData["Error"] = "Supporting Documents required!.";
                            client = clientUser;
                            // LM.20141029 - Gets list of required documents based on the document type key
                            ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId);
                        }
                        #endregion Sharepoint File upload

                        clientDocs = db.FileUploads.Where(f => f.ClientId == client.ClientId && f.referenceId == client.ClientId && f.Document.DocumentTypeId == documentTypeId).ToList();

                        if (clientDocs.Count() == docs.Count())
                        {
                            clientUser.ClientStatusId =
                            db.Status.Where(s => s.StatusKey == StatusKeys.ClientApproved).Select(s => s.StatusId).
                                FirstOrDefault();
                            db.Entry(clientUser).State = EntityState.Modified;
                            db.SaveChanges();

                            // LM.20141029 - Gets list of required documents based on the document type key
                            ViewData["Documents"] = (from d in docs
                                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                     select d).ToList();

                            ViewData["ClientData"] = client;
                            TempData["Success"] = "Saved Successfully!";

                            // TODO: Testing Email Service
                            #region Construct email
                            //var applicationId =
                            //    coreDataContext.pr_SELECT_ApplicationByKey("trade_license_application", true, false)
                            //        .First()
                            //        .ApplicationID;

                            ////LF2014a 
                            ////User email helper to get well formatted email.
                            //var mail = new EmailHelper();
                            //var body = mail.ConstructEmailBody(client.Fullname, "Your application has been successful proccessed and sent for inspection."); 
                            //coreDataContext.pr_INSERT_EmailQueue(applicationId, 5,
                            //                            "user@example.com", null, null,
                            //                            "TLS: Client Registration", body, true, 0,
                            //                             client.IdentityOrPassportNumber, 2, false);

                            //coreDataContext.SubmitChanges();
                            #endregion email
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
                            // LM.20141029 - Gets list of required documents based on the document type key
                            ViewData["Documents"] = (from d in docs
                                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                     select d).ToList();
                            return RedirectToAction("Index");
                        }

                        //ViewData["Client"] = client;
                        //ViewData["UploadList"] = clientDocs;
                        //TempData["Success"] = "Saved Successfully!";
                    }
                    else
                    {
                      docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).ToList();
                        TempData["Error"] = "Customer exists!";
                        // LM.20141029 - Gets list of required documents based on the document type key
                        ViewData["Documents"] = (from d in docs
                                                 where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                                 select d).ToList();
                        //ViewData["UploadList"] = clientDocs;
                    }

                }
            }
            else
            {
                docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
                TempData["Error"] = "Not a valid Id Number!";
                // LM.20141029 - Gets list of required documents based on the document type key
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
                // JK.20200110a - Populate viewdata for postback.
                ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town" , client.PostalAddress3 },client.PostalAddress3);
                ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town", client.ResidentialAddress3 },client.ResidentialAddress3);
                ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area", client.PostalAddress2 },client.PostalAddress2);
                ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area", client.ResidentialAddress2 },client.ResidentialAddress2);

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

            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            // LM 20141029a - Gets list of required documents based on the document type key.


            ViewBag.CustomerType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName", client.CustomerType);
            ViewBag.Nationality = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", client.Nationality);
            ViewBag.IndividualType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", client.Individual);

            ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town" });
            ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town" });
            ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" });
            ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" });
            var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            var documentType = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            var clientDocs =
                db.FileUploads.Include(d => d.Document)
                    .Where(d => d.ClientId == id && d.referenceId == id && d.Document.DocumentTypeId == documentTypeId)
                    .ToList();

            var businesses = db.Businesses.Include(b => b.ItemType)
                                     .Include(b => b.BusinessType)
                                     .Include(b => b.Client).Include(s => s.BusinessStatus)
                                     .Include(b => b.CreatedByUser)
                                     .Include(b => b.ModifiedByUser)
                                     .Include(b => b.OperationStructureType)
                                     .Include(b => b.TitleDeedType)
                               
                                     .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == id).ToList();

            // LM.20141031 - Get Outstanding documents for client.
            var docs = db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.DocumentKey != "Other" && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();
            var OustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName
                              ).ToList();

            ViewBag.OustandingDocuments = OustandingDocuments;
            ViewData["ClientData"] = client;
            ViewData["ClientBusinessesList"] = businesses;
            // LM.20141029 - Gets list of required documents based on the document type key
            ViewData["Documents"] = (from d in documentType
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList();
            ViewData["UploadList"] = clientDocs;



            return View(client);
        }

        // POST: /Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "ClientId,ClientUserId, ClientStatusId,IdentityOrPassportNumber,Name,Surname,ResidentialAddress1,ResidentialAddress2,ResidentialAddress3,ResidentialAddressCode,PostalAddress1,PostalAddress2,PostalAddress3,PostalAddressCode,TelephoneNumber,CellphoneNumber,AltCellphoneNumber,FaxNumber,EmailAddress,UserId,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,NationalityOther,ExpiryPermit,CustomerType,RegNumber,BusinessName,SameAs ")] Client client, string Nationality, string IndividualType,string CustomerType, IEnumerable<HttpPostedFileBase> files, string CommentData)
        {
         
            
            var DocumentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument).Select(d => d.DocumentTypeId).FirstOrDefault();
            var docs = db.Documents.Where(d => d.DocumentTypeId == DocumentTypeId && d.DocumentKey != "Other" && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

            var businesses = db.Businesses.Include(b => b.ItemType)
                                          .Include(b => b.BusinessType)
                                          .Include(b => b.Client)
                                          .Include(s => s.BusinessStatus)
                                          .Include(b => b.CreatedByUser)
                                          .Include(b => b.ModifiedByUser)
                                          .Include(b => b.OperationStructureType)
                                          .Include(b => b.TitleDeedType)
                                          .Where(c => c.IsDeleted == false && c.IsActive && c.ClientId == client.ClientId).ToList();

            var clientDocs = db.FileUploads.Where(d => d.ClientId == client.ClientId && d.referenceId == client.ClientId && d.Document.DocumentTypeId == DocumentTypeId)
                                           .Include(d => d.Document)
                                           .ToList();
            var docsupload = db.Documents.Where(d => d.DocumentTypeId == DocumentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

            var uploadedDocs = (from d in docsupload
                                where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                select d).ToList();
            var oustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName).ToList();

            if (ModelState.IsValid)
            {
                // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
                //IdentityManager.CurrentUser(User);
               
                if (files != null && files.Any())
                {
                    //TG20141028 - FileUpload
                    int counter = 0;
                    var service = new SharepointETMSoapClient();
                    service.Open();
                    foreach (HttpPostedFileBase file in files)
                    {

                        if (file != null)
                        {

                            var fileUrl = "http://10.10.9.71:168/Documents/Trade License/Clients/";
                            int fileLen = file.ContentLength;
                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(file.ContentLength);
                            }

                            int position = file.FileName.LastIndexOf(".", StringComparison.Ordinal);
                            string filename = file.FileName.Substring(0, position);
                            string extension = file.FileName.Substring(position + 1);

                            //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                            const string regExp = @"[^\w\d]";
                            string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                            string name = Regex.Replace(filename, regExp, "_");
                            string contentType = file.ContentType;

                            try
                            {
                              
                                //Generate Unique Document ID or use primary key from database
                                string UniqueID = client.Name + "_" + client.Surname + "_" + client.ClientId;
                                string trade = "Trade License" + " " + "-" + " " + DateTime.Now.Year;
                                string refNo = client.ClientId.ToString();
                                const string des = "Client Document";
                                const string application = @"Trade License\Clients";
                                string metadata = "Title:" + trade + ";ReferenceNo:" + refNo + ";Description:" + des + ";Application:" + application;
                               

                                // Send posted file to SharePoint
                                var status = service.Upload(name, UniqueID, extension, fileData, metadata);

                                fileUrl = fileUrl + uploadName;

                            }
                            catch (System.Web.Services.Protocols.SoapException ex)
                            {
                                throw ex;
                            }
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
                            FileUpload fileupload = new FileUpload
                            {
                                FileName = uploadName,
                                FilePath = fileUrl,
                                ClientId = client.ClientId,
                                ClientName = client.Fullname,
                                Comments = comment,
                                DocumentId = uploadedDocs[counter].DocumentId,
                                referenceId = client.ClientId,
                                IsActive = true,
                                IsDeleted = false,
                                IsLocked = false
                            };
                            db.FileUploads.Add(fileupload);
                            db.SaveChanges();

                            counter++;
                        }
                        else
                        {
                            counter++;
                        }
                    }
                    service.Close();
                }

                //var documentType = db.Documents.Where(d => d.DocumentTypeId == docTypeId).ToList();


                clientDocs = db.FileUploads.Where(d => d.ClientId == client.ClientId && d.referenceId == client.ClientId && d.Document.DocumentTypeId == DocumentTypeId).Include(d => d.Document).ToList();
                oustandingDocuments = (from d in docs
                                       where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                       select d.DocumentName).ToList();

                if (clientDocs.Count() >= docs.Count())
                {
                    client.ClientStatusId =
                    db.Status.Where(s => s.StatusKey == StatusKeys.ClientApproved).Select(s => s.StatusId).
                        FirstOrDefault();
                    //db.Entry(client).State = EntityState.Modified;
                    //db.SaveChanges();

                    //// LM.20141029 - Gets list of required documents based on the document type key
                    //ViewData["Documents"] = db.Documents.Where(d => d.DocumentTypeId == documentTypeId);
                    // TODO: Testing Email Service
                    //#region Construct email
                    //var applicationId =
                    //    coreDataContext.pr_SELECT_ApplicationByKey("trade_license_application", true, false)
                    //        .First()
                    //        .ApplicationID;

                    ////LF2014a 
                    ////User email helper to get well formatted email.
                    //var mail = new EmailHelper();
                    //var body = mail.ConstructEmailBody(client.Fullname, "Your application has been successful proccessed and sent for inspection.");
                    //coreDataContext.pr_INSERT_EmailQueue(applicationId, 5,
                    //                            "user@example.com", null, null,
                    //                            "TLS: Client Registration", body, true, 0,
                    //                             client.IdentityOrPassportNumber, 2, false);

                    //coreDataContext.SubmitChanges();
                    //#endregion email

                    ViewData["ClientData"] = client;
                    TempData["Success"] = "Updated Successfully!";
                }
                else
                {
                    //db.Entry(client).State = EntityState.Modified;
                    //db.SaveChanges();

                    ViewBag.OustandingDocuments = oustandingDocuments;
                    ViewData["UploadList"] = clientDocs;

                    ViewData["Documents"] = (from d in docs
                                             where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                             select d).ToList();
                    TempData["Success"] = "Updated Successfully!";
                }

                var local = db.Set<Client>().Local.FirstOrDefault(l => l.ClientId == client.ClientId);

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                client.Nationality = Nationality;
                client.CustomerType = CustomerType;
                client.Individual = IndividualType;
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.OustandingDocuments = oustandingDocuments;

                ViewData["UploadList"] = clientDocs;
                ViewData["Documents"] = (from d in docs
                                         where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                         select d).ToList();
                //TempData["Success"] = "Saved Successfully!";

                // return RedirectToAction("Index");
            }
            /// exclude other - to be done
            // LM 20141029a - Gets list of required documents based on the document type key.
            //var documentTypeId = db.DocumentTypes.Where(dt => dt.DocumentTypeKey == "client_document").Select(d => d.DocumentTypeId).FirstOrDefault();
            ViewData["Documents"] = (from d in docs
                                     where !(from f in clientDocs select f.DocumentId).Contains(d.DocumentId)
                                     select d).ToList(); //db.Documents.Where(d => d.DocumentTypeId == documentTypeId);
            ViewData["ClientBusinessesList"] = businesses;
            //
            ViewData["UploadList"] = clientDocs;
            //

            ViewBag.PostalAddress3 = new SelectList(new[] { "Select City/Town" });
            ViewBag.ResidentialAddress3 = new SelectList(new[] { "Select City/Town" });
            ViewBag.PostalAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" });
            ViewBag.ResidentialAddress2 = new SelectList(new[] { "Select Suburb/Postal Area" });
            ViewBag.CustomerType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.CustomerType), "ItemTypeName", "ItemTypeName", client.CustomerType);
            ViewBag.Nationality = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.Nationality), "ItemTypeName", "ItemTypeName", client.Nationality);
            ViewBag.IndividualType = new SelectList(db.DropdownItems.Where(d => d.ItemTypeKey == TLKeys.IndividualType), "ItemTypeName", "ItemTypeName", client.Individual);
            return View(client);
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
            // JK.20140906a - Pass the IPrincipal user and it will set the DbContext CurrentUser.
            IdentityManager.CurrentUser(User);

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
        public void Download(int? id, string file, int clientId)
        {
            var service = new SharepointETMSoapClient();
            int pos = file.LastIndexOf(".", StringComparison.Ordinal);
            string fileType = file.Substring(pos);
            string name = file.Substring(0, pos);

            var client = db.Clients.Find(clientId);

            var uploads = db.FileUploads.Find(id);

            var fileName = name + "_" + client.Name + "_" + client.Surname + "_" + uploads.FileUploadId + fileType;

            //var uploads = (from u in db.FileUploads
            //               where u.FileUploadId == id
            //               select u.FilePath).FirstOrDefault();
            
            if (uploads != null)
            {
                //string path = Path.GetFullPath(uploads.ToString());
                
                ////HttpContext.Response.AddHeader("content-dispostion", "attachment; filename=" + fileName);
                //return File(new FileStream(path, FileMode.Open), "content-dispostion", fileName);
                try
                {
                    byte[] arr = service.Download(fileName);


                  
                    //Set the appropriate ContentType.
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename= " + fileName);
                    //Write the file directly to the HTTP content output stream.
                    Response.BinaryWrite(arr);
                    Response.End();
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    throw ex;
                }
            }
            else
            {
                ViewData["error"] = "Invalid file name or file not exist";
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
