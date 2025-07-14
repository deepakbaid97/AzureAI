using System.Text;
using System.Text.Json;
using Azure.AI.ContentSafety.Utils;
using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Utility class for detecting protected material (copyrighted content) using Azure AI Content Safety Protected Material Detection API.
/// This API helps identify copyrighted songs, books, articles, code, and other protected intellectual property.
/// </summary>
public static class ProtectedMaterialDetection
{
    private static readonly HttpClient _httpClient = new();
    private const string UrlForProtectedMaterialText = "/contentsafety/text:detectProtectedMaterial?api-version=2024-09-01";
    private const string UrlForProtectedMaterialCode = "/contentsafety/text:detectProtectedMaterialForCode?api-version=2024-09-15-preview";

    /// <summary>
    /// Analyzes text content to detect protected material such as copyrighted songs, books, or articles.
    /// </summary>
    /// <param name="configuration">Configuration containing endpoint and API key</param>
    /// <param name="textToAnalyze">The text content to analyze for protected material</param>
    public static async Task AnalyzeProtectedTextAsync(IConfiguration configuration, string textToAnalyze)
    {
        try
        {
            Console.WriteLine("=== PROTECTED TEXT MATERIAL DETECTION MODE ===");
            Console.WriteLine($"Analyzing text for protected/copyrighted content...");
            Console.WriteLine($"Text to analyze: {textToAnalyze[..Math.Min(150, textToAnalyze.Length)]}...");
            Console.WriteLine(new string('-', 50));

            // Get endpoint and API key from configuration
            var endpoint = configuration["ContentSafety:Endpoint"];
            var apiKey = configuration["ContentSafety:ApiKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("ContentSafety endpoint and API key must be configured in appsettings.json");
            }

            // Perform the protected material detection for text
            var result = await CallProtectedMaterialTextApiAsync(endpoint, apiKey, textToAnalyze);

            // Display results
            DisplayAnalysisResultHelper.DisplayProtectedMaterialResults(result, "text");

            Console.WriteLine("\nProtected text material detection completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during protected text material detection: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Analyzes code content to detect protected material such as copyrighted code snippets or libraries.
    /// </summary>
    /// <param name="configuration">Configuration containing endpoint and API key</param>
    /// <param name="codeToAnalyze">The code content to analyze for protected material</param>
    public static async Task AnalyzeProtectedCodeAsync(IConfiguration configuration, string codeToAnalyze)
    {
        try
        {
            Console.WriteLine("=== PROTECTED CODE MATERIAL DETECTION MODE ===");
            Console.WriteLine($"Analyzing code for protected/copyrighted content...");
            Console.WriteLine($"Code to analyze: {codeToAnalyze[..Math.Min(150, codeToAnalyze.Length)]}...");
            Console.WriteLine(new string('-', 50));

            // Get endpoint and API key from configuration
            var endpoint = configuration["ContentSafety:Endpoint"];
            var apiKey = configuration["ContentSafety:ApiKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("ContentSafety endpoint and API key must be configured in appsettings.json");
            }

            // Perform the protected material detection for code
            var result = await CallProtectedMaterialCodeApiAsync(endpoint, apiKey, codeToAnalyze);

            // Display results
            DisplayAnalysisResultHelper.DisplayProtectedMaterialResults(result, "code");

            Console.WriteLine("\nProtected code material detection completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during protected code material detection: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Legacy method for backward compatibility - analyzes text content for protected material.
    /// </summary>
    /// <param name="configuration">Configuration containing endpoint and API key</param>
    /// <param name="textToAnalyze">The text content to analyze for protected material</param>
    public static async Task AnalyzeProtectedMaterialAsync(IConfiguration configuration, string textToAnalyze)
    {
        await AnalyzeProtectedTextAsync(configuration, textToAnalyze);
    }

    /// <summary>
    /// Calls the Azure Content Safety Protected Material Detection API to analyze text for copyrighted content.
    /// </summary>
    /// <param name="endpoint">The Azure Content Safety endpoint</param>
    /// <param name="apiKey">The API subscription key</param>
    /// <param name="text">The text content to analyze</param>
    /// <returns>The API response as a JsonDocument</returns>
    private static async Task<JsonDocument> CallProtectedMaterialTextApiAsync(string endpoint, string apiKey, string text)
    {
        try
        {
            // Construct the API URL for text
            var baseUrl = endpoint.TrimEnd('/');
            var apiUrl = $"{baseUrl}{UrlForProtectedMaterialText}";

            // Create the request payload
            var requestPayload = new
            {
                text
            };

            return await SendApiRequest(apiUrl, apiKey, requestPayload, "Protected Material Detection (Text)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Text API request failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Calls the Azure Content Safety Protected Material Detection API to analyze code for copyrighted content.
    /// </summary>
    /// <param name="endpoint">The Azure Content Safety endpoint</param>
    /// <param name="apiKey">The API subscription key</param>
    /// <param name="code">The code content to analyze</param>
    /// <returns>The API response as a JsonDocument</returns>
    private static async Task<JsonDocument> CallProtectedMaterialCodeApiAsync(string endpoint, string apiKey, string code)
    {
        try
        {
            // Construct the API URL for code
            var baseUrl = endpoint.TrimEnd('/');
            var apiUrl = $"{baseUrl}{UrlForProtectedMaterialCode}";

            // Create the request payload
            var requestPayload = new
            {
                code
            };

            return await SendApiRequest(apiUrl, apiKey, requestPayload, "Protected Material Detection (Code)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Code API request failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Sends a request to the Azure Content Safety API.
    /// </summary>
    /// <param name="apiUrl">The API URL to call</param>
    /// <param name="apiKey">The API subscription key</param>
    /// <param name="requestPayload">The request payload object</param>
    /// <param name="apiName">The name of the API for logging purposes</param>
    /// <returns>The API response as a JsonDocument</returns>
    private static async Task<JsonDocument> SendApiRequest(string apiUrl, string apiKey, object requestPayload, string apiName)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Create HTTP request
            using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Configure timeout for the request
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            // Send the request
            Console.WriteLine($"Sending request to {apiName} API...");
            using var response = await _httpClient.SendAsync(request, cts.Token);

            // Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"{apiName} API request failed with status {response.StatusCode}: {errorContent}");
            }

            // Parse and return the response
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(responseContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Disposes of the static HttpClient when the application shuts down.
    /// </summary>
    public static void Dispose()
    {
        _httpClient?.Dispose();
    }
}
