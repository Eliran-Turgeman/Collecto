﻿using Microsoft.AspNetCore.Mvc;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EmailCollector.Api.DTOs;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using EmailCollector.Api.Services;

namespace EmailCollector.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
    public async Task<ActionResult<IEnumerable<SignupForm>>> GetSignupForms()
    {
        _logger.LogInformation("Getting signup forms.");
        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            return Unauthorized();
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

    public async Task<ActionResult<SignupForm>> GetSignupForm(int id)
    {
        _logger.LogInformation($"Getting signup form {id}.");
        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            return BadRequest();
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
    public async Task<ActionResult<SignupForm>> PostSignupForm(CreateFormDto signupForm)
    {
        _logger.LogInformation("Creating signup form.");
        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            return BadRequest();
        }

        var formsDetails = await _formService.CreateFormAsync(userId, signupForm);
        _logger.LogInformation($"Created signup form {formsDetails.Id}.");
        return CreatedAtAction("GetSignupForm", new { id = formsDetails.Id }, signupForm);
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
    public async Task<IActionResult> DeleteSignupForm(int id)
    {
        _logger.LogInformation($"Deleting signup form {id}.");

        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            return BadRequest();
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
    public async Task<IActionResult> PutSignupForm(int id, CreateFormDto signupForm)
    {
        _logger.LogInformation($"Updating signup form {id}.");

        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            return BadRequest();
        }

        if (await _formService.GetFormByIdAsync(id, userId) == null)
        {
            return NotFound();
        }

        await _formService.UpdateFormAsync(id, userId, signupForm);

        _logger.LogInformation($"Updated signup form {id}.");

        return NoContent();
    }
}
