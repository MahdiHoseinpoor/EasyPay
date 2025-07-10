using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Domain.Identity
{
    public class AuthItemValue : EntityBase<long>
    {
        public int AuthItemId { get; set; }
        public string UserId { get; set; }
        public AuthItemValueType AuthItemValueType { get; set; }
        public string Value { get; set; }
    }
}
