using EasyPay.Application.Commands.Payment.UtilityBillEntity.DeleteUtilityBill;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.DeleteUtilityBill
{
    public class DeleteUtilityBillCommandHandler : IRequestHandler<DeleteUtilityBillCommand,Result>
    {
        IUtilityBillRepository _utilityBillRepository;
        public DeleteUtilityBillCommandHandler(IUtilityBillRepository utilityBillRepository)
        {
            _utilityBillRepository = utilityBillRepository;
        }
        public async Task<Result> Handle(DeleteUtilityBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _utilityBillRepository.GetByIdAsync(request.Id);
                if (entity == null) return Result.Failure(new NotFoundError());
                // TODO: check user have to have primisions
                await _utilityBillRepository.DeleteAsync(entity);
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
