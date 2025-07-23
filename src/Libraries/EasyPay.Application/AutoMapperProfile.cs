using AutoMapper;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.UpdateAccountType;
using EasyPay.Application.Commands.AccountManagement.BankCardEntity.CreateBankCard;
using EasyPay.Application.Commands.AccountManagement.BankCardEntity.UpdateBankCard;
using EasyPay.Application.Commands.Payment.MobileBillEntity.CreateMobileBill;
using EasyPay.Application.Commands.Payment.MobileBillEntity.UpdateMobileBill;
using EasyPay.Application.Commands.Payment.UtilityBillEntity.CreateUtilityBill;
using EasyPay.Application.Commands.Payment.UtilityBillEntity.UpdateUtilityBill;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateAccountCommand, Account>();
            CreateMap<UpdateAccountCommand, Account>();
            CreateMap<CreateAccountTypeCommand, AccountType>();
            CreateMap<UpdateAccountTypeCommand, AccountType>();
            CreateMap<CreateBankCardCommand, BankCard>();
            CreateMap<UpdateBankCardCommand, BankCard>();
            CreateMap<CreateMobileBillCommand, MobileBill>();
            CreateMap<UpdateMobileBillCommand, MobileBill>();
            CreateMap<CreateUtilityBillCommand, UtilityBill>();
            CreateMap<UpdateUtilityBillCommand, UtilityBill>();
        }
    }
}
