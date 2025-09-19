using AutoMapper;
using EasyPay.Common;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.Identity;
using EasyPay.Shared.DTOs.Identity;
using EasyPay.Shared.Enums.Identity;
using MediatR;
using System.Linq.Expressions;

namespace EasyPay.Application.Queries.Identity.AuthItemValueEntity.GetPendingSubmissions
{
    public class GetPendingSubmissionsQueryHandler : IRequestHandler<GetPendingSubmissionsQuery, Result<IPagedList<PendingSubmissionDto>>>
    {
        private readonly IAuthItemValueRepository _repository;
        private readonly IMapper _mapper;

        public GetPendingSubmissionsQueryHandler(IAuthItemValueRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<IPagedList<PendingSubmissionDto>>> Handle(GetPendingSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var includes = new List<Expression<Func<AuthItemValue, object>>> { v => v.User, v => v.AuthItem };

            var pagedSubmissions = await _repository.GetPagedListAsync(
                predicate: v => v.Status == VerificationStatus.Pending,
                orderByDescending: v => v.UploadDate,
                pageIndex: request.PageIndex,
                pageSize: request.PageSize,
                Includes: includes
            );

            var dtos = _mapper.Map<List<PendingSubmissionDto>>(pagedSubmissions.Items);

            var pagedResult = new PagedList<PendingSubmissionDto>(dtos, pagedSubmissions.PageIndex, pagedSubmissions.PageSize, pagedSubmissions.TotalCount);

            return Result<IPagedList<PendingSubmissionDto>>.Success(pagedResult);
        }
    }
}