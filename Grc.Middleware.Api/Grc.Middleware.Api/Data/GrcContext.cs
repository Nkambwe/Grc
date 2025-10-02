using Grc.Middleware.Api.Data.Entities.Compliance;
using Grc.Middleware.Api.Data.Entities.Configurations;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
        public DbSet<GuideDocument> GuideDocuments { get; set; }
        public DbSet<GuideDocumentType> DocumentTypes { get; set; }
        public DbSet<Responsibility> Responsibilities { get; set; }
        public DbSet<ReturnType> ReturnTypes { get; set; }
        public DbSet<StatutoryArticle> StatutoryArticles { get; set; }
        public DbSet<StatutoryRegulation> StatutoryRegulations { get; set; }

        public GrcContext(DbContextOptions<GrcContext> options)  
            : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            CompanyEntityConfiguration.Configure(modelBuilder.Entity<Company>());
            BranchEntityConfiguration.Configure(modelBuilder.Entity<Branch>());
            SystemErrorEntityConfiguration.Configure(modelBuilder.Entity<SystemError>());
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
            GuideDocumentEntityConfiguration.Configure(modelBuilder.Entity<GuideDocument>());
            GuideDocumentTypeEntityConfiguration.Configure(modelBuilder.Entity<GuideDocumentType>());
            ResponsibilityEntityConfiguration.Configure(modelBuilder.Entity<Responsibility>());
            ReturnTypeEntityConfiguration.Configure(modelBuilder.Entity<ReturnType>());
            StatutoryArticleEntityConfiguration.Configure(modelBuilder.Entity<StatutoryArticle>());
            StatutoryRegulationEntityConfiguration.Configure(modelBuilder.Entity<StatutoryRegulation>());

            base.OnModelCreating(modelBuilder);
        }
    }

}
