using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace C8.Console.Application
{
    public class FunctionCore
    {
        private static C8.TradeLicense.DataAccessLayer.TradeLicenseDbContext db = new TradeLicense.DataAccessLayer.TradeLicenseDbContext();
        private static readonly DateTime _currentDate = DateTime.Now;
        private const int PendingInspectionSlaNumDays = 90; // Entire license application process SLA

        /// <summary>
        /// LM.20150309a - Process background tasks
        ///              e.g License application termination
        /// </summary>
        public void ProcessBackgroundTasks()
        {
            #region Send Overdue Inspection for Refusal
            System.Console.WriteLine("Performing refusal backgound tasks...");
            List<TradeLicense.Models.InspectionRequest> inspectionsForRefusal = GetOverdueInspectionRequests();
            int refusal = db.Status.FirstOrDefault(s => s.StatusKey ==
                StatusKeys.RefusalInProgress && s.IsDeleted == false && s.IsActive)?.StatusId ?? 0;
            int inspectionOverdue = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.InspectionOverdue && s.IsActive)?.StatusId ?? 0;
            if (inspectionOverdue > 0 || refusal > 0)
            {
                foreach (var inspection in inspectionsForRefusal)
                {
                    inspection.StatusId = inspectionOverdue;
                    inspection.ModifiedDateTime = _currentDate;
                    db.Entry(inspection).State = EntityState.Modified;
                    TradeLicense.Models.License license = db.Licenses.FirstOrDefault(x => x.LicenseId == inspection.LicenseId);
                    license.StatusId = refusal;
                    license.ModifiedDateTime = _currentDate;
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            
            #endregion
            #region Abandon Licenses
            System.Console.WriteLine("Performing abandon backgound tasks...");
            var licensesToAbandon = GetLicensesWithoutAppealAfter21Days();
            var abandonStatusId = db.Status.FirstOrDefault(s => s.StatusKey == 
                StatusKeys.AppealAbandoned && s.IsDeleted == false && s.IsActive)?.StatusId ?? 0;

            if (abandonStatusId > 0)
            {
                foreach (var license in licensesToAbandon)
                {
                    license.StatusId = abandonStatusId;
                    license.ModifiedDateTime = _currentDate;
                    db.Entry(license).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }           
            #endregion
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
                    //db.SubmitChanges();
                    db.SaveChanges();
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
        public static List<TradeLicense.Models.License> GetLicensesRequireTermination()
        {
            try
            {
                var requireTerminationList = new List<TradeLicense.Models.License>();
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
        public static List<TradeLicense.Models.License> GetLicensesWithoutAppealAfter21Days()
        {
            List<int> licenseIds = new List<int>();
            List<int> inspectionRequestIdsOver21Days = GetInspectionRequestsAtAppealFinalDate();
            if (inspectionRequestIdsOver21Days.Count > 0)
            {
                List<Refusal> refusalsOver21Days = GetRefusalsForInspectionRequest(inspectionRequestIdsOver21Days);
                if (refusalsOver21Days.Count > 0)
                {
                    for (int i = 0; i < refusalsOver21Days.Count; i++)
                    {
                        //add licenseid to list if refusal does not have appeal
                        if (!db.InspectionAppeal.Select(x => x.InspectionResponseId).ToList().Contains(refusalsOver21Days[i].InspectionResponseId))
                        {
                            licenseIds.Add(refusalsOver21Days[i].LicenseId);
                        }
                    }
                }
            }
            return db.Licenses.Where(x => licenseIds.Contains(x.LicenseId)).ToList();
        }
        public static List<TradeLicense.Models.InspectionRequest> GetOverdueInspectionRequests()
        {
            List<int> inspectionRequestIdsOver21DaysWithoutResponse = new List<int>();
            List<int> inspectionRequestIdsOver21Days = GetInspectionRequestsAtResponseDate();
            if (inspectionRequestIdsOver21Days.Count > 0)
            {
                for (int i = 0; i < inspectionRequestIdsOver21Days.Count; i++)
                {
                    //add inspection to list if it does not have a reponse
                    if (!db.InspectionResponse.Select(x => x.InspectionRequestId).ToList().Contains(inspectionRequestIdsOver21Days[i]))
                    {
                        inspectionRequestIdsOver21DaysWithoutResponse.Add(inspectionRequestIdsOver21Days[i]);
                    }
                }
            }
            return db.InspectionRequests.Where(x => inspectionRequestIdsOver21DaysWithoutResponse.Contains(x.InspectionRequestId)).ToList();
        }
        private static List<Refusal> GetRefusalsForInspectionRequest(List<int> inspectionRequestIdsOver21Days)
        {
            //get refusals for insepctions
            return db.Refusa.Include(x => x.License)
                .Where(x => inspectionRequestIdsOver21Days.Contains(x.InspectionRequestId))
                .ToList();
        }

        private static List<int> GetInspectionRequestsAtAppealFinalDate()
        {
            int refusalStatus = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.RefusalInProgress && s.IsDeleted == false && s.IsActive)?.StatusId ?? 0;
            //check inspection requests with appeal final date already reached
            //and license status still at refusal
            List<int> inspectionRequestIdsOver21Days = db.InspectionRequests.Include(x => x.License)
                .Where(x => x.License.StatusId == refusalStatus && x.AppealFinalDate != null && x.AppealFinalDate <= DateTime.Now
                ).Select(x => x.InspectionRequestId).ToList();
            return inspectionRequestIdsOver21Days;
        }
        private static List<int> GetInspectionRequestsAtResponseDate()
        {
            int inspectionPending = db.Status.FirstOrDefault(s => s.StatusKey == StatusKeys.PendingLicenseInspection && s.IsActive)?.StatusId ?? 0;
            //check inspection requests with response final date already reached
            //and inspection still oend
            List<int> inspectionRequestIdsOver21Days = db.InspectionRequests.Include(x => x.License)
                .Where(x => x.StatusId == inspectionPending && x.InspectionFinalDate != null && x.InspectionFinalDate <= DateTime.Now
                ).Select(x => x.InspectionRequestId).ToList();
            return inspectionRequestIdsOver21Days;
        }
    }
}
