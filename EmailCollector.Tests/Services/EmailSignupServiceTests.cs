using EmailCollector.Api.Configurations;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace EmailCollector.Api.Tests.Services
{
    [TestFixture]
    public class EmailSignupServiceTests
    {
        private Mock<IEmailSignupRepository> _emailSignupRepositoryMock;
        private Mock<ISignupFormRepository> _signupFormRepositoryMock;
        private Mock<ILogger<EmailSignupService>> _loggerMock;
        private IEmailSignupService _emailSignupService;
        private Mock<IDistributedCache> _signupCandidatesCacheMock;
        private Mock<IAppEmailSender> _emailSenderMock;
        private Mock<IFeatureToggles> _featureTogglesServiceMock;
        private Mock<ISmtpEmailSettingsRepository> _smtpEmailSettingsRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _emailSignupRepositoryMock = new Mock<IEmailSignupRepository>();
            _signupFormRepositoryMock = new Mock<ISignupFormRepository>();
            _loggerMock = new Mock<ILogger<EmailSignupService>>();
            _signupCandidatesCacheMock = new Mock<IDistributedCache>();
            _emailSenderMock = new Mock<IAppEmailSender>();
            _featureTogglesServiceMock = new Mock<IFeatureToggles>();
            _smtpEmailSettingsRepositoryMock = new Mock<ISmtpEmailSettingsRepository>();
            
            _emailSignupService = new EmailSignupService(
                _emailSignupRepositoryMock.Object,
                _signupFormRepositoryMock.Object,
                _loggerMock.Object,
                _signupCandidatesCacheMock.Object,
                _emailSenderMock.Object,
                _featureTogglesServiceMock.Object,
                _smtpEmailSettingsRepositoryMock.Object);
        }

        [Test]
        public async Task GetSignupsByFormIdAsync_FormNotFound_ReturnsNull()
        {
            // Arrange
            Guid formId = Guid.NewGuid();
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId))
                .ReturnsAsync((SignupForm)null);

            // Act
            var result = await _emailSignupService.GetSignupsByFormIdAsync(formId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSignupsByFormIdAsync_FormFound_ReturnsSignups()
        {
            // Arrange
            Guid formId = Guid.NewGuid();
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
            var result = await _emailSignupService.GetSignupsByFormIdAsync(formId);

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
            var emailSignupDto = new EmailSignupDto { Email = "invalidemail", FormId = Guid.NewGuid() };
            var expectedResponse = new SignupResultDto
            {
                Success = false,
                Message = "Invalid email address.",
                ErrorCode = EmailSignupErrorCode.InvalidEmail,
            };

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(expectedResponse.ErrorCode));
        }

        [Test]
        public async Task SubmitEmailAsync_FormNotFound_ReturnsFormNotFoundResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@gmail.com", FormId = Guid.NewGuid() };
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

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(expectedResponse.ErrorCode));
        }

        [Test]
        public async Task SubmitEmailAsync_FormNotActive_ReturnsFormNotActiveResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@gmail.com", FormId = Guid.NewGuid() };
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

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo(expectedResponse.ErrorCode));
        }

        [Test]
        public async Task SubmitEmailAsync_ValidEmailAndForm_EmailConfirmation_Disabled_ReturnsSuccessResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@gmail.com", FormId = Guid.NewGuid() };
            var form = new SignupForm { FormName = "test", Status = FormStatus.Active, CreatedBy = Guid.NewGuid() };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(emailSignupDto.FormId))
                .ReturnsAsync(form);
            _featureTogglesServiceMock.Setup(service => service.IsEmailConfirmationEnabled()).Returns(false);

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            // Assert
            Assert.That(result.Success, Is.True);

            // assert distributed cache was not called
            _signupCandidatesCacheMock.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);

            // assert email sender was not called
            _emailSenderMock.Verify(sender => sender.SendEmail(It.IsAny<Message>(), It.IsAny<SmtpEmailSettings?>()), Times.Never);
        }

        [Test]
        public async Task SubmitEmailAsync_ValidEmailAndForm_EmailConfirmation_Enabled_ReturnsSuccessResult()
        {
            // Arrange
            var emailSignupDto = new EmailSignupDto { Email = "test@gmail.com", FormId = Guid.NewGuid() };
            var form = new SignupForm { FormName = "test", Status = FormStatus.Active, CreatedBy = Guid.NewGuid() };
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(emailSignupDto.FormId))
                .ReturnsAsync(form);
            _featureTogglesServiceMock.Setup(service => service.IsEmailConfirmationEnabled()).Returns(true);

            var expectedResponse = new SignupResultDto
            {
                Success = true,
                Message = "Email address submitted successfully.",
            };

            // Act
            var result = await _emailSignupService.SubmitEmailAsync(emailSignupDto);

            // Assert
            Assert.That(result.Success, Is.True);

            // assert distributed cache was not called
            _signupCandidatesCacheMock.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None), Times.Once);

            // assert email sender was not called
            _emailSenderMock.Verify(sender => sender.SendEmail(It.IsAny<Message>(), It.IsAny<SmtpEmailSettings?>()), Times.Once);
        }

        [Test]
        public async Task ConfirmEmailSignupAsync_ExpiredToken_ReturnsExpiredTokenResult()
        {
            // Arrange
            var confirmationToken = "expiredToken";
            _signupCandidatesCacheMock.Setup(cache => cache.GetAsync(confirmationToken, CancellationToken.None)).ReturnsAsync((byte[])null);

            // Act
            var result = await _emailSignupService.ConfirmEmailSignupAsync(confirmationToken);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Confirmation token expired."));
            Assert.That(result.ErrorCode, Is.EqualTo(EmailConfirmationErrorCode.ExpiredToken));
        }

        [Test]
        public async Task ConfirmEmailSignupAsync_InvalidToken_ReturnsInvalidTokenResult()
        {
            // Arrange
            var confirmationToken = "invalidToken";
            var encodedSignupCandidate = Encoding.UTF8.GetBytes("formId:1#signup:"); // signup candidate without email
            _signupCandidatesCacheMock.Setup(cache => cache.GetAsync(confirmationToken, CancellationToken.None)).ReturnsAsync(encodedSignupCandidate);

            // Act
            var result = await _emailSignupService.ConfirmEmailSignupAsync(confirmationToken);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid confirmation token."));
            Assert.That(result.ErrorCode, Is.EqualTo(EmailConfirmationErrorCode.InvalidToken));
        }

        [Test]
        public async Task ConfirmEmailSignupAsync_EmailAlreadyConfirmed_ReturnsEmailAlreadyConfirmedResult()
        {
            // Arrange
            var formId = Guid.NewGuid();
            var confirmationToken = "validToken";
            var encodedSignupCandidate = Encoding.UTF8.GetBytes($"formId:{formId}#signup:test@example.com");
            var email = "test@example.com";
            var existingSignups = new List<EmailSignup>
            {
                new EmailSignup { EmailAddress = email }
            };
            _signupCandidatesCacheMock.Setup(cache => cache.GetAsync(confirmationToken, CancellationToken.None)).ReturnsAsync(encodedSignupCandidate);
            _emailSignupRepositoryMock.Setup(repo => repo.GetByFormIdAsync(formId)).ReturnsAsync(existingSignups);

            // Act
            var result = await _emailSignupService.ConfirmEmailSignupAsync(confirmationToken);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email already confirmed."));
            Assert.That(result.ErrorCode, Is.EqualTo(EmailConfirmationErrorCode.EmailAlreadyConfirmed));
        }

        [Test]
        public async Task ConfirmEmailSignupAsync_ValidToken_AddsEmailSignupAndRemovesTokenAndReturnsSuccessResult()
        {
            // Arrange
            var formId = Guid.NewGuid();
            var confirmationToken = "validToken";
            var encodedSignupCandidate = Encoding.UTF8.GetBytes($"formId:{formId}#signup:test@example.com");
            var email = "test@example.com";
            var form = new SignupForm { Id = formId, FormName = "test", CreatedBy = Guid.NewGuid() };
            _signupCandidatesCacheMock.Setup(cache => cache.GetAsync(confirmationToken, CancellationToken.None)).ReturnsAsync(encodedSignupCandidate);
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId)).ReturnsAsync(form);
            //_emailSignupRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<EmailSignup>())).Returns(Task.CompletedTask);
            //_signupCandidatesCacheMock.Setup(cache => cache.RemoveAsync(confirmationToken, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var result = await _emailSignupService.ConfirmEmailSignupAsync(confirmationToken);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Email confirmed."));
            _emailSignupRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<EmailSignup>()), Times.Once);
            _signupCandidatesCacheMock.Verify(cache => cache.RemoveAsync(confirmationToken, CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task GetSignupsPerDayAsync_FormNotFound_ThrowsArgumentException()
        {
            // Arrange
            var formId = Guid.NewGuid();
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.Date;
            _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId)).ReturnsAsync((SignupForm)null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _emailSignupService.GetSignupsPerDayAsync(formId, startDate, endDate));
        }
    }
}
