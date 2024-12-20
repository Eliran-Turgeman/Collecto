namespace EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;

/// <summary>
/// Provides file system storage operations for email templates, managing template files on disk.
/// </summary>
public class FileSystemTemplateStorageProvider : ITemplateStorageProvider
{
    private readonly string _baseDirectory;

    /// <summary>
    /// Initializes a new instance of the FileSystemTemplateStorageProvider class.
    /// </summary>
    /// <param name="baseDirectory">The base directory path where template files will be stored</param>
    /// <exception cref="ArgumentException">Thrown when baseDirectory is null or empty</exception>
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

    /// <summary>
    /// Retrieves the template body content from the file system.
    /// </summary>
    /// <param name="uri">The URI (filename) of the template to retrieve</param>
    /// <returns>The template content, or empty string if file not found</returns>
    /// <exception cref="ArgumentException">Thrown when uri is null or empty</exception>
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

    /// <summary>
    /// Saves a template body to the file system.
    /// </summary>
    /// <param name="templateBody">The content to save</param>
    /// <param name="currentUri">Optional existing URI to update. If null, creates new file</param>
    /// <returns>The URI (filename) where the template was saved</returns>
    /// <exception cref="ArgumentNullException">Thrown when templateBody is null</exception>
    public async Task<string> SaveTemplateBodyAsync(string templateBody, string? currentUri = null)
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

    /// <summary>
    /// Deletes a template file from the file system.
    /// </summary>
    /// <param name="uri">The URI (filename) of the template to delete</param>
    public void DeleteTemplateBodyAsync(string uri)
    {
        var filePath = Path.Combine(_baseDirectory, uri);
        File.Delete(filePath);
    }
}