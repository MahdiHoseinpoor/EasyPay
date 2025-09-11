using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace EasyPay.Application.Commands.Identity.NaturalUserEntity.CreateNaturalUser
{
    public class CompleteNaturalUserProfileCommandValidator : AbstractValidator<CompleteNaturalUserProfileCommand>
    {
        public CompleteNaturalUserProfileCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage("Father's name is required.")
                .MaximumLength(50);

            RuleFor(x => x.NationalCode)
                .NotEmpty()
                .Length(10).WithMessage("National code must be 10 digits.")
                .Matches("^[0-9]*$").WithMessage("National code must only contain digits.");

            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("A valid gender must be specified.");
        }
    }
}