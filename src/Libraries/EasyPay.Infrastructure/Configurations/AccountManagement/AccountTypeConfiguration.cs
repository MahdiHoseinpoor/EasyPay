using EasyPay.Common;
using EasyPay.Domain.Entities.AccountManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyPay.Infrastructure.Configurations.AccountManagement
{
    public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
    {
        public void Configure(EntityTypeBuilder<AccountType> builder)
        {
        
            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(EntityConstraints.DefaultTitleMaxLength);

            builder.Property(p => p.Description)
                .HasMaxLength(EntityConstraints.DefaultDescriptionMaxLength);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(p => p.MaximumOverdraftLimit)
                .HasColumnType(SqlColumnTypes.Decimal());

            builder.Property(p => p.MinimumOpeningBalance)
                .HasColumnType(SqlColumnTypes.Decimal());

            builder.Property(p => p.MinimumBalanceRequirement)
                .HasColumnType(SqlColumnTypes.Decimal());

            builder.Property(p => p.InterestRate)
                .HasColumnType(SqlColumnTypes.Decimal(5,2));

            builder.Property(p => p.MonthlyMaintenanceFee)
                .HasColumnType(SqlColumnTypes.Decimal());

            builder.Property(p => p.DailyWithdrawalLimit)
                .HasColumnType(SqlColumnTypes.Decimal());

            builder.Property(p => p.SingleTransactionLimit)
                .HasColumnType(SqlColumnTypes.Decimal());

         
            builder.HasIndex(p => p.Code)
                .IsUnique();

            builder.HasIndex(p => p.IsActive);

            builder.HasMany(p => p.Accounts)
                .WithOne(a => a.AccountType)
                .HasForeignKey(a => a.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            //builder.HasData(
            //    new AccountType
            //    {
            //        Id = 1,
            //        Title = "حساب جاری",
            //        Code = "CUR",
            //        AllowsOverdraft = true,
            //        MinimumBalanceRequirement = 0,
            //        IsActive = true
            //    },
            //    new AccountType
            //    {
            //        Id = 2,
            //        Title = "حساب پس‌انداز",
            //        Code = "SAV",
            //        InterestRate = 5.0m,
            //        MinimumBalanceRequirement = 100000,
            //        IsActive = true
            //    }
            //);
        }
    }
}