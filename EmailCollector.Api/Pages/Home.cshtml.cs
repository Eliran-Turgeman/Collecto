using EmailCollector.Api.Areas.Identity.Data;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages;

public class HomeModel : PageModel
{
    private readonly SignInManager<EmailCollectorApiUser> _signInManager;
    private readonly UserManager<EmailCollectorApiUser> _userManager;
    private readonly IFormService _formService;

    public HomeModel(SignInManager<EmailCollectorApiUser> signInManager,
        UserManager<EmailCollectorApiUser> userManager,
        IFormService formService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _formService = formService;
    }

    public string ErrorMessage { get; set; } = string.Empty;
    public IEnumerable<FormDto> Forms { get; set; } = new List<FormDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ErrorMessage = "Please log in to view your forms dashboard.";
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsByUserAsync(userId);

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(string action)
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ErrorMessage = "Please log in to view your forms dashboard.";
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsByUserAsync(userId);

        return Page();
    }
}
