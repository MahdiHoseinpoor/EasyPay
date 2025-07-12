using EasyPay.Domain.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Configurations.Identity
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultMaxLength);
            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultMaxLength);
            builder.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultLongMaxLength);
            builder.Ignore(p => p.FullName);

            builder.HasMany(p => p.AuthItemValues)
                .WithOne(p=>p.User)
                .HasForeignKey(p=>p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(p => p.Accounts)
                .WithOne(p => p.OwnerUser)
                .HasForeignKey(p => p.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(p => p.LoginHistories)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); ;
        }
    }
}
