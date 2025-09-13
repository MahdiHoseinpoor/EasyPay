using EasyPay.Application.Commands.Report.TransactionEntity.CreateTransaction;
using EasyPay.Application.Services;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Report;
using EasyPay.Domain.Enums.AccountManagement;
using EasyPay.Domain.Enums.Report;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Report;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyPay.Common.Errors.Business;
using EasyPay.Common.Errors;

namespace EasyPay.Application.UnitTests.Commands.Report
{
    public class TransferMoneyCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly Mock<ITransactionRepository> _mockTransactionRepo;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly TransferMoneyCommandHandler _handler;

        public TransferMoneyCommandHandlerTests()
        {
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();

            _handler = new TransferMoneyCommandHandler(
                _mockAccountRepo.Object,
                _mockTransactionRepo.Object,
                _mockCurrentUser.Object
            );
        }

        private TransferMoneyCommand CreateValidCommand()
        {
            return new TransferMoneyCommand
            {
                SourceAccountId = Guid.NewGuid(),
                DestinationAccountNumber = "1122334455",
                Amount = 100,
                Description = "Test Transfer",
                RequestMetadata = new TransactionRequestMetadata("127.0.0.1", "Test Agent")
            };
        }

        [Fact]
        public async Task Handle_Should_Succeed_When_AllConditionsAreMet()
        {
            var command = CreateValidCommand();
            var userId = "source-user-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = userId, CurrentBalance = 500, Status = AccountStatus.Active };
            var destAccount = new Account { Id = Guid.NewGuid(), AccountNumber = command.DestinationAccountNumber, CurrentBalance = 200, Status = AccountStatus.Active };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber)).ReturnsAsync(destAccount);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            sourceAccount.CurrentBalance.Should().Be(400);
            destAccount.CurrentBalance.Should().Be(300);

            _mockAccountRepo.Verify(r => r.UpdateAsync(sourceAccount), Times.Once);
            _mockAccountRepo.Verify(r => r.UpdateAsync(destAccount), Times.Once);

            _mockTransactionRepo.Verify(r => r.AddAsync(It.Is<Transaction>(t => t.TransactionType == TransactionType.TransferOut)), Times.Once);
            _mockTransactionRepo.Verify(r => r.AddAsync(It.Is<Transaction>(t => t.TransactionType == TransactionType.TransferIn)), Times.Once);

            _mockTransactionRepo.Verify(r => r.BeginTransactionAsync(), Times.Once);
            _mockTransactionRepo.Verify(r => r.CommitTransactionAsync(), Times.Once);
            _mockTransactionRepo.Verify(r => r.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnInsufficientFundsError_When_SourceBalanceIsTooLow()
        {
            var command = CreateValidCommand();
            var userId = "source-user-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = userId, CurrentBalance = 50, Status = AccountStatus.Active };
            var destAccount = new Account { AccountNumber = command.DestinationAccountNumber, Status = AccountStatus.Active };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber)).ReturnsAsync(destAccount);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<InsufficientFundsError>();
        }

        [Fact]
        public async Task Handle_Should_ReturnAuthorizationError_When_UserDoesNotOwnSourceAccount()
        {
            var command = CreateValidCommand();
            var userId = "current-user-id";
            var ownerId = "actual-owner-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = ownerId };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Account, bool>>>())).ReturnsAsync(new Account());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<AuthorizationError>();
        }

        [Fact]
        public async Task Handle_Should_ReturnAccountInactiveError_When_SourceAccountIsNotActive()
        {
            var command = CreateValidCommand();
            var userId = "source-user-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = userId, Status = AccountStatus.Frozen };
            var destAccount = new Account { AccountNumber = command.DestinationAccountNumber, Status = AccountStatus.Active };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber)).ReturnsAsync(destAccount);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<AccountInactiveError>();
            result.error.message.Should().Contain("Source account");
        }

        [Fact]
        public async Task Handle_Should_ReturnBusinessRuleError_When_DestinationAccountIsNotActive()
        {
            var command = CreateValidCommand();
            var userId = "source-user-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = userId, CurrentBalance = 500, Status = AccountStatus.Active };
            var destAccount = new Account { AccountNumber = command.DestinationAccountNumber, Status = AccountStatus.Closed };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber)).ReturnsAsync(destAccount);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<BusinessRuleError>();
            result.error.message.Should().Contain("cannot receive funds");
        }

        [Fact]
        public async Task Handle_Should_ReturnNotFoundError_When_DestinationAccountDoesNotExist()
        {
            var command = CreateValidCommand();
            var userId = "source-user-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = userId, CurrentBalance = 500, Status = AccountStatus.Active };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber)).ReturnsAsync((Account)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.Should().BeOfType<NotFoundError>();
            result.error.message.Should().Contain("Destination account");
        }

        [Fact]
        public async Task Handle_Should_RollbackTransaction_When_AnExceptionOccurs()
        {
            var command = CreateValidCommand();
            var userId = "source-user-id";
            var sourceAccount = new Account { Id = command.SourceAccountId, OwnerUserId = userId, CurrentBalance = 500, Status = AccountStatus.Active };
            var destAccount = new Account { AccountNumber = command.DestinationAccountNumber, Status = AccountStatus.Active };

            _mockCurrentUser.Setup(s => s.UserId).Returns(userId);
            _mockAccountRepo.Setup(r => r.GetByIdAsync(command.SourceAccountId)).ReturnsAsync(sourceAccount);
            _mockAccountRepo.Setup(r => r.FirstOrDefaultAsync(a => a.AccountNumber == command.DestinationAccountNumber)).ReturnsAsync(destAccount);
            _mockTransactionRepo.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("Simulated DB error"));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.code.Should().Be(500);
            _mockTransactionRepo.Verify(r => r.BeginTransactionAsync(), Times.Once);
            _mockTransactionRepo.Verify(r => r.CommitTransactionAsync(), Times.Never);
            _mockTransactionRepo.Verify(r => r.RollbackTransactionAsync(), Times.Once);
        }
    }
}