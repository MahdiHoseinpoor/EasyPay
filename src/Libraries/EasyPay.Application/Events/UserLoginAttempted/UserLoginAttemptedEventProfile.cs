using AutoMapper;
using EasyPay.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Events.UserVerifyPasswordAttempted
{
    public class UserVerifyPasswordAttemptedEventProfile : Profile
    {
        public UserVerifyPasswordAttemptedEventProfile()
        {
            CreateMap<UserVerifyPasswordAttemptedEvent, VerifyPasswordHistory>();
        }
    }
}
