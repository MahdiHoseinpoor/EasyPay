using FluentValidation;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.CreateUtilityBill
{
    public class CreateUtilityBillCommandValidator : AbstractValidator<CreateUtilityBillCommand>
    {
        public CreateUtilityBillCommandValidator()
        {
            RuleFor(x => x.BillNumber)
                .NotEmpty().WithMessage("Bill number is required.")
                .MaximumLength(50).WithMessage("Bill number cannot exceed 50 characters.");

            RuleFor(x => x.UtilityType)
                .IsInEnum().WithMessage("A valid utility type must be specified.");
        }
    }
}