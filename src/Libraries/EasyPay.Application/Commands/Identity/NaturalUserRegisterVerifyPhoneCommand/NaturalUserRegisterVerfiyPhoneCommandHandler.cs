using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.ValueObjects.Identity;
using EasyPay.Domain.Enums.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.NaturalUserRegisterVerifyPhoneCommand
{
    public class NaturalUserRegisterVerfiyPhoneCommandHandler : IRequestHandler<NaturalUserRegisterVerfiyPhoneCommand, Result<NaturalUserRegisterVerifyPhoneResponse>>
    {
        private readonly IVerificationCodeCacheService _cache;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NaturalUserRegisterVerfiyPhoneCommandHandler(IVerificationCodeCacheService cache, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _cache = cache;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<NaturalUserRegisterVerifyPhoneResponse>> Handle(NaturalUserRegisterVerfiyPhoneCommand request, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(request.Phone, out VerificationCode storedCode))
            {
                if (storedCode.Code == request.Code && !storedCode.IsExpired)
                {
                    var existingUser = await _userManager.FindByNameAsync(request.Phone);
                    if (existingUser != null)
                    {
                        return Result<NaturalUserRegisterVerifyPhoneResponse>.Failure(
                            new Error(409, "A user with this phone number already exists."));
                    }

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
                        return Result<NaturalUserRegisterVerifyPhoneResponse>.Failure(
                            new Error(500, $"User creation failed: {errors}"));
                    }

                    _cache.Remove(request.Phone);

     
                    var token = await _tokenService.GenerateToken(newUser);

                    var response = new NaturalUserRegisterVerifyPhoneResponse(token.Token);
                    return Result<NaturalUserRegisterVerifyPhoneResponse>.Success(response);
                }
            }

            return Result<NaturalUserRegisterVerifyPhoneResponse>.Failure(new Error(400, "Invalid or expired verification code."));
        }
    }
}