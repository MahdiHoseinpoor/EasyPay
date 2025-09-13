using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.SetPasswordCommand
{
    public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, Result>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public SetPasswordCommandHandler(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            IdentityResult result;
            if (await _userManager.HasPasswordAsync(currentUser))
            {
                result = await _userManager.ChangePasswordAsync(currentUser, request.CurrentPassword, request.NewPassword);
            }
            else
            {
                result = await _userManager.AddPasswordAsync(currentUser, request.NewPassword);
            }
            if (result.Succeeded)
                return Result.Success();
            else
            {
                return Result.Failure(new Error(400,"there's an error"));
            }
        }
    }
}
