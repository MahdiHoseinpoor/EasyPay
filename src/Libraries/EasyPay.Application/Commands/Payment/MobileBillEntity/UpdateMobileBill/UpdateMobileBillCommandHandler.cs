using AutoMapper;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.UpdateMobileBill
{
    public class UpdateMobileBillCommandHandler : IRequestHandler<UpdateMobileBillCommand, Result>
    {
        IMobileBillRepository _mobileBillRepository;
        IMapper _mapper;
        public UpdateMobileBillCommandHandler(IMobileBillRepository mobileBillRepository, IMapper mapper)
        {
            _mobileBillRepository = mobileBillRepository;
            _mapper = mapper;
        }
        public async Task<Result> Handle(UpdateMobileBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _mobileBillRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError("there is no any entity this id: " + request.Id));
                await _mobileBillRepository.UpdateAsync(entity);
                await _mobileBillRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
