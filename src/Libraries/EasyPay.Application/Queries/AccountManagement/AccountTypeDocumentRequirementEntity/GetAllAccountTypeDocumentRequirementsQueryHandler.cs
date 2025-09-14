using AutoMapper;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.AccountTypeDocumentRequirementEntity
{
    public class GetAllAccountTypeDocumentRequirementsQueryHandler : IRequestHandler<GetAllAccountTypeDocumentRequirementsQuery, Result<List<AccountTypeDocumentRequirementDto>>>
    {
        private readonly IAccountTypeDocumentRequirementRepository _repository;
        private readonly IMapper _mapper;

        public GetAllAccountTypeDocumentRequirementsQueryHandler(IAccountTypeDocumentRequirementRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<AccountTypeDocumentRequirementDto>>> Handle(GetAllAccountTypeDocumentRequirementsQuery request, CancellationToken cancellationToken)
        {
            var requirements = await _repository.Query(
                predicate: r => r.AccountTypeId == request.AccountTypeId,
                orderBy: r => r.Order,
                Includes: new List<System.Linq.Expressions.Expression<System.Func<Domain.Entities.AccountManagement.AccountTypeDocumentRequirement, object>>> { r => r.AuthItem },
                disableTracking: true
            ).ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<AccountTypeDocumentRequirementDto>>(requirements);

            return Result<List<AccountTypeDocumentRequirementDto>>.Success(dtos);
        }
    }
}