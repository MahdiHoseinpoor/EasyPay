using EasyPay.Domain.ValueObjects.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public interface IVerificationCodeCacheService : ICacheService
    {
        void Set(VerificationCode value);
        bool TryGetValue(string key, out VerificationCode value);
    }
}
