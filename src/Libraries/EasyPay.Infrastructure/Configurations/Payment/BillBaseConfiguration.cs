using EasyPay.Common;
using EasyPay.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Configurations.Payment
{
    public class BillBaseConfiguration : IEntityTypeConfiguration<BillBase>
    {
        public void Configure(EntityTypeBuilder<BillBase> builder)
        {
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType(SqlColumnTypes.Decimal());

        }
    }
}
