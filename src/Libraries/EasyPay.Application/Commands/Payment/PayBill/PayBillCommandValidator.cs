using FluentValidation;

namespace EasyPay.Application.Commands.Payment.PayBill
{
    public class PayBillCommandValidator : AbstractValidator<PayBillCommand>
    {
        public PayBillCommandValidator()
        {
            RuleFor(x => x.BillId)
                .NotEmpty().WithMessage("Bill ID is required.");

            RuleFor(x => x.FromAccountId)
                .NotEmpty().WithMessage("Source Account ID is required.");
        }
    }
}