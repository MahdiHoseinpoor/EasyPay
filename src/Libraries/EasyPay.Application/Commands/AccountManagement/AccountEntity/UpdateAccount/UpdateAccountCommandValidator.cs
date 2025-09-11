using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount
{
    public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Account ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Account title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        }
    }
}