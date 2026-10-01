using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using Microsoft.Owin.Security;
using PagedList;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace C8.TradeLicense.Controllers
{
    public class UserController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private CesarDbContext core = new CesarDbContext();

        public UserController()
        {
            IdentityManager = new IdentityManager(db);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TradeLicenseDbContext()));
            inspectorList = UserRoleListing(roleManager.FindByName("Department Inspector").Users.ToList());
            chiefInspectorList = UserRoleListing(roleManager.FindByName("Chief Inspector").Users.ToList());
        }

        public IdentityManager IdentityManager { get; set; }
        public static List<object> inspectorList = new List<object>();
        public static List<object> chiefInspectorList = new List<object>();

        public User Users { get; set; }


        public int UserId { get; set; }
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

        #region User Index
        // GET: /User/Index
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Index(string currentFilter, string searchCriteria, string selectedSearch, string inputSearch, string Filter_Value, int? page)
        {
            try
            {
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

                var users = db.Users.Include(u => u.CreatedByUser).Include(u => u.ModifiedByUser).Where(u => u.IsActive == true && u.IsDeleted == false).ToList();

                if (selectedSearch != null)
                {
                    switch (selectedSearch)
                    {
                        case "UserFirstName":
                            //Search by User First Name
                            users = users.Where(u => (u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "UserName":
                            //Search by login UserName
                            users = users.Where(u => (u.Username.ToLower()).Contains(inputSearch.ToLower())).ToList();
                            break;
                        default:
                            users = users.ToList();
                            break;
                    }
                }

                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    page = 1;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                ViewBag.UserCount = users.Count;
                return View(users.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                return View("Error");
            }

        }
        #endregion

        #region User Details
        // GET: /User/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id != null)
                {
                    User user = db.Users.Find(id);
                    return View(user);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region User Create(GET)
        // GET: /User/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.Region = new SelectList(db.Regions.Where(r=> r.IsActive == true && r.IsDeleted == false), "RegionId", "RegionName");
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region User Create(POST)
        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Username,Password,EmailAddress,LandLine,Mobile,Fax,IpAddress,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,Region")] User user)
        {
            try
            {
                if (user.Username == null)
                {
                    TempData["Error"] = "Please enter a username";
                }
                else if (user.Username == null)
                {
                    TempData["Error"] = "Please enter a unique username";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        IdentityManager.CurrentUser(User);
                        bool usernameAssigned = db.Users.Any(u => u.Username.ToLower() == user.Username.ToLower() && u.IsActive == true && u.IsDeleted == false);
                        //var password = GeneratePassword();
                        var password = "Password";
                        if (!usernameAssigned)
                        {
                            user.IsActive = true;
                            user.IsDeleted = false;
                            user.IsLocked = false;
                            user.IsPasswordReset = false;
                            var applicationUser = new ApplicationUser()
                            {
                                UserName = user.Username,
                                Email = user.EmailAddress,
                                User = user
                            };


                            IdentityManager.CreateUser(applicationUser, password);
                            db.SaveChanges();

                            #region Construct Email
                            var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
                            var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                            var referenceTypeId = core.tb_ReferenceTypes.FirstOrDefault(r => r.ReferenceTypeKey == "eservices_identity_number" && r.IsDeleted == false && r.IsActive).ReferenceTypeId;
                            var body = String.Empty;
                            //LF2014a 
                            //User email helper to get well formatted email.
                            //var mail = new EmailHelper();
                            if (template != null)
                            {
                                body = template.EmailBody;
                            }

                            //L.M.20150303a - Replace variables with actual email content
                            body = body.Replace("#NAME#", user.FullName);
                            body = body.Replace("#BODYTEXT#", "You have succesfully registered on Trade License System. <br/><br/><b> Login Details:</b><br> " +
                                " Username: " + user.Username + "<br/>Password: " + password +
                                "<br/><br/> Please change your password on your first login.");
                            var email = new tb_EmailQueue
                            {
                                QueueDateTime = DateTime.Now,
                                ApplicationId = applicationId,
                                EmailAccountId = 5,//Hard coded
                                ToList = user.EmailAddress,
                                CcList = null,
                                BccList = null,
                                Subject = "TLS: User Login Details.",
                                Body = body,
                                IsHtml = true,
                                FailureCount = 0,
                                ReferenceId = applicationUser.UserId.ToString(),
                                HasAttachments = false
                            };

                            core.tb_EmailQueue.Add(email);
                            core.SaveChanges();

                            //coreDataContext.pr_INSERT_EmailQueue(applicationId,
                            //    5, // EmailAccountId,
                            //    user.EmailAddress,
                            //    null,
                            //    null,
                            //    "TLS: Client Registration", body, true,
                            //    0, // Failure counter
                            //    applicationUser.UserId.ToString(),
                            //    referenceTypeId,
                            //    false);

                            //coreDataContext.SubmitChanges();

                            #endregion email

                            //var applicationUser = new ApplicationUser() { UserName = user.Username, User = new User { FirstName = user.FirstName,  } };
                            //IdentityManager.CreateUser(applicationUser, user.Password);
                            // db.Users.Add(user);
                            // db.SaveChanges();

                            //return RedirectToAction("Index");
                            return RedirectToAction("Edit", new { id = applicationUser.UserId });
                        }
                        else
                        {
                            TempData["Error"] = "Username already assigned. Please choose a unique username";
                        }

                    }

                }
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", user.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", user.ModifiedByUserId);
                ViewBag.Region = new SelectList(db.Regions.Where(r => r.IsActive == true && r.IsDeleted == false), "RegionId", "RegionName",user.RegionId);
                return View(user);
            }
            catch (Exception e)
            {
                var error = e.InnerException;
                return View("Error");
            }
        }


        #endregion

        #region User Edit(GET)
        // GET: /User/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id != null)
                {
                    User user = db.Users.Find(id);
                    ViewBag.Region = new SelectList(db.Regions.Where(r => r.IsActive == true && r.IsDeleted == false), "RegionId", "RegionName");
                    ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", user.CreatedByUserId);
                    ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", user.ModifiedByUserId);
                    return View(user);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception e)
            {
                var error = e.InnerException;
                return View("Error");
            }
        }

        #endregion

        #region User Edit(POST)
        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,FirstName,LastName,Username,Password,EmailAddress,LandLine,Mobile,Fax,IpAddress,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,Region,Role")] User user)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

                    var cUser = await UserManager.FindByNameAsync(user.Username.ToString());
                    if (cUser.Email != user.EmailAddress)
                    {
                        await store.SetEmailAsync(cUser, user.EmailAddress);
                        await store.UpdateAsync(cUser);

                    }


     

                    IdentityManager.CurrentUser(User);

                    var local = db.Set<User>()
                    .Local.FirstOrDefault(l => l.UserId == user.UserId);

                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;
                    }

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", user.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", user.ModifiedByUserId);
                return View(user);
            }

            catch (Exception ex)
          
            {
                var em = ex.Message;
                return View("Error");
            }

            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //        eve.Entry.Entity.GetType().Name, eve.Entry.State);

            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
            //                ve.PropertyName,
            //                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
            //                ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
        }

        #endregion

        #region User Delete(EDIT)
        // GET: /User/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    User user = db.Users.Find(id);
                    return View(user);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region User Delete(POST)
        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                User user = db.Users.Find(id);
                user.IsActive = false;
                user.IsDeleted = true;

                IdentityManager.CurrentUser(User);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Add User Role(GET)
        //GET: /User/AddUserRole
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult AddUserRole(string data)
        {
            try
            {
                ViewBag.UserId = db.Users.Where(u => u.Username == data).Select(s => s.UserId).FirstOrDefault().ToString();
                ViewBag.UserName = data;
                ViewBag.Roles = db.Roles.OrderBy(r => r.Name).ToList();
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Add User Role(POST)
        //POST: /User/AddUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult AddUserRole( string UserName, string RoleName, string UserId)
        {
            try
            {
              

                IdentityManager.AddUserToRole(IdentityManager.CurrentUserId(UserName), RoleName);
     

                int userId = Int32.Parse(UserId);
                User users = db.Users.Find(userId);

                users.Role = RoleName;
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Edit User Role(GET)
        //GET: /User/EditUserRole
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult EditUserRole(string data)
        {
            try
            {
                var userGuidId = IdentityManager.CurrentUserId(data);
                var role = IdentityManager.UserManager.GetRoles(userGuidId).First();
                bool inspectorAssigned = false;
                List<DepartmentContact> departmentcontacts = new List<DepartmentContact>();
                var user = IdentityManager.CurrentUser(userGuidId);
                inspectorAssigned = db.DepartmentContacts.Any(dc => dc.UserId == user.UserId && dc._DepartmentId.DepartmentStructureType == "Hierarchy" && dc.IsActive == true && dc.IsDeleted == false);

                if (role == "Chief Inspector" || role == "Administrator")
                {
                    if (inspectorAssigned)
                    {
                        var department = db.DepartmentContacts.FirstOrDefault(dc => dc.UserId == user.UserId && dc.IsActive == true && dc.IsDeleted == false);
                        if (department != null)
                        {
                            departmentcontacts = db.DepartmentContacts.Include(dc => dc.User).Where(dc => dc.DepartmentId == department.DepartmentId && dc.DepartmentHeadContactId == null && dc.IsActive == true && dc.IsDeleted == false).ToList();
                        }
                    }

                    ViewBag.UserList = chiefInspectorList;
                }
                else
                {
                    ViewBag.UserList = inspectorList;
                }

                ViewBag.UserName = data;
                ViewBag.CurrentRole = role;
                ViewBag.IsAssigned = inspectorAssigned;
                ViewBag.AvailableDepartmentChiefInspectors = departmentcontacts.Count;
                ViewBag.AvailableChiefInspectors = chiefInspectorList.Count;
                ViewBag.AvailableInspectors = inspectorList.Count;
                ViewBag.Roles = db.Roles.OrderBy(r => r.Name).ToList();
                //ViewBag.ddlRoleName = new SelectList(db.Roles.OrderBy(r => r.Name), "Name", "Name", user.Role);
                ViewBag.UserId = db.Users.Where(u => u.Username == data).Select(s => s.UserId).FirstOrDefault().ToString();
                ViewBag.UserGuidId = userGuidId;
                ViewBag.DepartmentChiefInspectors = departmentcontacts.Where(dc => dc.UserId != user.UserId && dc.IsActive == true && dc.IsDeleted == false);
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Edit User Role(POST)
        //POST: /User/EditUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult EditUserRole(string UserGuidId, string UserId, string CurrentUserRole, string UserName, string ddlRoleName, bool AssignNewChiefInspector, string radNewDepartmentHead, string ddlUserList, string ddlDepartmentUserList)
        {
            try
            {
                if (IdentityManager.UserManager.IsInRole(UserGuidId, CurrentUserRole) && CurrentUserRole != ddlRoleName)
                {
                    IdentityManager.UserManager.RemoveFromRole(UserGuidId, CurrentUserRole);
                    IdentityManager.AddUserToRole(UserGuidId, ddlRoleName);

                    if (AssignNewChiefInspector)
                    {
                        var user = IdentityManager.CurrentUser(UserGuidId);
                        var inspector = db.DepartmentContacts.First(dc => dc.UserId == user.UserId && dc.IsActive == true && dc.IsDeleted == false);
                        int departmentId = inspector.DepartmentId;

                        if (radNewDepartmentHead == "Existing")
                        {
                            var inspectors = db.DepartmentContacts.Where(dc => dc.DepartmentId == departmentId && dc.DepartmentHeadContactId == inspector.DepartmentContactId && dc.IsActive == true && dc.IsDeleted == false).ToList();
                            DepartmentContact departmentcontact = db.DepartmentContacts.Find(inspector.DepartmentContactId);
                            departmentcontact.IsActive = false;
                            departmentcontact.IsDeleted = true;

                            IdentityManager.CurrentUser(User);
                            db.Entry(departmentcontact).State = EntityState.Modified;

                            int departmentHeadContactId = Convert.ToInt32(ddlDepartmentUserList);
                            var availableChiefInspector = db.DepartmentContacts.Find(departmentHeadContactId);

                            if (availableChiefInspector != null)
                            {
                                foreach (var inspt in inspectors)
                                {
                                    inspt.DepartmentHeadContactId = availableChiefInspector.DepartmentContactId;
                                    db.Entry(inspt).State = EntityState.Modified;
                                }
                            }
                        }
                        else
                        {
                            inspector.UserId = Convert.ToInt32(ddlUserList);
                        }

                        db.Entry(inspector).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    int userId = Int32.Parse(UserId);
                    User users = db.Users.Find(userId);

                    users.Role = ddlRoleName;
                    db.Entry(users).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        #endregion

        #region Delete User Role(GET)
        //GET: /User/DeleteUserRole
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteUserRole(string data)
        {
            try
            {
                var userGuidId = IdentityManager.CurrentUserId(data);
                var roleForUser = IdentityManager.UserManager.GetRoles(userGuidId).First();

                ViewBag.UserName = data;
                ViewBag.UserGuidId = userGuidId;
                ViewBag.RoleForUser = roleForUser;
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Delete User Role(POST)
        //POST: /User/DeleteUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteUserRole(string UserGuidId, string RoleName)
        {
            try
            {
                if (IdentityManager.UserManager.IsInRole(UserGuidId, RoleName))
                {
                    IdentityManager.UserManager.RemoveFromRole(UserGuidId, RoleName);
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region User Role Listing
        public List<object> UserRoleListing(List<IdentityUserRole> usersForRole)
        {
            IdentityManager identifyManager = new IdentityManager();
            var userRoleList = new List<object>();
            var departmentContacts = db.DepartmentContacts;

            foreach (var user in usersForRole)
            {
                var userId = user.UserId;
                var currentUser = identifyManager.CurrentUser(userId);
                bool userAssigned = departmentContacts.Any(dc => dc.UserId == currentUser.UserId && dc.IsActive == true && dc.IsDeleted == false);

                if (!userAssigned)
                {
                    userRoleList.Add(currentUser);
                }
            }

            return userRoleList;
        }
        #endregion

        #region Generate Default Password
        /// <summary>
        /// Generates the password for each user.
        /// </summary>
        /// <returns></returns>
        public string GeneratePassword()
        {
            int codeCount = 8;
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return GeneratePassword();
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
        #endregion
    }
}
