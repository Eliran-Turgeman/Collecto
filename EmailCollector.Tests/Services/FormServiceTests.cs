using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services.Exports;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmailCollector.Api.Tests.Services;

public class FormServiceTests
{
    private Mock<ISignupFormRepository> _signupFormRepositoryMock;
    private Mock<IEmailSignupRepository> _emailSignupRepositoryMock;
    private Mock<ISmtpEmailSettingsRepository> _smtpSettingsRepositoryMock;
    private Mock<IFormCorsSettingsRepository> _formCorsSettingsRepositoryMock;
    private Mock<IRepository<RecaptchaFormSettings>> _recaptchaSettingsRepositoryMock;
    private Mock<IExportService> _exportServiceMock;
    private Mock<ILogger<FormService>> _loggerMock;
    private IFormService _formService;

    [SetUp]
    public void Setup()
    {
        _signupFormRepositoryMock = new Mock<ISignupFormRepository>();
        _emailSignupRepositoryMock = new Mock<IEmailSignupRepository>();
        _smtpSettingsRepositoryMock = new Mock<ISmtpEmailSettingsRepository>();
        _formCorsSettingsRepositoryMock = new Mock<IFormCorsSettingsRepository>();
        _recaptchaSettingsRepositoryMock = new Mock<IRepository<RecaptchaFormSettings>>();
        _exportServiceMock = new Mock<IExportService>();
        _loggerMock = new Mock<ILogger<FormService>>();
        _formService = new FormService(_signupFormRepositoryMock.Object,
            _emailSignupRepositoryMock.Object,
            _smtpSettingsRepositoryMock.Object,
            _formCorsSettingsRepositoryMock.Object,
            _recaptchaSettingsRepositoryMock.Object,
            _exportServiceMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task CreateFormAsync_ShouldCreateFormAndReturnFormDetailsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createFormDto = new CreateFormDto
        {
            FormName = "Test Form",
        };
        var expectedFormId = 1;
        var expectedFormName = "Test Form";
        var signupForm = new SignupForm
        {
            Id = expectedFormId,
            FormName = expectedFormName,
            CreatedBy = userId,
        };
        _signupFormRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<SignupForm>()))
            .Callback<SignupForm>(form =>
            {
                form.Id = expectedFormId;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _formService.CreateFormAsync(userId, createFormDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedFormId));
            Assert.That(result.FormName, Is.EqualTo(expectedFormName));
        });
        _signupFormRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SignupForm>()), Times.Once);
    }

    [Test]
    public async Task GetFormsByUserAsync_ShouldReturnListOfFormDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var forms = new List<SignupForm>
            {
                new SignupForm { Id = 1, FormName = "Form 1", CreatedBy = userId },
                new SignupForm { Id = 2, FormName = "Form 2", CreatedBy = userId },
                new SignupForm { Id = 3, FormName = "Form 3", CreatedBy = userId },
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
        var formId = 1;
        var userId = Guid.NewGuid();
        var form = new SignupForm
        {
            Id = 1,
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
        var formId = 1;
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
        var formId = 1;
        var userId = Guid.NewGuid();
        var form = new SignupForm
        {
            Id = 1,
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
        var formId = 1;
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
        var formId = 1;
        var userId = Guid.NewGuid();
        var createFormDto = new CreateFormDto
        {
            FormName = "Updated Form",
            Status = FormStatus.Active,
        };
        var form = new SignupForm
        {
            Id = 1,
            FormName = "Test Form",
            CreatedBy = userId,
        };
        _signupFormRepositoryMock.Setup(repo => repo.GetByFormIdentifierAsync(formId, userId))
            .ReturnsAsync(form);

        // Act
        var result = await _formService.UpdateFormAsync(formId, userId, createFormDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(form.Id));
            Assert.That(result.FormName, Is.EqualTo(createFormDto.FormName));
        });
        _signupFormRepositoryMock.Verify(repo => repo.GetByFormIdentifierAsync(formId, userId), Times.Once);
        _signupFormRepositoryMock.Verify(repo => repo.Update(form), Times.Once);
    }

    [Test]
    public async Task UpdateFormAsync_ShouldReturnNull_WhenFormDoesNotExistOrUserIsNotCreator()
    {
        // Arrange
        var formId = 1;
        var userId = Guid.NewGuid();
        var createFormDto = new CreateFormDto
        {
            FormName = "Updated Form",
            Status = FormStatus.Active,
        };
        _signupFormRepositoryMock.Setup(repo => repo.GetByFormIdentifierAsync(formId, userId))
            .ReturnsAsync((SignupForm)null);

        // Act
        var result = await _formService.UpdateFormAsync(formId, userId, createFormDto);

        // Assert
        Assert.That(result, Is.Null);
        _signupFormRepositoryMock.Verify(repo => repo.GetByFormIdentifierAsync(formId, userId), Times.Once);
        _signupFormRepositoryMock.Verify(repo => repo.Update(It.IsAny<SignupForm>()), Times.Never);
    }
}
