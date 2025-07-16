using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyPay.Domain.Entities.AccountManagement;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType
{
    public class CreateAccountTypeCommandProfile : Profile
    {
        public CreateAccountTypeCommandProfile()
        {
            CreateMap<CreateAccountTypeCommand, AccountType>();
        }
    }
}
