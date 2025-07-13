using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Configurations.Identity
{
    public class NaturalUserConfiguration : IEntityTypeConfiguration<NaturalUser>
    {
        public void Configure(EntityTypeBuilder<NaturalUser> builder)
        {
            builder.HasBaseType<ApplicationDbContext>();
            builder.ToTable("NaturalUser");
            builder.Property(p => p.NationalCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(p => p.BirthCertificateNumber)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(p => p.BirthDate)
                .IsRequired();

            builder.Property(p => p.EducationLevel)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p => p.FatherName)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p => p.Gender)
               .IsRequired();

            builder.Property(p => p.PlaceOfBirth)
               .IsRequired()
               .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p => p.Occupation)
               .IsRequired()
               .HasMaxLength(EntityConstraints.DefaultMaxLength);
        }
    }
}
