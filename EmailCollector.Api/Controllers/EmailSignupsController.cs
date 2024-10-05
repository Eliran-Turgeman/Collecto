using Microsoft.AspNetCore.Mvc;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EmailCollector.Api.Interfaces;
using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailSignupsController : ControllerBase
{
    private readonly IEmailSignupService _emailSignupService;
    private readonly ILogger<EmailSignupsController> _logger;

    public EmailSignupsController(IEmailSignupService emailSignupService,
        ILogger<EmailSignupsController> logger)
    {
        _emailSignupService = emailSignupService;
        _logger = logger;
    }

    // GET: api/FormEmailSignups/5
    [HttpGet("form/{formId}"), Authorize]
    public async Task<ActionResult<IEnumerable<EmailSignupDto>>> GetFormEmailSignups(int formId)
    {
        _logger.LogInformation($"Getting email signups for form {formId}.");
        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            _logger.LogError("Invalid user ID.");
            return BadRequest();
        }

        var emailSignups = await _emailSignupService.GetSignupsByFormIdAsync(formId, userId);
        if (emailSignups == null)
        {
            return NotFound("Form not found.");
        }

        _logger.LogInformation($"Found {emailSignups.Count()} signups for form {formId}.");

        return Ok(emailSignups);
    }

    // POST: api/EmailSignups
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<EmailSignup>> PostEmailSignup(EmailSignupDto emailSignup)
    {
        _logger.LogInformation($"Submitting email signup for form {emailSignup.FormId}.");
        var result = await _emailSignupService.SubmitEmailAsync(emailSignup);

        if (!result.Success)
        {
            switch (result.ErrorCode)
            {
                case EmailSignupErrorCode.InvalidEmail:
                    return BadRequest(result.Message);

                case EmailSignupErrorCode.FormNotFound:
                    return NotFound(result.Message);

                case EmailSignupErrorCode.FormNotActive:
                    return Conflict(result.Message);
            } 
        }

        _logger.LogInformation($"Email signup submitted for form {emailSignup.FormId}.");
        return Ok(emailSignup);
    }
}
