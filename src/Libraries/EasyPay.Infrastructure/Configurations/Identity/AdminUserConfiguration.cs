using EasyPay.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Configurations.Identity
{
    public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> builder)
        {
            builder.HasBaseType<ApplicationUser>();
            builder.ToTable("AdminUser");

            builder.Property(p=>p.Department)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p=>p.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

        }
    }
}
