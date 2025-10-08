using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string speechKey = "SUBSCRIPTION_KEY";
        string speechRegion = "REGION";

        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

        // Enable auto language detection
        var autoDetectConfig = AutoDetectSourceLanguageConfig.FromLanguages(
            new string[] { "en-US", "es-ES", "fr-FR" });

        using var audioConfig = AudioConfig.FromWavFileInput("multi_language_speech.wav");

        using var recognizer = new SpeechRecognizer(
            speechConfig,
            autoDetectConfig,
            audioConfig);

        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            var detectedLanguage = AutoDetectSourceLanguageResult.FromResult(result);
            Console.WriteLine($"Recognized: {result.Text}");
            Console.WriteLine($"Detected language: {detectedLanguage.Language}");
        }
        else
        {
            Console.WriteLine($"Recognition failed: {result.Reason}");
        }
    }
}
