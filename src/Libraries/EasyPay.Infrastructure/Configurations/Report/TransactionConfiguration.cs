
using EasyPay.Common;
using EasyPay.Domain.Entities.Report;
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
                a.Property(p=>p.IpAddress).IsRequired().HasMaxLength(15);
                a.Property(p => p.DeviceInfo).IsRequired();
            });

            builder.Property(p => p.ReferenceId)
                .IsRequired();
            builder.HasIndex(p => p.ReferenceId);
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType(SqlColumnTypes.RialDecimal());

            builder.Property(p => p.GatewayName)
                .HasMaxLength(50);

            builder.Property(p => p.GatewayToken)
                .HasMaxLength(100);

            builder.HasIndex(p => p.GatewayToken)
                .IsUnique()
                .HasFilter("[GatewayToken] IS NOT NULL");

            builder.HasOne(p => p.Account)
                .WithMany(p => p.Transactions)
                .HasForeignKey(p => p.AccountId);
        }
    }
}
