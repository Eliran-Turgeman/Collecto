using System.Reflection;

namespace EmailCollector.Api.Services.Exports;

public class ExportFieldMetadata
{
    public PropertyInfo Property { get; set; }
    public string DisplayName { get; set; }
}