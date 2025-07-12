using EasyPay.Domain.Entites.AccountManagement;
using EasyPay.Domain.Entites.Identity;
using EasyPay.Domain.Entites.Payment;
using EasyPay.Domain.Entites.Report;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EasyPay.Infrastructure.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountType> AccountTypes { get; set; }
        public virtual DbSet<AccountTypeDocumentRequirement> AccountTypeDocumentRequirements { get; set; }
        public virtual DbSet<BankCard> BankCards { get; set; }

        public virtual DbSet<AdminUser> AdminUsers { get; set; }
        public virtual DbSet<AuthItem> AuthItems { get; set; }
        public virtual DbSet<AuthItemValue> AuthItemValues { get; set; }
        public virtual DbSet<LoginHistory> LoginHistory { get; set; }
        public virtual DbSet<NaturalUser> NaturalUsers { get; set; }

        public virtual DbSet<Transaction> Transaction { get; set; }

        public virtual DbSet<MobileBill> MobileBills { get; set; }
        public virtual DbSet<UtilityBill> UtilityBills { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}
