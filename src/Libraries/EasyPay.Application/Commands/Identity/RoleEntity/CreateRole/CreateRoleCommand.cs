using EasyPay.Application.Common;
using EasyPay.Application.DTOs.Identity;
using MediatR;

namespace EasyPay.Application.Commands.Identity.RoleEntity.CreateRole
{
    public class CreateRoleCommand : IRequest<Result<RoleDto>>, IAuthorizableRequest<Result<RoleDto>>
    {
        public string RoleName { get; set; }

        public string RequiredPermission => Permissions.Roles.Create;
    }
}