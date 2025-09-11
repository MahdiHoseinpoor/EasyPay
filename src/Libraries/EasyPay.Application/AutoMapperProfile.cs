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
using EasyPay.Application.DTOs.AccountManagement;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Payment;
using System.Linq;

namespace EasyPay.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Commands to Entities
            CreateMap<CreateAccountCommand, Account>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.AccountNumber, opt => opt.Ignore()); 
            CreateMap<CreateAccountTypeCommand, AccountType>();
            CreateMap<UpdateAccountTypeCommand, AccountType>();
            CreateMap<UpdateAccountCommand, Account>();
            CreateMap<CreateAccountTypeCommand, AccountType>();
            CreateMap<UpdateAccountTypeCommand, AccountType>();
            CreateMap<CreateBankCardCommand, BankCard>();
            CreateMap<UpdateBankCardCommand, BankCard>();
            CreateMap<CreateMobileBillCommand, MobileBill>();
            CreateMap<UpdateMobileBillCommand, MobileBill>();
            CreateMap<CreateUtilityBillCommand, UtilityBill>();
            CreateMap<UpdateUtilityBillCommand, UtilityBill>();

            // Entities to DTOs
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.AccountTypeName, opt => opt.MapFrom(src => src.AccountType != null ? src.AccountType.Title : null));
            CreateMap<AccountType, AccountTypeDto>();
            CreateMap<BankCard, BankCardDto>()
                .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)));
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 16)
                return "************";

            return $"{cardNumber.Substring(0, 4)}********{cardNumber.Substring(12, 4)}";
        }
    }
}