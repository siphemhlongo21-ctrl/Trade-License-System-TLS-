using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Web.UI.WebControls;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.SqlClient;

namespace C8.TradeLicense.DataAccessLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class TradeLicenseDbContext : IdentityDbContext<ApplicationUser>
    {
        public TradeLicenseDbContext()
            : base("TradeLicenseDbContext")
        { }

        public TradeLicenseDbContext(User currentUser)
            : base("TradeLicenseDbContext")
        {
            CurrentUser = currentUser;
        }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<LookupTable> LookupTable { get; set; }
        public DbSet<EscalationLog> EscalationLog { get; set; }
        public DbSet<EscalationMainTable> EscalationMainTable { get; set; }
        public User CurrentUser { get; set; }
        public DbSet<ChiefReview> ChiefReview { get; set; }
        public DbSet<RevokeCancelReview> RevokeCancelReview { get; set; }
        public DbSet<NumOfDays> NumOfDays { get; set; }
        public DbSet<InspectionAppealsReviews> InspectionAppealsReviews { get; set; }

        public DbSet<Refusal> Refusa { get; set; }
        public DbSet<PublicHolidays> PublicHolidays { get; set; }
        public DbSet<InspectionAppeal> InspectionAppeal { get; set; }
        public DbSet<LicensesRevoked> LicensesRevoked { get; set; }
        public DbSet<InspectionHistory> InspectionHistory { get; set; }
        public DbSet<ManagerReview> ManagerRevies { get; set; }
        public DbSet<AdministratorReview> AdministratorReviews { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessOperationType> BusinessOperationTypes { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<OperationStructureType> OperationStructureTypes { get; set; }
        public DbSet<TitleDeedType> TitleDeedTypes { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Models.License> Licenses { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<Audit> Audit { get; set; }
        public DbSet<InspectionRequest> InspectionRequests { get; set; }
        public DbSet<InspectionResponse> InspectionResponse { get; set; }
        public DbSet<DocumentCheckList> DocumentCheckLists { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentContact> DepartmentContacts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Models.FileUpload> FileUploads { get; set; }
        public DbSet<Models.BusinessOperator> BusinessOperators { get; set; }
        public DbSet<Models.ItemType> ItemTypes { get; set; }
        public DbSet<Models.ItemSubCategory> ItemSubCategories { get; set; }
        public DbSet<ItemCondition> ItemConditions { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<LicenseApplicationConditions> LicenseApplicationConditions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<RenewalHistory> RenewalHistory { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<MigratedClient> MigratedClient { get; set; }
        public DbSet<MigratedLicense> MigratedLicense { get; set; }
        public DbSet<MigratedBusiness> MigratedBusiness { get; set; }
        public DbSet<PaymentLicense> PaymentLicense { get; set; }
        public System.Data.Entity.DbSet<ServiceLevelAgreement> ServiceLevelAgreements { get; set; }
        public DbSet<DepartmentServiceLevelAgreement> DepartmentServiceLevelAgreements { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ViewSettings> ViewSettings { get; set; }
        public DbSet<BusinessManger> BusinessManger { get; set; }

        public DbSet<BusinessEmployee> BusinessEmployee { get; set; }
        public DbSet<Amendment> Amendments { get; set; }

        public DbSet<DropdownItem> DropdownItems { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<LicenseAmendment> LicenseAmendments { get; set; }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// The number of objects written to the underlying database.
        /// </returns>
        public virtual int cmn_casecounter_sp(Nullable<int> org_process_id, string seq_name, ObjectParameter case_desc)
        {
            var org_process_idParameter = org_process_id.HasValue ?
                new ObjectParameter("org_process_id", org_process_id) :
                new ObjectParameter("org_process_id", typeof(int));

            var seq_nameParameter = seq_name != null ?
                new ObjectParameter("seq_name", seq_name) :
                new ObjectParameter("seq_name", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("cmn_casecounter_sp", org_process_idParameter, seq_nameParameter, case_desc);
        }

        public void SaveAmendments(string amendmentType, int licenseId)
        {
            var auditableTables = new HashSet<string>
    {
        "Clients",
        "Businesses",
        "Licenses"
    };

            var modifiedEntries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in modifiedEntries)
            {
                var tableName = GetTableName(entry.Entity.GetType());

                if (!auditableTables.Contains(tableName))
                    continue;

                var databaseValues = entry.GetDatabaseValues();

                if (databaseValues == null)
                    continue;

                foreach (var propertyName in entry.CurrentValues.PropertyNames)
                {
                    var currentValue = entry.CurrentValues[propertyName];
                    var originalValue = databaseValues[propertyName];

                    if (object.Equals(currentValue, originalValue))
                        continue;

                    Amendments.Add(new Amendment
                    {
                        TableAmended = tableName,
                        FieldAmended = propertyName,
                        OldData = originalValue?.ToString(),
                        NewData = currentValue?.ToString(),
                        AmendmentType = amendmentType,
                        LicenseId = licenseId,
                        IsActive = true,
                        IsLocked = false,
                        IsDeleted = false
                    });
                }
            }
        }


        public override int SaveChanges()
        {
            var changeCount = 0;
            try
            {

                var transactionDateTime = DateTime.Now;
                var changeSet = ChangeTracker.Entries();
                int? currentUserId = CurrentUser != null ? CurrentUser.UserId : (int?)null;
                string entityState = string.Empty;
                string tableName = string.Empty;
                List<AuditEnitity> auditEntities = new List<AuditEnitity>();

                foreach (var entry in changeSet.Where(e => e.Entity is IAuditable))
                {
                    int licenseId = 0;
                    Type entryEntityType = entry.Entity.GetType();
                    string primaryKeyName = GetPrimaryKeyProperty(entryEntityType);
                    int primaryKey = Convert.ToInt32(entryEntityType.GetProperty(primaryKeyName).GetValue(entry.Entity));
                    if (entryEntityType.GetProperty("LicenseId") != null)
                        licenseId = Convert.ToInt32(entryEntityType.GetProperty("LicenseId").GetValue(entry.Entity));
                    entityState = entry.State.ToString();
                    tableName = GetTableName(entryEntityType);

                    ((IAuditable)entry.Entity).CreatedByUserId = currentUserId;
                    ((IAuditable)entry.Entity).CreatedDateTime = transactionDateTime;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            ((IAuditable)entry.Entity).CreatedByUserId = currentUserId;
                            ((IAuditable)entry.Entity).CreatedDateTime = transactionDateTime;
                            foreach (var propertyName in entry.CurrentValues.PropertyNames)
                            {
                                auditEntities.Add(new AuditEnitity()
                                {
                                    Audit = new Audit()
                                    {
                                        Action = entityState,
                                        TableName = tableName,
                                        PrimaryKey = primaryKey,
                                        LicenseId = licenseId,
                                        ColumnName = propertyName,
                                        CurrentValue = Convert.ToString(entry.CurrentValues[propertyName]),
                                        OriginalValue = string.Empty,
                                        AuditByUserId = currentUserId,
                                        AuditDateTime = transactionDateTime,

                                    },
                                    Entity = entry.Entity
                                });
                            }
                            break;
                        case EntityState.Modified:
                        case EntityState.Deleted:
                            ((IAuditable)entry.Entity).ModifiedByUserId = currentUserId;
                            ((IAuditable)entry.Entity).ModifiedDateTime = transactionDateTime;
                            foreach (var propertyName in entry.CurrentValues.PropertyNames)
                            {
                                if ((entry.CurrentValues[propertyName] != null && entry.GetDatabaseValues()[propertyName] != null) && !entry.CurrentValues[propertyName].Equals(entry.GetDatabaseValues()[propertyName]))
                                    auditEntities.Add(new AuditEnitity()
                                    {
                                        Audit = new Audit()
                                        {
                                            Action = entityState,
                                            TableName = tableName,
                                            PrimaryKey = primaryKey,
                                            LicenseId = licenseId,
                                            ColumnName = propertyName,
                                            CurrentValue = Convert.ToString(entry.CurrentValues[propertyName]),
                                            OriginalValue = Convert.ToString(entry.OriginalValues[propertyName]),
                                            AuditByUserId = currentUserId,
                                            AuditDateTime = transactionDateTime
                                        },
                                        Entity = entry.Entity
                                    });
                            }
                            break;
                    }
                    //if(licenseID != null)
                    //{
                    //    Amendmens = new Amendment()
                    //    {
                    //        LicenseId = int.Parse(licenseID.ToString()),
                    //        PrimaryKey = primaryKey

                    //    },


                    //}
                }

                // var changeCount = base.SaveChanges();
                changeCount = base.SaveChanges();

                // JK.20140902a - Auditing is processed here.
                foreach (var auditEnitity in auditEntities)
                {
                    if (auditEnitity.Entity != null)
                    {
                        Type entryEntityType = auditEnitity.Entity.GetType();
                        string primaryKeyName = GetPrimaryKeyProperty(entryEntityType);
                        int primaryKey =
                            Convert.ToInt32(entryEntityType.GetProperty(primaryKeyName).GetValue(auditEnitity.Entity));

                        auditEnitity.Audit.PrimaryKey = primaryKey;
                    }

                    Audit.Add(auditEnitity.Audit);
                }

                // Save audit trail.
                base.SaveChanges();


            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            return changeCount;
        }

        private static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Entity type not found in GetTableName</exception>
        private EntitySetBase GetEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;
                string typeName = ObjectContext.GetObjectType(type).Name;
                var es =
                    octx.MetadataWorkspace.GetItemCollection(DataSpace.SSpace).GetItems<EntityContainer>().SelectMany(
                        c => c.BaseEntitySets.Where(e => e.Name == typeName)).FirstOrDefault();

                if (es == null)
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }

        /// <summary>
        /// Gets the name of the table of the entity type.
        /// Used for auditing entities.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private string GetTableName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);
            return string.Format("{0}", es.MetadataProperties["Table"].Value);
        }

        /// <summary>
        /// Gets the primary key property of an entity type.
        /// Used to get the value of the primary key for auditing.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private string GetPrimaryKeyProperty(Type type)
        {
            EntitySetBase es = GetEntitySet(type);
            return es.ElementType.KeyMembers[0].Name;
        }

        /*
         * Code First Migrations has two primary commands that you are going to become familiar with. 
         * Add-Migration will scaffold the next migration based on changes you have made to your model since the last migration was created 
         * Update-Database will apply any pending migrations to the database
         * We need to scaffold a migration to take care of the new Url property we have added. 
         * The Add-Migration command allows us to give these migrations a name.
         * 
         * Run the Update-Database command in Package Manager Console to apply changes to the database.
         */
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JK.20140902a - Include this to remove cascade deletions.
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<User>()
                .HasOptional(f => f.CreatedByUser)
                .WithMany()
                .HasForeignKey(f => f.CreatedByUserId);

            modelBuilder.Entity<User>()
                .HasOptional(f => f.ModifiedByUser)
                .WithMany()
                .HasForeignKey(f => f.ModifiedByUserId);
        }


    }
}