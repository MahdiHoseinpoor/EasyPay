using EasyPay.Shared.Enums.Report;

namespace EasyPay.Web.Extensions;

public static class TransactionDisplayHelper
{
    public static (string Icon, string Color, string AmountSign) GetDisplayDetails(this TransactionType type) => type switch
    {
        TransactionType.Deposit or TransactionType.TransferIn =>
            ("bi bi-arrow-down-circle-fill", "var(--success-color)", "+"),
        TransactionType.Withdrawal or TransactionType.TransferOut or TransactionType.BillPayment =>
            ("bi bi-arrow-up-circle-fill", "var(--text-primary)", "-"),
        _ =>
            ("bi bi-receipt-cutoff", "var(--text-primary)", "-")
    };
}