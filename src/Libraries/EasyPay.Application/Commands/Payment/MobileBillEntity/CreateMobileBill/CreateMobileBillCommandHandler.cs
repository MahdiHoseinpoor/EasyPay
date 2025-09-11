using AutoMapper;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Payment;
using EasyPay.Domain.Enums.Payment;
using EasyPay.Infrastructure.Aggregates.Payment;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.CreateMobileBill
{
    public class CreateMobileBillCommandHandler : IRequestHandler<CreateMobileBillCommand, Result<Guid>>
    {
        private readonly IMobileBillRepository _mobileBillRepository;
        private readonly IMapper _mapper;
        private readonly IBillInquiryService _billInquiryService; 

        public CreateMobileBillCommandHandler(
            IMobileBillRepository mobileBillRepository,
            IMapper mapper,
            IBillInquiryService billInquiryService)
        {
            _mobileBillRepository = mobileBillRepository;
            _mapper = mapper;
            _billInquiryService = billInquiryService;
        }

        public async Task<Result<Guid>> Handle(CreateMobileBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var billInquiryResult = await _billInquiryService.GetMobileBillAsync(request.PhoneNumber);
                if (!billInquiryResult.IsSuccess)
                {
                    return Result<Guid>.Failure(billInquiryResult.error);
                }

                var entity = _mapper.Map<MobileBill>(request);

                entity.Amount = billInquiryResult.Value.Amount;
                entity.Type = BillType.Mobile; 

                await _mobileBillRepository.AddAsync(entity);
                await _mobileBillRepository.SaveChangesAsync();

                return Result<Guid>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(new Error(500, "An unexpected error occurred while creating the mobile bill."));
            }
        }
    }
}