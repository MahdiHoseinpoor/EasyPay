using EasyPay.Application.Services.PaymentGateways;
using EasyPay.Common;
using EasyPay.Common.Errors;
using EasyPay.Common.Errors.Business;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class VerifyGatewayDepositCommandHandler : IRequestHandler<VerifyGatewayDepositCommand, Result<string>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentGatewayFactory _gatewayFactory;

        public VerifyGatewayDepositCommandHandler(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IPaymentGatewayFactory gatewayFactory)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _gatewayFactory = gatewayFactory;
        }

        public async Task<Result<string>> Handle(VerifyGatewayDepositCommand request, CancellationToken cancellationToken)
        {
            var pendingTransaction = await _transactionRepository.FirstOrDefaultAsync(
                t => t.GatewayToken == request.GatewayToken && t.Status == Shared.Enums.Report.TransactionStatus.Pending
            );

            if (pendingTransaction == null)
            {
                return Result<string>.Failure(new NotFoundError("Payment session not found or it has already been processed. Please check your account balance."));
            }
            if (!request.Status.Equals("OK", System.StringComparison.OrdinalIgnoreCase))
            {
                pendingTransaction.Status = Shared.Enums.Report.TransactionStatus.Failed;
                pendingTransaction.Description = "Payment was cancelled or failed at the gateway before verification.";
                await _transactionRepository.UpdateAsync(pendingTransaction);
                await _transactionRepository.SaveChangesAsync();
                return Result<string>.Failure(new BusinessRuleError("Payment was cancelled by the user."));
            }
            var gatewayService = _gatewayFactory.Create(pendingTransaction.GatewayName);

            var verificationRequest = new PaymentVerificationRequest(pendingTransaction.Amount, pendingTransaction.GatewayToken);
            var verificationResult = await gatewayService.VerifyPaymentAsync(verificationRequest);

            if (!verificationResult.IsSuccess)
            {
                pendingTransaction.Status = Shared.Enums.Report.TransactionStatus.Failed;
                pendingTransaction.Description = $"Gateway verification failed: {verificationResult.error.message}";
                await _transactionRepository.UpdateAsync(pendingTransaction);
                await _transactionRepository.SaveChangesAsync();
                return Result<string>.Failure(verificationResult.error);
            }
            await _transactionRepository.BeginTransactionAsync();
            try
            {
                var account = await _accountRepository.GetByIdAsync(pendingTransaction.AccountId);
                if (account == null)
                {
                    throw new System.InvalidOperationException($"Account with ID {pendingTransaction.AccountId} not found for a verified transaction.");
                }

                account.CurrentBalance += pendingTransaction.Amount;
                account.LastActivityDate = System.DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account);
                pendingTransaction.Status = Shared.Enums.Report.TransactionStatus.Completed;
                pendingTransaction.Description = $"Deposit of {pendingTransaction.Amount:C} via {pendingTransaction.GatewayName}";
                pendingTransaction.ReferenceId = verificationResult.Value.FinalReferenceId; 
                pendingTransaction.TransactionDate = System.DateTime.UtcNow;
                await _transactionRepository.UpdateAsync(pendingTransaction);
                await _transactionRepository.SaveChangesAsync();
                await _transactionRepository.CommitTransactionAsync();

                return Result<string>.Success("Your account has been successfully credited.");
            }
            catch (System.Exception ex)
            {
                await _transactionRepository.RollbackTransactionAsync();
                return Result<string>.Failure(new Error(500, "A critical error occurred while crediting your account. Please contact support."));
            }
        }
    }
}