using AutoMapper;
using EasyPay.Common;
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
    public class CreateAccountTypeCommandHandler : IRequestHandler<CreateAccountTypeCommand, Result<int>>
    {
        private IAccountTypeRepository _accountTypeRepository;
        private IMapper _mapper;
        public CreateAccountTypeCommandHandler(IAccountTypeRepository accountTypeRepository, IMapper mapper)
        {
            _accountTypeRepository = accountTypeRepository;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateAccountTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountType = _mapper.Map<AccountType>(request);
                await _accountTypeRepository.AddAsync(accountType);
                return Result.Success(accountType.Id);
            }
            catch (Exception e)
            { 
                throw;
            }

        }
    }
}
