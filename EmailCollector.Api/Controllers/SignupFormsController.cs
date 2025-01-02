using System.Security.Claims;
using EmailCollector.Api.Authentication;
using Microsoft.AspNetCore.Mvc;
using EmailCollector.Domain.Entities;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.Exports;

namespace EmailCollector.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignupFormsController : ControllerBase
{
    private readonly IFormService _formService;
    private readonly ILogger<SignupFormsController> _logger;

    public SignupFormsController(IFormService formService, ILogger<SignupFormsController> logger)
    {
        _formService = formService;
        _logger = logger;
    }

    /// <summary>
    /// Get all signup forms for the current user.
    /// </summary>
    /// <returns>All signup forms the current user has created.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     Get /api/SignupForms
    /// 
    /// </remarks>
    /// <response code="200">Returns all signup forms the current user has created.</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<ActionResult<IEnumerable<FormDto>>> GetSignupForms()
    {
        _logger.LogInformation("Getting signup forms.");
        
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }
        
        return Ok(await _formService.GetFormsByUserAsync(userId));
    }

    /// <summary>
    /// Get a specific signup form.
    /// </summary>
    /// <param name="id">Signup form id</param>
    /// <returns>Details of the signup form matching the id.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     Get /api/SignupForms/5
    /// </remarks>
    /// <response code="200">Returns the signup form matching the id.</response>
    /// <response code="404">If the signup form is not found.</response>
    /// <response code="400">If the user is not authenticated.</response>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<ActionResult<SignupForm>> GetSignupForm(Guid id)
    {
        _logger.LogInformation($"Getting signup form {id}.");
        
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }

        var signupForm = await _formService.GetFormByIdAsync(id, userId);
        if (signupForm == null)
        {
            return NotFound();
        }

        _logger.LogInformation($"Found signup form {id}.");

        return Ok(signupForm);
    }

    /// <summary>
    /// Creates a new signup form.
    /// </summary>
    /// <param name="signupForm">Definition of the form.</param>
    /// <returns>Newly created form.</returns>
    /// <remarks>
    /// Sample Requests:
    /// 
    /// Create a new form:
    /// 
    ///     POST /api/SignupForms
    ///     {
    ///         "FormName": "My Form"
    ///     }
    ///     
    /// Create a new inactive form:
    ///     
    ///     POST /api/SignupForms
    ///     {
    ///         "FormName": "My Form",
    ///         "Status": "Inactive"
    ///     }
    /// </remarks>
    /// <response code="201">Returns the newly created form.</response>
    /// <response code="400">If the user is not authenticated.</response>
    [HttpPost]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<ActionResult<FormDetailsDto>> PostSignupForm(ExtendedCreateFormDto signupForm)
    {
        _logger.LogInformation("Creating signup form.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }

        var formsDetails = await _formService.CreateFormAsync(userId, signupForm);
        _logger.LogInformation($"Created signup form {formsDetails.Id}.");
        return CreatedAtAction("GetSignupForm", new { id = formsDetails.Id }, formsDetails);
    }

    /// <summary>
    /// Deletes a signup form.
    /// </summary>
    /// <param name="id">Id of form to delete.</param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/SignupForms/5
    ///     
    /// </remarks>
    /// <response code="204">If the form is deleted successfully.</response>
    /// <response code="404">If the form is not found.</response>
    /// <response code="400">If the user is not authenticated.</response>
    [HttpDelete("{id}")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> DeleteSignupForm(Guid id)
    {
        _logger.LogInformation($"Deleting signup form {id}.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }

        if (await _formService.GetFormByIdAsync(id, userId) == null)
        {
            return NotFound();
        }

        await _formService.GetFormByIdAsync(id, userId);

        _logger.LogInformation($"Deleted signup form {id}.");

        return NoContent();
    }

    /// <summary>
    /// Updates a signup form.
    /// </summary>
    /// <param name="id">Id of a form to update</param>
    /// <param name="signupForm">Definition of the form.</param>
    /// <returns></returns>
    /// <remarks>
    /// Sample requests:
    /// 
    /// Updating all info:
    /// 
    ///     PUT /api/SignupForms/5
    ///     {
    ///         "FormName": "Updated Form Name",
    ///         "Status": "Inactive"
    ///     }
    ///     
    /// Updating only status:
    /// 
    ///     PUT /api/SignupForms/5
    ///     {
    ///         "Status": "Active"
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> PutSignupForm(Guid id, ExtendedCreateFormDto signupForm)
    {
        _logger.LogInformation($"Updating signup form {id}.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }

        if (await _formService.GetFormByIdAsync(id, userId) == null)
        {
            return NotFound();
        }

        await _formService.UpdateFormAsync(id, userId, signupForm);

        _logger.LogInformation($"Updated signup form {id}.");

        return NoContent();
    }

    /// <summary>
    /// Export all user's forms.
    /// </summary>
    /// <param name="exportFormat">export format</param>
    /// <returns>file containing the exported data</returns>
    /// <exception cref="ArgumentException"></exception>
    [HttpGet("export")]
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
    [HttpGet("{id}/export")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> ExportForm(Guid formId, [FromQuery] ExportFormat exportFormat = ExportFormat.Csv)
    {
        _logger.LogInformation($"Exporting form {formId}.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier");
        }
        
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
