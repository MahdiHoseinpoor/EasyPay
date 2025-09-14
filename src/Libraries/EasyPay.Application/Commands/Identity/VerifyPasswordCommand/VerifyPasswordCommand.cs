using EasyPay.Shared.Models.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.VerifyPasswordCommand
{
    public class VerifyPasswordCommand : IRequest<Result<VerifyPasswordResponse>>
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public string IpAddress { get; init; }
        public string UserAgent { get; init; }
    }
}
