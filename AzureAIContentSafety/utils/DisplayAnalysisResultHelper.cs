using Azure.AI.ContentSafety;
using System;
using System.Linq;
using System.Text.Json;

namespace Azure.AI.ContentSafety.Utils;

public static class DisplayAnalysisResultHelper
{
    /// <summary>
    /// Displays the analysis results in a user-friendly format.
    /// Shows severity levels for each safety category.
    /// </summary>
    /// <param name="result">The analysis result from Content Safety service</param>
    public static void DisplayAnalysisResults(AnalyzeTextResult result)
    {
        Console.WriteLine("Content Safety Analysis Results:");
        Console.WriteLine(new string('=', 40));

        // Display results for each category
        (TextCategory, string)[] categories = new[]
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
    /// Displays the image analysis results in a user-friendly format.
    /// Shows severity levels for each safety category.
    /// </summary>
    /// <param name="result">The analysis result from Content Safety service</param>
    public static void DisplayAnalysisResults(AnalyzeImageResult result)
    {
        Console.WriteLine("Image Content Safety Analysis Results:");
        Console.WriteLine(new string('=', 40));

        // Display results for each category
        var categories = new[]
        {
            (ImageCategory.Hate, "Hate Speech"),
            (ImageCategory.SelfHarm, "Self Harm"),
            (ImageCategory.Sexual, "Sexual Content"),
            (ImageCategory.Violence, "Violence")
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
    /// Displays the results from the Shield Prompt API analysis.
    /// </summary>
    /// <param name="result">The JSON response from the API</param>
    public static void DisplayShieldPromptResults(JsonDocument result)
    {
        try
        {
            Console.WriteLine("\n=== SHIELD PROMPT ANALYSIS RESULTS ===");

            var root = result.RootElement;

            // Display overall attack detected status
            if (root.TryGetProperty("userPromptAnalysis", out var userPromptAnalysis))
            {
                if (userPromptAnalysis.TryGetProperty("attackDetected", out var attackDetected))
                {
                    var isAttackDetected = attackDetected.GetBoolean();
                    Console.WriteLine($"Attack Detected: {(isAttackDetected ? "YES" : "NO")}");                    
                }
            }

            // Display document analysis results if available
            if (root.TryGetProperty("documentsAnalysis", out var documentsAnalysis) && 
                documentsAnalysis.ValueKind == JsonValueKind.Array)
            {
                Console.WriteLine("\n--- Dclsocument Analysis ---");
                for (int i = 0; i < documentsAnalysis.GetArrayLength(); i++)
                {
                    var doc = documentsAnalysis[i];
                    Console.WriteLine($"Document {i + 1}:");
                    
                    if (doc.TryGetProperty("attackDetected", out var docAttackDetected))
                    {
                        var isDocAttackDetected = docAttackDetected.GetBoolean();
                        Console.WriteLine($"  Attack Detected: {(isDocAttackDetected ? "YES" : "NO")}");
                    }
                }
            }

            // Display raw JSON for detailed analysis (optional)
            Console.WriteLine("\n--- Raw API Response ---");
            Console.WriteLine(JsonSerializer.Serialize(result.RootElement, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error displaying Shield Prompt results: {ex.Message}");
            Console.WriteLine("Raw response:");
            Console.WriteLine(JsonSerializer.Serialize(result.RootElement, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }

    /// <summary>
    /// Provides human-readable severity descriptions.
    /// </summary>
    /// <param name="severity">Severity level (0-7)</param>
    /// <returns>Descriptive text for the severity level</returns>
    public static string GetSeverityDescription(int severity)
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
}
