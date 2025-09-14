using AutoMapper;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Common;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.AccountEntity
{
    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, Result<IPagedList<AccountDto>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAllAccountsQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result<IPagedList<AccountDto>>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            var pagedAccounts = await _accountRepository.GetPagedListAsync(
                orderBy: q => q.OpeningDate,
                pageIndex: request.PageIndex,
                pageSize: request.PageSize,
                disableTracking: true
            );

            var accountDtos = _mapper.Map<IReadOnlyList<AccountDto>>(pagedAccounts.Items);

            var pagedAccountDtos = new PagedList<AccountDto>(
                accountDtos,
                pagedAccounts.PageIndex,
                pagedAccounts.PageSize,
                pagedAccounts.TotalCount
            );

            return Result<IPagedList<AccountDto>>.Success(pagedAccountDtos);
        }
    }
}