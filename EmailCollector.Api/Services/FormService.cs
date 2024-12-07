using Blazorise.Extensions;
using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;
using EmailCollector.Api.Repositories;
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
    private readonly ISmtpEmailSettingsRepository _smtpEmailSettingsRepository;
    private readonly IFormCorsSettingsRepository _formCorsSettingsRepository;
    private readonly IRepository<RecaptchaFormSettings> _recaptchaFormSettingsRepository;
    private readonly IExportService _exportService;
    private readonly ILogger<FormService> _logger;

    public FormService(ISignupFormRepository signupFormRepository,
        IEmailSignupRepository emailSignupRepository,
        ISmtpEmailSettingsRepository smtpEmailSettingsRepository,
        IFormCorsSettingsRepository formCorsSettingsRepository,
        IRepository<RecaptchaFormSettings> recaptchaFormSettingsRepository,
        IExportService exportService,
        ILogger<FormService> logger)
    {
        _signupFormRepository = signupFormRepository;
        _emailSignupRepository = emailSignupRepository;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
        _formCorsSettingsRepository = formCorsSettingsRepository;
        _recaptchaFormSettingsRepository = recaptchaFormSettingsRepository;
        _exportService = exportService;
        _logger = logger;
    }

    public async Task<FormDetailsDto> CreateFormAsync(Guid userId, CreateFormDto createFormDto)
    {
        var signupForm = new SignupForm
        {
            FormName = createFormDto.FormName,
            CreatedBy = userId,
            Status = createFormDto.Status,
        };

        await _signupFormRepository.AddAsync(signupForm);

        _logger.LogInformation($"Created form {signupForm.Id} for user {userId}.");

        return new FormDetailsDto
        {
            Id = signupForm.Id,
            FormName = signupForm.FormName,
            Status = signupForm.Status,
        };
    }
    
    public async Task<FormDetailsDto> CreateFormAsync(Guid userId, ExtendedCreateFormDto extendedCreateFormDto)
    {
        var form = new SignupForm
        {
            FormName = extendedCreateFormDto.FormName,
            CreatedBy = userId,
            Status = extendedCreateFormDto.Status,
        };
        await _signupFormRepository.AddAsync(form);

        if (!extendedCreateFormDto.EmailFrom.IsNullOrEmpty() &&
            !extendedCreateFormDto.SmtpServer.IsNullOrEmpty() &&
            extendedCreateFormDto.SmtpPort.HasValue &&
            !extendedCreateFormDto.SmtpUsername.IsNullOrEmpty() &&
            !extendedCreateFormDto.SmtpPassword.IsNullOrEmpty())
        {
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
            
            await _smtpEmailSettingsRepository.AddAsync(smtpConfig);
        }

        if (!extendedCreateFormDto.AllowedOrigins.IsNullOrEmpty())
        {
            var corsConfig = new FormCorsSettings
            {
                Form = form,
                FormId = form.Id,
                AllowedOrigins = extendedCreateFormDto.AllowedOrigins!,
            };
            
            await _formCorsSettingsRepository.AddAsync(corsConfig);
        }

        if (!extendedCreateFormDto.CaptchaSiteKey.IsNullOrEmpty() &&
            !extendedCreateFormDto.CaptchaSecretKey.IsNullOrEmpty())
        {
            var recaptchaConfig = new RecaptchaFormSettings
            {
                Form = form,
                FormId = form.Id,
                SiteKey = extendedCreateFormDto.CaptchaSiteKey!,
                SecretKey = extendedCreateFormDto.CaptchaSecretKey!,
            };
            
            await _recaptchaFormSettingsRepository.AddAsync(recaptchaConfig);
        }
        
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

        _logger.LogInformation($"Found {forms.Count()} forms for user {userId}.");

        return forms.Select(form => new FormDto
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

    public async Task<FormDetailsDto?> UpdateFormAsync(Guid formId, Guid userId, CreateFormDto createFormDto)
    {
        var form = await _signupFormRepository.GetByFormIdentifierAsync(formId, userId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return null;
        }

        form.FormName = createFormDto.FormName;
        form.Status = createFormDto.Status;

        await _signupFormRepository.Update(form);

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
        _logger.LogInformation($"Found {signupForms.Count()} forms for user {userId}.");

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
        if (formData.IsNullOrEmpty())
        {
            _logger.LogInformation($"No available forms to export.");
            return [];
        }
        return await _exportService.ExportAsync(formData, format);
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