using EasyPay.Shared.Enums.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.DTOs.Identity
{
    public class AuthItemDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public AuthItemType AuthItemType { get; set; }
        public AuthItemValueType AuthItemValueType { get; set; }
    }
}
