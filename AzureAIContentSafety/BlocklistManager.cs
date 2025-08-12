using Azure.AI.ContentSafety.Samples.utils;
using Azure.Core;
using Microsoft.Extensions.Configuration;

namespace Azure.AI.ContentSafety.Samples;

/// <summary>
/// Provides functionality to demonstrate Azure AI Content Safety blocklist concepts and usage.
/// Note: This is a conceptual demonstration. Actual blocklist management requires 
/// REST API calls or the full BlocklistClient when available.
/// </summary>
public static class BlocklistManager
{
    private const string BlocklistName = "ProhibitStockAnalysis";
    private const string BlocklistDescription = "Contains terms related to stock analysis.";
    private static readonly string[] BlocklistItems = [
            "Stock",
            "Fundamentals",
            "Market Analysis",
            "Investment Strategy",
        ];
    private const string TextToAnalyze = "The stock market is volatile, and understanding fundamentals is key to investment strategy.";

    public static async Task CreateBlocklistAndAddItemsAsync(IConfiguration configuration)
    {
        try
        {
            Console.WriteLine("=== BLOCKLIST MANAGEMENT DEMONSTRATION ===");
            Console.WriteLine(new string('-', 50));

            var blocklistClient = CreateBlocklistClient(configuration);

            var data = new
            {
                description = BlocklistDescription,
            };

            // create blocklist
            var createResponse = await blocklistClient.CreateOrUpdateTextBlocklistAsync(BlocklistName, RequestContent.Create(data));

            if (createResponse.Status == 201)
            {
                Console.WriteLine("\nBlocklist {0} created.", BlocklistName);
            }

            var items = BlocklistItems.Select(x => new TextBlocklistItem(x)).ToArray();
            var addedBlocklistItems = blocklistClient.AddOrUpdateBlocklistItems(BlocklistName, new AddOrUpdateTextBlocklistItemsOptions(items));

            if (addedBlocklistItems != null && addedBlocklistItems.Value != null)
            {
                Console.WriteLine("\nBlocklistItems added:");
                foreach (var addedBlocklistItem in addedBlocklistItems.Value.BlocklistItems)
                {
                    Console.WriteLine("BlocklistItemId: {0}, Text: {1}, Description: {2}", addedBlocklistItem.BlocklistItemId, addedBlocklistItem.Text, addedBlocklistItem.Description);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred in blocklist creation: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Lists all available blocklists.
    /// </summary>
    /// <param name="configuration">Application configuration containing endpoint and API key</param>
    public static async Task ListBlocklistsAsync(IConfiguration configuration)
    {
        try
        {
            var blocklistClient = CreateBlocklistClient(configuration);

            Console.WriteLine("\nListing all blocklists:");
            Console.WriteLine(new string('-', 30));

            var blocklists = blocklistClient.GetTextBlocklistsAsync();

            bool hasBlocklists = false;
            await foreach (var blocklist in blocklists.AsPages())
            {
                foreach (var item in blocklist.Values)
                {
                    hasBlocklists = true;
                    Console.WriteLine($"- Name: {item.Name}");
                    Console.WriteLine($"  Description: {item.Description}");
                    Console.WriteLine();
                }
            }

            if (!hasBlocklists)
            {
                Console.WriteLine("No blocklists found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred while listing blocklists: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public static async Task ListBlocklistItemsAsync(IConfiguration configuration)
    {
        try
        {
            var blocklistClient = CreateBlocklistClient(configuration);

            // List items in the blocklist
            var blocklistItems = blocklistClient.GetTextBlocklistItemsAsync(BlocklistName);

            Console.WriteLine("\nBlocklist Items:");
            bool hasItems = false;
            await foreach (var item in blocklistItems)
            {
                hasItems = true;
                Console.WriteLine($"- ID: {item.BlocklistItemId}, Text: {item.Text}");
            }

            if (!hasItems)
            {
                Console.WriteLine("No items found in the blocklist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred while listing blocklist items: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public static async Task AnalyzeTextWithBlocklistAsync(IConfiguration configuration)
    {
        var client = ContentSafetyClientFactory.CreateContentSafetyClient(configuration);
        try
        {
            Console.WriteLine("=== TEXT ANALYSIS WITH BLOCKLIST ===");
            Console.WriteLine(new string('-', 50));

            // Set up the analysis request with blocklist
            var request = new AnalyzeTextOptions(TextToAnalyze);
            request.BlocklistNames.Add(BlocklistName);
            request.HaltOnBlocklistHit = true; //Whether to halt analysis on first blocklist match

            var response = await client.AnalyzeTextAsync(request);

            // Display blocklist match results
            if (response.Value.BlocklistsMatch != null && response.Value.BlocklistsMatch.Any())
            {
                Console.WriteLine("🚫 BLOCKLIST MATCHES FOUND:");
                Console.WriteLine(new string('-', 30));
                foreach (var matchResult in response.Value.BlocklistsMatch)
                {
                    Console.WriteLine($"- Blocklist: {matchResult.BlocklistName}");
                    Console.WriteLine($"  Item ID: {matchResult.BlocklistItemId}");
                    Console.WriteLine($"  Matched Text: {matchResult.BlocklistItemText}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("✅ No blocklist matches found.");
            }

            // Display standard content safety results if available
            if (response.Value.CategoriesAnalysis != null && response.Value.CategoriesAnalysis.Any())
            {
                Console.WriteLine("\nSTANDARD CONTENT SAFETY ANALYSIS:");
                Console.WriteLine(new string('-', 35));
                foreach (var result in response.Value.CategoriesAnalysis)
                {
                    Console.WriteLine($"- {result.Category}: Severity {result.Severity}");
                }
            }

            Console.WriteLine($"\nAnalysis completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An error occurred during text analysis with blocklist: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private static BlocklistClient CreateBlocklistClient(IConfiguration configuration)
    {
        // Get endpoint and API key from configuration
        var endpoint = configuration["ContentSafety:Endpoint"];
        var apiKey = configuration["ContentSafety:ApiKey"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Content Safety endpoint and API key must be configured.");
        }

        var blocklistClient = new BlocklistClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        return blocklistClient;
    }
}
