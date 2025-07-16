using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace YourProject.Services
{
    public class InvoiceProcessor
    {
        private readonly DocumentAnalysisClient _client;

        public InvoiceProcessor(DocumentAnalysisClient client)
        {
            _client = client;
        }

        public async Task ExtractInvoiceFromUriAsync(Uri invoiceUri)
        {
            AnalyzeDocumentOperation operation =
                await _client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-invoice", invoiceUri);

            AnalyzeResult result = operation.Value;

            for (int i = 0; i < result.Documents.Count; i++)
            {
                Console.WriteLine($"Document {i}:");

                AnalyzedDocument document = result.Documents[i];

                if (document.Fields.TryGetValue("VendorName", out DocumentField vendorNameField) &&
                    vendorNameField.FieldType == DocumentFieldType.String)
                {
                    Console.WriteLine($"Vendor Name: '{vendorNameField.Value.AsString()}', Confidence: {vendorNameField.Confidence}");
                }

                if (document.Fields.TryGetValue("CustomerName", out DocumentField customerNameField) &&
                    customerNameField.FieldType == DocumentFieldType.String)
                {
                    Console.WriteLine($"Customer Name: '{customerNameField.Value.AsString()}', Confidence: {customerNameField.Confidence}");
                }

                if (document.Fields.TryGetValue("Items", out DocumentField itemsField) &&
                    itemsField.FieldType == DocumentFieldType.List)
                {
                    foreach (DocumentField itemField in itemsField.Value.AsList())
                    {
                        if (itemField.FieldType == DocumentFieldType.Dictionary)
                        {
                            var itemFields = itemField.Value.AsDictionary();

                            if (itemFields.TryGetValue("Description", out DocumentField descField) &&
                                descField.FieldType == DocumentFieldType.String)
                            {
                                Console.WriteLine($"  Description: '{descField.Value.AsString()}', Confidence: {descField.Confidence}");
                            }

                            if (itemFields.TryGetValue("Amount", out DocumentField amountField) &&
                                amountField.FieldType == DocumentFieldType.Currency)
                            {
                                var amount = amountField.Value.AsCurrency();
                                Console.WriteLine($"  Amount: '{amount.Symbol}{amount.Amount}', Confidence: {amountField.Confidence}");
                            }
                        }
                    }
                }

                if (document.Fields.TryGetValue("SubTotal", out DocumentField subTotalField) &&
                    subTotalField.FieldType == DocumentFieldType.Currency)
                {
                    var subTotal = subTotalField.Value.AsCurrency();
                    Console.WriteLine($"Sub Total: '{subTotal.Symbol}{subTotal.Amount}', Confidence: {subTotalField.Confidence}");
                }

                if (document.Fields.TryGetValue("TotalTax", out DocumentField totalTaxField) &&
                    totalTaxField.FieldType == DocumentFieldType.Currency)
                {
                    var totalTax = totalTaxField.Value.AsCurrency();
                    Console.WriteLine($"Total Tax: '{totalTax.Symbol}{totalTax.Amount}', Confidence: {totalTaxField.Confidence}");
                }

                if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField invoiceTotalField) &&
                    invoiceTotalField.FieldType == DocumentFieldType.Currency)
                {
                    var invoiceTotal = invoiceTotalField.Value.AsCurrency();
                    Console.WriteLine($"Invoice Total: '{invoiceTotal.Symbol}{invoiceTotal.Amount}', Confidence: {invoiceTotalField.Confidence}");
                }
            }
        }
    }
}
