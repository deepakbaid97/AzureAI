using Azure.AI.Translation.Document;
using Azure;
using System.Text;

class Program
{
    private static readonly string endpoint = "<your-custom-endpoint>";
    private static readonly string key = "<your-key>";

    static async Task Main(string[] args)
    {
        SingleDocumentTranslationClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

        try
        {
            string filePath = Path.Combine("TestData", "test-input.txt");
            using Stream fileStream = File.OpenRead(filePath);
            var sourceDocument = new MultipartFormFileData(Path.GetFileName(filePath), fileStream, "text/html");
            DocumentTranslateContent content = new(sourceDocument);
            var response = await client.TranslateAsync("es", content).ConfigureAwait(false);

            var requestString = File.ReadAllText(filePath);
            var responseString = Encoding.UTF8.GetString(response.Value.ToArray());

            Console.WriteLine($"Request string for translation: {requestString}");
            Console.WriteLine($"Response string after translation: {responseString}");
        }
        catch (RequestFailedException exception)
        {
            Console.WriteLine($"Error Code: {exception.ErrorCode}");
            Console.WriteLine($"Message: {exception.Message}");
        }
    }
}
    

