using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.RoleEntity.UpdateRolePermissions
{
    public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, Result>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public UpdateRolePermissionsCommandHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                return Result.Failure(new NotFoundError("Role not found."));
            }

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var permissionClaims = currentClaims.Where(c => c.Type == "Permission").ToList();
            foreach (var claim in permissionClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permissionName in request.SelectedPermissions.Distinct())
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permissionName));
            }

            return Result.Success();
        }
    }
}