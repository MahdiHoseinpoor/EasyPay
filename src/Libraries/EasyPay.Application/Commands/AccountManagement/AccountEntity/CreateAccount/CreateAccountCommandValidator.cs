using EasyPay.Infrastructure.Aggregates.AccountManagement;
using FluentValidation;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        private readonly IAccountTypeRepository _accountTypeRepository;

        public CreateAccountCommandValidator(IAccountTypeRepository accountTypeRepository)
        {
            _accountTypeRepository = accountTypeRepository;

            RuleFor(x => x.AccountTypeId)
                .NotEmpty().WithMessage("Account Type ID is required.")
                .MustAsync(AccountTypeMustExist).WithMessage("The specified Account Type ID does not exist.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Account title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        }
      
        private async Task<bool> AccountTypeMustExist(int id, CancellationToken cancellationToken)
        { 
            return await _accountTypeRepository.ExistsAsync(a => a.Id == id);
        }
    }
}