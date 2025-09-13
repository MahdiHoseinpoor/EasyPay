using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Events.UserVerifyPasswordAttempted
{
    public class UserVerifyPasswordAttemptedEvent : INotification
    {
        public string Username { get; set; }

        public string UserId { get; set; }

        public DateTime VerifyPasswordTime { get; set; } = DateTime.Now;

        public string IPAddress { get; set; }

        public string UserAgent { get; set; }

        public string DeviceType { get; set; }

        public string OperatingSystem { get; set; }

        public string Browser { get; set; }

        public string Location { get; set; }

        public VerifyPasswordStatus Status { get; set; } = VerifyPasswordStatus.Success;

        public string FailureReason { get; set; }

        public string SessionId { get; set; }

        public bool IsPersistent { get; set; }

        public string AuthenticationMethod { get; set; }

        public bool TwoFactorUsed { get; set; }
        public string TwoFactorType { get; set; }
    }
}
