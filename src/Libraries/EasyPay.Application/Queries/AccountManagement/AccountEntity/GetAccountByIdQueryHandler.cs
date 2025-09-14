using AutoMapper;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountByIdQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<AccountDto>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var includes = new List<Expression<Func<Account, object>>>
            {
                p => p.AccountType
            };

            var account = (await _accountRepository.Query(p => p.Id == request.Id, Includes: includes, disableTracking: true).FirstOrDefaultAsync(cancellationToken));

            if (account == null)
            {
                return Result<AccountDto>.Failure(new NotFoundError($"Account with ID {request.Id} not found."));
            }

            var accountDto = _mapper.Map<AccountDto>(account);

            return Result<AccountDto>.Success(accountDto);
        }
    }
}