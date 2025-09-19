using System.Collections.Generic;

namespace EasyPay.Common.Errors
{
    public record ValidationError(Dictionary<string, string[]> Errors)
        : Error(400, "One or more validation errors occurred.");
}