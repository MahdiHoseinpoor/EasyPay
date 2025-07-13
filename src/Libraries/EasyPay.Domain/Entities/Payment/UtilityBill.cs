using EasyPay.Domain.Enums.Payment;

namespace EasyPay.Domain.Entities.Payment
{
    public class UtilityBill : BillBase
    {
        public UtilityType UtilityType { get; set; }

        public string BillNumber { get; set; }
    }
}
