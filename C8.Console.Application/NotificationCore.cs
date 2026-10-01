using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using C8.TradeLicense.DataAccessLayer;
using C8.Exchange.CommonCore;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Configuration;

namespace C8.Console.Application
{
    public class NotificationCore
    {
        private C8.TradeLicense.DataAccessLayer.TradeLicenseDbContext db = new TradeLicense.DataAccessLayer.TradeLicenseDbContext();
        private CommonCore_GoLiveEntities core = new CommonCore_GoLiveEntities();
        private int EmailAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["EmailAccountId"]);

        private DateTime _currentDate = DateTime.Now;

        //Constant values
        private const int BusinessLicensingSlaNumDays = 3;
        private const int OtherDepartmentSlaNumDays = 5;

        #region Public Methods

        /// <summary>
        /// LM.20150216- Sends out reminders, notifications, escalation alerts... 
        /// </summary>
        public void GenerateDailyReminders()
        {
            var count = 0;
            int applicationId =
                core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive)
                    .ApplicationID;
            var template =
                core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
            var body = string.Empty;
            try
            {
                #region Require Five day reminder.
                var requireFiveDayReminderList = GetLicensesRequireFiveDayReminder();
                foreach (var license in requireFiveDayReminderList)
                {
                    #region Construct Email
                    var client = db.Clients.FirstOrDefault(c => c.ClientId == license.ClientId && c.IsDeleted == false && c.IsActive);

                    if (client != null)
                    {
                        if (template != null)
                        {
                            body = template.EmailBody;
                        }

                        //L.M.20150303a - Replace variables with actual email content
                        body = body.Replace("#NAME#", client.Name + ' ' + client.Surname);
                        body = body.Replace("#BODYTEXT#", "This is a friendly reminder to renew your license. Your license expires on " + Convert.ToDateTime(license.LicenseExpiryDate).ToShortDateString());

                        var email = new tb_EmailQueue
                        {
                            QueueDateTime = DateTime.Now,
                            ApplicationId = applicationId,
                            EmailAccountId = EmailAccountId,
                            ToList = client.EmailAddress,
                            CcList = null,
                            BccList = null,
                            Subject = "TLS: License Renewal Reminder",
                            Body = body,
                            IsHtml = true,
                            FailureCount = 0,
                            ReferenceId = client.IdentityOrPassportNumber,
                            HasAttachments = false
                        };

                        core.tb_EmailQueue.Add(email);
                        core.SaveChanges();
                        count++;
                    }

                    #endregion
                }
                #endregion
                #region Inspection Requests require escalation.

                //Console.WriteLine("Escalating inspection requests overdue...");
                var requestRequireEscalationList = GetAllOverDueInspectionRequests();
                var chiefInspectors = new List<C8.TradeLicense.Models.DepartmentContact>();
                count = 0;
                foreach (var inspectionRequest in requestRequireEscalationList)
                {
                    var department = db.Departments.FirstOrDefault(d => d.DepartmentId == inspectionRequest.DepartmentId && d.IsDeleted == false && d.IsActive);
                    var request = inspectionRequest;
                    var licence = db.Licenses.Where(l => l.LicenseId == request.LicenseId && l.IsDeleted == false && l.IsActive)
                                             .Include(l => l.Business)
                                             .FirstOrDefault();
                    var client = db.Clients.FirstOrDefault(c => c.ClientId == inspectionRequest.ClientId && c.IsDeleted == false && c.IsActive);

                    if (department != null && department.DepartmentStructureType == "Hierarchy")
                    {
                        chiefInspectors = db.DepartmentContacts.Where(c => c.DepartmentId == department.DepartmentId && c.IsDeleted == false && c.IsActive && c.IsPrinciple)
                                                               .Include(c => c.User).ToList();

                        #region Construct Email
                        if (chiefInspectors.Count > 0)
                        {
                            foreach (var departmentContact in chiefInspectors)
                            {
                                //L.M.20150303a - Use default email template from the DB
                                if (template != null)
                                {
                                    body = template.EmailBody;
                                }

                                if (licence != null)
                                {
                                    //L.M.20150303a - Replace variables with actual email content
                                    //TODO: User2 - (OneToMany) User.UserId -> DepartmentContact.UserId
                                    body = body.Replace("#NAME#", departmentContact.User.FirstName + ' ' + departmentContact.User.LastName);
                                    body = body.Replace("#BODYTEXT#", "URGENT:License inspection request overdue for " +
                                        licence.Business.ProposedTradeName +
                                        "<br/> No. of days overdue: " + GetNumberOfWorkingDays((DateTime)inspectionRequest.CreatedDateTime, _currentDate));

                                    var email = new tb_EmailQueue
                                    {
                                        QueueDateTime = DateTime.Now,
                                        ApplicationId = applicationId,
                                        EmailAccountId = EmailAccountId,
                                        ToList = departmentContact.User.EmailAddress,
                                        CcList = null,
                                        BccList = null,
                                        Subject = "TLS: Inspection Request OverDue",
                                        Body = body,
                                        IsHtml = true,
                                        FailureCount = 0,
                                        ReferenceId = client.IdentityOrPassportNumber,
                                        HasAttachments = false
                                    };

                                    core.tb_EmailQueue.Add(email);
                                    core.SaveChanges();
                                    count++;
                                }

                            }

                        }
                        #endregion
                    }
                    else if (department != null && department.DepartmentStructureType == "Flat")
                    {
                        var departmentContactList = db.DepartmentContacts.Where(d => d.DepartmentContactId == department.DepartmentId && !d.IsDeleted && d.IsActive).ToList();

                        foreach (var departmentContact in departmentContactList)
                        {
                            //L.M.20150303a - Use default email template from the DB
                            if (template != null)
                            {
                                body = template.EmailBody;
                            }

                            if (licence != null)
                            {
                                //L.M.20150303a - Replace variables with actual email content
                                //TODO: User2 - (OneToMany) User.UserId -> DepartmentContact.UserId
                                body = body.Replace("#NAME#", departmentContact.User.FirstName + ' ' + departmentContact.User.LastName);
                                body = body.Replace("#BODYTEXT#", "URGENT:License inspection request overdue for " +
                                    licence.Business.ProposedTradeName +
                                    "<br/> No. of days overdue: " + GetNumberOfWorkingDays((DateTime)inspectionRequest.CreatedDateTime, _currentDate));

                                var email = new tb_EmailQueue
                                {
                                    QueueDateTime = DateTime.Now,
                                    ApplicationId = applicationId,
                                    EmailAccountId = EmailAccountId,
                                    ToList = departmentContact.User.EmailAddress,
                                    CcList = null,
                                    BccList = null,
                                    Subject = "TLS: Inspection Request OverDue",
                                    Body = body,
                                    IsHtml = true,
                                    FailureCount = 0,
                                    ReferenceId = client.IdentityOrPassportNumber,
                                    HasAttachments = false
                                };

                                core.tb_EmailQueue.Add(email);
                                core.SaveChanges();
                                count++;
                            }

                        }
                    }
                }

                #endregion
                #region Terminated Licenses Notification.

                var terminatedLicenseList = FunctionCore.GetLicensesRequireTermination();
                foreach (var license in terminatedLicenseList)
                {
                    #region Construct Email
                    var client = db.Clients.FirstOrDefault(c => c.ClientId == license.ClientId && c.IsDeleted == false && c.IsActive);

                    if (client == null) continue;
                    if (template != null)
                    {
                        body = template.EmailBody;
                    }

                    //L.M.20150303a - Replace variables with actual email content
                    body = body.Replace("#NAME#", client.Name + ' ' + client.Surname);
                    body = body.Replace("#BODYTEXT#", "Your license application has been terminated, contact relevant department to re-apply. <br/> Date Terminated: " + _currentDate.ToShortDateString());

                    var email = new tb_EmailQueue
                    {
                        QueueDateTime = DateTime.Now,
                        ApplicationId = applicationId,
                        EmailAccountId = EmailAccountId,
                        ToList = client.EmailAddress,
                        CcList = null,
                        BccList = null,
                        Subject = "TLS: License Termination Notification",
                        Body = body,
                        IsHtml = true,
                        FailureCount = 0,
                        ReferenceId = client.IdentityOrPassportNumber,
                        HasAttachments = false
                    };

                    core.tb_EmailQueue.Add(email);
                    core.SaveChanges();
                    count++;

                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Console.WriteLine(count + " notifications sent at " + DateTime.Now.ToShortDateString());
        }

        /// <summary>
        /// LM.20150216- Gets all licenses left with five days before expiry.
        /// </summary>
        /// <returns>List of licenses</returns>
        public List<C8.TradeLicense.Models.License> GetLicensesRequireFiveDayReminder()
        {

            var licenselist =
                db.Licenses.Where(
                    l =>
                        l.IsDeleted == false && l.IsActive == true &&
                        l.StatusId ==
                        (db.Status.FirstOrDefault(
                            s => s.StatusKey == "LicenseApproved" && s.IsDeleted == false && s.IsActive)).StatusId)
                    .ToList();
            //var licenses = db.Licenses.Where(l => l.IsDeleted == false && l.IsActive).ToList() ;
            var reqList = new List<C8.TradeLicense.Models.License>();
            try
            {
                foreach (var license in licenselist)
                {

                    if (license.LicenseExpiryDate != null)
                    {
                        var totalDays = GetNumberOfWorkingDays((DateTime)license.LicenseExpiryDate, _currentDate);
                        //var ts = (TimeSpan)(CurrentDate - license.LicenseExpiryDate);
                        if (totalDays == 5)
                        {
                            reqList.Add(license);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reqList;
        }

        /// <summary>
        /// L.M.20150216- Get all inspection requests overdue.
        /// </summary>
        /// <returns>List of inspection request.</returns>
        public List<C8.TradeLicense.Models.InspectionRequest> GetAllOverDueInspectionRequests()
        {
            var statusId =
                db.Status.FirstOrDefault(
                    s => s.StatusKey == "PendingLicenseInspection" && s.IsDeleted == false && s.IsActive).StatusId;
            var inspectionList =
                db.InspectionRequests.Where(i => i.StatusId == statusId && i.IsDeleted == false && i.IsActive).ToList();
            var reqlist = new List<C8.TradeLicense.Models.InspectionRequest>();
            try
            {
                foreach (var inspectionRequest in inspectionList)
                {

                    var dep =
                        db.Departments.FirstOrDefault(
                            d => d.DepartmentId == inspectionRequest.DepartmentId && d.IsDeleted == false && d.IsActive);
                    //var totalDays = GetNumberOfWorkingDays((DateTime)inspectionRequest.CreatedDateTime, _currentDate);
                    //var standardSla = (dep != null && dep.DepartmentKey == "ETK555"
                    //    ? BusinessLicensingSlaNumDays
                    //    : OtherDepartmentSlaNumDays);
                    //var slaDays = GetSlaDays(standardSla, totalDays);


                    //if (dep != null && totalDays == slaDays)
                    //{
                    //    reqlist.Add(inspectionRequest);
                    //}

                    reqlist.Add(inspectionRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return reqlist;
        }

        /// <summary>
        /// Count how many working days between start date and end date.
        /// </summary>
        /// <param name="start">date of the license application</param>
        /// <param name="stop">Current Date</param>
        /// <returns></returns>
        public static int GetNumberOfWorkingDays(DateTime start, DateTime stop)
        {
            int days = 0;
            while (start.Date <= stop.Date)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    ++days;
                }
                start = start.AddDays(1);
            }
            return days;
        }

        /// <summary>
        /// LM.20150729a - Gets SLA days
        ///                While total days a request is outstanding > Standard SLA, increments sla by its value 
        /// </summary>
        /// <param name="slaDays">Standard Department SLA</param>
        /// <param name="totalDays">Actual outstanding days</param>
        /// <returns></returns>
        public static int GetSlaDays(int slaDays, int totalDays)
        {
            try
            {
                var standardSla = slaDays;
                while (slaDays < totalDays)
                {
                    slaDays = slaDays + standardSla;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return slaDays;
        }

        /// <summary>
        /// L.M.20150819a - Gets all users with Administrator Role
        /// </summary>
        /// <returns>List of users</returns>
        public static List<TradeLicense.Models.User> GetAdministrators()
        {
            var identifyManager = new IdentityManager();
            var adminList = new List<IdentityUserRole>();
            var db = new TradeLicenseDbContext();
            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                adminList = roleManager.FindByName("Administrators").Users.ToList();
                var userDetails = new List<TradeLicense.Models.User>();

                foreach (var user in adminList)
                {
                    var userId = user.UserId;
                    var currentUser = identifyManager.CurrentUser(userId);

                    var sysUser = db.Users.FirstOrDefault(u => u.UserId == currentUser.UserId);
                    userDetails.Add(sysUser);

                }
                return userDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// L.M.20150819a - Notifies system administrator
        /// </summary>
        public static void NotifyAdmin()
        {
            try
            {
                var core = new CommonCore_GoLiveEntities();
                int applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && !a.IsDeleted && a.IsActive).ApplicationID;
                var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
                var body = string.Empty;

                if (template != null)
                {
                    body = template.EmailBody;
                }

                //L.M.20150303a - Replace variables with actual email content
                body = body.Replace("#NAME#", "Administrator");
                body = body.Replace("#BODYTEXT#",
                   "Urgent: Please note Trade License Service on " + Environment.MachineName + " has stopped at " + DateTime.Now);

                #region Build To List

                //var adminList = GetAdministrators();
                const string adminList = "user@example.com; user@example.com;user@example.com;user@example.com";
                //var toList = string.Empty;

                //if (null != adminList)
                //{
                //    foreach (var user in adminList)
                //    {
                //        if (toList.Length == 0)
                //        {
                //            toList = toList + user.EmailAddress;
                //        }
                //        toList = toList + ";" + user.EmailAddress;
                //    }
                //}
                #endregion
                var email = new tb_EmailQueue
                {
                    QueueDateTime = DateTime.Now,
                    ApplicationId = applicationId,
                    EmailAccountId = 5, //Hard coded => EAcountAdmin
                    ToList = adminList,
                    CcList = null,
                    BccList = null,
                    Subject = "TLS: Service Observer",
                    Body = body,
                    IsHtml = true,
                    FailureCount = 0,
                    ReferenceId = "-2",
                    HasAttachments = false
                };

                core.tb_EmailQueue.Add(email);
                core.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

    }
}
