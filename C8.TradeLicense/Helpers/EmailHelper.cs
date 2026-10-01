using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.DataAccessLayer.CesarDb;
using System;
using System.Linq;

namespace C8.TradeLicense.Helpers
{
    public static class EmailHelper
    {
        public static TradeLicenseDbContext _dbContext = new TradeLicenseDbContext();
        private static CesarDbContext core = new CesarDbContext();
        public static void SendEmail(int emailedAccountId,string toEmail, string subject,string receiptient, string bodyContent,string referenceId)
        {
            // Implement email sending logic here using SMTP or an email service provider
            var applicationId = core.tb_Applications.FirstOrDefault(a => a.ApplicationKey == "trade_license_application" && a.IsDeleted == false && a.IsActive).ApplicationID;
            var template = core.tb_EmailTemplates.FirstOrDefault(t => t.ApplicationID == applicationId && t.IsDeleted == false && t.IsActive);
            var body = string.Empty;

            if (template != null)
            {
                body = template.EmailBody;
            }

            body = body.Replace("#NAME#", receiptient);
            body = body.Replace("#BODYTEXT#", bodyContent);


            var email = new DataAccessLayer.CesarDb.tb_EmailQueue
            {
                QueueDateTime = DateTime.Now,
                ApplicationId = applicationId,
                EmailAccountId = emailedAccountId,
                ToList = toEmail,
                CcList = null,
                BccList = null,
                Subject = subject,
                Body = body,
                IsHtml = true,
                FailureCount = 0,
                ReferenceId = referenceId,
                HasAttachments = false
            };

            core.tb_EmailQueue.Add(email);
            core.SaveChanges();
        }
    }
}