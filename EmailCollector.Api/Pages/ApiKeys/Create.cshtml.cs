using System.ComponentModel.DataAnnotations;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.Users;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.ApiKeys;

public class CreateModel : PageModel
{
    private readonly IApiKeyService _apiKeyService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;


    public CreateModel(IApiKeyService apiKeyService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _apiKeyService = apiKeyService;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public ApiKeyCreatedDto ApiKey { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "API Key Name")]
        public string Name { get; set; }

        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime? Expiration { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);
        
        ApiKey = await _apiKeyService.GenerateApiKeyAsync(userId, Input.Name, Input.Expiration);

        ModelState.Clear(); // Clear the form

        return Page();
    }
}