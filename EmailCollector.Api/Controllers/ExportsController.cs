using System.Security.Claims;
using EmailCollector.Api.Authentication;
using EmailCollector.Api.Controllers.RouteConsts;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.Exports;
using Microsoft.AspNetCore.Mvc;

namespace EmailCollector.Api.Controllers;

[Route(Routes.ExportsControllerBase)]
public class ExportsController : ControllerBase
{
    private readonly IFormService _formService;
    private readonly ILogger<ExportsController> _logger;

    public ExportsController(IFormService formService, ILogger<ExportsController> logger)
    {
        _formService = formService;
        _logger = logger;
    }
    
    /// <summary>
    /// Export all user's forms.
    /// </summary>
    /// <param name="exportFormat">export format</param>
    /// <returns>file containing the exported data</returns>
    /// <exception cref="ArgumentException"></exception>
    [HttpGet(Routes.Export)]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> ExportForms([FromQuery] ExportFormat exportFormat = ExportFormat.Csv)
    {
        _logger.LogInformation($"Exporting forms.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }
        var forms = await _formService.GetFormsByUserAsync(userId);
        var formIds = forms.Select(f => f.Id).ToList();
        
        var fileBytes = await _formService.ExportFormsAsync(formIds, exportFormat);
        if (fileBytes == null || fileBytes.Length == 0)
        {
            _logger.LogInformation("No forms were available for export or result was empty.");
            return NotFound("No forms found to export.");
        }

        const string contentType = "application/octet-stream";
        var fileName = exportFormat switch
        {
            ExportFormat.Csv => "collecto_export.csv",
            ExportFormat.Json => "collecto_export.xlsx",
            // should not get there, since ExportFormat is type validated OOTB.
            _ => throw new ArgumentException("Unsupported export format")
        };

        return File(fileBytes, contentType, fileName);
    }

    /// <summary>
    /// Exports a single form.
    /// </summary>
    /// <param name="formId">form to export</param>
    /// <param name="exportFormat">export format</param>
    /// <returns>file containing the exported data</returns>
    /// <exception cref="ArgumentException"></exception>
    [HttpGet("{formId:guid}" + Routes.Export)]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> ExportForm(Guid formId, [FromQuery] ExportFormat exportFormat = ExportFormat.Csv)
    {
        _logger.LogInformation($"Exporting form {formId}.");
        
        var fileBytes = await _formService.ExportFormsAsync([formId], exportFormat);
        if (fileBytes == null || fileBytes.Length == 0)
        {
            _logger.LogInformation("No forms were available for export or result was empty.");
            return NotFound("No forms found to export.");
        }

        const string contentType = "application/octet-stream";
        var fileName = exportFormat switch
        {
            ExportFormat.Csv => "collecto_export.csv",
            ExportFormat.Json => "collecto_export.xlsx",
            // should not get there, since ExportFormat is type validated OOTB.
            _ => throw new ArgumentException("Unsupported export format")
        };

        return File(fileBytes, contentType, fileName);
    }
}