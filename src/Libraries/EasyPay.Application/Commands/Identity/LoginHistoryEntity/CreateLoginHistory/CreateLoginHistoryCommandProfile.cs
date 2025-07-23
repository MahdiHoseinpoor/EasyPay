using AutoMapper;
using EasyPay.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.LoginHistoryEntity.CreateLoginHistory
{
    public class CreateLoginHistoryCommandProfile : Profile
    {
        public CreateLoginHistoryCommandProfile()
        {
            CreateMap<CreateLoginHistoryCommand, LoginHistory>();
        }
    }
}
