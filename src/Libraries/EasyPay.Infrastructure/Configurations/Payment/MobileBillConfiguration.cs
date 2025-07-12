using EasyPay.Domain.Entites.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyPay.Infrastructure.Configurations.Payment
{
    public class MobileBillConfiguration : IEntityTypeConfiguration<MobileBill>
    {
        public void Configure(EntityTypeBuilder<MobileBill> builder)
        {
            builder.Property(p=>p.PhoneNumber).IsRequired();
        }
    }
}
