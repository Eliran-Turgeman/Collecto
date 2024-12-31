using Blazorise;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Repositories.DAL;
using EmailCollector.Api.Services.Exports;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmailCollector.Api.Tests.Services;

public class FormServiceTests
{
    private Mock<ISignupFormRepository> _signupFormRepositoryMock;
    private Mock<IEmailSignupRepository> _emailSignupRepositoryMock;
    private Mock<IExportService> _exportServiceMock;
    private Mock<ILogger<FormService>> _loggerMock;
    private IFormService _formService;
    private Mock<IFormsDAL> _formsDALMock;

    [SetUp]
    public void Setup()
    {
        _signupFormRepositoryMock = new Mock<ISignupFormRepository>();
        _emailSignupRepositoryMock = new Mock<IEmailSignupRepository>();
        _exportServiceMock = new Mock<IExportService>();
        _loggerMock = new Mock<ILogger<FormService>>();
        _formsDALMock = new Mock<IFormsDAL>();
        _formService = new FormService(_signupFormRepositoryMock.Object,
            _emailSignupRepositoryMock.Object,
            _exportServiceMock.Object,
            _loggerMock.Object,
            _formsDALMock.Object);
    }

    [Test]
    public async Task CreateFormAsync_ShouldCreateFormAndReturnFormDetailsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var formName = "Test Form";
        var createFormDto = new ExtendedCreateFormDto
        {
            FormName = formName,
            EmailFrom = "test@test.com",
            SmtpServer = "smtp.test.com",
            SmtpPort = 25,
            SmtpUsername = "test@test.com",
            SmtpPassword = "test",
            AllowedOrigins = "https://www.test.com",
            CaptchaSecretKey = "test",
            CaptchaSiteKey = "test",
        };
        
        _formsDALMock.Setup(dal => dal.SaveFormWithTransaction(It.IsAny<SignupForm>())).Returns(Task.FromResult(true));

