namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CesarDbContext : DbContext
    {
        public CesarDbContext()
            : base("name=CesarDbContext")
        {
        }

        public virtual DbSet<tb_Applications> tb_Applications { get; set; }
        public virtual DbSet<tb_AppSettings> tb_AppSettings { get; set; }
        public virtual DbSet<tb_EmailAccounts> tb_EmailAccounts { get; set; }
        public virtual DbSet<tb_EmailAttachmentQueue> tb_EmailAttachmentQueue { get; set; }
        public virtual DbSet<tb_EmailExceptions> tb_EmailExceptions { get; set; }
        public virtual DbSet<tb_EmailExceptionTypes> tb_EmailExceptionTypes { get; set; }
        public virtual DbSet<tb_EmailLogs> tb_EmailLogs { get; set; }
        public virtual DbSet<tb_EmailQueue> tb_EmailQueue { get; set; }
        public virtual DbSet<tb_EmailStaticAttachments> tb_EmailStaticAttachments { get; set; }
        public virtual DbSet<tb_EmailTemplates> tb_EmailTemplates { get; set; }
        public virtual DbSet<tb_EventLogs> tb_EventLogs { get; set; }
        public virtual DbSet<tb_ReadEmailAttachments> tb_ReadEmailAttachments { get; set; }
        public virtual DbSet<tb_ReadEmailItems> tb_ReadEmailItems { get; set; }
        public virtual DbSet<tb_ReferenceTypes> tb_ReferenceTypes { get; set; }
        public virtual DbSet<tb_UnreadEmailAttachments> tb_UnreadEmailAttachments { get; set; }
        public virtual DbSet<tb_UnreadEmailItems> tb_UnreadEmailItems { get; set; }
        public virtual DbSet<tb_EmailAttachmentQueueProcessed> tb_EmailAttachmentQueueProcessed { get; set; }
        public virtual DbSet<tb_EmailQueueProcessed> tb_EmailQueueProcessed { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tb_Applications>()
                .Property(e => e.ApplicationKey)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Applications>()
                .Property(e => e.ApplicationName)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Applications>()
                .Property(e => e.ApplicationDescription)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Applications>()
                .Property(e => e.ApplicationURL)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AppSettings>()
                .Property(e => e.AppKey)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AppSettings>()
                .Property(e => e.AppValue)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AppSettings>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAccounts>()
                .Property(e => e.EmailAccountDescription)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAccounts>()
                .Property(e => e.EmailAddress)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAccounts>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAccounts>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAccounts>()
                .Property(e => e.ServiceUrl)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAttachmentQueue>()
                .Property(e => e.Filename)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAttachmentQueue>()
                .Property(e => e.ContentType)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailExceptions>()
                .Property(e => e.ToList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailExceptions>()
                .Property(e => e.Exception)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailExceptionTypes>()
                .Property(e => e.EmailExceptionTypeName)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailExceptionTypes>()
                .Property(e => e.EmailExceptionTypeDescription)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailExceptionTypes>()
                .Property(e => e.EmailExceptionTypeKey)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailLogs>()
                .Property(e => e.EmailTo)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailLogs>()
                .Property(e => e.EmailFrom)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailLogs>()
                .Property(e => e.EmailSubject)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailLogs>()
                .Property(e => e.EmailBody)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueue>()
                .Property(e => e.ToList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueue>()
                .Property(e => e.CcList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueue>()
                .Property(e => e.BccList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueue>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueue>()
                .Property(e => e.ReferenceId)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailStaticAttachments>()
                .Property(e => e.Filename)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailStaticAttachments>()
                .Property(e => e.ContentType)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.EmailTemplateName)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.EmailTemplateDescription)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.EmailTemplateKey)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.EmailFrom)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.EmailSubject)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.EmailBody)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.Variables)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.ToVariable)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailTemplates>()
                .Property(e => e.CcVariable)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EventLogs>()
                .Property(e => e.EventData)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailAttachments>()
                .Property(e => e.Filename)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailAttachments>()
                .Property(e => e.ContentType)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailItems>()
                .Property(e => e.FromList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailItems>()
                .Property(e => e.ToList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailItems>()
                .Property(e => e.CcList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailItems>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReadEmailItems>()
                .Property(e => e.Body)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReferenceTypes>()
                .Property(e => e.ReferenceTypeName)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReferenceTypes>()
                .Property(e => e.ReferenceTypeDescription)
                .IsUnicode(false);

            modelBuilder.Entity<tb_ReferenceTypes>()
                .Property(e => e.ReferenceTypeKey)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailAttachments>()
                .Property(e => e.Filename)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailAttachments>()
                .Property(e => e.ContentType)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailItems>()
                .Property(e => e.FromList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailItems>()
                .Property(e => e.ToList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailItems>()
                .Property(e => e.CcList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailItems>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<tb_UnreadEmailItems>()
                .Property(e => e.Body)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAttachmentQueueProcessed>()
                .Property(e => e.Filename)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailAttachmentQueueProcessed>()
                .Property(e => e.ContentType)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueueProcessed>()
                .Property(e => e.ToList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueueProcessed>()
                .Property(e => e.CcList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueueProcessed>()
                .Property(e => e.BccList)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueueProcessed>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<tb_EmailQueueProcessed>()
                .Property(e => e.ReferenceId)
                .IsUnicode(false);
        }
    }
}
