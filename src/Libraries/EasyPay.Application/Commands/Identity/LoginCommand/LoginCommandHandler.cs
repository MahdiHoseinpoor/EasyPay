using EasyPay.Application.Events.UserLoginAttempted;
using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPublisher _publisher;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
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

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                await PublishLoginAttemptEvent(request, LoginStatus.Failed, "Invalid username or password");
                return Result<LoginResponse>.Failure(new AuthenticationError());
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                var reason = GetFailureReason(signInResult);
                await PublishLoginAttemptEvent(request, LoginStatus.Failed, reason, user.Id);
                return Result<LoginResponse>.Failure(new AuthenticationError(reason));
            }

            var token = await _tokenService.GenerateToken(user);
            await PublishLoginAttemptEvent(request, LoginStatus.Success, userId: user.Id);

            return Result<LoginResponse>.Success(new LoginResponse(token.Token, token.Expiry));
        }

        private async Task PublishLoginAttemptEvent(LoginCommand request, LoginStatus status, string failureReason = null, string userId = null)
        {
            var userLoginAttemptedEvent = new UserLoginAttemptedEvent
            {
                Username = request.Username,
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IPAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Status = status,
                FailureReason = failureReason
            };

            await _publisher.Publish(userLoginAttemptedEvent);
        }

        private string GetFailureReason(SignInResult result)
        {
            if (result.IsLockedOut) return "Account is locked out.";
            if (result.IsNotAllowed) return "Login is not allowed for this user.";
            if (result.RequiresTwoFactor) return "Two-factor authentication is required.";
            return "Invalid username or password.";
        }
    }
}