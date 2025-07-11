using EasyPay.Domain.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Configurations
{
    public class NaturalUserConfiguration : IEntityTypeConfiguration<NaturalUser>
    {
        public void Configure(EntityTypeBuilder<NaturalUser> builder)
        {
            builder.ToTable("NaturalUser");
        }
    }
}
