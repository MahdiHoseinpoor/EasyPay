using AutoMapper;
using EasyPay.Shared.DTOs.Report;
using EasyPay.Application.Services;
using EasyPay.Common;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.Report.TransactionEntity
{
    public class GetMyTransactionHistoryQueryHandler : IRequestHandler<GetMyTransactionHistoryQuery, Result<PagedList<TransactionDto>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetMyTransactionHistoryQueryHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PagedList<TransactionDto>>> Handle(GetMyTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return Result<PagedList<TransactionDto>>.Failure(new NotFoundError("Account not found."));

            if (account.OwnerUserId != userId)
                return Result<PagedList<TransactionDto>>.Failure(new Error(403, "Forbidden: You do not have access to this account's history."));

            var pagedTransactions = await _transactionRepository.GetPagedListAsync(
                predicate: t => t.AccountId == request.AccountId,
                orderBy: q => q.TransactionDate,
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var transactionDtos = _mapper.Map<List<TransactionDto>>(pagedTransactions.Items);
            var pagedResult = new PagedList<TransactionDto>(transactionDtos, pagedTransactions.PageIndex, pagedTransactions.PageSize, pagedTransactions.TotalCount);

            return Result<PagedList<TransactionDto>>.Success(pagedResult);
        }
    }
}