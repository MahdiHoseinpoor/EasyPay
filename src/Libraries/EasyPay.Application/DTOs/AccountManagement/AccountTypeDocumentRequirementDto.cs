namespace EasyPay.Application.DTOs.AccountManagement
{
    public class AccountTypeDocumentRequirementDto
    {
        public int Id { get; set; }
        public int AccountTypeId { get; set; }
        public int AuthItemId { get; set; }
        public string AuthItemTitle { get; set; }
        public bool IsRequired { get; set; }
        public int? Order { get; set; }
    }
}