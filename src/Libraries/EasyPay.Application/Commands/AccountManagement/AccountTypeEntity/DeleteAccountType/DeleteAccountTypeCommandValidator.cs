using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.DeleteAccountType
{
    public class DeleteAccountTypeCommandValidator : AbstractValidator<DeleteAccountTypeCommand>
    {
        public DeleteAccountTypeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Account Type ID is required.");
        }
    }
}