using EasyPay.Domain.Enums.Identity;

namespace EasyPay.Domain.Entities.Identity
{
    public class AuthItem:EntityBase<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public AuthItemType AuthItemType { get; set; }
        public AuthItemValueType AuthItemValueType { get; set; }

        public virtual ICollection<AuthItemValue> AuthItemValues { get; set; }
    }
}
