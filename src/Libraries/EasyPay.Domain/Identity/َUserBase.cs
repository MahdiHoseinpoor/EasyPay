using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EasyPay.Domain.Identity
{
    public class UserBase:IdentityUser, IEntityBase
    {
        public UserType UserType { get; set; }

    }
}
