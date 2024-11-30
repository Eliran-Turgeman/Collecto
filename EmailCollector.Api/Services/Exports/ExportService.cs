using EmailCollector.Api.Services.Exports.ExportStrategies;

namespace EmailCollector.Api.Services.Exports;

public class ExportService : IExportService
{
    private readonly IEnumerable<IExportFormatStrategy> _strategies;
    private readonly ExportMetadataResolver _metadataResolver;

    public ExportService(IEnumerable<IExportFormatStrategy> strategies, ExportMetadataResolver metadataResolver)
    {
        _strategies = strategies;
        _metadataResolver = metadataResolver;
    }

    public async Task<byte[]> ExportAsync<T>(IEnumerable<T> data, ExportFormat format) where T : IExportable
    {
        var strategy = _strategies.FirstOrDefault(s => s.Format == format);
        if (strategy == null)
        {
            throw new NotSupportedException($"Export format {format} is not supported.");
        }

        var fields = _metadataResolver.Resolve<T>();
        return await strategy.GenerateAsync(data, fields);
    }
}