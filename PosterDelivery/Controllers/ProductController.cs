using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nancy;
using Newtonsoft.Json;
using PosterDelivery.Infrastructure;
using PosterDelivery.Models;
using PosterDelivery.Services;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using Stowage;

namespace PosterDelivery.Controllers {
    public class ProductController : Controller 
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductController> _logger;
        private readonly AppSettings _appSettings;

        public ProductController(IProductService productService, IConfiguration configuration, ILogger<ProductController> logger, IOptions<AppSettings> settings) 
        {
            this._productService = productService;
            this._configuration = configuration;
            this._logger = logger;
            this._appSettings = settings.Value;
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddProduct(int productId) {

            Product product = new Product();
            List<ProductCategory> productCategories = new List<ProductCategory>();
            
            try 
            {
                if (productId != 0) {
                    var existingProduct = await _productService.GetProductInfo(productId);
                    if (existingProduct != null) {
                        product = existingProduct;
                    }
                }

                var result = await _productService.GetProductCategory();
                if (result != null || result.ToList().Count != 0) 
                {
                    productCategories = result.ToList();
                }
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, "Exception Caught");
            }

            ViewBag.ProductId = productId;
            productCategories.Add(new ProductCategory { CategoryId = 0, CategoryName = "Select Product Category" });
            product.productCategories = productCategories.OrderBy(x => x.CategoryId).ToList();
            product.QrCodeInlineImage = QrCodeGenerator.Generate(product.ProductSerial);

            return View(product);
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> SaveProduct(IFormFile productImage, int productId, string productSerial, string productName, int categoryId, double productPrice/*, string isActive*/) 
        {
            // if existing product, retrieve it

            Product productModel = productId > 0 ?
                await _productService.GetProductInfo(productId) :
                new Product();

            productModel.IsActive = "1";    // IsActive should be a boolean, not a string
            productModel.ProductId = productId;
            productModel.ProductSerial = productSerial;
            productModel.ProductName = productName;
            productModel.CategoryId = categoryId;
            productModel.ProductPrice = productPrice;
            //productModel.IsActive = isActive;

            try {
                if (productImage?.Length > 0) {

                    // read bytes from uploaded file
                    var fileBytes = new byte[productImage.Length];
                    using (var ms = new MemoryStream()) {
                        productImage.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    // upload file to azure
                    string filePath = $"ProductImage {productModel.ProductSerial} {DateTime.Now.ToString("yyyy-MM-dd")} {Guid.NewGuid()} {productImage.FileName}";
                    using (IFileStorage fileStorage = Files.Of.AzureBlobStorage(_appSettings.AzureStorageAccountName, _appSettings.AzureStorageKey, _appSettings.AzureProductImageStorageContainer)) {
                        using (Stream stream = await fileStorage.OpenWrite(filePath)) {
                            await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                        }
                    }

                    productModel.ProductImagePath = filePath;
                    productModel.ProductImageName = productImage.FileName;
                }

                var result = await _productService.SaveProductInformation(productModel);

                if (result == "1") {
                    return Json(new Confirmation { msg = "Data save successfully!!", output = "Success", returnvalue = productModel });
                } else {
                    return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = productModel });
                }
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = productModel });
            }
        }


        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> ProductImage([FromQuery] string filePath) {
            if (string.IsNullOrWhiteSpace(filePath)) {
                throw new Exception("Invalid path");
            }

            string mimeType = MimeTypes.GetMimeType(filePath);

            using (IFileStorage fileStorage = Files.Of.AzureBlobStorage(_appSettings.AzureStorageAccountName, _appSettings.AzureStorageKey, _appSettings.AzureProductImageStorageContainer)) {
                using (Stream stream = await fileStorage.OpenRead(filePath)) {
                    var memStream = new MemoryStream();
                    await stream.CopyToAsync(memStream);
                    memStream.Position = 0;

                    return File(memStream, mimeType);
                }
            }

        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> ProductList() 
        {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetProductList() 
        {
            try 
            {
                var products = await _productService.GetProducts();
                if (products == null || products.ToList().Count == 0) 
                {
                    products = new List<Product>();
                }

                foreach (Product p in products)
                {
                    p.QrCodeInlineImage = QrCodeGenerator.Generate(p.ProductSerial);
                }

                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = products.ToList() });
            } 
            catch (Exception ex) 
            {
                List<Product> products = new List<Product>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(products);
            }
        }


        //////Product Category
        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddProductCategory(int categoryId) 
        {
            ProductCategory productCategory = new ProductCategory();

            try 
            {
                if (categoryId != 0) {
                    productCategory = await _productService.GetProductCategoryInfo(categoryId);
                }
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, "Exception Caught");
            }

            return View(productCategory);
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> AddProductCategory([FromBody] ProductCategory productCategory) {
            try 
            {
                var result = await _productService.SaveProductCategoryInformation(productCategory);

                if (result == "1") 
                {
                    return Json(new Confirmation { msg = "Data save successfully!!", output = "Success", returnvalue = productCategory });
                } 
                else 
                {
                    return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = productCategory });
                }
            } 
            catch (Exception ex) 
            {

                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = productCategory });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> ProductCategoryList() 
        {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetProductCategoryList() {
            try 
            {
                var productCategories = await _productService.GetProductCategories();
                if (productCategories == null || productCategories.ToList().Count == 0) 
                {
                    productCategories = new List<ProductCategory>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = productCategories.ToList() });
            } 
            catch (Exception ex) 
            {
                List<ProductCategory> productCategories = new List<ProductCategory>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(productCategories);
            }
        }
    }
}
