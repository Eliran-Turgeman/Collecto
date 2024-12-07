using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages;

public class ConfigurationsPageModel : PageModel
{
    private readonly IFormService _formService;
    private readonly ISmtpEmailSettingsRepository _smtpEmailSettingsRepository;
    private readonly IFormCorsSettingsRepository _formCorsSettingsRepository;
    private readonly IRepository<RecaptchaFormSettings> _recaptchaSettingsRepository;
    private readonly SignInManager<EmailCollectorApiUser> _signInManager;
    private readonly UserManager<EmailCollectorApiUser> _userManager;

    public ConfigurationsPageModel(IFormService formService,
        ISmtpEmailSettingsRepository smtpEmailSettingsRepository,
        IFormCorsSettingsRepository formCorsSettingsRepository,
        IRepository<RecaptchaFormSettings> recaptchaSettingsRepository,
        SignInManager<EmailCollectorApiUser> signInManager,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _formService = formService;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
        _formCorsSettingsRepository = formCorsSettingsRepository;
        _recaptchaSettingsRepository = recaptchaSettingsRepository;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public Guid? FormId { get; set; }

    public IEnumerable<FormDto> Forms { get; set; } = new List<FormDto>();
    
    [BindProperty]
    public SmtpEmailSettings SmtpEmailSettings { get; set; }

    [BindProperty]
    public FormCorsSettings CorsSettings { get; set; }
    
    [BindProperty]
    public RecaptchaFormSettings RecaptchaFormSettings { get; set; }
    
    public string ErrorMessage { get; set; } = string.Empty;


    public async Task<IActionResult> OnGetAsync()
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ErrorMessage = "Please log in to view your forms dashboard.";
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsByUserAsync(userId);

        await LoadSettingsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ErrorMessage = "Please log in to view your forms dashboard.";
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);
        var formId = Guid.Parse(FormId.Value.ToString());
        Forms = await _formService.GetFormsByUserAsync(userId);

        if (action == "load" && FormId.HasValue)
        {
            return RedirectToPage(new { formId = FormId });
        }

        else if (action == "save" && FormId.HasValue)
        {
            // Save the email settings
            var existingEmailSettings = await _smtpEmailSettingsRepository.GetByIdAsync(FormId.Value);
            if (existingEmailSettings == null)
            {
                // Create new settings
                await _smtpEmailSettingsRepository.AddAsync(new SmtpEmailSettings
                {
                    FormId = formId,
                    EmailMethod = EmailMethod.Smtp,
                    EmailFrom = SmtpEmailSettings.EmailFrom,
                    SmtpServer = SmtpEmailSettings.SmtpServer,
                    SmtpPort = SmtpEmailSettings.SmtpPort,
                    SmtpUsername = SmtpEmailSettings.SmtpUsername,
                    SmtpPassword = SmtpEmailSettings.SmtpPassword,
                });
            }
            else
            {
                // Update existing settings
                existingEmailSettings.EmailFrom = SmtpEmailSettings.EmailFrom;
                existingEmailSettings.SmtpServer = SmtpEmailSettings.SmtpServer;
                existingEmailSettings.SmtpPort = SmtpEmailSettings.SmtpPort;
                existingEmailSettings.SmtpUsername = SmtpEmailSettings.SmtpUsername;
                existingEmailSettings.SmtpPassword = SmtpEmailSettings.SmtpPassword;

                await _smtpEmailSettingsRepository.Update(existingEmailSettings);
            }

            // Save the CORS settings
            var existingCorsSettings = await _formCorsSettingsRepository.GetByIdAsync(FormId.Value);
            if (existingCorsSettings == null)
            {
                // Create new settings
                await _formCorsSettingsRepository.AddAsync(new FormCorsSettings
                {
                    FormId = formId,
                    AllowedOrigins = CorsSettings.AllowedOrigins,
                });
            }
            else
            {
                // Update existing settings
                existingCorsSettings.AllowedOrigins = CorsSettings.AllowedOrigins;

                await _formCorsSettingsRepository.Update(existingCorsSettings);
            }
            
            var existingRecaptchaSettings = await _recaptchaSettingsRepository.GetByIdAsync(FormId.Value);
            if (existingRecaptchaSettings == null)
            {
                // Create new settings
                await _recaptchaSettingsRepository.AddAsync(new RecaptchaFormSettings
                {
                    FormId = formId,
                    SiteKey = RecaptchaFormSettings.SiteKey,
                    SecretKey = RecaptchaFormSettings.SecretKey
                });
            }
            else
            {
                // Update existing settings
                existingRecaptchaSettings.SiteKey = RecaptchaFormSettings.SiteKey;
                existingRecaptchaSettings.SecretKey = RecaptchaFormSettings.SecretKey;
                
                await _recaptchaSettingsRepository.Update(existingRecaptchaSettings);
            }
        }

        return Page();
    }

    private async Task LoadSettingsAsync()
    {
        if (FormId.HasValue)
        {
            SmtpEmailSettings = await _smtpEmailSettingsRepository.GetByIdAsync(FormId) ?? new SmtpEmailSettings();
            CorsSettings = await _formCorsSettingsRepository.GetByIdAsync(FormId) ?? new FormCorsSettings();
            RecaptchaFormSettings = await _recaptchaSettingsRepository.GetByIdAsync(FormId) ?? new RecaptchaFormSettings();
        }
    }
}

