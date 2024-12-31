using System.Transactions;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories.DAL;

public class FormsDAL : IFormsDAL
{
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly IRepository<SmtpEmailSettings> _smtpEmailSettingsRepository;
    private readonly IRepository<FormCorsSettings> _formCorsSettingsRepository;
    private readonly IRepository<RecaptchaFormSettings> _recaptchaFormSettingsRepository;
    private IFormRelatedRepository<CustomEmailTemplate> _customEmailTemplatesRepository;
    private readonly ILogger<FormsDAL> _logger;


    public FormsDAL(ISignupFormRepository signupFormRepository,
        IRepository<SmtpEmailSettings> smtpEmailSettingsRepository,
        IRepository<FormCorsSettings> formCorsSettingsRepository,
        IRepository<RecaptchaFormSettings> recaptchaFormSettingsRepository,
        IFormRelatedRepository<CustomEmailTemplate> customEmailTemplatesRepository,
        ILogger<FormsDAL> logger)
    {
        _signupFormRepository = signupFormRepository;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
        _formCorsSettingsRepository = formCorsSettingsRepository;
        _recaptchaFormSettingsRepository = recaptchaFormSettingsRepository;
        _customEmailTemplatesRepository = customEmailTemplatesRepository;
        _logger = logger;
    }
    
    /// <inheritdoc cref="IFormsDAL.SaveFormWithTransaction"/>
    public async Task SaveFormWithTransaction(SignupForm form)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            await _signupFormRepository.Upsert(form);
            await _formCorsSettingsRepository.Upsert(form.FormCorsSettings);
            await _recaptchaFormSettingsRepository.Upsert(form.RecaptchaSettings);
            foreach (var formCustomEmailTemplate in form.CustomEmailTemplates)
            {
                await _customEmailTemplatesRepository.Upsert(formCustomEmailTemplate);
            }
            if (form.FormEmailSettings is SmtpEmailSettings smtpEmailSettings)
            {
                await _smtpEmailSettingsRepository.Upsert(smtpEmailSettings);
            }

            transactionScope.Complete();
        }
        catch
        {
            _logger.LogError($"Failed to save form {form.Id} with transaction");
            throw;
        }
    }
}