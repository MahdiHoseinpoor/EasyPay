using EasyPay.Application.Common;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IAuthorizableRequest<TResponse>
        where TResponse : Result
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthorizationBehavior(ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return CreateFailureResponse(new Error(401, "User is not authenticated."));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return CreateFailureResponse(new Error(401, "Authenticated user could not be found."));
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
            {
                return await next();
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var hasPermission = userClaims.Any(c => c.Type == "Permission" && c.Value == request.RequiredPermission);

            if (hasPermission)
            {
                return await next();
            }

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                if (roleClaims.Any(c => c.Type == "Permission" && c.Value == request.RequiredPermission))
                {
                    return await next();
                }
            }
            return CreateFailureResponse(new Error(403, $"User does not have the required permission: {request.RequiredPermission}"));
        }

        private static TResponse CreateFailureResponse(Error error)
        {
            var responseType = typeof(TResponse);
            var failureMethod = responseType.GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                new[] { typeof(Error) });

            if (failureMethod == null)
            {
                throw new InvalidOperationException($"Could not find a static 'Failure' method on type {responseType.Name} that accepts an Error object.");
            }

            return (TResponse)failureMethod.Invoke(null, new object[] { error });
        }
    }
}