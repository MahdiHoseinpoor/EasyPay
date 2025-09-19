namespace EasyPay.Shared.DTOs.Identity
{
    public class PendingSubmissionDto
    {
        public long AuthItemValueId { get; set; }
        public string UserFullName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string AuthItemTitle { get; set; }
        public DateTime UploadDate { get; set; }
    }
}