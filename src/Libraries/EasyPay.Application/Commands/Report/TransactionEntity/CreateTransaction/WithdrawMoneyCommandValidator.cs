using FluentValidation;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class WithdrawMoneyCommandValidator : AbstractValidator<WithdrawMoneyCommand>
    {
        public WithdrawMoneyCommandValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty().WithMessage("Account ID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Withdrawal amount must be positive.");

            RuleFor(x => x.Description)
                .MaximumLength(200);
        }
    }
}