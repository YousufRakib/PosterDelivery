using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel {
    public class ProductsModel {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int DriverCustomerTrackId { get; set; }
        public IList<ProductDetails>? lstProductDetils { get; set; }
    }
    public class ProductDetails {
        public int ProductId { get; set; }
        public string? ProductSerial { get; set; }
        public string? ProductName { get; set; }
        public double? ProductPrice { get; set; }
        public int? IsActive { get; set; }
    }
}
