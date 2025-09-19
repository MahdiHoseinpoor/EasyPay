using EasyPay.Shared.Enums.Identity;

namespace EasyPay.Shared.DTOs.Identity
{
    public class SubmissionDetailDto
    {
        // Submission Info
        public long Id { get; set; }
        public string Value { get; set; }
        public string? ExtraInfo { get; set; }
        public VerificationStatus Status { get; set; }
        public DateTime UploadDate { get; set; }
        public int Version { get; set; }

        // Auth Item Info
        public string AuthItemTitle { get; set; }
        public string AuthItemDescription { get; set; }
        public AuthItemType AuthItemType { get; set; }

        // User Info
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public DateTime UserRegistrationDate { get; set; }
    }
}