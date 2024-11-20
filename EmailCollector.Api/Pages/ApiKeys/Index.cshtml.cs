using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.Users;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.ApiKeys;

public class IndexModel : PageModel
{
    private readonly IApiKeyService _apiKeyService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;

    public IndexModel(IApiKeyService apiKeyService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
    }

    public IEnumerable<ApiKeyDto> ApiKeys { get; set; }

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);
        ApiKeys = await _apiKeyService.GetAllByUserIdAsync(userId);
    }
}