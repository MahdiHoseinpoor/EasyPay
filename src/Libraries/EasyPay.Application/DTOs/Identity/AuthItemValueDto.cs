using EasyPay.Domain.Enums.Identity;
using System;

namespace EasyPay.Application.DTOs.Identity
{
    public class AuthItemValueDto
    {
        public long Id { get; set; }
        public string AuthItemTitle { get; set; }
        public string Value { get; set; }
        public VerificationStatus Status { get; set; }
        public DateTime UploadDate { get; set; }
        public string RejectionReason { get; set; }
        public DateTime? VerificationDate { get; set; }
    }
}