using System.Text;
using System.Text.Json;
using Azure.AI.ContentSafety.Utils;
using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Utility class for analyzing prompts for potential jailbreaking attempts using Azure AI Content Safety Shield Prompt API.
/// This API helps detect prompt injection attacks and other malicious prompt engineering techniques.
/// </summary>
public static class ShieldPrompt
{
    private static readonly HttpClient _httpClient = new();

    /// <summary>
    /// Analyzes a user prompt and documents for potential prompt injection or jailbreaking attempts.
    /// </summary>
    /// <param name="configuration">Configuration containing endpoint and API key</param>
    /// <param name="userPrompt">The user prompt to analyze</param>
    /// <param name="documents">Optional array of documents to analyze alongside the prompt</param>
    public static async Task AnalyzeShieldPromptAsync(IConfiguration configuration, string userPrompt, string[]? documents = null)
    {
        try
        {
            Console.WriteLine("=== SHIELD PROMPT ANALYSIS MODE ===");
            Console.WriteLine($"Analyzing prompt for potential jailbreaking attempts...");
            Console.WriteLine($"User Prompt: {userPrompt[..Math.Min(100, userPrompt.Length)]}...");
            Console.WriteLine(new string('-', 50));

            // Get endpoint and API key from configuration
            var endpoint = configuration["ContentSafety:Endpoint"];
            var apiKey = configuration["ContentSafety:ApiKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("ContentSafety endpoint and API key must be configured in appsettings.json");
            }

            // Perform the shield prompt analysis
            var result = await CallShieldPromptApiAsync(endpoint, apiKey, userPrompt, documents);

            // Display results
            DisplayAnalysisResultHelper.DisplayShieldPromptResults(result);

            Console.WriteLine("\nShield Prompt analysis completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during Shield Prompt analysis: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Calls the Azure Content Safety Shield Prompt API to analyze for prompt injection attempts.
    /// </summary>
    /// <param name="endpoint">The Azure Content Safety endpoint</param>
    /// <param name="apiKey">The API subscription key</param>
    /// <param name="userPrompt">The user prompt to analyze</param>
    /// <param name="documents">Optional documents to analyze</param>
    /// <returns>The API response as a JsonDocument</returns>
    private static async Task<JsonDocument> CallShieldPromptApiAsync(string endpoint, string apiKey, string userPrompt, string[]? documents)
    {
        try
        {
            // Construct the API URL
            var baseUrl = endpoint.TrimEnd('/');
            var apiUrl = $"{baseUrl}/contentsafety/text:shieldPrompt?api-version=2024-09-01";

            // Create the request payload
            var requestPayload = new
            {
                userPrompt,
                documents = documents ?? Array.Empty<string>()
            };

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
            Console.WriteLine("Sending request to Shield Prompt API...");
            using var response = await _httpClient.SendAsync(request, cts.Token);

            // Check if the request was successful
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Shield Prompt API request failed with status {response.StatusCode}: {errorContent}");
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
