using FluentValidation;

namespace EasyPay.Application.Commands.Identity.AuthItemValueEntity.RejectAuthItemValue
{
    public class RejectAuthItemValueCommandValidator : AbstractValidator<RejectAuthItemValueCommand>
    {
        public RejectAuthItemValueCommandValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason for rejection is required.")
                .MaximumLength(500).WithMessage("The reason cannot exceed 500 characters.");

            RuleFor(x => x.AuthItemValueId)
                .NotEmpty();
        }
    }
}