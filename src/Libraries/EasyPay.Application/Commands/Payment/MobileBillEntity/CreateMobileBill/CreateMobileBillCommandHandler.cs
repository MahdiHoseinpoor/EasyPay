using AutoMapper;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Payment;
using EasyPay.Shared.Enums.Payment;
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
        private readonly ICurrentUserService _currentUserService;

        public CreateMobileBillCommandHandler(
            IMobileBillRepository mobileBillRepository,
            IMapper mapper,
            IBillInquiryService billInquiryService,
            ICurrentUserService currentUserService)
        {
            _mobileBillRepository = mobileBillRepository;
            _mapper = mapper;
            _billInquiryService = billInquiryService;
            _currentUserService = currentUserService; 
        }

        public async Task<Result<Guid>> Handle(CreateMobileBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    return Result<Guid>.Failure(new Error(401, "User is not authenticated."));
                }
                var billInquiryResult = await _billInquiryService.GetMobileBillAsync(request.PhoneNumber);
                if (!billInquiryResult.IsSuccess)
                {
                    return Result<Guid>.Failure(billInquiryResult.error);
                }

                var entity = _mapper.Map<MobileBill>(request);

                entity.Amount = billInquiryResult.Value.Amount;
                entity.Type = BillType.Mobile;
                entity.UserId = userId;
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