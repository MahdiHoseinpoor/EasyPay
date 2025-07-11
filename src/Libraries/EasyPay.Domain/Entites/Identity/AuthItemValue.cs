using EasyPay.Domain.Enums.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Domain.Entites.Identity
{
    public class AuthItemValue : EntityBase<long>
    {
        public int AuthItemId { get; set; }
        public virtual AuthItem AuthItem { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Value { get; set; } 
        public string ExtraInfo { get; set; }
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
        public string RejectionReason { get; set; } 
        public DateTime? VerificationDate { get; set; } 
        public string VerifiedByUserId { get; set; }
        public virtual ApplicationUser VerifiedByUser { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;
        public DateTime? LastStatusChangeDate { get; set; }
     
        public int Version { get; set; } = 1;
        public bool IsLatestVersion { get; set; } = true;
    }
}
