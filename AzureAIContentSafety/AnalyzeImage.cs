using Azure.AI.ContentSafety.Samples.utils;
using Azure.AI.ContentSafety.Utils;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Azure.AI.ContentSafety.Samples;

public static class AnalyzeImage
{
    /// <summary>
    /// Analyzes image content for safety using the provided configuration.
    /// </summary>
    public static async Task AnalyzeImageAsync(IConfiguration configuration)
    {
        try
        {
            Console.WriteLine("=== IMAGE ANALYSIS MODE ===");
            Console.WriteLine(new string('-', 50));

            var dataPath = GetLocalPath();

            var imageToAnalyze = new ContentSafetyImageData(BinaryData.FromBytes(File.ReadAllBytes(dataPath)));
            var request = new AnalyzeImageOptions(imageToAnalyze);

            var client = ContentSafetyClientFactory.CreateContentSafetyClient(configuration);

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

            Console.WriteLine("\nImage analysis completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private static string GetLocalPath()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

        if (string.IsNullOrEmpty(assemblyDirectory))
        {
            Console.WriteLine("Could not determine assembly directory");
            throw new InvalidOperationException("Assembly directory is not available.");
        }

        var dataPath = Path.Combine(assemblyDirectory, "..", "..", "..", "SampleImages", "violent_sample_image.jpg");
        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"Sample image not found at: {dataPath}");
            throw new InvalidOperationException("File doesn't exist");
        }

        return dataPath;
    }
}
