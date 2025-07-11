namespace EasyPay.Domain.Entites.AccountManagement
{
    public class AccountType : EntityBase<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; } 

        public bool IsActive { get; set; } = true;
        public bool AllowsOverdraft { get; set; }
        public decimal? MaximumOverdraftLimit { get; set; }
        public decimal? MinimumOpeningBalance { get; set; }
        public decimal? MinimumBalanceRequirement { get; set; }

        public decimal? InterestRate { get; set; } 
        public InterestCalculationMethod InterestCalculationMethod { get; set; }
        public decimal? MonthlyMaintenanceFee { get; set; }

        public int? DailyTransactionLimit { get; set; }
        public decimal? DailyWithdrawalLimit { get; set; }
        public decimal? SingleTransactionLimit { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public int? MinimumAgeRequirement { get; set; }
        public int? MaximumAgeRequirement { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        
    }
    public enum InterestCalculationMethod
    {
        DailyBalance,
        MonthlyAverage,
        MinimumBalance,
        Quarterly,
        Yearly,
        None
    }
}
