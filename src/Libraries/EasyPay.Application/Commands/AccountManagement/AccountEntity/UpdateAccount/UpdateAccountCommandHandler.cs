using AutoMapper;
using EasyPay.Common.Errors;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount
{
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result>
    {
        private IAccountRepository _accountRepository;
        private IMapper _mapper;
        public UpdateAccountCommandHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }
        public async Task<Result> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            // Account's does'nt need update
            try
            {
                var entity = await _accountRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError("there is no any entity this id: " + request.Id));
                entity = _mapper.Map<Account>(request);
                await _accountRepository.UpdateAsync(entity);
                await _accountRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
