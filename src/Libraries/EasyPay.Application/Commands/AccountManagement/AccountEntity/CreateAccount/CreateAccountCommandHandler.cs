using AutoMapper;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount;
using EasyPay.Common;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyPay.Infrastructure.Aggregates.Identity;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Common.Errors;
namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<Guid>>
    {
        private IAccountRepository _accountRepository;
        private IAccountTypeDocumentRequirementRepository _accountTypeDocumentRequirementRepository;
        private IAuthItemValueRepository _authItemValueRepository;
        private IMapper _mapper;
        public CreateAccountCommandHandler(IAccountRepository accountRepository, IAccountTypeDocumentRequirementRepository accountTypeDocumentRequirementRepository, IAuthItemValueRepository authItemValueRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _accountTypeDocumentRequirementRepository = accountTypeDocumentRequirementRepository;
            _authItemValueRepository = authItemValueRepository;
            _mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string UserId = "";
                var requirments = _accountTypeDocumentRequirementRepository.Query(p => p.AccountTypeId == request.AccountTypeId).ToList();
                var authItemValues = requirments.Join(_authItemValueRepository.Query(p=>p.UserId == UserId),p=>p.AuthItemId,p=>p.AuthItemId,(a,b)=>b).ToList();
                List<AuthItem> UserRequirementAuthItems = new List<AuthItem>();
                foreach (var requirment in requirments)
                {
                   if(!authItemValues.Any(p=>p.AuthItemId == requirment.AuthItemId && p.Status == Domain.Enums.Identity.VerificationStatus.Approved))
                   {
                        //TODO: change it to get Authitem from its repository
                        UserRequirementAuthItems.Add(requirment.AuthItem);
                   } 
                }
                if (UserRequirementAuthItems.Count() != 0)
                {
                    return Result<Guid>.Failure(new Error(100,"User not have All AccountTypeRequierments"));
                }
                else
                {
                    var account = _mapper.Map<Account>(request);
                    await _accountRepository.AddAsync(account);
                    await _accountRepository.SaveChangesAsync();
                    return Result<Guid>.Success(account.Id);
                }

            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
