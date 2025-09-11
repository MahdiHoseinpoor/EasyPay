using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.DeleteAccountTypeDocumentRequirement
{
    public class DeleteAccountTypeDocumentRequirementCommandValidator : AbstractValidator<DeleteAccountTypeDocumentRequirementCommand>
    {
        public DeleteAccountTypeDocumentRequirementCommandValidator()
        {
            RuleFor(x => x.AccountTypeId).NotEmpty();
            RuleFor(x => x.RequirementId).NotEmpty();
        }
    }
}