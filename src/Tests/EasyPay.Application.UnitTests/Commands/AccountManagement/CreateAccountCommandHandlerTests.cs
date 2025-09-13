using Moq;
using FluentAssertions;
using AutoMapper;
using EasyPay.Application.Services;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using EasyPay.Infrastructure.Aggregates.Identity;
using EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.Identity;
using System.Linq.Expressions;

namespace EasyPay.Application.UnitTests.Commands.AccountManagement
{
    public class CreateAccountCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IAccountTypeDocumentRequirementRepository> _mockDocReqRepository;
        private readonly Mock<IAuthItemValueRepository> _mockAuthItemValueRepository;
        private readonly Mock<IAuthItemRepository> _mockAuthItemRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IAccountNumberService> _mockAccountNumberService;
        private readonly CreateAccountCommandHandler _handler;

        public CreateAccountCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockDocReqRepository = new Mock<IAccountTypeDocumentRequirementRepository>();
            _mockAuthItemValueRepository = new Mock<IAuthItemValueRepository>();
            _mockAuthItemRepository = new Mock<IAuthItemRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockAccountNumberService = new Mock<IAccountNumberService>();

            _handler = new CreateAccountCommandHandler(
                _mockAccountRepository.Object,
                _mockDocReqRepository.Object,
                _mockAuthItemValueRepository.Object,
                _mockAuthItemRepository.Object,
                _mockMapper.Object,
                _mockCurrentUserService.Object,
                _mockAccountNumberService.Object
            );

            _mockCurrentUserService.Setup(s => s.UserId).Returns("user-123");
            _mockMapper.Setup(m => m.Map<Account>(It.IsAny<CreateAccountCommand>())).Returns(new Account());
            _mockAccountNumberService.Setup(s => s.GenerateUniqueAccountNumberAsync()).ReturnsAsync("1234567890");
        }

        [Fact]
        public async Task Handle_Should_Succeed_WhenAccountTypeHasNoRequirements()
        {
            var command = new CreateAccountCommand { AccountTypeId = 1, Title = "Test Account" };
            _mockDocReqRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AccountTypeDocumentRequirement, bool>>>(), null, null, null, null, true))
                               .Returns(new List<AccountTypeDocumentRequirement>().AsQueryable());
            var result = await _handler.Handle(command, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _mockAccountRepository.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Succeed_WhenAllRequirementsAreMet()
        {
            var command = new CreateAccountCommand { AccountTypeId = 1, Title = "Test Account" };
            var requirements = new List<AccountTypeDocumentRequirement> { new AccountTypeDocumentRequirement { AuthItemId = 101 } };
            var userDocuments = new List<AuthItemValue> { new AuthItemValue(101, "user-123", "value") { Status = VerificationStatus.Approved } };

            _mockDocReqRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AccountTypeDocumentRequirement, bool>>>(), null, null, null, null, true))
                               .Returns(requirements.AsQueryable());
            _mockAuthItemValueRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AuthItemValue, bool>>>(), null, null, null, null, true))
                                        .Returns(userDocuments.AsQueryable());
            var result = await _handler.Handle(command, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _mockAccountRepository.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Fail_WhenDocumentIsMissing()
        {
            var command = new CreateAccountCommand { AccountTypeId = 1, Title = "Test Account" };
            var requirements = new List<AccountTypeDocumentRequirement> { new AccountTypeDocumentRequirement { AuthItemId = 101 } };
            var userDocuments = new List<AuthItemValue>();

            _mockDocReqRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AccountTypeDocumentRequirement, bool>>>(), null, null, null, null, true))
                               .Returns(requirements.AsQueryable());
            _mockAuthItemValueRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AuthItemValue, bool>>>(), null, null, null, null, true))
                                        .Returns(userDocuments.AsQueryable());
            _mockAuthItemRepository.Setup(r => r.GetByIdAsync(101)).ReturnsAsync(new AuthItem { Title = "National ID" });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.code.Should().Be(400);
            result.error.message.Should().Contain("National ID");
        }

        [Fact]
        public async Task Handle_Should_Fail_WhenDocumentIsNotApproved()
        {
            var command = new CreateAccountCommand { AccountTypeId = 1, Title = "Test Account" };
            var requirements = new List<AccountTypeDocumentRequirement> { new AccountTypeDocumentRequirement { AuthItemId = 101 } };

            var userDocuments = new List<AuthItemValue> { new AuthItemValue(101, "user-123", "value") { Status = VerificationStatus.Pending } };

            _mockDocReqRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AccountTypeDocumentRequirement, bool>>>(), null, null, null, null, true))
                               .Returns(requirements.AsQueryable());
            _mockAuthItemValueRepository.Setup(r => r.Query(It.IsAny<Expression<Func<AuthItemValue, bool>>>(), null, null, null, null, true))
                                        .Returns(userDocuments.AsQueryable());
            _mockAuthItemRepository.Setup(r => r.GetByIdAsync(101)).ReturnsAsync(new AuthItem { Title = "National ID" });

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.error.code.Should().Be(400);
            result.error.message.Should().Contain("User is missing the following approved documents");
        }
    }
}