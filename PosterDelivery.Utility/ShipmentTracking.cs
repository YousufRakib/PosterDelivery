using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility {
    public class ShipmentTracking 
    {
        public int CustomerId { get; set; }
        public int DriverCustomerTrackId { get; set; }
        public int ActualProductsShipped { get; set; }
        public string? AccountName { get; set; }
        public string? ShippingStreet { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }
        public string? ShippingCode { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ConsignmentOrBuyer { get; set; }
        public int DeliveryDay { get; set; }
        public int NumOfPosters { get; set; }
        public string? DeliveryDateString { get; set; }
        public string? ActualDateString { get; set; }
        public string? IsActive { get; set; }
        public string? ShipmentStatus { get; set; }
        public string? ProposedDate { get; set; }
        public string? UserName { get; set; }
        public int DriverId { get; set; }
        public string? UserDriver { get; set; }
        public int UserDriverId { get; set; }
        public string? DriverVisitDateString { get; set; }
        public int? ScannedStore { get; set; }
        public int? IsInvoiceGenerated { get; set; }
        public int? IsGreen { get; set; }
    }
}
