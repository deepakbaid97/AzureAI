using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using document_intelligence.Invoices;
using YourProject.Services;

//set `<your-endpoint>` and `<your-key>` variables with the values from the Azure portal to create your `AzureKeyCredential` and `FormRecognizerClient` instance
string endpoint = "YOUR_ENDPOINT";
string key = "YOUR_API_SUBSCRIPTION_KEY";
AzureKeyCredential credential = new AzureKeyCredential(key);
DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);


    // Create processors
    var invoiceProcessor = new InvoiceProcessor(client);
    var idProcessor = new IdDocumentProcessor(client);

    // Menu
    Console.WriteLine("Select an option:");
    Console.WriteLine("1. Extract Invoice");
    Console.WriteLine("2. Extract ID Document");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Uri invoiceUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf");
            await invoiceProcessor.ExtractInvoiceFromUriAsync(invoiceUri);
            break;

        case "2":
            string filePath = "C:\\Users\\SAHOMADKHAN\\Downloads\\Shahanaz_A_AAdhar.jpg"; // local file
            await idProcessor.ExtractIdDetailsAsync(filePath);
            break;

        default:
            Console.WriteLine("Invalid selection.");
            break;
    }