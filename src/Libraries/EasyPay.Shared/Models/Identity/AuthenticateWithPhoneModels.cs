using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Identity
{
    public record AuthenticateWithPhoneRequest(string phone, string code);
    public record AuthenticateWithPhoneResponse
    {
        public PhoneAuthenticationStatus Status { get; set; }
        public string Token { get; set; }
        public string Phone { get; set; }
        public AuthenticateWithPhoneResponse(PhoneAuthenticationStatus status, string phone, string token = null)
        {
            Status = status;
            Phone = phone;
            Token = token;
        }
    }
    public enum PhoneAuthenticationStatus
    {
        NeedsRegistration,
        NeedsPassword,
        LoggedIn
    }
}
