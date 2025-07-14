using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType
{
    public class CreateAccountTypeCommandValidator:AbstractValidator<CreateAccountTypeCommand>
    {
        public CreateAccountTypeCommandValidator()
        {
        }
    }
}
