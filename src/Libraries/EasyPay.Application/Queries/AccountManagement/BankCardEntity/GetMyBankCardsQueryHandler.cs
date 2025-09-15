using AutoMapper;
using EasyPay.Application.Queries.Identity.AuthItemValueEntity;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Shared.DTOs.AccountManagement;
using EasyPay.Shared.DTOs.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.AccountManagement.BankCardEntity
{
    public class GetMyBankCardsQueryHandler : IRequestHandler<GetMyBankCardsQuery, Result<List<BankCardDto>>>
    {
        private readonly IBankCardRepository _bankCardRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetMyBankCardsQueryHandler(IBankCardRepository bankCardRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _bankCardRepository = bankCardRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<BankCardDto>>> Handle(GetMyBankCardsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Result<List<BankCardDto>>.Failure(new Error(401, "User is not authenticated."));
            }

            var bankCards = await _bankCardRepository
                .Query(v => v.OwnerUserId == userId)
                .ToListAsync(cancellationToken);

            var bankCardDtos = _mapper.Map<List<BankCardDto>>(bankCards);
            return Result<List<BankCardDto>>.Success(bankCardDtos);
        }
    }
}
