using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Report;
using EasyPay.Shared.Enums.Report;
using EasyPay.Domain.ValueObjects.Report;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class WithdrawToBankCardCommandHandler : IRequestHandler<WithdrawToBankCardCommand, Result<Guid>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IBankCardRepository _bankCardRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _currentUserService;

        public WithdrawToBankCardCommandHandler(
            IAccountRepository accountRepository,
            IBankCardRepository bankCardRepository,
            ITransactionRepository transactionRepository,
            ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _bankCardRepository = bankCardRepository;
            _transactionRepository = transactionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(WithdrawToBankCardCommand request, CancellationToken cancellationToken)
        {
            if (request.RequestMetadata is null)
            {
                return Result<Guid>.Failure(new BusinessRuleError("Request metadata (IP, UserAgent) is missing."));
            }
            var userId = _currentUserService.UserId;
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null || account.OwnerUserId != userId)
                return Result<Guid>.Failure(new AuthorizationError("Forbidden: You do not have access to this account."));

            var bankCard = await _bankCardRepository.GetByIdAsync(request.DestinationBankCardId);
            if (bankCard == null) 
                return Result<Guid>.Failure(new NotFoundError("Destination bank card not found."));

            if (account.Status != Shared.Enums.AccountManagement.AccountStatus.Active)
                return Result<Guid>.Failure(new AccountInactiveError(account.Status.ToString()));

            if (account.CurrentBalance < request.Amount)
                return Result<Guid>.Failure(new InsufficientFundsError(account.CurrentBalance, request.Amount));

            await _transactionRepository.BeginTransactionAsync();
            try
            {
                account.CurrentBalance -= request.Amount;
                account.LastActivityDate = System.DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account);

                var metadata = new TransactionMetadata(request.RequestMetadata.IpAddress, request.RequestMetadata.UserAgent);
                var referenceId = System.Guid.NewGuid().ToString();
                var description = request.Description ?? $"Withdrawal to {bankCard.Title} ({bankCard.CardNumber})";

                var transaction = new Transaction(account.Id, request.Amount, TransactionType.Withdrawal, referenceId, metadata, description);
                await _transactionRepository.AddAsync(transaction);

                await _transactionRepository.SaveChangesAsync();
                await _transactionRepository.CommitTransactionAsync();

                // In a real scenario, you would now call a third-party payment gateway API here to process the withdrawal.

                return Result<Guid>.Success(transaction.Id);
            }
            catch (System.Exception)
            {
                await _transactionRepository.RollbackTransactionAsync();
                return Result<Guid>.Failure(new Error(500, "An unexpected error occurred during the withdrawal."));
            }
        }
    }
}