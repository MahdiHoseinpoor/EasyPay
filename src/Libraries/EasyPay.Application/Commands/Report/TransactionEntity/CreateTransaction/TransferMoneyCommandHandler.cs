using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Report;
using EasyPay.Shared.Enums.AccountManagement;
using EasyPay.Shared.Enums.Report;
using EasyPay.Domain.ValueObjects.Report;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class TransferMoneyCommandHandler : IRequestHandler<TransferMoneyCommand, Result<Guid>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _currentUserService;

        public TransferMoneyCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(TransferMoneyCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var sourceAccount = await _accountRepository.GetByIdAsync(request.SourceAccountId);
            if (sourceAccount == null)
                return Result<Guid>.Failure(new NotFoundError("Source account not found."));

            var destinationAccount = await _accountRepository.FirstOrDefaultAsync(a => a.AccountNumber == request.DestinationAccountNumber);
            if (destinationAccount == null)
                return Result<Guid>.Failure(new NotFoundError("Destination account not found."));

            if (sourceAccount.OwnerUserId != userId)
                return Result<Guid>.Failure(new AuthorizationError("Forbidden: You do not have access to the source account."));

            if (sourceAccount.Status != AccountStatus.Active)
                return Result<Guid>.Failure(new AccountInactiveError($"Source account is {sourceAccount.Status}."));

            if (destinationAccount.Status != AccountStatus.Active)
                return Result<Guid>.Failure(new BusinessRuleError($"Destination account is {destinationAccount.Status} and cannot receive funds."));

            if (sourceAccount.CurrentBalance < request.Amount)
                return Result<Guid>.Failure(new InsufficientFundsError(sourceAccount.CurrentBalance, request.Amount));

            await _transactionRepository.BeginTransactionAsync();
            try
            {
                sourceAccount.CurrentBalance -= request.Amount;
                sourceAccount.LastActivityDate = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(sourceAccount);

                destinationAccount.CurrentBalance += request.Amount;
                destinationAccount.LastActivityDate = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(destinationAccount);

                var metadata = new TransactionMetadata(request.RequestMetadata.IpAddress, request.RequestMetadata.UserAgent);
                var referenceId = Guid.NewGuid().ToString();

                var withdrawal = new Transaction(sourceAccount.Id, request.Amount, TransactionType.TransferOut, referenceId, metadata,
                    request.Description ?? $"Transfer to {destinationAccount.AccountNumber}");

                var deposit = new Transaction(destinationAccount.Id, request.Amount, TransactionType.TransferIn, referenceId, metadata,
                    request.Description ?? $"Transfer from {sourceAccount.AccountNumber}");

                await _transactionRepository.AddAsync(withdrawal);
                await _transactionRepository.AddAsync(deposit);

                await _transactionRepository.SaveChangesAsync();
                await _transactionRepository.CommitTransactionAsync();

                return Result<Guid>.Success(withdrawal.Id);
            }
            catch (Exception)
            {
                await _transactionRepository.RollbackTransactionAsync();
                return Result<Guid>.Failure(new Error(500, "An unexpected error occurred during the transfer."));
            }
        }
    }
}