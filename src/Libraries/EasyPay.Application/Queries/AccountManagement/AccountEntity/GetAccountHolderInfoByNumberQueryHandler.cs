using AutoMapper;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetAccountHolderInfoByNumberQueryHandler : IRequestHandler<GetAccountHolderInfoByNumberQuery, Result<AccountHolderDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountHolderInfoByNumberQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<AccountHolderDto>> Handle(GetAccountHolderInfoByNumberQuery request, CancellationToken cancellationToken)
        {
            var includes = new List<Expression<Func<Account, object>>> { a => a.OwnerUser };

            var account = await _accountRepository.Query(
                predicate: a => a.AccountNumber == request.AccountNumber,
                Includes: includes,
                disableTracking: true
            ).FirstOrDefaultAsync(cancellationToken);

            if (account == null)
            {
                return Result<AccountHolderDto>.Failure(new NotFoundError("Account not found."));
            }
            if (account.Status != Shared.Enums.AccountManagement.AccountStatus.Active)
            {
                return Result<AccountHolderDto>.Failure(new NotFoundError($"Destination account is {account.Status} and cannot receive funds."));
            }

            var dto = _mapper.Map<AccountHolderDto>(account);
            return Result<AccountHolderDto>.Success(dto);
        }
    }
}