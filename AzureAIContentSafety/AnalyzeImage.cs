using Azure.AI.ContentSafety.Utils;
using System.Reflection;

namespace Azure.AI.ContentSafety.Samples;

public static class AnalyzeImage
{
    /// <summary>
    /// Analyzes image content for safety using the provided configuration.
    /// </summary>
    public static async Task AnalyzeImageAsync(ContentSafetyClient client)
    {
        try
        {
            Console.WriteLine("=== IMAGE ANALYSIS MODE ===");
            Console.WriteLine(new string('-', 50));

            // Analyze the image content
            await AnalyzeImageContentAsync(client);

            Console.WriteLine("\nImage analysis completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Analyzes image content for safety categories including hate speech, 
    /// self-harm, sexual content, and violence.
    /// </summary>
    /// <param name="client">The ContentSafetyClient instance</param>
    private static async Task AnalyzeImageContentAsync(ContentSafetyClient client)
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        if (string.IsNullOrEmpty(assemblyDirectory))
        {
            Console.WriteLine("Could not determine assembly directory");
            return;
        }

        var dataPath = Path.Combine(assemblyDirectory, "..", "..", "..", "SampleImages", "violent_sample_image.jpg");
        
        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"Sample image not found at: {dataPath}");
            return;
        }

        var image = new ContentSafetyImageData(BinaryData.FromBytes(File.ReadAllBytes(dataPath)));
        var request = new AnalyzeImageOptions(image);

        Console.WriteLine($"Analyzing image: {Path.GetFileName(dataPath)}");

        Response<AnalyzeImageResult> response;
        try
        {
            response = await client.AnalyzeImageAsync(request);
            
            // Display the results
            DisplayAnalysisResultHelper.DisplayAnalysisResults(response.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image analysis failed: {ex.Message}");
            throw;
        }
    }
}
