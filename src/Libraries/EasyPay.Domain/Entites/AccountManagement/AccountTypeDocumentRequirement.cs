using EasyPay.Domain.Entites.Identity;

namespace EasyPay.Domain.Entites.AccountManagement
{
    public class AccountTypeDocumentRequirement : EntityBase<int>
    {
        public int AccountTypeId { get; set; }
        public virtual AccountType AccountType { get; set; }

        public int AuthItemId { get; set; }
        public virtual AuthItem AuthItem { get; set; }

        public bool IsRequired { get; set; } = true; 
        public int? Order { get; set; } 
        public string ValidationRules { get; set; }
    }
}
