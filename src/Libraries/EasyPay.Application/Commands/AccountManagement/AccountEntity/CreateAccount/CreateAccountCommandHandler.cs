using AutoMapper;
using EasyPay.Api.Services;
using EasyPay.Application.Services;
using EasyPay.Common;
using EasyPay.Common.Errors;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Shared.Enums.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<Guid>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountTypeDocumentRequirementRepository _accountTypeDocumentRequirementRepository;
        private readonly IAuthItemValueRepository _authItemValueRepository;
        private readonly IAuthItemRepository _authItemRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountNumberService _accountNumberService;

        public CreateAccountCommandHandler(
            IAccountRepository accountRepository,
            IAccountTypeDocumentRequirementRepository accountTypeDocumentRequirementRepository,
            IAuthItemValueRepository authItemValueRepository,
            IAuthItemRepository authItemRepository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IAccountNumberService accountNumberService)
        {
            _accountRepository = accountRepository;
            _accountTypeDocumentRequirementRepository = accountTypeDocumentRequirementRepository;
            _authItemValueRepository = authItemValueRepository;
            _authItemRepository = authItemRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _accountNumberService = accountNumberService;
        }

        public async Task<Result<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                var requirements = _accountTypeDocumentRequirementRepository
                    .Query(p => p.AccountTypeId == request.AccountTypeId)
                    .ToList();

                if (!requirements.Any())
                {
                    return await CreateAccount(request, userId);
                }

                var authItemValues = _authItemValueRepository
                    .Query(p => p.UserId == userId && p.IsLatestVersion)
                    .ToList();

                var missingRequirements = new List<string>();

                foreach (var requirement in requirements)
                {
                    if (!authItemValues.Any(p => p.AuthItemId == requirement.AuthItemId && p.Status == Domain.Enums.Identity.VerificationStatus.Approved))
                    {
                        var authItem = await _authItemRepository.GetByIdAsync(requirement.AuthItemId);
                        if (authItem != null)
                        {
                            missingRequirements.Add(authItem.Title);
                        }
                    }
                }

                if (missingRequirements.Any())
                {
                    return Result<Guid>.Failure(new MissingRequirementsError(missingRequirements));
                }

                return await CreateAccount(request, userId);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        private async Task<Result<Guid>> CreateAccount(CreateAccountCommand request, string userId)
        {
            var account = _mapper.Map<Account>(request);
            account.OwnerUserId = userId;
            account.AccountNumber = await _accountNumberService.GenerateUniqueAccountNumberAsync();
            account.Status = AccountStatus.Active;
            account.OpeningDate = DateTime.UtcNow;

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);
        }
    }
}