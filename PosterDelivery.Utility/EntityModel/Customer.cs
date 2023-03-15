using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string? AccountName { get; set; }
        public string? ShippingState { get; set; }
        public string? ShippingStreet { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingCode { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? AlternateContact { get; set; }
        public string? ConsignmentOrBuyer { get; set; }
        public int DeliveryDay { get; set; }
        public int TotalBoxes { get; set; }
        public string? DeliveryDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? IsActive { get; set; }
        public string? Notes { get; set; }
        public string? Email { get; set; }
        public string? LastVisitedDateString { get; set; }   
        public int InvoiceHeaderId { get; set; }
        public bool? IsInvoice { get; set; }

        public static explicit operator Customer(Task<Customer> v) {
            throw new NotImplementedException();
        }
        public string? UserName { get; set; }
    }
}
