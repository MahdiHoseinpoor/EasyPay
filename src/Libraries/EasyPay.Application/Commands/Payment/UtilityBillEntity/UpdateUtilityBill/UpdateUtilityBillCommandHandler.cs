using AutoMapper;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.UpdateUtilityBill
{
    public class UpdateUtilityBillCommandHandler : IRequestHandler<UpdateUtilityBillCommand, Result>
    {
        IUtilityBillRepository _utilityBillRepository;
        IMapper _mapper;

        public UpdateUtilityBillCommandHandler(IUtilityBillRepository utilityBillRepository,IMapper mapper)
        {
            _utilityBillRepository = utilityBillRepository;
            _mapper = mapper;
        }
        public async Task<Result> Handle(UpdateUtilityBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _utilityBillRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError("there is no any entity this id: " + request.Id));
                await _utilityBillRepository.UpdateAsync(entity);
                await _utilityBillRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
