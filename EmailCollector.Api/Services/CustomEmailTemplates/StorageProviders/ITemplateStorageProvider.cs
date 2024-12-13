namespace EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;

public interface ITemplateStorageProvider
{
    Task<string> GetTemplateBodyAsync(string uri);
    Task<string> SaveTemplateBodyAsync(string templateBody, string currentUri = null);
}