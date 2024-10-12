using Microsoft.Extensions.Options;

namespace EmailCollector.Api.Configurations;

public class FeatureTogglesService : IFeatureToggles
{
    private readonly FeatureToggles _featureFlags;

    public FeatureTogglesService(IOptions<FeatureToggles> featureFlags)
    {
        _featureFlags = featureFlags.Value;
    }

    public bool IsEmailConfirmationEnabled() => _featureFlags.EmailConfirmation;
}
