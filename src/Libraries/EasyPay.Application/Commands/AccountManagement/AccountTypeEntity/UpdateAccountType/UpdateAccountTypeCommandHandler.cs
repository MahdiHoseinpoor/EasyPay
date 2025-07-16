using AutoMapper;
using EasyPay.Common;
using EasyPay.Common.Errors;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.UpdateAccountType
{
    public class UpdateAccountTypeCommandHandler : IRequestHandler<UpdateAccountTypeCommand, Result>
    {
        private IAccountTypeRepository _accountTypeRepository;
        private IMapper _mapper;
        public UpdateAccountTypeCommandHandler(IAccountTypeRepository accountTypeRepository, IMapper mapper)
        {
            _accountTypeRepository = accountTypeRepository;
            _mapper = mapper;
        }

        async Task<Result> IRequestHandler<UpdateAccountTypeCommand, Result>.Handle(UpdateAccountTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _accountTypeRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError("there is no any entity this id: " + request.Id));
                await _accountTypeRepository.UpdateAsync(entity);
                return Result.Success();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
