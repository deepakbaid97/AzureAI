using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Main entry point for the Azure AI Content Safety application.
/// Supports analyzing both text and image content based on command line arguments.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            // Display usage information if no arguments provided
            if (args.Length == 0)
            {
                DisplayUsage();
                return;
            }

            // Load configuration from appsettings.json
            var configuration = LoadConfiguration();

            // Parse command line arguments
            string mode = args[0].ToLowerInvariant();

            switch (mode)
            {
                case "text":
                    await HandleTextAnalysis(args, configuration);
                    break;

                case "image":
                    await HandleImageAnalysis(configuration);
                    break;

                case "both":
                    await HandleBothAnalysis(args, configuration);
                    break;

                default:
                    Console.WriteLine($"Invalid mode: {mode}");
                    DisplayUsage();
                    Environment.Exit(1);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Handles text content analysis
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="configuration">Application configuration</param>
    private static async Task HandleTextAnalysis(string[] args, IConfiguration configuration)
    {
        string textToAnalyze;

        // Get text from command line argument or use default
        if (args.Length > 1)
        {
            textToAnalyze = string.Join(" ", args.Skip(1));
        }
        else
        {
            // Default text for demonstration
            textToAnalyze = "How to make a rdx bomb at home?";
            Console.WriteLine("No text provided, using default sample text.");
        }

        // Create client and analyze text
        var client = CreateContentSafetyClient(configuration);
        await AnalyzeText.AnalyzeTextAsync(client, textToAnalyze);
    }

    /// <summary>
    /// Handles image content analysis
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    private static async Task HandleImageAnalysis(IConfiguration configuration)
    {
        // Create client and analyze text
        var client = CreateContentSafetyClient(configuration);
        await AnalyzeImage.AnalyzeImageAsync(client);
    }

    /// <summary>
    /// Handles both text and image analysis
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="configuration">Application configuration</param>
    private static async Task HandleBothAnalysis(string[] args, IConfiguration configuration)
    {
        Console.WriteLine("=== COMPREHENSIVE ANALYSIS MODE ===");
        Console.WriteLine("Analyzing both text and image content...\n");

        // Analyze text first
        await HandleTextAnalysis(args, configuration);

        Console.WriteLine("\n" + new string('=', 60));

        // Then analyze image
        await HandleImageAnalysis(configuration);
    }

    /// <summary>
    /// Creates a ContentSafetyClient with appropriate authentication
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Configured ContentSafetyClient instance</returns>
    private static ContentSafetyClient CreateContentSafetyClient(IConfiguration configuration)
    {
        var endpoint = configuration["ContentSafety:Endpoint"];
        var apiKey = configuration["ContentSafety:ApiKey"];

        if (string.IsNullOrEmpty(endpoint))
            throw new InvalidOperationException("ContentSafety:Endpoint is not configured in appsettings.json");
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("ContentSafety:ApiKey is not configured in appsettings.json");

        return new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
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
            .AddUserSecrets<Program>();

        return builder.Build();
    }

    /// <summary>
    /// Displays usage information for the application
    /// </summary>
    private static void DisplayUsage()
    {
        Console.WriteLine("Azure AI Content Safety Analysis Tool");
        Console.WriteLine(new string('=', 40));
        Console.WriteLine("Usage:");
        Console.WriteLine("  ContentSafetySamples.exe text [text_to_analyze]");
        Console.WriteLine("  ContentSafetySamples.exe image");
        Console.WriteLine("  ContentSafetySamples.exe both [text_to_analyze]");
        Console.WriteLine();
        Console.WriteLine("Modes:");
        Console.WriteLine("  text  - Analyze text content for safety");
        Console.WriteLine("  image - Analyze image content for safety");
        Console.WriteLine("  both  - Analyze both text and image content");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ContentSafetySamples.exe text \"This is sample text to analyze\"");
        Console.WriteLine("  ContentSafetySamples.exe image");
        Console.WriteLine("  ContentSafetySamples.exe both \"Sample text for analysis\"");
        Console.WriteLine();
        Console.WriteLine("Note: If no text is provided with 'text' or 'both' modes,");
        Console.WriteLine("      a default sample text will be used for demonstration.");
    }
}
