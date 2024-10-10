using DnsClient;

public class DnsLookupService : IDnsLookupService
{
    private readonly ILogger<DnsLookupService> _logger;

    public DnsLookupService(ILogger<DnsLookupService> logger)
    {
        _logger = logger;
    }

    public bool HasMxRecords(string email)
    {
        var domain = email.Split('@')[1];
        _logger.LogInformation($"Validating email with dns lookup for domain {domain}");
        var lookup = new LookupClient();
        var result = lookup.Query(domain, QueryType.MX);
        return result.Answers.MxRecords().Any();
    }
}
