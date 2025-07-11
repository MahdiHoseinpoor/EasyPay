using EasyPay.Domain.Enums.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPay.Domain.Entites.Identity
{
    public class LegalUser : ApplicationUser
    {
        [Required]
        [StringLength(20)]
        public string CompanyNationalId { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(50)]
        public string RegistrationNumber { get; set; }

        public DateTime? RegistrationDate { get; set; }

        [StringLength(50)]
        public string EconomicCode { get; set; }

        [StringLength(50)]
        public string TaxId { get; set; }

        [StringLength(100)]
        public string ContactPersonName { get; set; }

        [StringLength(50)]
        public string ContactPersonPosition { get; set; }

        public CompanyType CompanyType { get; set; }

        [StringLength(200)]
        public string BusinessDescription { get; set; }

        public virtual ICollection<LegalUserBranch> Branches { get; set; }
        public virtual ICollection<LegalUserRepresentative> Representatives { get; set; }
    }
}