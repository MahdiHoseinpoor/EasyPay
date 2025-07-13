
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
                a.Property(p => p.DeviceInfo).IsRequired().HasMaxLength(EntityConstraints.DefaultMaxLength);
            });

            builder.Property(p => p.ReferenceId)
                .IsRequired();
            builder.HasIndex(p => p.ReferenceId)
                .IsUnique();
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType(SqlColumnTypes.Decimal());

            builder.HasOne(p => p.Account)
                .WithMany(p => p.Transactions)
                .HasForeignKey(p => p.AccountId);
        }
    }
}
