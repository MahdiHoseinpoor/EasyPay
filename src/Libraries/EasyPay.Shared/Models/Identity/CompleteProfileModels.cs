using EasyPay.Shared.Enums.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Identity
{
    public class CompleteProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string FatherName { get; set; }
        public GenderType Gender { get; set; }
    }
}
