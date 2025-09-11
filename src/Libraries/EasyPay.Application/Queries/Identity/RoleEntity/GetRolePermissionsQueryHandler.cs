using EasyPay.Application.Common;
using EasyPay.Application.DTOs.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.Identity.RoleEntity
{
    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, Result<ManageRolePermissionsDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRolePermissionsQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result<ManageRolePermissionsDto>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                return Result<ManageRolePermissionsDto>.Failure(new NotFoundError("Role not found."));
            }

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var rolePermissions = roleClaims.Where(c => c.Type == "Permission").Select(c => c.Value).ToHashSet();

            var allPermissions = GetAllPermissions();
            var permissionViewModels = allPermissions.Select(p => new PermissionViewModel
            {
                Name = p,
                Description = $"Allows user to {p.Split('.').Last()}",
                IsEnabled = rolePermissions.Contains(p)
            }).ToList();

            var model = new ManageRolePermissionsDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Permissions = permissionViewModels
            };

            return Result<ManageRolePermissionsDto>.Success(model);
        }

        private List<string> GetAllPermissions()
        {
            var permissions = new List<string>();
            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public);
            foreach (var type in nestedTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                permissions.AddRange(fields.Select(fi => (string)fi.GetValue(null)));
            }
            return permissions;
        }
    }
}