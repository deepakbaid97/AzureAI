using Azure.AI.ContentSafety.Utils;
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
    /// <summary>
    /// Demonstrates blocklist concepts and shows how text analysis would work with blocklists.
    /// In a real implementation, this would create actual blocklists via REST API.
    /// </summary>
    /// <param name="configuration">Application configuration containing endpoint and API key</param>
    /// <param name="blocklistName">Name for the custom blocklist</param>
    /// <param name="textToAnalyze">Text to analyze</param>
    /// <param name="blocklistItems">Array of items that would be in the blocklist</param>
    public static async Task CreateBlocklistAndAddItemsAsync(
        IConfiguration configuration,
        string blocklistName,
        string blocklistDescription,
        List<string> blocklistItems)
    {
        try
        {
            Console.WriteLine("=== BLOCKLIST MANAGEMENT DEMONSTRATION ===");
            Console.WriteLine(new string('-', 50));

            var blocklistClient = CreateBlocklistClient(configuration);

            var data = new
            {
                description = blocklistDescription,
            };

            // create blocklist
            var createResponse = await blocklistClient.CreateOrUpdateTextBlocklistAsync(blocklistName, RequestContent.Create(data));

            if (createResponse.Status == 201)
            {
                Console.WriteLine("\nBlocklist {0} created.", blocklistName);
            }

            var items = blocklistItems.Select(x => new TextBlocklistItem(x)).ToArray();
            var addedBlocklistItems = blocklistClient.AddOrUpdateBlocklistItems(blocklistName, new AddOrUpdateTextBlocklistItemsOptions(items));

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

    public static async Task ListBlocklistItemsAsync(
        IConfiguration configuration,
        string blocklistName)
    {
        try
        {
            var blocklistClient = CreateBlocklistClient(configuration);

            // List items in the blocklist
            var blocklistItems = blocklistClient.GetTextBlocklistItemsAsync(blocklistName);

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
