using EasyPay.Common;
using EasyPay.Domain.Entites.AccountManagement;
using EasyPay.Domain.Enums.AccountManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyPay.Infrastructure.Configurations.AccountManagement
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts", "account");

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultTitleMaxLength);

            builder.Property(p => p.AccountNumber)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(p => p.CurrentBalance)
                .HasColumnType(SqlColumnTypes.Decimal())
                .HasDefaultValue(0);

            builder.Property(p => p.OpeningDate)
                .IsRequired();

            builder.Property(p => p.LastActivityDate)
                .IsRequired(false);

            builder.HasIndex(p => p.AccountNumber)
                .IsUnique();

            builder.HasIndex(p => p.OwnerUserId);

            builder.HasOne(p => p.AccountType)
                .WithMany(p => p.Accounts)
                .HasForeignKey(p => p.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.OwnerUser)
                .WithMany(p => p.Accounts)
                .HasForeignKey(p => p.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Transactions)
              .WithOne(p => p.Account)
              .HasForeignKey(p => p.AccountId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (AccountStatus)Enum.Parse(typeof(AccountStatus), v))
                .HasMaxLength(20);

            //builder.HasData(
            //    new Account
            //    {
            //        Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            //        AccountTypeId = 1,
            //        Title = "حساب اصلی",
            //        AccountNumber = "1234567890",
            //        OwnerUserId = "1",
            //        CurrentBalance = 1000000,
            //        Status = AccountStatus.Active
            //    }
            //);
        }
    }
}