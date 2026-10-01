using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using License = C8.TradeLicense.Models.License;

namespace C8.TradeLicense.Helpers
{
    public static class LicenseCirculationHelper
    {
        private static TradeLicenseDbContext _dbContext = new TradeLicenseDbContext();
        private static int EmailAccountId = Convert.ToInt32(ConfigurationManager.AppSettings["EmailAccountId"]);

        public static string GetCirculationStatus(int circulationId)
        {
            var circulation = _dbContext.InspectionRequests.Include(ir => ir.Status).FirstOrDefault(c => c.InspectionRequestId == circulationId);
            if (circulation != null)
            {
                return circulation.Status.StatusName;
            }
            return "Unknown";
        }

        /// <summary>
        /// Re-circulates an existing license inspection request to a specified department, deactivating the original
        /// request and creating a new one for the target department. Also transfers related documents and notifies
        /// relevant department contacts.
        /// </summary>
        /// <remarks>This method deactivates and marks the original inspection request as deleted before
        /// creating a new request for the target department. All associated documents are reassigned to the new
        /// request, and department managers and clerks are notified via email. The method should be called within a
        /// valid web request context if email notifications with action links are required.</remarks>
        /// <param name="circulationId">The unique identifier of the inspection request to be re-circulated.</param>
        /// <param name="targetDepartmentId">The identifier of the department to which the license inspection request will be re-circulated.</param>
        /// <param name="controller">An optional controller instance used to generate URLs for notification emails. If null, URL generation for
        /// notifications may not occur.</param>
        public static void ReCirculateLicense(int circulationId, int targetDepartmentId, string reason, Controller controller = null)
        {
            var circulation = _dbContext.InspectionRequests.FirstOrDefault(c => c.InspectionRequestId == circulationId);
            if (circulation != null)
            {
                circulation.IsActive = false;
                circulation.IsDeleted = true;
                circulation.Comment = reason;
                circulation.ModifiedDateTime = DateTime.Now;
                _dbContext.InspectionRequests.AddOrUpdate(circulation);
                _dbContext.SaveChanges();


                    InspectionRequest newCirculation = new InspectionRequest
                    {
                        LicenseId = circulation.LicenseId,
                        DepartmentContactId = 0, // Assuming no specific contact for the new circulation, to be assigned later by Department Manager
                        RefNumber = circulation.RefNumber,
                    Comment = null,
                        AllocatedDate = DateTime.Now,
                        LicenseTypeId = circulation.LicenseTypeId,
                        DepartmentId = targetDepartmentId,
                        ClientId = circulation.ClientId,
                        StatusId = circulation.StatusId,
                    SlaExpiryDate = DateTime.Now.AddDays(90), // Example SLA
                    IsActive=true,
                    IsDeleted=false,
                    IsLocked=false
                    };
                    _dbContext.InspectionRequests.Add(newCirculation);
                    _dbContext.SaveChanges();

                    List<FileUpload> inspectionRequestDocuments = _dbContext.FileUploads.Where(f => f.referenceId == circulationId).ToList();
                    foreach (var document in inspectionRequestDocuments)
                    {
                        document.referenceId = newCirculation.InspectionRequestId;
                        _dbContext.FileUploads.AddOrUpdate(document);
                        _dbContext.SaveChanges();
                    }

                    License license = _dbContext.Licenses.Include(b => b.Business).Include(c => c.Client).FirstOrDefault(l => l.LicenseId == circulation.LicenseId);
                    var depContacts = _dbContext.DepartmentContacts.Where(d => d.DepartmentId == newCirculation.DepartmentId && d.IsDeleted == false && d.IsActive && (d.RoleName == RoleKeys.DepartmentManager || d.RoleName == RoleKeys.DepartmentClerk))
                                     .Include(u => u.User).ToList();
                    string actionLink = controller.Url.Action("DepartmentManagerCreate", "ManagerReviews", new { id = newCirculation.InspectionRequestId, newCirculation.DepartmentId }, controller.Request.Url.Scheme);

                    string emailSubject = $"New License Circulation for {license.Business.ProposedTradeName} - {license.LicenseNumber}";
                    string emailBody = $"A new license circulation has been created for {license.Business.ProposedTradeName} with license number {license.LicenseNumber}. Please review the circulation details and take necessary actions. You can access the circulation details using the following link: {actionLink}";
                    foreach (var contact in depContacts)
                    {
                        // Send notification to contact.User.Email about the new circulation
                        EmailHelper.SendEmail((int)EmailAccountId, contact.User.EmailAddress, emailSubject, contact.User.FullName, emailBody, license.Client.IdentityOrPassportNumber);
                    }
                }

            }
        }
    }
