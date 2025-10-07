using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples.utils;

public class ContentSafetyClientFactory
{
    /// <summary>
    /// Creates a ContentSafetyClient with appropriate authentication
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Configured ContentSafetyClient instance</returns>
    public static ContentSafetyClient CreateContentSafetyClient(IConfiguration configuration)
    {
        var endpoint = configuration["ContentSafety:Endpoint"];
        var apiKey = configuration["ContentSafety:ApiKey"];

        if (string.IsNullOrEmpty(endpoint))
            throw new InvalidOperationException("ContentSafety:Endpoint is not configured in appsettings.json");
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("ContentSafety:ApiKey is not configured in appsettings.json");

        return new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }
}
