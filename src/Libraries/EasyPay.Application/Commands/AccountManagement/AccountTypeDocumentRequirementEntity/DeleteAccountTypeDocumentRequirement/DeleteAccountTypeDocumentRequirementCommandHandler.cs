using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.DeleteAccountTypeDocumentRequirement
{
    public class DeleteAccountTypeDocumentRequirementCommandHandler : IRequestHandler<DeleteAccountTypeDocumentRequirementCommand, Result>
    {
        private readonly IAccountTypeDocumentRequirementRepository _repository;

        public DeleteAccountTypeDocumentRequirementCommandHandler(IAccountTypeDocumentRequirementRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteAccountTypeDocumentRequirementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FirstOrDefaultAsync(r =>
                r.Id == request.RequirementId && r.AccountTypeId == request.AccountTypeId);

            if (entity == null)
            {
                return Result.Failure(new NotFoundError("The specified document requirement was not found for this account type."));
            }

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();

            return Result.Success();
        }
    }
}