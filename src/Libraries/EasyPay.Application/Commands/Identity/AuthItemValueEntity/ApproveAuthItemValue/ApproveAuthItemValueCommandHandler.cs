using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Infrastructure.Aggregates.Identity;
using EasyPay.Shared.Enums.Identity;
using MediatR;

namespace EasyPay.Application.Commands.Identity.AuthItemValueEntity.ApproveAuthItemValue
{
    public class ApproveAuthItemValueCommandHandler : IRequestHandler<ApproveAuthItemValueCommand, Result>
    {
        private readonly IAuthItemValueRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public ApproveAuthItemValueCommandHandler(IAuthItemValueRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(ApproveAuthItemValueCommand request, CancellationToken cancellationToken)
        {
            var submission = await _repository.GetByIdAsync(request.AuthItemValueId);
            if (submission == null)
            {
                return Result.Failure(new NotFoundError("Submission not found."));
            }

            if (submission.Status != VerificationStatus.Pending)
            {
                return Result.Failure(new BusinessRuleError($"Cannot approve a submission with status '{submission.Status}'."));
            }

            var adminUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(adminUserId))
            {
                return Result.Failure(new AuthorizationError("Administrator not identified."));
            }

            submission.Status = VerificationStatus.Approved;
            submission.VerifiedByUserId = adminUserId;
            submission.VerificationDate = DateTime.UtcNow;
            submission.LastStatusChangeDate = DateTime.UtcNow;
            submission.RejectionReason = null;

            await _repository.UpdateAsync(submission);
            await _repository.SaveChangesAsync();

            return Result.Success();
        }
    }
}