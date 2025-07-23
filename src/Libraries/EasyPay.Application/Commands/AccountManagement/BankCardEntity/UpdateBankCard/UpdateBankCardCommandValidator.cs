using EasyPay.Application.Commands.AccountManagement.BankCardEntity.CreateBankCard;
using EasyPay.Infrastructure.Configurations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.UpdateBankCard
{
    public class UpdateBankCardCommandValidator : AbstractValidator<CreateBankCardCommand>
    {
        public UpdateBankCardCommandValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .MaximumLength(EntityConstraints.DefaultTitleMaxLength);
            RuleFor(p => p.InternationalBankAccountNumber)
                .Length(34);
            RuleFor(p => p.CardNumber)
                .Length(16);
            RuleFor(p => p.AccountNumber)
                .Length(10);
        }
    }
}
