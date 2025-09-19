using EasyPay.Application.Services;
using EasyPay.Application.Services.PaymentGateways;
using EasyPay.Common;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Report;
using EasyPay.Domain.ValueObjects.Report;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using EasyPay.Shared.Models.Report;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction
{
    public class RequestGatewayDepositCommandHandler : IRequestHandler<RequestGatewayDepositCommand, Result<GatewayDepositResponse>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentGatewayFactory _gatewayFactory;
        private readonly IConfiguration _configuration;

        public RequestGatewayDepositCommandHandler(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            ICurrentUserService currentUserService,
            IPaymentGatewayFactory gatewayFactory,
            IConfiguration configuration)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _currentUserService = currentUserService;
            _gatewayFactory = gatewayFactory;
            _configuration = configuration;
        }

        public async Task<Result<GatewayDepositResponse>> Handle(RequestGatewayDepositCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var account = await _accountRepository.GetByIdAsync(request.AccountId);

            if (account == null || account.OwnerUserId != userId)
            {
                return Result<GatewayDepositResponse>.Failure(new AuthorizationError("Account not found or access is denied."));
            }

            var transaction = new Transaction(
                request.AccountId, request.Amount, Shared.Enums.Report.TransactionType.Deposit,
                Guid.NewGuid().ToString(), new TransactionMetadata("API", "Gateway Deposit Init"),
                $"Pending deposit via {request.GatewayName}")
            {
                Status = Shared.Enums.Report.TransactionStatus.Pending,
                GatewayName = request.GatewayName
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            var gatewayService = _gatewayFactory.Create(request.GatewayName);
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5144";
            var callbackUrl = $"{apiBaseUrl}/api/v1/payment/callback";

            var gatewayRequest = new PaymentRequest(transaction.Amount, callbackUrl, transaction.Description, transaction.Id);
            var gatewayResult = await gatewayService.RequestPaymentAsync(gatewayRequest);

            return await gatewayResult.Match<Task<Result<GatewayDepositResponse>>>(
                async successResult =>
                {
                    transaction.GatewayToken = successResult.Token;
                    await _transactionRepository.UpdateAsync(transaction);
                    await _transactionRepository.SaveChangesAsync();
                    return Result<GatewayDepositResponse>.Success(new GatewayDepositResponse(successResult.RedirectUrl));
                },
                async error =>
                {
                    transaction.Status = Shared.Enums.Report.TransactionStatus.Failed;
                    transaction.Description = $"Gateway request failed: {error.message}";
                    await _transactionRepository.UpdateAsync(transaction);
                    await _transactionRepository.SaveChangesAsync();
                    return Result<GatewayDepositResponse>.Failure(error);
                }
            );
        }
    }
}