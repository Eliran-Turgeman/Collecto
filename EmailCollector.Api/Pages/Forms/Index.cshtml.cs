using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.Forms;

public class Index : PageModel
{
    private readonly UserManager<EmailCollectorApiUser> _userManager;
    private readonly IFormService _formService;
    
    public IEnumerable<FormSummaryDetailsDto?> Forms { get; set; } = new List<FormSummaryDetailsDto>();
    public Guid UserId { get; set; }

    
    public Index(UserManager<EmailCollectorApiUser> userManager,
        IFormService formService)
    {
        _userManager = userManager;
        _formService = formService;
    }
    
    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        UserId = new Guid(currentUser?.Id!);
        Forms = await _formService.GetFormsSummaryDetailsAsync(UserId);
    }
}