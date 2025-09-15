using AutoMapper;
using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.Identity;
using EasyPay.Shared.DTOs.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.Identity.AuthItemEntity
{
    public class GetAllAuthItemsQueryHandler(IAuthItemRepository authItemRepository, IMapper mapper) : IRequestHandler<GetAllAuthItemsQuery, Result<List<AuthItemDto>>>
    {
        private IAuthItemRepository _authItemRepository = authItemRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<List<AuthItemDto>>> Handle(GetAllAuthItemsQuery request, CancellationToken cancellationToken)
        {
            var authItems =await _authItemRepository.Query().ToListAsync(cancellationToken);
            if (authItems == null || !authItems.Any())
            {
                return Result<List<AuthItemDto>>.Success(new List<AuthItemDto>());
            }
            var authItemDtos = _mapper.Map<List<AuthItemDto>>(authItems);
            return Result<List<AuthItemDto>>.Success(authItemDtos);
        }
    }
}
