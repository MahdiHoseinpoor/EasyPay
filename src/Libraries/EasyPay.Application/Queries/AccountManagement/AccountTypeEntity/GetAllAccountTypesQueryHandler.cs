using AutoMapper;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.AccountTypeEntity
{
    public class GetAllAccountTypesQueryHandler : IRequestHandler<GetAllAccountTypesQuery, Result<List<AccountTypeDto>>>
    {
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly IMapper _mapper;

        public GetAllAccountTypesQueryHandler(IAccountTypeRepository accountTypeRepository, IMapper mapper)
        {
            _accountTypeRepository = accountTypeRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<AccountTypeDto>>> Handle(GetAllAccountTypesQuery request, CancellationToken cancellationToken)
        {
            var accountTypes = await _accountTypeRepository
                .Query(at => at.IsActive, orderBy: at => at.Title)
                .ToListAsync(cancellationToken);

            if (accountTypes == null || !accountTypes.Any())
            {
                return Result<List<AccountTypeDto>>.Success(new List<AccountTypeDto>());
            }

            var accountTypeDtos = _mapper.Map<List<AccountTypeDto>>(accountTypes);

            return Result<List<AccountTypeDto>>.Success(accountTypeDtos);
        }
    }
}