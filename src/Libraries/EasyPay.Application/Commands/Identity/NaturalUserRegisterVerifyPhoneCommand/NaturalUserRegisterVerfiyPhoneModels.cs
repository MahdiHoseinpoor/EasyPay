using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.NaturalUserRegisterVerifyPhoneCommand
{
    public record NaturalUserRegisterVerifyPhoneRequest(string phone,string code);
    public record NaturalUserRegisterVerifyPhoneResponse(string token);
}
