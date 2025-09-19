using EasyPay.Application.Commands.Identity.SetPasswordCommand;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.UnitTests.Commands.Identity
{
   
    public class SetPasswordCommandHandlerTests
    {
        public class MockUserManager : UserManager<ApplicationUser>
        {
            public MockUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                      null, null, null, null, null, null, null, null)
            { }
        }
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<MockUserManager> _mockUserManager;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly SetPasswordCommandHandler _handler;

        public SetPasswordCommandHandlerTests()
        {
            _mockUserManager = new Mock<MockUserManager>();

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null
            );

            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new SetPasswordCommandHandler(
                _mockSignInManager.Object,
                _mockUserManager.Object,
                _mockCurrentUserService.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenSetPasswordForFirstTime()
        {
            var command = new SetPasswordCommand("test-new-password");
            var user = new ApplicationUser { Id = "test-user-id", UserName = "test-user" };
            _mockCurrentUserService.Setup(cs => cs.UserId).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.HasPasswordAsync(user)).ReturnsAsync(false);
            _mockUserManager.Setup(um => um.AddPasswordAsync(user, command.NewPassword)).ReturnsAsync(IdentityResult.Success);

            var result =await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().Be(true);
            _mockUserManager.Verify(p => p.AddPasswordAsync(user, command.NewPassword), Times.Once);
            _mockUserManager.Verify(p => p.ChangePasswordAsync(user, command.CurrentPassword,command.NewPassword), Times.Never);
        }
    }
}
