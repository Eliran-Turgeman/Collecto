using EmailCollector.Api.Authentication;
using EmailCollector.Api.Controllers.RouteConsts;
using Microsoft.AspNetCore.Mvc;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Middlewares;
using EmailCollector.Domain.Enums;
using EmailCollector.Api.Services;
using Microsoft.AspNetCore.Cors;

namespace EmailCollector.Api.Controllers;

[Route(Routes.EmailSignupsControllerBase)]
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
    /// Submit a signup for an email form - depending on your email confirmation settings,
    /// user might need to confirm the signup first before it is registered.
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
    /// <response code="409">If the form is not active, or the email address is already signed up for this form.</response>
    /// <response code="429">If API calls quota exceeded - 10 calls per 1min</response>
    [EnableCors("AllowSpecificOrigin")]
    [HttpPost]
    [Produces("application/json")]
    [ServiceFilter(typeof(AllowedOriginsFilter))]
    public async Task<ActionResult<EmailSignup>> PostEmailSignup([FromBody] EmailSignupDto emailSignup)
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

                case EmailSignupErrorCode.EmailAlreadySignedUp:
                case EmailSignupErrorCode.FormNotActive:
                    return Conflict(result.Message);
            } 
        }

        _logger.LogInformation($"Email signup submitted for form {emailSignup.FormId}.");
        return Ok(result.Message);
    }
    
    /// <summary>
    /// This endpoint handles the confirmation of an email signup.
    /// When an email is sumbitted to PostEmailSignup, we send a confirmation email.
    /// The confirmation email contains a button that redirects to this endpoint with a valid confirmation token.
    /// Once this endpoint identifies the confirmation token in the redis cache, we sign up the associated email
    /// address to the correct form.
    /// </summary>
    ///     /// <param name="confirmationToken">confirmation token.</param>
    /// <returns></returns>
    /// /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/EmailSignups/confirmations?confirmationToken=abcdefg
    /// 
    /// </remarks>
    /// <response code="200">Confirmation token was valid, and email is added successfully</response>
    /// <response code="400">Confirmation token is invalid or expired</response>
    [AllowAnonymous]
    [HttpGet(Routes.Confirmations)]
    public async Task<IActionResult> ConfirmEmailSignup([FromQuery] string confirmationToken)
    {
        var confirmationResult = await _emailSignupService.ConfirmEmailSignupAsync(confirmationToken);

        switch (confirmationResult.ErrorCode)
        {
            case EmailConfirmationErrorCode.InvalidToken:
                return Problem(type: "Bad Request",
                    title: confirmationResult.Message,
                    detail: "Invalid token, make sure you call PostEmailSignup with the email address that needs to be signed up.",
                    statusCode: StatusCodes.Status400BadRequest);
            
            case EmailConfirmationErrorCode.ExpiredToken:
                return Problem(type: "Bad Request",
                    title: confirmationResult.Message,
                    detail: "The confirmation token is expired, please sign up to the form again.",
                    statusCode: StatusCodes.Status400BadRequest);

            case EmailConfirmationErrorCode.EmailAlreadyConfirmed:
                return Problem(type: "Conflict",
                    title: confirmationResult.Message,
                    detail: "The email address has already been confirmed.",
                    statusCode: StatusCodes.Status409Conflict);
        }

        return Ok(confirmationResult.Message);
    }
}
