using EasyPay.Domain.Enums.Identity;

namespace EasyPay.Domain.Entites.Identity
{
    public class AdminUser : ApplicationUser
    {
        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; }

        public AdminLevel AdminLevel { get; set; }

        [MaxLength(100)]
        public string Department { get; set; }
    }
}
