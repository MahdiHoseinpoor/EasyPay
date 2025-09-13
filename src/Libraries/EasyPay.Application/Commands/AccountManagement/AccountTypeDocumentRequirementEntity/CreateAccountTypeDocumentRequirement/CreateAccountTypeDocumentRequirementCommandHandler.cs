using AutoMapper;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement
{
    public class CreateAccountTypeDocumentRequirementCommandHandler : IRequestHandler<CreateAccountTypeDocumentRequirementCommand, Result<int>>
    {
        private readonly IAccountTypeDocumentRequirementRepository _repository;
        private readonly IMapper _mapper;

        public CreateAccountTypeDocumentRequirementCommandHandler(IAccountTypeDocumentRequirementRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateAccountTypeDocumentRequirementCommand request, CancellationToken cancellationToken)
        {
            var alreadyExists = await _repository.ExistsAsync(r =>
                r.AccountTypeId == request.AccountTypeId && r.AuthItemId == request.AuthItemId);

            if (alreadyExists)
            {
                return Result<int>.Failure(new DuplicateDocumentRequirementError());
            }

            var newRequirement = _mapper.Map<AccountTypeDocumentRequirement>(request);

            await _repository.AddAsync(newRequirement);
            await _repository.SaveChangesAsync();

            return Result<int>.Success(newRequirement.Id);
        }
    }
}