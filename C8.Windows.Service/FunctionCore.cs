using System;
using System.Collections.Generic;
using System.Linq;
using C8.Windows.Service;
using C8.TradeLicense;

namespace C8.Windows.Service
{
    public class FunctionCore
    {
        //private static TradeLicenseEntitiesDataContext db = new TradeLicenseEntitiesDataContext();
        private static C8.TradeLicense.DataAccessLayer.TradeLicenseDbContext db = new TradeLicense.DataAccessLayer.TradeLicenseDbContext();
        private static readonly DateTime _currentDate = DateTime.Now;
        private const int PendingInspectionSlaNumDays = 90; // Entire license application process SLA

        /// <summary>
        /// LM.20150309a - Process background tasks
        ///              e.g License application termination
        /// </summary>
        public void ProcessBackgroundTasks()
        {
            try
            {
                #region Terminating Licenses
                System.Console.WriteLine("Performing backgound tasks...");
                var licenses = GetLicensesRequireTermination();
                var status = db.Status.FirstOrDefault(s => s.StatusKey == "LicenseApplicationTerminated" && s.IsDeleted == false && s.IsActive);

                if (status == null) return;
                var statusId = status.StatusId;

                foreach (var license in licenses)
                {
                    license.IsActive = false;
                    license.IsDeleted = true;
                    license.StatusId = statusId;

                    license.ModifiedByUserId = -1;
                    license.ModifiedDateTime = _currentDate;

                    // Gets all requests sent out.
                    // Deleted all relevent requests.
                    var requests = db.InspectionRequests.Where(r => r.LicenseId == license.LicenseId && (!r.IsDeleted) && r.IsActive).ToList();
                    if (requests.Count > 0)
                    {
                        foreach (var request in requests)
                        {
                            request.IsActive = false;
                            request.IsDeleted = true;
                            request.StatusId = statusId;

                            request.ModifiedByUserId = -1;
                            request.ModifiedDateTime = _currentDate;
                        }
                    }
                    // Commit changes 
                    db.SaveChanges();
                    //db.SubmitChanges();
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LM.20150309a - All licenses require termination
        /// </summary>
        /// <returns>Licenses</returns>
        public static List<C8.TradeLicense.Models.License> GetLicensesRequireTermination()
        {
            try
            {
                var requireTerminationList = new List<C8.TradeLicense.Models.License>();
                var status = db.Status.FirstOrDefault(s => s.StatusKey == "PendingLicenseInspection" && s.IsDeleted == false && s.IsActive);
                var statusId = 0;
                if (null != status)
                {
                    statusId = status.StatusId;
                }
                var licenseList = db.Licenses.Where(l => l.StatusId == statusId && l.IsDeleted == false && l.IsActive).ToList();

                foreach (var license in licenseList)
                {
                    var totalDays = NotificationCore.GetNumberOfWorkingDays((DateTime)license.ApplicationDateTime, _currentDate);
                    if (totalDays == PendingInspectionSlaNumDays)
                    {
                        requireTerminationList.Add(license);
                    }
                }

                return requireTerminationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
