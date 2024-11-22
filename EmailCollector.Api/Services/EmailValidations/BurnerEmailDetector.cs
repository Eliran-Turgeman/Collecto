namespace EmailCollector.Api.Services.EmailValidations;

public static class BurnerEmailDetector
{
    private static readonly HashSet<string> BadDomains;

    static BurnerEmailDetector()
    {
        BadDomains = new HashSet<string>(
            File.ReadAllLines("Services/EmailValidations/emails.txt")
                .Select(line => line.Trim().ToLowerInvariant())
                .Where(line => !string.IsNullOrWhiteSpace(line))
        );
    }

    /// <summary>
    /// Checks if the given domain is on the burner email providers list.
    /// </summary>
    /// <param name="domain">The domain to check.</param>
    /// <returns>True if the domain is on the burner email provider list, false otherwise.</returns>
    public static bool IsBurnerDomain(string domain)
    {
        if (string.IsNullOrEmpty(domain))
            return false;

        domain = domain.ToLowerInvariant();

        if (BadDomains.Contains(domain))
            return true;

        // Check higher-level domains
        var dotIndex = domain.IndexOf('.');
        if (dotIndex >= 0)
        {
            var higherDomain = domain.Substring(dotIndex + 1);
            return IsBurnerDomain(higherDomain);
        }

        return false;
    }
}