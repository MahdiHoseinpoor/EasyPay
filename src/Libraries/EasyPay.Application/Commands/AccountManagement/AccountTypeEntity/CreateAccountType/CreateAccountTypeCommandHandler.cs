using AutoMapper;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.CreateAccountType
{
    public class CreateAccountTypeCommandHandler : IRequestHandler<CreateAccountTypeCommand, int>
    {
        private IAccountTypeRepository _accountTypeRepository;
        private IMapper _mapper;
        public CreateAccountTypeCommandHandler(IAccountTypeRepository accountTypeRepository, IMapper mapper)
        {
            _accountTypeRepository = accountTypeRepository;
            _mapper = mapper;
        }
        public async Task<int> Handle(CreateAccountTypeCommand request, CancellationToken cancellationToken)
        {
            var accountType = _mapper.Map<AccountType>(request);
            await _accountTypeRepository.AddAsync(accountType);
            return accountType.Id;
        }
    }
}
