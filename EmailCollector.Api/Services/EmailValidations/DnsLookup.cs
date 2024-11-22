using DnsClient;

public static class DnsLookup
{
    public static bool HasMxRecords(string domain)
    {
        var lookup = new LookupClient();
        var result = lookup.Query(domain, QueryType.MX);
        return result.Answers.MxRecords().Any();
    }
}
