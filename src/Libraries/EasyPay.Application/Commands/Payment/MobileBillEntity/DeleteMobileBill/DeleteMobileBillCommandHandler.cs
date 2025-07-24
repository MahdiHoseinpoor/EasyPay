using EasyPay.Domain.Entities.Payment;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.DeleteMobileBill
{
    public class DeleteMobileBillCommandHandler : IRequestHandler<DeleteMobileBillCommand, Result>
    {
        IMobileBillRepository _mobileBillRepository;
        public DeleteMobileBillCommandHandler(IMobileBillRepository mobileBillRepository)
        {
            _mobileBillRepository = mobileBillRepository;
        }
        public async Task<Result> Handle(DeleteMobileBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _mobileBillRepository.GetByIdAsync(request.Id);
                if (entity == null) return Result.Failure(new NotFoundError());
                // TODO: check user have to have primisions
                await _mobileBillRepository.DeleteAsync(entity);
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
