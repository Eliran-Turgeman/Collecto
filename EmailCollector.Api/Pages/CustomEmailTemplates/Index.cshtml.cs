using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.CustomEmailTemplates;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmailCollector.Api.Pages.CustomEmailTemplates;

public class Index : PageModel
{
    private readonly ICustomEmailTemplatesService _emailTemplatesService;
    private readonly IFormService _formService;
    private readonly UserManager<EmailCollectorApiUser> _userManager;


    public Index(ICustomEmailTemplatesService emailTemplatesService,
        IFormService formService,
        UserManager<EmailCollectorApiUser> userManager)
    {
        _emailTemplatesService = emailTemplatesService;
        _formService = formService;
        _userManager = userManager;
    }

    public IEnumerable<FormEmailTemplatesViewModel> CustomEmailTemplates { get; set; }

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var userId = new Guid(currentUser?.Id!);

        // Fetch user's forms
        var userForms = await _formService.GetFormsByUserAsync(userId);
        var formIdToNameMap = new Dictionary<Guid, string>();
        foreach (var form in userForms)
        {
            formIdToNameMap[form.Id] = form.FormName;
        }

        // Fetch all templates associated with user's forms
        var allTemplates = await _emailTemplatesService.GetCustomEmailTemplatesByFormIds(formIdToNameMap.Keys.ToList());

        // Build the ViewModel
        var viewModelList = new List<FormEmailTemplatesViewModel>();

        foreach (var form in userForms)
        {
            allTemplates.TryGetValue(form.Id, out var templates);

            viewModelList.Add(new FormEmailTemplatesViewModel
            {
                FormName = form.FormName,
                Templates = templates ?? Enumerable.Empty<CustomEmailTemplateDto>()
            });
        }

        CustomEmailTemplates = viewModelList;
    }

    public class FormEmailTemplatesViewModel
    {
        public string FormName { get; set; }
        public IEnumerable<CustomEmailTemplateDto> Templates { get; set; }
    }
}