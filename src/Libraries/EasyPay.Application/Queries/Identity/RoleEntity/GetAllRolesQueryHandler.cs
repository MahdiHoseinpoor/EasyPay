using AutoMapper;
using EasyPay.Application.DTOs.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.Identity.RoleEntity
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<List<RoleDto>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<Result<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            return Result<List<RoleDto>>.Success(roleDtos);
        }
    }
}