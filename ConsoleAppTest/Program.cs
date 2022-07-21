using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DemoApplication
{
    class Program
    {
        public static void Main(string[] args)
        {
            Test();

        }

        public class WalmartRecieptModel
        {
            public int ItemCount { get; set; }
            public string TC { get; set; }
            public string Brand { get; set; }
            public string DateTime { get; set; }
            public double Total { get; set; }
            public string Card { get; set; }
            public double SubTotal { get; set; }
            public List<Purchase> Purchases { get; set; } = new List<Purchase>();

            public class Purchase
            {
                public string Product { get; set; }
                public string SKU { get; set; }
                public string Type { get; set; }
                public string Price { get; set; }
                public string TaxType { get; set; }
            }
        }

        public async static void Test()
        {
            try
            {
                //set `<your-endpoint>` and `<your-key>` variables with the values from the Azure portal to create your `AzureKeyCredential` and `DocumentAnalysisClient` instance
                string endpoint = "https://testinstancemidwestdevops.cognitiveservices.azure.com/";
                string key = "a0297f2b7ffc4e60805c2a8388b0ef81";
                AzureKeyCredential credential = new AzureKeyCredential(key);
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                //sample invoice document

                Uri invoiceUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf");

                Stream imageStreamSource = new FileStream(@"C:\temp\IMG_0853.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);


                //using (FileStream fs = new(@"C:\temp\IMG_0853.jpg", FileMode.Open))
                //{
                //    fs.
                //    e.File.OpenReadStream(4_000_000).CopyToAsync(fs);
                //}

                AnalyzeDocumentOperation operation = client.StartAnalyzeDocument("walmart-model-4", imageStreamSource);
                //AnalyzeDocumentOperation operation = client.StartAnalyzeDocumentFromUri("prebuilt-receipt", invoiceUri);

                var r = operation.WaitForCompletionAsync().Result;

                AnalyzeResult result = operation.Value;

                for (int i = 0; i < result.Documents.Count; i++)
                {
                    Console.WriteLine($"Document {i}:");

                    AnalyzedDocument document = result.Documents[i];

                    if (document.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField))
                    {
                        if (vendorNameField.ValueType == DocumentFieldType.String)
                        {
                            string vendorName = vendorNameField.AsString();
                            Console.WriteLine($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField))
                    {
                        if (customerNameField.ValueType == DocumentFieldType.String)
                        {
                            string customerName = customerNameField.AsString();
                            Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("purchases", out DocumentField? itemsField))
                    {
                        if (itemsField.ValueType == DocumentFieldType.List)
                        {
                            foreach (DocumentField itemField in itemsField.AsList())
                            {
                                Console.WriteLine("Item:");

                                if (itemField.ValueType == DocumentFieldType.Dictionary)
                                {
                                    IReadOnlyDictionary<string, DocumentField> itemFields = itemField.AsDictionary();

                                    if (itemFields.TryGetValue("Product", out DocumentField? itemDescriptionField))
                                    {
                                        if (itemDescriptionField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemDescription = itemDescriptionField.AsString();

                                            Console.WriteLine($"  Product: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("SKU", out DocumentField? itemAmountField))
                                    {
                                        if (itemAmountField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemAmountField.AsString();

                                            Console.WriteLine($"  SKU: '{itemAmount}', with confidence {itemAmountField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Type", out DocumentField? itemTypeField))
                                    {
                                        if (itemTypeField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemTypeField.AsString();

                                            Console.WriteLine($"  Type: '{itemAmount}', with confidence {itemTypeField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Price", out DocumentField? itemPriceField))
                                    {
                                        if (itemPriceField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemPriceField.AsString();

                                            Console.WriteLine($"  Price: '{itemAmount}', with confidence {itemPriceField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Tax Type", out DocumentField? itemTaxTypeField))
                                    {
                                        if (itemTaxTypeField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemTaxTypeField.AsString();

                                            Console.WriteLine($"  Tax Type: '{itemAmount}', with confidence {itemTaxTypeField.Confidence}");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField))
                    {
                        if (subTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double subTotal = subTotalField.AsDouble();
                            Console.WriteLine($"Sub Total: '{subTotal}', with confidence {subTotalField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("TotalTax", out DocumentField? totalTaxField))
                    {
                        if (totalTaxField.ValueType == DocumentFieldType.Double)
                        {
                            double totalTax = totalTaxField.AsDouble();
                            Console.WriteLine($"Total Tax: '{totalTax}', with confidence {totalTaxField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField))
                    {
                        if (invoiceTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double invoiceTotal = invoiceTotalField.AsDouble();
                            Console.WriteLine($"Invoice Total: '{invoiceTotal}', with confidence {invoiceTotalField.Confidence}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}