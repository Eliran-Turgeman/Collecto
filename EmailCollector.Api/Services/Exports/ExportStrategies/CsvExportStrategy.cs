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
            var rows = GenerateRows(item, fields);
            foreach (var row in rows)
            {
                csv.AppendLine(string.Join(",", row));
            }
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private IEnumerable<List<string>> GenerateRows<T>(T item, IEnumerable<ExportFieldMetadata> fields)
    {
        // Separate enumerable fields from static fields
        var enumerableField = fields.FirstOrDefault(f => 
            typeof(IEnumerable<string>).IsAssignableFrom(f.Property.PropertyType));

        // Handle non-enumerable fields
        var staticValues = fields
            .Where(f => !typeof(IEnumerable<string>).IsAssignableFrom(f.Property.PropertyType))
            .Select(f => f.Property.GetValue(item)?.ToString() ?? string.Empty)
            .ToList();

        // If no enumerable field, return the static row as is
        if (enumerableField == null)
        {
            yield return staticValues;
            yield break;
        }

        // Handle the enumerable field
        var enumerableValues = enumerableField.Property.GetValue(item) as IEnumerable<string>;
        if (enumerableValues != null && enumerableValues.Any())
        {
            foreach (var value in enumerableValues)
            {
                // Add each enumerable value as a new row
                var row = new List<string>(staticValues)
                {
                    value // Add the enumerable value to the row
                };
                yield return row;
            }
        }
        else
        {
            // If enumerable is null or empty, include an empty column
            var row = new List<string>(staticValues) { string.Empty };
            yield return row;
        }
    }
}
