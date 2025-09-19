using EasyPay.Application.Services;
using EasyPay.Domain.ValueObjects.Identity;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.NaturalUserRegisterPhoneCommand
{
    public class NaturalUserRegisterPhoneCommandHandler : IRequestHandler<NaturalUserRegisterPhoneCommand, Result>
    {
        private readonly IVerificationCodeCacheService _cache;
        private readonly ISmsService _smsService;

        public NaturalUserRegisterPhoneCommandHandler(IVerificationCodeCacheService cache, ISmsService smsService)
        {
            _cache = cache;
            _smsService = smsService;
        }

        public async Task<Result> Handle(NaturalUserRegisterPhoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var verificationCode = new VerificationCode(request.Phone, TimeSpan.FromSeconds(90));

                _cache.Set(verificationCode);

                await _smsService.SendVerificationCodeAsync(verificationCode.Phone, verificationCode.Code);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(500, "An unexpected error occurred while sending the verification code."));
            }
        }
    }
}