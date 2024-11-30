using System.Reflection;

namespace EmailCollector.Api.Services.Exports;

public class ExportMetadataResolver
{
    public IEnumerable<ExportFieldMetadata> Resolve<T>() where T : IExportable
    {
        return typeof(T)
            .GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(ExportFieldAttribute)))
            .Select(prop => new ExportFieldMetadata
            {
                Property = prop,
                DisplayName = prop.GetCustomAttribute<ExportFieldAttribute>()?.DisplayName ?? prop.Name
            });
    }
}