using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Main entry point for the Azure AI Content Safety application.
/// Supports analyzing both text and image content based on command line arguments.
/// </summary>
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Azure AI Content Safety Analysis Tool");
        Console.WriteLine("Enter any option");
        Console.WriteLine("1. Text Analysis");
        Console.WriteLine("2. Image Analysis");
        Console.WriteLine("3. Shield Prompt Analysis (Jailbreaking Detection)");
        Console.WriteLine("4. Protected Material Detection (Text)");
        Console.WriteLine("5. Protected Material Detection (Code)");
        Console.WriteLine("6. Create Blocklist and Add Items");
        Console.WriteLine("7. List Blocklist Items");
        Console.WriteLine("8. Analyze Text with Blocklist");
        Console.WriteLine();

        int option = int.Parse(Console.ReadLine());


        // Load configuration from appsettings.json
        var configuration = LoadConfiguration();

        switch (option)
        {
            case 1:
                await AnalyzeText.AnalyzeTextAsync(configuration);
                break;
            case 2:
                await AnalyzeImage.AnalyzeImageAsync(configuration);
                break;
            case 3:
                await ShieldPrompt.AnalyzeShieldPromptAsync(configuration);
                break;
            case 4:
                await ProtectedMaterialDetection.AnalyzeProtectedTextAsync(configuration);
                break;
            case 5:
                await ProtectedMaterialDetection.AnalyzeProtectedCodeAsync(configuration);
                break;
            case 6:
                await BlocklistManager.CreateBlocklistAndAddItemsAsync(configuration);
                break;
            case 7:
                await BlocklistManager.ListBlocklistItemsAsync(configuration);
                break;
            case 8:
                await BlocklistManager.AnalyzeTextWithBlocklistAsync(configuration);
                break;

        }
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
}
