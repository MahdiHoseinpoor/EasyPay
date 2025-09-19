namespace EasyPay.Shared.DTOs.AccountManagement
{
    public class AccountTypeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MinimumOpeningBalance { get; set; }
    }
}