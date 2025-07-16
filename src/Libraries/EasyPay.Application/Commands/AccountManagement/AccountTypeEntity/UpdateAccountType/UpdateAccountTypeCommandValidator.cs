using EasyPay.Infrastructure.Configurations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.UpdateAccountType
{
    public class UpdateAccountTypeCommandValidator : AbstractValidator<UpdateAccountTypeCommand>
    {
        public UpdateAccountTypeCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(EntityConstraints.DefaultTitleMaxLength);

            RuleFor(x => x.Description)
                .MaximumLength(EntityConstraints.DefaultDescriptionMaxLength);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(20)
                .Matches(@"^[\u0000-\u007F]*$")
                .WithMessage("Code must be ASCII only.");
        }
    }
}
