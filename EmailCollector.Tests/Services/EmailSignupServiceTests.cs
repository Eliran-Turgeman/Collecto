using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using EmailCollector.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EmailCollector.Tests.Services
{
    [TestFixture]
    public class EmailSignupServiceTests
    {
        private Mock<IEmailSignupRepository> _emailSignupRepositoryMock;
        private Mock<ISignupFormRepository> _signupFormRepositoryMock;
        private Mock<ILogger<EmailSignupService>> _loggerMock;
        private Mock<IDnsLookupService> _dnsLookupServiceMock;
        private IEmailSignupService _emailSignupService;

        [SetUp]
        public void Setup()
        {
            _emailSignupRepositoryMock = new Mock<IEmailSignupRepository>();
            _signupFormRepositoryMock = new Mock<ISignupFormRepository>();
            _loggerMock = new Mock<ILogger<EmailSignupService>>();
            _dnsLookupServiceMock = new Mock<IDnsLookupService>();

            // Always return true for DNS lookup
            _dnsLookupServiceMock.Setup(service => service.HasMxRecords(It.IsAny<string>())).Returns(true);

            _emailSignupService = new EmailSignupService(
                _emailSignupRepositoryMock.Object,
                _signupFormRepositoryMock.Object,
                _loggerMock.Object,
                _dnsLookupServiceMock.Object);
        }

        [Test]
        public async Task GetSignupsByFormIdAsync_FormNotFound_ReturnsNull()
        {
            // Arrange
            int formId = 1;
            Guid userId = Guid.NewGuid();
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId))
                .ReturnsAsync((SignupForm)null);

            // Act
            var result = await _emailSignupService.GetSignupsByFormIdAsync(formId, userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSignupsByFormIdAsync_UserNotCreator_ReturnsNull()
        {
            // Arrange
            int formId = 1;
            Guid userId = Guid.NewGuid();
            var form = new SignupForm { FormName = "test", CreatedBy = Guid.NewGuid() };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId))
                .ReturnsAsync(form);

            // Act
            var result = await _emailSignupService.GetSignupsByFormIdAsync(formId, userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSignupsByFormIdAsync_FormFound_ReturnsSignups()
        {
            // Arrange
            int formId = 1;
            Guid userId = Guid.NewGuid();
            var form = new SignupForm { FormName = "test", CreatedBy = userId };
            var signups = new List<EmailSignup>
            {
                new() { EmailAddress = "test1@example.com", SignupFormId = formId, SignupDate = DateTime.Now },
                new() { EmailAddress = "test2@example.com", SignupFormId = formId, SignupDate = DateTime.Now },
            };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId))
                .ReturnsAsync(form);
            _emailSignupRepositoryMock.Setup(repo => repo.GetByFormIdAsync(formId))
                .ReturnsAsync(signups);

            // Act
            var result = await _emailSignupService.GetSignupsByFormIdAsync(formId, userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(signups.Count));
            foreach (var signup in signups)
            {
                Assert.That(result.Any(dto =>
                    dto.Email == signup.EmailAddress &&
                    dto.FormId == signup.SignupFormId &&
                    dto.SignupDate == signup.SignupDate), Is.True);
            }
        }

        [Test]
        public async Task SubmitEmailAsync_InvalidEmail_ReturnsInvalidEmailResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "invalidemail", FormId = 1 };
            var expectedResponse = new SignupResultDto
            {
                Success = false,
                Message = "Invalid email address.",
                ErrorCode = EmailSignupErrorCode.InvalidEmail,
            };

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Success, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo(expectedResponse.ErrorCode));
            });
        }

        [Test]
        public async Task SubmitEmailAsync_FormNotFound_ReturnsFormNotFoundResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@example.com", FormId = 1 };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(emailSignupDto.FormId))
                .ReturnsAsync((SignupForm)null);
            var expectedResponse = new SignupResultDto
            {
                Success = false,
                Message = "Form not found.",
                ErrorCode = EmailSignupErrorCode.FormNotFound,
            };

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Success, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo(expectedResponse.ErrorCode));
            });
        }

        [Test]
        public async Task SubmitEmailAsync_FormNotActive_ReturnsFormNotActiveResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@example.com", FormId = 1 };
            var form = new SignupForm { FormName = "test", Status = FormStatus.Inactive, CreatedBy = Guid.NewGuid() };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(emailSignupDto.FormId))
                .ReturnsAsync(form);
            var expectedResponse = new SignupResultDto
            {
                Success = false,
                Message = "Form is not active.",
                ErrorCode = EmailSignupErrorCode.FormNotActive,
            };

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Success, Is.False);
                Assert.That(result.ErrorCode, Is.EqualTo(expectedResponse.ErrorCode));
            });
        }

        [Test]
        public async Task SubmitEmailAsync_ValidEmailAndForm_ReturnsSuccessResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@example.com", FormId = 1 };
            var form = new SignupForm { FormName = "test", Status = FormStatus.Active, CreatedBy = Guid.NewGuid() };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(emailSignupDto.FormId))
                .ReturnsAsync(form);
            var expectedResponse = new SignupResultDto
            {
                Success = true,
                Message = "Email address submitted successfully.",
            };

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Success, Is.True);
            });
        }
    }
}
