using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.Configurations;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data {

    public class GrcContext : DbContext {
        public DbSet<Company> Organizations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<SystemError> SystemErrors { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<ActivityLogSetting> ActivitySettings { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentUnit> DepartmentUnits { get; set; }
        public DbSet<SystemRoleGroup> RoleGroups { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<UserPrefference> UserPrefferences { get; set; }
        public DbSet<UserView> UserViews { get; set; }
        public DbSet<LoginAttempt> Attempts { get; set; }
        public DbSet<UserQuickAction> QuickActions { get; set; }
        public DbSet<UserPinnedItem> PinnedItems { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        public DbSet<Frequency> Frequencies { get; set; }
        public DbSet<RegulatoryCategory> RegulatoryCategories { get; set; }
        public DbSet<RegulatoryReturn> RegulatoryReturns { get; set; }
        public DbSet<RegulatoryType> RegulatoryTypes { get; set; }
        public DbSet<ReturnSubmission> CircularSubmissions { get; set; }
        public DbSet<RegulatoryDocument> GuideDocuments { get; set; }
        public DbSet<RegulatoryDocumentType> DocumentTypes { get; set; }
        public DbSet<Responsebility> Responsibilities { get; set; }
        public DbSet<ReturnType> ReturnTypes { get; set; }
        public DbSet<StatutoryArticle> StatutoryArticles { get; set; }
        public DbSet<StatutoryRegulation> StatutoryRegulations { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<AuditTask> AuditTasks { get; set; }
        public DbSet<AuditException> AuditExceptions { get; set; }
        public DbSet<AuditReport> AuditReports { get; set; }
        public DbSet<ProcessTag> ProcessTypes { get; set; }
        public DbSet<ProcessTag> ProcessTags { get; set; }
        public DbSet<ProcessGroup> ProcessGroups { get; set; }
        public DbSet<ProcessActivity> ProcessActivities { get; set; }
        public DbSet<ProcessTask> ProcessTasks { get; set; }
        public DbSet<OperationProcess> Processes { get; set; }
        public DbSet<SubmissionNotification> Notifications { get; set; }
        public DbSet<SystemConfiguration> SystemConfiguration { get; set; }
        public DbSet<SystemPermission> SystemPermissions { get; set; }
        public DbSet<SystemPermissionPermissionSet> SystemPermissionSets { get; set; }
        
        public GrcContext(DbContextOptions<GrcContext> options)  
            : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            CompanyEntityConfiguration.Configure(modelBuilder.Entity<Company>());
            BranchEntityConfiguration.Configure(modelBuilder.Entity<Branch>());
            SystemErrorEntityConfiguration.Configure(modelBuilder.Entity<SystemError>());
            SystemConfigurationEntityConfiguration.Configure(modelBuilder.Entity<SystemConfiguration>());
            ActivityLogSettingEntityConfiguration.Configure(modelBuilder.Entity<ActivityLogSetting>());
            ActivityTypeEntityConfiguration.Configure(modelBuilder.Entity<ActivityType>());
            ActivityLogEntityConfiguration.Configure(modelBuilder.Entity<ActivityLog>());
            DepartmentEntityConfiguration.Configure(modelBuilder.Entity<Department>());
            DepartmentUnitEntityConfiguration.Configure(modelBuilder.Entity<DepartmentUnit>());
            SystemRoleGroupEntityConfiguration.Configure(modelBuilder.Entity<SystemRoleGroup>());
            SystemRoleEntityConfiguration.Configure(modelBuilder.Entity<SystemRole>());
            SystemUserEntityConfiguration.Configure(modelBuilder.Entity<SystemUser>());
            UserViewEntityConfiguration.Configure(modelBuilder.Entity<UserView>());
            UserPrefferenceEntityConfiguration.Configure(modelBuilder.Entity<UserPrefference>());
            LoginAttemptEntityConfiguration.Configure(modelBuilder.Entity<LoginAttempt>());
            QuickActionEntityConfiguration.Configure(modelBuilder.Entity<UserQuickAction>());
            PinnedItemEntityConfiguration.Configure(modelBuilder.Entity<UserPinnedItem>());
            AuthorityEntityConfiguration.Configure(modelBuilder.Entity<Authority>());
            FrequencyEntityConfiguration.Configure(modelBuilder.Entity<Frequency>());
            RegulatoryCategoryEntityConfiguration.Configure(modelBuilder.Entity<RegulatoryCategory>());
            RegulatoryReturnEntityConfiguration.Configure(modelBuilder.Entity<RegulatoryReturn>());
            RegulatoryTypeEntityConfiguration.Configure(modelBuilder.Entity<RegulatoryType>());
            GuideDocumentEntityConfiguration.Configure(modelBuilder.Entity<RegulatoryDocument>());
            GuideDocumentTypeEntityConfiguration.Configure(modelBuilder.Entity<RegulatoryDocumentType>());
            ResponsibilityEntityConfiguration.Configure(modelBuilder.Entity<Responsebility>());
            ReturnTypeEntityConfiguration.Configure(modelBuilder.Entity<ReturnType>());
            StatutoryArticleEntityConfiguration.Configure(modelBuilder.Entity<StatutoryArticle>());
            StatutoryRegulationEntityConfiguration.Configure(modelBuilder.Entity<StatutoryRegulation>());
            ReturnSubmissionEntityConfiguration.Configure(modelBuilder.Entity<ReturnSubmission>());
            SubmissionNotificationEntityConfiguration.Configure(modelBuilder.Entity<SubmissionNotification>());
            AuditEntityConfiguration.Configure(modelBuilder.Entity<Audit>());
            AuditTaskEntityConfiguration.Configure(modelBuilder.Entity<AuditTask>());
            AuditExceptionEntityConfiguration.Configure(modelBuilder.Entity<AuditException>());
            AuditReportEntityConfiguration.Configure(modelBuilder.Entity<AuditReport>());
            ProcessTagEntityConfiguration.Configure(modelBuilder.Entity<ProcessTag>());
            ProcessTypeEntityConfiguration.Configure(modelBuilder.Entity<ProcessType>());
            ProcessGroupEntityConfiguration.Configure(modelBuilder.Entity<ProcessGroup>());
            ProcessActivityEntityConfiguration.Configure(modelBuilder.Entity<ProcessActivity>());
            ProcessTaskEntityConfiguration.Configure(modelBuilder.Entity<ProcessTask>());
            OperationProcessEntityConfiguration.Configure(modelBuilder.Entity<OperationProcess>());
            ProcessProcessTagEntityConfiguration.Configure(modelBuilder.Entity<ProcessProcessTag>());
            ProcessProcessGroupEntityConfiguration.Configure(modelBuilder.Entity<ProcessProcessGroup>());
            SystemPermissionEntityConfiguration.Configure(modelBuilder.Entity<SystemPermission>());
            SystemPermissionSetEntityConfiguration.Configure(modelBuilder.Entity<SystemPermissionSet>());
            SystemPermissionPermissionSetEntityConfiguration.Configure(modelBuilder.Entity<SystemPermissionPermissionSet>());
            SystemRolePermissionSetEntityConfiguration.Configure(modelBuilder.Entity<SystemRolePermissionSet>());
            SystemRoleGroupPermissionSetEntityConfiguration.Configure(modelBuilder.Entity<SystemRoleGroupPermissionSet>());
            base.OnModelCreating(modelBuilder);
        }
    }

}
