using FluentValidation;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.UpdateUtilityBill
{
    public class UpdateUtilityBillCommandValidator : AbstractValidator<UpdateUtilityBillCommand>
    {
        public UpdateUtilityBillCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Utility Bill ID is required.");

            RuleFor(x => x.BillNumber)
                .NotEmpty().WithMessage("Bill number is required.")
                .MaximumLength(50).WithMessage("Bill number cannot exceed 50 characters.");

            RuleFor(x => x.UtilityType)
                .IsInEnum().WithMessage("A valid utility type must be specified.");
        }
    }
}