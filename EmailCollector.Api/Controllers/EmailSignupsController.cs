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

    /// <summary>
    /// Get email signups for a specific form.
    /// </summary>
    /// <param name="formId">Form id to get email signups for.</param>
    /// <returns>All emails signups for formId.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/EmailSignups/form/5
    ///     
    /// </remarks>
    /// <response code="200">Returns all email signups for the form.</response>
    /// <response code="400">If the user is not authenticated.</response>
    /// <response code="404">If the form is not found.</response>
    [HttpGet("form/{formId}"), Authorize]
    [Produces("application/json")]
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

    /// <summary>
    /// Signup for an email form.
    /// </summary>
    /// <param name="emailSignup">Email signup definition</param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/EmailSignups
    ///     {
    ///         "FormId": "5",
    ///         "Email": "example@email.com"
    ///     }
    /// </remarks>
    /// <response code="200">Email signup to form was successful</response>
    /// <response code="400">If the email is invalid</response>
    /// <response code="404">If the form is not found</response>
    /// <response code="409">If the form is not active</response>
    [HttpPost]
    [Produces("application/json")]
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
