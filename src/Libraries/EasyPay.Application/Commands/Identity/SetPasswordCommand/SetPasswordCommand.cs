using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.SetPasswordCommand
{
    public class SetPasswordCommand : IRequest<Result>
    {
        public SetPasswordCommand(string newPassword)
        {
            CurrentPassword = string.Empty;
            NewPassword = newPassword;
        }
        public SetPasswordCommand(string currentPassword, string newPassword) {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
        public string NewPassword { get; set; }
        public string CurrentPassword { get; set; }
    }
}
