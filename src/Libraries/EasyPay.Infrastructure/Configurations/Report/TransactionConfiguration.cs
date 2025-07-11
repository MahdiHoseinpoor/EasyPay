
using EasyPay.Domain.Entites.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyPay.Infrastructure.Configurations.Report
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.OwnsOne(o => o.TransactionMetadata,a =>
            {
                a.Property(p=>p.IpAddress).IsRequired();
            });
        }
    }
}
