using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
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

    public Guid UserId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public IEnumerable<FormSummaryDetailsDto?> Forms { get; set; } = new List<FormSummaryDetailsDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ErrorMessage = "Please log in to view your forms dashboard.";
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        UserId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsSummaryDetailsAsync(UserId);

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
        UserId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsSummaryDetailsAsync(UserId);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteFormAsync(string formId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        UserId = new Guid(currentUser?.Id!);

        if (!string.IsNullOrEmpty(formId) && int.TryParse(formId, out var intFormId))
        {
            await _formService.DeleteFormByIdAsync(intFormId, UserId);
        }
        return RedirectToPage();
    }
}
