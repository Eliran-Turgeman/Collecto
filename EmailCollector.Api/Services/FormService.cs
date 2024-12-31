using System.Text.Json;
using Blazorise.Extensions;
using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Repositories.DAL;
using EmailCollector.Api.Services.Exports;
using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.Services;

/// <summary>
/// Implements form management functionality.
/// </summary>
public class FormService : IFormService
{
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly IEmailSignupRepository _emailSignupRepository;
    private readonly IExportService _exportService;
    private readonly ILogger<FormService> _logger;
    private readonly IFormsDAL _formsDAL;

    public FormService(ISignupFormRepository signupFormRepository,
        IEmailSignupRepository emailSignupRepository,
        IExportService exportService,
        ILogger<FormService> logger,
        IFormsDAL formsDAL)
    {
        _signupFormRepository = signupFormRepository;
        _emailSignupRepository = emailSignupRepository;
        _exportService = exportService;
        _logger = logger;
        _formsDAL = formsDAL;
    }
    
    public async Task<FormDetailsDto> CreateFormAsync(Guid userId, ExtendedCreateFormDto extendedCreateFormDto)
    {
        var form = new SignupForm
        {
            Id = Guid.NewGuid(),
            FormName = extendedCreateFormDto.FormName,
            CreatedBy = userId,
            Status = extendedCreateFormDto.Status,
        };
        
        var smtpConfig = new SmtpEmailSettings
        {
            EmailMethod = EmailMethod.Smtp,
            EmailFrom = extendedCreateFormDto.EmailFrom!,
            Form = form,
            FormId = form.Id,
            SmtpServer = extendedCreateFormDto.SmtpServer!,
            SmtpPort = extendedCreateFormDto.SmtpPort!.Value,
            SmtpUsername = extendedCreateFormDto.SmtpUsername!,
            SmtpPassword = extendedCreateFormDto.SmtpPassword!
        };

        var corsConfig = new FormCorsSettings
        {
            Form = form,
            FormId = form.Id,
            AllowedOrigins = extendedCreateFormDto.AllowedOrigins!,
        };
        
        var recaptchaConfig = new RecaptchaFormSettings
        {
            Form = form,
            FormId = form.Id,
            SiteKey = extendedCreateFormDto.CaptchaSiteKey!,
            SecretKey = extendedCreateFormDto.CaptchaSecretKey!,
        };
        
        form.FormEmailSettings = smtpConfig;
        form.FormCorsSettings = corsConfig;
        form.RecaptchaSettings = recaptchaConfig;
        form.CustomEmailTemplates = [];
        
        await _formsDAL.SaveFormWithTransaction(form);
        
        return new FormDetailsDto
        {
            Id = form.Id,
            FormName = form.FormName,
            Status = form.Status,
        };
    }

    public async Task<IEnumerable<FormDto>> GetFormsByUserAsync(Guid userId)
    {
        var forms = await _signupFormRepository.GetByUserIdAsync(userId);

        var signupForms = forms as SignupForm[] ?? forms.ToArray();
        _logger.LogInformation($"Found {signupForms.Count()} forms for user {userId}.");

        return signupForms.Select(form => new FormDto
        {
            Id = form.Id,
            FormName = form.FormName,
        });
    }

    public async Task<FormDetailsDto?> GetFormByIdAsync(Guid formId, Guid userId)
    {
        var form = await _signupFormRepository.GetByFormIdentifierAsync(formId, userId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return null;
        }

        _logger.LogInformation($"Found form {formId}.");

        return new FormDetailsDto
        {
            Id = form.Id,
            FormName = form.FormName,
            Status = form.Status,
        };
    }

    public async Task DeleteFormByIdAsync(Guid formId)
    {
        var form = await _signupFormRepository.GetByIdAsync(formId);

        if (form == null)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return;
        }

