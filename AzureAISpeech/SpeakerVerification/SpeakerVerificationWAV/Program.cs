using NAudio.Wave;

class Program
{
    static void Main()
    {
        string filePath = "deeksha_enroll.wav";
        int sampleRate = 16000; // 16kHz mono

        using (var waveIn = new WaveInEvent())
        {
            waveIn.WaveFormat = new WaveFormat(sampleRate, 1);
            using (var writer = new WaveFileWriter(filePath, waveIn.WaveFormat))
            {
                waveIn.DataAvailable += (s, a) =>
                {
                    writer.Write(a.Buffer, 0, a.BytesRecorded);
                };

                Console.WriteLine("Recording... Press any key to stop.");
                waveIn.StartRecording();
                Console.ReadKey();
                waveIn.StopRecording();
            }
        }

        Console.WriteLine($"Voice recording saved to {Path.GetFullPath(filePath)}");
    }
}
