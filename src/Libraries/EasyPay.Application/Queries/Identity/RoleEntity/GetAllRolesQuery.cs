using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.Identity;
using MediatR;
using System.Collections.Generic;

namespace EasyPay.Application.Queries.Identity.RoleEntity
{
    public class GetAllRolesQuery : IRequest<Result<List<RoleDto>>>, IAuthorizableRequest<Result<List<RoleDto>>>
    {
        public string RequiredPermission => Permissions.Roles.View;
    }
}