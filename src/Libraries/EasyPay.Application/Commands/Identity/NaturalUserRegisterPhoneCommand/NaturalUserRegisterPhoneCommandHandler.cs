using EasyPay.Application.Services;
using EasyPay.Domain.ValueObjects.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.NaturalUserRegisterPhoneCommand
{
    public class NaturalUserRegisterPhoneCommandHandler : IRequestHandler<NaturalUserRegisterPhoneCommand, Result>
    {
        IVerificationCodeCacheService _cache;
        public NaturalUserRegisterPhoneCommandHandler(IVerificationCodeCacheService cache)
        {
            _cache = cache;
        }

        public async Task<Result> Handle(NaturalUserRegisterPhoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var verificationCode = new VerificationCode(request.Phone,TimeSpan.FromSeconds(90));
                _cache.Set(verificationCode);
                //Send verification code with sms
                return Result.Success();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
