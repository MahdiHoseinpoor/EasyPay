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
    public class AuthItemConfiguration : IEntityTypeConfiguration<AuthItem>
    {
        public void Configure(EntityTypeBuilder<AuthItem> builder)
        {
            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultTitleMaxLength);
            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultDescriptionMaxLength);

            builder.HasMany(p => p.AuthItemValues)
                .WithOne(p => p.AuthItem)
                .HasForeignKey(p => p.AuthItemId);
        }
    }
}
