using Blazorise;
using Duende.IdentityServer.Models;
using EmailCollector.Api.Areas.Identity.Data;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace EmailCollector.Api.Pages;

public class ConfigurationsPageModel : PageModel
{
    private readonly IFormService _formService;
    private readonly ISmtpEmailSettingsRepository _smtpEmailSettingsRepository;
    private readonly IFormCorsSettingsRepository _formCorsSettingsRepository;
    private readonly SignInManager<EmailCollectorApiUser> _signInManager;
    private readonly UserManager<EmailCollectorApiUser> _userManager;

    public ConfigurationsPageModel(IFormService formService,
        ISmtpEmailSettingsRepository smtpEmailSettingsRepository,
        IFormCorsSettingsRepository formCorsSettingsRepository,
        SignInManager<EmailCollectorApiUser> signInManager,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _formService = formService;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
        _formCorsSettingsRepository = formCorsSettingsRepository;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public int? SelectedFormId { get; set; }

    public IEnumerable<FormDto> Forms { get; set; } = new List<FormDto>();
    
    [BindProperty]
    public SmtpEmailSettings SmtpEmailSettings { get; set; }

    [BindProperty]
    public FormCorsSettings CorsSettings { get; set; }
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

        Forms = await _formService.GetFormsByUserAsync(userId);

        if (action == "load" && SelectedFormId.HasValue)
        {
            // Load the configurations for the selected form
            SmtpEmailSettings = await _smtpEmailSettingsRepository.GetByIdAsync(SelectedFormId) ?? new SmtpEmailSettings();
            CorsSettings = await _formCorsSettingsRepository.GetByIdAsync(SelectedFormId) ?? new FormCorsSettings();
        }

        else if (action == "save" && SelectedFormId.HasValue)
        {
            // Save the email settings
            var existingEmailSettings = await _smtpEmailSettingsRepository.GetByIdAsync(SelectedFormId.Value);
            if (existingEmailSettings == null)
            {
                // Create new settings
                await _smtpEmailSettingsRepository.AddAsync(new SmtpEmailSettings
                {
                    FormId = SelectedFormId.Value,
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
            var existingCorsSettings = await _formCorsSettingsRepository.GetByIdAsync(SelectedFormId.Value);
            if (existingCorsSettings == null)
            {
                // Create new settings
                await _formCorsSettingsRepository.AddAsync(new FormCorsSettings
                {
                    FormId = SelectedFormId.Value,
                    AllowedOrigins = CorsSettings.AllowedOrigins,
                });
            }
            else
            {
                // Update existing settings
                existingCorsSettings.AllowedOrigins = CorsSettings.AllowedOrigins;

                await _formCorsSettingsRepository.Update(existingCorsSettings);
            }
        }

        return Page();
    }
}

