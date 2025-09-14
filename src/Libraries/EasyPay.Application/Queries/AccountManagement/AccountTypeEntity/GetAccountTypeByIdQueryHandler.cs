using AutoMapper;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.AccountTypeEntity
{
    public class GetAccountTypeByIdQueryHandler : IRequestHandler<GetAccountTypeByIdQuery, Result<AccountTypeDto>>
    {
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly IMapper _mapper;

        public GetAccountTypeByIdQueryHandler(IAccountTypeRepository accountTypeRepository, IMapper mapper)
        {
            _accountTypeRepository = accountTypeRepository;
            _mapper = mapper;
        }

        public async Task<Result<AccountTypeDto>> Handle(GetAccountTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var accountType = await _accountTypeRepository.GetByIdAsync(request.Id);

            if (accountType == null)
            {
                return Result<AccountTypeDto>.Failure(new NotFoundError($"AccountType with ID {request.Id} not found."));
            }

            var accountTypeDto = _mapper.Map<AccountTypeDto>(accountType);

            return Result<AccountTypeDto>.Success(accountTypeDto);
        }
    }
}