using AutoMapper;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.Identity;
using EasyPay.Shared.DTOs.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyPay.Application.Queries.Identity.AuthItemValueEntity.GetSubmissionDetail
{
    public class GetSubmissionDetailQueryHandler : IRequestHandler<GetSubmissionDetailQuery, Result<SubmissionDetailDto>>
    {
        private readonly IAuthItemValueRepository _repository;
        private readonly IMapper _mapper;

        public GetSubmissionDetailQueryHandler(IAuthItemValueRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SubmissionDetailDto>> Handle(GetSubmissionDetailQuery request, CancellationToken cancellationToken)
        {
            var includes = new List<Expression<Func<AuthItemValue, object>>> { v => v.User, v => v.AuthItem };
            var submission = await _repository.Query(v => v.Id == request.Id, Includes: includes).FirstOrDefaultAsync(cancellationToken);

            if (submission == null)
            {
                return Result<SubmissionDetailDto>.Failure(new NotFoundError($"Submission with ID {request.Id} not found."));
            }

            var dto = _mapper.Map<SubmissionDetailDto>(submission);
            return Result<SubmissionDetailDto>.Success(dto);
        }
    }
}