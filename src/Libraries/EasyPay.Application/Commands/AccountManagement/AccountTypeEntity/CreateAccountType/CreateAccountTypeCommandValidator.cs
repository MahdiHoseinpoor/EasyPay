using EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType;
using EasyPay.Infrastructure.Configurations;
using FluentValidation;

public class CreateAccountTypeCommandValidator : AbstractValidator<CreateAccountTypeCommand>
{
    public CreateAccountTypeCommandValidator()
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
