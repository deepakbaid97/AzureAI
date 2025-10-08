using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;

class Program
{
    static async Task Main(string[] args)
    {
        string speechKey = "SUBSCRIPTION_KEY";
        string speechRegion = "REGION";

        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

        // Use default microphone as input
        using var recognizer = new SpeechRecognizer(speechConfig);

        Console.WriteLine("Say something...");

        // Recognize once (short utterance)
        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            Console.WriteLine($"Recognized: {result.Text}");
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            Console.WriteLine("No speech could be recognized.");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            Console.WriteLine($"Canceled: {cancellation.Reason}");
            if (cancellation.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"ErrorCode: {cancellation.ErrorCode}");
                Console.WriteLine($"ErrorDetails: {cancellation.ErrorDetails}");
            }
        }
    }
}


