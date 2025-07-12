using EasyPay.Domain.Enums.Payment;

namespace EasyPay.Domain.Entites.Payment
{
    public class UtilityBill : BillBase
    {
        public UtilityType UtilityType { get; set; }

        public string BillNumber { get; set; }
    }
}
