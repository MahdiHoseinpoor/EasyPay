using AutoMapper;
using EasyPay.Application.Services; // Add this
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.CreateBankCard
{
    public class CreateBankCardCommandHandler : IRequestHandler<CreateBankCardCommand, Result<int>>
    {
        private readonly IBankCardRepository _bankCardRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CreateBankCardCommandHandler(IBankCardRepository bankCardRepository, IMapper mapper, ICurrentUserService currentUserService) // Update constructor
        {
            _bankCardRepository = bankCardRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<Result<int>> Handle(CreateBankCardCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<int>.Failure(new Error(401, "User is not authenticated."));
            }

            try
            {
                var entity = _mapper.Map<BankCard>(request);
                entity.OwnerUserId = userId; 
                await _bankCardRepository.AddAsync(entity);
                await _bankCardRepository.SaveChangesAsync();
                return Result<int>.Success(entity.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}