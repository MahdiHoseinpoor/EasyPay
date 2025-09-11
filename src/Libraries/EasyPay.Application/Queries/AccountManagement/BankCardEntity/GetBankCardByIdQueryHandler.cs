using AutoMapper;
using EasyPay.Application.DTOs.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.BankCardEntity
{
    public class GetBankCardByIdQueryHandler : IRequestHandler<GetBankCardByIdQuery, Result<BankCardDto>>
    {
        private readonly IBankCardRepository _bankCardRepository;
        private readonly IMapper _mapper;

        public GetBankCardByIdQueryHandler(IBankCardRepository bankCardRepository, IMapper mapper)
        {
            _bankCardRepository = bankCardRepository;
            _mapper = mapper;
        }

        public async Task<Result<BankCardDto>> Handle(GetBankCardByIdQuery request, CancellationToken cancellationToken)
        {
            var bankCard = await _bankCardRepository.GetByIdAsync(request.Id);

            if (bankCard == null)
            {
                return Result<BankCardDto>.Failure(new NotFoundError($"BankCard with ID {request.Id} not found."));
            }

            var bankCardDto = _mapper.Map<BankCardDto>(bankCard);

            return Result<BankCardDto>.Success(bankCardDto);
        }
    }
}