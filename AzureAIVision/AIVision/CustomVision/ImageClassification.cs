using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

namespace AIVision.CustomVision;

public class ImageClassification
{
    // KEYS REQUIRED:
    // setx VISION_TRAINING_KEY <your-training-key>
    // setx VISION_TRAINING_ENDPOINT <your-training-endpoint>
    // setx VISION_PREDICTION_KEY <your-prediction-key>
    // setx VISION_PREDICTION_ENDPOINT <your-prediction-endpoint>
    // setx VISION_PREDICTION_RESOURCE_ID <your-resource-id>
    //
    // Get your keys from =  https://www.customvision.ai -> Setttings (Icon)


    // Retrieve the environment variables for your credentials:
    private static string trainingEndpoint = Environment.GetEnvironmentVariable("VISION_TRAINING_ENDPOINT");

    private static string trainingKey = Environment.GetEnvironmentVariable("VISION_TRAINING_KEY");
    private static string predictionEndpoint = Environment.GetEnvironmentVariable("VISION_PREDICTION_ENDPOINT");
    private static string predictionKey = Environment.GetEnvironmentVariable("VISION_PREDICTION_KEY");

    private static string predictionResourceId = Environment.GetEnvironmentVariable("VISION_PREDICTION_RESOURCE_ID");

    private static List<string> hemlockImages;
    private static List<string> japaneseCherryImages;
    private static Tag hemlockTag;
    private static Tag japaneseCherryTag;
    private static Iteration iteration;
    private static readonly string publishedModelName = "treeClassModel";
    private static MemoryStream testImage;

    public ImageClassification()
    {
        CustomVisionTrainingClient trainingApi = AuthenticateTraining(trainingEndpoint, trainingKey);
        CustomVisionPredictionClient predictionApi = AuthenticatePrediction(predictionEndpoint, predictionKey);

        Project project = CreateProject(trainingApi);
        AddTags(trainingApi, project);
        UploadImages(trainingApi, project);
        TrainProject(trainingApi, project);
        PublishIteration(trainingApi, project);
        TestIteration(predictionApi, project);
    }

    private static CustomVisionTrainingClient AuthenticateTraining(string endpoint, string trainingKey)
    {
        // Create the Api, passing in the training key
        CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(trainingKey))
        {
            Endpoint = endpoint
        };
        return trainingApi;
    }
    private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
    {
        // Create a prediction endpoint, passing in the obtained prediction key
        CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };
        return predictionApi;
    }

    private static Project CreateProject(CustomVisionTrainingClient trainingApi)
    {
        // Create a new project
        Console.WriteLine("Creating new project:");
        return trainingApi.CreateProject("My New Project");
    }

    private static void AddTags(CustomVisionTrainingClient trainingApi, Project project)
    {
        // Make two tags in the new project
        hemlockTag = trainingApi.CreateTag(project.Id, "Hemlock");
        japaneseCherryTag = trainingApi.CreateTag(project.Id, "Japanese Cherry");
    }

    private static void LoadImagesFromDisk()
    {
        // this loads the images to be uploaded from disk into memory
        hemlockImages = Directory.GetFiles(Path.Combine("Images", "Hemlock")).ToList();
        japaneseCherryImages = Directory.GetFiles(Path.Combine("Images", "Japanese_Cherry")).ToList();
        testImage = new MemoryStream(File.ReadAllBytes(Path.Combine("Images", "Test", "test_image.jpg")));
    }

    private static void UploadImages(CustomVisionTrainingClient trainingApi, Project project)
    {
        //In order to train your model effectively, use images with visual variety.Select images that vary by:
        //    camera angle
        //    lighting
        //    background
        //    visual style
        //    individual / grouped subject(s)
        //    size
        //    type
        //Additionally, make sure all of your training images meet the following criteria:
        //    must be .jpg, .png, .bmp, or.gif format
        //    no greater than 6 MB in size(4 MB for prediction images)
        //    no less than 256 pixels on the shortest edge; any images shorter than 256 pixels are automatically scaled up by the Custom Vision service

        // Save the contents from https://github.com/Azure-Samples/cognitive-services-sample-data-files/tree/master/CustomVision/ImageClassification/Images 
        
        // Add some images to the tags
        Console.WriteLine("\tUploading images");
        LoadImagesFromDisk();

        // Images can be uploaded one at a time
        foreach (var image in hemlockImages)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            {
                trainingApi.CreateImagesFromData(project.Id, stream, new List<Guid>() { hemlockTag.Id });
            }
        }

        // Or uploaded in a single batch 
        var imageFiles = japaneseCherryImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
        trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFiles, new List<Guid>() { japaneseCherryTag.Id }));

    }

    private static void TrainProject(CustomVisionTrainingClient trainingApi, Project project)
    {
        // Now there are images with tags start training the project
        Console.WriteLine("\tTraining");
        iteration = trainingApi.TrainProject(project.Id);

        // The returned iteration will be in progress, and can be queried periodically to see when it has completed
        while (iteration.Status == "Training")
        {
            Console.WriteLine("Waiting 10 seconds for training to complete...");
            Thread.Sleep(10000);

            // Re-query the iteration to get it's updated status
            iteration = trainingApi.GetIteration(project.Id, iteration.Id);
        }
    }

    private static void PublishIteration(CustomVisionTrainingClient trainingApi, Project project)
    {
        trainingApi.PublishIteration(project.Id, iteration.Id, publishedModelName, predictionResourceId);
        Console.WriteLine("Done!\n");

        // Now there is a trained endpoint, it can be used to make a prediction
    }

    private static void TestIteration(CustomVisionPredictionClient predictionApi, Project project)
    {

        // Make a prediction against the new project
        Console.WriteLine("Making a prediction:");
        var result = predictionApi.ClassifyImage(project.Id, publishedModelName, testImage);

        // Loop over each prediction and write out the results
        foreach (var c in result.Predictions)
        {
            Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
        }
    }
}
