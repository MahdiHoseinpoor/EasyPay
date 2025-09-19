using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.DeleteBankCard
{
    public class DeleteBankCardCommandValidator : AbstractValidator<DeleteBankCardCommand>
    {
        public DeleteBankCardCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Bank Card ID is required.");
        }
    }
}