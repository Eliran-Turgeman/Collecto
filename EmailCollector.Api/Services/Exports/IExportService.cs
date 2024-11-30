namespace EmailCollector.Api.Services.Exports;

public interface IExportService
{
    Task<byte[]> ExportAsync<T>(IEnumerable<T> data, ExportFormat format) where T : IExportable;
}