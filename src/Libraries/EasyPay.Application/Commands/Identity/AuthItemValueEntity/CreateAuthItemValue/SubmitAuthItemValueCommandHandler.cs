using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.Identity;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Application.Commands.Identity.AuthItemValueEntity.CreateAuthItemValue
{
    public class SubmitAuthItemValueCommandHandler : IRequestHandler<SubmitAuthItemValueCommand, Result<long>>
    {
        private readonly IAuthItemValueRepository _authItemValueRepository;
        private readonly ICurrentUserService _currentUserService;

        public SubmitAuthItemValueCommandHandler(IAuthItemValueRepository authItemValueRepository, ICurrentUserService currentUserService)
        {
            _authItemValueRepository = authItemValueRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<long>> Handle(SubmitAuthItemValueCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<long>.Failure(new Error(401, "User is not authenticated."));
            }
            var previousSubmissions = await _authItemValueRepository
                .Query(v => v.UserId == userId && v.AuthItemId == request.AuthItemId)
                .ToListAsync(cancellationToken);


            foreach (var oldSubmission in previousSubmissions)
            {
                oldSubmission.IsLatestVersion = false;
            }
            int newVersion = previousSubmissions.Any() ? previousSubmissions.Max(v => v.Version) + 1 : 1;
            var newAuthItemValue = new AuthItemValue(request.AuthItemId, userId, request.Value)
            {
                ExtraInfo = request.ExtraInfo,
                Status = Shared.Enums.Identity.VerificationStatus.Pending,
                UploadDate = DateTime.UtcNow,
                LastStatusChangeDate = DateTime.UtcNow,
                Version = newVersion,
                IsLatestVersion = true
            };

            await _authItemValueRepository.AddAsync(newAuthItemValue);
            await _authItemValueRepository.SaveChangesAsync();

            return Result<long>.Success(newAuthItemValue.Id);
        }
    }
}