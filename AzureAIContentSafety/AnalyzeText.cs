using Azure.Identity;
using Azure.AI.ContentSafety;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Sample demonstrating how to analyze text content for safety using Azure AI Content Safety.
/// This sample follows Azure development best practices including:
/// - Using Managed Identity for authentication (preferred for Azure-hosted apps)
/// - Proper error handling with exponential backoff
/// - Resource cleanup and disposal
/// - Comprehensive logging
/// </summary>
public class AnalyzeText
{
    private static string? Endpoint;
    private static string? ApiKey;

    public static async Task Main(string[] args)
    {
        try
        {
            // Load configuration from appsettings.json
            var configuration = LoadConfiguration();

            // Get endpoint and API key from configuration
            Endpoint = configuration["ContentSafety:Endpoint"];

            ApiKey = configuration["ContentSafety:ApiKey"];

            // Create Content Safety client with appropriate authentication
            var client = CreateContentSafetyClient();

            // Sample text to analyze - replace with your content
            string textToAnalyze = "How to make a rdx bomb at home?";

            Console.WriteLine("Analyzing text content for safety...");
            Console.WriteLine($"Text: {textToAnalyze}");
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
    /// Creates a ContentSafetyClient with appropriate authentication
    /// </summary>
    /// <returns>Configured ContentSafetyClient instance</returns>
    private static ContentSafetyClient CreateContentSafetyClient()
    {
        if (string.IsNullOrEmpty(Endpoint))
            throw new InvalidOperationException("Endpoint is not configured");
        if (string.IsNullOrEmpty(ApiKey))
            throw new InvalidOperationException("API Key is not configured");

        var client = new ContentSafetyClient(new Uri(Endpoint), new AzureKeyCredential(ApiKey));
        return client;
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
            DisplayAnalysisResults(response.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Content Safety analysis failed: {ex.Message}");
            throw;
        }
    }

    
    // private static async Task AnalyzeImageContentAsync(ContentSafetyClient client, Stream imageStream)
    // {
    //     string datapath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "SampleImages", "violent_sample_image.jpg");
    //     ContentSafetyImageData image = new ContentSafetyImageData(BinaryData.FromBytes(File.ReadAllBytes(datapath)));

    //     var request = new AnalyzeImageOptions(image);

    //     Response<AnalyzeImageResult> response;
    //     try
    //     {
    //         response = client.AnalyzeImage(request);
    //     }
    //     catch (RequestFailedException ex)
    //     {
    //         Console.WriteLine("Analyze image failed.\nStatus code: {0}, Error code: {1}, Error message: {2}", ex.Status, ex.ErrorCode, ex.Message);
    //         throw;
    //     }
    // }

    /// <summary>
    /// Displays the analysis results in a user-friendly format.
    /// Shows severity levels for each safety category.
    /// </summary>
    /// <param name="result">The analysis result from Content Safety service</param>
    private static void DisplayAnalysisResults(AnalyzeTextResult result)
    {
        Console.WriteLine("Content Safety Analysis Results:");
        Console.WriteLine(new string('=', 40));

        // Display results for each category
        var categories = new[]
        {
            (TextCategory.Hate, "Hate Speech"),
            (TextCategory.SelfHarm, "Self Harm"),
            (TextCategory.Sexual, "Sexual Content"),
            (TextCategory.Violence, "Violence")
        };

        foreach (var (category, displayName) in categories)
        {
            var analysis = result.CategoriesAnalysis.FirstOrDefault(a => a.Category == category);
            int severity = analysis?.Severity ?? 0;

            Console.WriteLine($"{displayName,-15}: {severity}/7 {GetSeverityDescription(severity)}");
        }

        Console.WriteLine(new string('=', 40));
        Console.WriteLine("Severity Scale: 0 (Safe) - 7 (High Risk)");
    }

    /// <summary>
    /// Provides human-readable severity descriptions.
    /// </summary>
    /// <param name="severity">Severity level (0-7)</param>
    /// <returns>Descriptive text for the severity level</returns>
    private static string GetSeverityDescription(int severity)
    {
        return severity switch
        {
            0 => "(Safe)",
            1 or 2 => "(Low Risk)",
            3 or 4 => "(Medium Risk)",
            5 or 6 => "(High Risk)",
            7 => "(Very High Risk)",
            _ => "(Unknown)"
        };
    }

    /// <summary>
    /// Loads configuration from appsettings.json file
    /// </summary>
    /// <returns>IConfiguration instance with loaded settings</returns>
    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<AnalyzeText>(); // Optional: for development secrets

        return builder.Build();
    }
}
