using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Payment;

namespace EasyPay.Domain.Entities.Payment
{
    public abstract class BillBase : EntityBase<Guid>
    {
        public decimal Amount { get; set; }
        public BillType Type { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}