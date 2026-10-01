using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;

namespace C8.TradeLicense.Controllers
{
    public class DepartmentContactController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public DepartmentContactController()
        {
            IdentityManager = new IdentityManager(db);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TradeLicenseDbContext()));
            inspectorList = UserRoleListing("Department Inspector", roleManager.FindByName("Department Inspector").Users.ToList());
            chiefInspectorList = UserRoleListing("Department Manager", roleManager.FindByName("Department Manager").Users.ToList());
            chiefInspectorList.AddRange(UserRoleListing("Chief Inspector", roleManager.FindByName("Chief Inspector").Users.ToList()));
            DepartmentClerkList = UserRoleListing("Department Clerk", roleManager.FindByName("Department Clerk").Users.ToList());
        }

        public IdentityManager IdentityManager { get; set; }
        public static IEnumerable<DepartmentContact> departmentContacts;
        public static IEnumerable<Department> departments;
        public static List<object> inspectorList = new List<object>();
        public static List<object> chiefInspectorList = new List<object>();
        public static List<object> DepartmentClerkList = new List<object>();


        #region DepartmentContact
        // GET: /DepartmentContact/
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

                departmentContacts = db.DepartmentContacts.Include(dc => dc._DepartmentId).Include(dc => dc.CreatedByUser).Include(dc => dc.ModifiedByUser).Include(dc => dc.User).ToList().Where(dc => dc.IsDeleted == false && dc.IsActive == true).OrderBy(dc => dc.DepartmentId).ThenBy(dc => dc.IsPrinciple == false);

                if (selectedSearch != null)
                {
                    switch (selectedSearch)
                    {
                        case "DepartmentName":
                            //Search by department name
                            departmentContacts = departmentContacts.Where(d => d._DepartmentId.DepartmentName.ToLower().Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "InspectorName":
                            //Search by inspector name                            
                            departmentContacts = departmentContacts.Where(d => (d.User.FirstName.ToLower() + " " + d.User.LastName.ToLower()).Contains(inputSearch.ToLower()) && d.DepartmentHeadContactId != null).ToList();
                            break;
                        case "ChiefInspectorName":
                            //Search by chief inspector name
                            departmentContacts = departmentContacts.Where(d => (d.User.FirstName.ToLower() + " " + d.User.LastName.ToLower()).Contains(inputSearch.ToLower()) && d.DepartmentHeadContactId == null).ToList();
                            break;
                        case "Department Clerk":
                            //Search by Department Clerk name
                            departmentContacts = departmentContacts.Where(d => (d.User.FirstName.ToLower() + " " + d.User.LastName.ToLower()).Contains(inputSearch.ToLower()) && d.DepartmentHeadContactId == null).ToList();
                            break;
                        default:
                            departmentContacts = departmentContacts.ToList();
                            break;
                    }
                }

                // LM.20141110a - Set parameters for paging
                if (Request.HttpMethod != "GET")
                {
                    page = 1;
                }

                int pageNumber = page ?? 1;
                int pageSize = 5;
                ViewBag.CurrentFilter = inputSearch;
                ViewBag.selecetedSearch = selectedSearch;
                ViewBag.DepartmentContactCount = departmentContacts.ToList().Count;
                return View(departmentContacts.ToList().ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                return View("Error");
            }

        }



        #endregion

        #region DepartmentContact Details
        // GET: /DepartmentContact/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id != null)
                {
                    DepartmentContact departmentcontact = departmentContacts.First(dc => dc.DepartmentContactId == id && dc.IsActive == true && dc.IsDeleted == false);
                    return View(departmentcontact);
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

        #region DepartmentContact Create(GET)
        // GET: /DepartmentContact/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create(int id)
        {
            try
            {
                Department department = db.Departments.Find(id);
                string departmentType = department.DepartmentStructureType;
                List<DepartmentContact> departmentHeadContactsList = new List<DepartmentContact>();

                if (departmentType == "Hierarchy")
                {
                    departmentHeadContactsList = db.DepartmentContacts.Where(dc => dc.DepartmentId == id && dc.DepartmentHeadContactId == null && dc.IsActive == true && dc.IsDeleted == false).Include(dc => dc.User).ToList();
                    ViewBag.DepartmentHeadContacts = departmentHeadContactsList.Count;
                }
                else
                {
                    chiefInspectorList.AddRange(inspectorList);
                }

                ViewBag.DepartmentClerkList = DepartmentClerkList;
                ViewBag.InspectorList = inspectorList;
                ViewBag.ChiefInspectorList = chiefInspectorList;
                ViewBag.DepartmentHeadContactsList = departmentHeadContactsList;
                ViewBag.AvailableInspectors = inspectorList.Count;
                ViewBag.AvailableChiefInspectors = chiefInspectorList.Count;
                ViewBag.DepartmentStructureType = departmentType;
                ViewBag.Departmentdesc = department.DepartmentDescription;

                return View();

            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        #endregion

        #region DepartmentContact Create(POST)
        // POST: /DepartmentContact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "DepartmentContactId,DepartmentId,UserId,IsPrinciple,IsActive,IsDeleted,IsLocked,CreatedBySystemUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,DepartmentHeadContactId")] DepartmentContact departmentcontact, string roleType, string structureType, string ddlInspector, string ddlChiefInspector, string ddlDepartmentHead, string ddlDepartmentClerk)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityManager.CurrentUser(User);

                    if ((structureType == "Hierarchy" || structureType == "Flat") && (roleType == "Department Manager"))
                    {
                        departmentcontact.IsPrinciple = true;
                        departmentcontact.UserId = Convert.ToInt32(ddlChiefInspector);
                        departmentcontact.DepartmentHeadContactId = null;
                        departmentcontact.RoleName = "Department Manager";
                    }
                    else
                    {
                        departmentcontact.IsPrinciple = false;
                        if (ddlDepartmentClerk != "")
                        {
                            departmentcontact.UserId = Convert.ToInt32(ddlDepartmentClerk);
                            departmentcontact.RoleName = "Department Clerk";
                        }
                        if (ddlInspector != "")
                        {
                            departmentcontact.RoleName = "Department Inspector";
                            departmentcontact.UserId = Convert.ToInt32(ddlInspector);
                        }
                        int departmentHeadUserId = Int32.Parse(ddlDepartmentHead);
                        departmentcontact.DepartmentHeadContactId = db.DepartmentContacts.Where(d => d.DepartmentHeadContactId == null && d.UserId == departmentHeadUserId && d.IsActive == true && d.IsDeleted == false).Select(s => s.DepartmentContactId).FirstOrDefault();
                    }

                    departmentcontact.IsActive = true;
                    departmentcontact.IsDeleted = false;
                    departmentcontact.IsLocked = false;

                    db.DepartmentContacts.Add(departmentcontact);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentcontact.DepartmentId);
                ViewBag.CreatedBySystemUserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.ModifiedByUserId);
                ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.UserId);
                return View(departmentcontact);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        #endregion

        #region DepartmentContact Edit(GET)
        // GET: /DepartmentContact/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id != null)
                {
                    DepartmentContact departmentcontact = departmentContacts.First(dc => dc.DepartmentId == id && dc.IsActive == true && dc.IsDeleted == false);
                    var departmentContactRole = IdentityManager.UserManager.GetRoles(IdentityManager.CurrentUserId(departmentcontact.User.Username)).FirstOrDefault();
                    var currentInspector = db.Users.Find(departmentcontact.UserId);
                    var departmentHeads = db.DepartmentContacts.Include(dc => dc.User).Where(dc => dc.DepartmentId == departmentcontact.DepartmentId && dc.DepartmentHeadContactId == null && dc.IsActive == true && dc.IsDeleted == false).ToList();

                    if (departmentcontact.DepartmentHeadContactId == null)
                    {
                        if (departmentcontact._DepartmentId.DepartmentStructureType == "Flat")
                        {
                            chiefInspectorList.AddRange(inspectorList);
                        }

                        chiefInspectorList.Insert(0, currentInspector);
                        ViewBag.InspectorUserList = chiefInspectorList;
                    }
                    else
                    {
                        inspectorList.Insert(0, currentInspector);
                        ViewBag.InspectorUserList = inspectorList;
                    }

                    ViewBag.DepartmentHeads = departmentHeads;
                    ViewBag.DepartmentContactRole = departmentContactRole;
                    ViewBag.DepartmentStructureType = departmentcontact._DepartmentId.DepartmentStructureType;
                    ViewBag.AvailableInspectors = inspectorList.Count;
                    ViewBag.AvailableChiefInspectors = chiefInspectorList.Count;
                    ViewBag.CreatedBySystemUserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.CreatedByUserId);
                    ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.ModifiedByUserId);

                    return View(departmentcontact);
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

        #region DepartmentContact Edit(POST)
        // POST: /DepartmentContact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "System Administrators" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "DepartmentContactId,DepartmentId,UserId,IsPrinciple,IsActive,IsDeleted,IsLocked,CreatedBySystemUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime,DepartmentHeadContactId")] DepartmentContact departmentcontact,
            string ddlInspectorUserList, string ddlDepartmentHeadUserList, string UserRole, string StructureType)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    departmentcontact.UserId = Convert.ToInt32(ddlInspectorUserList);

                    if (UserRole == "Inspectors" && StructureType == "Hierarchy")
                    {
                        departmentcontact.DepartmentHeadContactId = Convert.ToInt32(ddlDepartmentHeadUserList);
                    }

                    var local = db.Set<DepartmentContact>().Local
                        .FirstOrDefault(l => l.DepartmentContactId == departmentcontact.DepartmentContactId);

                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;
                    }

                    db.Entry(departmentcontact).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentcontact.DepartmentId);
                ViewBag.CreatedBySystemUserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.ModifiedByUserId);
                ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", departmentcontact.UserId);
                return View(departmentcontact);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region DepartmentContact Delete(GET)
        // GET: /DepartmentContact/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    DepartmentContact departmentcontact = departmentContacts.First(dc => dc.DepartmentContactId == id && dc.IsActive == true && dc.IsDeleted == false);
                    return View(departmentcontact);
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

        #region DepartmentContact Delete(POST)
        // POST: /DepartmentContact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                IdentityManager.CurrentUser(User);
                DepartmentContact departmentcontact = db.DepartmentContacts.Find(id);
                departmentcontact.IsActive = false;
                departmentcontact.IsDeleted = true;
                db.Entry(departmentcontact).State = EntityState.Modified;
                db.SaveChanges();
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
        public List<object> UserRoleListing(string roleType, List<IdentityUserRole> usersForRole)
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
            if (roleType == "Department Manager")
            {
                IdentityManager = new IdentityManager(db);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TradeLicenseDbContext()));
                usersForRole = roleManager.FindByName("Licensing Manager").Users.ToList();
                foreach (var user in usersForRole)
                {
                    var userId = user.UserId;
                    var currentUser = identifyManager.CurrentUser(userId);
                    userRoleList.Add(currentUser);
                }
            }
            if (roleType == "Department Clerk")
            {
                IdentityManager = new IdentityManager(db);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TradeLicenseDbContext()));
                usersForRole = roleManager.FindByName("Licensing Clerk").Users.ToList();
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
            }
            return userRoleList;
        }

        #endregion

    }
}
