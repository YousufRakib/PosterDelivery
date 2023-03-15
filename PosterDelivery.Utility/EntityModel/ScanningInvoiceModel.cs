using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel {
    public class ScanningInvoiceModel {
        public int HeaderId { get; set; }
        public int InitialRefHederId { get; set; }
        public int ProductId { get; set; }
        public string? ProductSerial { get; set; }
        public string? ProductName { get; set; }
        public double? ProductPrice { get; set; }
        public int SoldQuantity { get; set; }
    }
}
