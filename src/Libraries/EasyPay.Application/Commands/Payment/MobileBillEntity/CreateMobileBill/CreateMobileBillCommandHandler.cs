using AutoMapper;
using EasyPay.Domain.Entities.Payment;
using EasyPay.Domain.Enums.Payment;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.CreateMobileBill
{
    public class CreateMobileBillCommandHandler : IRequestHandler<CreateMobileBillCommand, Result<Guid>>
    {
        IMobileBillRepository _mobileBillRepository;
        IMapper _mapper;
        public CreateMobileBillCommandHandler(IMobileBillRepository mobileBillRepository, IMapper mapper)
        {
            _mobileBillRepository = mobileBillRepository;
            _mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateMobileBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<MobileBill>(request);
                //TODO: get bill amount with api
                await _mobileBillRepository.AddAsync(entity);
                await _mobileBillRepository.SaveChangesAsync();
                return Result<Guid>.Success(entity.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
