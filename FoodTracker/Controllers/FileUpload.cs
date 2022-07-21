using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace FoodTracker.Controllers
{
    /// <summary>
    /// controller for upload large file
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(ILogger<FileUploadController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Action for upload large file
        /// </summary>
        /// <remarks>
        /// Request to this action will not trigger any model binding or model validation,
        /// because this is a no-argument action
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(UploadLargeFile))]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue, ValueLengthLimit = int.MaxValue)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> UploadLargeFile()
        {
            var request = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    // Get the temporary folder, and combine a random file name with it

                    string directoriesFromPage = "";

                    if (string.IsNullOrEmpty(request.Headers["filePath"]) == false)
                    {
                        var filePathString = request.Headers["filePath"].ToString();

                        var base64EncodedBytes = Convert.FromBase64String(filePathString);
                        var unencodedFilePath = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                        directoriesFromPage = Uri.UnescapeDataString(unencodedFilePath);
                    }

                    var guid = Guid.NewGuid().ToString();
                    
                    var extenstion = contentDisposition.FileName.Value.Split(new char[] {'.'}).Last();
                    var saveToPath = Path.Combine(HomeController.StartingDirectory, directoriesFromPage, guid + "." + extenstion);

                    using (var targetStream = System.IO.File.Create(saveToPath))
                    {
                        await section.Body.CopyToAsync(targetStream);
                    }

                    var reciept = new DataEntities.Receipt();

                    var walmartReciept = OCR.OCRHandler.Test(saveToPath);
                    reciept.Brand = walmartReciept.Brand;
                    reciept.Card = walmartReciept.Card;
                    reciept.CreatedDate = DateTime.UtcNow;
                    reciept.GUID = guid;
                    reciept.TC = walmartReciept.TC;
                    reciept.FileExtension = extenstion;

                    if (DateTime.TryParse(walmartReciept.DateTime, out DateTime dateOnRecipt))
                        reciept.DateOnReceipt = dateOnRecipt;
                    if (int.TryParse(walmartReciept.ItemCount.ToString().Trim(), out int itemCount))
                        reciept.ItemCount = itemCount;
                    if (decimal.TryParse(walmartReciept.SubTotal.ToString().Trim(), out decimal subTotal))
                        reciept.SubTotal = subTotal;
                    if (decimal.TryParse(walmartReciept.Total.ToString().Trim(), out decimal total))
                        reciept.Total = total;

                    using (var productBLL = new BusinessLogicLayer.Receipts(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
                    {
                        var receiptID = productBLL.SaveReceipt(reciept);

                        foreach (var purchase in walmartReciept.Purchases)
                        {
                            DataEntities.Receipt.Purchase purchase1 = new DataEntities.Receipt.Purchase();
                            purchase1.SKU = purchase.SKU;
                            if (decimal.TryParse(purchase.Price.ToString().Trim(), out decimal price))
                                purchase1.Price = price;
                            purchase1.Product = purchase.Product;
                            purchase1.ReceiptID = (int)receiptID.GetValueOrDefault();
                            purchase1.TaxType = purchase.TaxType;
                            productBLL.SaveReceiptPurchase(purchase1);
                        }
                    }

                    //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(saveToPath);
                    //var r = System.Convert.ToBase64String(plainTextBytes);

                    return Ok(reciept.GUID);
                }

                section = await reader.ReadNextSectionAsync();
            }

            // If the code runs to this location, it means that no files have been saved
            return BadRequest("No files data in the request.");
        }
    }
}
