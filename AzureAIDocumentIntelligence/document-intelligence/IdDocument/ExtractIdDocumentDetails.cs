using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace document_intelligence.Invoices
{
    public class IdDocumentProcessor
    {
        private readonly DocumentAnalysisClient _client;

        public IdDocumentProcessor(DocumentAnalysisClient client)
        {
            _client = client;
        }

        public async Task ExtractIdDetailsAsync(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-idDocument", stream);
            var result = operation.Value;

            foreach (var document in result.Documents)
            {
                Console.WriteLine("=== ID Document Details ===");
                if (document.Fields.TryGetValue("FirstName", out var firstName))
                    Console.WriteLine($"First Name: {firstName.Content}");

                if (document.Fields.TryGetValue("LastName", out var lastName))
                    Console.WriteLine($"Last Name: {lastName.Content}");

                if (document.Fields.TryGetValue("DateOfBirth", out var dob))
                    Console.WriteLine($"DOB: {dob.Content}");

                if (document.Fields.TryGetValue("DocumentNumber", out var idNumber))
                    Console.WriteLine($"ID Number: {idNumber.Content}");

                if (document.Fields.TryGetValue("DateOfExpiration", out var expiry))
                    Console.WriteLine($"Expiry Date: {expiry.Content}");

                if (document.Fields.TryGetValue("Gender", out var gender))
                    Console.WriteLine($"Gender: {gender.Content}");

                if (document.Fields.TryGetValue("Issue Date", out var issue))
                    Console.WriteLine($"Issue Date: {issue.Content}");
            }
        }
    }
}
