using EasyPay.Domain.Enums.Payment;

namespace EasyPay.Domain.Entities.Payment
{
    public abstract class BillBase : EntityBase<Guid>
    {
        public decimal Amount { get; set; }
        public BillType Type { get; set; }
    }

}
