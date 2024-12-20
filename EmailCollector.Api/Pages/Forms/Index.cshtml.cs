using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.Exports;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.Forms;

public class Index : PageModel
{
    private readonly UserManager<EmailCollectorApiUser> _userManager;
    private readonly IFormService _formService;
    
    public IEnumerable<FormSummaryDetailsDto> Forms { get; set; } = new List<FormSummaryDetailsDto>();
    public Guid UserId { get; set; }
    
    [BindProperty]
    public List<KeyValuePair<Guid, bool>> FormCheckboxes { get; set; } = [];

    [BindProperty]
    public ExportFormat Format { get; set; } = ExportFormat.Csv;

    
    public Index(UserManager<EmailCollectorApiUser> userManager,
        IFormService formService)
    {
        _userManager = userManager;
        _formService = formService;
    }
    
    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        UserId = new Guid(currentUser?.Id!);
        Forms = await _formService.GetFormsSummaryDetailsAsync(UserId);
        
        FormCheckboxes = Forms
            .Select(form => new KeyValuePair<Guid, bool>(form.Id, false))
            .ToList();
    }
    
    public async Task<FileResult> OnPostExportAsync()
    {
        var selectedFormIds = FormCheckboxes
            .Where(kvp => kvp.Value) // Only take selected (checked) forms
            .Select(kvp => kvp.Key)
            .ToList();
        
        var exportedFile = await _formService.ExportFormsAsync(selectedFormIds, Format);

        var contentType = Format switch
        {
            ExportFormat.Csv => "text/csv",
            ExportFormat.Json => "application/json",
            _ => "application/octet-stream"
        };

        var fileName = $"collecto_export.{Format.ToString().ToLower()}";
        Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
        return File(exportedFile, contentType);
    }
}