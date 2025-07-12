using EasyPay.Domain.Enums.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPay.Domain.Entites.Identity
{
    public class NaturalUser : ApplicationUser
    {

        public string NationalCode { get; set; }

        public DateTime BirthDate { get; set; }

        public string BirthCertificateNumber { get; set; }

        public string FatherName { get; set; }

        public GenderType Gender { get; set; }

        public string PlaceOfBirth { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        public string Occupation { get; set; }

        public string EducationLevel { get; set; }

    }
}