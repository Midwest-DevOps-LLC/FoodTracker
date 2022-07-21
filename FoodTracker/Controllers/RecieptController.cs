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
    public class RecieptController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Display([FromQuery]string path)
        {
            return View("Display", path);
        }

        public IActionResult _View([FromQuery] string path)
        {
            var model = new Models.RecieptViewModel();

            using (var productBLL = new BusinessLogicLayer.Receipts(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var p = productBLL.GetReceiptByGUID(path);
                
                if (p == null)
                    return PartialView("_View", model);

                var purchases = productBLL.GetAllReceiptPurchasesByReceiptID(p.ID.GetValueOrDefault());

                model.WalmartReciept = new DataEntities.OCRModels.WalmartReciept(p);

                foreach (var purchase in purchases)
                {
                    model.WalmartReciept.Purchases.Add(new DataEntities.OCRModels.WalmartReciept.Purchase(purchase));
                }

                model.ImagePath = p.GUID;

                return PartialView("_View", model);
            }
        }

        public IActionResult GetFileByPath([FromQuery]string path)
        {
            var extenstion = "";

            using (var productBLL = new BusinessLogicLayer.Receipts(MDO.Utility.Standard.ConnectionHandler.GetConnectionString()))
            {
                var p = productBLL.GetReceiptByGUID(path);
                extenstion = p.FileExtension;
            }


            var pathh = Path.Combine(HomeController.StartingDirectory, path + "." + extenstion);

            var fileInfo = new FileInfo(pathh);

            var fileExtention = fileInfo.Extension;
            var contentType = "application/octet-stream";

            if (fileExtention.ToUpper() == "PNG")
            {
                contentType = "image/png";
            }
            else if (fileExtention.ToUpper() == "JPEG")
            {
                contentType = "image/jpeg";
            }
            else if (fileExtention.ToUpper() == "JPG")
            {
                contentType = "image/jpg";
            }
            else if (fileExtention.ToUpper() == "GIF")
            {
                contentType = "image/gif";
            }

            return File(System.IO.File.ReadAllBytes(fileInfo.FullName), contentType, fileInfo.Name);
        }
    }
}
