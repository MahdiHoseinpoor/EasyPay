using AutoMapper;
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
        private IBankCardRepository _bankCardRepository;
        private IMapper _mapper;
        public CreateBankCardCommandHandler(IBankCardRepository bankCardRepository, IMapper mapper)
        {
            _bankCardRepository = bankCardRepository;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateBankCardCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<BankCard>(request);
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
