using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Speaker;

string speechKey = "SUBSCRIPTION_KEY";
string speechRegion = "REGION";

var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
var client = new VoiceProfileClient(speechConfig);

// Create a profile for verification
var profile = await client.CreateProfileAsync(VoiceProfileType.TextIndependentVerification, "en-us");

// Enroll your voice
using var audioInput = AudioConfig.FromWavFileInput("deeksha_enroll.wav");
await client.EnrollProfileAsync(profile, audioInput);

Console.WriteLine($"Your profile is ready. ProfileId: {profile.Id}");

// STEP 3: Verification with Microphone
using (var verifyAudio = AudioConfig.FromDefaultMicrophoneInput())
using (var recognizer = new SpeakerRecognizer(speechConfig, verifyAudio))
{
    var verificationModel = SpeakerVerificationModel.FromProfile(profile);

    Console.WriteLine("\n--- VERIFICATION PHASE ---");
    Console.WriteLine("🎤 Please speak now for verification...");

    var result = await recognizer.RecognizeOnceAsync(verificationModel);

    if (result.Reason == ResultReason.RecognizedSpeaker)
    {
        Console.WriteLine($"✅ Verification successful! ProfileId: {result.ProfileId}, Score: {result.Score}");
    }
    else if (result.Reason == ResultReason.Canceled)
    {
        Console.WriteLine($"❌ Verification canceled: {result.Reason}");
    }
    else
    {
        Console.WriteLine($"❌ Verification failed. Reason: {result.Reason}");
    }
}

// STEP 4: Cleanup profile if needed
Console.WriteLine("\nDo you want to delete this profile? (y/n)");
if (Console.ReadLine()?.ToLower() == "y")
{
    await client.DeleteProfileAsync(profile);
    Console.WriteLine("Profile deleted.");
}
else
{
    Console.WriteLine("Profile kept for future use.");
}