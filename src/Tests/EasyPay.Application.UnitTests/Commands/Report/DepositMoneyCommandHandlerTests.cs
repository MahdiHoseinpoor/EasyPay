using EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction;
using EasyPay.Application.Services;
using EasyPay.Common.Errors;
using EasyPay.Common.Errors.Business;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Report;
using EasyPay.Domain.Enums.AccountManagement;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using FluentAssertions;
using Moq;

namespace EasyPay.Application.UnitTests.Commands.Report
{
    public class DepositMoneyCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly DepositMoneyCommandHandler _handler;

        public DepositMoneyCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            _handler = new DepositMoneyCommandHandler(
                _mockAccountRepository.Object,
                _mockTransactionRepository.Object,
                _mockCurrentUserService.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenDepositIsValid()
        {
            // Arrange
            var userId = "user-123";
            var accountId = Guid.NewGuid();
            var command = new DepositMoneyCommand
            {
                AccountId = accountId,
                Amount = 250,
                Description = "Test deposit",
                RequestMetadata = new TransactionRequestMetadata("127.0.0.1", "test-agent")
            };

            var account = new Account
            {
                Id = accountId,
                OwnerUserId = userId,
                CurrentBalance = 500,
                Status = AccountStatus.Active
            };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            account.CurrentBalance.Should().Be(750); // 500 + 250
            _mockTransactionRepository.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
            _mockAccountRepository.Verify(r => r.UpdateAsync(account), Times.Once);
            _mockTransactionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockTransactionRepository.Verify(r => r.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnNotFoundError_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new DepositMoneyCommand { AccountId = accountId, Amount = 100 };

            _mockCurrentUserService.Setup(s => s.UserId).Returns("user-123");
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Account)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<NotFoundError>();
        }

        [Fact]
        public async Task Handle_Should_ReturnForbiddenError_WhenUserDoesNotOwnAccount()
        {
            // Arrange
            var ownerUserId = "owner-user";
            var requesterUserId = "requester-user";
            var accountId = Guid.NewGuid();
            var command = new DepositMoneyCommand { AccountId = accountId, Amount = 100 };

            var account = new Account { Id = accountId, OwnerUserId = ownerUserId, Status = AccountStatus.Active };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(requesterUserId);
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<AuthorizationError>();
        }

        [Fact]
        public async Task Handle_Should_ReturnBadRequestError_WhenAccountIsNotActive()
        {
            // Arrange
            var userId = "user-123";
            var accountId = Guid.NewGuid();
            var command = new DepositMoneyCommand { AccountId = accountId, Amount = 100 };

            var account = new Account { Id = accountId, OwnerUserId = userId, Status = AccountStatus.Frozen };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<AccountInactiveError>();
        }
    }
}