using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.CustomEmailTemplates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.CustomEmailTemplates;

public class Delete : PageModel
{
    private readonly ICustomEmailTemplatesService _emailTemplatesService;

    public Delete(ICustomEmailTemplatesService emailTemplatesService)
    {
        _emailTemplatesService = emailTemplatesService;
    }

    [BindProperty]
    public CustomEmailTemplateDto CustomEmailTemplate { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        CustomEmailTemplate = await _emailTemplatesService.GetCustomEmailTemplateById(id);

        if (CustomEmailTemplate == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var template = await _emailTemplatesService.GetCustomEmailTemplateById(CustomEmailTemplate.Id);

        if (template == null)
        {
            return NotFound();
        }

        await _emailTemplatesService.DeleteCustomEmailTemplate(template.Id);

        return RedirectToPage("./Index");
    }
}
