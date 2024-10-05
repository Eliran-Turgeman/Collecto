using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EmailCollector.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmailSignupsController : ControllerBase
{
    private readonly EmailCollectorApiContext _context;

    public EmailSignupsController(EmailCollectorApiContext context)
    {
        _context = context;
    }

    // GET: api/EmailSignups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmailSignup>>> GetEmailSignups()
    {
        return await _context.EmailSignups.ToListAsync();
    }

    // GET: api/EmailSignups/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmailSignup>> GetEmailSignup(int id)
    {
        var emailSignup = await _context.EmailSignups.FindAsync(id);

        if (emailSignup == null)
        {
            return NotFound();
        }

        return emailSignup;
    }

    // GET: api/FormEmailSignups/5
    [HttpGet("form/{formId}")]
    public async Task<ActionResult<IEnumerable<EmailSignup>>> GetFormEmailSignups(int formId)
    {
        var emailSignups = await _context.EmailSignups.Where(signup => signup.SignupFormId == formId).ToListAsync();

        if (emailSignups == null || emailSignups.Count == 0)
        {
            return NotFound();
        }

        return emailSignups;
    }

    // PUT: api/EmailSignups/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmailSignup(int id, EmailSignup emailSignup)
    {
        if (id != emailSignup.Id)
        {
            return BadRequest();
        }

        _context.Entry(emailSignup).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmailSignupExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/EmailSignups
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<EmailSignup>> PostEmailSignup(EmailSignup emailSignup)
    {
        _context.EmailSignups.Add(emailSignup);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetEmailSignup", new { id = emailSignup.Id }, emailSignup);
    }

    // DELETE: api/EmailSignups/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmailSignup(int id)
    {
        var emailSignup = await _context.EmailSignups.FindAsync(id);
        if (emailSignup == null)
        {
            return NotFound();
        }

        _context.EmailSignups.Remove(emailSignup);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmailSignupExists(int id)
    {
        return _context.EmailSignups.Any(e => e.Id == id);
    }
}
