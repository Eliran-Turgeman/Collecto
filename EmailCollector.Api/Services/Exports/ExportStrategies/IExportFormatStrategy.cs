namespace EmailCollector.Api.Services.Exports.ExportStrategies;

public interface IExportFormatStrategy
{
    ExportFormat Format { get; }
    Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, IEnumerable<ExportFieldMetadata> fields);
}