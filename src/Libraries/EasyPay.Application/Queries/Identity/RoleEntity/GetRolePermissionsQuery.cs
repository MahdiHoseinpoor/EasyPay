using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.Identity;
using MediatR;

namespace EasyPay.Application.Queries.Identity.RoleEntity
{
    public class GetRolePermissionsQuery : IRequest<Result<ManageRolePermissionsDto>>, IAuthorizableRequest<Result<ManageRolePermissionsDto>>
    {
        public string RoleId { get; set; }

        public string RequiredPermission => Permissions.Roles.View;
    }
}