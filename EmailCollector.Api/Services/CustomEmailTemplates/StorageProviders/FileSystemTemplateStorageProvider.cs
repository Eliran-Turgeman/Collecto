namespace EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;

public class FileSystemTemplateStorageProvider : ITemplateStorageProvider
{
    private readonly string _baseDirectory;

    public FileSystemTemplateStorageProvider(string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException("Base directory cannot be null or empty.", nameof(baseDirectory));

        _baseDirectory = baseDirectory;

        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }

    public async Task<string> GetTemplateBodyAsync(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            throw new ArgumentException("URI cannot be null or empty.", nameof(uri));

        var filePath = Path.Combine(_baseDirectory, uri);

        if (!File.Exists(filePath))
        {
            // should throw error, and reconcile the database entity.
            // if we reach here, we have a dangling template - row in db exist, but template isn't.
            return string.Empty;
        }

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task<string> SaveTemplateBodyAsync(string templateBody, string currentUri = null)
    {
        if (templateBody == null)
            throw new ArgumentNullException(nameof(templateBody));

        string fileName;

        if (string.IsNullOrWhiteSpace(currentUri))
        {
            fileName = $"{Guid.NewGuid()}.html";
        }
        else
        {
            fileName = Path.GetFileName(currentUri);
        }

        var filePath = Path.Combine(_baseDirectory, fileName);

        await File.WriteAllTextAsync(filePath, templateBody);

        return fileName;
    }
}