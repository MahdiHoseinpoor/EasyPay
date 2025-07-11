namespace EasyPay.Domain.Identity
{
    public class LegalUserRepresentative
    {
        public int Id { get; set; }
        public string LegalUserId { get; set; }
        public LegalUser LegalUser { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string NationalCode { get; set; }

        [MaxLength(50)]
        public string Position { get; set; }

        public bool IsAuthorizedSignatory { get; set; }
    }
}
