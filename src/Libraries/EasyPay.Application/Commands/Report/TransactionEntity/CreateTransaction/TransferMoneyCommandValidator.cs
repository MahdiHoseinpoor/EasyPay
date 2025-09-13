using EasyPay.Infrastructure.Aggregates.AccountManagement;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class TransferMoneyCommandValidator : AbstractValidator<TransferMoneyCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public TransferMoneyCommandValidator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;

            RuleFor(x => x.SourceAccountId)
                .NotEmpty().WithMessage("Source Account ID is required.");

            RuleFor(x => x.DestinationAccountNumber)
                .NotEmpty().WithMessage("Destination Account Number is required.")
                .Length(10).WithMessage("Account Number must be 10 digits.")
                .Matches("^[0-9]*$").WithMessage("Account Number must only contain digits.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Transfer amount must be positive.");

            RuleFor(x => x.Description)
                .MaximumLength(200);

            RuleFor(x => x)
                .MustAsync(NotBeSameAccount)
                .WithMessage("Source and destination accounts cannot be the same.")
                .WhenAsync(async (x, ct) => await DestinationAccountExists(x.DestinationAccountNumber, ct));
        }

        private async Task<bool> DestinationAccountExists(string accountNumber, CancellationToken cancellationToken)
        {
            return await _accountRepository.ExistsAsync(a => a.AccountNumber == accountNumber);
        }

        private async Task<bool> NotBeSameAccount(TransferMoneyCommand command, CancellationToken cancellationToken)
        {
            var destinationAccount = await _accountRepository.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber);
            return destinationAccount?.Id != command.SourceAccountId;
        }
    }
}