using EmailCollector.Api.Authentication;
using EmailCollector.Api.Controllers.RouteConsts;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.CustomEmailTemplates;
using EmailCollector.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EmailCollector.Api.Controllers;

[Route(Routes.CustomTemplatesControllerBase)]
[ApiController]
public class CustomTemplatesController : ControllerBase
{
    private readonly ILogger<CustomTemplatesController> _logger;
    private readonly CustomEmailTemplatesService _customEmailTemplatesService;

    public CustomTemplatesController(ILogger<CustomTemplatesController> logger,
        CustomEmailTemplatesService customEmailTemplatesService)
    {
        _logger = logger;
        _customEmailTemplatesService = customEmailTemplatesService;
    }

    [HttpGet("{formId:guid}" + Routes.Templates)]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> GetCustomTemplatesByFormId(Guid formId, [FromQuery] TemplateEvent? templateEvent = null)
    {
        _logger.LogInformation($"Getting custom template for form {formId}.");
        
        return templateEvent == null ? 
            Ok(await _customEmailTemplatesService.GetCustomEmailTemplatesByFormId(formId)) : 
            Ok(await _customEmailTemplatesService.GetCustomEmailTemplateByFormIdAndEvent(formId, (TemplateEvent)templateEvent));
    }
    
    [HttpGet(Routes.Templates + "/{templateId:guid}")]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> GetCustomTemplatesByTemplateId(Guid templateId)
    {
        _logger.LogInformation($"Getting custom template with id {templateId}.");
        
        return Ok(await _customEmailTemplatesService.GetCustomEmailTemplateById(templateId));
    }

    [HttpPost(Routes.Templates)]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<ActionResult> PostCustomTemplate(CustomEmailTemplateDto templateDto)
    {
        _logger.LogInformation("Creating custom template.");

        var templateId = await _customEmailTemplatesService.SaveCustomEmailTemplate(templateDto);
        return CreatedAtAction("GetCustomTemplatesByTemplateId", new { id = templateId }, templateDto);
    }
    
    [HttpDelete(Routes.Templates + "/{templateId:guid}")]
    [Produces("application/json")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<ActionResult> DeleteCustomTemplate(Guid templateId)
    {
        _logger.LogInformation("Creating custom template.");

        await _customEmailTemplatesService.DeleteCustomEmailTemplate(templateId);
        return Ok();
    }
}