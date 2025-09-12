using FluentValidation;
using System.Text.RegularExpressions;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.CreateMobileBill
{
    public class CreateMobileBillCommandValidator : AbstractValidator<CreateMobileBillCommand>
    {
        public CreateMobileBillCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(11).WithMessage("Phone number must be 11 digits.")
                .Matches("^[0-9]*$").WithMessage("Phone number must only contain digits.");
        }
    }
}