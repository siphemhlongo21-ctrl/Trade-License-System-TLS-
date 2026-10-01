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
    public class DepartmentController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();

        public DepartmentController()
        {
            IdentityManager = new IdentityManager(db);
        }

        public IdentityManager IdentityManager { get; set; }
        public static string currentDepartmentName;
        public static IEnumerable<Department> departments;
        public static IEnumerable<DepartmentContact> departmentContacts;
        public static IEnumerable<IdentityUserRole> usersForChiefInspectorRole;
        public static List<object> currentChiefInspectors = new List<object>();


        #region Department Index
        // GET: /Department/
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

                departments = db.Departments.Include(d => d.CreatedByUser).Include(d => d.ModifiedByUser).Include(d => d.Region).Where(d => d.IsDeleted == false && d.IsActive == true).OrderBy(d => d.Region.RegionName).ToList();
                departmentContacts = db.DepartmentContacts.Where(dc => dc.IsActive == true && dc.IsDeleted == false).ToList();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TradeLicenseDbContext()));
                usersForChiefInspectorRole = roleManager.FindByName("Department Manager").Users.ToList();

                if (selectedSearch != null)
                {
                    switch (selectedSearch)
                    {
                        case "DepartmentName":
                            //Search by department name
                            departments = departments.Where(d => d.DepartmentName.ToLower().Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "DepartmentKey":
                            //Search by department key
                            departments = departments.Where(d => d.DepartmentKey.ToLower().Contains(inputSearch.ToLower())).ToList();
                            break;
                        case "Region":
                            //Search by department region
                            departments = departments.Where(d => d.Region.RegionName.ToLower().Contains(inputSearch.ToLower())).ToList();
                            break;
                        default:
                            departments = departments.ToList();
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
                ViewBag.DepartmentCount = departments.ToList().Count;
                return View(departments.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

#endregion

        #region Department Details
        // GET: /Department/Details/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id != null)
                {
                    Department department = departments.First(d => d.DepartmentId == id && d.IsActive == true && d.IsDeleted == false);
                    return View(department);
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

        #region Department Create(GET)
        // GET: /Department/Create
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName");
                ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true), "RegionId", "RegionName");

                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Department Create(POST)
        // POST: /Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Create([Bind(Include = "DepartmentId,DepartmentName,DepartmentDescription,DepartmentKey,DepartmentStructureType,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime, RegionId")] Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    departments = departments.Where(d => d.DepartmentName.Replace(" ", String.Empty).Equals(department.DepartmentName.Replace(" ", String.Empty), StringComparison.InvariantCultureIgnoreCase) && d.IsActive == true && d.IsDeleted == false);

                    if (departments.Count() == 0)
                    {
                        IdentityManager.CurrentUser(User);
                        department.IsActive = true;
                        department.IsDeleted = false;
                        department.IsLocked = false;

                        db.Departments.Add(department);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Department Name already exists";
                    }
                }

                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", department.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", department.ModifiedByUserId);
                ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true), "RegionId", "RegionName", department.RegionId);
                return View(department);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Department Edit(GET)
        // GET: /Department/Edit/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id != null)
                {
                    Department department = departments.First(d => d.DepartmentId == id && d.IsActive == true && d.IsDeleted == false);
                    currentDepartmentName = department.DepartmentName;
                    departmentContacts = departmentContacts.Where(dc => dc.DepartmentId == department.DepartmentId && dc.IsActive == true && dc.IsDeleted == false);
                    var users = db.Users.Where(u => u.IsActive == true && u.IsDeleted == false).ToList();
                    currentChiefInspectors.Clear();

                    foreach (var contact in departmentContacts)
                    {
                        string userGuidId = IdentityManager.CurrentUserId(users.Find(u => u.UserId == contact.UserId && u.IsActive == true && u.IsDeleted == false).Username);

                        if (userGuidId != null)
                        {
                            bool isChiefInspectorRole = IdentityManager.UserManager.IsInRole(userGuidId, "Department Manager");

                            if (isChiefInspectorRole)
                            {
                                var currentUser = IdentityManager.CurrentUser(userGuidId);
                                currentChiefInspectors.Add(currentUser);
                            }
                        }
                    }

                    ViewBag.CurrentStructureType = department.DepartmentStructureType;
                    ViewBag.CurrentChiefInspectorRole = currentChiefInspectors;
                    ViewBag.DepartmentHeadContacts = currentChiefInspectors.Count;
                    ViewBag.UsersForChiefInspectorRole = UserRoleListing(usersForChiefInspectorRole.ToList());
                    ViewBag.AvailableChiefInspectors = UserRoleListing(usersForChiefInspectorRole.ToList()).Count;
                    ViewBag.RegionId = new SelectList(db.Regions.Where(r => r.IsDeleted == false && r.IsActive == true), "RegionId", "RegionName", department.RegionId);
                    ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", department.CreatedByUserId);
                    ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", department.ModifiedByUserId);
                    return View(department);
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

        #region Department Edit(POST)
        // POST: /Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentName,DepartmentDescription,DepartmentKey,DepartmentStructureType,IsActive,IsDeleted,IsLocked,CreatedByUserId,CreatedDateTime,ModifiedByUserId,ModifiedDateTime, RegionId")] Department department, string ddlChiefInspector, string currentStructureType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    departments = departments.Where(d => d.DepartmentName != currentDepartmentName && d.DepartmentName.Replace(" ", String.Empty).Equals(department.DepartmentName.Replace(" ", String.Empty), StringComparison.InvariantCultureIgnoreCase) && d.IsActive == true && d.IsDeleted == false);
                    departmentContacts = db.DepartmentContacts.AsNoTracking().Where(dc => dc.DepartmentId == department.DepartmentId && dc.IsActive == true && dc.IsDeleted == false);

                    if (departments.Count() == 0)
                    {
                        IdentityManager.CurrentUser(User);
                    
                        if (!currentStructureType.Equals(department.DepartmentStructureType))
                        {
                            if (department.DepartmentStructureType == "Flat")
                            {
                                foreach (var contact in departmentContacts)
                                {
                                    contact.IsPrinciple = true;
                                    contact.DepartmentHeadContactId = null;
                                    db.Entry(contact).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                int chiefInspectorUserId = Convert.ToInt32(ddlChiefInspector);
                                bool chiefInspectorAssigned = departmentContacts.Where(dc => dc.IsActive == true && dc.IsDeleted == false).ToList().Exists(dc => dc.UserId == chiefInspectorUserId);

                                if (!chiefInspectorAssigned)
                                {
                                    DepartmentContact departmentContact = new DepartmentContact();
                                    departmentContact.DepartmentId = department.DepartmentId;
                                    departmentContact.UserId = chiefInspectorUserId;
                                    departmentContact.IsPrinciple = true;
                                    departmentContact.DepartmentHeadContactId = null;
                                    departmentContact.IsActive = true;
                                    departmentContact.IsDeleted = false;
                                    departmentContact.IsLocked = false;
                                    db.DepartmentContacts.Add(departmentContact);
                                    db.SaveChanges();
                                }

                                int departmentHeadContactId = departmentContacts.First(dc => dc.UserId == chiefInspectorUserId && dc.IsActive == true && dc.IsDeleted == false).DepartmentContactId;

                                foreach (var contact in departmentContacts)
                                {
                                    if (contact.UserId != chiefInspectorUserId)
                                    {
                                        contact.IsPrinciple = false;
                                        contact.DepartmentHeadContactId = departmentHeadContactId;
                                        db.Entry(contact).State = EntityState.Modified;
                                    }
                                }
                            }
                        }

                        var local = db.Set<Department>().Local.FirstOrDefault(l => l.DepartmentId == department.DepartmentId);

                        if (local != null)
                        {
                            db.Entry(local).State = EntityState.Detached;
                        }

                        db.Entry(department).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = "Department Name already exists";
                    }
                }

                ViewBag.CurrentChiefInspectorRole = currentChiefInspectors;
                ViewBag.DepartmentHeadContacts = currentChiefInspectors.Count;
                ViewBag.UsersForChiefInspectorRole = UserRoleListing(usersForChiefInspectorRole.ToList());
                ViewBag.CreatedByUserId = new SelectList(db.Users, "UserId", "FirstName", department.CreatedByUserId);
                ViewBag.ModifiedByUserId = new SelectList(db.Users, "UserId", "FirstName", department.ModifiedByUserId);
                return View(department);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        #endregion

        #region Department Delete(EDIT)
        // GET: /Department/Delete/5
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    Department department = departments.First(d => d.DepartmentId == id && d.IsActive == true && d.IsDeleted == false);
                    return View(department);
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

        #region Department Delete(POST)
        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Licensing Administrator" + "," + "System Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Department department = db.Departments.Find(id);
                department.IsActive = false;
                department.IsDeleted = true;

                IdentityManager.CurrentUser(User);
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();

                DeleteDepartmentContacts(id);

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

        #region Cascade Delete On Department Contacts
        public void DeleteDepartmentContacts(int id)
        {
            var departmentContacts = db.DepartmentContacts.ToList().Where(d => d.DepartmentId == id && d.IsDeleted == false && d.IsActive == true);

            foreach (var departmentcontact in departmentContacts)
            {
                IdentityManager.CurrentUser(User);
                departmentcontact.IsActive = false;
                departmentcontact.IsDeleted = true;

                db.Entry(departmentcontact).State = EntityState.Modified;
                db.SaveChanges();
            }
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
                bool userAssigned = departmentContacts.Any(d => d.UserId == currentUser.UserId && d.IsActive == true && d.IsDeleted == false);

                if (!userAssigned)
                {
                    userRoleList.Add(currentUser);
                }
            }

            return userRoleList;
        }
        #endregion
    }
}
