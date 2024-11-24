using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETCore.MailKit.Core;

namespace EmailCollector.Api.Pages.Forms;

public class Delete : PageModel
{
    private readonly IFormService _formService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;

    public Delete(IFormService formService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _formService = formService;
        _userManager = userManager;
    }
    
    [BindProperty]
    public FormDto Form { get; set; }
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);
        
        var forms = await _formService.GetFormsByUserAsync(userId);
        Form = forms.FirstOrDefault(f => f.Id == id);

        if (Form == null)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        
        if (Form?.Id == null || currentUser == null)
        {
            return BadRequest();
        }
        
        await _formService.DeleteFormByIdAsync(Form.Id);
        
        return RedirectToPage("./Index");
    }
}