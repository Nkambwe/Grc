using Grc.Middleware.Api.Data.Entities.Configurations;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data {

    public class GrcContext : DbContext {
        public DbSet<Company> Organizations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<SystemError> SystemErrors { get; set; }
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

        public GrcContext(DbContextOptions<GrcContext> options)  
            : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            CompanyEntityConfiguration.Configure(modelBuilder.Entity<Company>());
            BranchEntityConfiguration.Configure(modelBuilder.Entity<Branch>());
            SystemErrorEntityConfiguration.Configure(modelBuilder.Entity<SystemError>());
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
            
            base.OnModelCreating(modelBuilder);
        }
    }

}
