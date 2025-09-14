using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Models.AccountManagement
{
    public record CreateAccountRequest(int AccountTypeId, string Title);
}
