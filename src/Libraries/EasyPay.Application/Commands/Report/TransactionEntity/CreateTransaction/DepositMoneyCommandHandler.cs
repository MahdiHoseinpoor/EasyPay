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
    public class DepositMoneyCommandHandler : IRequestHandler<DepositMoneyCommand, Result<Guid>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _currentUserService;

        public DepositMoneyCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return Result<Guid>.Failure(new NotFoundError("Account not found."));

            if (account.OwnerUserId != userId)
                return Result<Guid>.Failure(new AuthorizationError("Forbidden: You do not have access to this account."));

            if (account.Status != Domain.Enums.AccountManagement.AccountStatus.Active)
                return Result<Guid>.Failure(new AccountInactiveError(account.Status.ToString()));

            await _transactionRepository.BeginTransactionAsync();
            try
            {
                account.CurrentBalance += request.Amount;
                account.LastActivityDate = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account);
                var metadata = new TransactionMetadata(request.RequestMetadata.IpAddress, request.RequestMetadata.UserAgent);
                var referenceId = Guid.NewGuid().ToString();
                var transaction = new Transaction(account.Id, request.Amount, TransactionType.Deposit, referenceId, metadata, request.Description);

                await _transactionRepository.AddAsync(transaction);

                await _transactionRepository.SaveChangesAsync();
                await _transactionRepository.CommitTransactionAsync();

                return Result<Guid>.Success(transaction.Id);
            }
            catch (System.Exception ex)
            {
                await _transactionRepository.RollbackTransactionAsync();
                return Result<Guid>.Failure(new Error(500, "An unexpected error occurred during the deposit."));
            }
        }
    }
}