        await _signupFormRepository.Remove(form);
    }

    public async Task<FormDetailsDto?> UpdateFormAsync(Guid formId, Guid userId, ExtendedCreateFormDto createFormDto)
    {
        var form = await _signupFormRepository.GetByFormIdentifierAsync(formId, userId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return null;
        }

        form.FormName = createFormDto.FormName;
        form.Status = createFormDto.Status;

        form.FormCorsSettings.AllowedOrigins = createFormDto.AllowedOrigins ?? form.FormCorsSettings.AllowedOrigins;
        form.FormEmailSettings.EmailFrom = createFormDto.EmailFrom ?? form.FormEmailSettings.EmailFrom;
        form.RecaptchaSettings.SecretKey = createFormDto.CaptchaSecretKey ?? form.RecaptchaSettings.SecretKey;
        form.RecaptchaSettings.SiteKey = createFormDto.CaptchaSiteKey ?? form.RecaptchaSettings.SiteKey;
        
        if (form.FormEmailSettings is SmtpEmailSettings currentSmtpEmailSettings)
        {
            currentSmtpEmailSettings.SmtpServer = createFormDto.SmtpServer ?? currentSmtpEmailSettings.SmtpServer;
            currentSmtpEmailSettings.SmtpPort = createFormDto.SmtpPort ?? currentSmtpEmailSettings.SmtpPort;
            currentSmtpEmailSettings.SmtpUsername = createFormDto.SmtpUsername ?? currentSmtpEmailSettings.SmtpUsername;
            currentSmtpEmailSettings.SmtpPassword = createFormDto.SmtpPassword ?? currentSmtpEmailSettings.SmtpPassword;
        }

        await _formsDAL.SaveFormWithTransaction(form);

        _logger.LogInformation($"Updated form {formId}.");

        return new FormDetailsDto
        {
            Id = form.Id,
            FormName = form.FormName,
            Status = form.Status,
        };
    }

    /// <summary>
    /// Gets the details of all forms by userId including the number of signups.
    /// Mainly used for the form summary page.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<FormSummaryDetailsDto>> GetFormsSummaryDetailsAsync(Guid userId)
    {
        var forms = await _signupFormRepository.GetByUserIdAsync(userId);

        var signupForms = forms.ToList();
        _logger.LogInformation($"Found {signupForms.Count} forms for user {userId}.");

        var formSummaryDetails = new List<FormSummaryDetailsDto>();

        foreach (var form in signupForms)
        {
            var signupCount = await _emailSignupRepository.GetSignupCountByFormId(form.Id);

            formSummaryDetails.Add(new FormSummaryDetailsDto
            {
                Id = form.Id,
                FormName = form.FormName,
                SubmissionsCount = signupCount,
                CreatedAt = form.CreatedAt,
            });
        }

        return formSummaryDetails;
    }
    
    public async Task<byte[]> ExportFormsAsync(IEnumerable<Guid> formIds, ExportFormat format)
    {
        var forms = await GetFormsSubmissionsData(formIds);
        var formData = forms.ToList();
        if (!formData.IsNullOrEmpty()) return await _exportService.ExportAsync(formData, format);
        
        _logger.LogInformation($"No available forms to export.");
        return [];
    }
    
    
    private async Task<IEnumerable<FormSubmissionsDataDto>> GetFormsSubmissionsData(IEnumerable<Guid> formIds)
    {
        var forms = await _signupFormRepository.GetByIds(formIds);
        var signupForms = forms.ToList();

        var formSummaryDetails = new List<FormSubmissionsDataDto>();

        foreach (var form in signupForms)
        {
            var signups = await _emailSignupRepository.GetByFormIdAsync(form.Id);

            formSummaryDetails.Add(new FormSubmissionsDataDto
            {
                Id = form.Id,
                FormName = form.FormName,
                Emails = signups.Select(s=>s.EmailAddress).ToList(),
            });
        }

        return formSummaryDetails;
    }
}