        // Act
        var result = await _formService.CreateFormAsync(userId, createFormDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.FormName, Is.EqualTo(formName));
        });
    }

    [Test]
    public async Task GetFormsByUserAsync_ShouldReturnListOfFormDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var forms = new List<SignupForm>
            {
                new SignupForm { Id = Guid.NewGuid(), FormName = "Form 1", CreatedBy = userId },
                new SignupForm { Id = Guid.NewGuid(), FormName = "Form 2", CreatedBy = userId },
                new SignupForm { Id = Guid.NewGuid(), FormName = "Form 3", CreatedBy = userId },
            };
        _signupFormRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
            .ReturnsAsync(forms);

        // Act
        var result = await _formService.GetFormsByUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Replace the problematic line with the following code
        Assert.That(result.Count(), Is.EqualTo(forms.Count));
        foreach (var form in forms)
        {
            Assert.That(result.Any(f => f.Id == form.Id && f.FormName == form.FormName), Is.True);
        }
        _signupFormRepositoryMock.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetFormByIdAsync_ShouldReturnFormDetailsDto_WhenFormExistsAndUserIsCreator()
    {
        // Arrange
        var formId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var form = new SignupForm
        {
            Id = formId,
            FormName = "Test Form",
            CreatedBy = userId,
        };
        _signupFormRepositoryMock.Setup(repo => repo.GetByFormIdentifierAsync(formId, userId))
            .ReturnsAsync(form);

        // Act
        var result = await _formService.GetFormByIdAsync(formId, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(form.Id));
            Assert.That(result.FormName, Is.EqualTo(form.FormName));
        });
        _signupFormRepositoryMock.Verify(repo => repo.GetByFormIdentifierAsync(formId, userId), Times.Once);
    }

    [Test]
    public async Task GetFormByIdAsync_ShouldReturnNull_WhenFormDoesNotExistOrUserIsNotCreator()
    {
        // Arrange
        var formId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _signupFormRepositoryMock.Setup(repo => repo.GetByFormIdentifierAsync(formId, userId))
            .ReturnsAsync((SignupForm)null);

        // Act
        var result = await _formService.GetFormByIdAsync(formId, userId);

        // Assert
        Assert.That(result, Is.Null);
        _signupFormRepositoryMock.Verify(repo => repo.GetByFormIdentifierAsync(formId, userId), Times.Once);
    }

    [Test]
    public async Task DeleteFormByIdAsync_ShouldDeleteForm_WhenFormExistsAndUserIsCreator()
    {
        // Arrange
        var formId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var form = new SignupForm
        {
            Id = formId,
            FormName = "Test Form",
            CreatedBy = userId,
        };
        _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId))
            .ReturnsAsync(form);

        // Act
        await _formService.DeleteFormByIdAsync(formId);

        // Assert
        _signupFormRepositoryMock.Verify(repo => repo.GetByIdAsync(formId), Times.Once);
        _signupFormRepositoryMock.Verify(repo => repo.Remove(form), Times.Once);
    }

    [Test]
    public async Task DeleteFormByIdAsync_ShouldNotDeleteForm_WhenFormDoesNotExistOrUserIsNotCreator()
    {
        // Arrange
        var formId = Guid.NewGuid();
        _signupFormRepositoryMock.Setup(repo => repo.GetByIdAsync(formId))
            .ReturnsAsync((SignupForm)null);

        // Act
        await _formService.DeleteFormByIdAsync(formId);

        // Assert
        _signupFormRepositoryMock.Verify(repo => repo.GetByIdAsync(formId), Times.Once);
        _signupFormRepositoryMock.Verify(repo => repo.Remove(It.IsAny<SignupForm>()), Times.Never);
    }

    [Test]
    public async Task UpdateFormAsync_ShouldUpdateFormAndReturnFormDetailsDto_WhenFormExistsAndUserIsCreator()
    {
        // Arrange
        var formId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        const string formName = "Test Form";
        const string updatedFormName = "Updated Form";
        var createFormDto = new ExtendedCreateFormDto
        {
            FormName = updatedFormName,
            EmailFrom = "test@test.com",
            SmtpServer = "smtp.test.com",
            SmtpPort = 25,
            SmtpUsername = "test@test.com",
            SmtpPassword = "test",
            AllowedOrigins = "https://www.test.com",
            CaptchaSecretKey = "test",
            CaptchaSiteKey = "test",
            Status = FormStatus.Inactive
        };
        var form = new SignupForm
        {
            Id = formId,
            FormName = formName,
            CreatedBy = userId,
            FormCorsSettings = new FormCorsSettings
            {
                AllowedOrigins = "https://www.test.com",
                FormId = formId
            },
            CustomEmailTemplates = [],
            FormEmailSettings = new SmtpEmailSettings
            {
                EmailFrom = "test@test.com",
                EmailMethod = EmailMethod.Smtp,
                SmtpPort = 465,
                SmtpUsername = "test@test.com",
                SmtpPassword = "test",
                SmtpServer = "smtp.test.com",
                FormId = formId,
            },
            RecaptchaSettings = new RecaptchaFormSettings
            {
                FormId = formId,
                SecretKey = "test",
                SiteKey = "test",
            }
        };
        _signupFormRepositoryMock.Setup(repo => repo.GetByFormIdentifierAsync(formId, userId))
            .ReturnsAsync(form);
        
        _formsDALMock.Setup(dal => dal.SaveFormWithTransaction(It.IsAny<SignupForm>())).Returns(Task.FromResult(true));
        
        // Act
        var result = await _formService.UpdateFormAsync(formId, userId, createFormDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(form.Id));
            Assert.That(result.FormName, Is.EqualTo(updatedFormName));
        });
    }

    [Test]
    public async Task UpdateFormAsync_ShouldReturnNull_WhenFormDoesNotExistOrUserIsNotCreator()
    {
        // Arrange
        var formId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var formName = "Test Form";
        var createFormDto = new ExtendedCreateFormDto
        {
            FormName = formName,
            EmailFrom = "test@test.com",
            SmtpServer = "smtp.test.com",
            SmtpPort = 25,
            SmtpUsername = "test@test.com",
            SmtpPassword = "test",
            AllowedOrigins = "https://www.test.com",
            CaptchaSecretKey = "test",
            CaptchaSiteKey = "test",
            Status = FormStatus.Inactive
        };
        _signupFormRepositoryMock.Setup(repo => repo.GetByFormIdentifierAsync(formId, userId))
            .ReturnsAsync((SignupForm)null!);

        // Act
        var result = await _formService.UpdateFormAsync(formId, userId, createFormDto);

        // Assert
        Assert.That(result, Is.Null);
        _signupFormRepositoryMock.Verify(repo => repo.GetByFormIdentifierAsync(formId, userId), Times.Once);
        _signupFormRepositoryMock.Verify(repo => repo.Update(It.IsAny<SignupForm>()), Times.Never);
    }
}
