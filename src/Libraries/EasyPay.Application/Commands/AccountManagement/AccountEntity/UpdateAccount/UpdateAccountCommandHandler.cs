using AutoMapper;
using EasyPay.Common.Errors;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.UpdateAccount
{
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public UpdateAccountCommandHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _accountRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError($"There is no entity with this id: {request.Id}"));

                _mapper.Map(request, entity);

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