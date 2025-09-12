using FluentValidation;
using System;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.DeleteMobileBill
{
    public class DeleteMobileBillCommandValidator : AbstractValidator<DeleteMobileBillCommand>
    {
        public DeleteMobileBillCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Mobile Bill ID is required.");
        }
    }
}