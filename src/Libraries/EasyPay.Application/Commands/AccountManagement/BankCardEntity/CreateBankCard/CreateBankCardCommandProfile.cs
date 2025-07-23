using AutoMapper;
using EasyPay.Domain.Entities.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.CreateBankCard
{
    public class CreateBankCardCommandProfile: Profile
    {
        public CreateBankCardCommandProfile()
        {
            CreateMap<CreateBankCardCommand, BankCard>();
        }
    }
}
