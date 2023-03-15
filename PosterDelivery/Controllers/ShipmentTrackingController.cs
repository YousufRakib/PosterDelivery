using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PosterDelivery.Infrastructure;
using PosterDelivery.Models;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using PosterDelivery.Utility;
using System.Security.Claims;
using PosterDelivery.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using Stowage;

namespace PosterDelivery.Controllers {
    public class ShipmentTrackingController : Controller {
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IScanningService _scanningService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShipmentTrackingController> _logger;
        private readonly AppSettings _appSettings;

        public ShipmentTrackingController(IShipmentTrackingService shipmentTrackingService,
            IScanningService scanningService, ICustomerService customerService, IConfiguration configuration,
            ILogger<ShipmentTrackingController> logger, IOptions<AppSettings> settings,
            IProductService _productService) {
            this._shipmentTrackingService = shipmentTrackingService;
            this._scanningService = scanningService;
            this._customerService = customerService;
            this._configuration = configuration;
            this._logger = logger;
            this._appSettings = settings.Value;
            this._productService = _productService;
        }

        [Route("/ShipmentTracking/StartScanning/")]
        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> StartScanning(int customerId, int driverCustomerTrackId) {
            await _shipmentTrackingService.UpdateDeliveryVisitDate(driverCustomerTrackId, 1);

            ProductsModel objProductModel = new ProductsModel();
            objProductModel.lstProductDetils = await _productService.GetProductDetailsByStore(customerId, driverCustomerTrackId);
            objProductModel.CustomerId = customerId;
            objProductModel.CustomerName = (await _customerService.GetCustomerInfo(customerId)).AccountName;
            objProductModel.DriverCustomerTrackId = driverCustomerTrackId;
            return View(objProductModel);
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> StartNonScanning(int customerId, int driverCustomerTrackId, int IsEdit = 0) {
            if (IsEdit == 1) {
                ViewBag.IsEdit = true;
                var result = await _shipmentTrackingService.GetInvoiceDataForEdit(customerId, driverCustomerTrackId);
                if (result.Count != 0) {
                    ViewBag.PickupCount = result[0].PickupCount;
                    ViewBag.DeliveryCount = result[0].DeliveryCount;
                    ViewBag.SoldQuantity = result[0].SoldQuantity;
                    ViewBag.InvoiceSerialNum = result[0].InvoiceSerialNum;
                    ViewBag.InvoiceAmount = result[0].InvoiceAmount;
                }
                ViewBag.DataCount = result.Count;
            } else {
                await _shipmentTrackingService.UpdateDeliveryVisitDate(driverCustomerTrackId, 0);
            }
            var role = HttpContext.Session.GetString("Roles");
            ViewBag.Role = role;

            ProductsModel objProductModel = new ProductsModel();
            var data = _customerService.GetCustomerInfo(customerId);
            objProductModel.lstProductDetils = new List<ProductDetails>();
            objProductModel.CustomerId = customerId;
            objProductModel.CustomerName = (await _customerService.GetCustomerInfo(customerId)).AccountName;
            objProductModel.DriverCustomerTrackId = driverCustomerTrackId;
            return View(objProductModel);
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> StartNonScanningForNewBuyer(int customerId, int driverCustomerTrackId) {
            await _shipmentTrackingService.UpdateDeliveryVisitDate(driverCustomerTrackId, 0);
            var role = HttpContext.Session.GetString("Roles");
            ViewBag.Role = role;

            ProductsModel objProductModel = new ProductsModel();
            var data = _customerService.GetCustomerInfo(customerId);
            objProductModel.lstProductDetils = new List<ProductDetails>();
            objProductModel.CustomerId = customerId;
            objProductModel.CustomerName = (await _customerService.GetCustomerInfo(customerId)).AccountName;
            objProductModel.DriverCustomerTrackId = driverCustomerTrackId;
            return View(objProductModel);
        }

        [Route("/ShipmentTracking/StartDeliveryScanning/")]
        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> StartDeliveryScanning(int customerId, int driverCustomerTrackId) {
            var role = HttpContext.Session.GetString("Roles");
            ViewBag.UserRole = role;

            ProductsModel objProductModel = new ProductsModel();
            // IList<ProductDetails> lstProductData = new List<ProductDetails>();
            objProductModel.lstProductDetils = await _productService.GetProductDetailsService();
            objProductModel.CustomerId = customerId;
            objProductModel.CustomerName = (await _customerService.GetCustomerInfo(customerId)).AccountName;
            objProductModel.DriverCustomerTrackId = driverCustomerTrackId;
            return View(objProductModel);
        }

        [Route("/ShipmentTracking/StartDeliveryScanningForNewBuyer/")]
        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> StartDeliveryScanningForNewBuyer(int customerId, int driverCustomerTrackId) {
            var role = HttpContext.Session.GetString("Roles");
            ViewBag.UserRole = role;
            await _shipmentTrackingService.UpdateDeliveryVisitDate(driverCustomerTrackId, 1);
            ProductsModel objProductModel = new ProductsModel();
            // IList<ProductDetails> lstProductData = new List<ProductDetails>();
            objProductModel.lstProductDetils = await _productService.GetProductDetailsService();
            objProductModel.CustomerId = customerId;
            objProductModel.CustomerName = (await _customerService.GetCustomerInfo(customerId)).AccountName;
            objProductModel.DriverCustomerTrackId = driverCustomerTrackId;
            return View(objProductModel);
        }

        [Route("/ShipmentTracking/GetScanItemWithoutPickUp/")]
        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> GetScanItemWithoutPickUp(int customerId, int driverCustomerTrackId) {
            try {
                var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                var scanData = await _scanningService.GetScanItemWithoutPickUp(customerId, driverCustomerTrackId, userId);

                if (scanData.Count > 0) {
                    return Json(new Confirmation { msg = "Data found!!", output = "Success", returnvalue = scanData });
                } else {
                    return Json(new Confirmation { msg = "Data not found!!", output = "DataTypeIssue", returnvalue = scanData });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

        [HttpGet]
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignDriver() {
            ViewBag.DriverList = await _shipmentTrackingService.GetDriverList();
            return View();
        }

        [HttpGet]
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAssignDriver() {
            try {
                var customerTrackingData = await _shipmentTrackingService.GetAssignDriverList();
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult> SaveAssignDriver([FromBody] DriverCustomerTrack driverCustomerTrack) {
            try {
                driverCustomerTrack.CreatedByUser = Convert.ToInt16(HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                var userInfo = await _shipmentTrackingService.SaveAssignDriver(driverCustomerTrack);

                if (userInfo > 0) {
                    return Json(new Confirmation { msg = "Driver Assigned successfully!!", output = "Success", returnvalue = driverCustomerTrack });
                } else {
                    return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = driverCustomerTrack });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = driverCustomerTrack });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetDrivers() {
            List<Registration> driverList = new List<Registration>();
            try {
                var result = await _shipmentTrackingService.GetDriverList();
                if (result != null || result.Count > 0) {
                    driverList = result.ToList();
                }

                return Json(driverList);
            } catch (Exception ex) {

                _logger.LogError(ex.Message, "Exception Caught");
                return Json(driverList);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> PendingStores() {
            ViewBag.DriverList = await _shipmentTrackingService.GetDriverList();
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetPendingStores() {
            try {
                var customerTrackingData = await _shipmentTrackingService.ShipmentTrackingData("Pending", null);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetDateWisePendingStores(string deliveryDate) {
            try {
                var customerTrackingData = await _shipmentTrackingService.DateWisePendingStore("Pending", deliveryDate);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetDateWiseProgressStores(string deliveryDate) {
            try {
                var customerTrackingData = await _shipmentTrackingService.DateWiseProgressStore("Inprogress", deliveryDate);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> PendingStoresForDriver() {
            List<DeliveryDateNext7Days> deliveryDateNext7Days = new List<DeliveryDateNext7Days>();
            DateTime today = DateTime.Now;
            for (int i = 0; i <= 6; i++) {
                string deliveryDate = today.ToString("yyyy-MM-dd");
                deliveryDateNext7Days.Add(new DeliveryDateNext7Days { DeliveryDate = deliveryDate });
                today = today.AddDays(1);
            }
            ViewBag.DriverList = await _shipmentTrackingService.GetDriverList();
            return View(deliveryDateNext7Days);
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> GetPendingStoresForDriver(string deliveryDate) {
            try {
                var customerTrackingData = await _shipmentTrackingService.ShipmentTrackingDataForDriver("Pending", deliveryDate);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> CompletedStores() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetCompletedStores(string deliveryDate) {
            try {
                var customerTrackingData = await _shipmentTrackingService.ShipmentTrackingData("Completed", deliveryDate);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Driver")]
        public async Task<IActionResult> CompletedStoresForDriver() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> GetCompletedStoresForDriver(string deliveryDate) {
            try {
                var customerTrackingData = await _shipmentTrackingService.ShipmentTrackingDataForDriver("Completed", deliveryDate);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        //[CustomAuth(Roles = "Admin,Manager")]
        //public async Task<IActionResult> ChangeDriver() {
        //    return View();
        //}

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> ProgressStores() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> ProgressStoresForNewBuyer() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetProgressStores() {
            try {
                var customerTrackingData = await _shipmentTrackingService.ShipmentTrackingData("Inprogress", null);
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetProgressStoresForNewBuyers() {
            try {
                var customerTrackingData = await _shipmentTrackingService.GetProgressStoresForNewBuyers();
                if (customerTrackingData == null || customerTrackingData.Count == 0) {
                    customerTrackingData = new List<ShipmentTracking>();
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customerTrackingData.ToList() });
            } catch (Exception ex) {
                List<ShipmentTracking> products = new List<ShipmentTracking>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult> AssignDriver([FromBody] DriverCustomerTrack driverCustomerTrack) {
            try {
                driverCustomerTrack.CreatedByUser = Convert.ToInt16(HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                var userInfo = await _shipmentTrackingService.AssignDriver(driverCustomerTrack);

                if (userInfo == "1") {
                    return Json(new Confirmation { msg = "Driver Assigned successfully!!", output = "Success", returnvalue = driverCustomerTrack });
                } else {
                    return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = driverCustomerTrack });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = driverCustomerTrack });
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> CaptureDeliveryScan([FromBody] IList<PickupScanningModel> lstPickupModel) {
            try {
                if (lstPickupModel.Count > 0) {
                    lstPickupModel.ToList().ForEach(c => {
                        c.ScannedBy = Convert.ToInt16(HttpContext.User.Claims?
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                        c.Quantity = 1;
                    });
                    var userInfo = await _scanningService.CaptureDeliveryScan(lstPickupModel);

                    if (userInfo > 0) {
                        return Json(new Confirmation { msg = "Delivery scan captured successfully!!", output = "Success", returnvalue = userInfo });
                    } else {
                        return Json(new Confirmation { msg = "Delivery scan didn't captured successfully!!", output = "DataTypeIssue", returnvalue = userInfo });
                    }
                } else {
                    return Json(new Confirmation { msg = "No data available for delivery scan!!", output = "DataTypeIssue", returnvalue = null });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> CaptureNonScanningSoldCount(int customerId, int drivercustId, int pickupCount, int deliveryCount) {
            try {

                var result = await _scanningService.GetNoScanningSalesService(Convert.ToInt32(HttpContext.Session.GetString("UserId")), customerId, drivercustId, pickupCount, deliveryCount);
                return Json(new Confirmation { msg = "Delivery scan captured successfully!!", output = "Success", returnvalue = result });

            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> CapturePickupScan([FromBody] IList<PickupScanningModel> lstPickupModel) {
            try {
                if (lstPickupModel.Count > 0) {
                    lstPickupModel.ToList().ForEach(c => {
                        c.ScannedBy = Convert.ToInt16(HttpContext.User.Claims?
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                        c.Quantity = 1;
                    });
                    var userInfo = await _scanningService.GetDeliveryInvoice(lstPickupModel);

                    if (userInfo.Count > 0) {
                        return Json(new Confirmation { msg = "Pickup scan captured successfully!!", output = "Success", returnvalue = userInfo });
                    } else {
                        return Json(new Confirmation { msg = "Pickup scan didn't captured successfully!!", output = "DataTypeIssue", returnvalue = userInfo });
                    }
                } else {
                    return Json(new Confirmation { msg = "No data available for Pickup scan!!", output = "DataTypeIssue", returnvalue = null });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> SaveDriverCustomerTrackForNewBuyer(int customerId) {
            try {
                var result = await _shipmentTrackingService.SaveDriverCustomerTrackForNewBuyer(Convert.ToInt16(HttpContext.User.Claims?
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value), customerId);
                if (result > 0) {
                    return Json(new Confirmation { msg = "Driver customer track for new buyer captured successfully!!", output = "Success", returnvalue = result });
                } else {
                    return Json(new Confirmation { msg = "Driver customer track for new buyer didn't captured successfully!!", output = "DataTypeIssue", returnvalue = result });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> CaptureBuyerFirstVisit(IFormFile file, int customerId, int DriverCustomerTrackId,
            double InvoiceAmount, string InvoiceSerialNum, int SoldQuantity) {
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
                objStoreInvoiceInputModel.DeliveryCount = SoldQuantity;
                objStoreInvoiceInputModel.PickupCount = 0;
                objStoreInvoiceInputModel.SoldQuantity = SoldQuantity;
                var result = await _shipmentTrackingService.CaptureBuyerFirstVisit(objStoreInvoiceInputModel);
                if (result > 0) {
                    return Json(new Confirmation { msg = "Invoice generated successfully!!", output = "Success", returnvalue = result });
                } else {
                    return Json(new Confirmation { msg = "Invoice not generated successfully!!", output = "Error", returnvalue = result });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!! " + ex.Message.ToString(), output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [HttpPost]
        public async Task<ActionResult> FinishScan(int customerId, int driverCustomerTrackId) {
            try {
                var result = await _shipmentTrackingService.FinishScan(customerId, driverCustomerTrackId);

                if (result > 0) {
                    return Json(new Confirmation { msg = "Scanning finished!!", output = "Success", returnvalue = result });
                } else {
                    return Json(new Confirmation { msg = "Scanning not finished!!", output = "Error", returnvalue = result });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }
    }
}
