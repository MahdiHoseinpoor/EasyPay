using EasyPay.Shared.Enums.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Domain.Entities.Identity
{
    public class AuthItemValue : EntityBase<long>
    {
        private AuthItemValue() { }

        [SetsRequiredMembers]
        public AuthItemValue(int authItemId, string userId, string value)
        {
            AuthItemId = authItemId;
            UserId = userId;
            Value = value;
            Status = VerificationStatus.Pending;
            UploadDate = DateTime.UtcNow;
            LastStatusChangeDate = DateTime.UtcNow;
            Version = 1;
            IsLatestVersion = true;
        }

        public int AuthItemId { get; set; }
        public virtual AuthItem AuthItem { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Value { get; set; }
        public string? ExtraInfo { get; set; }
        public VerificationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? VerifiedByUserId { get; set; }
        public virtual ApplicationUser VerifiedByUser { get; set; }

        public DateTime UploadDate { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }

        public int Version { get; set; }
        public bool IsLatestVersion { get; set; }
    }
}