namespace EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;

/// <summary>
/// Defines operations for storing and retrieving email template bodies.
/// </summary>
public interface ITemplateStorageProvider
{
    /// <summary>
    /// Retrieves the template body content from storage.
    /// </summary>
    /// <param name="uri">The URI of the template to retrieve</param>
    /// <returns>The template content</returns>
    Task<string> GetTemplateBodyAsync(string uri);

    /// <summary>
    /// Saves a template body to storage.
    /// </summary>
    /// <param name="templateBody">The content to save</param>
    /// <param name="currentUri">Optional existing URI to update</param>
    /// <returns>The URI where the template was saved</returns>
    Task<string> SaveTemplateBodyAsync(string templateBody, string? currentUri = null);
    
    /// <summary>
    /// Deletes a template from storage.
    /// </summary>
    /// <param name="uri">The URI of the template to delete</param>
    void DeleteTemplateBodyAsync(string uri);
}