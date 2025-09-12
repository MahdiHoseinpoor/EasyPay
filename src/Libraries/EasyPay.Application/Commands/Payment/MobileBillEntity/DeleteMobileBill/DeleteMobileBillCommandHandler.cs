using EasyPay.Application.Services;
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
        private readonly ICurrentUserService _currentUserService; 

        public DeleteMobileBillCommandHandler(IMobileBillRepository mobileBillRepository, ICurrentUserService currentUserService)
        {
            _mobileBillRepository = mobileBillRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(DeleteMobileBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                var entity = await _mobileBillRepository.GetByIdAsync(request.Id);

                if (entity == null)
                    return Result.Failure(new NotFoundError());
                if (entity.UserId != userId)
                {
                    return Result.Failure(new Error(403, "Forbidden: You do not have permission to delete this bill."));
                }

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
