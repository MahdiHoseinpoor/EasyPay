using AutoMapper;
using EasyPay.Application.DTOs.AccountManagement;
using EasyPay.Application.Services;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyPay.Domain.Entities.AccountManagement;
using System.Linq.Expressions;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetMyAccountsQueryHandler : IRequestHandler<GetMyAccountsQuery, Result<List<AccountDto>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetMyAccountsQueryHandler(IAccountRepository accountRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<AccountDto>>> Handle(GetMyAccountsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<List<AccountDto>>.Failure(new Error(401, "User is not authenticated."));
            }

            var includes = new List<Expression<System.Func<Account, object>>> { a => a.AccountType };

            var accounts = await _accountRepository
                .Query(a => a.OwnerUserId == userId, Includes: includes)
                .ToListAsync(cancellationToken);

            var accountDtos = _mapper.Map<List<AccountDto>>(accounts);

            return Result<List<AccountDto>>.Success(accountDtos);
        }
    }
}