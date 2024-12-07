using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.Forms;

public class Create : PageModel
{
    private readonly IFormService _formService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;
    
    public FormDetailsDto Form { get; set; }

    public Create(IFormService formService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _formService = formService;
        _userManager = userManager;
    }
    
    [BindProperty]
    public ExtendedCreateFormDto Input { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);
        
        Form = await _formService.CreateFormAsync(userId, Input);

        return RedirectToPage("./Index");
    }
}