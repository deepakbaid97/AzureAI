using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

namespace AIVision.CustomVision;

public class ObjectDetector
{
    // KEYS REQUIRED:
    // setx VISION_TRAINING_KEY <your-training-key>
    // setx VISION_TRAINING_ENDPOINT <your-training-endpoint>
    // setx VISION_PREDICTION_KEY <your-prediction-key>
    // setx VISION_PREDICTION_ENDPOINT <your-prediction-endpoint>
    // setx VISION_PREDICTION_RESOURCE_ID <your-resource-id>
    //
    // Get your keys from =  https://www.customvision.ai -> Setttings (Icon)


    string trainingEndpoint = Environment.GetEnvironmentVariable("VISION_TRAINING_ENDPOINT");

    string trainingKey = Environment.GetEnvironmentVariable("VISION_TRAINING_KEY");
    string predictionEndpoint = Environment.GetEnvironmentVariable("VISION_PREDICTION_ENDPOINT");
    string predictionKey = Environment.GetEnvironmentVariable("VISION_PREDICTION_KEY");

    private static Iteration iteration;
    private static string publishedModelName = "CustomODModel";

    private static Tag forkTag;
    private static Tag scissorsTag;

    public ObjectDetector()
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

    private CustomVisionTrainingClient AuthenticateTraining(string endpoint, string trainingKey)
    {
        // Create the Api, passing in the training key
        CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(trainingKey))
        {
            Endpoint = endpoint
        };
        return trainingApi;
    }
    private CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
    {
        // Create a prediction endpoint, passing in the obtained prediction key
        CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };
        return predictionApi;
    }

    private Project CreateProject(CustomVisionTrainingClient trainingApi)
    {
        // Find the object detection domain
        var domains = trainingApi.GetDomains();
        var objDetectionDomain = domains.FirstOrDefault(d => d.Type == "ObjectDetection");

        // Create a new project
        Console.WriteLine("Creating new project:");
        var project = trainingApi.CreateProject("My New Project", null, objDetectionDomain.Id);

        return project;
    }

    private void AddTags(CustomVisionTrainingClient trainingApi, Project project)
    {
        // Make two tags in the new project
        forkTag = trainingApi.CreateTag(project.Id, "fork");
        scissorsTag = trainingApi.CreateTag(project.Id, "scissors");
    }

    private void UploadImages(CustomVisionTrainingClient trainingApi, Project project)
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

        // Save the contents from - https://github.com/Azure-Samples/cognitive-services-sample-data-files/tree/master/CustomVision/ObjectDetection/Images

        // For your own projects, if you don't have a click-and-drag utility to mark the coordinates of regions, you can use the web UI at the - https://www.customvision.ai/
        Dictionary<string, double[]> fileToRegionMap = new Dictionary<string, double[]>()
        {
            // FileName, Left, Top, Width, Height
            {"scissors_1", new double[] { 0.4007353, 0.194068655, 0.259803921, 0.6617647 } },
            {"scissors_2", new double[] { 0.426470578, 0.185898721, 0.172794119, 0.5539216 } },
            {"scissors_3", new double[] { 0.289215684, 0.259428144, 0.403186262, 0.421568632 } },
            {"scissors_4", new double[] { 0.343137264, 0.105833367, 0.332107842, 0.8055556 } },
            {"scissors_5", new double[] { 0.3125, 0.09766343, 0.435049027, 0.71405226 } },
            {"scissors_6", new double[] { 0.379901975, 0.24308826, 0.32107842, 0.5718954 } },
            {"scissors_7", new double[] { 0.341911763, 0.20714055, 0.3137255, 0.6356209 } },
            {"scissors_8", new double[] { 0.231617644, 0.08459154, 0.504901946, 0.8480392 } },
            {"scissors_9", new double[] { 0.170343131, 0.332957536, 0.767156839, 0.403594762 } },
            {"scissors_10", new double[] { 0.204656869, 0.120539248, 0.5245098, 0.743464053 } },
            {"scissors_11", new double[] { 0.05514706, 0.159754932, 0.799019635, 0.730392158 } },
            {"scissors_12", new double[] { 0.265931368, 0.169558853, 0.5061275, 0.606209159 } },
            {"scissors_13", new double[] { 0.241421565, 0.184264734, 0.448529422, 0.6830065 } },
            {"scissors_14", new double[] { 0.05759804, 0.05027781, 0.75, 0.882352948 } },
            {"scissors_15", new double[] { 0.191176474, 0.169558853, 0.6936275, 0.6748366 } },
            {"scissors_16", new double[] { 0.1004902, 0.279036, 0.6911765, 0.477124184 } },
            {"scissors_17", new double[] { 0.2720588, 0.131977156, 0.4987745, 0.6911765 } },
            {"scissors_18", new double[] { 0.180147052, 0.112369314, 0.6262255, 0.6666667 } },
            {"scissors_19", new double[] { 0.333333343, 0.0274019931, 0.443627447, 0.852941155 } },
            {"scissors_20", new double[] { 0.158088237, 0.04047389, 0.6691176, 0.843137264 } },
            {"fork_1", new double[] { 0.145833328, 0.3509314, 0.5894608, 0.238562092 } },
            {"fork_2", new double[] { 0.294117659, 0.216944471, 0.534313738, 0.5980392 } },
            {"fork_3", new double[] { 0.09191177, 0.0682516545, 0.757352948, 0.6143791 } },
            {"fork_4", new double[] { 0.254901975, 0.185898721, 0.5232843, 0.594771266 } },
            {"fork_5", new double[] { 0.2365196, 0.128709182, 0.5845588, 0.71405226 } },
            {"fork_6", new double[] { 0.115196079, 0.133611143, 0.676470637, 0.6993464 } },
            {"fork_7", new double[] { 0.164215669, 0.31008172, 0.767156839, 0.410130739 } },
            {"fork_8", new double[] { 0.118872553, 0.318251669, 0.817401946, 0.225490168 } },
            {"fork_9", new double[] { 0.18259804, 0.2136765, 0.6335784, 0.643790841 } },
            {"fork_10", new double[] { 0.05269608, 0.282303959, 0.8088235, 0.452614367 } },
            {"fork_11", new double[] { 0.05759804, 0.0894935, 0.9007353, 0.3251634 } },
            {"fork_12", new double[] { 0.3345588, 0.07315363, 0.375, 0.9150327 } },
            {"fork_13", new double[] { 0.269607842, 0.194068655, 0.4093137, 0.6732026 } },
            {"fork_14", new double[] { 0.143382356, 0.218578458, 0.7977941, 0.295751631 } },
            {"fork_15", new double[] { 0.19240196, 0.0633497, 0.5710784, 0.8398692 } },
            {"fork_16", new double[] { 0.140931368, 0.480016381, 0.6838235, 0.240196079 } },
            {"fork_17", new double[] { 0.305147052, 0.2512582, 0.4791667, 0.5408496 } },
            {"fork_18", new double[] { 0.234068632, 0.445702642, 0.6127451, 0.344771236 } },
            {"fork_19", new double[] { 0.219362751, 0.141781077, 0.5919118, 0.6683006 } },
            {"fork_20", new double[] { 0.180147052, 0.239820287, 0.6887255, 0.235294119 } }
        };

        // Add all images for fork
        var imagePath = Path.Combine("Images", "fork");
        var imageFileEntries = new List<ImageFileCreateEntry>();
        foreach (var fileName in Directory.EnumerateFiles(imagePath))
        {
            var region = fileToRegionMap[Path.GetFileNameWithoutExtension(fileName)];
            imageFileEntries.Add(new ImageFileCreateEntry(fileName, File.ReadAllBytes(fileName), null, new List<Region>(new Region[] { new Region(forkTag.Id, region[0], region[1], region[2], region[3]) })));
        }
        trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFileEntries));

        // Add all images for scissors
        imagePath = Path.Combine("Images", "scissors");
        imageFileEntries = new List<ImageFileCreateEntry>();
        foreach (var fileName in Directory.EnumerateFiles(imagePath))
        {
            var region = fileToRegionMap[Path.GetFileNameWithoutExtension(fileName)];
            imageFileEntries.Add(new ImageFileCreateEntry(fileName, File.ReadAllBytes(fileName), null, new List<Region>(new Region[] { new Region(scissorsTag.Id, region[0], region[1], region[2], region[3]) })));
        }
        trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFileEntries));
    }

    private void TrainProject(CustomVisionTrainingClient trainingApi, Project project)
    {

        // Now there are images with tags start training the project
        Console.WriteLine("\tTraining");
        iteration = trainingApi.TrainProject(project.Id);

        // The returned iteration will be in progress, and can be queried periodically to see when it has completed
        while (iteration.Status == "Training")
        {
            Thread.Sleep(1000);

            // Re-query the iteration to get its updated status
            iteration = trainingApi.GetIteration(project.Id, iteration.Id);
        }
    }

    private void PublishIteration(CustomVisionTrainingClient trainingApi, Project project)
    {

        // The iteration is now trained. Publish it to the prediction end point.
        var predictionResourceId = Environment.GetEnvironmentVariable("VISION_PREDICTION_RESOURCE_ID");
        trainingApi.PublishIteration(project.Id, iteration.Id, publishedModelName, predictionResourceId);
        Console.WriteLine("Done!\n");
    }

    private void TestIteration(CustomVisionPredictionClient predictionApi, Project project)
    {

        // Make a prediction against the new project
        Console.WriteLine("Making a prediction:");
        var imageFile = Path.Combine("Images", "test", "test_image.jpg");
        using (var stream = File.OpenRead(imageFile))
        {
            var result = predictionApi.DetectImage(project.Id, publishedModelName, stream);

            // Loop over each prediction and write out the results
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"\t{c.TagName}: {c.Probability:P1} [ {c.BoundingBox.Left}, {c.BoundingBox.Top}, {c.BoundingBox.Width}, {c.BoundingBox.Height} ]");
            }
        }
        Console.ReadKey();
    }
}
