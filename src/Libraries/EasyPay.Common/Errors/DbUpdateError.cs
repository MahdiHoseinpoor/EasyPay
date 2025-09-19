using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Common.Errors
{
    public record DbUpdateError(string message = "there is a error while updating db") : Error(600, message)
    {
    }
}
