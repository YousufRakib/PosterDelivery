using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel {
    public class CaptureStoreInvoiceInputModel {
        public int HeaderId { get; set; }
        public int userId { get; set; }
        public int customerId { get; set; }

        public int DriverCustomerTrackId { get; set; }
        public double InvoiceAmount { get; set; }
        public string? InvoiceSerialNum { get; set; }
        public double ActualInvoiceAmt { get; set; }
        public double TotalInvoiceAmt { get; set; }
        public string? InvoiceFileName { get; set; }
        public string? InvoiceFilePath { get; set; }
        public int DeliveryCount { get; set; }
        public int PickupCount { get; set; }
        public int SoldQuantity { get; set; }
    }
}
