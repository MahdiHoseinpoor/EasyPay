
namespace EasyPay.Domain.Identity
{
    public class LegalUserBranch
    {
        public int Id { get; set; }
        public string LegalUserId { get; set; }
        public LegalUser LegalUser { get; set; }

        [MaxLength(100)]
        public string BranchName { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }
    }
}
