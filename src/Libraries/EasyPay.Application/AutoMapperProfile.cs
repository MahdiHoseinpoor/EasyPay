using AutoMapper;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount;
using EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType;
using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.UpdateAccountType;
using EasyPay.Application.Commands.AccountManagement.BankCardEntity.CreateBankCard;
using EasyPay.Application.Commands.AccountManagement.BankCardEntity.UpdateBankCard;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.DTOs.Identity;
using EasyPay.Shared.DTOs.Report;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Entities.Report;
using Microsoft.AspNetCore.Identity;
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
            CreateMap<CreateBankCardCommand, BankCard>();
            CreateMap<UpdateBankCardCommand, BankCard>();
            CreateMap<IdentityRole, RoleDto>();
            CreateMap<CreateAccountCommand, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountNumber, opt => opt.Ignore());
            CreateMap<CreateAccountTypeDocumentRequirementCommand, AccountTypeDocumentRequirement>();
            CreateMap<Account, AccountDto>()
             .ForMember(dest => dest.AccountTypeName, opt => opt.MapFrom(src => src.AccountType != null ? src.AccountType.Title : null));
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.AccountTypeName, opt => opt.MapFrom(src => src.AccountType != null ? src.AccountType.Title : null));
            CreateMap<AccountType, AccountTypeDto>();
            CreateMap<BankCard, BankCardDto>()
                .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)));
            CreateMap<Transaction, TransactionDto>();
            CreateMap<AuthItem, AuthItemDto>();
            CreateMap<AuthItemValue, AuthItemValueDto>()
                .ForMember(dest => dest.AuthItemTitle, opt => opt.MapFrom(src => src.AuthItem != null ? src.AuthItem.Title : "Unknown"));
            CreateMap<AccountTypeDocumentRequirement, AccountTypeDocumentRequirementDto>()
              .ForMember(dest => dest.AuthItemTitle, opt => opt.MapFrom(src => src.AuthItem != null ? src.AuthItem.Title : "Unknown"));
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 16)
                return "************";

            return $"{cardNumber.Substring(0, 4)}********{cardNumber.Substring(12, 4)}";
        }
    }
}