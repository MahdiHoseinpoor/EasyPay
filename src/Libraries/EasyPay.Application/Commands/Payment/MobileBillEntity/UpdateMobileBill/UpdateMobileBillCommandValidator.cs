using FluentValidation;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.UpdateMobileBill
{
    public class UpdateMobileBillCommandValidator : AbstractValidator<UpdateMobileBillCommand>
    {
        public UpdateMobileBillCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Mobile Bill ID is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(11).WithMessage("Phone number must be 11 digits.")
                .Matches("^[0-9]*$").WithMessage("Phone number must only contain digits.");
        }
    }
}