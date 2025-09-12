using Moq;
using FluentAssertions;
using EasyPay.Application.Services;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Enums.AccountManagement;
using EasyPay.Domain.Entities.Report;
using EasyPay.Common.Errors;

namespace EasyPay.Application.UnitTests.Commands.Report
{
    public class WithdrawMoneyCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly WithdrawMoneyCommandHandler _handler;

        // This is the setup method that runs before each test
        public WithdrawMoneyCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();

            // Instantiate the handler we are testing, injecting our mocks
            _handler = new WithdrawMoneyCommandHandler(
                _mockAccountRepository.Object,
                _mockTransactionRepository.Object,
                _mockCurrentUserService.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenWithdrawalIsValid()
        {
            // Arrange
            var userId = "user-123";
            var accountId = Guid.NewGuid();
            var command = new WithdrawMoneyCommand
            {
                AccountId = accountId,
                Amount = 100,
                Description = "Test withdrawal",
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
            account.CurrentBalance.Should().Be(400); // 500 - 100
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
            var command = new WithdrawMoneyCommand { AccountId = accountId, Amount = 100 };

            _mockCurrentUserService.Setup(s => s.UserId).Returns("user-123");
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Account)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<NotFoundError>();
            result.error.message.Should().Be("Account not found.");
        }

        [Fact]
        public async Task Handle_Should_ReturnForbiddenError_WhenUserDoesNotOwnAccount()
        {
            // Arrange
            var ownerUserId = "owner-user";
            var requesterUserId = "requester-user"; // A different user
            var accountId = Guid.NewGuid();
            var command = new WithdrawMoneyCommand { AccountId = accountId, Amount = 100 };

            var account = new Account { Id = accountId, OwnerUserId = ownerUserId };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(requesterUserId);
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.code.Should().Be(403);
            result.error.message.Should().Be("Forbidden: You do not have access to this account.");
        }

        [Fact]
        public async Task Handle_Should_ReturnBadRequestError_WhenAccountIsNotActive()
        {
            // Arrange
            var userId = "user-123";
            var accountId = Guid.NewGuid();
            var command = new WithdrawMoneyCommand { AccountId = accountId, Amount = 100 };

            var account = new Account
            {
                Id = accountId,
                OwnerUserId = userId,
                Status = AccountStatus.Frozen // Inactive status
            };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.code.Should().Be(400);
            result.error.message.Should().Be("Account is not active.");
        }

        [Fact]
        public async Task Handle_Should_ReturnBadRequestError_WhenInsufficientFunds()
        {
            // Arrange
            var userId = "user-123";
            var accountId = Guid.NewGuid();
            var command = new WithdrawMoneyCommand { AccountId = accountId, Amount = 1000 }; // Trying to withdraw 1000

            var account = new Account
            {
                Id = accountId,
                OwnerUserId = userId,
                CurrentBalance = 500, // But only has 500
                Status = AccountStatus.Active
            };

            _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.error.code.Should().Be(400);
            result.error.message.Should().Be("Insufficient funds for this withdrawal.");
        }
    }
}