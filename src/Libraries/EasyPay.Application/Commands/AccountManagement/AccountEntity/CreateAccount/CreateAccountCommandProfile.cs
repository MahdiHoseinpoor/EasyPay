using AutoMapper;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType;
using EasyPay.Domain.Entities.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount
{
    public class CreateAccountCommandProfile : Profile
    {
        public CreateAccountCommandProfile()
        {
            CreateMap<CreateAccountCommand, Account>();
        }
    }
}
