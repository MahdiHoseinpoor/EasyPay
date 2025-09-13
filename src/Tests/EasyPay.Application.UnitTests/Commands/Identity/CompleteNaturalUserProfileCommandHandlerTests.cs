using EasyPay.Application.Commands.Identity.NaturalUserEntity.CreateNaturalUser;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using EasyPay.Common.Errors;

namespace EasyPay.Application.UnitTests.Commands.Identity
{
    public class CompleteNaturalUserProfileCommandHandlerTests
    {
        public class MockUserManager : UserManager<ApplicationUser>
        {
            public MockUserManager() : base(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null) { }
        }

        private readonly Mock<MockUserManager> _mockUserManager;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly CompleteNaturalUserProfileCommandHandler _handler;

        public CompleteNaturalUserProfileCommandHandlerTests()
        {
            _mockUserManager = new Mock<MockUserManager>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _handler = new CompleteNaturalUserProfileCommandHandler(_mockUserManager.Object, _mockCurrentUserService.Object);
        }

        [Fact]
        public async Task Handle_Should_UpdateUserAndSetStepToProfileCompleted_When_CurrentStepIsPhoneNumberVerification()
        {
            var command = new CompleteNaturalUserProfileCommand { FirstName = "John", LastName = "Doe" };
            var user = new NaturalUser { Id = "test-user", CurrentStep = RegistrationStep.PhoneNumberVerification };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            user.CurrentStep.Should().Be(RegistrationStep.Completed);
            user.FirstName.Should().Be(command.FirstName);
            _mockUserManager.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_UserIsNotInPhoneNumberVerificationStep()
        {
            var command = new CompleteNaturalUserProfileCommand { FirstName = "John", LastName = "Doe" };
            var user = new NaturalUser { Id = "test-user", CurrentStep = RegistrationStep.Completed };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_UserNotFound()
        {
            var command = new CompleteNaturalUserProfileCommand();

            _mockCurrentUserService.Setup(s => s.UserId).Returns("test-user");
            _mockUserManager.Setup(um => um.FindByIdAsync("test-user")).ReturnsAsync((ApplicationUser)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<NotFoundError>();
        }
    }
}