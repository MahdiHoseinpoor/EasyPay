using EasyPay.Domain.Enums.Identity;

namespace EasyPay.Domain.Entities.Identity
{
    public class AdminUser : ApplicationUser
    {
        public string EmployeeCode { get; set; }

        public AdminLevel AdminLevel { get; set; }

        public string Department { get; set; }
    }
}
