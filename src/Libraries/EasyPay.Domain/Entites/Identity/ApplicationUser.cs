using EasyPay.Domain.Entites.AccountManagement;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyPay.Domain.Entites.Identity
{
    public class ApplicationUser : IdentityUser, IEntityBase
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public UserType UserType { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsAuthorized { get; set; } = true;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public DateTime? PasswordChangeDate { get; set; }
        public bool ForcePasswordChange { get; set; }

        public virtual ICollection<AuthItemValue> AuthItemValues { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<LoginHistory> LoginHistories { get; set; }

        public string FullName => $"{FirstName} {LastName}";

    }


}