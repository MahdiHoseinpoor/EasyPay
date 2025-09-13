using EasyPay.Application.Events.UserVerifyPasswordAttempted;
using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.VerifyPasswordCommand
{
    public class VerifyPasswordCommandHandler : IRequestHandler<VerifyPasswordCommand, Result<VerifyPasswordResponse>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPublisher _publisher;
        private readonly ITokenService _tokenService;

        public VerifyPasswordCommandHandler(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IPublisher publisher,
            ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _publisher = publisher;
            _tokenService = tokenService;
        }

        public async Task<Result<VerifyPasswordResponse>> Handle(VerifyPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                await PublishVerifyPasswordAttemptEvent(request, VerifyPasswordStatus.Failed, "Invalid username or password");
                return Result<VerifyPasswordResponse>.Failure(new AuthenticationError());
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                var reason = GetFailureReason(signInResult);
                await PublishVerifyPasswordAttemptEvent(request, VerifyPasswordStatus.Failed, reason, user.Id);
                return Result<VerifyPasswordResponse>.Failure(new AuthenticationError(reason));
            }

            var token = await _tokenService.GenerateToken(user);
            await PublishVerifyPasswordAttemptEvent(request, VerifyPasswordStatus.Success, userId: user.Id);

            return Result<VerifyPasswordResponse>.Success(new VerifyPasswordResponse(token.Token, token.Expiry));
        }

        private async Task PublishVerifyPasswordAttemptEvent(VerifyPasswordCommand request, VerifyPasswordStatus status, string failureReason = null, string userId = null)
        {
            var userVerifyPasswordAttemptedEvent = new UserVerifyPasswordAttemptedEvent
            {
                Username = request.Username,
                UserId = userId,
                VerifyPasswordTime = DateTime.UtcNow,
                IPAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Status = status,
                FailureReason = failureReason
            };

            await _publisher.Publish(userVerifyPasswordAttemptedEvent);
        }

        private string GetFailureReason(SignInResult result)
        {
            if (result.IsLockedOut) return "Account is locked out.";
            if (result.IsNotAllowed) return "VerifyPassword is not allowed for this user.";
            if (result.RequiresTwoFactor) return "Two-factor authentication is required.";
            return "Invalid username or password.";
        }
    }
}