using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;

class Program
{
    static async Task Main(string[] args)
    {
        string speechKey = "SUBSCRIPTION_KEY";
        string speechRegion = "REGION";

        // Configure translation
        var translationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);

        // Source language (what user speaks)
        translationConfig.SpeechRecognitionLanguage = "en-US";

        // Add target languages
        translationConfig.AddTargetLanguage("fr"); // French
        translationConfig.AddTargetLanguage("de"); // German

        using var recognizer = new TranslationRecognizer(translationConfig);

        Console.WriteLine("Say something in English...");

        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.TranslatedSpeech)
        {
            Console.WriteLine($"Recognized (EN): {result.Text}");
            foreach (var element in result.Translations)
            {
                Console.WriteLine($"Translated ({element.Key}): {element.Value}");
            }
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

