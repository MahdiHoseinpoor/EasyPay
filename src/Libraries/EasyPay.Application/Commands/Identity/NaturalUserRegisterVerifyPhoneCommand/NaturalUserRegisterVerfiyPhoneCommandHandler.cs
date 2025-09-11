using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.ValueObjects.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.NaturalUserRegisterVerifyPhoneCommand
{
    public class NaturalUserRegisterVerfiyPhoneCommandHandler : IRequestHandler<NaturalUserRegisterVerfiyPhoneCommand, Result<NaturalUserRegisterVerifyPhoneResponse>>
    {
        IVerificationCodeCacheService _cache;
        ITokenService _tokenService;
        UserManager<ApplicationUser> _userManager;
        public NaturalUserRegisterVerfiyPhoneCommandHandler(IVerificationCodeCacheService cache, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _cache = cache;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<Result<NaturalUserRegisterVerifyPhoneResponse>> Handle(NaturalUserRegisterVerfiyPhoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_cache.TryGetValue(request.Code, out VerificationCode verificationCode)) { 
                    if(verificationCode.Code == request.Code && verificationCode.IsExpired)
                    {
                        var newUser = new ApplicationUser()
                        {
                            UserName = request.Phone,
                            PhoneNumber = request.Phone,
                            PhoneNumberConfirmed = true,
                            IsActive = false
                        };
                        var existingUser = await _userManager.FindByNameAsync(request.Phone);
                        if (existingUser != null)
                        {
                            return Result<NaturalUserRegisterVerifyPhoneResponse>.Failure(
                                new Error(409, "User already exists"));
                        }
                        var createResult = await _userManager.CreateAsync(newUser);
                        if (!createResult.Succeeded)
                        {
                            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));  
                            return Result<NaturalUserRegisterVerifyPhoneResponse>.Failure(
                                new Error(500, $"User creation failed: {errors}"));
                        }
                        _cache.Remove(request.Phone);
                        var token = await _tokenService.GenerateToken(newUser);
                    }
                }
                return Result<NaturalUserRegisterVerifyPhoneResponse>.Failure(new Error(401, "Invalid verification code"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
