using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Shared.Enums.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.NaturalUserEntity.CreateNaturalUser
{
    public class CompleteNaturalUserProfileCommandHandler : IRequestHandler<CompleteNaturalUserProfileCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public CompleteNaturalUserProfileCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(CompleteNaturalUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Failure(new Error(401, "User is not authenticated."));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure(new NotFoundError("Authenticated user could not be found."));
            }

            if (user.CurrentStep == RegistrationStep.Completed)
            {
                return Result.Failure(new Error(400, "User profile has already been completed."));
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            if (user is NaturalUser naturalUser)
            {
                naturalUser.NationalCode = request.NationalCode;
                naturalUser.BirthDate = request.BirthDate;
                naturalUser.FatherName = request.FatherName;
                naturalUser.Gender = request.Gender;
            }
            else
            {
                return Result.Failure(new Error(500, "User is not a Natural User type and cannot be updated with these details."));
            }

            user.IsActive = true;
            user.CurrentStep = RegistrationStep.Completed;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Result.Failure(new Error(500, "Failed to update user profile."));
            }
            var applyRoleResult = await _userManager.AddToRoleAsync(user, SystemRoles.BasicUser);
            return Result.Success();
        }
    }
}