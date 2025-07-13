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
    public class AuthItemValueConfiguration : IEntityTypeConfiguration<AuthItemValue>
    {
        public void Configure(EntityTypeBuilder<AuthItemValue> builder)
        {
            builder.Property(p => p.Value)
                .IsRequired();
            builder.HasIndex(p => p.AuthItemId);
            builder.HasIndex(p => p.UserId);
            builder.HasOne(p => p.User)
                .WithMany(p => p.AuthItemValues)
                .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.AuthItem)
               .WithMany(p => p.AuthItemValues)
               .HasForeignKey(p => p.AuthItemId);
        }
    }
}
