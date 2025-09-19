using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.Identity
{
        public record SubmitAuthItemValueRequset(int AuthItemId, string Value, string? ExtraInfo = null);
}