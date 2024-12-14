using System.ComponentModel.DataAnnotations;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.CustomEmailTemplates;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmailCollector.Api.Pages.CustomEmailTemplates;

public class Edit : PageModel
{
    private readonly ICustomEmailTemplatesService _emailTemplatesService;
    private readonly IFormService _formService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;


    public Edit(ICustomEmailTemplatesService emailTemplatesService,
        IFormService formService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _emailTemplatesService = emailTemplatesService;
        _formService = formService;
        _userManager = userManager;
    }

    [BindProperty]
    public EditCustomEmailTemplateViewModel CustomEmailTemplate { get; set; }

    public IEnumerable<SelectListItem> FormOptions { get; set; }
    public IEnumerable<SelectListItem> EventOptions { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        // Fetch the template
        var customEmailTemplateDto = await _emailTemplatesService.GetCustomEmailTemplateById(id);
        
        if (customEmailTemplateDto == null)
        {
            return NotFound();
        }

        CustomEmailTemplate = new EditCustomEmailTemplateViewModel
        {
            FormId = customEmailTemplateDto.FormId,
            Event = customEmailTemplateDto.Event,
            TemplateBody = customEmailTemplateDto.TemplateBody,
            TemplateSubject = customEmailTemplateDto.TemplateSubject,
            IsActive = customEmailTemplateDto.IsActive
        };

        // Verify that the template's FormId belongs to the current user
        var userForms = await _formService.GetFormsByUserAsync(userId);
        var userFormIds = userForms.Select(f => f.Id).ToHashSet();

        if (!userFormIds.Contains(CustomEmailTemplate.FormId))
        {
            return Forbid();
        }

        // Populate FormOptions (disabled in UI)
        FormOptions = userForms.Select(f => new SelectListItem
        {
            Value = f.Id.ToString(),
            Text = f.FormName,
            Selected = f.Id == CustomEmailTemplate.FormId
        });

        // Populate event options
        PopulateEventOptions();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        // Fetch the existing template
        var existingTemplate = await _emailTemplatesService.GetCustomEmailTemplateById(id);
        if (existingTemplate == null)
        {
            return NotFound();
        }

        // Verify that the template's FormId belongs to the current user
        var userForms = await _formService.GetFormsByUserAsync(userId);
        var userFormIds = userForms.Select(f => f.Id).ToHashSet();

        if (!userFormIds.Contains(existingTemplate.FormId))
        {
            return Forbid();
        }

        // Since FormId is not editable, ensure it's not altered
        CustomEmailTemplate.FormId = existingTemplate.FormId;

        // Sanitize HTML content
        var sanitizer = new HtmlSanitizer();
        CustomEmailTemplate.TemplateBody = sanitizer.Sanitize(CustomEmailTemplate.TemplateBody);

        // Update the template DTO
        var updatedDto = new CustomEmailTemplateDto
        {
            Id = existingTemplate.Id,
            FormId = existingTemplate.FormId,
            Event = CustomEmailTemplate.Event,
            TemplateSubject = CustomEmailTemplate.TemplateSubject,
            TemplateBody = CustomEmailTemplate.TemplateBody,
            TemplateBodyUri = existingTemplate.TemplateBodyUri, // Assuming URI remains same
            IsActive = CustomEmailTemplate.IsActive,
            CreatedAt = existingTemplate.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        Console.WriteLine("saving");
        await _emailTemplatesService.SaveCustomEmailTemplate(updatedDto);
        Console.WriteLine("saved");
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
    
    public class EditCustomEmailTemplateViewModel
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
        
        public bool IsActive { get; set; }
    }
}