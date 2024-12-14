using System.ComponentModel.DataAnnotations;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.CustomEmailTemplates;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmailCollector.Api.Pages.CustomEmailTemplates;

public class Create : PageModel
{
    private readonly ICustomEmailTemplatesService _emailTemplatesService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;
    private readonly IFormService _formService;


    public Create(ICustomEmailTemplatesService emailTemplatesService,
        UserManager<EmailCollectorApiUser> userManager,
        IFormService formService)
    {
        _emailTemplatesService = emailTemplatesService;
        _userManager = userManager;
        _formService = formService;
    }

    [BindProperty]
    public CreateCustomEmailTemplateViewModel CustomEmailTemplate { get; set; }

    public IEnumerable<SelectListItem> EventOptions { get; set; }
    
    public IEnumerable<SelectListItem> FormOptions { get; set; }

    public async Task OnGetAsync()
    {
        PopulateEventOptions();
        await PopulateFormOptions();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        PopulateEventOptions();
        await PopulateFormOptions();
        
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var templateToSave = new CustomEmailTemplateDto
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            FormId = CustomEmailTemplate.FormId,
            Event = CustomEmailTemplate.Event,
            TemplateSubject = CustomEmailTemplate.TemplateSubject,
            TemplateBody = CustomEmailTemplate.TemplateBody,
        };

        var existingTemplate = await _emailTemplatesService.GetCustomEmailTemplateByFormIdAndEvent(
            CustomEmailTemplate.FormId,
            CustomEmailTemplate.Event);

        if (existingTemplate != null)
        {
            var formName = FormOptions
                .Where(f => f.Value == existingTemplate.FormId.ToString())
                .Select(f => f.Text).FirstOrDefault();
            
            ModelState.AddModelError("TemplateExists", $"Custom email template already exists for form {formName} and event {CustomEmailTemplate.Event}");
            return Page();
        }
        
        await _emailTemplatesService.SaveCustomEmailTemplate(templateToSave);

        return RedirectToPage("./Index");
    }

    private void PopulateEventOptions()
    {
        EventOptions = Enum.GetValues(typeof(TemplateEvent))
            .Cast<TemplateEvent>()
            .Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.ToString().Replace('_', ' ')
            });
    }

    private async Task PopulateFormOptions()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        var forms = await _formService.GetFormsByUserAsync(userId);
        FormOptions = forms.Select(f => new SelectListItem
        {
            Value = f.Id.ToString(),
            Text = f.FormName
        });
    }
    
    public class CreateCustomEmailTemplateViewModel
    {
        [Required(ErrorMessage = "Event is required.")]
        public TemplateEvent Event { get; set; }

        [Required(ErrorMessage = "Form is required.")]
        public Guid FormId { get; set; }

        [Required(ErrorMessage = "Template Subject is required.")]
        [StringLength(100, ErrorMessage = "Template Subject cannot exceed 100 characters.")]
        public string TemplateSubject { get; set; }

        [Required(ErrorMessage = "Template Body is required.")]
        public string TemplateBody { get; set; }
    }
}
