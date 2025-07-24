using AutoMapper;
using EasyPay.Application.Commands.Payment.UtilityBillEntity.CreateUtilityBill;
using EasyPay.Domain.Entities.Payment;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.UtilityBillEntity.CreateUtilityBill
{
    public class CreateUtilityBillCommandHandler : IRequestHandler<CreateUtilityBillCommand,Result<Guid>>
    {
        IUtilityBillRepository _utilityBillRepository;
        IMapper _mapper;
        public CreateUtilityBillCommandHandler(IUtilityBillRepository utilityBillRepository, IMapper mapper)
        {
            _utilityBillRepository = utilityBillRepository;
            _mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateUtilityBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<UtilityBill>(request);
                //TODO: get bill amount with api
                await _utilityBillRepository.AddAsync(entity);
                await _utilityBillRepository.SaveChangesAsync();
                return Result<Guid>.Success(entity.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
