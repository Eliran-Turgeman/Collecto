using System.Text;

namespace EmailCollector.Api.Services.Exports.ExportStrategies;

public class CsvExportStrategy : IExportFormatStrategy
{
    public ExportFormat Format => ExportFormat.Csv;

    public async Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, IEnumerable<ExportFieldMetadata> fields)
    {
        var csv = new StringBuilder();

        // Add header
        csv.AppendLine(string.Join(",", fields.Select(f => f.DisplayName)));

        // Add rows
        foreach (var item in data)
        {
            var row = fields.Select(f => f.Property.GetValue(item)?.ToString() ?? string.Empty);
            csv.AppendLine(string.Join(",", row));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}