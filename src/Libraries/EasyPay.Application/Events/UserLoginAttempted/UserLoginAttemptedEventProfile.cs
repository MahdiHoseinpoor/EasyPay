using AutoMapper;
using EasyPay.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Events.UserLoginAttempted
{
    public class UserLoginAttemptedEventProfile : Profile
    {
        public UserLoginAttemptedEventProfile()
        {
            CreateMap<UserLoginAttemptedEvent, LoginHistory>();
        }
    }
}
