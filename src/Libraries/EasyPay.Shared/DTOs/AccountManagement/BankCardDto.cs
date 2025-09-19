namespace EasyPay.Shared.DTOs.AccountManagement
{
    public class BankCardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CardNumber { get; set; } 
        public string AccountNumber { get; set; }
        public string InternationalBankAccountNumber { get; set; }
    }
}