using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel {
    public class PickupScanningModel {
        public int customerId { get; set; }
        public int DriverCustomerTrackId { get; set; }
        public int ScannedBy { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
