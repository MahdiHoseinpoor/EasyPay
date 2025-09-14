using EasyPay.Application.Commands.Identity.AuthenticateWithPhone;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.ValueObjects.Identity;
using EasyPay.Shared.Models.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyPay.Application.UnitTests.Commands.Identity
{
    public class AuthenticateWithPhoneCommandHandlerTests
    {
        // A mockable UserManager is needed because the original has protected methods and a complex constructor.
        public class MockUserManager : UserManager<ApplicationUser>
        {
            public MockUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                      null, null, null, null, null, null, null, null)
            { }
        }

        private readonly Mock<MockUserManager> _mockUserManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IVerificationCodeCacheService> _mockVerificationCodeCacheService;
        private readonly AuthenticateWithPhoneCommandHandler _handler;

        public AuthenticateWithPhoneCommandHandlerTests()
        {
            _mockUserManager = new Mock<MockUserManager>();
            _mockVerificationCodeCacheService = new Mock<IVerificationCodeCacheService>();
            _mockTokenService = new Mock<ITokenService>();

            _handler = new AuthenticateWithPhoneCommandHandler(
                _mockVerificationCodeCacheService.Object,
                _mockUserManager.Object,
                _mockTokenService.Object
            );
        }

        [Fact]
        public async Task Handle_Should_CreateNewUserAndReturnNeedsRegistration_When_PhoneNumberIsNew()
        {
            const string phoneNumber = "+989123456789";

            var validCodeInstance = new VerificationCode(phoneNumber, TimeSpan.FromMinutes(2));

            var command = new AuthenticateWithPhoneCommand { Phone = phoneNumber, Code = validCodeInstance.Code };

            _mockVerificationCodeCacheService.Setup(s => s.TryGetValue(command.Phone, out validCodeInstance)).Returns(true);

            _mockUserManager.Setup(um => um.FindByNameAsync(command.Phone)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            var tokenResult = new TokenResult("new-user-token", DateTime.UtcNow.AddHours(1));
            _mockTokenService.Setup(ts => ts.GenerateToken(It.IsAny<ApplicationUser>())).ReturnsAsync(tokenResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PhoneAuthenticationStatus.NeedsRegistration);
            result.Value.Token.Should().Be("new-user-token");
            _mockUserManager.Verify(um => um.CreateAsync(It.Is<ApplicationUser>(u => u.UserName == command.Phone)), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnNeedsPassword_When_ExistingUserTwoFactorIsEnabled()
        {
            const string phoneNumber = "+989123456789";
            var existingUser = new ApplicationUser { UserName = phoneNumber, PhoneNumber = phoneNumber, TwoFactorEnabled = true };

            var validCodeInstance = new VerificationCode(phoneNumber, TimeSpan.FromMinutes(2));
            var command = new AuthenticateWithPhoneCommand { Phone = phoneNumber, Code = validCodeInstance.Code };

            _mockVerificationCodeCacheService.Setup(s => s.TryGetValue(command.Phone, out validCodeInstance)).Returns(true);
            _mockUserManager.Setup(um => um.FindByNameAsync(command.Phone)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(um => um.HasPasswordAsync(existingUser)).ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PhoneAuthenticationStatus.NeedsPassword);
            result.Value.Token.Should().BeNull();
            _mockTokenService.Verify(ts => ts.GenerateToken(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnLoggedIn_When_ExistingUserHasNoPassword()
        {
            const string phoneNumber = "+989123456789";
            var existingUser = new ApplicationUser { UserName = phoneNumber, PhoneNumber = phoneNumber };

            var validCodeInstance = new VerificationCode(phoneNumber, TimeSpan.FromMinutes(2));
            var command = new AuthenticateWithPhoneCommand { Phone = phoneNumber, Code = validCodeInstance.Code };

            _mockVerificationCodeCacheService.Setup(s => s.TryGetValue(command.Phone, out validCodeInstance)).Returns(true);
            _mockUserManager.Setup(um => um.FindByNameAsync(command.Phone)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(um => um.HasPasswordAsync(existingUser)).ReturnsAsync(false);

            var tokenResult = new TokenResult("existing-user-token", DateTime.UtcNow.AddHours(1));
            _mockTokenService.Setup(ts => ts.GenerateToken(existingUser)).ReturnsAsync(tokenResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PhoneAuthenticationStatus.LoggedIn);
            result.Value.Token.Should().Be("existing-user-token");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_VerificationCodeIsInvalid()
        {
            var command = new AuthenticateWithPhoneCommand
            {
                Phone = "+989123456789",
                Code = "wrong-code"
            };
            VerificationCode nullCode = null;
            _mockVerificationCodeCacheService.Setup(s => s.TryGetValue(command.Phone, out nullCode)).Returns(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.message.Should().Contain("Invalid or expired verification code");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_UserCreationFails()
        {
            const string phoneNumber = "+989123456789";

            var validCodeInstance = new VerificationCode(phoneNumber, TimeSpan.FromMinutes(2));
            var command = new AuthenticateWithPhoneCommand { Phone = phoneNumber, Code = validCodeInstance.Code };

            _mockVerificationCodeCacheService.Setup(s => s.TryGetValue(command.Phone, out validCodeInstance)).Returns(true);
            _mockUserManager.Setup(um => um.FindByNameAsync(command.Phone)).ReturnsAsync((ApplicationUser)null);

            var identityError = new IdentityError { Description = "Database error" };
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(identityError));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.message.Should().Contain("User creation failed");
        }
    }
}