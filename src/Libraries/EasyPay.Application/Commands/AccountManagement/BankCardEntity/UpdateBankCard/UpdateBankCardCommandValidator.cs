using EasyPay.Infrastructure.Configurations;
using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.UpdateBankCard
{
    // CORRECTED: Now targets UpdateBankCardCommand
    public class UpdateBankCardCommandValidator : AbstractValidator<UpdateBankCardCommand>
    {
        public UpdateBankCardCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("Bank Card ID is required.");

            RuleFor(p => p.Title)
                .NotEmpty()
                .MaximumLength(EntityConstraints.DefaultTitleMaxLength);

            RuleFor(p => p.InternationalBankAccountNumber)
                .NotEmpty()
                .Length(34).WithMessage("IBAN must be 34 characters.");

            RuleFor(p => p.CardNumber)
                .NotEmpty()
                .Length(16).WithMessage("Card Number must be 16 digits.")
                .Matches("^[0-9]*$").WithMessage("Card Number must only contain digits.");

            RuleFor(p => p.AccountNumber)
                .NotEmpty()
                .Length(10).WithMessage("Account Number must be 10 digits.")
                .Matches("^[0-9]*$").WithMessage("Account Number must only contain digits.");
        }
    }
}