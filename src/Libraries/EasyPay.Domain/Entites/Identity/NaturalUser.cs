using EasyPay.Domain.Enums.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPay.Domain.Entites.Identity
{
    public class NaturalUser : ApplicationUser
    {
        [Required]
        [StringLength(10)]
        public string NationalCode { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [MaxLength(10)]
        public string BirthCertificateNumber { get; set; }

        [MaxLength(50)]
        public string FatherName { get; set; }

        public GenderType Gender { get; set; }

        [MaxLength(50)]
        public string PlaceOfBirth { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }

        [MaxLength(100)]
        public string EducationLevel { get; set; }

    }
}