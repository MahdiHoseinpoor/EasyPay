using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.DeleteAccount
{
    public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
    {
        public DeleteAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Account ID is required.");
        }
    }
}