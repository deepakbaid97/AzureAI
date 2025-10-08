using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

class Program
{
    static async Task Main()
    {
        string speechKey = "SUBSCRIPTION_KEY";
        string speechRegion = "REGION";

        var config = SpeechConfig.FromSubscription(speechKey, speechRegion);

    //    < voice name = 'en-US-AriaNeural' >
    //    Hello, how are you ?
    //</ voice >
    //< voice name = 'es-ES-ElviraNeural' >
    //    Hola, ¿cómo estás?
    //</ voice >
    //< voice name = 'fr-FR-DeniseNeural' >
    //    Bonjour, comment ça va ?
    //</ voice >

        // Build SSML with multiple voices/languages
        string ssml = @"
<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' 
       xmlns:mstts='https://www.w3.org/2001/mstts' xml:lang='en-US'>
    <voice name='es-ES-ElviraNeural'>
        Hola, ¿cómo estás?
    </voice>
</speak>";

        using var fileOutput = AudioConfig.FromWavFileOutput("multi_language_speech.wav");
        using var synthesizer = new SpeechSynthesizer(config, fileOutput);

        var result = await synthesizer.SpeakSsmlAsync(ssml);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine("✅ Multilingual speech generated: multi_language_speech.wav");
        }
        else
        {
            Console.WriteLine($"❌ Error: {result.Reason}");
        }

    }
}
