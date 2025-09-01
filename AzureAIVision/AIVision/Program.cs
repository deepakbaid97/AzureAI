using AIVision.Face;
using AIVision.Image_Analysis;
using Azure;
using Azure.AI.Vision.Face;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.IO;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace AzureAI.AzureAIVision.AIVision.Samples;

public class Program
{
    public static void Main (string[] args)
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

        if (string.IsNullOrEmpty(assemblyDirectory))
        {
            Console.WriteLine("Could not determine assembly directory");
            return;
        }

        // RunImageAnalysis(assemblyDirectory);

        RunFaceAnalysis(assemblyDirectory);
    }

    private static void RunImageAnalysis(String assemblyDirectory)
    {
        var dataPath = Path.Combine(assemblyDirectory, "..", "..", "..", "Images", "testing-image.png");

        string endpoint = Environment.GetEnvironmentVariable("VISION_ENDPOINT");
        string key = Environment.GetEnvironmentVariable("VISION_KEY");

        // Create an Image Analysis client.
        ImageAnalysisClient client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

        ImageAnalysis analysis = new ImageAnalysis(client);

        analysis.Anayse(VisualFeatures.Caption | VisualFeatures.Read, dataPath);
    }

    private static void RunFaceAnalysis(String assemblyDirectory)
    {
        var dataPath = Path.Combine(assemblyDirectory, "..", "..", "..", "Images", "people.jpg");

        string endpoint = Environment.GetEnvironmentVariable("VISION_ENDPOINT");
        string key = Environment.GetEnvironmentVariable("VISION_KEY");

        FaceClient client = new FaceClient(new Uri(endpoint), new AzureKeyCredential(key));

        FaceAnalysis analysis = new FaceAnalysis(client);

        analysis.Analyse(dataPath);
    }

}
