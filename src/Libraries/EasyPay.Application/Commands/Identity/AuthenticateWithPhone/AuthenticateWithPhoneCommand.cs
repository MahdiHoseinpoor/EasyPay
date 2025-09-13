using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.AuthenticateWithPhone
{
    public class NaturalUserRegisterVerfiyPhoneCommand :IRequest<Result<AuthenticateWithPhoneResponse>>
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }
}
