using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace EmailCollector.Api.Services.Exports.ExportStrategies;

public class JsonExportStrategy : IExportFormatStrategy
{
    public ExportFormat Format => ExportFormat.Json;

    public async Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, IEnumerable<ExportFieldMetadata> fields)
    {
        var exportData = data.Select(item =>
        {
            var obj = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                obj[field.DisplayName] = field.Property.GetValue(item);
            }
            return obj;
        });

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Avoids escaping characters like '+'
        };

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(exportData, options));
    }
}