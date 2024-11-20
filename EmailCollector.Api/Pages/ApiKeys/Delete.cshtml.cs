using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.Users;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.ApiKeys;

public class DeleteModel : PageModel
{
    private readonly IApiKeyService _apiKeyService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;

    public DeleteModel(IApiKeyService apiKeyService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
    }

    [BindProperty]
    public ApiKeyDto ApiKey { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        var apiKeys = await _apiKeyService.GetAllByUserIdAsync(userId);
        ApiKey = apiKeys.FirstOrDefault(k => k.Id == id);

        if (ApiKey == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        
        if (ApiKey?.Id == null || currentUser == null)
        {
            return BadRequest();
        }

        await _apiKeyService.RevokeApiKeyAsync(ApiKey.Id);

        return RedirectToPage("./Index");
    }
}