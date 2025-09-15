using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Report;
using EasyPay.Shared.Enums.AccountManagement;
using EasyPay.Shared.Enums.Payment;
using EasyPay.Shared.Enums.Report;
using EasyPay.Domain.ValueObjects.Report;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Payment;
using EasyPay.Infrastructure.Aggregates.Report;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.PayBill
{
    public class PayBillCommandHandler : IRequestHandler<PayBillCommand, Result<Guid>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _currentUserService;

        public PayBillCommandHandler(
            IBillRepository billRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ICurrentUserService currentUserService)
        {
            _billRepository = billRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(PayBillCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<Guid>.Failure(new AuthorizationError("User is not authenticated."));
            }

            var bill = await _billRepository.GetByIdAsync(request.BillId);
            if (bill == null)
            {
                return Result<Guid>.Failure(new NotFoundError("Bill not found."));
            }

            if (bill.UserId != userId)
            {
                return Result<Guid>.Failure(new AuthorizationError("You do not have permission to pay this bill."));
            }

            if (bill.Status == BillStatus.Paid)
            {
                return Result<Guid>.Failure(new BusinessRuleError("This bill has already been paid."));
            }

            var account = await _accountRepository.GetByIdAsync(request.FromAccountId);
            if (account == null)
            {
                return Result<Guid>.Failure(new NotFoundError("Source account not found."));
            }

            if (account.OwnerUserId != userId)
            {
                return Result<Guid>.Failure(new AuthorizationError("You do not have permission to use this account."));
            }

            if (account.Status != AccountStatus.Active)
            {
                return Result<Guid>.Failure(new AccountInactiveError($"Source account is {account.Status}."));
            }

            if (account.CurrentBalance < bill.Amount)
            {
                return Result<Guid>.Failure(new InsufficientFundsError(account.CurrentBalance, bill.Amount));
            }

            await _transactionRepository.BeginTransactionAsync();
            try
            {
                account.CurrentBalance -= bill.Amount;
                account.LastActivityDate = DateTime.UtcNow;
                await _accountRepository.UpdateAsync(account);

                var metadata = new TransactionMetadata(request.RequestMetadata.IpAddress, request.RequestMetadata.UserAgent);
                var referenceId = Guid.NewGuid().ToString();
                var description = $"Payment for {bill.Type} Bill (ID: {bill.Id})";

                var transaction = new Transaction(account.Id, bill.Amount, TransactionType.BillPayment, referenceId, metadata, description);
                await _transactionRepository.AddAsync(transaction);

                bill.Status = BillStatus.Paid;
                bill.PaymentDate = DateTime.UtcNow;
                await _billRepository.UpdateAsync(bill);

                await _transactionRepository.SaveChangesAsync();
                await _transactionRepository.CommitTransactionAsync();

                return Result<Guid>.Success(transaction.Id);
            }
            catch (Exception ex)
            {
                await _transactionRepository.RollbackTransactionAsync();
                return Result<Guid>.Failure(new Error(500, "An unexpected error occurred during bill payment."));
            }
        }
    }
}