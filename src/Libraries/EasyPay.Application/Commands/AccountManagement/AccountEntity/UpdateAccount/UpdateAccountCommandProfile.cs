using AutoMapper;
using EasyPay.Domain.Entities.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount
{
    public class UpdateAccountCommandProfile : Profile
    {
        public UpdateAccountCommandProfile()
        {
            CreateMap<UpdateAccountCommand, Account>();
        }
    }
}
