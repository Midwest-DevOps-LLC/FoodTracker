using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace FoodTracker.OCR
{
    public static class OCRHandler
    {
        /// <summary>
        /// Returns data from a image of a walmart reciept
        /// </summary>
        /// <param name="path">File Path</param>
        public static DataEntities.OCRModels.WalmartReciept Test(string path)
        {
            DataEntities.OCRModels.WalmartReciept ret = new DataEntities.OCRModels.WalmartReciept();

            try
            {
                //set `<your-endpoint>` and `<your-key>` variables with the values from the Azure portal to create your `AzureKeyCredential` and `DocumentAnalysisClient` instance
                string endpoint = "https://testinstancemidwestdevops.cognitiveservices.azure.com/";
                string key = "";
                AzureKeyCredential credential = new AzureKeyCredential(key);
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                //sample invoice document

                Uri invoiceUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf");

                var pathh = string.IsNullOrEmpty(path) ? @"C:\temp\IMG_0853.jpg" : path;

                Stream imageStreamSource = new FileStream(pathh, FileMode.Open, FileAccess.Read, FileShare.Read);


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

                    if (document.Fields.TryGetValue("Item Count", out DocumentField? vendorNameField))
                    {
                        if (vendorNameField.ValueType == DocumentFieldType.Int64)
                        {
                            long vendorName = vendorNameField.AsInt64();
                            ret.ItemCount = vendorName;
                            Console.WriteLine($"Item Count: '{vendorName}', with confidence {vendorNameField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("TC", out DocumentField? customerNameField))
                    {
                        if (customerNameField.ValueType == DocumentFieldType.String)
                        {
                            string customerName = customerNameField.AsString();
                            ret.TC = customerName ?? "";
                            Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("purchases", out DocumentField? itemsField))
                    {
                        if (itemsField.ValueType == DocumentFieldType.List)
                        {
                            foreach (DocumentField itemField in itemsField.AsList())
                            {
                                var purchase = new DataEntities.OCRModels.WalmartReciept.Purchase();

                                Console.WriteLine("Item:");

                                if (itemField.ValueType == DocumentFieldType.Dictionary)
                                {
                                    IReadOnlyDictionary<string, DocumentField> itemFields = itemField.AsDictionary();

                                    if (itemFields.TryGetValue("Product", out DocumentField? itemDescriptionField))
                                    {
                                        if (itemDescriptionField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemDescription = itemDescriptionField.AsString();
                                            purchase.Product = itemDescription ?? "";
                                            Console.WriteLine($"  Product: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("SKU", out DocumentField? itemAmountField))
                                    {
                                        if (itemAmountField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemAmountField.AsString();
                                            purchase.SKU = itemAmount ?? "";
                                            Console.WriteLine($"  SKU: '{itemAmount}', with confidence {itemAmountField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Type", out DocumentField? itemTypeField))
                                    {
                                        if (itemTypeField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemTypeField.AsString();
                                            purchase.Type = itemAmount ?? "";
                                            Console.WriteLine($"  Type: '{itemAmount}', with confidence {itemTypeField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Price", out DocumentField? itemPriceField))
                                    {
                                        if (itemPriceField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemPriceField.AsString();
                                            purchase.Price = itemAmount ?? "";
                                            Console.WriteLine($"  Price: '{itemAmount}', with confidence {itemPriceField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Tax Type", out DocumentField? itemTaxTypeField))
                                    {
                                        if (itemTaxTypeField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemTaxTypeField.AsString();
                                            purchase.TaxType = itemAmount ?? "";
                                            Console.WriteLine($"  Tax Type: '{itemAmount}', with confidence {itemTaxTypeField.Confidence}");
                                        }
                                    }

                                    ret.Purchases.Add(purchase);
                                }
                            }
                        }
                    }

                    if (document.Fields.TryGetValue("Brand", out DocumentField? subBrandField))
                    {
                        if (subBrandField.ValueType == DocumentFieldType.String)
                        {
                            string subTotal = subBrandField.AsString();
                            ret.Brand = subTotal ?? "";
                            Console.WriteLine($"Brand: '{subTotal}', with confidence {subBrandField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("Date Time", out DocumentField? subDateTimeField))
                    {
                        if (subDateTimeField.ValueType == DocumentFieldType.String)
                        {
                            string subTotal = subDateTimeField.AsString();
                            ret.DateTime = subTotal ?? "";
                            Console.WriteLine($"Sub Total: '{subTotal}', with confidence {subDateTimeField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField))
                    {
                        if (subTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double subTotal = subTotalField.AsDouble();
                            ret.SubTotal = subTotal;
                            Console.WriteLine($"Sub Total: '{subTotal}', with confidence {subTotalField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("Card", out DocumentField? totalTaxField))
                    {
                        if (totalTaxField.ValueType == DocumentFieldType.String)
                        {
                            string totalTax = totalTaxField.AsString();
                            ret.Card = totalTax ?? "";
                            Console.WriteLine($"Card: '{totalTax}', with confidence {totalTaxField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("Total", out DocumentField? invoiceTotalField))
                    {
                        if (invoiceTotalField.ValueType == DocumentFieldType.Double)
                        {
                            try
                            {
                                double invoiceTotal = invoiceTotalField.AsDouble();
                                ret.Total = invoiceTotal;
                                Console.WriteLine($"Total: '{invoiceTotal}', with confidence {invoiceTotalField.Confidence}");
                            }
                            catch(Exception ex) { }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return ret;
        }

        public static DataEntities.OCRModels.WalmartReciept Test(Stream path)
        {
            DataEntities.OCRModels.WalmartReciept ret = new DataEntities.OCRModels.WalmartReciept();

            try
            {
                //set `<your-endpoint>` and `<your-key>` variables with the values from the Azure portal to create your `AzureKeyCredential` and `DocumentAnalysisClient` instance
                string endpoint = "https://testinstancemidwestdevops.cognitiveservices.azure.com/";
                string key = "a0297f2b7ffc4e60805c2a8388b0ef81";
                AzureKeyCredential credential = new AzureKeyCredential(key);
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                //sample invoice document

                Uri invoiceUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf");

                //var pathh = string.IsNullOrEmpty(path) ? @"C:\temp\IMG_0853.jpg" : path;

                //Stream imageStreamSource = new FileStream(pathh, FileMode.Open, FileAccess.Read, FileShare.Read);


                //using (FileStream fs = new(@"C:\temp\IMG_0853.jpg", FileMode.Open))
                //{
                //    fs.
                //    e.File.OpenReadStream(4_000_000).CopyToAsync(fs);
                //}

                AnalyzeDocumentOperation operation = client.StartAnalyzeDocument("walmart-model-4", path);
                //AnalyzeDocumentOperation operation = client.StartAnalyzeDocumentFromUri("prebuilt-receipt", invoiceUri);

                var r = operation.WaitForCompletionAsync().Result;

                AnalyzeResult result = operation.Value;

                for (int i = 0; i < result.Documents.Count; i++)
                {
                    Console.WriteLine($"Document {i}:");

                    AnalyzedDocument document = result.Documents[i];

                    if (document.Fields.TryGetValue("ItemCount", out DocumentField? vendorNameField))
                    {
                        if (vendorNameField.ValueType == DocumentFieldType.Int64)
                        {
                            long vendorName = vendorNameField.AsInt64();
                            ret.ItemCount = vendorName;
                            Console.WriteLine($"Item Count: '{vendorName}', with confidence {vendorNameField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("TC", out DocumentField? customerNameField))
                    {
                        if (customerNameField.ValueType == DocumentFieldType.String)
                        {
                            string customerName = customerNameField.AsString();
                            ret.TC = customerName;
                            Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("purchases", out DocumentField? itemsField))
                    {
                        if (itemsField.ValueType == DocumentFieldType.List)
                        {
                            foreach (DocumentField itemField in itemsField.AsList())
                            {
                                var purchase = new DataEntities.OCRModels.WalmartReciept.Purchase();

                                Console.WriteLine("Item:");

                                if (itemField.ValueType == DocumentFieldType.Dictionary)
                                {
                                    IReadOnlyDictionary<string, DocumentField> itemFields = itemField.AsDictionary();

                                    if (itemFields.TryGetValue("Product", out DocumentField? itemDescriptionField))
                                    {
                                        if (itemDescriptionField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemDescription = itemDescriptionField.AsString();
                                            purchase.Product = itemDescription;
                                            Console.WriteLine($"  Product: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("SKU", out DocumentField? itemAmountField))
                                    {
                                        if (itemAmountField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemAmountField.AsString();
                                            purchase.SKU = itemAmount;
                                            Console.WriteLine($"  SKU: '{itemAmount}', with confidence {itemAmountField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Type", out DocumentField? itemTypeField))
                                    {
                                        if (itemTypeField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemTypeField.AsString();
                                            purchase.Type = itemAmount;
                                            Console.WriteLine($"  Type: '{itemAmount}', with confidence {itemTypeField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Price", out DocumentField? itemPriceField))
                                    {
                                        if (itemPriceField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemPriceField.AsString();
                                            purchase.Price = itemAmount;
                                            Console.WriteLine($"  Price: '{itemAmount}', with confidence {itemPriceField.Confidence}");
                                        }
                                    }

                                    if (itemFields.TryGetValue("Tax Type", out DocumentField? itemTaxTypeField))
                                    {
                                        if (itemTaxTypeField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemAmount = itemTaxTypeField.AsString();
                                            purchase.TaxType = itemAmount;
                                            Console.WriteLine($"  Tax Type: '{itemAmount}', with confidence {itemTaxTypeField.Confidence}");
                                        }
                                    }

                                    ret.Purchases.Add(purchase);
                                }
                            }
                        }
                    }

                    if (document.Fields.TryGetValue("Brand", out DocumentField? subBrandField))
                    {
                        if (subBrandField.ValueType == DocumentFieldType.String)
                        {
                            string subTotal = subBrandField.AsString();
                            ret.Brand = subTotal;
                            Console.WriteLine($"Brand: '{subTotal}', with confidence {subBrandField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("Date Time", out DocumentField? subDateTimeField))
                    {
                        if (subDateTimeField.ValueType == DocumentFieldType.String)
                        {
                            string subTotal = subDateTimeField.AsString();
                            ret.DateTime = subTotal;
                            Console.WriteLine($"Sub Total: '{subTotal}', with confidence {subDateTimeField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField))
                    {
                        if (subTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double subTotal = subTotalField.AsDouble();
                            ret.SubTotal = subTotal;
                            Console.WriteLine($"Sub Total: '{subTotal}', with confidence {subTotalField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("Card", out DocumentField? totalTaxField))
                    {
                        if (totalTaxField.ValueType == DocumentFieldType.String)
                        {
                            string totalTax = totalTaxField.AsString();
                            ret.Card = totalTax;
                            Console.WriteLine($"Card: '{totalTax}', with confidence {totalTaxField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("Total", out DocumentField? invoiceTotalField))
                    {
                        if (invoiceTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double invoiceTotal = invoiceTotalField.AsDouble();
                            ret.Total = invoiceTotal;
                            Console.WriteLine($"Total: '{invoiceTotal}', with confidence {invoiceTotalField.Confidence}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return ret;
        }
    }
}
