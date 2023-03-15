using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nancy;
using PosterDelivery.Infrastructure;
using PosterDelivery.Models;
using PosterDelivery.Services;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using Stowage;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace PosterDelivery.Controllers {
    public class InvoiceController : Controller {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<InvoiceController> _logger;
        private readonly AppSettings _appSettings;

        public InvoiceController(IInvoiceService invoiceService, ICustomerService customerService, ILogger<InvoiceController> logger, IOptions<AppSettings> settings) {
            this._invoiceService = invoiceService;
            this._customerService = customerService;
            this._logger = logger;
            this._appSettings = settings.Value;
        }
        public IActionResult Index() {
            return View();
        }
        [Route("/Invoice/InvoiceList/{customerId:int}")]
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> InvoiceList(int customerId) {
            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = (await _customerService.GetCustomerInfo(customerId)).AccountName;
            return View();
        }

        [HttpPost]
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetInvoiceByCustomer(int customerId) {
            IList<InvoiceModel> lstInvoice = new List<InvoiceModel>();
            try {
                lstInvoice = await _invoiceService.GetInvoiceService(customerId);
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
            }
            return Json(lstInvoice);
        }

        [HttpPost]
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> UploadFile(IFormFile file, string invoiceDate, string invoiceSerialNo, string invoiceAmount, int customerId) {
            int status = 0;
            try {
                // validations
                if (file == null) {
                    throw new Exception("No file uploaded");
                }

                if (file.Length == 0) {
                    throw new Exception("Invalid file uploaded");
                }

                if (!DateTime.TryParse(invoiceDate, out DateTime parsedInvoiceDate)) {
                    throw new Exception("Invalid date entered");
                }

                // read bytes from uploaded file
                var fileBytes = new byte[file.Length];
                using (var ms = new MemoryStream()) {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                // upload file to azure
                string filePath = $"Invoice {customerId} {parsedInvoiceDate.ToString("yyyy-MM-dd")} {Guid.NewGuid()} {file.FileName}";

                using (IFileStorage fileStorage = Files.Of.AzureBlobStorage(_appSettings.AzureStorageAccountName, _appSettings.AzureStorageKey, _appSettings.AzureStorageContainer)) {
                    using (Stream stream = await fileStorage.OpenWrite(filePath)) {
                        await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                    }
                }

                status = await _invoiceService.UploadInvoiceService(invoiceDate, customerId, Convert.ToDouble(invoiceAmount), invoiceSerialNo,
                    file.FileName, filePath, Convert.ToInt32(HttpContext.Session.GetString("UserId")));
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
            }
            return Json(status);
        }


        //[Route("/Invoice/Download/{invoiceId:int}")]
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> Download([FromQuery] int invoiceId) {

            InvoiceModel invoiceModel = await _invoiceService.GetInvoiceById(invoiceId);
            if (invoiceModel == null) {
                throw new Exception("Invalid Invoice ID");
            }

            string mimeType = MimeTypes.GetMimeType(invoiceModel.InvoiceFilePath);

            using (IFileStorage fileStorage = Files.Of.AzureBlobStorage(_appSettings.AzureStorageAccountName, _appSettings.AzureStorageKey, _appSettings.AzureStorageContainer)) {
                using (Stream stream = await fileStorage.OpenRead(invoiceModel.InvoiceFilePath)) {

                    var memStream = new MemoryStream();
                    await stream.CopyToAsync(memStream);

                    memStream.Position = 0;

                    return File(memStream, mimeType, invoiceModel.InvoiceFileName);
                }
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> ViewImage([FromQuery] int invoiceId) {
            InvoiceModel invoiceModel = await _invoiceService.GetInvoiceById(invoiceId);
            if (invoiceModel == null) {
                throw new Exception("Invalid Invoice ID");
            }

            string mimeType = MimeTypes.GetMimeType(invoiceModel.InvoiceFilePath);

            using (IFileStorage fileStorage = Files.Of.AzureBlobStorage(_appSettings.AzureStorageAccountName, _appSettings.AzureStorageKey, _appSettings.AzureStorageContainer)) {
                using (Stream stream = await fileStorage.OpenRead(invoiceModel.InvoiceFilePath)) {

                    var memStream = new MemoryStream();
                    await stream.CopyToAsync(memStream);

                    memStream.Position = 0;

                    return File(memStream, mimeType);
                }
            }

        }

        [CustomAuth(Roles = "Admin,Manager")]
        public IActionResult AllInvoiceList() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> CaptureStoreInvoice(IFormFile file, int HeaderId, int customerId, double InvoiceAmount,
            int DriverCustomerTrackId, string InvoiceSerialNum, string InvoiceFileName, int DeliveryCount, int PickupCount, int SoldQuantity, bool IsEdit = false) {
            try {
                string filePath = "";

                if (SoldQuantity > 0) {
                    if (file == null) {
                        throw new Exception("No file uploaded");
                    }

                    if (file.Length == 0) {
                        throw new Exception("Invalid file uploaded");
                    }

                    // read bytes from uploaded file
                    var fileBytes = new byte[file == null ? 0 : file.Length];
                    using (var ms = new MemoryStream()) {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    if (!DateTime.TryParse(DateTime.Now.ToString(), out DateTime parsedInvoiceDate)) {
                        throw new Exception("Invalid date entered");
                    }

                    filePath = $"Invoice {customerId} {parsedInvoiceDate.ToString("yyyy-MM-dd")} {Guid.NewGuid()} {file.FileName}";

                    // upload file to azure
                    using (IFileStorage fileStorage = Files.Of.AzureBlobStorage(_appSettings.AzureStorageAccountName, _appSettings.AzureStorageKey, _appSettings.AzureStorageContainer)) {
                        using (Stream stream = await fileStorage.OpenWrite(filePath)) {
                            await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                        }
                    }
                }

                CaptureStoreInvoiceInputModel objStoreInvoiceInputModel = new CaptureStoreInvoiceInputModel();
                objStoreInvoiceInputModel.HeaderId = HeaderId;
                objStoreInvoiceInputModel.userId = Convert.ToInt16(HttpContext.User.Claims?
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                objStoreInvoiceInputModel.customerId = customerId;
                objStoreInvoiceInputModel.DriverCustomerTrackId = DriverCustomerTrackId;
                objStoreInvoiceInputModel.InvoiceAmount = InvoiceAmount;
                objStoreInvoiceInputModel.InvoiceSerialNum = InvoiceSerialNum;
                objStoreInvoiceInputModel.ActualInvoiceAmt = InvoiceAmount;
                objStoreInvoiceInputModel.TotalInvoiceAmt = InvoiceAmount;
                objStoreInvoiceInputModel.InvoiceFileName = file == null ? "" : file.FileName;
                objStoreInvoiceInputModel.InvoiceFilePath = filePath;
                objStoreInvoiceInputModel.DeliveryCount = DeliveryCount;
                objStoreInvoiceInputModel.PickupCount = PickupCount;
                objStoreInvoiceInputModel.SoldQuantity = SoldQuantity;
                if (IsEdit) {
                    var result = await _invoiceService.UpdateStoreInvoice(objStoreInvoiceInputModel);
                    if (result > 0) {
                        return Json(new Confirmation { msg = "Store invoice updated successfully!!", output = "Success", returnvalue = result });
                    } else {
                        return Json(new Confirmation { msg = "Store invoice not updated successfully!!", output = "Error", returnvalue = result });
                    }
                } else {
                    var result = await _invoiceService.CaptureStoreInvoiceService(objStoreInvoiceInputModel);
                    if (result > 0) {
                        return Json(new Confirmation { msg = "Store invoice generated successfully!!", output = "Success", returnvalue = result });
                    } else {
                        return Json(new Confirmation { msg = "Store invoice not generated successfully!!", output = "Error", returnvalue = result });
                    }
                }

            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

    }
}
