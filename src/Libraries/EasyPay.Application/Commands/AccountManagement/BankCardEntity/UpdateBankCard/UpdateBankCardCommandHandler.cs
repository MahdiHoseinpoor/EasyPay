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

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.UpdateBankCard
{
    public class UpdateBankCardCommandHandler : IRequestHandler<UpdateBankCardCommand, Result>
    {
        private IBankCardRepository _bankCardRepository;
        private IMapper _mapper;
        public UpdateBankCardCommandHandler(IBankCardRepository bankCardRepository, IMapper mapper)
        {
            _bankCardRepository = bankCardRepository;
            _mapper = mapper;
        }
        public async Task<Result> Handle(UpdateBankCardCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _bankCardRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError("there is no any entity this id: " + request.Id));
               _mapper.Map(request, entity);
                await _bankCardRepository.UpdateAsync(entity);
                await _bankCardRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
