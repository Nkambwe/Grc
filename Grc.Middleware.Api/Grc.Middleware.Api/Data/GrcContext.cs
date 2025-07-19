using Grc.Middleware.Api.Data.Entities.Configurations;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data {

    public class GrcContext : DbContext {
        public DbSet<Company> Organizations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentUnit> DepartmentUnits { get; set; }
        public DbSet<SystemRoleGroup> RoleGroups { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }

        public GrcContext(DbContextOptions<GrcContext> options)  
            : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            CompanyEntityConfiguration.Configure(modelBuilder.Entity<Company>());
            BranchEntityConfiguration.Configure(modelBuilder.Entity<Branch>());
            DepartmentEntityConfiguration.Configure(modelBuilder.Entity<Department>());
            DepartmentUnitEntityConfiguration.Configure(modelBuilder.Entity<DepartmentUnit>());
            SystemRoleGroupEntityConfiguration.Configure(modelBuilder.Entity<SystemRoleGroup>());
            SystemRoleEntityConfiguration.Configure(modelBuilder.Entity<SystemRole>());
            SystemUserEntityConfiguration.Configure(modelBuilder.Entity<SystemUser>());
            base.OnModelCreating(modelBuilder);
        }
    }

}
