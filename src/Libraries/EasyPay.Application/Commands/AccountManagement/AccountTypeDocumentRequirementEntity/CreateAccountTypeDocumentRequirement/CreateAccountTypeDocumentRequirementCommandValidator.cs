using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Identity;
using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement
{
    public class CreateAccountTypeDocumentRequirementCommandValidator : AbstractValidator<CreateAccountTypeDocumentRequirementCommand>
    {
        public CreateAccountTypeDocumentRequirementCommandValidator(IAccountTypeRepository accountTypeRepository, IAuthItemRepository authItemRepository)
        {
            RuleFor(x => x.AccountTypeId)
                .NotEmpty()
                .MustAsync(async (id, cancellation) => await accountTypeRepository.ExistsAsync(a => a.Id == id))
                .WithMessage("The specified Account Type ID does not exist.");

            RuleFor(x => x.AuthItemId)
                .NotEmpty()
                .MustAsync(async (id, cancellation) => await authItemRepository.ExistsAsync(a => a.Id == id))
                .WithMessage("The specified Authentication Item ID does not exist.");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).When(x => x.Order.HasValue)
                .WithMessage("Order must be a non-negative number.");
        }
    }
}