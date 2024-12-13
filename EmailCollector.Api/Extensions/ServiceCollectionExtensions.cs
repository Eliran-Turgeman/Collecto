using EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;

namespace EmailCollector.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTemplateStorageProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var storageProviderType = configuration["TemplateStorage:Provider"];

        if (string.IsNullOrWhiteSpace(storageProviderType))
        {
            throw new InvalidOperationException("TemplateStorage:Provider configuration is missing.");
        }

        switch (storageProviderType)
        {
            case "FileSystem":
                var baseDirectory = configuration["TemplateStorage:FileSystem:BaseDirectory"];
                if (string.IsNullOrWhiteSpace(baseDirectory))
                {
                    throw new InvalidOperationException("TemplateStorage:FileSystem:BaseDirectory configuration is missing.");
                }

                services.AddSingleton<ITemplateStorageProvider>(_ =>
                    new FileSystemTemplateStorageProvider(baseDirectory));
                break;

            default:
                throw new InvalidOperationException($"Unsupported TemplateStorage provider: {storageProviderType}");
        }
    }
}