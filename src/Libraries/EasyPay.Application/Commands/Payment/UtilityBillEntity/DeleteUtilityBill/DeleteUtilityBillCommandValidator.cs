using FluentValidation;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.DeleteUtilityBill
{
    public class DeleteUtilityBillCommandValidator : AbstractValidator<DeleteUtilityBillCommand>
    {
        public DeleteUtilityBillCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Utility Bill ID is required.");
        }
    }
}