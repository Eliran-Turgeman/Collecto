using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages;

public class DashboardModel : PageModel
{
    private readonly SignInManager<EmailCollectorApiUser> _signInManager;
    private readonly UserManager<EmailCollectorApiUser> _userManager;
    private readonly IFormService _formService;
    private readonly IEmailSignupService _emailSignupService;

    public IEnumerable<FormDto> Forms { get; set; } = new List<FormDto>();

    [BindProperty(SupportsGet = true)]
    public string FormId { get; set; }
    
    public Guid UserId { get; set; }

    [BindProperty]
    public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

    [BindProperty]
    public DateTime EndDate { get; set; } = DateTime.Now;

    public int TotalSubscribers { get; set; }
    public string FormStatus { get; set; }
    public double AvgSubsPerDay { get; set; }

    public List<string> Dates { get; set; } = new List<string>();
    public List<int> Counts { get; set; } = new List<int>();
    public List<int> CumulativeCounts { get; set; } = new List<int>();

    public string ErrorMessage { get; set; } = string.Empty;

    public DashboardModel(SignInManager<EmailCollectorApiUser> signInManager,
        UserManager<EmailCollectorApiUser> userManager,
        IFormService formService,
        IEmailSignupService emailSignupService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _formService = formService;
        _emailSignupService = emailSignupService;
    }


    public async Task<IActionResult> OnGetAsync()
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ErrorMessage = "Please log in to view your forms dashboard.";
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        UserId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsByUserAsync(UserId);

        // Ensure SelectedFormId is valid
        if (string.IsNullOrEmpty(FormId) || !int.TryParse(FormId, out var intSelectedFormId))
        {
            ErrorMessage = "Please select a form.";
            return Page();
        }

        await LoadFormData(intSelectedFormId, UserId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        UserId = new Guid(currentUser?.Id!);

        Forms = await _formService.GetFormsByUserAsync(UserId);
        
        if (string.IsNullOrEmpty(FormId) || !int.TryParse(FormId, out var intSelectedFormId))
        {
            ErrorMessage = "Please select a form.";
            return Page();
        }
        await LoadFormData(intSelectedFormId, UserId);
        return Page();
    }

    private async Task LoadFormData(int formId, Guid userId)
    {
        var form = await _formService.GetFormByIdAsync(formId, userId);
        if (form == null)
        {
            ErrorMessage = "Form not found.";
            return;
        }
        
        var emailSignupsData = await _emailSignupService.GetSignupsPerDayAsync(formId, StartDate, EndDate);
        
        TotalSubscribers = emailSignupsData.Sum(s => s.Count);
        FormStatus = form.Status.ToString();
        AvgSubsPerDay = emailSignupsData.Any() ? (TotalSubscribers / emailSignupsData.Count()) : 0;

        Dates = emailSignupsData.Select(s => s.Date.ToString("yyyy-MM-dd")).ToList();
        Counts = emailSignupsData.Select(s => s.Count).ToList();

        int cumulativeSum = 0;
        CumulativeCounts = Counts.Select(count => cumulativeSum += count).ToList();
    }
}
