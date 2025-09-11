using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Domain.ValueObjects.Identity
{
    public record VerificationCode
    {
        public string Code { get; }
        public string Phone { get; }
        public DateTime CreatedAt { get; }
        public TimeSpan Duration { get; }

        public bool IsExpired => DateTime.UtcNow > CreatedAt.Add(Duration);

        public VerificationCode(string phone, TimeSpan duration)
        {
            Code = GenerateRandomCode();
            Phone = phone;
            CreatedAt = DateTime.UtcNow;
            Duration = duration;
        }

        private string? GenerateRandomCode()
        {
            return RandomNumberGenerator.GetInt32(10000, 99999).ToString();
        }
    }
}
