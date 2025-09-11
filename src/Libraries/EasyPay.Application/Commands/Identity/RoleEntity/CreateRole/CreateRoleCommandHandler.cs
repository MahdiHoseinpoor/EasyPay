using AutoMapper;
using EasyPay.Application.DTOs.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.RoleEntity.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(request.RoleName))
            {
                return Result<RoleDto>.Failure(new Error(409, "A role with this name already exists."));
            }

            var newRole = new IdentityRole(request.RoleName);
            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                return Result<RoleDto>.Failure(new Error(500, "Failed to create role."));
            }

            var roleDto = _mapper.Map<RoleDto>(newRole);
            return Result<RoleDto>.Success(roleDto);
        }
    }
}