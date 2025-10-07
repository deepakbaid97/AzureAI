using Azure.AI.ContentSafety.Utils;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

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
    // Default text for demonstration (from the API documentation - song lyrics)
    private const string TextToAnalyze = "Kiss me out of the bearded barley Nightly beside the green, green grass Swing, swing, swing the spinning step You wear those shoes and I will wear that dress Oh, kiss me beneath the milky twilight Lead me out on the moonlit floor Lift your open hand Strike up the band and make the fireflies dance Silver moon's sparkling So, kiss me Kiss me down by the broken tree house Swing me upon its hanging tire Bring, bring, bring your flowered hat We'll take the trail marked on your father's map.";

    // Default code for demonstration (from the API documentation - Python game code)
    private const string CodeToAnalyze = "python import pygame pygame.init() win = pygame.display.set_mode((500, 500)) pygame.display.set_caption(My Game) x = 50 y = 50 width = 40 height = 60 vel = 5 run = True while run: pygame.time.delay(100) for event in pygame.event.get(): if event.type == pygame.QUIT: run = False keys = pygame.key.get_pressed() if keys[pygame.K_LEFT] and x > vel: x -= vel if keys[pygame.K_RIGHT] and x < 500 - width - vel: x += vel if keys[pygame.K_UP] and y > vel: y -= vel if keys[pygame.K_DOWN] and y < 500 - height - vel: y += vel win.fill((0, 0, 0)) pygame.draw.rect(win, (255, 0, 0), (x, y, width, height)) pygame.display.update() pygame.quit()";


    public static async Task AnalyzeProtectedTextAsync(IConfiguration configuration)
    {
        // Get endpoint and API key from configuration
        var endpoint = configuration["ContentSafety:Endpoint"];
        var apiKey = configuration["ContentSafety:ApiKey"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("ContentSafety endpoint and API key must be configured in appsettings.json");
        }

        try
        {
            Console.WriteLine("=== PROTECTED TEXT MATERIAL DETECTION MODE ===");
            Console.WriteLine($"Analyzing text for protected/copyrighted content...");
            Console.WriteLine(new string('-', 50));

            // Construct the API URL for text
            var baseUrl = endpoint.TrimEnd('/');
            var apiUrl = $"{baseUrl}{UrlForProtectedMaterialText}";

            // Create the request payload
            var requestPayload = new
            {
                TextToAnalyze
            };

            var result = await SendApiRequest(apiUrl, apiKey, requestPayload, "Protected Material Detection (Text)");

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
    public static async Task AnalyzeProtectedCodeAsync(IConfiguration configuration)
    {
        // Get endpoint and API key from configuration
        var endpoint = configuration["ContentSafety:Endpoint"];
        var apiKey = configuration["ContentSafety:ApiKey"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("ContentSafety endpoint and API key must be configured in appsettings.json");
        }

        try
        {
            Console.WriteLine("=== PROTECTED CODE MATERIAL DETECTION MODE ===");
            Console.WriteLine($"Analyzing code for protected/copyrighted content...");
            Console.WriteLine(new string('-', 50));

            // Construct the API URL for code
            var baseUrl = endpoint.TrimEnd('/');
            var apiUrl = $"{baseUrl}{UrlForProtectedMaterialCode}";

            // Create the request payload
            var requestPayload = new
            {
                CodeToAnalyze
            };

            var result = await SendApiRequest(apiUrl, apiKey, requestPayload, "Protected Material Detection (Code)");

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
