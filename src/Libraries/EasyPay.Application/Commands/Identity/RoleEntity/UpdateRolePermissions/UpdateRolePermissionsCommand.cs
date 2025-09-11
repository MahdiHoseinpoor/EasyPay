using EasyPay.Application.Common;
using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Identity.RoleEntity.UpdateRolePermissions
{
    public class UpdateRolePermissionsCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        [JsonIgnore]
        public string RoleId { get; set; }
        public List<string> SelectedPermissions { get; set; }

        public string RequiredPermission => Permissions.Roles.ManagePermissions;
    }
}