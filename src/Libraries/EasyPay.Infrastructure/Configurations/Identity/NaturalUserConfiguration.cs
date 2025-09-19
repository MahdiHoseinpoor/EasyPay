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
            builder.HasBaseType<ApplicationUser>();
            builder.ToTable("NaturalUser");
            builder.Property(p => p.NationalCode)
                .HasMaxLength(10);

            builder.Property(p => p.BirthCertificateNumber)
                .HasMaxLength(10);


            builder.Property(p => p.EducationLevel)
                .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p => p.FatherName)
                .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p => p.Gender);

            builder.Property(p => p.PlaceOfBirth)
               .HasMaxLength(EntityConstraints.DefaultMaxLength);

            builder.Property(p => p.Occupation)
               .HasMaxLength(EntityConstraints.DefaultMaxLength);
        }
    }
}
