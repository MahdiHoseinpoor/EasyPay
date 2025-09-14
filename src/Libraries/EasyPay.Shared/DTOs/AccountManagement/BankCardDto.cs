namespace EasyPay.Shared.DTOs.AccountManagement
{
    public class BankCardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CardNumber { get; set; } // Masked for security
        public string AccountNumber { get; set; }
    }
}