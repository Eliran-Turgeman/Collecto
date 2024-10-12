using Microsoft.Extensions.Options;

namespace EmailCollector.Api.Configurations;

public class FeatureToggles
{
    public bool EmailConfirmation { get; set; }
}
