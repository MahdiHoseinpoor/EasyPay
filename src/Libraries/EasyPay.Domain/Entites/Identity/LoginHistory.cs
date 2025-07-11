using EasyPay.Common;
using EasyPay.Domain.Enums.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPay.Domain.Entites.Identity
{
    public class LoginHistory : EntityBase<long>
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public DateTime LoginTime { get; set; } = DateTime.Now;

        public DateTime? LogoutTime { get; set; }

        [MaxLength(45)]
        public string IPAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        [MaxLength(100)]
        public string DeviceType { get; set; }

        [MaxLength(100)]
        public string OperatingSystem { get; set; }

        [MaxLength(100)]
        public string Browser { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public LoginStatus Status { get; set; } = LoginStatus.Success;

        [MaxLength(500)]
        public string FailureReason { get; set; }

        [MaxLength(100)]
        public string SessionId { get; set; }

        public bool IsPersistent { get; set; }

        [MaxLength(100)]
        public string AuthenticationMethod { get; set; }

        public bool TwoFactorUsed { get; set; }
        public string TwoFactorType { get; set; }

        public TimeSpan? Duration
        {
            get
            {
                if (LoginTime != default && LogoutTime.HasValue)
                    return LogoutTime.Value - LoginTime;
                return null;
            }
        }
    }
}