using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.ValueObjects.Identity;
using EasyPay.Shared.Enums.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.AuthenticateWithPhone
{
    public class AuthenticateWithPhoneCommandHandler : IRequestHandler<AuthenticateWithPhoneCommand, Result<AuthenticateWithPhoneResponse>>
    {
        private readonly IVerificationCodeCacheService _cache;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticateWithPhoneCommandHandler(IVerificationCodeCacheService cache, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _cache = cache;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthenticateWithPhoneResponse>> Handle(AuthenticateWithPhoneCommand request, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(request.Phone, out VerificationCode storedCode) || storedCode.Code != request.Code || storedCode.IsExpired)
            {
                return Result<AuthenticateWithPhoneResponse>.Failure(new Error(400, "Invalid or expired verification code."));
            }

            _cache.Remove(request.Phone);

            var existingUser = await _userManager.FindByNameAsync(request.Phone);

            if (existingUser == null)
            {
                var newUser = new ApplicationUser()
                {
                    UserName = request.Phone,
                    PhoneNumber = request.Phone,
                    PhoneNumberConfirmed = true,
                    IsActive = false,
                    UserType = UserType.Natural,
                    CurrentStep = RegistrationStep.PhoneNumberVerification
                };

                var createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return Result<AuthenticateWithPhoneResponse>.Failure(
                        new Error(500, $"User creation failed: {errors}"));
                }

                var token = await _tokenService.GenerateToken(newUser);
                var response = new AuthenticateWithPhoneResponse(PhoneAuthenticationStatus.NeedsRegistration, request.Phone, token.Token);
                return Result<AuthenticateWithPhoneResponse>.Success(response);
            }

            if (existingUser.TwoFactorEnabled && await _userManager.HasPasswordAsync(existingUser))
            {
                var response = new AuthenticateWithPhoneResponse(PhoneAuthenticationStatus.NeedsPassword, existingUser.PhoneNumber);
                return Result<AuthenticateWithPhoneResponse>.Success(response);
            }
            else
            {
                var token = await _tokenService.GenerateToken(existingUser);
                var response = new AuthenticateWithPhoneResponse(PhoneAuthenticationStatus.LoggedIn, existingUser.PhoneNumber, token.Token);
                return Result<AuthenticateWithPhoneResponse>.Success(response);
            }
        }
    }
}