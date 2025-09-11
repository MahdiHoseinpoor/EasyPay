using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.AccountTypeId)
                .NotEmpty().WithMessage("Account Type ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Account title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        }
    }
}