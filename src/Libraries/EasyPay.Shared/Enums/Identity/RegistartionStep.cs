using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Shared.Enums.Identity
{
    public enum RegistrationStep
    {
        PhoneNumberEntry,
        PhoneNumberVerification,
        BasicInfo,
        Completed
    }
}
