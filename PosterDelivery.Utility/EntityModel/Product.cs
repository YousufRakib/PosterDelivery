using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel {
    public class Product 
    {
        public int ProductId { get; set; }
        public string? ProductSerial { get; set; }
        public string? ProductName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double? ProductPrice { get; set; }
        public int? IsBarcodeGenerated { get; set; }
        public string? BarcodeImageName { get; set; }
        public string? BarcodeImagePath { get; set; }
        public string? ProductImageName { get; set; }
        public string? ProductImagePath { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string? DateCreatedString { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string? IsActive { get; set; }
        public List<ProductCategory>? productCategories { get; set; }

        public string QrCodeInlineImage { get; set; }

    }
}
