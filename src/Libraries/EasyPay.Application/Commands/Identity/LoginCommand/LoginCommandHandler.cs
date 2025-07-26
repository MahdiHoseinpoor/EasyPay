using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyPay.Application.Services;
using Microsoft.AspNetCore.Identity;
using EasyPay.Application.Events.UserLoginAttempted;
namespace EasyPay.Application.Commands.Identity.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPublisher _publisher;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            SignInManager<ApplicationUser> signInManager,
            IPublisher publisher,
            ITokenService tokenService)
        {
            _signInManager = signInManager;
            _publisher = publisher;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(
                request.Username,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            var userLoginAttemptedEvent = new UserLoginAttemptedEvent
            {
                Username = request.Username,
                LoginTime = DateTime.UtcNow,
                IPAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Status = signInResult.Succeeded ? LoginStatus.Success : LoginStatus.Failed,
                FailureReason = signInResult.Succeeded ? null : GetFailureReason(signInResult)
            };
                
            await _publisher.Publish(userLoginAttemptedEvent, cancellationToken);

            if (!signInResult.Succeeded)
            {
                return Result<LoginResponse>.Failure(new Error(0,GetFailureReason(signInResult)));
            }

            var user = await _signInManager.UserManager.FindByNameAsync(request.Username);
            var token = await _tokenService.GenerateToken(user);

            return Result<LoginResponse>.Success(new LoginResponse(token.Token, token.Expiry));
        }

        private string GetFailureReason(SignInResult result)
        {
            if (result.IsLockedOut) return "Account locked out";
            if (result.IsNotAllowed) return "Login not allowed";
            if (result.RequiresTwoFactor) return "Two factor required";
            return "Invalid username or password";
        }
    }
}
