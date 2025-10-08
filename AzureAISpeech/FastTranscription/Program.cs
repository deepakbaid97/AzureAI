using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Transcription;

class Program
{
    static async Task Main()
    {
        string speechKey = "SUBSCRIPTION_KEY";
        string speechRegion = "REGION";

        // 🔹 Input audio file for transcription
        string audioFilePath = "deeksha_enroll.wav";

        // Create a speech config
        var config = SpeechConfig.FromSubscription(speechKey, speechRegion);
        config.SpeechRecognitionLanguage = "en-US";  // Set your language

        // 🔹 Fast Transcription API
        var audioConfig = AudioConfig.FromWavFileInput(audioFilePath);
        using var recognizer = new SpeechRecognizer(config, audioConfig);

        Console.WriteLine("Transcribing audio file...");

        // Perform recognition
        var result = await recognizer.RecognizeOnceAsync();

        // Show result
        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            Console.WriteLine($"Transcription: {result.Text}");
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            Console.WriteLine("No speech could be recognized.");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            Console.WriteLine($"Canceled: {cancellation.Reason}, {cancellation.ErrorDetails}");
        }
    }
}
