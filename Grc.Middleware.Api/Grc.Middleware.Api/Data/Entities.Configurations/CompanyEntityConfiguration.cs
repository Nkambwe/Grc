﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Configurations {
    public class CompanyEntityConfiguration {

        public static void Configure(EntityTypeBuilder<Company> builder) {
            builder.ToTable("TBL_GRC_COMPANY");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CompanyName).HasColumnName("Company_name").HasMaxLength(250).IsRequired();
            builder.Property(c => c.ShortName).HasColumnName("Alias").HasMaxLength(250).IsRequired();
            builder.Property(c => c.RegistrationNumber).HasColumnName("Reg_number").HasMaxLength(50).IsRequired(false);
            builder.Property(c => c.SystemLanguage).HasColumnName("Language").HasMaxLength(10).IsRequired(false);
            builder.Property(c => c.IsDeleted).HasColumnName("Is_Deleted");
            builder.Property(c => c.CreatedOn).HasColumnName("Created_on").IsRequired();
            builder.Property(c => c.CreatedBy).HasColumnName("Created_by").HasMaxLength(10).IsFixedLength().IsRequired();
            builder.Property(c => c.LastModifiedOn).HasColumnName("Modefied_on").IsRequired(false);
            builder.Property(c => c.LastModifiedBy).HasColumnName("Modified_by").HasMaxLength(10).IsFixedLength().IsRequired(false);
            
        }
    }
}
