namespace EmailCollector.Api.Services.Exports;

[AttributeUsage(AttributeTargets.Property)]
public class ExportFieldAttribute : Attribute
{
    public string DisplayName { get; }

    public ExportFieldAttribute(string displayName = null)
    {
        DisplayName = displayName;
    }
}