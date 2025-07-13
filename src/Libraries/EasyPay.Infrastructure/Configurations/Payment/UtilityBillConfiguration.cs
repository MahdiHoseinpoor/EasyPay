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
    public class UtilityBillConfiguration : IEntityTypeConfiguration<UtilityBill>
    {
        public void Configure(EntityTypeBuilder<UtilityBill> builder)
        {
            builder.Property(p => p.BillNumber).IsRequired();
        }
    }
}
