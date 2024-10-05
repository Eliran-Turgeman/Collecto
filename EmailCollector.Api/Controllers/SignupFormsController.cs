using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EmailCollector.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SignupFormsController : ControllerBase
{
    private readonly EmailCollectorApiContext _context;

    public SignupFormsController(EmailCollectorApiContext context)
    {
        _context = context;
    }

    // GET: api/SignupForms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SignupForm>>> GetSignupForms()
    {
        return await _context.SignupForms.ToListAsync();
    }

    // GET: api/SignupForms/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SignupForm>> GetSignupForm(int id)
    {
        var signupForm = await _context.SignupForms.FindAsync(id);

        if (signupForm == null)
        {
            return NotFound();
        }

        return signupForm;
    }

    // PUT: api/SignupForms/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSignupForm(int id, SignupForm signupForm)
    {
        if (id != signupForm.Id)
        {
            return BadRequest();
        }

        _context.Entry(signupForm).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SignupFormExists(id))
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

    // POST: api/SignupForms
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<SignupForm>> PostSignupForm(SignupForm signupForm)
    {
        _context.SignupForms.Add(signupForm);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSignupForm", new { id = signupForm.Id }, signupForm);
    }

    // DELETE: api/SignupForms/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSignupForm(int id)
    {
        var signupForm = await _context.SignupForms.FindAsync(id);
        if (signupForm == null)
        {
            return NotFound();
        }

        _context.SignupForms.Remove(signupForm);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SignupFormExists(int id)
    {
        return _context.SignupForms.Any(e => e.Id == id);
    }
}
