using EasyPay.Domain.ValueObjects.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public class VerificationCodeCacheService : CacheService , IVerificationCodeCacheService
    {
        public VerificationCodeCacheService(IMemoryCache cache) : base(cache)
        {
        }

        public void Set(VerificationCode value)
        {
            Set(value.Phone, value, value.Duration);
        }

        public bool TryGetValue(string key, out VerificationCode value)
        {
            if (base.TryGetValue(key, out value))
            {
                return true; 
            }
            else return false;
        }
    }
}
