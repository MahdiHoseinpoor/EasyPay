using EasyPay.Infrastructure.Aggregates.AccountManagement;
using FluentValidation;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class WithdrawToBankCardCommandValidator : AbstractValidator<WithdrawToBankCardCommand>
    {
        public WithdrawToBankCardCommandValidator(IBankCardRepository bankCardRepository)
        {
            RuleFor(x => x.AccountId)
                .NotEmpty().WithMessage("Source Account ID is required.");

            RuleFor(x => x.DestinationBankCardId)
                .NotEmpty().WithMessage("Destination Bank Card ID is required.")
                .MustAsync(async (id, cancellation) => await bankCardRepository.ExistsAsync(c => c.Id == id))
                .WithMessage("The specified Bank Card does not exist.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Withdrawal amount must be positive.");

            RuleFor(x => x.Description)
                .MaximumLength(200);
        }
    }
}