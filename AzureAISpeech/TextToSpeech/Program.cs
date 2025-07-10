using Microsoft.CognitiveServices.Speech;

var subscriptionKey = "ADD_YOUR_SUBSCRIPTION_KEY";
var serviceRegion = "ADD_YOUR_REGION";

var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);

// https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=tts#text-to-speech
config.SpeechSynthesisVoiceName = "fr-CA-JeanNeural";

//en-US-JennyNeural
//fr-CA-JeanNeural
//fr-CA-SylvieNeural

using var synthesizer = new SpeechSynthesizer(config);

Console.WriteLine("Azure AI Text-to-Speech");
Console.Write("Enter text to synthesize: ");
var text = Console.ReadLine();

var result = await synthesizer.SpeakTextAsync(text);

if (result.Reason == ResultReason.SynthesizingAudioCompleted)
{
    Console.WriteLine("Speech synthesis completed successfully.");
}
else if (result.Reason == ResultReason.Canceled)
{
    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
    Console.WriteLine("Speech synthesis canceled.");
    Console.WriteLine($"Reason: {cancellation.Reason}");
    Console.WriteLine($"Error Code: {cancellation.ErrorCode}");
    Console.WriteLine($"Details: {cancellation.ErrorDetails}");
}