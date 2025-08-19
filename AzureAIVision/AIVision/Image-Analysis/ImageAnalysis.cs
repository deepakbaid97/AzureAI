using Azure.AI.Vision.ImageAnalysis;

namespace AIVision.Image_Analysis;

public class ImageAnalysis(ImageAnalysisClient client)
{
    public void Anayse(VisualFeatures visualFeatures, String dataPath)
    {
        if (visualFeatures == VisualFeatures.None)
        {
            throw new ArgumentException("At least one visual feature must be specified.", nameof(visualFeatures));
        }
        // Perform the image analysis using the provided client and options.
        ImageAnalysisResult result = client.Analyze(
            BinaryData.FromBytes(File.ReadAllBytes(dataPath)),
            visualFeatures,
            new ImageAnalysisOptions { GenderNeutralCaption = true });

        // Check properties dynamically and invoke appropriate methods
        if (result.Caption != null)
        {
            AnalyseCaptions(result);
        }

        if (result.Read != null)
        {
            AnalyseRead(result);
        }

        if (result.DenseCaptions != null)
        {
            AnalyseDenseCaptions(result);
        }

        if (result.Objects != null)
        {
            AnalyseObjects(result);
        }

        if (result.People != null)
        {
            AnalysePeople(result);
        }

        if (result.SmartCrops != null)
        {
            AnalyseSmartCrops(result);
        }

        if (result.Tags != null)
        {
            AnalyseTags(result);
        }
    }

    private void AnalyseCaptions(ImageAnalysisResult result)
    {
        if (result.Caption != null)
        {
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" Caption:");
            Console.WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");
        }
        else
        {
            Console.WriteLine("No caption available for this image.");
        }

        Console.WriteLine("==========================================================");
    }

    private void AnalyseRead(ImageAnalysisResult result)
    {
        if (result.Read != null && result.Read.Blocks.Count > 0)
        {
            Console.WriteLine($"Image read results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" Text:");
            foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
            {
                Console.WriteLine($"   Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
                foreach (DetectedTextWord word in line.Words)
                {
                    Console.WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
                }
            }
        }
        else
        {
            Console.WriteLine("No text detected in the image.");
        }

        Console.WriteLine("==========================================================");
    }

    private void AnalyseDenseCaptions(ImageAnalysisResult result)
    {
        if (result.DenseCaptions != null && result.DenseCaptions.Values.Count > 0)
        {
            // Print dense caption results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" Dense Captions:");
            foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
            {
                Console.WriteLine($"   Region: '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
            }
        }
        else
        {
            Console.WriteLine("No dense captions available for this image.");
        }

        Console.WriteLine("==========================================================");
    }

    private void AnalyseObjects(ImageAnalysisResult result)
    {
        if (result.Objects != null && result.Objects.Values.Count > 0)
        {
            // Print object detection results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" Objects:");
            foreach (DetectedObject detectedObject in result.Objects.Values)
            {
                Console.WriteLine($"   Object: '{detectedObject.Tags.First().Name}', Bounding box {detectedObject.BoundingBox.ToString()}");
            }
        }
        else
        {
            Console.WriteLine("No objects detected in the image.");
        }

        Console.WriteLine("==========================================================");
    }

    private void AnalysePeople(ImageAnalysisResult result)
    {
        if (result.People != null && result.People.Values.Count > 0)
        {
            // Print people detection results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" People:");
            foreach (DetectedPerson person in result.People.Values)
            {
                Console.WriteLine($"   Person: Bounding box {person.BoundingBox.ToString()}, Confidence {person.Confidence:F4}");
            }
        }
        else
        {
            Console.WriteLine("No people detected in the image.");
        }

        Console.WriteLine("==========================================================");
    }

    private void AnalyseSmartCrops(ImageAnalysisResult result)
    {
        if (result.SmartCrops != null && result.SmartCrops.Values.Count > 0)
        {
            // Print smart-crops analysis results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" SmartCrops:");
            foreach (CropRegion cropRegion in result.SmartCrops.Values)
            {
                Console.WriteLine($"   Aspect ratio: {cropRegion.AspectRatio}, Bounding box: {cropRegion.BoundingBox}");
            }
        }
        else
        {
            Console.WriteLine("No smart crops available for this image.");
        }

        Console.WriteLine("==========================================================");
    }

    private void AnalyseTags(ImageAnalysisResult result)
    {
        if (result.Tags != null && result.Tags.Values.Count > 0)
        {
            // Print tags results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" Tags:");
            foreach (DetectedTag tag in result.Tags.Values)
            {
                Console.WriteLine($"   '{tag.Name}', Confidence {tag.Confidence:F4}");
            }
        }
        else
        {
            Console.WriteLine("No tags available for this image.");
        }

        Console.WriteLine("==========================================================");
    }
}
