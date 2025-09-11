using AutoMapper;
using EasyPay.Application.DTOs.Identity;
using EasyPay.Application.Services;
using EasyPay.Infrastructure.Aggregates.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EasyPay.Domain.Entities.Identity;

namespace EasyPay.Application.Queries.Identity.AuthItemValueEntity
{
    public class GetMySubmittedDocumentsQueryHandler : IRequestHandler<GetMySubmittedDocumentsQuery, Result<List<AuthItemValueDto>>>
    {
        private readonly IAuthItemValueRepository _authItemValueRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetMySubmittedDocumentsQueryHandler(IAuthItemValueRepository authItemValueRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _authItemValueRepository = authItemValueRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<AuthItemValueDto>>> Handle(GetMySubmittedDocumentsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<List<AuthItemValueDto>>.Failure(new Error(401, "User is not authenticated."));
            }

            var includes = new List<Expression<System.Func<AuthItemValue, object>>> { v => v.AuthItem };

            var submissions = await _authItemValueRepository
                .Query(v => v.UserId == userId, orderByDescending: v => v.UploadDate, Includes: includes)
                .ToListAsync(cancellationToken);

            var submissionDtos = _mapper.Map<List<AuthItemValueDto>>(submissions);
            return Result<List<AuthItemValueDto>>.Success(submissionDtos);
        }
    }
}