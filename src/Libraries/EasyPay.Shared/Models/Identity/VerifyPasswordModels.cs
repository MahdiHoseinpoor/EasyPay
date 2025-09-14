using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Identity
{
    public record VerifyPasswordRequest(string Username, string Password);
    public record VerifyPasswordResponse(string Token, DateTime Expiry);
}
