using AutoMapper;
using EasyPay.Api.Services;
using EasyPay.Application.Services;
using EasyPay.Common;
using EasyPay.Common.Errors;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
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
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CreateAccountCommandHandler(
            IAccountRepository accountRepository,
            IAccountTypeDocumentRequirementRepository accountTypeDocumentRequirementRepository,
            IAuthItemValueRepository authItemValueRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _accountTypeDocumentRequirementRepository = accountTypeDocumentRequirementRepository;
            _authItemValueRepository = authItemValueRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                var requirments = _accountTypeDocumentRequirementRepository.Query(p => p.AccountTypeId == request.AccountTypeId).ToList();
                var authItemValues = requirments.Join(_authItemValueRepository.Query(p => p.UserId == userId), p => p.AuthItemId, p => p.AuthItemId, (a, b) => b).ToList();

                var UserRequirementAuthItems = new List<AuthItem>();
                foreach (var requirment in requirments)
                {
                    if (!authItemValues.Any(p => p.AuthItemId == requirment.AuthItemId && p.Status == Domain.Enums.Identity.VerificationStatus.Approved))
                    {
   
                        UserRequirementAuthItems.Add(requirment.AuthItem);
                    }
                }

                if (UserRequirementAuthItems.Any())
                {
                    return Result<Guid>.Failure(new Error(100, "User not have All AccountTypeRequierments"));
                }

                var account = _mapper.Map<Account>(request);
                account.OwnerUserId = userId;

                await _accountRepository.AddAsync(account);
                await _accountRepository.SaveChangesAsync();

                return Result<Guid>.Success(account.Id);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}