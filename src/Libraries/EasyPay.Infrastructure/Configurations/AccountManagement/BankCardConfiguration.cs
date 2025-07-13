using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyPay.Infrastructure.Configurations.AccountManagement
{
    public class BankCardConfiguration : IEntityTypeConfiguration<BankCard>
    {
        public void Configure(EntityTypeBuilder<BankCard> builder)
        {
            builder.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(EntityConstraints.DefaultTitleMaxLength);
            builder.Property(p => p.AccountNumber)
                .HasMaxLength(10);
            builder.Property(p => p.CardNumber)
                .HasMaxLength(16);
            builder.Property(p => p.InternationalBankAccountNumber)
                .HasMaxLength(34);
        }
    }
}
