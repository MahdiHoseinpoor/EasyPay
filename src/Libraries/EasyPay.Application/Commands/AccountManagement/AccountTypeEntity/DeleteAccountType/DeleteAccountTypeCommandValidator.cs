using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.DeleteAccountType
{
    public class DeleteAccountTypeCommandValidator : AbstractValidator<DeleteAccountTypeCommand>
    {
        public DeleteAccountTypeCommandValidator()
        {
                
        }
    }
}
