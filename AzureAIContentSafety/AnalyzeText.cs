using Azure.AI.ContentSafety.Utils;

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
    public static async Task AnalyzeTextAsync(ContentSafetyClient client, string textToAnalyze)
    {
        try
        {
            Console.WriteLine("=== TEXT ANALYSIS MODE ===");
            Console.WriteLine($"Analyzing text: {textToAnalyze}");
            Console.WriteLine(new string('-', 50));

            // Analyze the text content
            await AnalyzeTextContentAsync(client, textToAnalyze);

            Console.WriteLine("\nText analysis completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Analyzes text content for safety categories including hate speech, 
    /// self-harm, sexual content, and violence.
    /// </summary>
    /// <param name="client">The ContentSafetyClient instance</param>
    /// <param name="text">Text content to analyze</param>
    private static async Task AnalyzeTextContentAsync(ContentSafetyClient client, string text)
    {
        try
        {
            // Create and send analysis request
            var request = new AnalyzeTextOptions(text);
            var response = await client.AnalyzeTextAsync(request);

            // Display the results
            DisplayAnalysisResultHelper.DisplayAnalysisResults(response.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Content Safety analysis failed: {ex.Message}");
            throw;
        }
    }
}
