using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Data.Entities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data {

    public class GrcContext : DbContext {
        public DbSet<Company> Organizations { get; set; }

        public GrcContext(DbContextOptions<GrcContext> options)  
            : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            CompanyEntityConfiguration.Configure(modelBuilder.Entity<Company>());

            base.OnModelCreating(modelBuilder);
        }
    }

}
