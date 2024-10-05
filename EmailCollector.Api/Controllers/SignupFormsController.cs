using Microsoft.AspNetCore.Mvc;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using EmailCollector.Api.Interfaces;
using EmailCollector.Api.DTOs;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

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

    // GET: api/SignupForms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SignupForm>>> GetSignupForms()
    {
        _logger.LogInformation("Getting signup forms.");
        if (!Guid.TryParse(HttpContext.Items["UserId"] as string, out var userId))
        {
            return BadRequest();
        }

        return Ok(await _formService.GetFormsByUserAsync(userId));
    }

    // GET: api/SignupForms/5
    [HttpGet("{id}")]
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

    // POST: api/SignupForms
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
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

    // DELETE: api/SignupForms/5
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
