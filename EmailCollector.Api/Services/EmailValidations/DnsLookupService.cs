using DnsClient;

public class DnsLookupService : IDnsLookupService
{
    public bool HasMxRecords(string domain)
    {
        var lookup = new LookupClient();
        var result = lookup.Query(domain, QueryType.MX);
        return result.Answers.MxRecords().Any();
    }
}
