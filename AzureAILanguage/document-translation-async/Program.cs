using System.Text;


class Program
{

    static readonly string route = "?api-version={date}";

    private static readonly string basePath = "{your-document-translation-endpoint}/translator/document/batches";

    private static readonly string key = "{your-api-key}";

    static readonly string json = ("{\"inputs\": [{\"source\": {\"sourceUrl\": \"https://YOUR-SOURCE-URL-WITH-READ-LIST-ACCESS-SAS\",\"storageSource\": \"AzureBlob\",\"language\": \"en\"}, \"targets\": [{\"targetUrl\": \"https://YOUR-TARGET-URL-WITH-WRITE-LIST-ACCESS-SAS\",\"storageSource\": \"AzureBlob\",\"category\": \"general\",\"language\": \"es\"}]}]}");

    static async Task Main(string[] args)
    {
        using HttpClient client = new();
        using HttpRequestMessage request = new();
        {

            StringContent content = new(json, Encoding.UTF8, "application/json");

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(basePath + route);
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);
            string result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Status code: {response.StatusCode}");
                Console.WriteLine();
                Console.WriteLine($"Response Headers:");
                Console.WriteLine(response.Headers);
            }
            else
                Console.Write("Error");

        }

    }

}