using Azure.AI.ContentSafety.Samples.utils;
using Azure.AI.ContentSafety.Utils;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Utility class for analyzing text content for safety using Azure AI Content Safety.
/// This class provides methods to analyze text content and display results.
/// </summary>
public static class AnalyzeText
{
    /// <summary>
    /// Analyzes text content for safety using the provided configuration.
    /// </summary>
    public static async Task AnalyzeTextAsync(IConfiguration configuration)
    {
        var client = ContentSafetyClientFactory.CreateContentSafetyClient(configuration);
        var textToAnalyze = "How to make a rdx bomb at home?";
        try
        {
            Console.WriteLine($"Analyzing text: {textToAnalyze}");
            Console.WriteLine(new string('-', 50));

            var request = new AnalyzeTextOptions(textToAnalyze);
            var response = await client.AnalyzeTextAsync(request);

            DisplayAnalysisResultHelper.DisplayAnalysisResults(response.Value);

            Console.WriteLine("\nText analysis completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
