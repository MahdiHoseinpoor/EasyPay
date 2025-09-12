using EasyPay.Application.Commands.Identity.LoginCommand;
using EasyPay.Application.Services;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.Identity;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace EasyPay.Application.UnitTests.Commands.Identity
{
    // A helper class to mock UserManager since its constructor is protected
    public class MockUserManager : UserManager<ApplicationUser>
    {
        public MockUserManager()
            : base(new Mock<IUserStore<ApplicationUser>>().Object,
                  null, null, null, null, null, null, null, null)
        { }
    }

    public class LoginCommandHandlerTests
    {
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<MockUserManager> _mockUserManager;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _mockUserManager = new Mock<MockUserManager>();

            // SignInManager constructor needs more mocks, which we can provide as nulls for our tests
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null
            );

            _mockPublisher = new Mock<IPublisher>();
            _mockTokenService = new Mock<ITokenService>();

            _handler = new LoginCommandHandler(
                _mockSignInManager.Object,
                _mockUserManager.Object,
                _mockPublisher.Object,
                _mockTokenService.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var command = new LoginCommand { Username = "testuser", Password = "Password123!" };
            var user = new ApplicationUser { UserName = "testuser" };
            var tokenResult = new TokenResult("fake-jwt-token", DateTime.UtcNow.AddHours(1));

            _mockUserManager.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, command.Password, true)).ReturnsAsync(SignInResult.Success);
            _mockTokenService.Setup(ts => ts.GenerateToken(user)).ReturnsAsync(tokenResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be("fake-jwt-token");
            _mockPublisher.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var command = new LoginCommand { Username = "nonexistent", Password = "Password123!" };

            _mockUserManager.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<AuthenticationError>();
            _mockPublisher.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenPasswordIsInvalid()
        {
            // Arrange
            var command = new LoginCommand { Username = "testuser", Password = "WrongPassword" };
            var user = new ApplicationUser { UserName = "testuser" };

            _mockUserManager.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, command.Password, true)).ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<AuthenticationError>();
            _mockPublisher.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}